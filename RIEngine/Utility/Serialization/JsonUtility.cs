using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;

using RIEngine.Core;

namespace RIRenderer.Serialization;

public class OpenTkVector3Converter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var vector = (Vector3)value!;
        writer.WriteStartArray();
        writer.WriteValue(vector.X);
        writer.WriteValue(vector.Y);
        writer.WriteValue(vector.Z);
        writer.WriteEndArray();
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        var obj = JToken.Load(reader);
        if (obj.Type == JTokenType.Array)
        {
            var arr = (JArray)obj;
            if (arr.Count == 3 && arr.All(token => token.Type == JTokenType.Float))
            {
                return new Vector3(arr[0].Value<float>(), arr[1].Value<float>(), arr[2].Value<float>());
            }
        }

        return null;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector3);
    }
}

public class QuaternionConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var quat = (Quaternion)value!;
        writer.WriteStartArray();
        writer.WriteValue(quat.X);
        writer.WriteValue(quat.Y);
        writer.WriteValue(quat.Z);
        writer.WriteValue(quat.W);
        writer.WriteEndArray();
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        var obj = JToken.Load(reader);
        if (obj.Type == JTokenType.Array)
        {
            var arr = (JArray)obj;
            if (arr.Count == 4 && arr.All(token => token.Type == JTokenType.Float))
            {
                return new Quaternion(arr[0].Value<float>(), arr[1].Value<float>(), arr[2].Value<float>(),
                    arr[3].Value<float>());
            }
        }

        return null;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Quaternion);
    }
}

public class RenderObjectConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value != null)
        {
            var jo = new JObject();
            
            Type type = value!.GetType();
            jo.AddFirst(new JProperty("$type", type.FullName));
            
            //jo.Add(new JProperty("$id",serializer.ReferenceResolver.GetReference(serializer, value)));
            foreach (var property in type.GetProperties())
            {
                if (property.CanRead && property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length == 0)
                {
                    object propertyValue = property.GetValue(value, null)!;
                    jo.Add(property.Name, JToken.FromObject(propertyValue,serializer));
                }
            }
            
            jo.WriteTo(writer);
        }
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var jObject = serializer.Deserialize<JObject>(reader);
        var type = jObject!.GetValue("$type")!.Value<string>();

        if (type != null)
        {
            Type t = Type.GetType(type)!;
            if (t.FullName != null)
            {
                var obj = t.Assembly.CreateInstance(t.FullName);
                foreach (var property in t.GetProperties())
                {
                    if (property.CanWrite && property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length == 0)
                    {
                        var propertyValue = jObject.GetValue(property.Name);
                        if (propertyValue != null)
                        {
                            property.SetValue(obj, propertyValue.ToObject(property.PropertyType, serializer));
                        }
                    }
                }
                return obj;
            }
        }

        return null;
    }

    public override bool CanConvert(Type objectType)
    {
        return (objectType.IsSubclassOf(typeof(RIObject)) || objectType == typeof(RIObject));
    }
}