using System.Text.Json;
using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

public class ItemPropertyValuesConverter : JsonConverter<List<ItemPropertyValue>>
{
    public override List<ItemPropertyValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException();

        var values = new List<ItemPropertyValue>();

        reader.Read(); // Move to first array element
        while (reader.TokenType != JsonTokenType.EndArray)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            reader.Read(); // Move to first element of inner array
            var value = reader.GetString();
            reader.Read(); // Move to second element
            var index = reader.GetInt32();
            reader.Read(); // Move to end of inner array

            values.Add(new ItemPropertyValue { Value = value, Index = index });

            reader.Read(); // Move to next array element or end
        }

        return values;
    }

    public override void Write(Utf8JsonWriter writer, List<ItemPropertyValue> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var prop in value)
        {
            writer.WriteStartArray();
            writer.WriteStringValue(prop.Value);
            writer.WriteNumberValue(prop.Index);
            writer.WriteEndArray();
        }
        writer.WriteEndArray();
    }
}
