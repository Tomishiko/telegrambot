namespace telegramBot.Endpoints;

using Microsoft.AspNetCore.Mvc.Routing;
using telegramBot.Helpers;
using telegramBot.Models;
using telegramBot.Services;

public partial class UpdatesEndpoint
{
    private const int retryTimes = 3;
    private const int retryDelay = 2500;
    private static readonly string[] allowedHosts = ["www.youtube.com", "youtube.com", "youtu.be", "yt.be"];

    public static async Task<IResult> PostNewUpdate(UpdateModel update,
                                                    UpdateProcessor updateProcessor,
                                                    MessageSender sender,
                                                    ILogger<UpdatesEndpoint> logger,
                                                    IYoutubeExtractorService youtubeExtractor)
    {
        if (string.IsNullOrEmpty(update?.Msg?.Text) || update.Msg.Chat?.Id is null)
            return Results.BadRequest();

        var links = update.Msg.Text.ExtractUrls();

        foreach (var link in links)
        {
            string? youtubeLink;

            if (link.Contains("share.google", StringComparison.OrdinalIgnoreCase))
            {

                youtubeLink = (await youtubeExtractor.ExtractYoutubeLinksAsync(link)).FirstOrDefault();
            }
            else
            {
                youtubeLink = link;
            }

            if (youtubeLink is null)
            {

                await sender.SendMessage(update.Msg.Chat.Id, "Youtube link list is empty");
                logger.LogCritical("The youtube link list was empty original msg is {Msg}", update.Msg.Text);
                return Results.Ok();
            }

            await updateProcessor.UpdateHandler(update.Msg,youtubeLink);
        }
        return Results.Ok();
    }
}

