using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PoeTrade.Contracts;

/// <summary>
/// Custom JSON converter that converts between JSON string arrays and Mod objects.
/// Parses modifier strings to extract numeric values and create structured Mod objects.
/// </summary>
public class ModListConverter : JsonConverter<List<Mod>?>
{
    private static readonly Regex NumberPattern = new(@"[\d]+(?:\.[\d]+)?", RegexOptions.Compiled);

    public override List<Mod>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException($"Expected StartArray token, got {reader.TokenType}");
        }

        var mods = new List<Mod>();
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }
            
            if (reader.TokenType == JsonTokenType.String)
            {
                var modString = reader.GetString();
                if (!string.IsNullOrEmpty(modString))
                {
                    mods.Add(ParseModString(modString));
                }
            }
        }

        return mods;
    }

    public override void Write(Utf8JsonWriter writer, List<Mod>? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        
        foreach (var mod in value)
        {
            writer.WriteStringValue(mod.RawModText);
        }
        
        writer.WriteEndArray();
    }

    /// <summary>
    /// Parses multiple modifier strings to create a list of Mod objects.
    /// This method is used internally by the converter and for protobuf conversions.
    /// </summary>
    /// <param name="modStrings">The collection of modifier strings to parse.</param>
    /// <returns>A list of Mod objects with parsed values and processed text.</returns>
    public static List<Mod> ParseModStrings(IEnumerable<string> modStrings)
    {
        return modStrings
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(ParseModString)
            .ToList();
    }
    
    /// <summary>
    /// Parses a modifier string to create a Mod object with extracted values.
    /// </summary>
    /// <param name="modString">The raw modifier string to parse.</param>
    /// <returns>A Mod object with parsed values and processed text.</returns>
    public static Mod ParseModString(string modString)
    {
        var values = new List<double>();
        var modText = modString;
        
        // Extract numeric values and replace them with placeholders
        while (NumberPattern.IsMatch(modText))
        {
            var match = NumberPattern.Match(modText);
            if (double.TryParse(match.Value, out var value))
            {
                values.Add(value);
            }
            modText = NumberPattern.Replace(modText, "#", 1);
        }
        
        // Clean up the mod text (remove trailing question marks)
        modText = modText.Trim('?');
        
        return new Mod
        {
            RawModText = modString,
            ModText = modText,
            Values = values,
            ValueString = values.Count > 0 ? string.Join(" ", values) : null
        };
    }
}