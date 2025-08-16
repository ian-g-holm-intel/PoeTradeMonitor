using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PoeLib.JSON.Poe2Scout;

public class ItemMetadata
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("base_type")]
    public string? BaseType { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("stack_size")]
    public int StackSize { get; set; }

    [JsonPropertyName("max_stack_size")]
    public int MaxStackSize { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("effect")]
    public List<string>? Effect { get; set; }
}