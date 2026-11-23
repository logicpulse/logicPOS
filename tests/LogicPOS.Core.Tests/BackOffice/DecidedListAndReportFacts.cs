using LogicPOS.Application.Features.Finance.PaymentMethods;
using LogicPOS.Core.BackOffice;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class DecidedListAndReportFacts
{
    [Fact]
    public void Report_month_and_list_year_run_through_the_last_day()
    {
        var day = new DateTime(2026, 11, 23);
        var month = DefaultPeriods.Month(day);
        var year = DefaultPeriods.Year(day);

        Assert.Equal(new DateTime(2026, 11, 1), month.Start);
        Assert.Equal(new DateTime(2026, 11, 30, 23, 59, 59), month.End);
        Assert.Equal(new DateTime(2026, 1, 1), year.Start);
        Assert.Equal(new DateTime(2026, 12, 31, 23, 59, 59), year.End);
    }

    [Fact]
    public void Todos_is_a_choice_and_an_empty_filter_is_refused()
    {
        Assert.Null(ReportFilterSelection.Missing(showCustomer: true, customerChosen: true, supplier: false, showArticle: true, articleChosen: true));
        Assert.Equal("Escolha um cliente.", ReportFilterSelection.Missing(true, false, false, false, true));
        Assert.Equal("Escolha um artigo.", ReportFilterSelection.Missing(false, true, false, true, false));
    }

    [Fact]
    public void Customer_detail_accepts_todos()
    {
        var report = ReportCatalog.All.Single(item => item.Key == "customer-balance-details");
        Assert.True(report.ShowCustomer);
        Assert.False(report.NeedsCustomer);
    }

    [Fact]
    public void Work_session_list_keeps_open_terminal_sessions()
    {
        Assert.True(WorkSessionListing.Include(WorkSessionPeriodType.Terminal, WorkSessionPeriodStatus.Open));
        Assert.False(WorkSessionListing.Include(WorkSessionPeriodType.Terminal, WorkSessionPeriodStatus.Closed));
        Assert.False(WorkSessionListing.Include(WorkSessionPeriodType.Day, WorkSessionPeriodStatus.Open));
    }

    [Fact]
    public void Volta_tip_says_what_the_button_does()
    {
        Assert.Equal(
            "Emite o talão de reembolso do depósito de vasilhame e imprime o documento.",
            ListingLabelTips.Tooltip("Reembolso de depósito"));
    }

    [Fact]
    public void Mb_way_is_only_offered_for_portugal_and_is_not_multibanco()
    {
        var portugal = PosPaymentMethods.For(true).Select(item => item.Token).ToArray();
        var other = PosPaymentMethods.For(false).Select(item => item.Token).ToArray();

        Assert.Contains(PosPaymentMethods.MbWayToken, portugal);
        Assert.Contains(PosPaymentMethods.MultibancoToken, portugal);
        Assert.DoesNotContain(PosPaymentMethods.MbWayToken, other);
        Assert.Contains(PosPaymentMethods.MultibancoToken, other);
        Assert.Equal("Multibanco", PosPaymentMethods.RenameCashMachine("MB Way"));
        Assert.Null(PosPaymentMethods.RenameCashMachine("Personalizado"));
    }
}
