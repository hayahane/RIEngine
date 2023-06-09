using System.Drawing;
using RIEngine.Patterns;
using Newtonsoft.Json;
using RIEngine.Graphics;
using RIEngine.Utility.Serialization;

namespace RIEngine.Core;

public class RIWorld : Singleton<RIWorld>
{
    public RIView RIView { get; set; } = new RIView();

    [JsonIgnore] public GameTime GameTime { get; } = new GameTime();
    public RIObject WorldRoot { get; set; } = new RIObject("Root", null);

    private void HandleObjectReference()
    {
        foreach (var riObject in WorldRoot)
        {
            for (int i = 0; i < riObject.Components.Count; i++)
            {
                var component = riObject.Components[i];
                if (component is not Behaviour)
                {
                    component = GuidReferenceHelper.GuidReferenceMap[component.Guid] as Component;
                }
            }
        }
    }
    

    /// <summary>
    /// Read .riScene file and load scene.
    /// </summary>
    /// <param name="path">File path.</param>
    /// <param name="fileName">File name.</param>
    /// <exception cref="Exception">Failed to load scene. The file may be damaged.</exception>
    public void LoadScene(string path)
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            NullValueHandling = NullValueHandling.Ignore,

            //Converters
            Converters =
            {
                new OpenTkVector3Serializer(), 
                new OpenTkQuaternionSerializer(),
                new ComponentSerializer(), new OpenTkVector2iSerializer(),
                new RIObjectSerializer()
            }
        };
        string jsonData = File.ReadAllText(path);
        WorldRoot = JsonConvert.DeserializeObject<RIObject>(jsonData)
                                    ?? throw new Exception("Failed to load scene. The file may be damaged.");
        
        HandleObjectReference();
        OnSpawnRIObject(WorldRoot);

        RIView.UpdateActiveCamera();
    }


    /// <summary>
    /// Add scene to current RIWorld.
    /// </summary>
    /// <param name="path">Scene file path.</param>
    public void AddScene(string path)
    {
        string jsonData = File.ReadAllText(path);
        
        RIObject newObjects = JsonConvert.DeserializeObject<RIObject>(jsonData)
                                    ?? throw new Exception("Failed to load scene. The file may be damaged.");
        WorldRoot.Children.Add(newObjects);
        newObjects.Parent = WorldRoot;
        OnSpawnRIObject(newObjects);

        RIView.UpdateActiveCamera();
    }
    
    /// <summary>
    /// Initialize RIWorld.
    /// </summary>
    public void Initialize()
    {
        RIView.Initialize();
        OnSpawnRIObject(WorldRoot);
    }

    #region OnSpawn and OnDestroy

    public void OnSpawnRIObject(RIObject riObject)
    {
        TraversalRIObjects(riObject, ro =>
            {
                foreach (var component in ro.Components)
                {
                    if (component is Behaviour behaviour)
                        behaviour.OnSpawn();
                }
            },
            false);


        TraversalRIObjects(riObject, obj =>
        {
            foreach (var component in obj.Components)
            {
                if (component is Behaviour behaviour)
                    behaviour.OnEnable();
            }
        });
    }

    public void OnDestroyRIObject(RIObject riObject)
    {
        TraversalRIObjects(riObject, obj =>
        {
            foreach (var component in obj.Components)
            {
                if (component is Behaviour behaviour)
                    behaviour.OnDisable();
            }
        });
        TraversalRIObjects(riObject, obj =>
            {
                foreach (var component in obj.Components)
                {
                    if (component is Behaviour behaviour)
                        behaviour.OnDestroy();
                }
            },
            false);
    }

    #endregion

    /// <summary>
    /// Traversal all RIObjects and call the action.
    /// </summary>
    /// <param name="riObject">Current RIObject.</param>
    /// <param name="action">Action to do.</param>
    /// <param name="activeCompareOn">Whether to enable active tag compare.</param>
    private void TraversalRIObjects(RIObject riObject, Action<RIObject> action, bool activeCompareOn = true)
    {
        if (!riObject.IsActive && activeCompareOn) return;

        action(riObject);
        foreach (var child in riObject.Children)
        {
            TraversalRIObjects(child, action);
        }
    }
    
    /// <summary>
    /// Render current scene to target RIView.
    /// </summary>
    /// <param name="riView">Target RIView.</param>
    private void RenderToView(RIView riView)
    {
        riView.PreRender();
        int lightIndex = 0;
        TraversalRIObjects(WorldRoot, obj =>
        {
            BaseLight? light = obj.GetComponent<BaseLight>();
            if (riView.DirectionalLight == null && light is DirectionalLight{IsEnabled:true} dl)
            {
                riView.DirectionalLight = dl;
            }

            if (lightIndex < RIView.PointLightLimits && light is PointLight{IsEnabled:true} pl)
            {
                riView.AddPointLight(pl);
                lightIndex++;
            }
        });
        
        TraversalRIObjects(WorldRoot, obj =>
        {

            MeshRenderer? meshRenderer = obj.GetComponent<MeshRenderer>();
            if (meshRenderer is not { IsEnabled: true }) return;

            meshRenderer.Render2View(riView);
        });
    }

    /// <summary>
    /// Call ActorScript methods by order to update the world objects.
    /// </summary>
    public void UpdateWorld()
    {
        GameTime.UpdateTime();

        // Update Transform 
        TraversalRIObjects(WorldRoot, obj =>
        {
            if (obj == WorldRoot && !obj.IsActive) return;
            obj.Transform.UpdateTransform();
        });

        // Update ActorScripts
        TraversalRIObjects(WorldRoot, obj =>
        {
            if (obj == WorldRoot || !obj.IsActive) return;
            
            foreach (var component in obj.Components)
            {
                if (component is Behaviour { IsSpawnInit: false } be)
                {
                    be.OnSpawn();
                }
                if (component is Behaviour { IsEnabled: true, IsSpawnInit: true } behaviour)
                {
                    if (behaviour.IsInitialized)
                        behaviour.OnUpdate();
                    else
                        behaviour.OnInit();
                }
            }
        });
    }
    
    /// <summary>
    /// Render update.
    /// </summary>
    public void RenderWorld()
    {
        // Render
        RenderToView(RIView);

        // Render Finished
        TraversalRIObjects(WorldRoot, obj =>
        {
            if (obj == WorldRoot || !obj.IsActive) return;
            
            foreach (var component in obj.Components)
            {
                if (component is ActorScript{IsEnabled:true, IsSpawnInit:true} behaviour)
                    behaviour.OnRenderFinished();
            }
        });
    }

    /// <summary>
    /// End program and dispose RIWorld.
    /// </summary>
    public void DestroyWorld()
    {
        OnDestroyRIObject(WorldRoot);
        WorldRoot.Children.Clear();
    }

    #region Find RIObject and Components
    
    /// <summary>
    /// Find a RIObject by name.
    /// </summary>
    /// <param name="name">Name to filter a RIObject.</param>
    /// <returns>The target object. Null if not found.</returns>
    public RIObject? FindRIObject(string name)
    {
        foreach (var riObject in WorldRoot)
        {
            if (riObject.Name == name) return riObject;
        }

        return null;
    }
    
    /// <summary>
    /// Find a component by type.
    /// A component should be inherited from Component.
    /// </summary>
    /// <typeparam name="T">Type of the component.</typeparam>
    /// <returns>Target component. Null if not found.</returns>
    public T? FindComponent<T>() where T : Component
    {
        foreach (var riObject in WorldRoot)
        {
            foreach (var component in riObject.Components)
            {
                if (component is T t) return t;
            }
        }

        return null;
    }

    #endregion
}