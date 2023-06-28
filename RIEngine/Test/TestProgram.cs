using Newtonsoft.Json;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RIEngine.Core;
using RIEngine.Graphics;
using RIEngine.Utility.Serialization;

namespace RIEngine.Test;

/// <summary>
/// Test program for RIEngine.
/// Inherit from GameWindow.
/// (which is a class provided by OpenTK to create a window and update it)
/// RIEngine runs as a singleton called RIWorld.
/// To change what is rendered, change the current scene in RIWorld.
/// This Test program will automatically load the scene from the resources folder,
/// and update RIWorld every frame. 
/// </summary>
public class TestProgram : GameWindow
{
    private RIView _riView;
    public static KeyboardState? Input;
    public TestProgram(int width, int height, string title) : base(
        GameWindowSettings.Default, 
        new NativeWindowSettings
        {
            Size = (width, height),Title = title,
            WindowState = WindowState.Normal
        })
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
        //define resolution
        _riView = new RIView(new Vector2i(width, height));
        RIWorld.Instance.RIView = _riView;
        // Load main riScene
        RIWorld.Instance.LoadScene(AppDomain.CurrentDomain.BaseDirectory + "resources\\sample.riScene");
    }

    protected override void OnResize(ResizeEventArgs e)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        RIWorld.Instance.Initialize();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        RIWorld.Instance.DestroyWorld();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        Input = KeyboardState;
        RIWorld.Instance.UpdateWorld();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        RIWorld.Instance.RenderWorld();
        SwapBuffers();
    }

    static void Main()
    {
        using (TestProgram gameProc = new TestProgram(1280, 720, "RIEngine_RenderTest"))
        {
            gameProc.Run();
        }
    }
}