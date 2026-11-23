using System.Globalization;

namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Documents page chrome captions. Prefer Resx via the App layer when a key exists;
/// this covers the few hardcoded listing strings that must follow <see cref="UiCulture"/>.
/// </summary>
public static class DocumentsListingText
{
    public static string SelectedCount(int count, string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => $"{count} document(s) selected",
            "fr" => $"{count} document(s) sélectionné(s)",
            "es" => $"{count} documento(s) seleccionado(s)",
            _ => $"{count} documento(s) selecionado(s)"
        };
    }

    public static string EmptyPeriod(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "There are no documents in this period.",
            "fr" => "Il n'y a aucun document pour cette période.",
            "es" => "No hay documentos en este período.",
            _ => "Não existem documentos neste período."
        };
    }

    public static string SelectAll(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "Select all",
            "fr" => "Tout sélectionner",
            "es" => "Seleccionar todo",
            _ => "Selecionar tudo"
        };
    }

    public static string Columns(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "Columns ▾",
            "fr" => "Colonnes ▾",
            "es" => "Columnas ▾",
            _ => "Colunas ▾"
        };
    }

    public static string Search(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "Search",
            "fr" => "Rechercher",
            "es" => "Buscar",
            _ => "Procurar"
        };
    }

    public static string Period(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "Period",
            "fr" => "Période",
            "es" => "Período",
            _ => "Período"
        };
    }

    public static string Filters(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "Filters",
            "fr" => "Filtres",
            "es" => "Filtros",
            _ => "Filtros"
        };
    }

    public static string More(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "More",
            "fr" => "Plus",
            "es" => "Más",
            _ => "Mais"
        };
    }

    private static string Language(string? cultureName)
    {
        var name = string.IsNullOrWhiteSpace(cultureName) ? UiCulture.Read(null) : cultureName!;
        try
        {
            return CultureInfo.GetCultureInfo(name).TwoLetterISOLanguageName;
        }
        catch (CultureNotFoundException)
        {
            return "pt";
        }
    }
}
