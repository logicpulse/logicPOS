using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Core.BackOffice;

/// <summary>
/// User-facing save errors. Business text is kept; Entity Framework and provider text is logged and replaced.
/// </summary>
public static class SaveExceptionText
{
    public const string Generic = "Não foi possível concluir a operação.";

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

            if (current is DbUpdateException or DbUpdateConcurrencyException)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(current.Message) || IsTechnical(current.Message))
            {
                continue;
            }

            business ??= current.Message;
        }

        return business ?? Generic;
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

    private static bool IsTechnical(string message)
        => message.Contains("EntityFramework", StringComparison.OrdinalIgnoreCase)
            || message.Contains("DbUpdate", StringComparison.Ordinal)
            || message.Contains("SQLite", StringComparison.OrdinalIgnoreCase)
            || message.Contains("SqlException", StringComparison.Ordinal)
            || message.Contains("See the inner exception", StringComparison.OrdinalIgnoreCase)
            || message.Contains("Microsoft.Data", StringComparison.Ordinal)
            || message.Contains("MySql", StringComparison.OrdinalIgnoreCase);
}
