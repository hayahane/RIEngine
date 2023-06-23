using Newtonsoft.Json;
using OpenTK.Mathematics;
using RIEngine.Graphics;
using RIEngine.Utility.Serialization;

namespace RIEngine.Utility.AssetImporter;

public static class ObjFileImporter
{
    public static void ImportObjFile(string objFilePath, string libFilePath)
    {
        string[] lines = File.ReadAllLines(objFilePath);
        List<Vector3> positions = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> texCoord = new List<Vector2>();
        List<uint> indices = new List<uint>();
        
        List<Vertex> vertices = new List<Vertex>();
        Dictionary<string, uint> vertexMap = new Dictionary<string, uint>();
        
        uint vertexIndex = 0;

        foreach (var line in lines)
        {
            int headEnd = line.IndexOf(" ");
            string head = line.Substring(0, headEnd);
            string subLine = line.Substring(headEnd + 1);
            switch (head)
            {
                // vertex positions
                case "v":
                {
                    string[] data = subLine.Split(" ");
                    positions.Add(new Vector3(float.Parse(data[0]),
                        float.Parse(data[1]),
                        float.Parse(data[2])));
                    break;
                }
                case "vn":
                {
                    string[] data = subLine.Split(" ");
                    normals.Add(new Vector3(float.Parse(data[0]),
                        float.Parse(data[1]),
                        float.Parse(data[2])));
                    break;
                }
                // tex coordinate (uv1)
                case "vt":
                {
                    string[] data = subLine.Split(" ");
                    texCoord.Add(new Vector2(float.Parse(data[0]), float.Parse(data[1])));
                    break;
                }
                // fragment indices
                case "f":
                {
                    string[] data = subLine.Split(" ");
                    foreach (var d in data)
                    {
                        string[] indexData = d.Split('/');
                        uint vertIndex = uint.Parse(indexData[0]);
                        uint vtIndex = uint.Parse(indexData[1]);
                        uint vnIndex = uint.Parse(indexData[2]);
                        
                        if (vertexMap.TryGetValue(d, out var index))
                        {
                            indices.Add(index);
                        }
                        else
                        {
                            vertices.Add(new Vertex(positions[(int)(vertIndex - 1)],
                                                        normals[(int)(vnIndex - 1)],
                                                        texCoord[(int)(vtIndex - 1)]));
                            indices.Add(vertexIndex);
                            vertexMap.Add(d, vertexIndex);
                            vertexIndex++;
                        }
                    }
                    break;
                }
            }
        }
        
        Mesh mesh = new Mesh();
        mesh.VertexBuffer = vertices.ToArray();
        mesh.Indices = indices.ToArray();
        
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            
            Converters =
            {
                new OpenTkVector3Converter(),
                new QuaternionConverter(),
                new ComponentConverter()
            }
        };
        string jsonData = JsonConvert.SerializeObject(mesh);
        File.WriteAllText(libFilePath, jsonData);
        Console.WriteLine("Import successfully. with vertex:" + mesh.VertexBuffer.Length + " triangles:"+ mesh.TriangleCount);
    }
}