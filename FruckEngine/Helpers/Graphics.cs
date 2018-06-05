using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Helpers
{
    public static class Graphics
    {
        /// <summary>
        /// Load a shader from file. Shader identifier is returned
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="program"></param>
        /// <param name="id"></param>
        public static void LoadShader(string name, ShaderType type, int program, out int id)
        {
            id = GL.CreateShader(type);
            using (var sr = new StreamReader(name)) GL.ShaderSource(id, sr.ReadToEnd());
            GL.CompileShader(id);
            GL.AttachShader(program, id);

            // Check if compiled
            int status;
            GL.GetShader(id, ShaderParameter.CompileStatus, out status);
            if (status == 0)
                throw new GraphicsException(String.Format("Error compiling {0} shader: {1}", type.ToString(),
                    GL.GetShaderInfoLog(id)));
        }
    }
}