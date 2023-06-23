namespace RIEngine.Core;

public class RIObject
{
    public String Name { get; set; } = "RIObject";
    public string Tag { get; protected set; } = "Default";
    public bool IsActive { get; set; } = true;
    public Transform Transform { get; }

    public List<Component> Components { get; set; } = new List<Component>();


    private RIObject()
    {
        Transform = (Activator.CreateInstance(typeof(Transform), this) as Transform)!;
    }

    private RIObject(string name, RIObject? parent)
    {
        Name = name;
        Transform = (Activator.CreateInstance(typeof(Transform), this) as Transform)!;
        Transform.Parent = parent?.Transform;
    }

    #region APIs

    #region RIObject

    /// <summary>
    /// Spawn an empty RIObject.
    /// </summary>
    /// <returns>The Generated RIObject.</returns>
    public static RIObject Spawn()
    {
        var obj = new RIObject();
        RIWorld.Instance.RIObjects.Add(obj);
        return obj;
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
        Components.Add(component);
        return component;
    }

    /// <summary>
    /// Remove a component from current RIObject by type.
    /// </summary>
    /// <typeparam name="T">Type of the component.</typeparam>
    public void RemoveComponent<T>() where T : Behaviour
    {
        Components.Remove(GetComponent<T>()!);
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