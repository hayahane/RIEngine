using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;

namespace RIEngine.Utility.Serialization;

public class OpenTkVector3Serializer : JsonConverter
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