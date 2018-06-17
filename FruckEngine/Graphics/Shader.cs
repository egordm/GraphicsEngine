using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics {
    public class Shader {
        protected int Pointer = Constants.UNCONSTRUCTED;
        protected Dictionary<string, int> AttribPointers = new Dictionary<string, int>();
        protected bool Linked;

        public void Link(List<int> shaders) {
            Pointer = GL.CreateProgram();
            for (int i = 0; i < shaders.Count; ++i) {
                GL.AttachShader(Pointer, shaders[i]);
            }

            GL.LinkProgram(Pointer);

            int status;
            GL.GetProgram(Pointer, ProgramParameter.LinkStatus, out status);
            if (status == Constants.GL_FAILURE) {
                throw new GraphicsException($"Error linking program. Error: \n{GL.GetProgramInfoLog(Pointer)}");
            }
            
            foreach (var t in shaders) GL.DeleteShader(t);

            Linked = true;
        }

        public void Use() {
            GL.UseProgram(Pointer);
        }

        public void UnUse() {
            GL.UseProgram(0);
        }

        int AddAttribVar(string name) {
            if (!AttribPointers.ContainsKey(name)) {
                AttribPointers.Add(name, GL.GetAttribLocation(Pointer, name));
            }
            return AttribPointers[name];
        }
        
        int AddUniformVar(string name) {
            if (!AttribPointers.ContainsKey(name)) {
                AttribPointers.Add(name, GL.GetUniformLocation(Pointer, name));
            }
            return AttribPointers[name];
        }

        int GetVar(string name) {
            if (!AttribPointers.ContainsKey(name)) return -1;
            return AttribPointers[name];
        }
        
        void setBool(string name, bool value) {
            GL.Uniform1(GetVar(name), value ? 1 : 0);
        }
        
        void setInt(string name, int value) {
            GL.Uniform1(GetVar(name), value);
        }
        
        void setFloat(string name, float value) {
            GL.Uniform1(GetVar(name), value);
        }
        
        void setVec2(string name, Vector2 value) {
            GL.Uniform2(GetVar(name), value);
        }
        
        void setVec2(string name, float x, float y) {
            GL.Uniform2(GetVar(name), x, y);
        }
        
        void setVec3(string name, Vector3 value) {
            GL.Uniform3(GetVar(name), value);
        }
        void setVec3(string name, float x, float y, float z) {
            GL.Uniform3(GetVar(name), x, y, z);
        }
        
        void setVec4(string name, Vector4 value) {
            GL.Uniform4(GetVar(name), value);
        }
        void setVec4(string name, float x, float y, float z, float w) {
            GL.Uniform4(GetVar(name), x, y, z, w);
        }
        
        void setMat2(string name, Matrix2 mat) {
            GL.UniformMatrix2(GetVar(name), false, ref mat);
        }
        
        void setMat3(string name, Matrix3 mat) {
            GL.UniformMatrix3(GetVar(name), false, ref mat);
        }
        
        void setMat4(string name, Matrix4 mat) {
            GL.UniformMatrix4(GetVar(name), false, ref mat);
        }

        public static Shader Create(string vs_path, string fs_path) {
            var ret = new Shader();
            var shaders = new List<int> {
                LoadShader(vs_path, ShaderType.VertexShader),
                LoadShader(fs_path, ShaderType.FragmentShader)
            };
            ret.Link(shaders);
            return ret;
        }


        private static int LoadShader(string path, ShaderType type) {
            int pointer = GL.CreateShader(type);
            using (var sr = new StreamReader(path)) GL.ShaderSource(pointer, sr.ReadToEnd());
            GL.CompileShader(pointer);

            int status;
            GL.GetShader(pointer, ShaderParameter.CompileStatus, out status);
            if (status == Constants.GL_FAILURE) {
                throw new GraphicsException(
                    $"Error compiling shader ({type.ToString()}). Error: \n{GL.GetShaderInfoLog(pointer)}");
            }

            return pointer;
        }
    }
}