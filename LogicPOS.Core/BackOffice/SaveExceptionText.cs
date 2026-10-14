namespace LogicPOS.Core.BackOffice;

/// <summary>
/// User-facing save errors. Business text is kept; Entity Framework and provider text is logged and replaced.
/// </summary>
public static class SaveExceptionText
{
    public const string Generic = "Não foi possível concluir a operação.";

    /// <summary>Only <see cref="BusinessException"/> may show its message. Duplicate code and NIF are mapped separately.</summary>
    public static readonly Type[] BusinessExceptions =
    [
        typeof(BusinessException)
    ];

    public static string Describe(Exception exception)
    {
        AppLogging.Error(exception, "BackOffice operation failed");
        return Message(exception);
    }

    public static string Message(Exception exception)
    {
        string? business = null;
        for (var current = exception; current is not null; current = current.InnerException)
        {
            var duplicate = DuplicateMessage(current.Message);
            if (duplicate is not null)
            {
                return duplicate;
            }

            if (IsBusiness(current) == false || string.IsNullOrWhiteSpace(current.Message))
            {
                continue;
            }

            business ??= current.Message;
        }

        return business ?? Generic;
    }

    private static bool IsBusiness(Exception exception)
    {
        foreach (var type in BusinessExceptions)
        {
            if (type.IsInstanceOfType(exception))
            {
                return true;
            }
        }

        return false;
    }

    private static string? DuplicateMessage(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return null;
        }

        var unique = message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase)
            || message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase)
            || message.Contains("Duplicate entry", StringComparison.Ordinal);
        if (unique == false)
        {
            return null;
        }

        if (message.Contains("FiscalNumber", StringComparison.OrdinalIgnoreCase)
            || message.Contains("NIF", StringComparison.OrdinalIgnoreCase))
        {
            return "Já existe um registo com este NIF.";
        }

        if (message.Contains(".Code", StringComparison.Ordinal)
            || message.Contains("_Code", StringComparison.Ordinal)
            || message.Contains(" Code", StringComparison.Ordinal))
        {
            return "Já existe um registo com este código.";
        }

        return "Já existe um registo com estes dados.";
    }
}
