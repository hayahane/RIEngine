using Newtonsoft.Json;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RIEngine.BasicComponents.Movement;
using RIEngine.Core;
using RIEngine.Graphics;
using RIEngine.Utility.Serialization;

namespace RIEngine.Test;

public class TestProgram : GameWindow
{
    private RIView _riView;
    public static KeyboardState Input;
    private int m = 0;
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
        _riView = new RIView(new Vector2i(width, height));
        RIWorld.Instance.RIView = _riView;
        Console.WriteLine();
        RIWorld.Instance.LoadScene("/Users/hayahane/RiderProjects/RIEngine/RIEngine/Assets/sample.riScene");
        
        
        var camera = RIObject.Spawn().AddComponent<Camera>();
        camera.RIObject.Name = "camera";
        camera.CastMode = CastMode.Perspective;
        camera.RIObject.Transform.Position = new Vector3(0, 0, 0);
        camera.RIObject.AddComponent<CharacterMove>();

        var monkeyHead = RIObject.Spawn().AddComponent<MeshRenderer>();
        monkeyHead.RIObject.Name = "monkeyHead";
        monkeyHead.MeshPath =
            "/Users/hayahane/RiderProjects/RIEngine/RIEngine/Library/ModelAsset/monkeyHead.modelAsset";
        monkeyHead.VertPath =
            "/Users/hayahane/RiderProjects/RIEngine/RIEngine/Assets/Shaders/BasicUnlit.vert";
        monkeyHead.FragPath =
            "/Users/hayahane/RiderProjects/RIEngine/RIEngine/Assets/Shaders/BasicUnlit.frag";
        monkeyHead.TexturePath = 
            "/Users/hayahane/RiderProjects/RIEngine/RIEngine/Assets/Textures/OIP.jpg";
        monkeyHead.RIObject.Transform.LocalRotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(45f));
        monkeyHead.RIObject.Transform.Position = new Vector3(0, 0, -5);
        
        var c = new RIObject("cube", monkeyHead.RIObject);
        var cube = c.AddComponent<MeshRenderer>();
        cube.MeshPath =
            "/Users/hayahane/RiderProjects/RIEngine/RIEngine/Library/ModelAsset/cube.modelAsset";
        cube.VertPath =
            "/Users/hayahane/RiderProjects/RIEngine/RIEngine/Assets/Shaders/BasicUnlit.vert";
        cube.FragPath =
            "/Users/hayahane/RiderProjects/RIEngine/RIEngine/Assets/Shaders/BasicUnlit.frag";
        cube.TexturePath = 
            "/Users/hayahane/RiderProjects/RIEngine/RIEngine/Assets/Textures/Otto.jpeg";
        cube.RIObject.Transform.Position = new Vector3(2, 0, -7);

        var floor = RIObject.Spawn();
        var fm = floor.AddComponent<MeshRenderer>();
        fm.MeshPath =
            "/Users/hayahane/RiderProjects/RIEngine/RIEngine/Library/ModelAsset/cube.modelAsset";
        fm.VertPath =
            "/Users/hayahane/RiderProjects/RIEngine/RIEngine/Assets/Shaders/BasicUnlit.vert";
        fm.FragPath =
            "/Users/hayahane/RiderProjects/RIEngine/RIEngine/Assets/Shaders/BasicUnlit.frag";
        fm.TexturePath = 
            "/Users/hayahane/RiderProjects/RIEngine/RIEngine/Assets/Textures/Otto.jpeg";
        
        floor.Transform.Scale = new Vector3(10, 0.1f, 10);
        floor.Transform.Position = new Vector3(0, -1, 0);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        RIWorld.Instance.Initialize();
        
        var dl = RIObject.Spawn().AddComponent<DirectionalLight>();
        dl.RIObject.Name = "Direction Light";
        dl.RIObject.Tag = "Light";
        dl.LightColor = new Color4(1f, 0.8f, 0.8f, 1);
        dl.LightIntensity = 1f;

        var pl = RIObject.Spawn().AddComponent<PointLight>();
        pl.RIObject.Name = "Point Light";
        pl.RIObject.Tag = "Light";
        pl.LightColor = new Color4(1f, 0f, 1f, 1f);
        pl.LightIntensity = 5f;
        pl.Range = 3f;
        pl.RIObject.Transform.Position = new Vector3(0, 0, 0);
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
        if (m == 0)
        {
            string json = JsonConvert.SerializeObject(RIWorld.Instance.WorldRoot);
            File.WriteAllText("/Users/hayahane/RiderProjects/RIEngine/RIEngine/Assets/sample.riScene", json);
            m = 1;
        }
        
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