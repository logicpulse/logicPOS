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

    public static string Type(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "Type",
            "fr" => "Type",
            "es" => "Tipo",
            _ => "Tipo"
        };
    }

    public static string All(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "All",
            "fr" => "Tous",
            "es" => "Todos",
            _ => "Todos"
        };
    }

    public static string Open(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "Open",
            "fr" => "Ouvrir",
            "es" => "Abrir",
            _ => "Abrir"
        };
    }

    public static string SeriesWithoutAt(int count, bool moduleMissing, string? cultureName = null)
    {
        var language = Language(cultureName);
        if (language == "en")
        {
            var message =
                $"{count} series created locally still without AT communication. In Series, use «Communicate fiscal series».";
            return moduleMissing
                ? message + " The fiscal certification module is not installed on this station."
                : message;
        }

        var pt =
            $"{count} série(s) criadas localmente ainda sem comunicação à AT. Em Séries, use «Comunicar série fiscal».";
        return moduleMissing
            ? pt + " O módulo de certificação fiscal não está instalado neste posto."
            : pt;
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
