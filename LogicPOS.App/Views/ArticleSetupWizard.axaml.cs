using Avalonia.Controls;
using Avalonia.Interactivity;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class ArticleSetupWizard : UserControl
{
    private static readonly string[] Steps = ["Família", "Subfamília", "Artigo"];

    private ArticleSetupDraft? _draft;
    private int _step;
    private bool _busy;

    public ArticleSetupWizard()
    {
        InitializeComponent();
    }

    public event EventHandler<string?>? Finished;

    public async Task ShowAsync()
    {
        Notice.Text = string.Empty;
        _step = 0;
        SetBusy(true, "A preparar o assistente de artigo...");
        var wizard = AppComposition.Services?.GetService<IArticleSetupWizard>();
        if (wizard is null)
        {
            SetBusy(false);
            Notice.Text = "O assistente de artigo não está disponível.";
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

        FamilyBox.Text = _draft.FamilyDesignation;
        SubfamilyBox.Text = _draft.SubfamilyDesignation;
        ArticleBox.Text = _draft.ArticleDesignation;
        PriceBox.Text = _draft.Price;
        Notice.Text = _draft.BlockedReason ?? string.Empty;
        ShowStep();
        SetBusy(false);
    }

    private void SetBusy(bool busy, string? message = null)
    {
        _busy = busy;
        BackButton.IsEnabled = busy == false;
        NextButton.IsEnabled = busy == false;
        FinishButton.IsEnabled = busy == false;
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
        FamilyStep.IsVisible = _step == 0;
        SubfamilyStep.IsVisible = _step == 1;
        ArticleStep.IsVisible = _step == 2;
        BackButton.IsVisible = _step > 0;
        NextButton.IsVisible = _step < Steps.Length - 1;
        FinishButton.IsVisible = _step == Steps.Length - 1;
        FinishButton.IsEnabled = _busy == false && string.IsNullOrWhiteSpace(_draft?.BlockedReason);

        TabBar.Children.Clear();
        for (var index = 0; index < Steps.Length; index++)
        {
            var tab = new Button
            {
                Classes = { "bo_fiscal_wizard_tab" },
                Content = $"{index + 1}. {Steps[index]}"
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

        return _step switch
        {
            0 when string.IsNullOrWhiteSpace(FamilyBox.Text) => "Indique a designação da família.",
            1 when string.IsNullOrWhiteSpace(SubfamilyBox.Text) => "Indique a designação da subfamília.",
            2 when string.IsNullOrWhiteSpace(ArticleBox.Text) => "Indique a designação do artigo.",
            2 when FormDecimal.TryParse(PriceBox.Text, FormDecimalKind.Money, out var price) == false || price <= 0
                => "Indique um preço válido maior do que zero.",
            _ => null
        };
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

        var wizard = AppComposition.Services?.GetService<IArticleSetupWizard>();
        if (wizard is null)
        {
            Notice.Text = "O assistente de artigo não está disponível.";
            return;
        }

        Notice.Text = string.Empty;
        SetBusy(true, "A criar a família, a subfamília e o artigo...");
        try
        {
            var result = await wizard.CreateAsync(new ArticleSetupRequest
            {
                FamilyDesignation = FamilyBox.Text?.Trim() ?? string.Empty,
                SubfamilyDesignation = SubfamilyBox.Text?.Trim() ?? string.Empty,
                ArticleDesignation = ArticleBox.Text?.Trim() ?? string.Empty,
                Price = PriceBox.Text?.Trim() ?? string.Empty
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
