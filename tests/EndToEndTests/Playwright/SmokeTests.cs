using Microsoft.Playwright;
using Xunit;

namespace Microsoft.eShopWeb.EndToEndTests.Playwright;

public sealed class SmokeTests(BrowserFixture browserFixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task HomePage_ShowsExpectedProductData()
    {
        await using IBrowserContext context = await browserFixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });
        IPage page = await context.NewPageAsync();

        await page.GotoAsync($"{browserFixture.BaseUrl}/");
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        string pageText = await page.InnerTextAsync("body");
        Assert.Contains("SQL Server", pageText);
        Assert.Contains("Visual Studio", pageText);
    }
}
