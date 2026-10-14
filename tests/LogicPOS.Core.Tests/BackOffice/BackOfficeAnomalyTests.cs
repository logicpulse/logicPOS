using LogicPOS.Application.Features.System.Users;
using LogicPOS.Core.BackOffice;

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
