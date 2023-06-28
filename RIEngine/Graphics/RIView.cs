using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using RIEngine.Core;

namespace RIEngine.Graphics;

public class RIView
{
    public Camera ActiveCamera { get; set; } = null!;
    public Vector2i Resolution { get; set; }
    public Color4 BackgroundColor { get; set; } = new Color4(0.05f, 0.50f, 0.50f, 1f);
    
    public DirectionalLight? DirectionalLight = null;
    public const int PointLightLimits = 4;
    public int PlCount => _plCount;
    private int _plCount = 0;
    private PointLight[] _pointLights = new PointLight[PointLightLimits];
    public PointLight[] PointLights => _pointLights;
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
        Camera? tmpCam = RIWorld.Instance.FindComponent<Camera>();
        if (tmpCam == null) ActiveCamera = RIObject.Spawn().AddComponent<Camera>();
        return tmpCam!;
    }

    public void UpdateActiveCamera()
    {
        var tmpActiveCamera = RIWorld.Instance.FindComponent<Camera>();
        if (tmpActiveCamera != null) ActiveCamera = tmpActiveCamera;
    }
    
    /// <summary>
    /// Handles pre-rendering functions：
    /// - Clear color buffer and depth buffer.
    /// - Update active camera.
    /// - Clear lights.
    /// </summary>
    public void PreRender()
    {
        GL.ClearColor(BackgroundColor);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        DirectionalLight = null;
        _pointLights = new PointLight[PointLightLimits];
        _plCount = 0;
        ActiveCamera.Update(this);
    }

    public void Initialize()
    {
        GL.Viewport(0, 0, Resolution.X, Resolution.Y);
        GL.ClearColor(BackgroundColor);
        GL.Enable(EnableCap.DepthTest);
        ActiveCamera = GetActiveCamera();
    }
    
    /// <summary>
    /// Try add a point light to the view.
    /// </summary>
    /// <param name="pointLight">Target point light.</param>
    /// <returns>Whether point light is successfully added.</returns>
    public bool AddPointLight(PointLight pointLight)
    {
        if (_plCount >= PointLightLimits) return false;
        _pointLights[_plCount] = pointLight;
        _plCount++;

        return true;
    }
}