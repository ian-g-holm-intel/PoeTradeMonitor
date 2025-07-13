using System;
using System.Collections.Generic;

namespace PoeLib.JSON.Currency;

public class CurrencyExchangeResponse
{
    public string id { get; set; }
    public string complexity { get; set; }
    public Dictionary<string, Result> result { get; set; }
    public int total { get; set; }
}

public class Account
{
    public string name { get; set; }
    public Online online { get; set; }
    public string lastCharacterName { get; set; }
    public string language { get; set; }
    public string realm { get; set; }
}

public class Exchange
{
    public string currency { get; set; }
    public int amount { get; set; }
    public string whisper { get; set; }
}

public class Item
{
    public string currency { get; set; }
    public int amount { get; set; }
    public int stock { get; set; }
    public string id { get; set; }
    public string whisper { get; set; }
}

public class Listing
{
    public DateTime indexed { get; set; }
    public Account account { get; set; }
    public List<Offer> offers { get; set; }
    public string whisper { get; set; }
    public string whisper_token { get; set; }
}

public class Offer
{
    public Exchange exchange { get; set; }
    public Item item { get; set; }
}

public class Online
{
    public string league { get; set; }
}

public class Result
{
    public string id { get; set; }
    public object item { get; set; }
    public Listing listing { get; set; }
}


