using OpenTK.Mathematics;
namespace RIEngine.Graphics;

public struct Vertex
{
    public float PosX;
    public float PosY;
    public float PosZ;

    public float NormalX;
    public float NormalY;
    public float NormalZ;
    
    public float TexCoordX;
    public float TexCoordY;

    public Vertex(Vector3 position, Vector3 normal, Vector2 tex)
    {
        PosX = position.X;
        PosY = position.Y;
        PosZ = position.Z;

        NormalX = normal.X;
        NormalY = normal.Y;
        NormalZ = normal.Z;

        TexCoordX = tex.X;
        TexCoordY = tex.Y;
    }
}