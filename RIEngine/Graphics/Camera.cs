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
    public float Near { get; set; } = 0.1f;
    public float Far { get; set; } = 1000f;
    
    public float FieldOfView
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set => _fov = MathHelper.DegreesToRadians(value);
    }
    public float Size { get; set; }= 5f;
    
    private float _fov = MathHelper.PiOver3;

    
    [JsonIgnore]
    public Matrix4 ProjectionMatrix;
    [JsonIgnore]
    public Matrix4 ViewMatrix;
    
    public Camera(RIObject riObject) : base(riObject)
    { }

    public void Update(RIView riView)
    {
        ViewMatrix = Matrix4.LookAt(RIObject.Transform.Position, RIObject.Transform.Position - RIObject.Transform.Forward, RIObject.Transform.Up);
        if (CastMode == CastMode.Perspective)
            ProjectionMatrix =  Matrix4.CreatePerspectiveFieldOfView(_fov, (float)riView.Resolution.X/riView.Resolution.Y, Near, Far);
        else
        {
            ProjectionMatrix =
                Matrix4.CreateOrthographic((float)riView.Resolution.X / riView.Resolution.Y * Size, Size, Near, Far);
        }
    }
}