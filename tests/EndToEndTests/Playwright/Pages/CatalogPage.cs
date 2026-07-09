using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace Microsoft.eShopWeb.EndToEndTests.Playwright.Pages;

public sealed class CatalogPage(IPage page)
{
    public Task AssertPageTitleAsync()
        => Expect(page).ToHaveTitleAsync("Catalog - Microsoft.eShopOnWeb");

    public ILocator AddToBasketButton => page.GetByRole(AriaRole.Button, new() { Name = "[ ADD TO BASKET ]" }).First;

    public async Task GotoAsync(string baseUrl)
    {
        await page.GotoAsync($"{baseUrl}/");
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}
