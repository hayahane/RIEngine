using RIEngine.Patterns;

namespace RIEngine.Utility.Serialization;

public static class GuidReferenceHandler
{
    public static Dictionary<Guid, object> GuidReferenceMap { get; } = new Dictionary<Guid, object>();

    public static bool TryAddReference(object obj, out Guid? guid)
    {
        if (GuidReferenceMap.Values.Contains(obj))
        {
            guid = GuidReferenceMap.First(pair => pair.Value == obj).Key;
            return true;
        }

        guid = null;
        return false;
    }
}