using System.Collections;
using Newtonsoft.Json;
using RIEngine.Utility.Serialization;

namespace RIEngine.Core;

[JsonConverter(typeof(RIObjectSerializer))]
public class RIObject : SerializableObject, IEnumerable<RIObject>
{
    public String Name { get; set; }
    public string Tag { get; set; } = "Default";
    public bool IsActive { get; set; } = true;
    public Transform Transform { get; }
    public List<Component> Components { get; set; } = new List<Component>();
    [JsonIgnore] private RIObject? _parent;

    public RIObject? Parent
    {
        get => _parent;
        set
        {
            _parent = value;
            if (value != null)
                Transform.Parent = value.Transform;
        }
    }

    public List<RIObject> Children { get; set; } = new List<RIObject>();

    [JsonIgnore] public bool IsInWorld => Parent != null;

    public RIObject(Guid guid, Guid transformGuid) : base(guid)
    {
        Name = "RIObject";
        Transform = new Transform(this, transformGuid);
    }

    public RIObject(string name, RIObject? parent) : base()
    {
        Name = name;
        Transform = (Activator.CreateInstance(typeof(Transform), this) as Transform)!;
        Parent = parent;
        Parent?.Children.Add(this);
    }

    #region APIs

    #region RIObject

    /// <summary>
    /// Spawn an empty RIObject.
    /// </summary>
    /// <returns>The Generated RIObject.</returns>
    public static RIObject Spawn()
    {
        var obj = new RIObject("RIObject", null);
        RIWorld.Instance.WorldRoot.Children.Add(obj);
        obj.Parent = RIWorld.Instance.WorldRoot;
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
        {
            RIWorld.Instance.WorldRoot.Children.Add(riObject);
            riObject.Parent = RIWorld.Instance.WorldRoot;
        }
        else
            riObject.Children.Add(riObject);

        RIWorld.Instance.OnSpawnRIObject(riObject);
    }

    /// <summary>
    /// Destroy an RIObject.
    /// </summary>
    /// <param name="riObject">RIObject to be destroyed.</param>
    public static void Destroy(RIObject riObject)
    {
        foreach (var obj in RIWorld.Instance.WorldRoot)
        {
            if (obj == riObject)
            {
                riObject.Parent?.Children.Remove(riObject);
            }
        }


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

    public IEnumerator<RIObject> GetEnumerator()
    {
        yield return this;

        foreach (var child in Children)
        {
            foreach (var node in child)
            {
                yield return node;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}