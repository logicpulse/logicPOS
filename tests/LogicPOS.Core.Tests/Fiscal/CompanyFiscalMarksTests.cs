using LogicPOS.Core.Fiscal;

namespace LogicPOS.Core.Tests.Fiscal;

public sealed class CompanyFiscalMarksTests
{
    [Fact]
    public void Reads_company_marks_from_parameters()
    {
        var parameters = new[]
        {
            (CompanyFiscalMarkTokens.RegimeArtigo53, "True"),
            (CompanyFiscalMarkTokens.VendedorAmbulante, "false"),
        };
        var state = CompanyFiscalMarks.Read(parameters);
        Assert.True(state.RegimeArtigo53);
        Assert.False(state.VendedorAmbulante);
    }
}