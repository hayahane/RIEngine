using OpenTK.Windowing.GraphicsLibraryFramework;
using RIEngine.Core;
using RIEngine.Graphics;

namespace RIEngine.Test;

public class OpenAndCloseLight : ActorScript
{
    public OpenAndCloseLight(RIObject riObject, Guid guid) : base(riObject, guid)
    { }
    
    public OpenAndCloseLight(RIObject riObject) : base(riObject)
    { }

    private DirectionalLight _light;
    public override void OnEnable()
    {
        base.OnEnable();
        _light = this.RIObject.GetComponent<DirectionalLight>();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        KeyboardState input = TestProgram.Input;

        if (input.IsKeyPressed(Keys.O))
        {
            _light.IsEnabled = !_light.IsEnabled;
        }
        
    }
}