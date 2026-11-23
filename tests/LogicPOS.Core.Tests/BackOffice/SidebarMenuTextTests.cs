using System.Globalization;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class SidebarMenuTextTests
{
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
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUi;
            CultureInfo.DefaultThreadCurrentUICulture = previousDefault;
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
        }
        finally
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
}
