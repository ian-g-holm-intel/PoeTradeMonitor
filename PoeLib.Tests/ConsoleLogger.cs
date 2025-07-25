﻿using Microsoft.Extensions.Logging;
using System;

namespace PoeLib.Tests;

public class ConsoleLogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: [{logLevel}] {formatter(state, exception)}");
    }
}
