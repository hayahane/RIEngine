using RIEngine.Patterns;
using Newtonsoft.Json;
using RIEngine.Graphics;

namespace RIEngine.Core;

public class RIWorld : Singleton<RIWorld>
{
    public RIView RIView { get; set; } = new RIView();
    public GameTime GameTime { get;} = new GameTime();
    public List<RIObject> RIObjects { get; set; } = new List<RIObject>();
    
    /// <summary>
    /// Read .riScene file and load scene.
    /// </summary>
    /// <param name="path">File path.</param>
    /// <param name="fileName">File name.</param>
    /// <exception cref="Exception">Failed to load scene. The file may be damaged.</exception>
    public void LoadScene(string path, string fileName)
    {
        string jsonData = File.ReadAllText(path + fileName);
        List<RIObject> newObjects = JsonConvert.DeserializeObject<List<RIObject>>(jsonData)
            ?? throw new Exception("Failed to load scene. The file may be damaged.");
        SpawnRIObjects(newObjects);
        RIObjects = RIObjects.Concat(newObjects).ToList();
        
        RIView.UpdateActiveCamera();
    }

    public void Initialize()
    {
        RIView.Initialize();
    }
    
    /// <summary>
    /// Called when the game starts or Load new scenes.
    /// </summary>
    private void SpawnRIObjects(List<RIObject> objects)
    {
        foreach (var riObject in objects)
        {
            foreach (var component in riObject.Components)
            {
                if (component is Behaviour behaviour)
                    behaviour.OnSpawn();
            }
        }
    }
    
    /// <summary>
    /// Called when the game ends or change scenes.
    /// </summary>
    private void DestroyRIObjects()
    {
        foreach (var riObject in RIObjects)
        {
            foreach (var component in riObject.Components)
            {
                if (component is Behaviour behaviour)
                    behaviour.OnDestroy();
            }
        }
    }
    
    /// <summary>
    /// Call ActorScript methods by order to update the world objects.
    /// </summary>
    private void UpdateWorld()
    {
        GameTime.UpdateTime();
        
        // Update Transform 
        foreach (var riObject in RIObjects)
        {
            if (!riObject.IsActive) continue;
            
            riObject.Transform.UpdateTransform();
        }
        
        // Update ActorScripts
        foreach (var riObject in RIObjects)
        {
            if (!riObject.IsActive) continue;
            
            foreach (var component in riObject.Components)
            {
                if (component is ActorScript script && script.IsEnabled)
                    script.OnUpdate();
            }
        }
        
        // Render
        RIView.Render();
        
        
        // Render Finished
        foreach (var riObject in RIObjects)
        {
            if (!riObject.IsActive) continue;
            
            foreach (var component in riObject.Components)
            {
                if (component is ActorScript script && script.IsEnabled)
                    script.OnRenderFinished();
            }
        }
    }
}