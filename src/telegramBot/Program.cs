using System.Text.Json.Serialization;
using telegramBot.Endpoints;
using telegramBot.Models;
using telegramBot.Services;

namespace telegramBot.Core;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            //options.SerializerOptions.TypeInfoResolverChain.Add(new DefaultJsonTypeInfoResolver());
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default);
        });

        builder.Services.AddHttpClient();
        builder.Services.AddHealthChecks();
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("Api"));
        builder.Services.AddHostedService<BackgroundUpdates>();
        builder.Services.AddTransient<MessageSender>();
        builder.Services.AddTransient<UpdateProcessor>();
        builder.Services.AddTransient<SubprocessAsyncHelper>();
        builder.Services.AddTransient<IYoutubeExtractorService, YoutubeExtractorService>();


        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            //app.MapOpenApi();
        }

        app.MapEndpoints();
        app.MapHealthChecks("/health");
        app.Run();
    }
}
[JsonSerializable(typeof(WebhookRequestModel))]
[JsonSerializable(typeof(UpdateModel))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(Message))]
[JsonSerializable(typeof(Chat))]
[JsonSerializable(typeof(TextMessage))]
public partial class AppJsonContext : JsonSerializerContext
{

}

