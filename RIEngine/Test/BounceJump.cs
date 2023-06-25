using Newtonsoft.Json;
using OpenTK.Mathematics;
using RIEngine.Core;

namespace RIEngine.Test;

public class BounceJump : ActorScript
{
    public BounceJump(RIObject riObject, Guid guid) : base(riObject, guid)
    { }

    public BounceJump(RIObject riObject) : base(riObject)
    { }
    
    public float LifeUpDistance { get; set; } = 1f;
    public float MoveSpeed { get; set; } = 2f;
    public RIObject Player { get; set; }
    
    private Vector3 _targetPosition;
    private int dir = 1;

    public override void OnEnable()
    {
        base.OnEnable();
        _targetPosition = this.RIObject.Transform.Position + Vector3.UnitY * LifeUpDistance;
    }

    public override void OnUpdate()
    {
        this.RIObject.Transform.Position = Vector3.Lerp(RIObject.Transform.Position, _targetPosition, 
            MoveSpeed * RIWorld.Instance.GameTime.DeltaTime);

        if (Vector3.Distance(RIObject.Transform.Position, _targetPosition) < 0.01f)
        {
            _targetPosition -= Vector3.UnitY * LifeUpDistance * dir;
            dir = -dir;
        }
    }
}