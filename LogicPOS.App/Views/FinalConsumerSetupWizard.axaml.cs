using Avalonia.Controls;
using Avalonia.Interactivity;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class FinalConsumerSetupWizard : UserControl
{
    private FinalConsumerSetupDraft? _draft;
    private bool _busy;

    public FinalConsumerSetupWizard()
    {
        InitializeComponent();
    }

    public event EventHandler<string?>? Finished;

    public async Task ShowAsync()
    {
        Notice.Text = string.Empty;
        SetBusy(true, "A preparar o cliente de balcão...");
        var wizard = AppComposition.Services?.GetService<IFinalConsumerSetupWizard>();
        if (wizard is null)
        {
            SetBusy(false);
            Notice.Text = "O assistente de cliente de balcão não está disponível.";
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

        NameBox.Text = _draft.Name;
        FiscalBox.Text = _draft.FiscalNumber;
        Notice.Text = _draft.AlreadyExists
            ? $"O cliente «{_draft.Name}» já existe."
            : _draft.BlockedReason ?? string.Empty;
        FinishButton.IsEnabled = _draft.AlreadyExists == false && string.IsNullOrWhiteSpace(_draft.BlockedReason);
        SetBusy(false);
    }

    private void SetBusy(bool busy, string? message = null)
    {
        _busy = busy;
        FinishButton.IsEnabled = busy == false
            && _draft?.AlreadyExists != true
            && string.IsNullOrWhiteSpace(_draft?.BlockedReason);
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

    private async void OnFinishClick(object? sender, RoutedEventArgs e)
    {
        if (_busy)
        {
            return;
        }

        var wizard = AppComposition.Services?.GetService<IFinalConsumerSetupWizard>();
        if (wizard is null)
        {
            Notice.Text = "O assistente de cliente de balcão não está disponível.";
            return;
        }

        Notice.Text = string.Empty;
        SetBusy(true, "A criar o cliente de balcão...");
        try
        {
            var result = await wizard.CreateAsync(new FinalConsumerSetupRequest
            {
                Name = NameBox.Text?.Trim() ?? "Consumidor Final",
                FiscalNumber = FiscalBox.Text?.Trim() ?? "999999990"
            });
            if (result.Ok == false)
            {
                Notice.Text = result.Message;
                return;
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
}
