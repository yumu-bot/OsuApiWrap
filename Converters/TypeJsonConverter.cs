using Newtonsoft.Json;

namespace OsuApi.Converters;

public class TypeJsonConverter : JsonConverter<Type>
{
    public override void WriteJson(JsonWriter writer, Type? value, JsonSerializer serializer)
    {
        writer.WriteValue(value?.FullName);
    }

    public override Type? ReadJson(JsonReader reader, Type objectType, Type? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var typeName = reader.Value?.ToString();
        return typeName == null ? null : Type.GetType(typeName);
    }
}
