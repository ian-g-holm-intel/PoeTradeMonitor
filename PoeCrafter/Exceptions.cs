using System;

namespace PoeCrafter;

public class NotEnoughCurrencyException : Exception
{
    public TradeCurrencyType TradeCurrencyType { get; set; }
    public NotEnoughCurrencyException(TradeCurrencyType type) : base($"Not enough currency of type: {type}")
    {
        TradeCurrencyType = type;
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
