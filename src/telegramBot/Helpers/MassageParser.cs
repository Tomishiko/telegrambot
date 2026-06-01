using System.Text.RegularExpressions;


namespace telegramBot.Helpers;

public static partial class RegexPatterns
{

    // Matches standard http/https links as well as standalone "www." links,
    // stopping when it hits whitespace or common HTML/JSON delimiters.
    //[GeneratedRegex(@"(?:https?:\/\/|www\.)[^\s""'>]+", RegexOptions.IgnoreCase)]
    [GeneratedRegex("""(?:https?:\/\/|www\.)[^\s"'>]+?(?=[.,!?;:’"']*(?:\s|$))""", RegexOptions.IgnoreCase)]
    public static partial Regex GeneralUrlPattern();

    [GeneratedRegex("""(?:https?:\/\/)?share\.google\/[a-zA-Z0-9_-]+""", RegexOptions.IgnoreCase)]
    public static partial Regex GoogleSharePattern();
}
public static partial class StringExtensions
{

    public static IEnumerable<string> ExtractUrls(this string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return Enumerable.Empty<string>();
        }

        var matches = RegexPatterns.GeneralUrlPattern().Matches(message);

        return matches
            .Select(m => m.Value)
            .Select(url => url.StartsWith("www.", StringComparison.OrdinalIgnoreCase) ? $"https://{url}" : url)
            .Distinct();
    }
}
