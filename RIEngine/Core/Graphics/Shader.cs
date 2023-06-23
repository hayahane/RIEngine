using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;

namespace RIEngine.Core.Graphics
{
    public sealed class Shader : IDisposable
    {
        public readonly int Handle;
        private bool _isDisposing;

        private readonly Dictionary<string, int> _uniformLocations;

        public Shader(string vertexPath, string fragmentPath)
        {
            string vertexShaderSource = File.ReadAllText(vertexPath);
            string fragmentShaderSource = File.ReadAllText(fragmentPath);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            
            Shader.Compile(vertexShader);
            Shader.Compile(fragmentShader);
            
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle,fragmentShader);
            
            GL.LinkProgram(Handle);
            
            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(fragmentShader);
            
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var uniformCount);

            _uniformLocations = new Dictionary<string, int>();
            for (var i = 0; i < uniformCount; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out _);
                var location = GL.GetUniformLocation(Handle, key);
                _uniformLocations.Add(key, location);
            }
        }

        public static void Compile(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int isSucceeded);
            if (isSucceeded == 0)
            {
                string logInfo = GL.GetShaderInfoLog(shader);
                Console.WriteLine(logInfo);
            }
        }

        public int GetAttribLocation(string name)
        {
            return GL.GetAttribLocation(Handle, name);
        }
        
        public void SetInt(string name, int property)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], property);
        }

        public void SetFloat(string name, float property)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], property);
        }
        
        public void SetMat4(string name, Matrix4 property)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(_uniformLocations[name], false, ref property);
        }
        
        public void SetVec3(string name, Vector3 property)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(_uniformLocations[name], property);
        }
        
        
        ~Shader()
        {
            if (_isDisposing == false)
            {
                Console.WriteLine("GPU resource leak! Call Dispose() on this object to avoid this message.");
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                GL.DeleteProgram(Handle);
                _isDisposing = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public void Use()
        {
            GL.UseProgram(Handle);
        }
    }
}