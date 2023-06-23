using RIEngine.Core.Graphics;
using RIEngine.Patterns;

namespace RIEngine.Core;

public class RIScene : Singleton<RIScene>
{
    RenderView RenderView { get; set; }
    public List<RIObject> RIObjects { get; set; } = new List<RIObject>();
    public void LoadScene(string path)
    {
        throw new NotImplementedException();
    }
}