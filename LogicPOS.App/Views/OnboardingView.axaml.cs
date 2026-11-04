using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class OnboardingView : UserControl
{
    public OnboardingView()
    {
        InitializeComponent();
    }

    public event EventHandler<OnboardingStep>? StepRequested;

    public async Task<OnboardingStatus?> ReloadAsync()
    {
        var service = AppComposition.Services?.GetService<IOnboardingService>();
        if (service is null)
        {
            CardsHost.Children.Clear();
            PercentText.Text = "—";
            ProgressLabel.Text = "Serviço de configuração indisponível.";
            return null;
        }

        OnboardingStatus status;
        try
        {
            status = await service.GetStatusAsync();
        }
        catch (Exception exception)
        {
            App.WriteCrash("OnboardingView.ReloadAsync", exception);
            CardsHost.Children.Clear();
            PercentText.Text = "—";
            ProgressLabel.Text = "Não foi possível carregar os passos de configuração.";
            return null;
        }

        PercentText.Text = $"{status.PercentComplete}%";
        ProgressLabel.Text = $"{status.CompletedCount} de {status.TotalCount} passos concluídos";
        CardsHost.Children.Clear();

        foreach (var step in status.Steps)
        {
            CardsHost.Children.Add(BuildCard(step));
        }

        return status;
    }

    private Border BuildCard(OnboardingStep step)
    {
        var body = new StackPanel { Classes = { "onboarding_card_body" } };
        body.Children.Add(new TextBlock
        {
            Classes = { "onboarding_card_title" },
            Text = step.Title,
            TextWrapping = TextWrapping.Wrap
        });
        body.Children.Add(new TextBlock
        {
            Classes = { "onboarding_card_description" },
            Text = step.Description,
            TextWrapping = TextWrapping.Wrap
        });

        var footer = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto") };
        footer.Children.Add(new TextBlock
        {
            Classes = { step.IsComplete ? "onboarding_card_done" : "onboarding_card_pending" },
            Text = step.IsComplete ? "Concluído" : "Pendente",
            VerticalAlignment = VerticalAlignment.Center
        });

        if (step.IsComplete == false)
        {
            var start = new Button
            {
                Classes = { "onboarding_card_start" },
                Content = "Começar",
                Tag = step
            };
            Grid.SetColumn(start, 1);
            start.Click += (_, _) =>
            {
                if (start.Tag is OnboardingStep requested)
                {
                    StepRequested?.Invoke(this, requested);
                }
            };
            footer.Children.Add(start);
        }

        body.Children.Add(footer);

        return new Border
        {
            Classes = { step.IsComplete ? "onboarding_card onboarding_card_complete" : "onboarding_card" },
            Child = body
        };
    }
}
