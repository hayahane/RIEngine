using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;

namespace RIEngine.Utility.Serialization;

// ReSharper disable once InconsistentNaming
public class OpenTkVector2iSerializer : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var vector = (Vector2i)value!;
            writer.WriteStartArray();
            writer.WriteValue(vector.X);
            writer.WriteValue(vector.Y);
            writer.WriteEndArray();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            var obj = JToken.Load(reader);
            if (obj.Type == JTokenType.Array)
            {
                var arr = (JArray)obj;
                if (arr.Count == 2 && arr.All(token => token.Type == JTokenType.Integer))
                {
                    return new Vector2i(arr[0].Value<int>(), arr[1].Value<int>());
                }
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2i);
        }
}