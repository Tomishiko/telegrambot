//********************************
//https://gist.github.com/AlexMAS
//********************************
namespace telegramBot.Services;
using System.Diagnostics;
using System.Text;
using telegramBot.Models;

public class SubprocessAsyncHelper
{
    readonly ILogger<SubprocessAsyncHelper> _logger;
    readonly string _procName;

    public SubprocessAsyncHelper(ILogger<SubprocessAsyncHelper> logger, string procName = "yt-dlp")
    {
        _logger = logger;
        _procName = procName;
    }

    public SubprocessStream GetSubprocessStream(string args, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo
        {
            FileName = _procName,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        var process = new Process() { StartInfo = psi };
        process.Start();

        return new SubprocessStream(process.StandardOutput.BaseStream, process);

    }
    public async Task<ProcessResult> ExecuteShellCommand(string arguments, int timeout)
    {
        var result = new ProcessResult();

        using (var process = new Process())
        {
            // If you run bash-script on Linux it is possible that ExitCode can be 255.
            // To fix it you can try to add '#!/bin/bash' header to the script.

            process.StartInfo.FileName = _procName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            var outputBuilder = new StringBuilder();
            var outputCloseEvent = new TaskCompletionSource<bool>();

            process.OutputDataReceived += (s, e) =>
                                            {
                                                // The output stream has been closed i.e. the process has terminated
                                                if (e.Data == null)
                                                {
                                                    outputCloseEvent.SetResult(true);
                                                }
                                                else
                                                {
                                                    outputBuilder.AppendLine(e.Data);
                                                }
                                            };

            var errorBuilder = new StringBuilder();
            var errorCloseEvent = new TaskCompletionSource<bool>();

            process.ErrorDataReceived += (s, e) =>
                                            {
                                                // The error stream has been closed i.e. the process has terminated
                                                if (e.Data == null)
                                                {
                                                    errorCloseEvent.SetResult(true);
                                                }
                                                else
                                                {
                                                    errorBuilder.AppendLine(e.Data);
                                                }
                                            };

            bool isStarted;

            try
            {
                isStarted = process.Start();
            }
            catch (Exception error)
            {
                // Usually it occurs when an executable file is not found or is not executable

                result.Completed = true;
                result.ExitCode = -1;
                result.Output = error.Message;

                isStarted = false;
            }

            if (isStarted)
            {
                // Reads the output stream first and then waits because deadlocks are possible
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Creates task to wait for process exit using timeout
                var waitForExit = WaitForExitAsync(process, timeout);

                // Create task to wait for process exit and closing all output streams
                var processTask = Task.WhenAll(waitForExit, outputCloseEvent.Task, errorCloseEvent.Task);

                // Waits process completion and then checks it was not completed by timeout
                if (await Task.WhenAny(Task.Delay(timeout), processTask) == processTask && waitForExit.Result)
                {
                    result.Completed = true;
                    result.ExitCode = process.ExitCode;

                    // Adds process output if it was completed with error
                    if (process.ExitCode != 0)
                    {
                        result.Output = $"{outputBuilder}{errorBuilder}";
                    }
                    else
                        result.Output = $"{outputBuilder}";
                }
                else
                {
                    try
                    {
                        // Kill hung process
                        process.Kill();
                    }
                    catch
                    {
                    }
                }
            }
        }

        return result;
    }


    private Task<bool> WaitForExitAsync(Process process, int timeout)
    {
        return Task.Run(() => process.WaitForExit(timeout));
    }


    public struct ProcessResult
    {
        public bool Completed;
        public int? ExitCode;
        public string Output;
    }
}
