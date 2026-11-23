using System.Globalization;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class SidebarMenuTextTests
{
    /// <summary>
    /// Every BackOffice sidebar key from <c>BackOfficeWindow.BuildMenu</c> / <c>ApplySidebarTexts</c>
    /// (Portuguese navigation keys — must stay unchanged).
    /// </summary>
    private static readonly string[] MenuKeys =
    [
        "Documentos",
        "Novo Doc.",
        "Emissão Recibos",
        "Recibos",
        "Conta corrente",
        "Sessões de Trabalho",
        "Relatórios",
        "Artigos",
        "Famílias",
        "Subfamílias",
        "Tipo de artigos",
        "Classe do artigo",
        "Tipo de Preço",
        "Gestão de Stocks",
        "Importar Artigos",
        "Exportar Artigos",
        "Informação fiscal",
        "Abertura de ano fiscal",
        "Séries",
        "Tipo de documento",
        "Taxas de imposto",
        "Motivo de isenção de IVA",
        "Condições de Pagamento",
        "Métodos de pagamento",
        "Clientes",
        "Tipo de clientes",
        "Grupo de descontos",
        "Importar Clientes",
        "Exportar Clientes",
        "Utilizadores",
        "Permissões",
        "Grupo de comissões",
        "Dispositivos",
        "Impressoras",
        "Dispositivos de Entrada",
        "Display de Cliente",
        "Balanças",
        "Outras Tabelas",
        "País",
        "Códigos-postais",
        "Moeda",
        "Locais",
        "Mesas",
        "Tipo de Movimento",
        "Unidades de medida",
        "Unidades de tamanho",
        "Feriados",
        "Armazém",
        "Tipos de impressora",
        "Configuração",
        "Parâmetros da Empresa",
        "Parâmetros de Sistema",
        "Terminais",
        "SAFT",
        "SAF-T ano",
        "SAF-T último mês",
        "SAF-T período",
        "AGT",
        "AGT ano",
        "AGT último mês",
        "AGT período",
        "Séries AGT",
        "Documentos AGT",
        "Sistema",
        "Notificações",
        "Registo de alterações",
        "Backup DB",
        "Restaurar DB",
        "Restaurar de ficheiro",
        "Atualizar",
        "Sair da Sessão",
        "Primeiros passos",
        "Dashboard",
        "FrontOffice",
        "Sair da Aplicação",
        "Acerca"
    ];

    [Fact]
    public void Pt_ui_culture_keeps_portuguese_labels_when_os_culture_is_english()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        var previousUi = CultureInfo.CurrentUICulture;
        var previousDefault = CultureInfo.DefaultThreadCurrentUICulture;
        var path = Path.Combine(AppContext.BaseDirectory, "ui-culture.txt");
        var previousFile = File.Exists(path) ? File.ReadAllText(path) : null;
        try
        {
            File.WriteAllText(path, "pt-PT");
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Assert.Equal("en", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            Assert.Equal("pt-PT", UiCulture.Read(null));
            Assert.Equal("Primeiros passos", SidebarMenuText.Get("Primeiros passos"));
            Assert.Equal("Documentos", SidebarMenuText.Get("Documentos"));
            Assert.Equal("Outras Tabelas", SidebarMenuText.Get("Outras Tabelas"));
            Assert.Equal("Sair da Aplicação", SidebarMenuText.Get("Sair da Aplicação"));
            Assert.Equal("FrontOffice", SidebarMenuText.Get("FrontOffice"));
            Assert.Equal("Restaurar de ficheiro", SidebarMenuText.Get("Restaurar de ficheiro"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUi;
            CultureInfo.DefaultThreadCurrentUICulture = previousDefault;
            RestoreCultureFile(path, previousFile);
        }
    }

    [Fact]
    public void En_ui_culture_translates_sidebar_labels()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "ui-culture.txt");
        var previousFile = File.Exists(path) ? File.ReadAllText(path) : null;
        try
        {
            File.WriteAllText(path, "en-US");
            Assert.Equal("Getting started", SidebarMenuText.Get("Primeiros passos"));
            Assert.Equal("Documents", SidebarMenuText.Get("Documentos"));
            Assert.Equal("Other tables", SidebarMenuText.Get("Outras Tabelas"));
            Assert.Equal("Exit", SidebarMenuText.Get("Sair da Aplicação"));
            Assert.Equal("Front office", SidebarMenuText.Get("FrontOffice"));
            Assert.Equal("Restore from file", SidebarMenuText.Get("Restaurar de ficheiro"));
        }
        finally
        {
            RestoreCultureFile(path, previousFile);
        }
    }

    [Fact]
    public void English_table_covers_every_sidebar_menu_key()
    {
        var missing = MenuKeys.Where(key => SidebarMenuText.EnglishKeys.Contains(key) == false).ToArray();
        Assert.True(
            missing.Length == 0,
            "Missing EN captions for: " + string.Join(", ", missing));
    }

    [Fact]
    public void En_ui_culture_translates_every_sidebar_menu_key()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "ui-culture.txt");
        var previousFile = File.Exists(path) ? File.ReadAllText(path) : null;
        try
        {
            File.WriteAllText(path, "en-US");
            var unchanged = new List<string>();
            foreach (var key in MenuKeys)
            {
                var label = SidebarMenuText.Get(key);
                // Acronyms / proper nouns may match the PT key (SAFT, AGT, Dashboard).
                if (label == key && key is not ("SAFT" or "AGT" or "Dashboard"))
                {
                    // Still OK when the EN table maps the key to itself.
                    if (SidebarMenuText.EnglishKeys.Contains(key) == false)
                    {
                        unchanged.Add(key);
                    }
                }
            }

            Assert.True(
                unchanged.Count == 0,
                "No EN caption for: " + string.Join(", ", unchanged));
            Assert.Equal("Restore from file", SidebarMenuText.Get("Restaurar de ficheiro"));
        }
        finally
        {
            RestoreCultureFile(path, previousFile);
        }
    }

    private static void RestoreCultureFile(string path, string? previousFile)
    {
        if (previousFile is null)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        else
        {
            File.WriteAllText(path, previousFile);
        }
    }
}
