using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace RIEngine.Graphics
{
    public class Mesh
    {
        [JsonIgnore]
        public const int BufferStride = 8;
        
        /// <summary>
        /// Vertex BuffetData ready to be sent to the GPU
        /// </summary>
        public Vertex[] VertexBuffer { get; set; }

        [JsonIgnore]
        public int VertexCount => VertexBuffer.Length / BufferStride;

        public uint[] Indices { get; set; }
        
        [JsonIgnore]
        public int TriangleCount => Indices.Length / 3;

        public static Mesh LoadFromFile(string path)
        {
            string jsonData = File.ReadAllText(path);
            var mesh = JsonConvert.DeserializeObject<Mesh>(jsonData);
            if (mesh == null) throw new Exception("Failed to load mesh from file");
            return mesh;
        }
        
        public Mesh()
        {
            VertexBuffer = Array.Empty<Vertex>();
            Indices = Array.Empty<uint>();
        }
    }
}