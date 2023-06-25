using RIEngine.Core;
using OpenTK.Mathematics;
using Newtonsoft.Json;

namespace RIEngine.Graphics;

public class BaseLight : Behaviour
{
    public BaseLight(RIObject riObject, Guid guid) : base(riObject, guid)
    {
    }
    public BaseLight(RIObject riObject) : base(riObject)
    {
    }
    
    public Color4 LightColor { get; set; } = Color4.White;
    public float LightIntensity { get; set; } = 1f;

    [JsonIgnore]
    public Color4 FinalColor => new Color4(LightColor.R * LightIntensity,
        LightColor.G * LightIntensity, LightColor.B * LightIntensity,LightColor.A);
}