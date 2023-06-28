using OpenTK.Mathematics;
using RIEngine.Core;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RIEngine.Test;

namespace RIEngine.BasicComponents.Movement;

public class CharacterMove : ActorScript
{
    public float MoveSpeed { get; set; } = 1f;
    public float RotateSpeed { get; set; } = 1f;
    public KeyboardState? Input;

    public CharacterMove(RIObject riObject, Guid guid) : base(riObject,guid)
    { }
    public CharacterMove(RIObject riObject) : base(riObject)
    {
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        Input = TestProgram.Input;
        if (Input == null) return;
        if (Input.IsKeyDown(Keys.W))
        {
            RIObject.Transform.Position -=
                (new Vector3(RIObject.Transform.Forward.X,0,RIObject.Transform.Forward.Z).Normalized()) * (RIWorld.Instance.GameTime.DeltaTime * MoveSpeed);
        }

        if (Input.IsKeyDown(Keys.S))
        {
            RIObject.Transform.Position +=
                new Vector3(RIObject.Transform.Forward.X,0,RIObject.Transform.Forward.Z).Normalized()* (RIWorld.Instance.GameTime.DeltaTime * MoveSpeed);
        }

        if (Input.IsKeyDown(Keys.A))
        {
            RIObject.Transform.Position -=
                new Vector3(RIObject.Transform.Right.X,0,RIObject.Transform.Right.Z).Normalized() * (RIWorld.Instance.GameTime.DeltaTime * MoveSpeed);
        }

        if (Input.IsKeyDown(Keys.D))
        {
            RIObject.Transform.Position +=
                new Vector3(RIObject.Transform.Right.X,0,RIObject.Transform.Right.Z).Normalized() * (RIWorld.Instance.GameTime.DeltaTime * MoveSpeed);
        }
        
        if (Input.IsKeyDown(Keys.E))
            RIObject.Transform.Rotation = Quaternion.FromEulerAngles(0, 
                 -RotateSpeed*RIWorld.Instance.GameTime.DeltaTime, 0) * RIObject.Transform.Rotation;
        if (Input.IsKeyDown(Keys.Q))
            RIObject.Transform.Rotation = Quaternion.FromEulerAngles(0, 
                RotateSpeed*RIWorld.Instance.GameTime.DeltaTime, 0) * RIObject.Transform.Rotation;
    }
}