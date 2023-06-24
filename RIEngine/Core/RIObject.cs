namespace RIEngine.Core;

public class RIObject
{
    public String Name { get; set; }
    public string Tag { get; protected set; } = "Default";
    public bool IsActive { get; set; } = true;
    public Transform Transform { get; }

    public List<Component> Components { get; set; } = new List<Component>();
    

    public RIObject(string name, RIObject? parent)
    {
        Name = name;
        Transform = (Activator.CreateInstance(typeof(Transform), this) as Transform)!;
        Transform.Parent = parent?.Transform;
        parent?.Transform.Children.Add(this.Transform);
    }

    #region APIs

    #region RIObject

    /// <summary>
    /// Spawn an empty RIObject.
    /// </summary>
    /// <returns>The Generated RIObject.</returns>
    public static RIObject Spawn()
    {
        var obj = new RIObject("RIObject",null);
        RIWorld.Instance.RIObjects.Add(obj);
        RIWorld.Instance.OnSpawnRIObject(obj);
        return obj;
    }
    
    /// <summary>
    ///  Spawn a riObject with a RIObject instance.
    /// </summary>
    /// <param name="riObject"></param>
    public static void Spawn(RIObject riObject)
    {
        if (riObject.Transform.Parent == null)
            RIWorld.Instance.RIObjects.Add(riObject);
        else
            riObject.Transform.Parent.Children.Add(riObject.Transform);
        RIWorld.Instance.OnSpawnRIObject(riObject);
    }

    /// <summary>
    /// Destroy an RIObject.
    /// </summary>
    /// <param name="riObject">RIObject to be destroyed.</param>
    public static void Destroy(RIObject riObject)
    {
        RIWorld.Instance.RIObjects.Remove(riObject);

        foreach (var component in riObject.Components)
        {
            if (component is Behaviour behaviour)
                behaviour.OnDestroy();
        }
    }

    #endregion

    #region Componenets

    /// <summary>
    /// Add a component to current RIObject.
    /// </summary>
    /// <typeparam name="T">Type of the component. (Inherit type Behaviour)</typeparam>
    /// <returns>The component added to target RIObject.</returns>
    public T AddComponent<T>() where T : Behaviour
    {
        T component = (Activator.CreateInstance(typeof(T), this) as T)!;
        return component;
    }

    /// <summary>
    /// Remove a component from current RIObject by type.
    /// </summary>
    /// <typeparam name="T">Type of the component.</typeparam>
    public void RemoveComponent<T>() where T : Behaviour
    {
        var component = GetComponent<T>();
        if (component == null) return;
        Components.Remove(component);
    }

    /// <summary>
    /// Get a component by type. This will return the first component found.
    /// </summary>
    /// <typeparam name="T">Type of the component.</typeparam>
    /// <returns>Target component.</returns>
    public T? GetComponent<T>() where T : Behaviour
    {
        return Components.Find(c => c is T) as T;
    }

    /// <summary>
    /// Get all components on this RIObject by type.
    /// </summary>
    /// <typeparam name="T">Type of the component.</typeparam>
    /// <returns>Array of target component.</returns>
    public T[] GetComponents<T>() where T : Behaviour
    {
        return Components.FindAll(c => c is T).Cast<T>().ToArray();
    }

    #endregion

    #endregion
}