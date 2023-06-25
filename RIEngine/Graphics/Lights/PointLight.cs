using System.Drawing;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using RIEngine.Core;
namespace RIEngine.Graphics;

public class PointLight : BaseLight
{
    public PointLight(RIObject riObject, Guid guid) : base(riObject, guid)
    {
    }
    public PointLight(RIObject riObject) : base(riObject)
    {
    }
    public float Range { get; set; } = 3f;
}