using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests.Finance;

public sealed class CanUseAsSourceDocumentTests
{
    [Fact]
    public void FT_Normal_can_be_used_as_origin()
    {
        Assert.True(PosDocumentRules.CanUseAsSourceDocument("FT", "N", isDraft: false));
    }

    [Fact]
    public void Draft_cancelled_and_unknown_type_cannot_be_used_as_origin()
    {
        Assert.False(PosDocumentRules.CanUseAsSourceDocument("FT", "N", isDraft: true));
        Assert.False(PosDocumentRules.CanUseAsSourceDocument("FT", "A", isDraft: false));
        Assert.False(PosDocumentRules.CanUseAsSourceDocument("ZZ", "N", isDraft: false));
    }

    [Fact]
    public void Row_infers_type_from_number_when_cloud_omits_Type()
    {
        // LogicPulse Cloud ListByPeriod historically left Type/StatusCode empty.
        var row = new PosDocumentRow
        {
            Number = "FT A001/12",
            Status = "Normal",
            Type = string.Empty,
            StatusCode = string.Empty,
            IsDraft = false
        };

        Assert.True(row.CanUseAsSource);
    }

    [Fact]
    public void Row_hides_origin_for_draft_status_even_without_flags()
    {
        var row = new PosDocumentRow
        {
            Number = "FT rascunho",
            Status = "Rascunho",
            Type = string.Empty,
            StatusCode = string.Empty,
            IsDraft = false
        };

        Assert.False(row.CanUseAsSource);
    }

    [Fact]
    public void FT_may_be_origin_of_NC_and_guias()
    {
        var targets = PosDocumentRules.ValidTargetTypesForSource("FT");

        Assert.Contains("NC", targets);
        Assert.Contains("GR", targets);
        Assert.Contains("GT", targets);
        Assert.Equal("NC", PosDocumentRules.PreferredTargetTypeForSource("FT"));
    }

    [Fact]
    public void Delivery_note_prefers_invoice_as_target()
    {
        var targets = PosDocumentRules.ValidTargetTypesForSource("GR");

        Assert.Contains("FT", targets);
        Assert.DoesNotContain("NC", targets);
        Assert.Equal("FT", PosDocumentRules.PreferredTargetTypeForSource("GR"));
    }
}
