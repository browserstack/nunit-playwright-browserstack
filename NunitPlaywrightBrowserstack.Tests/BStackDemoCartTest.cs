namespace NunitPlaywrightBrowserstack.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class BStackDemoCartTest : PlaywrightFixtureBase
{
    [Test]
    public async Task AddTheFirstItemToCart()
    {
        await Page.GotoAsync("https://bstackdemo.com/");

        var firstProduct = Page.Locator("[id=\"\\31 \"]");
        var titles = await firstProduct.Locator(".shelf-item__title").AllInnerTextsAsync();
        var productTitle = titles[0];
        await firstProduct.GetByText("Add to Cart").ClickAsync();

        var quantity = await Page.Locator(".bag__quantity").InnerTextAsync();
        Assert.That(quantity, Is.EqualTo("1"));

        var cartTitle = await Page.Locator(".shelf-item__details").Locator(".title").InnerTextAsync();
        Assert.That(cartTitle, Is.EqualTo(productTitle));
    }
}
