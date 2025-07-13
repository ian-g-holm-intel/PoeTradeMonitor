using System;
using PoeLib;

namespace PoeCrafter;

public class NotEnoughCurrencyException : Exception
{
    public CurrencyType CurrencyType { get; set; }
    public NotEnoughCurrencyException(CurrencyType type) : base($"Not enough currency of type: {type}")
    {
        CurrencyType = type;
    }
}

public class NotEnoughCurrencyToRareException : Exception
{
    public NotEnoughCurrencyToRareException() : base($"Not enough currency left to make item rare") { }
}

public class ModsNotFoundException : Exception
{
    public ModsNotFoundException() : base("Mods could not be found, make sure you're in the right location") { }
}

public class CurrencyNotFoundException : Exception
{
    public CurrencyNotFoundException() : base("Currency could not be found, make sure you're in the right location") { }
}
