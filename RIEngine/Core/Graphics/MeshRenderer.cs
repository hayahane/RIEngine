using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;

namespace RIEngine.Core.Graphics;

public class MeshRenderer : Behaviour
{
    [JsonIgnore] public Mesh Mesh { get; set; }
    private float[] _vertexBuffer = Array.Empty<float>();

    [JsonIgnore] public Shader Shader { get; set; }
    [JsonIgnore] public Texture Texture { get; set; }

    public string? MeshPath { get; set; }
    public string? VertPath { get; set; }
    public string? FragPath { get; set; }
    public string? TexturePath { get; set; }

    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private int _elementBufferObject;

    public MeshRenderer(RIObject riObject) : base(riObject)
    {
        Mesh = null!;
        Shader = null!;
        Texture = null!;
    }

    public void Initialize()
    {
        if (MeshPath != null) Mesh = Mesh.LoadFromFile(MeshPath);
        if (VertPath != null && FragPath != null) Shader = new Shader(VertPath, FragPath);
        if (TexturePath != null) Texture = Texture.LoadFromFile(TexturePath);
    }


    public void WriteMeshToGPU()
    {
        // Generate Vertex Buffer Object
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, Mesh.Vertices.Length * Mesh.BufferStride * sizeof(float),
            Mesh.VertexBuffer,
            BufferUsageHint.DynamicDraw);

        // Generate Vertex Array Object
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);
        // Vertex Positions
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 23 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        // Vertex Colors
        GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 23 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        // Vertex UVs
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 23 * sizeof(float), 13 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        // Generate Element Buffer Object
        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Mesh.Indices.Length * sizeof(uint), Mesh.Indices,
            BufferUsageHint.StaticDraw);
    }

    public override void OnSpawn()
    {
        base.OnSpawn();

        Initialize();

        WriteMeshToGPU();
    }

    public override void OnUpdateFrame()
    {
        base.OnUpdateFrame();
    }

    public void OnRenderFrame()
    {
        GL.BindVertexArray(_vertexArrayObject);

        Shader.SetMat4("model", RIObject.Transform.LocalToWorldMatrix);
        //Shader.SetMat4("view", RIEngine.Core.RenderView.ActiveCamera.ViewMatrix);
        //Shader.SetMat4("projection", RIEngine.Core.RenderView.ActiveCamera.ProjectionMatrix);
        Shader.Use();
        Texture.Use(TextureUnit.Texture0);

        GL.DrawElements(PrimitiveType.Triangles, Mesh.Indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public override void OnDestroy()
    {
        Shader?.Dispose();
    }
}