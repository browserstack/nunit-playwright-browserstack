namespace NunitPlaywrightBrowserstack.Tests;

// Mirrors browserstack/csharp-playwright-browserstack -> SampleLocalTest.cs:
//   page.GotoAsync("http://bs-local.com:45454/")  +  title.Contains("BrowserStack Local")
//
// Requires:
//   * browserstack.yml has `browserstackLocal: true`
//   * a local HTTP server is serving a page with title "BrowserStack Local Test"
//     on port 45454 (the sanity workflow stands up `python -m http.server 45454`
//     against a one-file index.html harness).
[TestFixture]
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
