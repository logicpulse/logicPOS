using Avalonia.Controls;
using Avalonia.VisualTree;

namespace LogicPOS.App.Views;

/// <summary>
/// Resolve o host FO/BO (UserControl) — TopLevel no desktop é o shell Window, não o IOfficeSurface.
/// </summary>
internal static class OfficeSurface
{
    public static IOfficeSurface? Find(Control control)
    {
        if (control.FindAncestorOfType<BackOfficeWindow>() is { } backOffice)
        {
            return backOffice;
        }

        if (control.FindAncestorOfType<PosWindow>() is { } pos)
        {
            return pos;
        }

        return TopLevel.GetTopLevel(control) as IOfficeSurface;
    }
}
