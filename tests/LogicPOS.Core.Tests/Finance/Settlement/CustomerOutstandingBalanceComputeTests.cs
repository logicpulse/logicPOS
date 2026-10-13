using LogicPOS.Application.Features.Finance.Customers;

namespace LogicPOS.Core.Tests.Finance.Settlement;

public class CustomerOutstandingBalanceComputeTests
{
    [Fact]
    public void Compute_rounds_each_document_total_before_summing_debit()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var rows = new[]
        {
            new CustomerOutstandingBalanceCompute.DocumentBalanceRow(id1, 10.005m),
            new CustomerOutstandingBalanceCompute.DocumentBalanceRow(id2, 10.005m),
        };

        var (totalDebit, outstanding, totalCredit, balance) =
            CustomerOutstandingBalanceCompute.Compute(rows, new Dictionary<Guid, decimal>());

        Assert.Equal(20.02m, totalDebit);
        Assert.Equal(20.02m, outstanding);
        Assert.Equal(0m, totalCredit);
        Assert.Equal(-20.02m, balance);
    }

    [Fact]
    public void Compute_outstanding_rounds_per_document_remainder()
    {
        var id = Guid.NewGuid();
        var rows = new[] { new CustomerOutstandingBalanceCompute.DocumentBalanceRow(id, 10.005m) };
        var paid = new Dictionary<Guid, decimal> { [id] = 5.002m };

        var (totalDebit, outstanding, totalCredit, balance) =
            CustomerOutstandingBalanceCompute.Compute(rows, paid);

        Assert.Equal(10.01m, totalDebit);
        Assert.Equal(5.00m, outstanding);
        Assert.Equal(5.01m, totalCredit);
        Assert.Equal(-5.00m, balance);
    }
}
