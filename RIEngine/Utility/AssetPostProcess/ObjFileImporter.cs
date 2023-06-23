using System.Diagnostics;
using OpenTK.Mathematics;

namespace RIRenderer.Utility.AssetPostProcess;

public static class ObjFileImporter
{
    public static void ImportObjFile(string objFilePath, string libFilePath)
    {
        string[] lines = File.ReadAllLines(objFilePath);
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> vertNormal =  new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> texCoord = new List<Vector2>();
        List<uint> indices = new List<uint>();

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
                    vertices.Add(new Vector3(float.Parse(data[0]),
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
                // fragment indices
                case "f":
                {
                    string[] data = subLine.Split(" ");
                    foreach (var d in data)
                    {
                        string[] indexData = d.Split('/');
                        uint vertIndex = uint.Parse(indexData[0]);
                        uint vnIndex = uint.Parse(indexData[2]);
                        indices.Add(vertIndex);
                    }
                    break;
                }
                // tex coordinate (uv1)
                case "vt":
                {
                    string[] data = subLine.Split(" ");
                    texCoord.Add(new Vector2(float.Parse(data[0]), float.Parse(data[1])));
                    break;
                }
            }
        }
    }
}