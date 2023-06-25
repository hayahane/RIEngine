using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
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

        var jTransform = jObject.SelectToken("Transform") as JObject;
        var transGuid = new Guid(jTransform?.GetValue("Guid")?.Value<string>()!);
        RIObject riObject = new RIObject(guid, transGuid);
        riObject.Name = name;
        GuidReferenceHelper.GuidReferenceMap.TryAdd(guid, riObject);

        riObject.Parent = riObject.Parent;
        riObject.Transform.Position = jTransform!.GetValue("Position")!.ToObject<Vector3>(serializer);
        riObject.Transform.Rotation = jTransform!.GetValue("Rotation")!.ToObject<Quaternion>(serializer);
        riObject.Transform.Scale = jTransform!.GetValue("Scale")!.ToObject<Vector3>(serializer);

        foreach (var property in objectType.GetProperties())
        {
            if (property.Name == "Name") continue;
            if (!property.CanWrite ||
                property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length != 0) continue;
            var propertyValue = jObject.GetValue(property.Name)!;
            if (propertyValue.Type == JTokenType.Null)
                propertyValue = null;
            if (propertyValue != null)
                property.SetValue(riObject, propertyValue.ToObject(property.PropertyType, serializer));
            else
                property.SetValue(riObject, null);
        }

        return riObject;
    }

    public override bool CanConvert(Type objectType)
    {
        return false; //objectType == typeof(RIObject);
    }
}