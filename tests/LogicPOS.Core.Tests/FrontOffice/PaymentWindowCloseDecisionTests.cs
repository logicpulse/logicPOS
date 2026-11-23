using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests.FrontOffice;

public sealed class PaymentWindowCloseDecisionTests
{
    [Theory]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    public void Blocks_close_only_while_issuing_before_success(bool issuing, bool confirmPayment, bool expected)
    {
        Assert.Equal(expected, PaymentWindowCloseDecision.ShouldCancelWhileIssuing(issuing, confirmPayment));
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    public void Forces_success_close_after_issue_unless_already_closing_with_result(
        bool confirmPayment,
        bool closingWithResult,
        bool expected)
    {
        Assert.Equal(
            expected,
            PaymentWindowCloseDecision.ShouldForceSuccessClose(confirmPayment, closingWithResult));
    }
}
