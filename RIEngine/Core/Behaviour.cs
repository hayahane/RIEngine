namespace RIEngine.Core;

public abstract class Behaviour : Component
{
    public bool IsEnabled { get; set; } = true;
    public bool IsSelfActive => (IsEnabled && RIObject.IsActive);
    
    protected Behaviour(RIObject riObject) : base(riObject){}

    #region Callbacks
    public virtual void OnSpawn(){}
    public virtual void OnDestroy(){}
    #endregion
}