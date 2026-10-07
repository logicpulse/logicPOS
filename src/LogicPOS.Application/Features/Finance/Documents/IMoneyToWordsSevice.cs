namespace LogicPOS.Application.Features.Finance.Documents;

public interface IMoneyToWordsSevice
{
    string ConvertToWords(decimal amount, string currency);
}
