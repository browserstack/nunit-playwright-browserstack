// Allow NUnit to run [TestFixture] classes in parallel within each SDK-spawned
// platform process. Combined with `parallelsPerPlatform: 2` in browserstack.yml,
// each `dotnet test` produces 4 platforms x 2 fixtures = 8 concurrent sessions.
[assembly: NUnit.Framework.Parallelizable(NUnit.Framework.ParallelScope.Fixtures)]
[assembly: NUnit.Framework.LevelOfParallelism(4)]
