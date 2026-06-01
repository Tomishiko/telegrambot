using System.Diagnostics;

namespace telegramBot.Models;

public class SubprocessStream : IDisposable
{
    private readonly Process _proc;
    public readonly Stream Stream;
    public int ExitCode { get => _proc.ExitCode; }
    public Task<string> Output { get => _proc.StandardError.ReadToEndAsync(); }

    public SubprocessStream(Stream processStream, Process proc)
    {
        Stream = processStream;
        _proc = proc;
    }
    public async Task WaitForExitAsync(int timeout)
    {
        var ct = new CancellationTokenSource(timeout);
        await _proc.WaitForExitAsync(ct.Token);
    }

    public void Dispose()
    {
        if (!_proc.HasExited) _proc.Kill();
        _proc.Dispose();

    }
}
