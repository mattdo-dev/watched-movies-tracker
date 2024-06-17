using System.Globalization;
using System.Text;

namespace DatabaseService.Utils;

public class NormalizeTitle
{
    public static string Normalize(string input)
    {
        var normalized = input.Trim();
        normalized = normalized.ToLowerInvariant();
        normalized = RemoveDiacritics(normalized);
        return normalized;
    }

    private static string RemoveDiacritics(string input)
    {
        var normalized = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalized)
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