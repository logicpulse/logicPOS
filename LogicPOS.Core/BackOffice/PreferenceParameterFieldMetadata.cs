using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Portal-aligned input types for preference tokens (seeds may store <see cref="PreferenceParameterInputType.Text"/>).
/// </summary>
public static class PreferenceParameterFieldMetadata
{
    private static readonly HashSet<string> PasswordTokens = new(StringComparer.OrdinalIgnoreCase)
    {
        "SERVICE_AT_PRODUCTION_ACCOUNT_PASSWORD",
        "SEND_MAIL_SMTP_PASSWORD",
        "TRACK_API_PASSWORD",
    };

    public static bool IsPasswordToken(string? token)
        => string.IsNullOrWhiteSpace(token) == false && PasswordTokens.Contains(token);

    public static void Apply(PreferenceParameter parameter)
    {
        if (IsPasswordToken(parameter.Token) == false)
        {
            return;
        }

        if (parameter.InputType != PreferenceParameterInputType.TextPassword)
        {
            parameter.InputType = PreferenceParameterInputType.TextPassword;
        }
    }
}