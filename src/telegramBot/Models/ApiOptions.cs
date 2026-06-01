namespace telegramBot.Models;

public class ApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string botToken { get; set; } = string.Empty;
    public string HookUrl { get; set; } = string.Empty;

    public void ThrowIfBadConfig()
    {
        ArgumentException.ThrowIfNullOrEmpty(BaseUrl, nameof(BaseUrl));
        ArgumentException.ThrowIfNullOrEmpty(botToken, nameof(botToken));
        ArgumentException.ThrowIfNullOrEmpty(HookUrl, nameof(HookUrl));

    }

}
