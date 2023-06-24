namespace RIEngine.Core;

public class SerializableObject
{
    public Guid Guid { get; }
    
    public SerializableObject()
    {
        Guid = Guid.NewGuid();
    }

    public SerializableObject(Guid guid)
    {
        Guid = guid;
    }
}