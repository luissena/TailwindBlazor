using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TailwindBlazor;

public sealed class TailwindHostedService : IHostedService, IDisposable
{
    private readonly TailwindOptions _options;
    private readonly IHostEnvironment _env;
    private readonly ILogger<TailwindHostedService> _logger;
    private Process? _process;

    public TailwindHostedService(
        IOptions<TailwindOptions> options,
        IHostEnvironment env,
        ILogger<TailwindHostedService> logger)
    {
        _options = options.Value;
        _env = env;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_env.IsDevelopment())
        {
            _logger.LogDebug("TailwindBlazor: Watch mode skipped (not Development environment)");
            return;
        }

        await TailwindCliDownloader.EnsureCliAsync(_options, _logger, cancellationToken);

        var cliPath = TailwindCliDownloader.ResolveCliPath(_options);
        var contentRoot = _env.ContentRootPath;
        var inputPath = Path.Combine(contentRoot, _options.InputFile);
        var outputPath = Path.Combine(contentRoot, _options.OutputFile);

        var outputDir = Path.GetDirectoryName(outputPath);
        if (outputDir != null)
            Directory.CreateDirectory(outputDir);

        _logger.LogInformation("TailwindBlazor: Starting watch mode ({Input} -> {Output})", _options.InputFile, _options.OutputFile);

        var psi = new ProcessStartInfo
        {
            FileName = cliPath,
            Arguments = $"-i \"{inputPath}\" -o \"{outputPath}\" --watch",
            WorkingDirectory = contentRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _process = new Process { StartInfo = psi, EnableRaisingEvents = true };
        _process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
                _logger.LogDebug("[tailwindcss] {Line}", e.Data);
        };
        _process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
                _logger.LogWarning("[tailwindcss] {Line}", e.Data);
        };

        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        _logger.LogInformation("TailwindBlazor: Watch mode started (PID {Pid})", _process.Id);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_process is null || _process.HasExited)
            return Task.CompletedTask;

        _logger.LogInformation("TailwindBlazor: Stopping watch mode (PID {Pid})", _process.Id);

        try
        {
            _process.Kill(entireProcessTree: true);
        }
        catch (InvalidOperationException)
        {
            // Process already exited
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_process is null)
            return;

        if (!_process.HasExited)
        {
            try { _process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) { }
        }

        _process.Dispose();
        _process = null;
    }
}
