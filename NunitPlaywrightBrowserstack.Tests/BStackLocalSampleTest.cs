namespace NunitPlaywrightBrowserstack.Tests;

// Requires browserstack.yml has `browserstackLocal: true` and a local HTTP server
// is serving a page with title containing "BrowserStack Local" on port 45454.
[TestFixture]
[Category("sample-local-test")]
[Parallelizable(ParallelScope.Self)]
public class BStackLocalSampleTest : PlaywrightFixtureBase
{
    [Test]
    public async Task ReachPrivateHostViaBrowserStackLocal()
    {
        await Page.GotoAsync("http://bs-local.com:45454/");

        var title = await Page.TitleAsync();
        Assert.That(title, Does.Contain("BrowserStack Local"));
    }
}
