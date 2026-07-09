using System.Diagnostics;
using System.Net.Http;
using Microsoft.Playwright;
using Xunit;

namespace Microsoft.eShopWeb.EndToEndTests.Playwright;

public sealed class BrowserFixture : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private Process? _webProcess;
    private IPlaywright? _playwright;

    public IBrowser Browser { get; private set; } = default!;
    public string BaseUrl { get; private set; } = string.Empty;

    public BrowserFixture()
    {
        _httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        })
        {
            Timeout = TimeSpan.FromSeconds(1)
        };
    }

    public async ValueTask InitializeAsync()
    {
        BaseUrl = await StartWebAppAsync();

        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (Browser is not null)
        {
            await Browser.DisposeAsync();
        }

        _playwright?.Dispose();

        _httpClient.Dispose();

        if (_webProcess is { HasExited: false })
        {
            _webProcess.Kill(entireProcessTree: true);
            await _webProcess.WaitForExitAsync();
        }
    }

    private async Task<string> StartWebAppAsync()
    {
        const string baseUrl = "https://localhost:5001";
        string solutionRoot = FindSolutionRoot();
        string webProject = Path.Combine(solutionRoot, "src", "Web", "Web.csproj");

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --no-build --project \"{webProject}\"",
            WorkingDirectory = solutionRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        _webProcess = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start Web app process.");

        await WaitForServerAsync(baseUrl, TimeSpan.FromSeconds(90));
        return baseUrl;
    }

    private async Task WaitForServerAsync(string baseUrl, TimeSpan timeout)
    {
        DateTimeOffset deadline = DateTimeOffset.UtcNow.Add(timeout);

        while (DateTimeOffset.UtcNow < deadline)
        {
            try
            {
                if (_webProcess is { HasExited: true })
                {
                    string errorOutput = await _webProcess.StandardError.ReadToEndAsync();
                    throw new InvalidOperationException($"Web app exited during startup. {errorOutput}".Trim());
                }

                using HttpResponseMessage response = await _httpClient.GetAsync(baseUrl);
                if ((int)response.StatusCode > 0)
                {
                    return;
                }
            }
            catch
            {
                // App is still starting.
            }

            await Task.Delay(250);
        }

        if (_webProcess is { HasExited: false })
        {
            _webProcess.Kill(entireProcessTree: true);
            await _webProcess.WaitForExitAsync();
        }

        throw new TimeoutException($"Web app did not start within {timeout.TotalSeconds} seconds.");
    }

    private static string FindSolutionRoot()
    {
        string? current = AppContext.BaseDirectory;
        while (!string.IsNullOrWhiteSpace(current))
        {
            if (File.Exists(Path.Combine(current, "eShopOnWeb.slnx")))
            {
                return current;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new DirectoryNotFoundException("Could not locate repository root containing eShopOnWeb.slnx.");
    }
}
