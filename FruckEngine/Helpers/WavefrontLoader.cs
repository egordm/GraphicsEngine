using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;

namespace FruckEngine.Helpers
{
    public struct WavefrontVertex
    {
        public Vector3 Point;
        public Vector2 TexCoord;
        public Vector3 Normal;
    }

    public class WavefrontMesh
    {
        public WavefrontVertex[] Vertices;
        public Triangle[] Triangles;
        public Quad[] Quads;

        public WavefrontMesh(WavefrontVertex[] vertices, Triangle[] triangles, Quad[] quads)
        {
            Vertices = vertices;
            Triangles = triangles;
            Quads = quads;
        }
    }

    public class Triangle
    {
        public int v1, v2, v3;

        public Triangle(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    public class Quad
    {
        public int v1, v2, v3, v4;

        public Quad(int v1, int v2, int v3, int v4)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.v4 = v4;
        }
    }

    public class WavefrontLoader
    {
        private List<Vector3> vertexCoords = new List<Vector3>();
        private List<Vector2> texCoords = new List<Vector2>();
        private List<Vector3> normals = new List<Vector3>();
        private List<WavefrontVertex> vertices = new List<WavefrontVertex>();
        private List<Triangle> triangles = new List<Triangle>();
        private List<Quad> quads = new List<Quad>();

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

        private int ParseVertex(string data)
        {
            var subtokens = data.Split('/');
            var vertex = new WavefrontVertex();

            vertex.Point = vertexCoords[int.Parse(subtokens[0])];
            if (subtokens.Length > 1 && !string.IsNullOrEmpty(subtokens[1])) {
                vertex.TexCoord = texCoords[int.Parse(subtokens[1])];
            }

            if (subtokens.Length > 2 && !string.IsNullOrEmpty(subtokens[2])) {
                vertex.Normal = normals[int.Parse(subtokens[2])];
            }
            
            vertices.Add(vertex);
            return vertices.Count - 1;
        }


        /// <summary>
        /// Parses a Wavefront file
        /// https://en.wikipedia.org/wiki/Wavefront_.obj_file
        /// </summary>
        /// <param name="path"></param>
        public WavefrontMesh LoadMesh(string path)
        {
            var file = new StreamReader(path);
            string line;

            while ((line = file.ReadLine()) != null) {
                if (line.Length == 0 || line[0] == '#') continue;
                var tokens = line.Split();
                switch (tokens[0]) {
                    case "v":
                        vertexCoords.Add(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]),
                            float.Parse(tokens[3])));
                        break;
                    case "vt":
                        texCoords.Add(new Vector2(float.Parse(tokens[1]), float.Parse(tokens[2])));
                        break;
                    case "vn":
                        normals.Add(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]),
                            float.Parse(tokens[3])));
                        break;
                    case "f":
                        switch (tokens.Length) {
                            case 4:
                                triangles.Add(new Triangle(ParseVertex(tokens[1]), ParseVertex(tokens[2]),
                                    ParseVertex(tokens[3])));
                                break;
                            case 5:
                                quads.Add(new Quad(ParseVertex(tokens[1]), ParseVertex(tokens[2]), ParseVertex(tokens[3]),
                                    ParseVertex(tokens[4])));
                                break;
                        }

                        break;
                    default:
                        Console.WriteLine($"Parsing file {path}. Skipping unexpected line {line}");
                        break;
                }
            }

            return new WavefrontMesh(vertices.ToArray(), triangles.ToArray(), quads.ToArray());
        }
    }
}