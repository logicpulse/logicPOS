using Avalonia.Controls;
using Avalonia.Interactivity;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class TableSetupWizard : UserControl
{
    private TableSetupDraft? _draft;
    private readonly List<string> _steps = new();
    private int _step;
    private bool _busy;
    private bool _needsPlace = true;

    public TableSetupWizard()
    {
        InitializeComponent();
        TouchFields.Attach(this);
        ListingNotice.BindToast(Notice, this);
    }

    public event EventHandler<string?>? Finished;

    public async Task ShowAsync()
    {
        Notice.Text = string.Empty;
        _step = 0;
        SetBusy(true, "A preparar o assistente de mesa...");
        var wizard = AppComposition.Services?.GetService<ITableSetupWizard>();
        if (wizard is null)
        {
            SetBusy(false);
            Notice.Text = "O assistente de mesa não está disponível.";
            ShowStep();
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
            ShowStep();
            return;
        }

        _needsPlace = _draft.HasPlace == false;
        _steps.Clear();
        if (_needsPlace)
        {
            _steps.Add("Local");
        }

        _steps.Add("Mesa");
        PlaceBox.Text = _draft.PlaceDesignation;
        TableBox.Text = _draft.TableDesignation;
        TableHint.Text = _draft.HasPlace
            ? $"A mesa será criada no local «{_draft.ExistingPlaceLabel}»."
            : "A mesa fica associada ao local definido no passo anterior.";
        Notice.Text = _draft.BlockedReason ?? string.Empty;
        ShowStep();
        SetBusy(false);
    }

    private void SetBusy(bool busy, string? message = null)
    {
        _busy = busy;
        BackButton.IsEnabled = busy == false;
        NextButton.IsEnabled = busy == false;
        FinishButton.IsEnabled = busy == false && string.IsNullOrWhiteSpace(_draft?.BlockedReason);
        CloseButton.IsEnabled = busy == false;
        TabBar.IsEnabled = busy == false;
        if (busy)
        {
            Busy.Show(message);
        }
        else
        {
            Busy.Hide();
        }
    }

    private void ShowStep()
    {
        var onPlace = _needsPlace && _step == 0;
        PlaceStep.IsVisible = onPlace;
        TableStep.IsVisible = onPlace == false;
        BackButton.IsVisible = _step > 0;
        NextButton.IsVisible = _step < _steps.Count - 1;
        FinishButton.IsVisible = _step == _steps.Count - 1;

        TabBar.Children.Clear();
        for (var index = 0; index < _steps.Count; index++)
        {
            var tab = new Button
            {
                Classes = { "bo_fiscal_wizard_tab" },
                Content = $"{index + 1}. {_steps[index]}"
            };
            if (index == _step)
            {
                tab.Classes.Add("bo_fiscal_wizard_tab_on");
            }
            else if (index < _step)
            {
                tab.Classes.Add("bo_fiscal_wizard_tab_done");
            }

            var target = index;
            tab.Click += (_, _) =>
            {
                if (target <= _step)
                {
                    _step = target;
                    Notice.Text = string.Empty;
                    ShowStep();
                }
            };
            TabBar.Children.Add(tab);
        }
    }

    private string? ValidateCurrentStep()
    {
        if (_draft?.BlockedReason is { Length: > 0 })
        {
            return _draft.BlockedReason;
        }

        if (_needsPlace && _step == 0 && string.IsNullOrWhiteSpace(PlaceBox.Text))
        {
            return "Indique a designação do local.";
        }

        if ((_needsPlace == false || _step == 1) && string.IsNullOrWhiteSpace(TableBox.Text))
        {
            return "Indique a designação da mesa.";
        }

        return null;
    }

    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        if (_step > 0)
        {
            _step--;
            Notice.Text = string.Empty;
            ShowStep();
        }
    }

    private void OnNextClick(object? sender, RoutedEventArgs e)
    {
        var error = ValidateCurrentStep();
        if (error is not null)
        {
            Notice.Text = error;
            return;
        }

        Notice.Text = string.Empty;
        _step++;
        ShowStep();
    }

    private async void OnFinishClick(object? sender, RoutedEventArgs e)
    {
        if (_busy)
        {
            return;
        }

        var error = ValidateCurrentStep();
        if (error is not null)
        {
            Notice.Text = error;
            return;
        }

        var wizard = AppComposition.Services?.GetService<ITableSetupWizard>();
        if (wizard is null)
        {
            Notice.Text = "O assistente de mesa não está disponível.";
            return;
        }

        Notice.Text = string.Empty;
        SetBusy(true, "A criar a mesa...");
        try
        {
            var result = await wizard.CreateAsync(new TableSetupRequest
            {
                PlaceDesignation = PlaceBox.Text?.Trim() ?? string.Empty,
                TableDesignation = TableBox.Text?.Trim() ?? string.Empty,
                ExistingPlaceId = _needsPlace ? null : _draft?.ExistingPlaceId
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
