using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using RIEngine.Core;

namespace RIEngine.Graphics;

public class MeshRenderer : Behaviour
{
    [JsonIgnore] public Mesh Mesh { get; set; }

    [JsonIgnore] public Shader Shader { get; set; }
    [JsonIgnore] public Texture Texture { get; set; }

    public string? MeshPath { get; set; }
    public string? VertPath { get; set; }
    public string? FragPath { get; set; }
    public string? TexturePath { get; set; }

    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private int _elementBufferObject;
    
    public MeshRenderer(RIObject riObject, Guid guid) : base(riObject, guid)
    {
        Mesh = null!;
        Shader = null!;
        Texture = null!;
    }
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


    private void WriteMeshToBuffer()
    {
        // Generate Vertex Buffer Object
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, Mesh.VertexBuffer.Length * Mesh.BufferStride * sizeof(float),
            Mesh.VertexBuffer,
            BufferUsageHint.DynamicDraw);

        // Generate Vertex Array Object
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);
        // Vertex Positions
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        // Vertex Normals
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        // Vertex UVs
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        // Generate Element Buffer Object
        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Mesh.Indices.Length * sizeof(uint), Mesh.Indices,
            BufferUsageHint.StaticDraw);
    }


    #region Callbacks

    public override void OnSpawn()
    {
        base.OnSpawn();
        Initialize();
        WriteMeshToBuffer();
    }

    public void Render2View(RIView riView)
    {
        GL.BindVertexArray(_vertexArrayObject);

        Shader.SetMat4("model", RIObject.Transform.LocalToWorldMatrix);
        Shader.SetMat4("view", riView.ActiveCamera.ViewMatrix);
        Shader.SetMat4("projection", riView.ActiveCamera.ProjectionMatrix);

        if (riView.DirectionalLight != null)
        {
            Shader.SetVec3("directionalLight.direction", riView.DirectionalLight.Direction);
            Shader.SetVec3("directionalLight.color", new Vector3(riView.DirectionalLight.FinalColor.R,
                riView.DirectionalLight.FinalColor.G,riView.DirectionalLight.FinalColor.B));
        }
        else
        {
            Shader.SetVec3("directionalLight.direction",Vector3.Zero);
            Shader.SetVec3("directionalLight.color", Vector3.Zero);
        }

        for (int i = 0; i < RIView.PointLightLimits; i++)
        {
            if (i < riView.PlCount)
            {
                Shader.SetVec3("pointLights[" + i + "].position", riView.PointLights[i].RIObject.Transform.Position);
                Shader.SetVec3("pointLights[" + i + "].color", new Vector3(riView.PointLights[i].FinalColor.R,
                    riView.PointLights[i].FinalColor.G, riView.PointLights[i].FinalColor.B));
                Shader.SetFloat("pointLights[" + i + "].range", riView.PointLights[i].Range);
                continue;
            }
            Shader.SetVec3("pointLights[" + i + "].position", new Vector3(0, 0, 0));
            Shader.SetVec3("pointLights[" + i + "].color", new Vector3(0, 0, 0));
            Shader.SetFloat("pointLights[" + i + "].range", 0.1f);
        }
        
        Shader.Use();
        Texture.Use(TextureUnit.Texture0);

        GL.DrawElements(PrimitiveType.Triangles, Mesh.Indices.Length, DrawElementsType.UnsignedInt, 0);
    }
    
    public override void OnDestroy()
    {
        Shader.Dispose();
    }

    #endregion

}