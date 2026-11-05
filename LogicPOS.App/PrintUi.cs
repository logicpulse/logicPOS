using Avalonia;
using Avalonia.Controls;
using LogicPOS.App.Views;

namespace LogicPOS.App;

/// <summary>Shared print feedback: spinner + top toast (sweet-alert style).</summary>
internal static class PrintUi
{
    public static async Task RunAsync(Visual source, ListingBusy? busy, Func<Task> print, string successMessage = "Documento enviado para a impressora.")
    {
        busy?.Show("A imprimir...");
        try
        {
            await print();
            Toast.Success(source, successMessage);
        }
        catch (Exception exception)
        {
            Toast.Error(
                source,
                string.IsNullOrWhiteSpace(exception.Message)
                    ? "Não foi possível imprimir o documento."
                    : exception.Message);
        }
        finally
        {
            busy?.Hide();
        }
    }

    public static void Success(Visual source, string message) => Toast.Success(source, message);

    public static void Error(Visual source, string message) => Toast.Error(source, message);

    public static void Warning(Visual source, string message) => Toast.Warning(source, message);
}
