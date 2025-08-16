using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Custom JSON converter for TradeCurrencyType enum that maps JSON string values 
/// to enum values using the TradeCurrencyTypeAttribute.
/// </summary>
public class TradeCurrencyTypeConverter : JsonConverter<TradeCurrencyType>
{
    private static readonly Dictionary<string, TradeCurrencyType> StringToEnumMap = new();
    private static readonly Dictionary<TradeCurrencyType, string> EnumToStringMap = new();
    
    static TradeCurrencyTypeConverter()
    {
        InitializeMappings();
    }

    /// <summary>
    /// Initializes the bidirectional mappings between enum values and their attribute tag values.
    /// </summary>
    private static void InitializeMappings()
    {
        var enumType = typeof(TradeCurrencyType);
        var enumValues = Enum.GetValues<TradeCurrencyType>();

        foreach (var enumValue in enumValues)
        {
            var fieldInfo = enumType.GetField(enumValue.ToString());
            if (fieldInfo != null)
            {
                var attribute = fieldInfo.GetCustomAttribute<TradeCurrencyTypeAttribute>();
                if (attribute != null)
                {
                    StringToEnumMap[attribute.Tag] = enumValue;
                    EnumToStringMap[enumValue] = attribute.Tag;
                }
            }
        }
    }

    /// <summary>
    /// Reads a JSON string value and converts it to the corresponding TradeCurrencyType enum value.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert to.</param>
    /// <param name="options">JSON serializer options.</param>
    /// <returns>The TradeCurrencyType enum value corresponding to the JSON string.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value is not a valid currency type.</exception>
    public override TradeCurrencyType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string value for TradeCurrencyType, got {reader.TokenType}");
        }

        var stringValue = reader.GetString();
        if (string.IsNullOrEmpty(stringValue))
        {
            throw new JsonException("TradeCurrencyType value cannot be null or empty");
        }

        if (StringToEnumMap.TryGetValue(stringValue, out var enumValue))
        {
            return enumValue;
        }

        // Return Unknown for unrecognized values (like "~price", "~b/o", etc.)
        // These are pricing modes, not currency types
        return TradeCurrencyType.Unknown;
    }

    /// <summary>
    /// Writes a TradeCurrencyType enum value as its corresponding JSON string value.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The TradeCurrencyType value to write.</param>
    /// <param name="options">JSON serializer options.</param>
    /// <exception cref="JsonException">Thrown when the enum value doesn't have a corresponding attribute.</exception>
    public override void Write(Utf8JsonWriter writer, TradeCurrencyType value, JsonSerializerOptions options)
    {
        if (EnumToStringMap.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            // Fallback to "unknown" for any unmapped enum values
            writer.WriteStringValue("unknown");
        }
    }

    /// <summary>
    /// Gets the string representation of a TradeCurrencyType enum value using its attribute.
    /// </summary>
    /// <param name="currencyType">The TradeCurrencyType enum value.</param>
    /// <returns>The string representation from the TradeCurrencyTypeAttribute, or null if not found.</returns>
    public static string? GetStringValue(TradeCurrencyType currencyType)
    {
        return EnumToStringMap.TryGetValue(currencyType, out var stringValue) ? stringValue : null;
    }

    /// <summary>
    /// Gets the TradeCurrencyType enum value from its string representation.
    /// </summary>
    /// <param name="stringValue">The string value to convert.</param>
    /// <returns>The corresponding TradeCurrencyType enum value, or null if not found.</returns>
    public static TradeCurrencyType? GetEnumValue(string stringValue)
    {
        return StringToEnumMap.TryGetValue(stringValue, out var enumValue) ? enumValue : null;
    }
}