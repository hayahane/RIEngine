namespace RIEngine.Core;

public abstract class Behaviour : Component
{
    private bool _isEnabled = true;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if(value && !_isEnabled)
                OnEnable();
            else if(!value && _isEnabled)
                OnDisable();
            
            _isEnabled = value;
        }
    }
    public bool IsSelfActive => (IsEnabled && RIObject.IsActive);
    
    protected Behaviour(RIObject riObject) : base(riObject){}

    #region Callbacks
    public virtual void OnSpawn(){}
    public virtual void OnEnable(){}
    public virtual void OnUpdate(){}
    public virtual void OnDisable(){}
    public virtual void OnDestroy(){}
    #endregion
}