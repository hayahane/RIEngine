using Newtonsoft.Json;
using OpenTK.Mathematics;
using RIEngine.Core;

namespace RIEngine.Test;

public class TriggerJump : ActorScript
{
    public TriggerJump(RIObject riObject, Guid guid) : base(riObject, guid)
    { }

    public TriggerJump(RIObject riObject) : base(riObject)
    { }

    public float TriggerRadius { get; set; } = 1f;
    [JsonIgnore]
    private bool _isTriggered = false;
    public float LifeUpDistance { get; set; } = 0.5f;
    public RIObject Player { get; set; }

    public override void OnEnable()
    {
        base.OnEnable();
        _isTriggered = false;
    }

    public override void OnUpdate()
    {
        if (Vector3.Distance(Player.Transform.Position, this.RIObject.Transform.Position) < TriggerRadius)
        {
            _isTriggered = true;
        }

        if (_isTriggered)
        {
            this.RIObject.Transform.Position += Vector3.UnitY * (RIWorld.Instance.GameTime.DeltaTime * LifeUpDistance);
        }
    }
}