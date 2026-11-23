namespace LogicPOS.Application.Features.Finance.PaymentMethods;

/// <summary>
/// Multibanco and MB Way are different methods. MB Way is shown only for a Portugal database.
/// </summary>
public static class PosPaymentMethods
{
    public const string MultibancoToken = "CASH_MACHINE";

    public const string MbWayToken = "MB_WAY";

    public const string MbWayDesignation = "MB Way";

    public const string MultibancoDesignation = "Multibanco";

    public static IReadOnlyList<(string Token, string Label)> For(bool portugal)
    {
        var methods = new List<(string Token, string Label)>
        {
            ("MONEY", "Numerário"),
            (MultibancoToken, MultibancoDesignation)
        };
        if (portugal)
        {
            methods.Add((MbWayToken, MbWayDesignation));
        }

        methods.Add(("DEBIT_CARD", "Cartão de Débito"));
        methods.Add(("BANK_CHECK", "Cheque"));
        methods.Add(("CREDIT_CARD", "Cartão de Crédito"));
        methods.Add(("CURRENT_ACCOUNT", "Conta Corrente"));
        return methods;
    }

    /// <summary>The version change for the old cash-machine row that was labeled MB Way.</summary>
    public static string? RenameCashMachine(string? designation)
        => designation == MbWayDesignation ? MultibancoDesignation : null;
}
