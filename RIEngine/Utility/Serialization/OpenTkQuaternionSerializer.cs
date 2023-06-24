using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;

namespace RIEngine.Utility.Serialization;

public class OpenTkQuaternionSerializer : JsonConverter
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