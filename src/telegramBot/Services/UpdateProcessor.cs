using telegramBot.Models;
using telegramBot.Helpers;

namespace telegramBot.Services;

public class UpdateProcessor
{
    readonly ILogger<UpdateProcessor> _logger;
    readonly MessageSender _fileSender;
    readonly SubprocessAsyncHelper _subproc;

    public UpdateProcessor(SubprocessAsyncHelper subproc,
                           MessageSender fileSender,
                           ILogger<UpdateProcessor> logger)
    {
        _subproc = subproc;
        _fileSender = fileSender;
        _logger = logger;
    }
    public async Task UpdateHandler(Message msg,string link)
    {
        _logger.LogInformation($"recived reqest from {msg.From.Username}: {link}");
        string vidUrl = link;
        string fname;

        // get name of video
        var result = await _subproc.ExecuteShellCommand($"--get-filename -o \"%(title)s\" \"{vidUrl}\"", 50000);
        if (result.ExitCode == 0) fname = result.Output;
        else
        {
            _logger.LogWarning($"failed to get files name {result.Output}");
            fname = "defaultName";
        }

        var attempts = new (string Args, bool RunUpdate, string AudioFormat)[]
        {
            ("-x", false,AudioFormats.Default),
            ("-x", true,AudioFormats.Default),
            ("--format m4a", false,AudioFormats.Fallback)
        };

        foreach (var (args, updateFirst, format) in attempts)
        {
            if (updateFirst)
            {
                _logger.LogInformation("Beginning update for yt-dlp");
                await TryUpdateYtdlp();
            }

            if (await TryPipeStream(args, fname, vidUrl, msg.Chat.Id, format))
                return;

            _logger.LogWarning($"failed to pipe stram with params: args:{args},updateFirst:{updateFirst},format:{format}");
        }
        _logger.LogError("Filed all retries to upload");

    }
    private async Task TryUpdateYtdlp()
    {
        var result = await _subproc.ExecuteShellCommand("-U", 50000);
        _logger.LogInformation($"Finished update for yt-dlp(exit code {result.ExitCode}): {result.Output}");
    }
    private async Task<bool> TryPipeStream(string extractionArgs, string fname, string vidUrl, long chatid, string audioFormat)
    {
        bool status = true;

        // Get stream
        try
        {
            _logger.LogInformation($"Opening stream pipe");
            using SubprocessStream subStream = _subproc.GetSubprocessStream($"{extractionArgs} -o - --no-warnings \"{vidUrl}\"");
            (bool isSuccess, string? responseMsg) = await _fileSender.SendFileAsync(subStream.Stream,
                                                                  fname,
                                                                  audioFormat,
                                                                  chatid);
            _logger.LogInformation("Waiting for subproc");
            await subStream.WaitForExitAsync(1000);

            if (subStream.ExitCode != 0)
            {
                _logger.LogError($"{await subStream.Output}");
                status = false;
            }
            if (!isSuccess)
            {
                _logger.LogError(responseMsg);
                status = false;
            }

            _logger.LogInformation("Successfully finished subproc");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error whe trying to pipe stream");
            return false;
        }

        return status;
    }
}
