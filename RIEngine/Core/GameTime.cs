using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RIEngine.Core;

public class GameTime
{
    public float TimeScale { get; set; } = 1f;
    public float UnscaledDeltaTime { get; set; }
    public float DeltaTime => UnscaledDeltaTime * TimeScale;
    private double LastTime { get; set; }

    public GameTime()
    {
        LastTime = GLFW.GetTime();
    }

    public void UpdateTime()
    {
        double tmpTime = GLFW.GetTime();
        UnscaledDeltaTime = (float)(tmpTime - LastTime);
        LastTime = tmpTime;
    }
}