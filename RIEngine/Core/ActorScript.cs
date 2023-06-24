namespace RIEngine.Core;

/// <summary>
/// Scripting type for expansion.
/// Inherit this type and write your scripts in it.
/// Callback methods will be called automatically by the following order.
/// 
/// - OnSpawn() called when the RIObject is spawned.
/// - OnEnable() called when the RIObject is enabled.
/// - OnStart() called when the RIObject is started. Usually used for initialization.
/// 
/// - OnUpdate() called every frame.
/// - OnFixedUpdate() called every fixed frame. (TODO
/// - OnRenderFinished() called when the render of the frame is ended.
/// 
/// - OnDisable() called when the RIObject is disabled.
/// - OnDestroy() called when the RIObject is destroyed.
/// </summary>
public abstract class ActorScript : Behaviour
{
    protected ActorScript(RIObject riObject) : base(riObject)
    { }
    
    protected ActorScript(RIObject riObject, Guid guid) : base(riObject, guid)
    { }
    
    public virtual void OnFixedUpdate(){}
    public virtual void OnRenderFinished(){}
}