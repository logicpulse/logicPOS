using System.Text.RegularExpressions;

namespace LogicPOS.Domain.Entities.Common.Utilities;

public static class CodeValidator
{
    public const string ValidCodeRegex = @"^[a-zA-Z0-9]+$";
    public static bool IsValidCode(string code)
    {
        return Regex.IsMatch(code, ValidCodeRegex);
    }
}