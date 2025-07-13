using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using PoeLib.JSON.PoeNinja;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace PoeLib;

public static class ExtensionMethods
{
    public static WebProxy GetProxy(this string serverAddress, string port, string username, string password)
    {
        var proxyAddress = new Uri($"socks5://{serverAddress}:{port}/");
        return new WebProxy(proxyAddress, true, new string[0], new NetworkCredential(username, password));
    }

    public static bool Volatile(this Sparkline history)
    {
        var data = history.data;
        if (!data[data.Count - 1].HasValue || !data[data.Count - 2].HasValue || data[data.Count - 2].Value == 0 || data.Count < 2)
            return false;
        var delta = data[data.Count - 1].Value - data[data.Count - 2].Value;
        return delta < -20 || delta > 40;
    }

    public static bool Volatile(this List<decimal?> history)
    {
        if (history.Count < 2 || !history[history.Count - 1].HasValue || !history[history.Count - 2].HasValue || history[history.Count - 2].Value == 0)
            return false;
        var delta = 100 * (history[history.Count - 1].Value / history[history.Count - 2].Value - 1);
        return delta < -20 || delta > 40;
    }

    public static string RemoveDiacritics(this string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static void Clear<T>(this ConcurrentQueue<T> queue)
    {
        while (!queue.IsEmpty)
            queue.TryDequeue(out _);
    }

    public static Task StartSTATask(this Task task)
    {
        TaskCompletionSource<object> source = new TaskCompletionSource<object>();
        Thread thread = new Thread(() =>
        {
            try
            {
                task.Wait();
                source.SetResult(null);
            }
            catch (Exception ex)
            {
                source.SetException(ex);
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return source.Task;
    }

    public static Task<T> StartSTATask<T>(this Task<T> task)
    {
        TaskCompletionSource<T> source = new TaskCompletionSource<T>();
        Thread thread = new Thread(() =>
        {
            try
            {
                var result = task.Result;
                source.SetResult(result);
            }
            catch (Exception ex)
            {
                source.SetException(ex);
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return source.Task;
    }

    public static async Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken)
    {
        RegisteredWaitHandle registeredHandle = null;
        CancellationTokenRegistration tokenRegistration = default(CancellationTokenRegistration);
        try
        {
            var tcs = new TaskCompletionSource<bool>();
            registeredHandle = ThreadPool.RegisterWaitForSingleObject(
                handle,
                (state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
                tcs,
                millisecondsTimeout,
                true);
            tokenRegistration = cancellationToken.Register(
                state => ((TaskCompletionSource<bool>)state).TrySetCanceled(),
                tcs);
            return await tcs.Task;
        }
        finally
        {
            if (registeredHandle != null)
                registeredHandle.Unregister(null);
            tokenRegistration.Dispose();
        }
    }

    public static Task<bool> WaitOneAsync(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken)
    {
        return handle.WaitOneAsync((int)timeout.TotalMilliseconds, cancellationToken);
    }

    public static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken)
    {
        return handle.WaitOneAsync(Timeout.Infinite, cancellationToken);
    }

    public static bool WaitOne(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken)
    {
        int n = WaitHandle.WaitAny(new[] { handle, cancellationToken.WaitHandle }, millisecondsTimeout);
        switch (n)
        {
            case WaitHandle.WaitTimeout:
                return false;
            case 0:
                return true;
            default:
                cancellationToken.ThrowIfCancellationRequested();
                return false; // never reached
        }
    }

    public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
    {
        return source.Skip(Math.Max(0, source.Count() - N));
    }

    public static string GetCurrencyDescription(this CurrencyType currency)
    {
        var fi = currency.GetType().GetField(currency.ToString());
        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

        return attributes.Length <= 0 ? currency.ToString() : attributes[0].Description;
    }

    public static Point GetCurrencyLocation(this CurrencyType currency, Rectangle clientBounds)
    {
        var fi = currency.GetType().GetField(currency.ToString());
        var attributes = (LocationAttribute[])fi.GetCustomAttributes(typeof(LocationAttribute), false);

        if(attributes.Length == 0)
            return new Point();

        foreach (var attrib in attributes)
        {
            if (attrib.Screen.Width == clientBounds.Width && attrib.Screen.Height == clientBounds.Height)
                return new Point(attrib.Screen.X, attrib.Screen.Y);
        }
        return new Point();
    }

    public static int GetCurrencyStackSize(this CurrencyType currency)
    {
        var fi = currency.GetType().GetField(currency.ToString());
        var attributes = (StackSizeAttribute[])fi.GetCustomAttributes(typeof(StackSizeAttribute), false);

        return attributes.Length > 0 ? attributes[0].Size : 0;
    }

    public static bool IsSelling(this CurrencyType currency)
    {
        var fi = currency.GetType().GetField(currency.ToString());
        var attributes = (SellingAttribute[])fi.GetCustomAttributes(typeof(SellingAttribute), false);

        return attributes.Length > 0;
    }

    public static bool IsBuying(this CurrencyType currency)
    {
        var fi = currency.GetType().GetField(currency.ToString());
        var attributes = (BuyingAttribute[])fi.GetCustomAttributes(typeof(BuyingAttribute), false);

        return attributes.Length > 0;
    }

    public static int GetBuyingStockSize(this CurrencyType currency)
    {
        var fi = currency.GetType().GetField(currency.ToString());
        var attributes = (BuyingAttribute[])fi.GetCustomAttributes(typeof(BuyingAttribute), false);

        return attributes.Length > 0 ? attributes[0].StockSize : 0;
    }

    public static int GetSellingStockSize(this CurrencyType currency)
    {
        var fi = currency.GetType().GetField(currency.ToString());
        var attributes = (SellingAttribute[])fi.GetCustomAttributes(typeof(SellingAttribute), false);

        return attributes.Length > 0 ? attributes[0].StockSize : 0;
    }

    public static int GetCurrencyStatcks(this Currency currency)
    {
        var stacks = (int)currency.Amount / currency.Type.GetCurrencyStackSize();
        var stackRemainder = (int)currency.Amount % currency.Type.GetCurrencyStackSize();
        return stackRemainder == 0 ? stacks : stacks + 1;
    }

    public static string GetCurrencyTradeName(this CurrencyType currency)
    {
        var fi = currency.GetType().GetField(currency.ToString());
        var attributes = (TradeNameAttribute[])fi.GetCustomAttributes(typeof(TradeNameAttribute), false);

        return attributes.Length > 0 ? attributes[0].Name : currency.ToString();
    }

    public static decimal GetFraction(this decimal input)
    {
        return input - Math.Truncate(input);
    }

    public static CurrencyType GetCurrencyType(this string currencyString)
    {
        return ((CurrencyType[]) Enum.GetValues(typeof(CurrencyType))).FirstOrDefault(currencyType => currencyString.Equals(currencyType.GetCurrencyDescription(), StringComparison.InvariantCultureIgnoreCase) || currencyString.Equals(currencyType.GetCurrencyTradeName(), StringComparison.InvariantCultureIgnoreCase));
    }

    public static bool ShouldTrade(this CurrencyPrice p)
    {
        if (p.BuyFraction.N == p.SellFraction.D && p.BuyFraction.D == p.SellFraction.N)
            return false;

        if (p.AvgPrice > 1)
        {
            if (p.BuyFraction.InverseDecimalValue > p.SellFraction.DecimalValue)
                return false;
        }
        else
        {
            if (p.SellFraction.InverseDecimalValue > p.BuyFraction.DecimalValue)
                return false;
        }
        return true;
    }

    public static bool HasHundredsDecimal(this decimal value)
    {
        return ((value * 10) - Math.Truncate(value * 10)) != 0;
    }

    public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask)
    {
        byte[] ipAdressBytes = address.GetAddressBytes();
        byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

        if (ipAdressBytes.Length != subnetMaskBytes.Length)
            throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

        byte[] broadcastAddress = new byte[ipAdressBytes.Length];
        for (int i = 0; i < broadcastAddress.Length; i++)
        {
            broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
        }
        return new IPAddress(broadcastAddress);
    }

    public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
    {
        byte[] ipAdressBytes = address.GetAddressBytes();
        byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

        if (ipAdressBytes.Length != subnetMaskBytes.Length)
            throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

        byte[] broadcastAddress = new byte[ipAdressBytes.Length];
        for (int i = 0; i < broadcastAddress.Length; i++)
        {
            broadcastAddress[i] = (byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
        }
        return new IPAddress(broadcastAddress);
    }

    public static bool IsInSameSubnet(this IPAddress address2, IPAddress address, IPAddress subnetMask)
    {
        IPAddress network1 = address.GetNetworkAddress(subnetMask);
        IPAddress network2 = address2.GetNetworkAddress(subnetMask);

        return network1.Equals(network2);
    }

    public static Fraction ToFraction(this decimal d)
    {
        if (d < 1)
        {
            decimal inverse = Math.Round(1 / d, 1);
            if(inverse % 1 == 0)
                return new Fraction(1, decimal.ToInt32(inverse));
            return new Fraction(10, decimal.ToInt32(inverse*10));
        }

        var rounded = Math.Round(d, 1);
        if(rounded % 1 == 0)
            return new Fraction(decimal.ToInt32(rounded), 1);
        return new Fraction(decimal.ToInt32(rounded*10), 10);
    }

    public static string ShardIdsToNextChangeId(this long[] shardIds)
    {
        return shardIds.Aggregate("", (current, shardid) => current + (shardid + "-")).TrimEnd('-');
    }

    public static long[] NextChangeIdToShardIds(this string changeIdString)
    {
        return (from string shardId in changeIdString.Split('-') select long.Parse(shardId)).ToArray();
    }

    public static KeyValuePair<string, string>[] ToKeyValuePairs(this string parameters)
    {
        var paramArray = parameters.Split('&');
        var paramList = new List<KeyValuePair<string, string>>();
        foreach (var parameter in paramArray)
        {
            var paramPair = parameter.Split('=');
            paramList.Add(new KeyValuePair<string, string>(paramPair[0], paramPair[1]));
        }
        return paramList.ToArray();
    }

    public static SocketColor GetSocketColor(this string socketText)
    {
        switch (socketText)
        {
            case "socketD":
                return SocketColor.G;
            case "socketI":
                return SocketColor.B;
            case "socketS":
                return SocketColor.R;
            case "socketG":
                return SocketColor.W;
            case "socketA":
                return SocketColor.A;
            default:
                return SocketColor.W;
        }
    }

    public static StringContent ToStringContent(this object request, out string json)
    {
        var jsonSerializerOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        json = JsonSerializer.Serialize(request, jsonSerializerOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
