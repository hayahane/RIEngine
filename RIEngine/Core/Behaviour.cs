using Newtonsoft.Json;

namespace RIEngine.Core;

public abstract class Behaviour : Component
{
    private bool _isEnabled = true;

    public bool IsEnabled
    {
        get => _isEnabled && IsSpawnInit;
        set
        {
            if (value && !_isEnabled)
                OnEnable();
            else if (!value && _isEnabled)
                OnDisable();

            _isEnabled = value;
        }
    }
    
    [JsonIgnore]
    public bool IsInitialized { get; private set; }
    [JsonIgnore]
    public bool IsSpawnInit  {get; private set;}
    
    [JsonIgnore]
    public bool IsSelfActive => (IsEnabled && RIObject.IsActive);

    protected Behaviour(RIObject riObject) : base(riObject)
    {
        riObject?.Components.Add(this);
    }

    protected Behaviour(RIObject riObject, Guid guid) : base(riObject, guid)
    {
        riObject?.Components.Add(this);
    }

    #region Callbacks

    public virtual void OnSpawn()
    {
        IsSpawnInit = true;
    }

    public virtual void OnEnable()
    {
    }

    public virtual void OnInit()
    {
        IsInitialized = true;
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnDisable()
    {
    }

    public virtual void OnDestroy()
    {
    }

    #endregion
}