using System.Globalization;
using LogicPOS.Application.Features.System.Users;
using Microsoft.EntityFrameworkCore;
using LogicPOS.Core.BackOffice;
using LogicPOS.Domain.Entities.Finance.Documents.Receipts.Settlement;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Core.Tests.BackOffice;

public class BackOfficeAnomalyTests
{
    [Fact]
    public void Receipt_list_profile_includes_number_customer_and_total()
    {
        var columns = ListingColumns.For(
            "Recibos",
            ["Date", "Number", "Status", "CustomerId", "FiscalNumber", "TotalFinal", "UpdatedAt"]);

        Assert.Contains(columns, column => column.Key == "Number" && column.Header == "Número do Doc.");
        Assert.Contains(columns, column => column.Key == "CustomerId" && column.Header == "Entidade");
        Assert.Contains(columns, column => column.Key == "FiscalNumber");
        Assert.Contains(columns, column => column.Key == "TotalFinal" && column.Header == "Total");
        Assert.Contains(columns, column => column.Key == "Status");
    }

    [Fact]
    public void Receipt_notes_and_status_are_searchable_labels()
    {
        var first = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var second = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var ids = ReceiptListFormatting.DocumentIds($"{first}, {second};not-a-guid");

        Assert.Equal([first, second], ids);
        Assert.Equal("Normal", ReceiptListFormatting.StatusLabel("N"));
        Assert.Equal("Anulado", ReceiptListFormatting.StatusLabel("A"));
        Assert.Equal("Ana, Bruno", ReceiptListFormatting.JoinDistinct(["Ana", " ", "Ana", "Bruno"]));
    }

    [Fact]
    public void Composed_article_accepts_several_children()
    {
        var parent = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var first = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var second = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var fields = ValidArticle(parent, new Dictionary<string, string>
        {
            ["IsComposed"] = "true",
            ["ChildArticleId:0"] = first.ToString(),
            ["ChildQuantity:0"] = "1",
            ["ChildArticleId:1"] = second.ToString(),
            ["ChildQuantity:1"] = "2,5",
            ["ChildArticleId:2"] = string.Empty,
            ["ChildQuantity:2"] = "1"
        });

        var error = ArticleFormRules.Validate(fields, vatRequiresExemption: false, parent);
        var lines = ArticleFormRules.ReadComposition(fields, parent, out var readError);

        Assert.Null(error);
        Assert.Null(readError);
        Assert.Equal(2, lines.Count);
        Assert.Equal(first, lines[0].ChildId);
        Assert.Equal(1m, lines[0].Quantity);
        Assert.Equal(second, lines[1].ChildId);
        Assert.Equal(2.5m, lines[1].Quantity);
    }

    [Fact]
    public void Composed_article_rejects_duplicate_and_self()
    {
        var parent = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var child = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var duplicate = ValidArticle(parent, new Dictionary<string, string>
        {
            ["IsComposed"] = "true",
            ["ChildArticleId:0"] = child.ToString(),
            ["ChildQuantity:0"] = "1",
            ["ChildArticleId:1"] = child.ToString(),
            ["ChildQuantity:1"] = "1"
        });
        var self = ValidArticle(parent, new Dictionary<string, string>
        {
            ["IsComposed"] = "true",
            ["ChildArticleId"] = parent.ToString(),
            ["ChildQuantity"] = "1"
        });

        Assert.Equal("O artigo associado está repetido.", ArticleFormRules.Validate(duplicate, false, parent));
        Assert.Equal("O artigo composto não pode incluir o próprio artigo.", ArticleFormRules.Validate(self, false, parent));
    }

    [Fact]
    public void Profile_menu_keeps_ungated_titles_and_filters_the_rest()
    {
        Assert.False(ProfileMenuAccess.IsRestricted("Backup DB"));
        Assert.False(ProfileMenuAccess.IsRestricted("Novo Doc."));
        Assert.True(ProfileMenuAccess.IsRestricted("Artigos"));
        Assert.True(ProfileMenuAccess.IsRestricted("Recibos"));

        var allowed = ProfileMenuAccess.AllowedTitles(
        [
            UserProfilePermissions.Articles.BACKOFFICE_MAN_ARTICLE_MENU,
            UserProfilePermissions.Finance.Documents.BACKOFFICE_MAN_DOCUMENTSPAYMENTS_MENU
        ]);

        Assert.Contains("Artigos", allowed);
        Assert.Contains("Recibos", allowed);
        Assert.DoesNotContain("Famílias", allowed);
        Assert.DoesNotContain("Clientes", allowed);
    }

    [Theory]
    [InlineData("pt-PT")]
    [InlineData("en-US")]
    public void Decimal_text_treats_dot_and_comma_as_decimal_not_thousands(string culture)
    {
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
        try
        {
            Assert.True(FormDecimal.TryParse("2.5", out var dotted));
            Assert.Equal(2.5m, dotted);
            Assert.True(FormDecimal.TryParse("2,5", out var comma));
            Assert.Equal(2.5m, comma);
            Assert.False(FormDecimal.TryParse("1.234,56", out _));
            Assert.False(FormDecimal.TryParse("1,234.56", out _));
            Assert.True(FormDecimal.TryParse("1.50", out var twoPlaces));
            Assert.Equal(1.50m, twoPlaces);
            Assert.True(FormDecimal.TryParse("1500", out var plain));
            Assert.Equal(1500m, plain);

            var parent = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var child = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var fields = ValidArticle(parent, new Dictionary<string, string>
            {
                ["IsComposed"] = "true",
                ["ChildArticleId"] = child.ToString(),
                ["ChildQuantity"] = "2.5"
            });
            var lines = ArticleFormRules.ReadComposition(fields, parent, out var error);
            Assert.Null(error);
            Assert.Equal(2.5m, lines[0].Quantity);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void Empty_or_unchecked_composition_clears_children_and_errors_are_kept()
    {
        var parent = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var child = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var empty = ValidArticle(parent, new Dictionary<string, string>
        {
            ["IsComposed"] = "true",
            ["ChildArticleId:0"] = string.Empty
        });
        var off = ValidArticle(parent, new Dictionary<string, string>
        {
            ["IsComposed"] = "false",
            ["ChildArticleId:0"] = child.ToString(),
            ["ChildQuantity:0"] = "1"
        });
        var invalid = ValidArticle(parent, new Dictionary<string, string>
        {
            ["IsComposed"] = "true",
            ["ChildArticleId:0"] = parent.ToString(),
            ["ChildQuantity:0"] = "1"
        });

        var cleared = ArticleFormRules.PlanComposition(empty, parent);
        var removed = ArticleFormRules.PlanComposition(off, parent);
        var failed = ArticleFormRules.PlanComposition(invalid, parent);

        Assert.True(cleared.ClearExisting);
        Assert.Empty(cleared.Lines);
        Assert.Null(cleared.Error);
        Assert.True(removed.ClearExisting);
        Assert.Empty(removed.Lines);
        Assert.False(failed.ClearExisting);
        Assert.Equal("O artigo composto não pode incluir o próprio artigo.", failed.Error);
    }

    [Fact]
    public void Partial_receipt_credit_is_the_amount_received()
    {
        Assert.Equal(60m, ReceiptCredit.Outstanding(100m, 40m));
        Assert.Equal(0m, ReceiptCredit.Outstanding(100m, 100m));

        var invoiceId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var allocations = DocumentSettlementService.AllocateInvoicePayments(
        [
            new DocumentSettlementService.SettlementDocument(
                invoiceId,
                "FT",
                TotalFinal: 100m,
                TotalTax: 23m,
                WithholdingTaxAmount: 0m,
                TotalPaid: 40m,
                CreatedAt: DateTime.UtcNow)
        ],
        receiptCashAmount: 60m,
        creditNotePoolTotal: 0m);

        Assert.Equal(60m, allocations[0].AmountToPay);
        Assert.NotEqual(100m, allocations[0].AmountToPay);
    }

    [Fact]
    public void Owner_profile_keeps_the_permissions_menu()
    {
        Assert.True(ProfileMenuAccess.KeepsPermissionsMenu(ProfileMenuAccess.OwnerProfileId));
        Assert.False(ProfileMenuAccess.KeepsPermissionsMenu(Guid.NewGuid()));

        var allowed = ProfileMenuAccess.AllowedTitles(
            [UserProfilePermissions.Articles.BACKOFFICE_MAN_ARTICLE_MENU],
            keepPermissionsMenu: true);
        Assert.Contains(ProfileMenuAccess.PermissionsTitle, allowed);
        Assert.Contains("Artigos", allowed);
        Assert.DoesNotContain("Clientes", allowed);
    }

    [Fact]
    public void Bulk_location_move_keeps_status_and_does_not_delete_an_empty_row()
    {
        var plan = LocalStockService.PlanBulkLocation(ArticleSerialNumberStatus.Available);
        Assert.False(plan.DeleteSourceWhenEmpty);
        Assert.Equal(ArticleSerialNumberStatus.Available, plan.DestinationStatus);
    }

    [Fact]
    public void Lookup_choice_ignores_a_partial_match()
    {
        var cafe = new LookupOption { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Label = "Café - Expresso", Search = "A1 Café Expresso" };
        var other = new LookupOption { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Label = "Café", Search = "A2 Café" };
        var items = new[] { cafe, other };

        Assert.Null(LookupChoice.Choose(null, null, "Expresso", items));
        Assert.Null(LookupChoice.Choose(null, null, "A1", items));
        Assert.Equal(other.Id, LookupChoice.Choose(null, null, "Café", items)?.Id);
        Assert.Equal(cafe.Id, LookupChoice.Choose(null, cafe, "Expresso", items)?.Id);
    }

    [Fact]
    public void Three_digit_fraction_is_rejected_for_money_and_for_future_MoneyFormat()
    {
        // POS Desktop MoneyFormat must call FormDecimal and keep this rule. Do not parse "1.500" as 1.5 or 1500.
        Assert.False(FormDecimal.TryParse("1.500", out _, out var dotted));
        Assert.Equal(FormDecimalIssue.AmbiguousThousands, dotted);
        Assert.False(FormDecimal.TryParse("1,500", out _, out var comma));
        Assert.Equal(FormDecimalIssue.AmbiguousThousands, comma);
        Assert.False(FormDecimal.TryParse("-1.500", out _, out var negative));
        Assert.Equal(FormDecimalIssue.AmbiguousThousands, negative);
        Assert.True(FormDecimal.TryParse("1.5000", out var four));
        Assert.Equal(1.5000m, four);
        Assert.Contains("3 dígitos", FormDecimal.Rule, StringComparison.Ordinal);

        var parent = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var fields = ValidArticle(parent, new Dictionary<string, string>
        {
            ["Price1"] = "1.500",
            ["MinimumStock"] = "1,500"
        });
        var error = ArticleFormRules.Validate(fields, vatRequiresExemption: false, parent);
        Assert.Equal(FormDecimal.Rule, error);
    }

    [Fact]
    public void Save_errors_keep_business_text_and_hide_provider_text()
    {
        var missing = new InvalidOperationException("Registo não encontrado.");
        Assert.Equal("Registo não encontrado.", SaveExceptionText.Message(missing));

        var duplicateCode = new DbUpdateException(
            "An error occurred while saving the entity changes. See the inner exception for details.",
            new Exception("SQLite Error 19: 'UNIQUE constraint failed: Articles.Code'"));
        var codeMessage = SaveExceptionText.Message(duplicateCode);
        Assert.Equal("Já existe um registo com este código.", codeMessage);
        Assert.DoesNotContain("SQLite", codeMessage, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DbUpdate", codeMessage, StringComparison.OrdinalIgnoreCase);

        var duplicateNif = new DbUpdateException(
            "An error occurred while saving the entity changes. See the inner exception for details.",
            new Exception("UNIQUE constraint failed: Customers.FiscalNumber"));
        Assert.Equal("Já existe um registo com este NIF.", SaveExceptionText.Message(duplicateNif));

        var technical = new DbUpdateException("An error occurred while saving the entity changes. See the inner exception for details.");
        Assert.Equal(SaveExceptionText.Generic, SaveExceptionText.Message(technical));
        Assert.Equal("Já existe um registo com este NIF.", SaveExceptionText.Describe(duplicateNif));
    }

    private static Dictionary<string, string> ValidArticle(Guid id, Dictionary<string, string> extra)
    {
        var fields = new Dictionary<string, string>
        {
            ["Code"] = "A1",
            ["Designation"] = "Artigo teste",
            ["SubfamilyId"] = Guid.NewGuid().ToString(),
            ["TypeId"] = Guid.NewGuid().ToString(),
            ["ClassId"] = Guid.NewGuid().ToString(),
            ["MeasurementUnitId"] = Guid.NewGuid().ToString(),
            ["SizeUnitId"] = Guid.NewGuid().ToString(),
            ["VatDirectSellingId"] = Guid.NewGuid().ToString(),
            ["VatOnTableId"] = Guid.NewGuid().ToString(),
            ["Price1"] = "1"
        };
        foreach (var pair in extra)
        {
            fields[pair.Key] = pair.Value;
        }

        _ = id;
        return fields;
    }
}
