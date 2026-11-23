using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests.FrontOffice;

public sealed class CashPadAccumulationTests
{
    [Fact]
    public void Two_ten_euro_notes_sum_to_twenty()
    {
        var total = 0m;
        total = CashPadAccumulation.ApplyNote(total, 10m, wasTyping: false, typedValue: null);
        total = CashPadAccumulation.ApplyNote(total, 10m, wasTyping: false, typedValue: null);
        Assert.Equal(20m, total);
    }

    [Fact]
    public void Five_ten_and_twenty_sum_to_thirty_five()
    {
        var total = 0m;
        total = CashPadAccumulation.ApplyNote(total, 5m, wasTyping: false, typedValue: null);
        total = CashPadAccumulation.ApplyNote(total, 10m, wasTyping: false, typedValue: null);
        total = CashPadAccumulation.ApplyNote(total, 20m, wasTyping: false, typedValue: null);
        Assert.Equal(35m, total);
    }

    [Fact]
    public void Typed_value_seeds_total_before_adding_note()
    {
        var total = CashPadAccumulation.ApplyNote(
            currentTotal: 99m,
            noteAmount: 10m,
            wasTyping: true,
            typedValue: 7m);
        Assert.Equal(17m, total);
    }

    [Fact]
    public void Failed_typed_parse_seeds_zero_before_adding_note()
    {
        var total = CashPadAccumulation.ApplyNote(
            currentTotal: 99m,
            noteAmount: 10m,
            wasTyping: true,
            typedValue: null);
        Assert.Equal(10m, total);
    }
}
