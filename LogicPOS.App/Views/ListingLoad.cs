using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using LogicPOS.App;

namespace LogicPOS.App.Views;

/// <summary>
/// Feedback da UI: esconde o texto inline e mostra toast estilo sweet-alert.
/// </summary>
internal static class ListingNotice
{
    private static readonly Regex ListingCount = new(@"^\d+\s+registo", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static void HideWhenEmpty(TextBlock block)
    {
        void Apply() => block.IsVisible = string.IsNullOrEmpty(block.Text) == false;
        block.PropertyChanged += (_, args) =>
        {
            if (args.Property == TextBlock.TextProperty)
            {
                Apply();
            }
        };
        Apply();
    }

    /// <summary>
    /// Qualquer texto escrito neste bloco passa a toast (erro / info / sucesso).
    /// Contagens tipo "12 registo(s)" e textos de progresso ("A …") podem ficar inline.
    /// </summary>
    public static void BindToast(
        TextBlock block,
        Visual source,
        bool keepCountsInline = false,
        bool skipProgress = false,
        Func<string, bool>? keepInline = null)
    {
        var suppressing = false;
        block.IsVisible = false;
        block.PropertyChanged += (_, args) =>
        {
            if (suppressing || args.Property != TextBlock.TextProperty)
            {
                return;
            }

            var text = block.Text?.Trim() ?? string.Empty;
            if (text.Length == 0)
            {
                block.IsVisible = false;
                return;
            }

            if (keepCountsInline && ListingCount.IsMatch(text))
            {
                block.IsVisible = true;
                return;
            }

            if (skipProgress && IsProgress(text))
            {
                block.IsVisible = true;
                return;
            }

            if (keepInline?.Invoke(text) == true)
            {
                block.IsVisible = true;
                return;
            }

            block.IsVisible = false;
            Toast.Show(source, InferKind(text), text);

            suppressing = true;
            try
            {
                block.Text = string.Empty;
            }
            finally
            {
                suppressing = false;
            }
        };
    }

    private static bool IsProgress(string text)
        => text.StartsWith("A ", StringComparison.OrdinalIgnoreCase)
           && (text.EndsWith("...", StringComparison.Ordinal) || text.EndsWith("…", StringComparison.Ordinal));

    private static ToastKind InferKind(string text)
    {
        var folded = text.ToLowerInvariant();
        if (ContainsAny(folded,
                "sucesso",
                "com sucesso",
                "enviado",
                "anulado.",
                "anulado ",
                "gravado",
                "eliminado",
                "preenchidos",
                "atualizado",
                "criado",
                "emitido",
                "restaur"))
        {
            return ToastKind.Success;
        }

        if (ContainsAny(folded,
                "erro",
                "impossível",
                "nao foi possivel",
                "não foi possível",
                "falhou",
                "falha",
                "inválid",
                "invalido",
                "exceção",
                "exception",
                "sem morada",
                "não encontrado",
                "nao encontrado"))
        {
            return ToastKind.Error;
        }

        if (ContainsAny(folded,
                "selecione",
                "seleccione",
                "indique",
                "preencha",
                "escolha",
                "obrigatório",
                "obrigatorio",
                "atenção",
                "atencao",
                "aviso",
                "não pode",
                "nao pode",
                "tem de",
                "deve "))
        {
            return ToastKind.Warning;
        }

        return ToastKind.Info;
    }

    private static bool ContainsAny(string text, params string[] needles)
    {
        foreach (var needle in needles)
        {
            if (text.Contains(needle, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}

internal sealed class ListingLoad
{
    private readonly Control _overlay;
    private int _depth;

    public ListingLoad(Control overlay) => _overlay = overlay;

    public async Task RunAsync(Func<Task> action)
    {
        _depth++;
        if (_depth == 1)
        {
            // Show immediately — a delay made Documentos look like it had no loading
            // when ListByPeriod finished under ~180ms.
            ShowOverlay();
        }

        try
        {
            await action();
        }
        finally
        {
            _depth--;
            if (_depth <= 0)
            {
                _depth = 0;
                HideOverlay();
            }
        }
    }

    private void ShowOverlay()
    {
        void Apply()
        {
            if (_overlay is ListingBusy busy)
            {
                busy.Show();
                return;
            }

            _overlay.IsVisible = true;
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            Apply();
        }
        else
        {
            Dispatcher.UIThread.Post(Apply);
        }
    }

    private void HideOverlay()
    {
        void Apply()
        {
            if (_overlay is ListingBusy busy)
            {
                busy.Hide();
                return;
            }

            _overlay.IsVisible = false;
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            Apply();
        }
        else
        {
            Dispatcher.UIThread.Post(Apply);
        }
    }
}
