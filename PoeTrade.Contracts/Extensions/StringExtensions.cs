using System.Globalization;
using System.Text;

namespace PoeTrade.Contracts.Extensions;

/// <summary>
/// Extension methods for string manipulation in the PoeTrade.Contracts library.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Removes diacritical marks (accents) from a string to normalize text for comparison.
    /// </summary>
    /// <param name="text">The text to normalize.</param>
    /// <returns>The text with diacritical marks removed.</returns>
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
}
