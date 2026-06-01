using System.Text.RegularExpressions;

namespace telegramBot.Services;

public interface IYoutubeExtractorService
{
    Task<IEnumerable<string>> ExtractYoutubeLinksAsync(string googleShareUrl);
}

public partial class YoutubeExtractorService : IYoutubeExtractorService
{
    private readonly HttpClient _httpClient;

    [GeneratedRegex(@"(?:https?:)?\/\/(?:www\.)?(?:youtube\.com\/watch\?v=|youtu\.be\/|youtube\.com\/embed\/)[a-zA-Z0-9_-]+", RegexOptions.IgnoreCase)]
    private static partial Regex YoutubePattern();

    public YoutubeExtractorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<string>> ExtractYoutubeLinksAsync(string googleShareUrl)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, googleShareUrl);
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string rawHtml = await response.Content.ReadAsStringAsync();

        string decodedHtml = Uri.UnescapeDataString(rawHtml);

        var matches = YoutubePattern().Matches(decodedHtml);

        return matches
            .Select(m => m.Value)
            .Select(url => url.StartsWith("//") ? $"https:{url}" : url)
            .Distinct();
    }
}
