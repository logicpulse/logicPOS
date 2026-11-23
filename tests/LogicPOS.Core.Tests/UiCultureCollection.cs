namespace LogicPOS.Core.Tests;

/// <summary>Serializes tests that mutate the shared <see cref="LogicPOS.Core.UiCulture"/> cache.</summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class UiCultureCollection
{
    public const string Name = "UiCulture";
}
