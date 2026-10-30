namespace LogicPOS.Core.Tests;

public class TextSearchTests
{
    [Theory]
    [InlineData("Joao", "João")]
    [InlineData("João", "Joao")]
    [InlineData("JOAO", "João")]
    [InlineData("cafe", "café")]
    [InlineData("São", "Sao")]
    public void Contains_ignores_accents_both_ways(string query, string candidate)
        => Assert.True(TextSearch.Contains(candidate, query));

    [Fact]
    public void Contains_still_requires_a_match()
        => Assert.False(TextSearch.Contains("João Silva", "Maria"));

    [Fact]
    public void Fold_strips_combining_marks()
        => Assert.Equal("Joao", TextSearch.Fold("João"));
}