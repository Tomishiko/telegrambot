using Microsoft.Extensions.Options;
using telegramBot.Core;
using telegramBot.Helpers;
using telegramBot.Models;

namespace telegramBot.Services;

public class MessageSender
{
    readonly ILogger<MessageSender> _logger;
    readonly HttpClient _client;
    readonly IOptions<ApiOptions> _options;

    public MessageSender(SubprocessAsyncHelper subprocess,
                      ILogger<MessageSender> logger,
                      HttpClient client,
                      IOptions<ApiOptions> options)
    {
        _logger = logger;
        _client = client;
        _options = options;
    }
    public async Task<(bool, string?)> SendFileAsync(Stream stream,
                                          string fname,
                                          string audioType,
                                          long chat_id,
                                          CancellationToken ct = default)
    {
        ApiOptions options = _options.Value;
        try
        {
            using var multipart = new MultipartFormDataContent();
            var chatiIdSection = new StringContent(chat_id.ToString());

            //Correct extension if we fallbacked to mp4
            if (ReferenceEquals(audioType, AudioFormats.Default)) fname = $"{fname}.opus";
            else fname = $"{fname}.mp4";

            var cdHeader = $"form-data; name=\"document\"; filename=\"{Uri.EscapeDataString(fname)}\"";
            var fsContent = new StreamContent(stream)
            {
                Headers =
                {
                    {"Content-Type", $"audio/{audioType}"},
                    {"Content-Disposition", cdHeader},
                },
            };

            multipart.Add(chatiIdSection, "chat_id");
            multipart.Add(fsContent);

            var apiUrl = $"{options.BaseUrl}/{options.botToken}";
            _logger.LogInformation($"sending post to {apiUrl}/sendDocument");

            var response = await _client.PostAsync($"{apiUrl}/sendDocument", multipart);
            var responseMsg = await response.Content.ReadAsStringAsync();

            if (responseMsg is not null && response.IsSuccessStatusCode)
            {
                _logger.LogInformation(responseMsg);
            }

            return (response.IsSuccessStatusCode, responseMsg);

        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Error when trying to send file {fname}");
            return (false, ex.ToString());
        }

    }
    public async Task SendMessage(long chat_id, string message)
    {
        var payload = new TextMessage
        {
            ChatId = chat_id,
            Text = message
        };
        ApiOptions options = _options.Value;
        var apiUrl = $"{options.BaseUrl}/{options.botToken}";

        var response = await _client.PostAsJsonAsync($"{apiUrl}/SendMessage",
                                      payload,
                                      AppJsonContext.Default.TextMessage);
    }

}

