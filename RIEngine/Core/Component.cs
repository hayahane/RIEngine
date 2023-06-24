using System.Text.Json.Serialization;

namespace RIEngine.Core;

public class Component : SerializableObject
{
    public RIObject RIObject { get; }
    
    public Component(RIObject riObject) : base()
    {
        RIObject = riObject;
    }
    
    [JsonConstructor]
    public Component(RIObject riObject, Guid guid):base(guid)
    {
        RIObject = riObject;
    }
}