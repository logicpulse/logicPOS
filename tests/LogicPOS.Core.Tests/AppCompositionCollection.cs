namespace LogicPOS.Core.Tests;

/// <summary>
/// Serializes tests that call <see cref="AppComposition.Configure"/> so they do not
/// race on the static <see cref="AppComposition.Services"/> provider.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class AppCompositionCollection : ICollectionFixture<object>
{
    public const string Name = "AppComposition";
}
