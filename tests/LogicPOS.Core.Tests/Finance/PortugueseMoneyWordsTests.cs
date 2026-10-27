using LogicPOS.Application.Features.Finance.Documents;
using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Tests.Finance;

public class PortugueseMoneyWordsTests
{
    [Theory]
    [InlineData(0.01, "EUR", "um cêntimo")]
    [InlineData(1, "EUR", "um euro")]
    [InlineData(2, "EUR", "dois euros")]
    [InlineData(1.01, "EUR", "um euro e um cêntimo")]
    [InlineData(16.16, "EUR", "dezasseis euros e dezasseis cêntimos")]
    [InlineData(193.73, "EUR", "cento e noventa e três euros e setenta e três cêntimos")]
    [InlineData(1001, "EUR", "mil e um euros")]
    [InlineData(1_000_000, "AOA", "um milhão de kwanzas")]
    [InlineData(2, "MZN", "dois meticais")]
    [InlineData(0.01, "MZN", "um centavo")]
    public void Writes_portugal_portuguese(decimal amount, string currency, string expected)
        => Assert.Equal(expected, PortugueseMoneyWords.Convert(amount, currency));

    [Fact]
    public void One_cent_is_not_the_nut_phrase()
        => Assert.Equal("um cêntimo", PortugueseMoneyWords.Convert(0.01m, "eur"));

    [Fact]
    public void Portugal_pay_dialog_hides_exchange_and_keeps_the_debt()
    {
        var euro = new ReceiptPayOption { Id = Guid.NewGuid(), Name = "Euro", Code = "EUR", ExchangeRate = 1m };
        var kwanza = new ReceiptPayOption { Id = Guid.NewGuid(), Name = "Kwanza", Code = "AOA", ExchangeRate = 187.58m };
        var pay = ReceiptPayDefaults.For(portugal: true, net: 0.01m, kwanza.Id, [kwanza, euro]);

        Assert.False(ReceiptPayDefaults.ShowExchange(portugal: true));
        Assert.Equal(euro.Id, pay.Currency!.Id);
        Assert.Equal(1m, pay.Rate);
        Assert.Equal(0.01m, pay.Delivered);
    }

    [Fact]
    public void Other_countries_keep_the_exchange_rate()
    {
        var euro = new ReceiptPayOption { Id = Guid.NewGuid(), Name = "Euro", Code = "EUR", ExchangeRate = 1m };
        var kwanza = new ReceiptPayOption { Id = Guid.NewGuid(), Name = "Kwanza", Code = "AOA", ExchangeRate = 187.58m };
        var pay = ReceiptPayDefaults.For(portugal: false, net: 0.01m, kwanza.Id, [euro, kwanza]);

        Assert.True(ReceiptPayDefaults.ShowExchange(portugal: false));
        Assert.Equal(kwanza.Id, pay.Currency!.Id);
        Assert.Equal(187.58m, pay.Rate);
        Assert.Equal(1.88m, pay.Delivered);
    }
}
