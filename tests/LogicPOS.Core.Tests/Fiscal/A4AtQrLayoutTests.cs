using LogicPOS.Core.FrontOffice.Pdf;

namespace LogicPOS.Core.Tests.Fiscal;

public sealed class A4AtQrLayoutTests
{
    [Fact]
    public void Portugal_A4_AT_QR_size_meets_30mm_minimum()
    {
        Assert.True(DocumentModel.PortugalAtQrSizeMillimetres >= 30f);
        Assert.Equal(31f, DocumentModel.PortugalAtQrSizeMillimetres);
    }
}