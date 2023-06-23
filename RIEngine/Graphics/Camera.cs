using System.Text.Json.Serialization;
using OpenTK.Mathematics;
using RIEngine.Core;

namespace RIEngine.Graphics;

public enum CastMode
{
    Perspective, Orthographic
}

public class Camera : Behaviour
{

    public CastMode CastMode { get; set; } = CastMode.Perspective;
    public float FieldOfView = 60;
    
    [JsonIgnore]
    public Matrix4 ProjectionMatrix;
    [JsonIgnore]
    public Matrix4 ViewMatrix;
    
    private Camera(RIObject riObject) : base(riObject)
    { }

    public void Update()
    {
        ViewMatrix = Matrix4.LookAt(RIObject.Transform.Position, RIObject.Transform.Position - RIObject.Transform.Forward, RIObject.Transform.Up);
        //ProjectionMatrix =  Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, RIView.Resolution.X/ RIView.Resolution.Y, 0.1f, 1000f);
    }
}