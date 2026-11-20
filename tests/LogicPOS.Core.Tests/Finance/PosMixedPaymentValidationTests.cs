using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests.Finance;

public sealed class PosMixedPaymentValidationTests
{
    [Fact]
    public void Matching_portions_pass()
    {
        var error = PosMixedPaymentValidation.ValidatePortions(50m, [20m, 30m]);
        Assert.Null(error);
    }

    [Fact]
    public void Short_portions_fail()
    {
        var error = PosMixedPaymentValidation.ValidatePortions(50m, [20m, 20m]);
        Assert.NotNull(error);
        Assert.Contains("soma dos pagamentos", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Zero_portion_fails()
    {
        var error = PosMixedPaymentValidation.ValidatePortions(50m, [50m, 0m]);
        Assert.NotNull(error);
    }
}