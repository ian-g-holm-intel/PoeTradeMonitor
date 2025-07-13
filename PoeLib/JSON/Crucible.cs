using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PoeLib.JSON;

public class Crucible
{
    public string layout { get; set; }
    [JsonConverter(typeof(CrucibleConverter))]
    public List<Node> nodes { get; set; }
}

public class Nodes
{
    [JsonPropertyName("0")]
    public Node Node0 { get; set; }

    [JsonPropertyName("1")]
    public Node Node1 { get; set; }

    [JsonPropertyName("2")]
    public Node Node2 { get; set; }

    [JsonPropertyName("3")]
    public Node Node3 { get; set; }

    [JsonPropertyName("4")]
    public Node Node4 { get; set; }

    [JsonPropertyName("5")]
    public Node Node5 { get; set; }

    [JsonPropertyName("6")]
    public Node Node6 { get; set; }

    [JsonPropertyName("7")]
    public Node Node7 { get; set; }

    [JsonPropertyName("8")]
    public Node Node8 { get; set; }

    [JsonPropertyName("9")]
    public Node Node9 { get; set; }

    [JsonPropertyName("10")]
    public Node Node10 { get; set; }

    [JsonPropertyName("11")]
    public Node Node11 { get; set; }

    [JsonPropertyName("12")]
    public Node Node12 { get; set; }

    [JsonPropertyName("13")]
    public Node Node13 { get; set; }

    [JsonPropertyName("14")]
    public Node Node14 { get; set; }

    public IEnumerable<Node> GetAllNodes()
    {
        if (Node0 != null) yield return Node0;
        if (Node1 != null) yield return Node1;
        if (Node2 != null) yield return Node2;
        if (Node3 != null) yield return Node3;
        if (Node4 != null) yield return Node4;
        if (Node5 != null) yield return Node5;
        if (Node6 != null) yield return Node6;
        if (Node7 != null) yield return Node7;
        if (Node8 != null) yield return Node8;
        if (Node9 != null) yield return Node9;
        if (Node10 != null) yield return Node10;
        if (Node11 != null) yield return Node11;
        if (Node12 != null) yield return Node12;
        if (Node13 != null) yield return Node13;
        if (Node14 != null) yield return Node14;
    }
}

public class Node
{
    public string name => (Enumerable.Range(0, orbit).Sum() + orbitIndex).ToString();
    public int orbit { get; set; }
    public int orbitIndex { get; set; }
    public string icon { get; set; }
    public bool isReward { get; set; }
    public bool allocated { get; set; }
    public List<string> stats { get; set; }
    public List<string> @in { get; set; }
    public List<string> @out { get; set; }
    public int skill { get; set; }
    public int tier { get; set; }
}
