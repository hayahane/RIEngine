using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace RIEngine.Core.Graphics;

public class RenderView
{
    public RIObject Root { get; set; }
    public Camera ActiveCamera { get; set; }
    public Vector2i Resolution { get; set; }

    public RenderView(RIObject root, Vector2i resolution)
    {
        Resolution = resolution;
        Root = root;
        if (ActiveCamera == null!) ActiveCamera = RIObject.Spawn().AddComponent<Camera>();
    }

    public void Initialize()
    {
        GL.Viewport(0, 0, 800, 800);
        IterateRenderTree(Root, IterateMode.Initialize);
    }

    private enum IterateMode
    {
        OnUpdateFrame,
        OnRenderFrame,
        OnUnload,
        Initialize
    }

    private void IterateRenderTree(RIObject ro, IterateMode mode)
    {
        /*if (ro.IsActive == false) return;

        switch (mode)
        {
            case IterateMode.OnUnload:
                ro.OnUnload();
                break;
            case IterateMode.OnUpdateFrame:
                ro.OnUpdateFrame();
                break;
            case IterateMode.OnRenderFrame:
                ro.OnRenderFrame();
                break;

            default:
                ro.Start();
                break;
        }


        if (ro.Children.Count == 0) return;
        foreach (var t in ro.Children)
        {
            IterateRenderTree(t, mode);
        }*/
    }

    #region Callbacks

    public void Start()
    {
        Initialize();
    }

    public void OnUpdateFrame()
    {
        IterateRenderTree(Root, IterateMode.OnUpdateFrame);
    }

    public void OnRenderFrame()
    {
        IterateRenderTree(Root, IterateMode.OnRenderFrame);
    }

    public void OnUnload()
    {
        IterateRenderTree(Root, IterateMode.OnUnload);
    }

    #endregion


    /*
    private RIObject? FindRenderObject(RIObject ro, Func<RIObject, bool> condition)
    {
        if (condition(ro))
            return ro;
        foreach (var child in ro.Children)
        {
            var result = FindRenderObject(child, condition);
            if (result != null)
                return result;
        }

        return null;
    }*/

    /*
    public List<RIObject>? FindingRenderObjects(Func<RIObject, bool> condition)
    {
        _findingObjectsList.Clear();

        FindRenderObjectsInternal(Root, condition);

        return _findingObjectsList;
    }*/

    /*private void FindRenderObjectsInternal(RIObject ro, Func<RIObject, bool> condition)
    {
        if (condition(ro))
            _findingObjectsList.Add(ro);

        foreach (var child in ro.Children)
        {
            FindRenderObjectsInternal(child, condition);
        }
    }*/
}