using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RIEngine.Core;

namespace RIEngine.Utility.Serialization;

public class RIObjectSerializer : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null) return;

        var jo = new JObject();
        var obj = value as RIObject;

        foreach (var property in value.GetType().GetProperties())
        {
            if (!property.CanRead ||
                property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length != 0) continue;

            var propertyValue = property.GetValue(value, null);
            if (propertyValue != null)
                jo.Add(property.Name, JToken.FromObject(propertyValue, serializer));
            else
                jo.Add(property.Name, JValue.CreateNull());
        }

        jo.WriteTo(writer);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        object? obj = null;
        var jObject = JObject.Load(reader);
        var guid = new Guid(jObject.GetValue("Guid")!.Value<string>()!);
        var name = jObject.GetValue("Name")!.Value<string>()!;

        var transform = jObject.SelectToken("Transform") as JObject;
        var transGuid = new Guid(transform?.GetValue("Guid")?.Value<string>()!);
        RIObject riObject = new RIObject(guid, transGuid);
        riObject.Name = name;
        GuidReferenceHandler.GuidReferenceMap.TryAdd(guid, riObject);


        foreach (var property in objectType.GetProperties())
        {
            if (!property.CanWrite ||
                property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length != 0) continue;
            if (property.Name == "Transform" || property.Name == "Name") continue;

            if (!property.PropertyType.IsSubclassOf(typeof(Component)))
            {
                var propertyValue = jObject.GetValue(property.Name)!;
                if (!propertyValue.HasValues && propertyValue.Type != JTokenType.String) 
                    propertyValue = null;
                if (propertyValue != null)
                    property.SetValue(riObject, propertyValue.ToObject(property.PropertyType, serializer));
                else
                    property.SetValue(riObject, null);
            }
            else
            {
                var componentGuid = new Guid(jObject.GetValue("Guid")!.Value<string>()!);
                var component = Activator.CreateInstance(property.PropertyType, this, componentGuid)!;
                riObject.Components.Add(component as Component);
                GuidReferenceHandler.GuidReferenceMap.TryAdd(componentGuid, component!);

                foreach (var soProperty in property.PropertyType.GetProperties())
                {
                    if (!soProperty.CanWrite ||
                        soProperty.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length != 0) continue;

                    var propertyValue = jObject.GetValue(soProperty.Name);
                    soProperty.SetValue(component, propertyValue);
                }
            }
        }

        return riObject;
    }

    public override bool CanConvert(Type objectType)
    {
        return false; //objectType == typeof(RIObject);
    }
}