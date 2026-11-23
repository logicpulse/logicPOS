using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class BackOfficeFileRestoreTests
{
    [Fact]
    public void Menu_title_is_recognized()
    {
        Assert.True(BackOfficeFileRestore.IsMenuTitle("Restaurar de ficheiro"));
        Assert.False(BackOfficeFileRestore.IsMenuTitle("Restaurar DB"));
        Assert.False(BackOfficeFileRestore.IsMenuTitle("Restore from file"));
    }

    [Fact]
    public void English_sidebar_caption_exists_for_file_restore()
    {
        Assert.Contains(BackOfficeFileRestore.MenuTitle, SidebarMenuText.EnglishKeys);
        Assert.Equal("Restore from file", SidebarMenuText.Get(BackOfficeFileRestore.MenuTitle, "en-US"));
        Assert.Equal("Restaurer depuis un fichier", SidebarMenuText.Get(BackOfficeFileRestore.MenuTitle, "fr-FR"));
    }
}
