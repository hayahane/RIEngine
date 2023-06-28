using OpenTK.Windowing.GraphicsLibraryFramework;
using RIEngine.Core;
using RIEngine.Graphics;

namespace RIEngine.Test;

public class OpenAndCloseLight : ActorScript
{
    #region Constructors

    public OpenAndCloseLight(RIObject riObject, Guid guid) : base(riObject, guid)
    {
    }

    public OpenAndCloseLight(RIObject riObject) : base(riObject)
    {
    }

    #endregion

    private DirectionalLight? _light;

    public override void OnEnable()
    {
        base.OnEnable();
        _light = this.RIObject.GetComponent<DirectionalLight>();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        KeyboardState? input = TestProgram.Input;

        if (input != null && input.IsKeyPressed(Keys.O) && _light != null)
        {
            _light.IsEnabled = !_light.IsEnabled;
        }
    }
}