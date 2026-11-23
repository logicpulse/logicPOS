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

    /// <summary>Documents listing «Valor Pago» column — listing-local EN wording.</summary>
    public static string AmountPaid(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "Amount Paid",
            "fr" => "Montant payé",
            "es" => "Importe pagado",
            _ => "Valor Pago"
        };
    }

    /// <summary>Documents listing «Valor em Dívida» column — listing-local EN wording.</summary>
    public static string AmountDue(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "Amount Due",
            "fr" => "Montant dû",
            "es" => "Importe adeudado",
            _ => "Valor em Dívida"
        };
    }

    public static string MissingFiscalYear(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "Create the fiscal year before issuing documents.",
            "fr" => "Créez l'année fiscale avant d'émettre des documents.",
            "es" => "Cree el año fiscal antes de emitir documentos.",
            _ => "Falta criar o ano fiscal antes de emitir documentos."
        };
    }

    public static string MissingCurrentFiscalYear(int year, string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => $"There is no fiscal year for {year}. Create the current year opening.",
            "fr" => $"Il n'y a pas d'année fiscale pour {year}. Créez l'ouverture de l'année en cours.",
            "es" => $"No hay año fiscal para {year}. Cree la apertura del año en curso.",
            _ => $"Não existe ano fiscal para {year}. Crie a abertura do ano corrente."
        };
    }

    public static string MissingDocumentSeries(string? cultureName = null)
    {
        return Language(cultureName) switch
        {
            "en" => "Document series are missing. Use Fiscal year opening to create them.",
            "fr" => "Il manque des séries de documents. Utilisez l'ouverture d'année fiscale pour les créer.",
            "es" => "Faltan series de documento. Use la apertura de año fiscal para crearlas.",
            _ => "Faltam séries de documento. Use a Abertura de ano fiscal para as criar."
        };
    }

    public static string SeriesWithoutAt(int count, bool moduleMissing, string? cultureName = null)
    {
        var language = Language(cultureName);
        string message = language switch
        {
            "en" =>
                $"{count} series created locally still without AT communication. In Series, use «Communicate fiscal series».",
            "fr" =>
                $"{count} série(s) créée(s) localement encore sans communication à l'AT. Dans Séries, utilisez «Communiquer la série fiscale».",
            "es" =>
                $"{count} serie(s) creada(s) localmente aún sin comunicación a la AT. En Series, use «Comunicar serie fiscal».",
            _ =>
                $"{count} série(s) criadas localmente ainda sem comunicação à AT. Em Séries, use «Comunicar série fiscal»."
        };

        if (moduleMissing == false)
        {
            return message;
        }

        var suffix = language switch
        {
            "en" => " The fiscal certification module is not installed on this station.",
            "fr" => " Le module de certification fiscale n'est pas installé sur ce poste.",
            "es" => " El módulo de certificación fiscal no está instalado en este puesto.",
            _ => " O módulo de certificação fiscal não está instalado neste posto."
        };
        return message + suffix;
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
