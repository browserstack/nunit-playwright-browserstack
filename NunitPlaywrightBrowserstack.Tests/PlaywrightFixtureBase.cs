using Microsoft.Playwright;

namespace NunitPlaywrightBrowserstack.Tests;

// Base [SetUp]/[TearDown] for every NUnit fixture that needs an IPage.
// Customer code calls `pw.Chromium.LaunchAsync()` unconditionally; the
// BrowserStack SDK rewrites the launch at runtime to route to the per-platform
// browser configured in browserstack.yml (chrome / playwright-webkit /
// playwright-firefox / edge). No Chromium.ConnectAsync(wss_url) plumbing here.
public abstract class PlaywrightFixtureBase
{
    private IPlaywright? _pw;
    private IBrowser? _browser;
    private IBrowserContext? _context;

    protected IPage Page { get; private set; } = null!;

    [SetUp]
    public async Task SetUp()
    {
        _pw = await Playwright.CreateAsync();
        _browser = await _pw.Chromium.LaunchAsync();
        _context = await _browser.NewContextAsync();
        Page = await _context.NewPageAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_context is not null) await _context.CloseAsync();
        if (_browser is not null) await _browser.CloseAsync();
        _pw?.Dispose();
    }
}
