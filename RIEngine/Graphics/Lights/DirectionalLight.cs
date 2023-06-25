using OpenTK.Mathematics;
using RIEngine.Core;

namespace RIEngine.Graphics;

public class DirectionalLight : BaseLight
{
    public DirectionalLight(RIObject riObject, Guid guid) : base(riObject, guid)
    {
    }

    public DirectionalLight(RIObject riObject) : base(riObject)
    {
    }
    
    public Vector3 Direction{get; set;} = new Vector3(0f, -1f, 0f);
}