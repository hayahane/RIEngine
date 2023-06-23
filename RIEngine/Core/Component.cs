namespace RIEngine.Core;

public abstract class Component
{
    public RIObject RIObject { get; }
    
    public Component(RIObject riObject)
    {
        RIObject = riObject;
    }
}