using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests.Finance;

public sealed class ImmediatePaymentConditionTests
{
    [Fact]
    public void Picks_pronto_pagamento_by_acronym()
    {
        var thirty = Guid.Parse("198235e4-12e3-42ad-b6fb-2675b5baeac2");
        var pronto = Guid.Parse("4261daa6-c0bd-4ac9-949a-cae0be2dd472");
        var conditions = new[]
        {
            new PosLookupItem(thirty, "30 Dias", "30"),
            new PosLookupItem(pronto, "Pronto Pagamento", "PP"),
        };

        Assert.Equal(pronto, PosDocumentRules.ResolveImmediatePaymentConditionId(conditions));
    }

    [Fact]
    public void Falls_back_to_first_when_pp_missing()
    {
        var first = Guid.NewGuid();
        var conditions = new[]
        {
            new PosLookupItem(first, "15 Dias", "15"),
            new PosLookupItem(Guid.NewGuid(), "30 Dias", "30"),
        };

        Assert.Equal(first, PosDocumentRules.ResolveImmediatePaymentConditionId(conditions));
    }

    [Fact]
    public void Returns_null_when_empty()
    {
        Assert.Null(PosDocumentRules.ResolveImmediatePaymentConditionId(Array.Empty<PosLookupItem>()));
    }
}
