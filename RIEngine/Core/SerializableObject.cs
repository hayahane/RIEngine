using RIEngine.Utility.Serialization;

namespace RIEngine.Core;

public class SerializableObject
{
    public Guid Guid { get; }
    
    public SerializableObject()
    {
        Guid = Guid.NewGuid();
        GuidReferenceHelper.GuidReferenceMap.Add(Guid, this);
    }

    public SerializableObject(Guid guid)
    {
        Guid = guid;
    }
}