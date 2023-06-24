using RIEngine.Patterns;
using Newtonsoft.Json;
using RIEngine.Graphics;

namespace RIEngine.Core;

public class RIWorld : Singleton<RIWorld>
{
    public RIView RIView { get; set; } = new RIView();

    [JsonIgnore] public GameTime GameTime { get; } = new GameTime();
    public List<RIObject> RIObjects { get; set; } = new List<RIObject>();

    /// <summary>
    /// Read .riScene file and load scene.
    /// </summary>
    /// <param name="path">File path.</param>
    /// <param name="fileName">File name.</param>
    /// <exception cref="Exception">Failed to load scene. The file may be damaged.</exception>
    public void LoadScene(string path, string fileName)
    {
        string jsonData = File.ReadAllText(path +@"\" + fileName);
        List<RIObject> newObjects = JsonConvert.DeserializeObject<List<RIObject>>(jsonData)
                                    ?? throw new Exception("Failed to load scene. The file may be damaged.");
        RIObjects = RIObjects.Concat(newObjects).ToList();

        RIView.UpdateActiveCamera();
    }
    
    
    /// <summary>
    /// Add scene to current RIWorld.
    /// </summary>
    /// <param name="path">Scene file path.</param>
    /// <param name="fileName">Scene file name.</param>
    public void AddScene(string path, string fileName)
    {
        string jsonData = File.ReadAllText(path + fileName);
        List<RIObject> newObjects = JsonConvert.DeserializeObject<List<RIObject>>(jsonData)
                                    ?? throw new Exception("Failed to load scene. The file may be damaged.");
        OnSpawnRIObjects(newObjects);
        RIObjects = RIObjects.Concat(newObjects).ToList();

        RIView.UpdateActiveCamera();
    }

    public void Initialize()
    {
        RIView.Initialize();
        OnSpawnRIObjects(RIObjects);
    }

    #region OnSpawn and OnDestroy

        public void OnSpawnRIObject(RIObject riObject)
    {
        TraversalRIObjects(riObject, obj =>
            {
                foreach (var component in obj.Components)
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


    /// <summary>
    /// Called when the game starts or Load new scenes.
    /// </summary>
    private void OnSpawnRIObjects(List<RIObject> objects)
    {
        foreach (var riObject in objects)
        {
            TraversalRIObjects(riObject, obj =>
                {
                    foreach (var component in obj.Components)
                    {
                        if (component is Behaviour behaviour)
                            behaviour.OnSpawn();
                    }
                },
                false);
        }

        foreach (var riObject in objects)
        {
            TraversalRIObjects(riObject, obj =>
            {
                foreach (var component in obj.Components)
                {
                    if (component is Behaviour behaviour)
                        behaviour.OnEnable();
                }
            });
        }
    }

    /// <summary>
    /// Called when the game ends or change scenes.
    /// </summary>
    private void OnDestroyRIObjects()
    {
        foreach (var riObject in RIObjects)
        {
            TraversalRIObjects(riObject, obj =>
            {
                foreach (var component in obj.Components)
                {
                    if (component is Behaviour behaviour)
                        behaviour.OnDisable();
                }
            });
        }

        foreach (var riObject in RIObjects)
        {
            TraversalRIObjects(riObject, obj =>
                {
                    foreach (var component in obj.Components)
                    {
                        if (component is Behaviour behaviour)
                            behaviour.OnDestroy();
                    }
                }
                , false);
        }
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
        foreach (var child in riObject.Transform.Children)
        {
            TraversalRIObjects(child.RIObject, action);
        }
    }

    private void RenderToView(RIView riView)
    {
        riView.PreRender();
        riView.ActiveCamera.Update(riView);

        foreach (var riObject in RIObjects)
        {
            TraversalRIObjects(riObject, obj =>
            {
                MeshRenderer? meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer is not { IsEnabled: true }) return;

                meshRenderer.Render2View(riView);
            });
        }
    }
    
    /// <summary>
    /// Call ActorScript methods by order to update the world objects.
    /// </summary>
    public void UpdateWorld()
    {
        GameTime.UpdateTime();

        // Update Transform 
        foreach (var riObject in RIObjects)
        {
            TraversalRIObjects(riObject, obj => { obj.Transform.UpdateTransform(); });
        }

        // Update ActorScripts
        foreach (var riObject in RIObjects)
        {
            TraversalRIObjects(riObject, obj =>
            {
                foreach (var component in obj.Components)
                {
                    if (component is not ActorScript { IsEnabled: true } script) continue;
                    if (script.IsInitialized)
                        script.OnUpdate();
                    else
                        script.OnInit();
                }
            });
        }
    }

    public void RenderWorld()
    {
        // Render
        RenderToView(RIView);

        // Render Finished
        foreach (var riObject in RIObjects)
        {
            TraversalRIObjects(riObject, obj =>
            {
                foreach (var component in obj.Components)
                {
                    if (component is ActorScript { IsEnabled: true } script)
                        script.OnRenderFinished();
                }
            });
        }
    }


    public void DestroyWorld()
    {
        OnDestroyRIObjects();
        RIObjects.Clear();
    }

    #region Find RIObject and Components
    
    public RIObject? FindRIObject(string name)
    {
        RIObject? result = null;
        
        foreach (var riObject in RIObjects)
        {
            result = FindRIObject_Internal(riObject, obj => obj.Name == name);
            if (result != null) break;
        }

        return result;
    }

    private RIObject? FindRIObject_Internal(RIObject current,Func<RIObject,bool> condition)
    {
        if (condition(current)) return current;

        foreach (var child in current.Transform.Children)
        {
            var riObject = FindRIObject_Internal(child.RIObject, condition);
            if (riObject != null) return riObject;
        }

        return null;
    }

    public T? FindComponent<T>() where T : Component
    {
        T? component = null;

        foreach (var riObject in RIObjects)
        {
            component = FindComponent_Internal<T>(riObject);
            if (component != null) break;
        }

        return component;
    }
    
    private T? FindComponent_Internal<T>(RIObject current) where T: Component
    {
        foreach (var component in current.Components)
        {
            if (component is T t)
                return t;
        }
        
        T? result = null;
        foreach (var child in current.Transform.Children)
        {
            result =  FindComponent_Internal<T>(child.RIObject);
            if (result != null) return result;
        }

        return result;
    }

    #endregion
}