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
    public TestProgram(int width, int height, string title) : base(
        GameWindowSettings.Default, new NativeWindowSettings{Size = (width, height),Title = title})
    {
        _riView = new RIView(new Vector2i(width, height));
        RIWorld.Instance.RIView = _riView;
        RIWorld.Instance.LoadScene(@"E:\Monologist\CsProjects\RIEngine\RIEngine\Assets", "sample.riScene");

        /*var camera = RIObject.Spawn().AddComponent<Camera>();
        camera.RIObject.Name = "camera";
        camera.CastMode = CastMode.Perspective;
        camera.RIObject.Transform.Position = new Vector3(0, 0, 0);
        var controller = camera.RIObject.AddComponent<CharacterMove>();

        
        var monkeyHead = RIObject.Spawn().AddComponent<MeshRenderer>();
        monkeyHead.RIObject.Name = "monkeyHead";
        monkeyHead.MeshPath = @"E:\Monologist\CsProjects\RIEngine\RIEngine\Library\ModelAsset\monkeyHead.modelAsset";
        monkeyHead.FragPath = @"E:\Monologist\CsProjects\RIEngine\RIEngine\Assets\Shaders\BasicUnlit.frag";
        monkeyHead.VertPath = @"E:\Monologist\CsProjects\RIEngine\RIEngine\Assets\Shaders\BasicUnlit.vert";
        monkeyHead.TexturePath = @"E:\Monologist\CsProjects\RIEngine\RIEngine\Assets\Textures\OIP.jpg";
        monkeyHead.RIObject.Transform.LocalRotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(45f));
        monkeyHead.RIObject.Transform.Position = new Vector3(0, 0, -5);
        
        var c = new RIObject("cube", monkeyHead.RIObject);
        var cube = c.AddComponent<MeshRenderer>();
        cube.MeshPath = @"E:\Monologist\CsProjects\RIEngine\RIEngine\Library\ModelAsset\cube.modelAsset";
        cube.FragPath = @"E:\Monologist\CsProjects\RIEngine\RIEngine\Assets\Shaders\BasicUnlit.frag";
        cube.VertPath = @"E:\Monologist\CsProjects\RIEngine\RIEngine\Assets\Shaders\BasicUnlit.vert";
        cube.TexturePath = @"E:\Monologist\CsProjects\RIEngine\RIEngine\Assets\Textures\Otto.jpeg";
        cube.RIObject.Transform.Position = new Vector3(-2, 0, -7);*/

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            NullValueHandling = NullValueHandling.Include,

            //Converters
            Converters =
            {
                new OpenTkVector3Converter(), new QuaternionConverter(),
                new ComponentConverter(), new OpenTkVector2iConverter()
            }
        };

        string jsonData = JsonConvert.SerializeObject(RIWorld.Instance.RIObjects);
        File.WriteAllText(@"E:\Monologist\CsProjects\RIEngine\RIEngine\Assets\sample.riScene", jsonData);
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