using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using LogicPOS.Core;

namespace LogicPOS.App.Views;

/// <summary>
/// Paints <see cref="UiCaption"/> onto controls that still carry Portuguese literals.
/// Runs only while the UI culture is English, and puts Portuguese back when leaving it.
/// </summary>
internal static class EnglishUi
{
    private static int _installed;

    public static void Install()
    {
        if (Interlocked.Exchange(ref _installed, 1) != 0)
        {
            return;
        }

        TextBlock.TextProperty.Changed.AddClassHandler<TextBlock>(OnText);
        TextBox.PlaceholderTextProperty.Changed.AddClassHandler<TextBox>(OnPlaceholder);
        ContentControl.ContentProperty.Changed.AddClassHandler<ContentControl>(OnContent);
        ToolTip.TipProperty.Changed.AddClassHandler<Control>(OnTip);
    }

    public static void Apply(Control root)
    {
        if (UiCaption.IsEnglish)
        {
            Refresh(root);
        }
    }

    public static void RevertToPortuguese(Control root)
    {
        if (UiCaption.IsEnglish == false)
        {
            Refresh(root);
        }
    }

    public static void ApplyOpenWindows()
    {
        if (UiCaption.IsEnglish)
        {
            EachWindow(Refresh);
        }
    }

    public static void RevertOpenWindows()
    {
        if (UiCaption.IsEnglish == false)
        {
            EachWindow(Refresh);
        }
    }

    private static void EachWindow(Action<Control> visit)
    {
        if (global::Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            foreach (var window in desktop.Windows)
            {
                visit(window);
            }
        }

        if (global::Avalonia.Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime single
            && single.MainView is Control view)
        {
            visit(view);
        }
    }

    private static void Refresh(Control root)
    {
        if (root is EntityListing listing)
        {
            listing.ApplyColumnCulture();
        }

        foreach (var child in root.GetVisualDescendants().OfType<EntityListing>())
        {
            child.ApplyColumnCulture();
        }

        Visit(root);
        foreach (var child in root.GetVisualDescendants().OfType<Control>())
        {
            Visit(child);
        }
    }

    private static void Visit(Control control)
    {
        if (Skip(control))
        {
            return;
        }

        switch (control)
        {
            case TextBlock block:
                if (block.Inlines.Count > 0)
                {
                    foreach (var inline in block.Inlines)
                    {
                        if (inline is Run run)
                        {
                            var current = run.Text ?? string.Empty;
                            var next = UiCaption.ForUi(current);
                            if (string.Equals(next, current, StringComparison.Ordinal) == false)
                            {
                                run.Text = next;
                            }
                        }
                    }
                }
                else
                {
                    Set(block.Text, value => block.Text = value);
                }

                break;
            case TextBox box:
                Set(box.PlaceholderText, value => box.PlaceholderText = value);
                break;
            case Button or ToggleButton when control is ContentControl content && content.Content is string caption:
                Set(caption, value => content.Content = value);
                break;
            case Window window:
                Set(window.Title, value => window.Title = value);
                break;
        }

        if (ToolTip.GetTip(control) is string tip)
        {
            Set(tip, value => ToolTip.SetTip(control, value));
        }
    }

    private static void OnText(TextBlock block, AvaloniaPropertyChangedEventArgs change)
    {
        if (UiCaption.IsEnglish == false || change.NewValue is not string text || Skip(block))
        {
            return;
        }

        Set(text, value => block.Text = value);
    }

    private static void OnPlaceholder(TextBox box, AvaloniaPropertyChangedEventArgs change)
    {
        if (UiCaption.IsEnglish == false || change.NewValue is not string text || Skip(box))
        {
            return;
        }

        Set(text, value => box.PlaceholderText = value);
    }

    private static void OnContent(ContentControl control, AvaloniaPropertyChangedEventArgs change)
    {
        if (control is not (Button or ToggleButton)
            || UiCaption.IsEnglish == false
            || change.NewValue is not string text
            || Skip(control))
        {
            return;
        }

        Set(text, value => control.Content = value);
    }

    private static void OnTip(Control control, AvaloniaPropertyChangedEventArgs change)
    {
        if (UiCaption.IsEnglish == false || change.NewValue is not string text || Skip(control))
        {
            return;
        }

        Set(text, value => ToolTip.SetTip(control, value));
    }

    private static void Set(string? current, Action<string> assign)
    {
        var text = current ?? string.Empty;
        var next = UiCaption.ForUi(text);
        if (string.Equals(next, text, StringComparison.Ordinal) == false)
        {
            assign(next);
        }
    }

    private static bool Skip(StyledElement element)
    {
        var node = element;
        for (var depth = 0; node is not null && depth < 12; depth++)
        {
            if (node is DataGridRow)
            {
                return true;
            }

            foreach (var name in node.Classes)
            {
                if (name.StartsWith("pos_menu", StringComparison.Ordinal)
                    || name.StartsWith("pos_list", StringComparison.Ordinal)
                    || name.StartsWith("pos_fo", StringComparison.Ordinal)
                    || name.StartsWith("pos_ticket", StringComparison.Ordinal))
                {
                    return true;
                }
            }

            node = node.Parent;
        }

        return false;
    }
}
