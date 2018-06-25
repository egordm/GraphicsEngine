using System;
using System.Collections.Generic;
using System.IO;
using Assimp;
using FruckEngine.Graphics;
using FruckEngine.Structs;
using FruckEngine.Utils;
using OpenTK;
using Material = FruckEngine.Structs.Material;
using Mesh = FruckEngine.Graphics.Mesh;
using Object = FruckEngine.Objects.Object;

namespace FruckEngine.Helpers {
    /// <summary>
    /// To load the models we are using a third party library called assimp. Loading models accoring to the standard
    /// specified in <b>Wikipedia</b> produces gaps in some models. Therefore we use assimp which also calculates tangents
    /// and allows loading fbx and other model types.
    /// </summary>
    public class AssimpLoadHelper {
        private string Directory;
        /// <summary>
        /// Cache for already used textures since they are often reused by different meshes in file
        /// </summary>
        private Dictionary<string, Texture> TextureCache = new Dictionary<string, Texture>();
        /// <summary>
        /// List of loaded meshes that are in the file
        /// </summary>
        private List<Mesh> Meshes = new List<Mesh>();
        /// <summary>
        /// Wether U texture coordinates need to be flipped
        /// </summary>
        private bool FlipU = false;
        /// <summary>
        /// Wether the materials should be loaded as pbr materials
        /// </summary>
        private bool PBR = true;

        /// <summary>
        /// Singleton 
        /// </summary>
        private static AssimpLoadHelper Instance = new AssimpLoadHelper();

        /// <summary>
        /// Loads meshes from given file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="PBR"></param>
        /// <param name="flipU"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Object Load(string path, bool PBR = true, bool flipU = false) {
            Meshes = new List<Mesh>();
            FlipU = flipU;
            this.PBR = PBR;
            var importer = new AssimpContext();
            // Import file. Also make convert all face to triangle, flip the UVs, calculate tangents and calculate nromals
            // if those are not provided. 
            // EZ life right?
            var scene = importer.ImportFile(path,
                PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace |
                PostProcessSteps.GenerateSmoothNormals);

            if ((int) (scene.SceneFlags & SceneFlags.Incomplete) == 1) throw new Exception("ERROR::ASSIMP");

            // Set directory to textures will be loaded relative to that
            Directory = Path.GetDirectoryName(path) + "/";

            ProcessNode(scene, scene.RootNode);

            return new Object(Meshes);
        }

        /// <summary>
        /// Static function Load the meshes from given file 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="PBR"></param>
        /// <param name="flipU"></param>
        /// <returns></returns>
        public static Object LoadModel(string path, bool PBR = true, bool flipU = false) {
            return Instance.Load(path, PBR, flipU);
        }

        /// <summary>
        /// Recursive function to process all meshes in the assimp scene
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="node"></param>
        private void ProcessNode(Scene scene, Node node) {
            foreach (var meshIndex in node.MeshIndices) {
                Meshes.Add(LoadMesh(scene, scene.Meshes[meshIndex]));
            }

            foreach (var child in node.Children) {
                ProcessNode(scene, child);
            }
        }

        /// <summary>
        /// Loads a mesh from assimp scene
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
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

                if (mesh.Tangents.Count > 0) {
                    vertex.Tangent = new Vector3(mesh.Tangents[i].X, mesh.Tangents[i].Y, mesh.Tangents[i].Z);
                    vertex.Bitangent = new Vector3(mesh.BiTangents[i].X, mesh.BiTangents[i].Y, mesh.BiTangents[i].Z);
                } else {
                    vertex.Tangent = MathFuncs.CalculateTangent(vertex.Normal);
                    vertex.Bitangent = Vector3.Cross(vertex.Tangent, vertex.Normal);
                }

                vertices.Add(vertex);
            }

            // Load Faces
            foreach (var face in mesh.Faces) {
                foreach (var faceIndex in face.Indices) indices.Add((uint) faceIndex);
            }

            // Load material
            var material = LoadMaterial(scene.Materials[mesh.MaterialIndex]);

            return new Mesh(vertices.ToArray(), indices.ToArray(), material);
        }

        /// <summary>
        /// Load material form a assimp material
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        private Material LoadMaterial(Assimp.Material material) {
            Material ret;

            // If material is pbr the pbr values are stored in the legacy variables
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
            
            // Copy the material name for indentification
            ret.Name = material.Name;

            // Load the textures and add their names to tags of the material
            // TODO: support multiple textures. But usesless until shaders do too
            if (material.HasTextureDiffuse) {
                ret.Tags.Add(Path.GetFileNameWithoutExtension(material.TextureDiffuse.FilePath));
                ret.Textures.Add(LoadTexture(material.TextureDiffuse, TextureType.Diffuse));
            }

            if (material.HasTextureSpecular)
                ret.Textures.Add(LoadTexture(material.TextureSpecular, TextureType.Specular));
            if (material.HasTextureHeight) ret.Textures.Add(LoadTexture(material.TextureHeight, TextureType.Height));
            if (material.HasTextureAmbient) ret.Textures.Add(LoadTexture(material.TextureAmbient, TextureType.Ambient));

            return ret;
        }

        /// <summary>
        /// Load the texture from a assimpt texture wrapper
        /// </summary>
        /// <param name="textureSlot"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Texture LoadTexture(TextureSlot textureSlot, TextureType type) {
            // Match the type
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

            // Get full path
            string path = Directory + textureSlot.FilePath;
            path = path.Replace("\\", "/");

            // Check if texture is already loaded. If yes just copy
            if (TextureCache.ContainsKey(path)) {
                var tmp = (Texture) TextureCache[path].Clone();
                tmp.ShadeType = shadeType;
                return tmp;
            }

            // Otherwise load and save to cache
            var ret = TextureHelper.LoadFromImage(path);
            ret.ShadeType = shadeType;
            TextureCache.Add(path, ret);

            return ret;
        }

        /// <summary>
        /// Convert assimp color to vector3
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private Vector3 ConvertColor(Color4D color) {
            return new Vector3(color.R, color.G, color.B);
        }
    }
}