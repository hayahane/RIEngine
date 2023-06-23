using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace RIEngine.Core.Graphics
{
    public class Mesh
    {
        /// <summary>
        /// Vertex BuffetData ready to be sent to the GPU
        /// </summary>
        public float[] VertexBuffer { get; set; }
        public const int BufferStride = 12;
        public int VertexBufferCount => VertexBuffer.Length;

        [JsonIgnore]
        public int VertexCount => VertexBuffer.Length / BufferStride;
        
        [JsonIgnore]
        public Vector3[] Vertices
        {
            get
            {
                var vertices = new Vector3[VertexCount];

                for (int i = 0; i < VertexCount; i++)
                {
                    vertices[i] = new Vector3(VertexBuffer[i * BufferStride], VertexBuffer[i * BufferStride + 1],
                        VertexBuffer[i * BufferStride + 2]);
                }

                return vertices;
            }
            set
            {
                if (VertexBuffer.Length < value.Length * BufferStride)
                {
                    var newBuffer = new float[value.Length * BufferStride];
                    VertexBuffer.CopyTo(newBuffer, 0);
                    VertexBuffer = newBuffer;
                }
                
                for (int i = 0; i < value.Length; i++)
                {
                    VertexBuffer[i * BufferStride] = value[i].X;
                    VertexBuffer[i * BufferStride + 1] = value[i].Y;
                    VertexBuffer[i * BufferStride + 2] = value[i].Z;
                }
            }
        }

        [JsonIgnore]
        public Color4[] Colors
        {
            get
            {
                var colors = new Color4[VertexCount];
                
                for (int i = 0; i < VertexCount; i++)
                {
                    colors[i] = new Color4(VertexBuffer[i * BufferStride + 3], VertexBuffer[i * BufferStride + 4],
                        VertexBuffer[i * BufferStride + 5], VertexBuffer[i * BufferStride + 6]);
                }

                return colors;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    VertexBuffer[i * BufferStride + 3] = value[i].R;
                    VertexBuffer[i * BufferStride + 4] = value[i].G;
                    VertexBuffer[i * BufferStride + 5] = value[i].B;
                    VertexBuffer[i * BufferStride + 6] = value[i].A;
                }
            }
        }

        [JsonIgnore]
        public Vector2[] UV
        {
            get
            {
                var uvResult = new Vector2[VertexCount];
                for (int i = 0; i < VertexCount; i++)
                {
                    uvResult[i].X = VertexBuffer[i * BufferStride + 10];
                    uvResult[i].Y = VertexBuffer[i * BufferStride + 11];
                }

                return uvResult;
            }
            
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    VertexBuffer[i * BufferStride + 10] = value[i].X;
                    VertexBuffer[i * BufferStride + 11] = value[i].Y;
                }
            }
        }

        public uint[] Indices { get; set; }
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
            VertexBuffer = Array.Empty<float>();
            Indices = Array.Empty<uint>();
        }
        
        
    }
}