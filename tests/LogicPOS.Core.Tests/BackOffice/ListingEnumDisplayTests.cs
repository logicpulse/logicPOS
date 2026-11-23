using LogicPOS.Core.BackOffice;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class ListingEnumDisplayTests
{
    [Fact]
    public void Table_status_labels_use_feminine_forms()
    {
        Assert.Equal("Livre", ListingEnumDisplay.Label(TableStatus.Free));
        Assert.Equal("Ocupada", ListingEnumDisplay.Label(TableStatus.Open));
        Assert.Equal("Reservada", ListingEnumDisplay.Label(TableStatus.Reserved));
    }

    [Fact]
    public void Work_session_status_labels_use_feminine_forms()
    {
        Assert.Equal("Aberta", ListingEnumDisplay.Label(WorkSessionPeriodStatus.Open));
        Assert.Equal("Fechada", ListingEnumDisplay.Label(WorkSessionPeriodStatus.Closed));
    }

    [Fact]
    public void Choices_keep_clr_name_as_value_and_localized_label()
    {
        var open = ListingEnumDisplay.Choices(typeof(TableStatus))
            .Single(choice => choice.Value == nameof(TableStatus.Open));
        Assert.Equal(nameof(TableStatus.Open), open.Value);
        Assert.Equal("Ocupada", open.Label);
        Assert.Equal("Ocupada", open.ToString());
    }

    [Fact]
    public void Editor_persists_clr_enum_name_not_portuguese_label()
    {
        var occupied = ListingEnumDisplay.Choices(typeof(TableStatus))
            .Single(choice => choice.Label == "Ocupada");
        Assert.Equal(nameof(TableStatus.Open), occupied.Value);
        Assert.NotEqual("Ocupada", occupied.Value);
        Assert.True(Enum.TryParse(occupied.Value, ignoreCase: false, out TableStatus status));
        Assert.Equal(TableStatus.Open, status);
        Assert.False(Enum.TryParse("Ocupada", ignoreCase: false, out TableStatus _));
    }
}
