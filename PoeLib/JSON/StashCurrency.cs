using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PoeLib.JSON;

public class ItemLocation
{
    public double x { get; set; }
    public double y { get; set; }
    public int w { get; set; }
    public int h { get; set; }
}

public class CurrencyLayout
{
    [JsonPropertyName("0")]
    public ItemLocation Currency0 { get; set; }
    [JsonPropertyName("1")]
    public ItemLocation Currency1 { get; set; }
    [JsonPropertyName("2")]
    public ItemLocation Currency2 { get; set; }
    [JsonPropertyName("3")]
    public ItemLocation Currency3 { get; set; }
    [JsonPropertyName("4")]
    public ItemLocation Currency4 { get; set; }
    [JsonPropertyName("5")]
    public ItemLocation Currency5 { get; set; }
    [JsonPropertyName("6")]
    public ItemLocation Currency6 { get; set; }
    [JsonPropertyName("7")]
    public ItemLocation Currency7 { get; set; }
    [JsonPropertyName("8")]
    public ItemLocation Currency8 { get; set; }
    [JsonPropertyName("9")]
    public ItemLocation Currency9 { get; set; }
    [JsonPropertyName("10")]
    public ItemLocation Currency10 { get; set; }
    [JsonPropertyName("11")]
    public ItemLocation Currency11 { get; set; }
    [JsonPropertyName("12")]
    public ItemLocation Currency12 { get; set; }
    [JsonPropertyName("13")]
    public ItemLocation Currency13 { get; set; }
    [JsonPropertyName("14")]
    public ItemLocation Currency14 { get; set; }
    [JsonPropertyName("15")]
    public ItemLocation Currency15 { get; set; }
    [JsonPropertyName("16")]
    public ItemLocation Currency16 { get; set; }
    [JsonPropertyName("17")]
    public ItemLocation Currency17 { get; set; }
    [JsonPropertyName("18")]
    public ItemLocation Currency18 { get; set; }
    [JsonPropertyName("19")]
    public ItemLocation Currency19 { get; set; }
    [JsonPropertyName("20")]
    public ItemLocation Currency20 { get; set; }
    [JsonPropertyName("21")]
    public ItemLocation Currency21 { get; set; }
    [JsonPropertyName("22")]
    public ItemLocation Currency22 { get; set; }
    [JsonPropertyName("23")]
    public ItemLocation Currency23 { get; set; }
    [JsonPropertyName("24")]
    public ItemLocation Currency24 { get; set; }
    [JsonPropertyName("25")]
    public ItemLocation Currency25 { get; set; }
    [JsonPropertyName("26")]
    public ItemLocation Currency26 { get; set; }
    [JsonPropertyName("27")]
    public ItemLocation Currency27 { get; set; }
    [JsonPropertyName("28")]
    public ItemLocation Currency28 { get; set; }
    [JsonPropertyName("30")]
    public ItemLocation Currency30 { get; set; }
    [JsonPropertyName("31")]
    public ItemLocation Currency31 { get; set; }
    [JsonPropertyName("32")]
    public ItemLocation Currency32 { get; set; }
    [JsonPropertyName("33")]
    public ItemLocation Currency33 { get; set; }
    [JsonPropertyName("34")]
    public ItemLocation Currency34 { get; set; }
    [JsonPropertyName("35")]
    public ItemLocation Currency35 { get; set; }
    [JsonPropertyName("36")]
    public ItemLocation Currency36 { get; set; }
    [JsonPropertyName("37")]
    public ItemLocation Currency37 { get; set; }
    [JsonPropertyName("38")]
    public ItemLocation Currency38 { get; set; }
    [JsonPropertyName("39")]
    public ItemLocation Currency39 { get; set; }
    [JsonPropertyName("40")]
    public ItemLocation Currency40 { get; set; }
    [JsonPropertyName("41")]
    public ItemLocation Currency41 { get; set; }
    [JsonPropertyName("42")]
    public ItemLocation Currency42 { get; set; }
    [JsonPropertyName("43")]
    public ItemLocation Currency43 { get; set; }
    [JsonPropertyName("44")]
    public ItemLocation Currency44 { get; set; }
    [JsonPropertyName("45")]
    public ItemLocation Currency45 { get; set; }
    [JsonPropertyName("46")]
    public ItemLocation Currency46 { get; set; }
    [JsonPropertyName("47")]
    public ItemLocation Currency47 { get; set; }
    [JsonPropertyName("48")]
    public ItemLocation Currency48 { get; set; }
    [JsonPropertyName("49")]
    public ItemLocation Currency49 { get; set; }
    [JsonPropertyName("50")]
    public ItemLocation Currency50 { get; set; }
    [JsonPropertyName("51")]
    public ItemLocation Currency51 { get; set; }
    [JsonPropertyName("52")]
    public ItemLocation Currency52 { get; set; }
    [JsonPropertyName("53")]
    public ItemLocation Currency53 { get; set; }
    [JsonPropertyName("54")]
    public ItemLocation Currency54 { get; set; }
}

public class StashCurrency
{
    public int numTabs { get; set; }
    public List<Item> items { get; set; }
    public CurrencyLayout currencyLayout { get; set; }
}
