using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RIEngine.Core;

namespace RIEngine.Utility.Serialization;

public class ComponentSerializer : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value != null)
        {
            var jo = new JObject();
            var obj = value as Component;

            var type = value.GetType();
            jo.AddFirst(new JProperty("$type", type.FullName));

            foreach (var property in type.GetProperties())
            {
                if (!property.CanRead ||
                    property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length != 0) continue;

                var propertyValue = property.GetValue(value, null);
                if (propertyValue == null)
                {
                    jo.Add(property.Name, JValue.CreateNull());
                    continue;
                }

                SerializableObject? so = propertyValue as SerializableObject;
                jo.Add(property.Name,
                    so != null
                        ? JToken.FromObject(so.Guid.ToString(), serializer)
                        : JToken.FromObject(propertyValue, serializer));
            }

            jo.WriteTo(writer);
        }
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);
        var componentGuid = new Guid(jObject.GetValue("Guid")!.Value<string>()!);
        var guid = new Guid(jObject.GetValue("RIObject")!.Value<string>()!);
        var type = Type.GetType(jObject.GetValue("$type")!.Value<string>()!)!;

        if (GuidReferenceHandler.GuidReferenceMap.TryGetValue(guid, out var riObject))
        {
            var component = Activator.CreateInstance(type, riObject, componentGuid)!;
            foreach (var property in type.GetProperties())
            {
                if (!property.CanWrite ||
                    property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length != 0) continue;

                var propertyValue = jObject.GetValue(property.Name)!;

                if (property.PropertyType.IsSubclassOf(typeof(SerializableObject)))
                {
                    var so = new SerializableObject(new Guid(propertyValue.Value<string>()!));
                    property.SetValue(component, so);
                }
                else
                {
                    if (!propertyValue.HasValues && propertyValue.Type != JTokenType.String) 
                        propertyValue = null;
                    property.SetValue(component,
                        propertyValue != null ? propertyValue.ToObject(property.PropertyType, serializer) : null);
                }
                
            }

            return component;
        }

        return null;
    }

    public override bool CanConvert(Type objectType)
    {
        return (objectType.IsSubclassOf(typeof(Component)) || objectType == typeof(Component));
    }
}