using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics {
    /// <summary>
    /// Abstraction for shader program. 
    /// </summary>
    public class Shader {
        /// <summary>
        /// Pointer to shader program aka shader program id
        /// </summary>
        protected int Pointer = Constants.UNCONSTRUCTED;
        
        /// <summary>
        /// List with all registered uniform and attributes
        /// </summary>
        protected Dictionary<string, int> AttribPointers = new Dictionary<string, int>();

        /// <summary>
        /// Links the shader proram with given shader ids
        /// </summary>
        /// <param name="shaders"></param>
        /// <exception cref="GraphicsException"></exception>
        public void Link(List<int> shaders) {
            // Attach shaders and link
            Pointer = GL.CreateProgram();
            for (int i = 0; i < shaders.Count; ++i) GL.AttachShader(Pointer, shaders[i]);
            GL.LinkProgram(Pointer);

            // Assert link status
            int status;
            GL.GetProgram(Pointer, ProgramParameter.LinkStatus, out status);
            if (status == Constants.GL_FAILURE) {
                throw new GraphicsException($"Error linking program. Error: \n{GL.GetProgramInfoLog(Pointer)}");
            }
            
            // Delete shader object since we already linked it.
            foreach (var t in shaders) GL.DeleteShader(t);
        }

        /// <summary>
        /// Use program
        /// </summary>
        public void Use() {
            GL.UseProgram(Pointer);
        }

        /// <summary>
        /// Unuse program
        /// </summary>
        public void UnUse() {
            GL.UseProgram(0);
        }

        /// <summary>
        /// Register attribute
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int AddAttribVar(string name) {
            if (!AttribPointers.ContainsKey(name)) AttribPointers.Add(name, GL.GetAttribLocation(Pointer, name));
            return AttribPointers[name];
        }
        
        /// <summary>
        /// Register uniform
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int AddUniformVar(string name) {
            if (!AttribPointers.ContainsKey(name)) AttribPointers.Add(name, GL.GetUniformLocation(Pointer, name));
            return AttribPointers[name];
        }

        /// <summary>
        /// Check wether a uniform or attribute is registered
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasVar(string name) {
            return AttribPointers.ContainsKey(name) && AttribPointers[name] != -1;
        }

        /// <summary>
        /// Return uniform or attribute id if it is registered.
        /// If not returns -1
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetVar(string name) {
            if (!AttribPointers.ContainsKey(name)) {
                Console.WriteLine($"Warining: Accessing not existing property: {name}");
                return -1;
            }
            return AttribPointers[name];
        }
        
        public void SetBool(string name, bool value) {
            GL.Uniform1(GetVar(name), value ? 1 : 0);
        }
        
        public void SetInt(string name, int value) {
            GL.Uniform1(GetVar(name), value);
        }
        
        public void SetFloat(string name, float value) {
            GL.Uniform1(GetVar(name), value);
        }
        
        public void SetVec2(string name, Vector2 value) {
            GL.Uniform2(GetVar(name), value);
        }
        
        public void SetVec2(string name, float x, float y) {
            GL.Uniform2(GetVar(name), x, y);
        }
        
        public void SetVec3(string name, Vector3 value) {
            GL.Uniform3(GetVar(name), value);
        }
        public void SetVec3(string name, float x, float y, float z) {
            GL.Uniform3(GetVar(name), x, y, z);
        }
        
        public void SetVec4(string name, Vector4 value) {
            GL.Uniform4(GetVar(name), value);
        }
        public void SetVec4(string name, float x, float y, float z, float w) {
            GL.Uniform4(GetVar(name), x, y, z, w);
        }
        
        public void SetMat2(string name, Matrix2 mat) {
            GL.UniformMatrix2(GetVar(name), false, ref mat);
        }
        
        public void SetMat3(string name, Matrix3 mat) {
            GL.UniformMatrix3(GetVar(name), false, ref mat);
        }
        
        public void SetMat4(string name, Matrix4 mat) {
            GL.UniformMatrix4(GetVar(name), false, ref mat);
        }

        /// <summary>
        /// Create a vanilla shader which has a vertex shader and fragment shader.
        /// Good for most shader programs
        /// </summary>
        /// <param name="vs_path"></param>
        /// <param name="fs_path"></param>
        /// <returns></returns>
        public static Shader Create(string vs_path, string fs_path) {
            var ret = new Shader();
            var shaders = new List<int> {
                LoadShader(vs_path, ShaderType.VertexShader),
                LoadShader(fs_path, ShaderType.FragmentShader)
            };
            ret.Link(shaders); // Link programs with both shaders
            return ret;
        }

        /// <summary>
        /// Load and compile a shader of a given type from a file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="GraphicsException"></exception>
        private static int LoadShader(string path, ShaderType type) {
            // Load and compile
            int pointer = GL.CreateShader(type);
            using (var sr = new StreamReader(path)) GL.ShaderSource(pointer, sr.ReadToEnd());
            GL.CompileShader(pointer);

            // Assert status
            int status;
            GL.GetShader(pointer, ShaderParameter.CompileStatus, out status);
            if (status == Constants.GL_FAILURE) {
                Console.WriteLine( $"Error compiling shader ({type.ToString()}). Error: \n{GL.GetShaderInfoLog(pointer)}");
                throw new GraphicsException("Rip shader");
            }

            return pointer;
        }
    }
}