using LogicPOS.Core.FrontOffice;
using LogicPOS.Domain.Entities.Documents;

namespace LogicPOS.Core.Tests.Finance;

public sealed class RequiresPaymentMethodsTests
{
    [Theory]
    [InlineData("FS")]
    [InlineData("FR")]
    [InlineData("VD")]
    [InlineData("TRV")]
    public void Uses_payment_methods_for_web_aligned_types(string acronym)
    {
        Assert.True(new DocumentTypeAnalyzer(acronym).UsesPaymentMethodsByAcronym());
        Assert.True(PosDocumentRules.RequiresPaymentMethods(acronym));
    }

    [Theory]
    [InlineData("FT")]
    [InlineData("NC")]
    [InlineData("OR")]
    [InlineData("GT")]
    [InlineData(null)]
    [InlineData("")]
    public void Does_not_require_payment_methods_for_other_types(string? acronym)
    {
        if (string.IsNullOrWhiteSpace(acronym) == false)
        {
            Assert.False(new DocumentTypeAnalyzer(acronym).UsesPaymentMethodsByAcronym());
        }

        Assert.False(PosDocumentRules.RequiresPaymentMethods(acronym));
    }
}
