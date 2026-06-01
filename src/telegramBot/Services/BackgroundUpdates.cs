namespace telegramBot.Services;

public class BackgroundUpdates : BackgroundService
{
    private IServiceProvider _services;
    private ILogger<BackgroundService> _logger;

    public BackgroundUpdates(IServiceProvider services, ILogger<BackgroundService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _services.CreateAsyncScope();
        var service = scope.ServiceProvider
                           .GetRequiredService<SubprocessAsyncHelper>();
        await Update(service);
        using var timer = new PeriodicTimer(TimeSpan.FromHours(6));
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await Update(service);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Background update is stopped");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected error when trying to update ");
        }
    }
    private async Task Update(SubprocessAsyncHelper subproc)
    {
        await subproc.ExecuteShellCommand("-U", 60000);
    }
}
