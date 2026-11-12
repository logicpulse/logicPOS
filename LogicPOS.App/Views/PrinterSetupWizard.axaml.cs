using Avalonia.Controls;
using Avalonia.Interactivity;
using LogicPOS.App.Hardware;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.FrontOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class PrinterSetupWizard : UserControl
{
    private const string A4PaperLabel = "A4";

    private PrinterSetupDraft? _draft;
    private bool _busy;

    public PrinterSetupWizard()
    {
        InitializeComponent();
        TouchFields.Attach(this);
        ListingNotice.BindToast(Notice, this);
    }

    public event EventHandler<string?>? Finished;

    private PrinterSetupType? SelectedType
        => TypeBox.SelectedIndex >= 0 && _draft is not null && TypeBox.SelectedIndex < _draft.Types.Count
            ? _draft.Types[TypeBox.SelectedIndex]
            : null;

    public async Task ShowAsync()
    {
        Notice.Text = string.Empty;
        SetBusy(true, "A preparar a impressora...");
        var wizard = AppComposition.Services?.GetService<IPrinterSetupWizard>();
        if (wizard is null)
        {
            SetBusy(false);
            Notice.Text = "O assistente de impressora não está disponível.";
            return;
        }

        try
        {
            _draft = await wizard.LoadAsync();
        }
        catch (Exception exception)
        {
            SetBusy(false);
            Notice.Text = exception.GetBaseException().Message;
            return;
        }

        var printers = InstalledPrinters(out var printersError);
        if (string.IsNullOrWhiteSpace(_draft.PrinterName) == false
            && printers.Contains(_draft.PrinterName, StringComparer.OrdinalIgnoreCase) == false)
        {
            printers.Insert(0, _draft.PrinterName);
        }

        PrinterBox.ItemsSource = printers;
        PrinterBox.SelectedIndex = string.IsNullOrWhiteSpace(_draft.PrinterName)
            ? (printers.Count > 0 ? 0 : -1)
            : printers.FindIndex(item => string.Equals(item, _draft.PrinterName, StringComparison.OrdinalIgnoreCase));
        TypeBox.ItemsSource = _draft.Types.Select(item => item.Designation).ToList();
        TypeBox.SelectedIndex = _draft.Types.ToList().FindIndex(item => item.Id == _draft.TypeId);
        Hint.Text = string.IsNullOrWhiteSpace(_draft.TerminalLabel)
            ? "A impressora fica associada a este terminal."
            : $"A impressora fica associada ao terminal «{_draft.TerminalLabel}». Os talões e faturas do posto saem nela.";
        Notice.Text = _draft.BlockedReason
            ?? printersError
            ?? (printers.Count == 0 ? "Não há impressoras instaladas no Windows." : string.Empty);
        ApplyType();
        SetBusy(false);
    }

    private void ApplyType()
    {
        var thermal = SelectedType?.Thermal == true;
        var current = PaperBox.SelectedItem as string ?? _draft?.PaperLabel;
        var papers = thermal ? ThermalPaper.Labels.ToList() : [A4PaperLabel];
        PaperBox.ItemsSource = papers;
        var match = papers.FindIndex(item => string.Equals(item, current, StringComparison.OrdinalIgnoreCase));
        PaperBox.SelectedIndex = match >= 0 ? match : 0;
        PaperBox.IsEnabled = thermal;
        TestButton.IsVisible = thermal;
    }

    private void OnTypeChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_draft is not null)
        {
            ApplyType();
        }
    }

    private void SetBusy(bool busy, string? message = null)
    {
        _busy = busy;
        FinishButton.IsEnabled = busy == false && string.IsNullOrWhiteSpace(_draft?.BlockedReason);
        TestButton.IsEnabled = busy == false;
        CloseButton.IsEnabled = busy == false;
        if (busy)
        {
            Busy.Show(message);
        }
        else
        {
            Busy.Hide();
        }
    }

    private string? Validate()
    {
        if (_draft?.BlockedReason is { Length: > 0 })
        {
            return _draft.BlockedReason;
        }

        if (SelectedType is null)
        {
            return "Escolha o tipo de impressora.";
        }

        return PrinterBox.SelectedItem is string { Length: > 0 } ? null : "Escolha a impressora do Windows.";
    }

    private async void OnTestClick(object? sender, RoutedEventArgs e)
    {
        if (_busy)
        {
            return;
        }

        var error = Validate();
        if (error is not null)
        {
            Notice.Text = error;
            return;
        }

        var columns = ThermalPaper.Columns(PaperBox.SelectedItem as string);
        var printer = new ThermalPrinterSettings
        {
            Designation = (string)PrinterBox.SelectedItem!,
            ColumnsNormal = columns.Normal,
            ColumnsBold = columns.Bold,
            ColumnsSmall = columns.Small
        };
        Notice.Text = string.Empty;
        SetBusy(true, "A imprimir teste...");
        try
        {
            var failure = await FrontOfficePrinting.PrintTestAsync(printer);
            if (failure is null)
            {
                PrintUi.Success(this, $"Teste enviado para {printer.Designation}.");
            }
            else
            {
                Notice.Text = failure;
            }
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnFinishClick(object? sender, RoutedEventArgs e)
    {
        if (_busy)
        {
            return;
        }

        var error = Validate();
        if (error is not null)
        {
            Notice.Text = error;
            return;
        }

        var wizard = AppComposition.Services?.GetService<IPrinterSetupWizard>();
        if (wizard is null)
        {
            Notice.Text = "O assistente de impressora não está disponível.";
            return;
        }

        var name = (string)PrinterBox.SelectedItem!;
        Notice.Text = string.Empty;
        SetBusy(true, "A associar a impressora...");
        try
        {
            var result = await wizard.SaveAsync(new PrinterSetupRequest
            {
                TypeId = SelectedType!.Id,
                PrinterName = name,
                PaperLabel = PaperBox.SelectedItem as string
            });
            if (result.Ok == false)
            {
                Notice.Text = result.Message;
                return;
            }

            if (result.Thermal == false)
            {
                PdfPrinterMemory.RememberA4(name);
            }

            Finished?.Invoke(this, result.Message);
        }
        catch (Exception exception)
        {
            Notice.Text = exception.GetBaseException().Message;
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Finished?.Invoke(this, null);

    private static List<string> InstalledPrinters(out string? error)
    {
        error = null;
        try
        {
            return WindowsPrinters.Installed().ToList();
        }
        catch (Exception exception)
        {
            error = exception.Message;
            return [];
        }
    }
}
