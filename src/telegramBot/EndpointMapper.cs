using Microsoft.Extensions.Options;
using telegramBot.Models;

namespace telegramBot.Endpoints;

public static class EndpointMapper
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapPost("/updates", UpdatesEndpoint.PostNewUpdate);

        var ctrlGroup = app.MapGroup("/ctrl");
        ctrlGroup.MapGet("/setWebhook", WebhookCtrlEndpoint.WebhookCtrl);

    }
}
