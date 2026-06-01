using System.Text.Json.Serialization;

namespace telegramBot.Models;

public class TextMessage
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }
}
