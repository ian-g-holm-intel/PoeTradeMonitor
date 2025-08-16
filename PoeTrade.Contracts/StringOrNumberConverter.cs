using System.Text.Json;
using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Custom JSON converter that handles JSON values that can be either strings or numbers,
/// converting them all to string representation for consistent handling.
/// </summary>
public class StringOrNumberConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString() ?? string.Empty,
            JsonTokenType.Number => reader.GetDecimal().ToString(),
            JsonTokenType.Null => string.Empty,
            _ => throw new JsonException($"Cannot convert token type {reader.TokenType} to string")
        };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}