using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class OnboardingView : UserControl
{
    private const string MenuIconRoot = "avares://LogicPOS.App/Assets/Images/BackOffice/Menu/";

    private static readonly Uri AssetsBase = new("avares://LogicPOS.App/");

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
            ShowUnavailable("Serviço de configuração indisponível.");
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
            ShowUnavailable("Não foi possível carregar os passos de configuração.");
            return null;
        }

        PercentText.Text = $"{status.PercentComplete}%";
        ProgressArc.SweepAngle = 360.0 * status.PercentComplete / 100;
        ProgressLabel.Text = $"{status.CompletedCount} de {status.TotalCount} passos obrigatórios concluídos";
        var next = status.NextStep;
        NextStepLabel.Text = next is null
            ? "Todos os passos obrigatórios estão concluídos."
            : $"Próximo passo recomendado: {next.Title}";

        RequiredCardsHost.Children.Clear();
        OptionalCardsHost.Children.Clear();
        var number = 0;
        foreach (var step in status.Steps)
        {
            if (step.IsOptional)
            {
                OptionalCardsHost.Children.Add(BuildCard(step, number: null, isNext: false));
                continue;
            }

            number++;
            RequiredCardsHost.Children.Add(BuildCard(step, number, ReferenceEquals(step, next)));
        }

        OptionalSection.IsVisible = OptionalCardsHost.Children.Count > 0;
        return status;
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        var columns = e.NewSize.Width >= 1080 ? 3 : e.NewSize.Width >= 680 ? 2 : 1;
        RequiredCardsHost.Columns = columns;
        OptionalCardsHost.Columns = columns;
    }

    private void ShowUnavailable(string message)
    {
        RequiredCardsHost.Children.Clear();
        OptionalCardsHost.Children.Clear();
        OptionalSection.IsVisible = false;
        PercentText.Text = "—";
        ProgressArc.SweepAngle = 0;
        ProgressLabel.Text = message;
        NextStepLabel.Text = string.Empty;
    }

    private Border BuildCard(OnboardingStep step, int? number, bool isNext)
    {
        var icon = new Border
        {
            Classes = { "onboarding_card_icon" },
            Child = new Avalonia.Svg.Skia.Svg(AssetsBase) { Path = IconPath(step.Key) }
        };

        var heading = new StackPanel { Classes = { "onboarding_card_heading" } };
        if (number is not null)
        {
            heading.Children.Add(new TextBlock
            {
                Classes = { "onboarding_card_caption" },
                Text = isNext ? $"Passo {number} · recomendado" : $"Passo {number}"
            });
        }

        heading.Children.Add(new TextBlock
        {
            Classes = { "onboarding_card_title" },
            Text = step.Title,
            TextWrapping = TextWrapping.Wrap
        });

        var header = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto") };
        header.Children.Add(icon);
        Grid.SetColumn(heading, 1);
        header.Children.Add(heading);
        var chip = BuildChip(step);
        Grid.SetColumn(chip, 2);
        header.Children.Add(chip);

        var description = new TextBlock
        {
            Classes = { "onboarding_card_description" },
            Text = step.Description,
            TextWrapping = TextWrapping.Wrap
        };
        Grid.SetRow(description, 1);

        var footer = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto") };
        Grid.SetRow(footer, 2);
        if (string.IsNullOrWhiteSpace(step.StatusText) == false)
        {
            footer.Children.Add(new TextBlock
            {
                Classes = { "onboarding_card_detail" },
                Text = step.StatusText,
                VerticalAlignment = VerticalAlignment.Center
            });
        }

        var action = BuildAction(step, isNext);
        Grid.SetColumn(action, 1);
        footer.Children.Add(action);

        var body = new Grid { RowDefinitions = new RowDefinitions("Auto,*,Auto") };
        body.Children.Add(header);
        body.Children.Add(description);
        body.Children.Add(footer);

        var card = new Border { Child = body };
        card.Classes.Add("onboarding_card");
        if (step.IsComplete)
        {
            card.Classes.Add("onboarding_card_complete");
        }
        else if (isNext)
        {
            card.Classes.Add("onboarding_card_next");
        }
        else if (step.IsOptional)
        {
            card.Classes.Add("onboarding_card_optional");
        }

        return card;
    }

    private static Border BuildChip(OnboardingStep step)
    {
        var content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5 };
        if (step.IsComplete)
        {
            content.Children.Add(new Avalonia.Controls.Shapes.Path
            {
                Classes = { "onboarding_chip_check" },
                Data = Geometry.Parse("M1,5 L4,8 L10,1")
            });
        }

        content.Children.Add(new TextBlock
        {
            Classes = { "onboarding_chip_text" },
            Text = step.IsComplete ? "Concluído" : step.IsOptional ? "Opcional" : "Pendente"
        });

        var chip = new Border { Child = content, VerticalAlignment = VerticalAlignment.Top };
        chip.Classes.Add("onboarding_chip");
        chip.Classes.Add(step.IsComplete
            ? "onboarding_chip_done"
            : step.IsOptional ? "onboarding_chip_optional" : "onboarding_chip_pending");
        return chip;
    }

    private Button BuildAction(OnboardingStep step, bool isNext)
    {
        var label = step.IsOptional ? "Adicionar" : step.IsComplete ? "Rever" : "Começar";
        var button = new Button
        {
            Tag = step,
            Content = new StackPanel
            {
                Classes = { "login_alert_action_content" },
                Children =
                {
                    new TextBlock { Classes = { "login_alert_action_text" }, Text = label }
                }
            }
        };
        button.Classes.Add("login_alert_action");
        button.Classes.Add(isNext ? "action_primary" : "action_secondary");
        button.Classes.Add("onboarding_card_action");
        button.Click += (_, _) =>
        {
            if (button.Tag is OnboardingStep requested)
            {
                StepRequested?.Invoke(this, requested);
            }
        };
        return button;
    }

    private static string IconPath(string key) => MenuIconRoot + key switch
    {
        "first-user" or "more-users" => "menu_utilizadores.svg",
        "company" => "menu_configuracoes_preferencias_empresa.svg",
        "fiscal-year" => "menu_informacoes_fiscais_anos_fiscais.svg",
        "article" => "menu_artigos_artigos.svg",
        "final-consumer" => "menu_clientes.svg",
        "table" => "menu_outras_tabelas_mesas.svg",
        "payment-method" => "menu_informacoes_fiscais_metodo_pagamento.svg",
        "printer" => "menu_dispositivos_impressoras.svg",
        _ => "menu_dashboard.svg"
    };
}
