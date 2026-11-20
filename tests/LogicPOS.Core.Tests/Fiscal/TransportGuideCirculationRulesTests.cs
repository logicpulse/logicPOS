using LogicPOS.Core.Fiscal;

namespace LogicPOS.Core.Tests.Fiscal;

public sealed class TransportGuideCirculationRulesTests
{
    private static readonly DateTime Communicated = new(2026, 3, 10, 9, 0, 0);
    private static readonly DateTime StartAfter = Communicated.AddMinutes(5);
    private static readonly DateTime StartBefore = Communicated.AddMinutes(-5);

    [Theory]
    [InlineData("True", true)]
    [InlineData("true", true)]
    [InlineData("1", true)]
    [InlineData("False", false)]
    [InlineData(null, false)]
    public void Company_mark_parses_from_preference(string? value, bool marked)
        => Assert.Equal(marked, TransportGuideCirculationRules.IsObligationMarked(value));

    [Fact]
    public void Without_mark_guides_print_without_at_code()
    {
        Assert.True(TransportGuideCirculationRules.MayPrintWithoutAtCode(obligationMarked: false));
        Assert.Null(TransportGuideCirculationRules.CirculationRefusal(
            obligationMarked: false,
            documentType: "GT",
            atDocCodeId: null,
            movementStart: StartAfter,
            communicatedAt: null,
            atCommunicationAvailable: true));
    }

    [Fact]
    public void With_mark_circulation_requires_at_code()
    {
        var refusal = TransportGuideCirculationRules.CirculationRefusal(
            obligationMarked: true,
            documentType: "GT",
            atDocCodeId: null,
            movementStart: StartAfter,
            communicatedAt: null,
            atCommunicationAvailable: true);
        Assert.Equal("A guia só circula depois de comunicada à AT e de receber o código.", refusal);
    }

    [Fact]
    public void With_mark_start_time_must_follow_communication()
    {
        var refusal = TransportGuideCirculationRules.CirculationRefusal(
            obligationMarked: true,
            documentType: "GT",
            atDocCodeId: "AT-123",
            movementStart: StartBefore,
            communicatedAt: Communicated,
            atCommunicationAvailable: true);
        Assert.Equal("A hora de início do transporte tem de ser posterior à comunicação à AT.", refusal);

        Assert.Null(TransportGuideCirculationRules.CirculationRefusal(
            obligationMarked: true,
            documentType: "GT",
            atDocCodeId: "AT-123",
            movementStart: StartAfter,
            communicatedAt: Communicated,
            atCommunicationAvailable: true));
    }

    [Fact]
    public void With_mark_offline_blocks_goods_leaving()
    {
        var issue = TransportGuideCirculationRules.IssueRefusalWhenOffline(
            obligationMarked: true,
            documentType: "GT",
            isDraft: false,
            atCommunicationAvailable: false);
        Assert.Equal("Sem ligação à AT não é possível comunicar a guia. Os bens não podem sair.", issue);

        var print = TransportGuideCirculationRules.CirculationRefusal(
            obligationMarked: true,
            documentType: "GT",
            atDocCodeId: null,
            movementStart: StartAfter,
            communicatedAt: null,
            atCommunicationAvailable: false);
        Assert.Equal("Sem ligação à AT não é possível comunicar a guia. Os bens não podem sair.", print);
    }

    [Fact]
    public void Non_waybill_documents_are_out_of_scope()
    {
        Assert.Null(TransportGuideCirculationRules.CirculationRefusal(
            obligationMarked: true,
            documentType: "FT",
            atDocCodeId: null,
            movementStart: StartAfter,
            communicatedAt: null,
            atCommunicationAvailable: false));
    }
}