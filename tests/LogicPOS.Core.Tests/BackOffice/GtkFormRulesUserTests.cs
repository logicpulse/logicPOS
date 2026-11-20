using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Tests.BackOffice;

public class GtkFormRulesUserTests
{
    private static Dictionary<string, string> UserFields(string? profileId) => new()
    {
        ["Name"] = "Novo Utilizador",
        ["ProfileId"] = profileId ?? string.Empty,
    };

    [Fact]
    public void Utilizadores_requires_profile()
    {
        var error = GtkFormRules.Validate("Utilizadores", UserFields(null));
        Assert.Equal("Indique o perfil.", error);
    }

    [Fact]
    public void Utilizadores_accepts_profile()
    {
        Assert.Null(GtkFormRules.Validate("Utilizadores", UserFields(Guid.NewGuid().ToString())));
    }
}