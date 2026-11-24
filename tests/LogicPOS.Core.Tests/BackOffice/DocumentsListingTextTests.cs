using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class DocumentsListingTextTests
{
    [Fact]
    public void Selected_count_follows_culture()
    {
        Assert.Equal("2 documento(s) selecionado(s)", DocumentsListingText.SelectedCount(2, "pt-PT"));
        Assert.Equal("2 document(s) selected", DocumentsListingText.SelectedCount(2, "en-US"));
    }

    [Fact]
    public void Empty_period_follows_culture()
    {
        Assert.Equal("Não existem documentos neste período.", DocumentsListingText.EmptyPeriod("pt-AO"));
        Assert.Equal("There are no documents in this period.", DocumentsListingText.EmptyPeriod("en-US"));
    }

    [Fact]
    public void Type_all_open_and_series_banner_follow_culture()
    {
        Assert.Equal("Tipo", DocumentsListingText.Type("pt-PT"));
        Assert.Equal("Type", DocumentsListingText.Type("en-US"));
        Assert.Equal("Todos", DocumentsListingText.All("pt-PT"));
        Assert.Equal("All", DocumentsListingText.All("en-US"));
        Assert.Equal("Abrir", DocumentsListingText.Open("pt-PT"));
        Assert.Equal("Open", DocumentsListingText.Open("en-US"));

        var pt = DocumentsListingText.SeriesWithoutAt(17, moduleMissing: false, "pt-PT");
        Assert.Contains("série(s) criadas localmente", pt, StringComparison.Ordinal);
        var en = DocumentsListingText.SeriesWithoutAt(17, moduleMissing: true, "en-US");
        Assert.Contains("series created locally", en, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("certification module", en, StringComparison.OrdinalIgnoreCase);
        var fr = DocumentsListingText.SeriesWithoutAt(17, moduleMissing: false, "fr-FR");
        Assert.Contains("série(s) créée(s) localement", fr, StringComparison.Ordinal);
        var es = DocumentsListingText.SeriesWithoutAt(17, moduleMissing: true, "es-ES");
        Assert.Contains("serie(s) creada(s) localmente", es, StringComparison.Ordinal);
        Assert.Contains("certificación fiscal", es, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Startup_readiness_messages_follow_culture()
    {
        Assert.Contains("ano fiscal", DocumentsListingText.MissingFiscalYear("pt-PT"), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("fiscal year", DocumentsListingText.MissingFiscalYear("en-US"), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("2026", DocumentsListingText.MissingCurrentFiscalYear(2026, "en-US"), StringComparison.Ordinal);
        Assert.Contains("séries", DocumentsListingText.MissingDocumentSeries("pt-PT"), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("series", DocumentsListingText.MissingDocumentSeries("en-US"), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Amount_paid_and_due_use_listing_local_english()
    {
        Assert.Equal("Valor Pago", DocumentsListingText.AmountPaid("pt-PT"));
        Assert.Equal("Amount Paid", DocumentsListingText.AmountPaid("en-US"));
        Assert.Equal("Valor em Dívida", DocumentsListingText.AmountDue("pt-PT"));
        Assert.Equal("Amount Due", DocumentsListingText.AmountDue("en-US"));
    }

    [Fact]
    public void Tip_keys_resolve_in_pt_en_fr_es_and_keep_pt_for_ao_mz()
    {
        Assert.Equal(25, DocumentsListingText.TipKeys.Count);
        foreach (var key in DocumentsListingText.TipKeys)
        {
            var pt = DocumentsListingText.Tip(key, "pt-PT");
            Assert.False(string.IsNullOrWhiteSpace(pt));
            Assert.Equal(pt, DocumentsListingText.Tip(key, "pt-AO"));
            Assert.Equal(pt, DocumentsListingText.Tip(key, "pt-MZ"));
            Assert.False(string.IsNullOrWhiteSpace(DocumentsListingText.Tip(key, "en-US")));
            Assert.False(string.IsNullOrWhiteSpace(DocumentsListingText.Tip(key, "fr-FR")));
            Assert.False(string.IsNullOrWhiteSpace(DocumentsListingText.Tip(key, "es-ES")));
        }

        Assert.Equal("Exportar para PDF", DocumentsListingText.Tip("export-pdf", "pt-PT"));
        Assert.Equal("Export to PDF", DocumentsListingText.Tip("export-pdf", "en-US"));
        Assert.Equal("Escolher no calendário", DocumentsListingText.Tip("choose-calendar", "pt-PT"));
        Assert.Equal("Choose on the calendar", DocumentsListingText.Tip("choose-calendar", "en-US"));
        Assert.Equal("Teclado virtual", DocumentsListingText.Tip("virtual-keyboard", "pt-PT"));
        Assert.Equal("On-screen keyboard", DocumentsListingText.Tip("virtual-keyboard", "en-US"));
        Assert.Equal("Abrir lista", DocumentsListingText.Tip("open-list", "pt-PT"));
        Assert.Equal("Open list", DocumentsListingText.Tip("open-list", "en-US"));
    }

    [Fact]
    public void Action_bar_tip_keys_follow_culture()
    {
        Assert.Equal("Editar rascunho", DocumentsListingText.Tip("action-edit-draft", "pt-PT"));
        Assert.Equal("Edit draft", DocumentsListingText.Tip("action-edit-draft", "en-US"));
        Assert.Equal("Comunicar a guia à AT", DocumentsListingText.Tip("action-send-at", "pt-PT"));
        Assert.Equal("Submit transport document to AT", DocumentsListingText.Tip("action-send-at", "en-US"));
        Assert.Equal("Enviar Email", DocumentsListingText.Tip("action-send-email", "pt-PT"));
        Assert.Equal("Send email", DocumentsListingText.Tip("action-send-email", "en-US"));
        Assert.Equal("Abrir", DocumentsListingText.Tip("open", "pt-PT"));
        Assert.Equal("Open", DocumentsListingText.Tip("open", "en-US"));
    }
}
