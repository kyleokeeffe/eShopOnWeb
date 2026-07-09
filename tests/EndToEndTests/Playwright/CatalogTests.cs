using static Microsoft.Playwright.Assertions;
using Microsoft.Playwright;
using Microsoft.eShopWeb.EndToEndTests.Playwright.Pages;
using Xunit;

namespace Microsoft.eShopWeb.EndToEndTests.Playwright;

public sealed class CatalogTests(BrowserFixture browserFixture) : IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task HomePage_ShowsCatalog()
    {
        await using IBrowserContext context = await browserFixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });
        IPage page = await context.NewPageAsync();

        var catalogPage = new CatalogPage(page);

        await catalogPage.GotoAsync(browserFixture.BaseUrl);

        await catalogPage.AssertPageTitleAsync();
        await Expect(catalogPage.AddToBasketButton).ToBeVisibleAsync();
    }
}
