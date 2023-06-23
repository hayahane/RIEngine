using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RIEngine.Core;

namespace RIEngine.Graphics;

public class RIView
{
    public Camera ActiveCamera { get; set; } = null!;
    public Vector2i Resolution { get; set; }


    public RIView()
    {
        Resolution = new Vector2i(1280, 720);
    }
    
    public RIView(Vector2i resolution)
    {
        Resolution = resolution;
    }

    private Camera GetActiveCamera()
    {
        Camera? tmpCam = null;
        foreach (var riObject in RIWorld.Instance.RIObjects)
        {
            tmpCam = riObject.GetComponent<Camera>();
            if (tmpCam != null) break;
        }
        
        if (tmpCam == null) ActiveCamera = RIObject.Spawn().AddComponent<Camera>();
        return tmpCam!;
    }

    public void UpdateActiveCamera()
    {
        ActiveCamera = GetActiveCamera();
    }

    public void Initialize()
    {
        GL.Viewport(0, 0, Resolution.X, Resolution.Y);
        ActiveCamera = GetActiveCamera();
    }
    
    /// <summary>
    /// Find every MeshRenderer and render them.
    /// Setting a unique camera space matrix to get the correct view.
    /// </summary>
    public void Render()
    {
        foreach (var riObject in RIWorld.Instance.RIObjects)
        {
            if (!riObject.IsActive) continue;
            MeshRenderer? mr = riObject.GetComponent<MeshRenderer>();
            if (mr == null || !mr.IsEnabled) continue;
            
            mr.Render2View(this);
        }
    }
}