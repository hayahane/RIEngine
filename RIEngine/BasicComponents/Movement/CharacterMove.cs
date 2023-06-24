using RIEngine.Core;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RIEngine.Test;

namespace RIEngine.BasicComponents.Movement;

public class CharacterMove : ActorScript
{
    public float MoveSpeed = 1f;
    public KeyboardState Input;
    
    public CharacterMove(RIObject riObject, Guid guid) : base(riObject,guid)
    { }
    public CharacterMove(RIObject riObject) : base(riObject)
    {
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        Input = TestProgram.Input;
        if (Input.IsKeyDown(Keys.W))
        {
            RIObject.Transform.Position -=
                RIObject.Transform.Forward * (RIWorld.Instance.GameTime.DeltaTime * MoveSpeed);
        }

        if (Input.IsKeyDown(Keys.S))
        {
            RIObject.Transform.Position +=
                RIObject.Transform.Forward * (RIWorld.Instance.GameTime.DeltaTime * MoveSpeed);
        }

        if (Input.IsKeyDown(Keys.A))
        {
            RIObject.Transform.Position -=
                RIObject.Transform.Right * (RIWorld.Instance.GameTime.DeltaTime * MoveSpeed);
        }

        if (Input.IsKeyDown(Keys.D))
        {
            RIObject.Transform.Position +=
                RIObject.Transform.Right * (RIWorld.Instance.GameTime.DeltaTime * MoveSpeed);
        }
    }
}