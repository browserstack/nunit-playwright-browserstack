// Run [TestFixture] classes in parallel within each SDK-spawned platform process.
[assembly: NUnit.Framework.Parallelizable(NUnit.Framework.ParallelScope.Fixtures)]
[assembly: NUnit.Framework.LevelOfParallelism(4)]
