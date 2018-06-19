using System;
using System.Collections.Generic;
using System.IO;
using Assimp;
using FruckEngine.Graphics;
using FruckEngine.Structs;
using OpenTK;
using Material = FruckEngine.Structs.Material;
using Mesh = FruckEngine.Graphics.Mesh;
using Object = FruckEngine.Objects.Object;

namespace FruckEngine.Helpers {
    public class AssimpLoadHelper {
        private string Directory;
        private Dictionary<string, Texture> TextureCache= new Dictionary<string, Texture>();
        private List<Mesh> Meshes = new List<Mesh>();
        private bool FlipU = false;
        private bool PBR = true;

        private static AssimpLoadHelper Instance = new AssimpLoadHelper();

        private Object Load(string path, bool PBR = true, bool flipU = false) {
            Meshes.Clear();
            FlipU = flipU;
            this.PBR = PBR;
            var importer = new AssimpContext();
            var scene = importer.ImportFile(path,
                PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);

            if ((int) (scene.SceneFlags & SceneFlags.Incomplete) == 1) throw new Exception("ERROR::ASSIMP");

            Directory = Path.GetDirectoryName(path) + "/";

            ProcessNode(scene, scene.RootNode);

            return new Object(Meshes);
        }

        public static Object LoadModel(string path, bool PBR = true, bool flipU = false) {
            return Instance.Load(path, PBR, flipU);
        }

        private void ProcessNode(Scene scene, Node node) {
            foreach (var meshIndex in node.MeshIndices) {
                Meshes.Add(LoadMesh(scene, scene.Meshes[meshIndex]));
            }

            foreach (var child in node.Children) {
                ProcessNode(scene, child);
            }
        }

        private Mesh LoadMesh(Scene scene, Assimp.Mesh mesh) {
            var vertices = new List<Vertex>();
            vertices.Capacity = mesh.VertexCount;
            var indices = new List<uint>();

            // Load vertices
            bool loadTexCoords = mesh.HasTextureCoords(0);
            for (int i = 0; i < mesh.VertexCount; i++) {
                var vertex = new Vertex();
                vertex.Position = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
                vertex.Normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);
                if (loadTexCoords) {
                    var texCrd = mesh.TextureCoordinateChannels[0][i];
                    vertex.UV = new Vector2(texCrd.X, texCrd.Y);
                }

                vertex.Tangent = new Vector3(mesh.Tangents[i].X, mesh.Tangents[i].Y, mesh.Tangents[i].Z);
                vertex.Bitangent = new Vector3(mesh.BiTangents[i].X, mesh.BiTangents[i].Y, mesh.BiTangents[i].Z);

                vertices.Add(vertex);
            }

            // Load Faces
            foreach (var face in mesh.Faces) {
                foreach (var faceIndex in face.Indices) indices.Add((uint) faceIndex);
            }

            var material = LoadMaterial(scene.Materials[mesh.MaterialIndex]);

            return new Mesh(vertices.ToArray(), indices.ToArray(), material);
        }

        private Material LoadMaterial(Assimp.Material material) {
            Material ret;

            if (PBR) {
                ret = new PBRMaterial();
                ((PBRMaterial) ret).Albedo = ConvertColor(material.ColorDiffuse);
                ((PBRMaterial) ret).Metallic = material.ColorSpecular.R;
                ((PBRMaterial) ret).Roughness = material.Shininess;
            } else {
                ret = new LegacyMaterial();
                ((LegacyMaterial) ret).Diffuse = ConvertColor(material.ColorDiffuse);
                ((LegacyMaterial) ret).Specular = ConvertColor(material.ColorSpecular);
                ((LegacyMaterial) ret).Shinyness = material.Shininess;
            }

            // TODO: support multiple textures. But usesless until shaders do too
            if (material.HasTextureDiffuse) ret.Textures.Add(LoadTexture(material.TextureDiffuse, TextureType.Diffuse));
            if (material.HasTextureSpecular)
                ret.Textures.Add(LoadTexture(material.TextureSpecular, TextureType.Specular));
            if (material.HasTextureHeight) ret.Textures.Add(LoadTexture(material.TextureHeight, TextureType.Height));
            if (material.HasTextureAmbient) ret.Textures.Add(LoadTexture(material.TextureAmbient, TextureType.Ambient));

            return ret;
        }

        private Texture LoadTexture(TextureSlot textureSlot, TextureType type) {
            var shadeType = ShadeType.TEXTURE_TYPE_DIFFUSE;
            switch (type) {
                case TextureType.Diffuse:
                    shadeType = (PBR) ? ShadeType.TEXTURE_TYPE_ALBEDO : ShadeType.TEXTURE_TYPE_DIFFUSE;
                    break;
                case TextureType.Specular:
                    shadeType = (PBR) ? ShadeType.TEXTURE_TYPE_METALLIC : ShadeType.TEXTURE_TYPE_SPECULAR;
                    break;
                case TextureType.Height:
                    shadeType = ShadeType.TEXTURE_TYPE_NORMAL;
                    break;
                case TextureType.Ambient:
                    shadeType = (PBR) ? ShadeType.TEXTURE_TYPE_AO : ShadeType.TEXTURE_TYPE_AMBIENT;
                    break;
            }

            string path = Directory + textureSlot.FilePath;
            path = path.Replace("\\", "/");

            if (TextureCache.ContainsKey(path)) {
                var tmp = (Texture) TextureCache[path].Clone();
                tmp.ShadeType = shadeType;
                return tmp;
            }

            var ret = TextureHelper.LoadFromImage(path);
            ret.ShadeType = shadeType;
            TextureCache.Add(path, ret);

            return ret;
        }

        private Vector3 ConvertColor(Color4D color) {
            return new Vector3(color.R, color.G, color.B);
        }
    }
}