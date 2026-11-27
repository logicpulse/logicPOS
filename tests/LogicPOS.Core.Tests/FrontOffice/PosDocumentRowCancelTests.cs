using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests.FrontOffice;

public sealed class PosDocumentRowCancelTests
{
    [Theory]
    [InlineData(true, "Rascunho", true, true)]
    [InlineData(false, "Rascunho", true, true)]
    [InlineData(false, "Normal", false, true)]
    [InlineData(false, "Anulado", false, false)]
    public void CanDelete_and_CanCancel_follow_document_state(bool isDraft, string status, bool expectedDelete, bool expectedCancel)
    {
        var row = new PosDocumentRow
        {
            IsDraft = isDraft,
            Status = status
        };

        Assert.Equal(expectedDelete, row.CanDelete);
        Assert.Equal(expectedCancel, row.CanCancel);
    }
}