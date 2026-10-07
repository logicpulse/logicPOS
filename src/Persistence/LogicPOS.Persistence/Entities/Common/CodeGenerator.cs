
namespace LogicPOS.Persistence.Entities.Common;

internal class CodeGenerator
{
    private const int MinIncrement = 1;
    private const int MaxIncrement = 10;

    public static string DefaultFirstCode() => MinIncrement.ToString();
   
    public static string Generate(IEnumerable<string> existentCodes)
    {
        var numericCodes = existentCodes.Where(code => code.All(char.IsNumber)).ToArray();

        if (numericCodes.Any() == false)
        {
            return DefaultFirstCode();
        }

        ulong maxNumericCode = ulong.Parse(numericCodes.MaxBy(ulong.Parse)!);
        
        return (maxNumericCode + (ulong)Random.Shared.Next(MinIncrement, MaxIncrement)).ToString();
    }

}