using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics
{
     /// <summary>
    /// A warapper around a standard GL shader program which can handle loading shaders, compiling and setting
    /// attributes or uniforms
    /// </summary>
    public class Shader
    {
        private int prID, vsID, fsID;
        private Dictionary<string, int> attributes;

        public Shader(string vs, string fs)
        {
            prID = GL.CreateProgram();
            if(vs != null) Helpers.Graphics.LoadShader(vs, ShaderType.VertexShader, prID, out vsID);
            if(fs != null) Helpers.Graphics.LoadShader(fs, ShaderType.FragmentShader, prID, out fsID);
            GL.LinkProgram(prID);
            attributes = new Dictionary<string, int>();
        }

        public int AddAttributeVar(string name)
        {
            if (attributes.ContainsKey(name)) return -1;
            int id = GL.GetAttribLocation(prID, name);
            attributes.Add(name, id);
            return id;
        }

        public int AddUniformVar(string name)
        {
            if (attributes.ContainsKey(name)) return -1;
            int id = GL.GetUniformLocation(prID, name);
            attributes.Add(name, id);
            return id;
        }

        public int GetVar(string name)
        {
            if (!attributes.ContainsKey(name)) return -1;
            return attributes[name];
        }    

        public bool SetVar(string name, int v)
        {
            int id = GetVar(name);
            if (id == -1) return false;
            GL.Uniform1(id, v);
            return true;
        }
        
        public bool SetVar(string name, float v)
        {
            int id = GetVar(name);
            if (id == -1) return false;
            GL.Uniform1(id, v);
            return true;
        }

        public bool SetVar(string name, Vector3 v)
        {
            int id = GetVar(name);
            if (id == -1) return false;
            GL.Uniform3(id, v);
            return true;
        }

        public bool SetVar(string name, ref Matrix4 v)
        {
            int id = GetVar(name);
            if (id == -1) return false;
            GL.UniformMatrix4(id, false, ref v);
            return true;
        }

        public bool SetVar(string name, Vector2 v)
        {
            int id = GetVar(name);
            if (id == -1) return false;
            GL.Uniform2(id, v);
            return true;
        }

        public void Use()
        {
            GL.UseProgram(prID);
        }

        public void UnUse()
        {
            GL.UseProgram(0);
        }
    }
}