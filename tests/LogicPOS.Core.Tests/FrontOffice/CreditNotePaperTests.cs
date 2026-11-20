using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests.FrontOffice;

public class CreditNotePaperTests
{
    [Theory]
    [InlineData("NC", false)]
    [InlineData("nc", false)]
    [InlineData("FT", true)]
    [InlineData("FR", false)]
    [InlineData("ND", true)]
    [InlineData("FS", false)]
    public void Named_customer_is_not_required_for_a_credit_note(string acronym, bool required)
    {
        Assert.Equal(required, PosDocumentRules.RequiresNamedCustomer(acronym));
    }

    [Theory]
    [InlineData("FS", true)]
    [InlineData("FR", true)]
    [InlineData("fr", true)]
    [InlineData("FT", false)]
    [InlineData("NC", false)]
    [InlineData(null, false)]
    public void Credit_note_follows_the_ticket_when_the_origin_is_a_ticket(string? sourceType, bool ticket)
    {
        Assert.Equal(ticket, PosDocumentRules.CreditNoteUsesTicket(sourceType));
    }

    [Fact]
    public void Printed_credit_note_amounts_are_positive()
    {
        Assert.Equal(12.50m, PosDocumentRules.CreditNotePaperAmount(-12.50m));
        Assert.Equal(3m, PosDocumentRules.CreditNotePaperAmount(3m));
    }
}
