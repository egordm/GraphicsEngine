using System;
using System.Collections.Generic;
using FruckEngine.Graphics;
using FruckEngine.Structs;
using OpenTK;

namespace FruckEngine.Helpers {
    /// <summary>
    /// Default models / primitives
    /// </summary>
    public static class DefaultModels {
        public static Mesh GetGrassPlane(int size)
        {
            List<Vertex> vertices;
            vertices = new List<Vertex>() {
                    new Vertex(new Vector3(-1, 0, 1), new Vector2(0.0f, size), Vector3.UnitY, Vector3.UnitX,
                        Vector3.UnitZ),
                    new Vertex(new Vector3(-1, 0, -1), new Vector2(0.0f, 0.0f), Vector3.UnitY, Vector3.UnitX,
                        Vector3.UnitZ),
                    new Vertex(new Vector3(1, 0, 1), new Vector2(size, size), Vector3.UnitY, Vector3.UnitX,
                        Vector3.UnitZ),
                    new Vertex(new Vector3(1, 0, -1), new Vector2(size, 0.0f), Vector3.UnitY, Vector3.UnitX,
                        Vector3.UnitZ)
                };
            var indices = new List<uint>() { 0, 1, 2, 1, 2, 3 };
            return new Mesh(vertices.ToArray(), indices.ToArray(), new PBRMaterial());
        }

        public static Mesh GetPlane(bool vertical) {
            List<Vertex> vertices;
            if (!vertical) {
                vertices = new List<Vertex>() {
                    new Vertex(new Vector3(-1, 0, 1), new Vector2(0.0f, 1.0f), Vector3.UnitY, Vector3.UnitX,
                        Vector3.UnitZ),
                    new Vertex(new Vector3(-1, 0, -1), new Vector2(0.0f, 0.0f), Vector3.UnitY, Vector3.UnitX,
                        Vector3.UnitZ),
                    new Vertex(new Vector3(1, 0, 1), new Vector2(1.0f, 1.0f), Vector3.UnitY, Vector3.UnitX,
                        Vector3.UnitZ),
                    new Vertex(new Vector3(1, 0, -1), new Vector2(1.0f, 0.0f), Vector3.UnitY, Vector3.UnitX,
                        Vector3.UnitZ)
                };
            } else {
                vertices = new List<Vertex>() {
                    new Vertex(new Vector3(-1, 1, 0), new Vector2(0.0f, 1.0f), Vector3.UnitZ, Vector3.UnitX,
                        Vector3.UnitY),
                    new Vertex(new Vector3(-1, -1, 0), new Vector2(0.0f, 0.0f), Vector3.UnitY, Vector3.UnitX,
                        Vector3.UnitY),
                    new Vertex(new Vector3(1, 1, 0), new Vector2(1.0f, 1.0f), Vector3.UnitZ, Vector3.UnitX,
                        Vector3.UnitY),
                    new Vertex(new Vector3(1, -1, 0), new Vector2(1.0f, 0.0f), Vector3.UnitZ, Vector3.UnitX,
                        Vector3.UnitY)
                };
            }

            var indices = new List<uint>() {0, 1, 2, 1, 2, 3};
            return new Mesh(vertices.ToArray(), indices.ToArray(), new PBRMaterial());
        }

        public static Mesh GetSphere(uint xSegements = 64, uint ySegments = 64) {
            var vertices = new List<Vertex>();
            var indices = new List<uint>();

            for (int y = 0; y <= ySegments; ++y) {
                float ySegment = y / (float) ySegments;
                float sinTheta = (float) Math.Sin(ySegment * Math.PI);
                float cosTheta = (float) Math.Cos(ySegment * Math.PI);

                for (int x = 0; x <= xSegements; ++x) {
                    float xSegment = x / (float) xSegements;
                    float sinPhi = (float) Math.Sin(xSegment * 2 * Math.PI);
                    float cosPhi = (float) Math.Cos(xSegment * 2 * Math.PI);

                    var vertex = new Vertex();
                    vertex.UV = new Vector2(xSegment, ySegment);
                    vertex.Normal = new Vector3(cosPhi * sinTheta, cosTheta, sinPhi * sinTheta);
                    vertex.Position = vertex.Normal;
                    vertex.Tangent = new Vector3(-sinTheta, 0, cosTheta);
                    vertex.Bitangent = Vector3.Cross(vertex.Normal, vertex.Tangent);

                    vertices.Add(vertex);
                }
            }

            for (int y = 0; y <= ySegments; y++) {
                for (int x = 0; x <= xSegements; ++x) {
                    uint first = (uint) ((y * (xSegements + 1)) + x);
                    uint second = first + xSegements + 1;

                    indices.Add(first);
                    indices.Add(second);
                    indices.Add(first + 1);

                    indices.Add(second);
                    indices.Add(second + 1);
                    indices.Add(first + 1);
                }
            }

            return new Mesh(vertices.ToArray(), indices.ToArray(), new PBRMaterial());
        }

        public static Mesh GetCube() {
            var vertices = new List<Vertex>();
            var indices = new List<uint>();

            float[] vs = {
                // back face
                -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, // bottom-left
                1.0f, 1.0f, -1.0f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f, // top-right
                1.0f, -1.0f, -1.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f, // bottom-right
                1.0f, 1.0f, -1.0f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f, // top-right
                -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, // bottom-left
                -1.0f, 1.0f, -1.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f, // top-left
                // front face
                -1.0f, -1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, // bottom-left
                1.0f, -1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f, // bottom-right
                1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, // top-right
                1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, // top-right
                -1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, // top-left
                -1.0f, -1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, // bottom-left
                // left face
                -1.0f, 1.0f, 1.0f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f, // top-right
                -1.0f, 1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 1.0f, 1.0f, // top-left
                -1.0f, -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f, // bottom-left
                -1.0f, -1.0f, -1.0f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f, // bottom-left
                -1.0f, -1.0f, 1.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, // bottom-right
                -1.0f, 1.0f, 1.0f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f, // top-right
                // right face
                1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, // top-left
                1.0f, -1.0f, -1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f, // bottom-right
                1.0f, 1.0f, -1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, // top-right
                1.0f, -1.0f, -1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f, // bottom-right
                1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, // top-left
                1.0f, -1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, // bottom-left
                // bottom face
                -1.0f, -1.0f, -1.0f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f, // top-right
                1.0f, -1.0f, -1.0f, 0.0f, -1.0f, 0.0f, 1.0f, 1.0f, // top-left
                1.0f, -1.0f, 1.0f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f, // bottom-left
                1.0f, -1.0f, 1.0f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f, // bottom-left
                -1.0f, -1.0f, 1.0f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f, // bottom-right
                -1.0f, -1.0f, -1.0f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f, // top-right
                // top face
                -1.0f, 1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, // top-left
                1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, // bottom-right
                1.0f, 1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, // top-right
                1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, // bottom-right
                -1.0f, 1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, // top-left
                -1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f // bottom-left
            };
            
            for(uint i = 0, j = 0; i < 36*8;) {
                var vertex = new Vertex {
                    Position = new Vector3(vs[i++], vs[i++], vs[i++]),
                    Normal = new Vector3(vs[i++], vs[i++], vs[i++]),
                    UV = new Vector2(vs[i++], vs[i++])
                };
                vertices.Add(vertex);
                indices.Add(j++);
            }
            
            return new Mesh(vertices.ToArray(), indices.ToArray(), new PBRMaterial());
        }
    }
}