using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;

namespace FruckEngine.Helpers
{
    public static class Wavefront
    {
        public struct Face
        {
            public int[] VertexIndices;
            public int[] TextureCoordIndices;
            public int[] NormalIndices;

            public Face(string[] tokens, bool useTexCoords, bool useNormals)
            {
                VertexIndices = new int[tokens.Length - 1];
                TextureCoordIndices = useTexCoords ? new int[tokens.Length - 1] : null;
                NormalIndices = useNormals ? new int[tokens.Length - 1] : null;

                for (int i = 0; i < tokens.Length - 1; ++i) {
                    var subtokens = tokens[i].Split();
                    VertexIndices[i] = int.Parse(subtokens[0]);
                    if (useTexCoords && !string.IsNullOrEmpty(subtokens[1]))
                        TextureCoordIndices[i] = int.Parse(subtokens[1]);
                    if (useNormals && !string.IsNullOrEmpty(subtokens[2])) 
                        NormalIndices[i] = int.Parse(subtokens[2]);
                }
            }
        }

        public class Model
        {
            public List<Vector3> Vertices;
            public List<Vector2> TextureCoordinates;
            public List<Vector3> VertexNormals;
            public List<Face> Faces;

            public Model()
            {
                Vertices = new List<Vector3>();
                TextureCoordinates = new List<Vector2>();
                VertexNormals = new List<Vector3>();
                Faces = new List<Face>();
            }

            public bool hasTextureCoords()
            {
                return TextureCoordinates.Count > 0;
            }

            public bool hasNormals()
            {
                return VertexNormals.Count > 0;
            }
        }


        /// <summary>
        /// Parses a Wavefront file
        /// https://en.wikipedia.org/wiki/Wavefront_.obj_file
        /// </summary>
        /// <param name="path"></param>
        public static Model LoadModel(string path)
        {
            var file = new StreamReader(path);
            string line;
            var ret = new Model();
            while ((line = file.ReadLine()) != null) {
                if (line.Length == 0 || line[0] == '#') continue;
                var tokens = line.Split();
                switch (tokens[0]) {
                    case "v":
                        ret.Vertices.Add(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]),
                            float.Parse(tokens[3])));
                        break;
                    case "vt":
                        ret.TextureCoordinates.Add(new Vector2(float.Parse(tokens[1]), float.Parse(tokens[2])));
                        break;
                    case "vn":
                        ret.VertexNormals.Add(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]),
                            float.Parse(tokens[3])));
                        break;
                    case "f":
                        ret.Faces.Add(new Face(tokens, ret.hasTextureCoords(), ret.hasNormals()));
                        break;
                    default:
                        Console.WriteLine($"Parsing file {path}. Skipping unexpected line {line}");
                        break;
                }
            }
            return ret;
        }
    }
}