namespace LogicPOS.Core.BackOffice;

/// <summary>
/// «Todos» is a selection. Gerar refuses only when a visible customer or article filter has nothing selected.
/// </summary>
public static class ReportFilterSelection
{
    public static string? Missing(
        bool showCustomer,
        bool customerChosen,
        bool supplier,
        bool showArticle,
        bool articleChosen)
    {
        if (showCustomer && customerChosen == false)
        {
            return supplier ? "Escolha um fornecedor." : "Escolha um cliente.";
        }

        if (showArticle && articleChosen == false)
        {
            return "Escolha um artigo.";
        }

        return null;
    }
}
