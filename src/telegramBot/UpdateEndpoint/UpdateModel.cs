using System.Text.Json.Serialization;

namespace telegramBot.Models;

public class UpdateModel
{
    [JsonPropertyName("update_id")]
    public long Id { get; set; }
    [JsonPropertyName("message")]
    public Message Msg { get; set; }
}
