namespace telegramBot.Endpoints;

using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using telegramBot.Core;
using telegramBot.Models;
using telegramBot.Services;

public static class WebhookCtrlEndpoint
{


    public static async Task WebhookCtrl(bool enabled, HttpClient client, HttpContext context, IOptions<ApiOptions> options)
    {
        var apiConfig = options.Value;
        apiConfig.ThrowIfBadConfig();

        string url = apiConfig.HookUrl;
        string token = apiConfig.botToken;
        string apiHost = apiConfig.BaseUrl;


        if (!enabled) url = string.Empty;


        using var content = JsonContent
            .Create(new WebhookRequestModel(url),
                    AppJsonContext.Default.WebhookRequestModel,
                    new MediaTypeHeaderValue("application/json"));


        var response = await client.PostAsync($"{apiHost}/{token}/setWebhook", content);

        foreach (var header in response.Headers)
            context.Response.Headers[header.Key] = header.Value.ToArray();

        foreach (var header in response.Content.Headers)
            context.Response.Headers[header.Key] = header.Value.ToArray();

        // Some headers like transfer-encoding might conflict
        context.Response.Headers.Remove("transfer-encoding");

        await response.Content.CopyToAsync(context.Response.Body);

    }
}

