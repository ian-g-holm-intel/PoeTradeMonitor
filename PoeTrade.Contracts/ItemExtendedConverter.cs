using System.Text.Json;
using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Custom JSON converter for ItemExtended objects that handles cases where the JSON 
/// contains an empty array instead of an object for the extended property.
/// </summary>
public class ItemExtendedConverter : JsonConverter<ItemExtended?>
{
    public override ItemExtended? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            // Handle case where extended is an empty array []
            // Skip the entire array
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                // Skip array contents
            }
            return null;
        }
        
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            // Use default serialization without custom converters
            return JsonSerializer.Deserialize<ItemExtended>(ref reader);
        }
        
        throw new JsonException("Expected object, array, or null for ItemExtended");
    }

    public override void Write(Utf8JsonWriter writer, ItemExtended? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            // Use default serialization without custom converters
            JsonSerializer.Serialize(writer, value);
        }
    }
}