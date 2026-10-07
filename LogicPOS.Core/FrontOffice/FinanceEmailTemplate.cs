namespace LogicPOS.Core.FrontOffice;

public sealed class PosEmailDraft
{
    public string Subject { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public string To { get; init; } = string.Empty;
}

public static class FinanceEmailTemplate
{
    public static PosEmailDraft Compose(
        string? subject,
        string? body,
        bool htmlBody,
        string? companyName,
        string? businessName,
        string? website,
        string? email,
        string? phone,
        string? address,
        string? postalCode,
        string? city,
        string? country,
        IEnumerable<string> documentNumbers,
        string? to)
    {
        var numbers = documentNumbers.Where(number => string.IsNullOrWhiteSpace(number) == false);
        var list = string.Join(htmlBody ? ";<br/>" : ";\n", numbers);
        var filled = (body ?? string.Empty)
            .Replace("${DOCUMENT_LIST}", list)
            .Replace("${COMPANY_NAME}", companyName ?? string.Empty)
            .Replace("${COMPANY_BUSINESS_NAME}", businessName ?? string.Empty)
            .Replace("${COMPANY_WEBSITE}", website ?? string.Empty)
            .Replace("${COMPANY_EMAIL}", email ?? string.Empty)
            .Replace("${COMPANY_TELEPHONE}", phone ?? string.Empty)
            .Replace("${COMPANY_ADDRESS}", address ?? string.Empty)
            .Replace("${COMPANY_POSTALCODE}", postalCode ?? string.Empty)
            .Replace("${COMPANY_CITY}", city ?? string.Empty)
            .Replace("${COMPANY_COUNTRY}", country ?? string.Empty);

        if (string.IsNullOrWhiteSpace(filled))
        {
            filled = string.IsNullOrWhiteSpace(list) ? "Seguem os documentos em anexo." : list;
        }

        return new PosEmailDraft
        {
            Subject = string.IsNullOrWhiteSpace(subject) ? "Documentos" : subject.Trim(),
            Body = filled,
            To = to?.Trim() ?? string.Empty
        };
    }
}
