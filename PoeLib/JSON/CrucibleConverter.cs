using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PoeLib.JSON;

public class CrucibleConverter : JsonConverter<List<Node>>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return base.CanConvert(typeToConvert);
    }
    public override List<Node> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var doc = JsonDocument.ParseValue(ref reader);
        var json = doc.RootElement.GetRawText();
        try
        {
            var nodes = JsonSerializer.Deserialize<Nodes>(json);
            return nodes.GetAllNodes().ToList();
        }
        catch 
        {
            return JsonSerializer.Deserialize<List<Node>>(json);
        }
    }

    public override void Write(Utf8JsonWriter writer, List<Node> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
