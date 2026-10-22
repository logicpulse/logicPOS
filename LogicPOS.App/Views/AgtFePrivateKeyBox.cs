using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace LogicPOS.App.Views;

/// <summary>
/// AGT-FE PEM private key: idle state matches a normal single-line company field
/// (preview = first line); expands to multiline with the full key only while focused.
/// </summary>
internal static class AgtFePrivateKeyBox
{
    public const string Token = "AGT_FE_RSA_PRIVATE_KEY";
    private const double CollapsedHeight = 32;

    public static bool IsToken(string? token)
        => string.Equals(token, Token, StringComparison.OrdinalIgnoreCase);

    public static TextBox Create(string? value)
    {
        var state = new PemState { Full = value ?? string.Empty };
        var box = new TextBox
        {
            Tag = state,
            Text = Preview(state.Full)
        };
        Collapse(box, state);
        box.GotFocus += (_, _) => Expand(box, state);
        box.LostFocus += (_, _) =>
        {
            state.Full = box.Text ?? string.Empty;
            Collapse(box, state);
        };
        box.TextChanged += (_, _) =>
        {
            if (state.Expanded)
            {
                state.Full = box.Text ?? string.Empty;
            }
        };
        return box;
    }

    public static bool TryRead(Control? input, out string value)
    {
        if (input is TextBox box && box.Tag is PemState state)
        {
            value = state.Expanded ? (box.Text ?? string.Empty) : state.Full;
            return true;
        }

        value = string.Empty;
        return false;
    }

    private static void Expand(TextBox box, PemState state)
    {
        state.Expanded = true;
        box.Text = state.Full;
        box.Classes.Clear();
        box.Classes.Add("bo_entity_pem");
        box.AcceptsReturn = true;
        box.TextWrapping = TextWrapping.Wrap;
        box.ClearValue(Layoutable.HeightProperty);
        box.MinHeight = 220;
        box.MaxHeight = 320;
        box.ClipToBounds = true;
        box.VerticalContentAlignment = VerticalAlignment.Top;
        ScrollViewer.SetVerticalScrollBarVisibility(box, ScrollBarVisibility.Auto);
        ScrollViewer.SetHorizontalScrollBarVisibility(box, ScrollBarVisibility.Disabled);
    }

    private static void Collapse(TextBox box, PemState state)
    {
        state.Expanded = false;
        box.Text = Preview(state.Full);
        box.Classes.Clear();
        box.Classes.Add("bo_entity_input");
        box.AcceptsReturn = false;
        box.TextWrapping = TextWrapping.NoWrap;
        box.Height = CollapsedHeight;
        box.MinHeight = CollapsedHeight;
        box.MaxHeight = CollapsedHeight;
        box.ClipToBounds = true;
        box.VerticalContentAlignment = VerticalAlignment.Center;
        ScrollViewer.SetVerticalScrollBarVisibility(box, ScrollBarVisibility.Disabled);
        ScrollViewer.SetHorizontalScrollBarVisibility(box, ScrollBarVisibility.Disabled);
    }

    private static string Preview(string full)
    {
        if (string.IsNullOrEmpty(full))
        {
            return string.Empty;
        }

        var end = full.IndexOfAny(['\r', '\n']);
        return end < 0 ? full : full[..end];
    }

    private sealed class PemState
    {
        public string Full { get; set; } = string.Empty;
        public bool Expanded { get; set; }
    }
}
