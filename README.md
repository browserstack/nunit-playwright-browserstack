# nunit-playwright-browserstack

This sample shows how to run [NUnit](https://nunit.org/) + [Playwright](https://playwright.dev/dotnet) tests on BrowserStack using the [BrowserStack .NET SDK](https://www.nuget.org/packages/BrowserStack.TestAdapter). The SDK reads `browserstack.yml`, fans your tests out across the platforms listed there, starts and stops BrowserStack Local automatically, and reports test status to the BrowserStack dashboard. Your test code stays pure `Microsoft.Playwright` + NUnit -- no manual `ConnectAsync`, no caps in code.

![BrowserStack Logo](https://d98b8t1nnulk5.cloudfront.net/production/images/layout/logo-header.png?1469004780)

## Run Sample Build

* Clone the repo
* Open the solution `NunitPlaywrightBrowserstack.sln` in Visual Studio (or your IDE of choice)
* Build the solution (`dotnet build`)
* Replace the `userName` and `accessKey` placeholders in `browserstack.yml` with your [BrowserStack Username and Access Key](https://www.browserstack.com/accounts/settings). Alternatively, remove those two lines from the yml and set `BROWSERSTACK_USERNAME` and `BROWSERSTACK_ACCESS_KEY` as environment variables -- the SDK falls back to env vars only when the yml fields are absent

### Running your tests from CLI

```sh
cd NunitPlaywrightBrowserstack.Tests
dotnet test --filter "Category=sample-test"
```

The sample runs `BStackDemoCartTest` across all four platforms declared in `browserstack.yml` (Windows 11 / Chrome, macOS / WebKit, Windows 11 / Firefox, Windows 11 / Edge) in parallel.

Understand how many parallel sessions you need by using our [Parallel Test Calculator](https://www.browserstack.com/automate/parallel-calculator?ref=github).

### Testing a private host (BrowserStack Local)

If your app lives on `localhost`, a staging host, or behind a firewall, run the local fixture (`browserstackLocal: true` is already set in `browserstack.yml`):

```sh
# 1. Stand up a tiny static page locally on port 45454 (any HTTP server works)
mkdir -p bs-local-harness && cd bs-local-harness
cat > index.html <<'HTML'
<!doctype html>
<html><head><title>BrowserStack Local Test</title></head>
<body>OK</body></html>
HTML
python3 -m http.server 45454 &

# 2. Run the local fixture
cd ../NunitPlaywrightBrowserstack.Tests
dotnet test --filter "Category=sample-local-test"
```

The SDK starts and stops the BrowserStack Local tunnel for you -- no manual binary download or lifecycle management. The cloud browser reaches your local server through `http://bs-local.com:<port>/`.

## Integrate your test suite

This repository uses the BrowserStack SDK to run tests on BrowserStack. To wire the SDK into your own test suite:

* Create a `browserstack.yml` at the project root with your BrowserStack credentials and platform list (see this repo for a working template)
* Add the `BrowserStack.TestAdapter` NuGet package:

  ```sh
  dotnet add package BrowserStack.TestAdapter
  ```

* Build the project (`dotnet build`); the SDK installs the `browserstack-sdk` dotnet tool and patches the test assembly so Playwright launches are routed to BrowserStack at runtime

## How the SDK changes things

- **One `browserstack.yml`** declares platforms, parallelism, the Local toggle, and reporting; the SDK picks them up automatically
- **The SDK runs platforms in parallel for you** -- one NUnit run per `(platform x parallelsPerPlatform)` cell, no per-platform branching needed
- **The SDK rewrites Playwright launches** -- `PlaywrightFixtureBase.cs` calls `pw.Chromium.LaunchAsync()` and the SDK transparently redirects to the per-platform browser configured in the yml (`chrome` / `playwright-webkit` / `playwright-firefox` / `edge`). No `Chromium.ConnectAsync(wss_url)` plumbing
- **The SDK starts and stops BrowserStack Local** when `browserstackLocal: true` -- no manual tunnel lifecycle management

## Parallelism: SDK fan-out x NUnit

This sample stacks two layers of parallelism:

| Layer | Configured in | Effect |
|---|---|---|
| SDK platform fan-out | `browserstack.yml` -> `platforms` + `parallelsPerPlatform` | The SDK spawns one NUnit run per `(platform x parallelsPerPlatform)` cell |
| NUnit fixture-level parallelism | `AssemblyInfo.cs` -> `[assembly: Parallelizable(ParallelScope.Fixtures)]` | Within each NUnit run, `[TestFixture]` classes execute concurrently |

With the shipped defaults (4 platforms x `parallelsPerPlatform: 1`, 2 fixtures: `BStackDemoCartTest` + `BStackLocalSampleTest` running in parallel via NUnit), a single `dotnet test` produces **up to 8 concurrent BrowserStack sessions**. Tune `parallelsPerPlatform` and `LevelOfParallelism` in `AssemblyInfo.cs` to match your BrowserStack plan.

## Repo layout

```
.
├── NunitPlaywrightBrowserstack.sln
└── NunitPlaywrightBrowserstack.Tests/
    ├── NunitPlaywrightBrowserstack.Tests.csproj
    ├── browserstack.yml             # SDK config: credentials, 4 platforms, Local toggle, reporting
    ├── AssemblyInfo.cs              # NUnit fixture-level parallelism
    ├── PlaywrightFixtureBase.cs     # [SetUp]/[TearDown] managing IPage; SDK routes the launch
    ├── BStackDemoCartTest.cs        # bstackdemo add-to-cart scenario
    └── BStackLocalSampleTest.cs     # BrowserStack Local + bs-local.com:45454 scenario
```

## Notes

* You can view your test results on the [BrowserStack Automate dashboard](https://www.browserstack.com/automate)
* To test on a different set of browsers, see our [list of supported browsers and platforms](https://www.browserstack.com/list-of-browsers-and-platforms?product=automate)
* You can export the environment variables for the Username and Access Key of your BrowserStack account:

  * For Unix-like or Mac machines:
  ```sh
  export BROWSERSTACK_USERNAME=<browserstack-username> &&
  export BROWSERSTACK_ACCESS_KEY=<browserstack-access-key>
  ```
  * For Windows Cmd:
  ```cmd
  set BROWSERSTACK_USERNAME=<browserstack-username>
  set BROWSERSTACK_ACCESS_KEY=<browserstack-access-key>
  ```
  * For Windows Powershell:
  ```powershell
  $env:BROWSERSTACK_USERNAME=<browserstack-username>
  $env:BROWSERSTACK_ACCESS_KEY=<browserstack-access-key>
  ```

## Further Reading

- [NUnit](https://nunit.org/)
- [Playwright .NET](https://playwright.dev/dotnet/)
- [BrowserStack documentation for Playwright in C#](https://www.browserstack.com/docs/automate/playwright/getting-started/c-sharp)
- [BrowserStack.TestAdapter on NuGet](https://www.nuget.org/packages/BrowserStack.TestAdapter)
- [xUnit + Reqnroll reference sample](https://github.com/browserstack/xunit-reqnroll-playwright-browserstack)
- [Original C# Playwright reference sample](https://github.com/browserstack/csharp-playwright-browserstack)

Happy Testing!
