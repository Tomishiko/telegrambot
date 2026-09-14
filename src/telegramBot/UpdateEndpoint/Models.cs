namespace telegramBot.Models;

using System.Diagnostics;
using System.Text.Json.Serialization;

public class Message
{
    [JsonPropertyName("message_id")]
    public long Id { get; set; }
    [JsonPropertyName("from")]
    public User From { get; set; }
    [JsonPropertyName("date")]
    public long Date { get; set; }//unix time
    [JsonPropertyName("chat")]
    public Chat Chat { get; set; }
    [JsonPropertyName("text")]
    public string Text { get; set; }
}

public class User
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("is_bot")]
    public bool IsBot { get; set; }
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }
    [JsonPropertyName("username")]
    public string? Username { get; set; }
}

public class Chat
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; } // "private", "group", "supergroup", "channel"
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("username")]
    public string? Username { get; set; }
}

public record WebhookRequestModel(string url, string? ip_address = null);

