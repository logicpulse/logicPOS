using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using LogicPOS.App.Branding;
using LogicPOS.Core;
using LogicPOS.Core.Authentication;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Core.Licensing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class BackOfficeWindow : UserControl, IOfficeSurface
{
    private const string ClockFormat = "dddd, dd' de 'MMMM' de 'yyyy' || 'HH:mm:ss tt";
    private const string IconRoot = "avares://LogicPOS.App/Assets/Images/BackOffice/";

    private readonly DispatcherTimer _clock = new() { Interval = TimeSpan.FromSeconds(1) };
    private readonly List<StackPanel> _sectionPanels = new();
    private readonly List<Button> _sectionButtons = new();
    private readonly HashSet<string> _dismissedSetupAlerts = new(StringComparer.OrdinalIgnoreCase);
    private IReadOnlyCollection<string>? _allowedMenu;
    private Button? _selectedItem;
    private bool _allowClose;
    private bool _backOfficeOnly;
    private bool _setupToastsShown;
    private bool _onboardingIncomplete;
    private StockManagementView? _stockView;
    private string? _pendingSaftAction;
    private bool _portrait;
    private bool _drawerOpen;
    private UiViewport _viewport;

    /// <summary>
    /// When true, finishing mandatory onboarding opens the FrontOffice instead of the Painel.
    /// Set by login when the session was forced into BackOffice for setup.
    /// </summary>
    public bool HandoffToFrontOfficeWhenReady { get; set; }

    public BackOfficeWindow()
        : this(string.Empty)
    {
    }

    public BackOfficeWindow(string sessionLabel)
    {
        InitializeComponent();
        DevEditionBanner.Apply(DevBanner);
        ArticleDashboardHost.OpenStockRequested += (_, _) => ShowPage("Gestão de Stocks");
        NewDocumentHost.PreviewRequested += async (path, title) => await ShowPdfAsync(path, title);
        ReportsHost.PreviewRequested += async (path, title) => await ShowPdfAsync(path, title);
        ReportsHost.Closed += (_, _) => ReportsOverlay.IsVisible = false;
        MarkFlag(UiCulture.Read(null));
        ApplySidebarTexts();
        SetSessionLabel(sessionLabel);
        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        var number = string.IsNullOrWhiteSpace(version)
            ? Assembly.GetExecutingAssembly().GetName().Version?.ToString(3)
            : version.Split('+')[0];
        var copyright = $"{DateTime.Now.Year} Copyright © Logicpulse";
        VersionLabel.Text = string.IsNullOrWhiteSpace(number) ? "Versão" : $"Versão {number}";
        CopyrightLabel.Text = copyright;
        AboutCopyright.Text = copyright;
        ApplyBranding(number ?? "1.6.0");
        BuildMenu();
        _clock.Tick += (_, _) =>
        {
            var text = DateTime.Now.ToString(ClockFormat);
            ClockLabel.Text = text;
        };
        EntityHost.FiscalYearCompleted += (_, _) => _ = RefreshHomeAfterSetupAsync();
        EntityHost.ArticleSetupCompleted += (_, _) => _ = RefreshHomeAfterSetupAsync();
        EntityHost.FinalConsumerSetupCompleted += (_, _) => _ = RefreshHomeAfterSetupAsync();
        EntityHost.TableSetupCompleted += (_, _) => _ = RefreshHomeAfterSetupAsync();
        EntityHost.EntitySaved += (_, _) => _ = RefreshHomeAfterSetupAsync();
        EntityHost.OverlayDismissed += (_, _) => RestoreOnboardingAfterOverlay();
        OnboardingHost.StepRequested += OnOnboardingStepRequested;
        AttachedToVisualTree += (_, _) =>
        {
            RelayoutForViewport();
            LayoutUpdated += OnLayoutUpdated;
        };
        DetachedFromVisualTree += (_, _) => LayoutUpdated -= OnLayoutUpdated;
    }

    public event EventHandler? FrontOfficeHandoffRequested;

    public async Task ActivateHostedAsync()
    {
        // Ceder o UI thread antes/depois do relayout para reduzir risco de ANR no Android.
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
        await Task.Delay(16);
        RelayoutForViewport();
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Loaded);
        RefreshSession();
    }

    public bool TryRequestClose()
    {
        if (_allowClose)
        {
            return true;
        }

        QuitOverlay.IsVisible = true;
        return false;
    }

    private Window? HostWindow => TopLevel.GetTopLevel(this) as Window;

    private void RequestHostClose()
    {
        if (HostWindow is Window window)
        {
            _allowClose = true;
            window.Close();
        }
    }

    private void OnLayoutUpdated(object? sender, EventArgs e) => RelayoutForViewport();

    private void RelayoutForViewport()
    {
        var next = UiViewport.From(this);
        if (next.Width == _viewport.Width && next.Height == _viewport.Height && Bounds.Width > 0)
        {
            return;
        }

        _viewport = next;
        // Portrait follows the real viewport so rotating a tablet restores the desktop shell.
        _portrait = next.IsPortrait;
        Classes.Set("bo_portrait", _portrait);
        Classes.Set("bo_landscape", _portrait == false);
        ApplyShellLayout();
    }

    private void ApplyShellLayout()
    {
        PortraitAppBar.IsVisible = _portrait;
        StatusBar.IsVisible = _portrait == false;
        WindowButtons.IsVisible = _portrait == false && OperatingSystem.IsAndroid() == false;
        SidebarToggle.IsVisible = _portrait == false;

        if (_portrait)
        {
            ShellGrid.ColumnDefinitions = new ColumnDefinitions("*");
            Grid.SetColumn(Sidebar, 0);
            Grid.SetColumnSpan(Sidebar, 1);
            Grid.SetColumn(PageArea, 0);
            Grid.SetColumnSpan(PageArea, 1);
            Grid.SetColumn(DrawerScrim, 0);
            Grid.SetColumnSpan(DrawerScrim, 1);
            SetDrawerOpen(_drawerOpen);
            PortraitTitle.Text = string.IsNullOrWhiteSpace(PageTitle.Text) ? AppText.Get("Painel") : PageTitle.Text;
            PortraitSession.Text = TerminalLabel.Text;
            PortraitChrome.FitToContent(QuitFrame, this, 480, minWidth: 360);
            PortraitChrome.FitToContent(SaftConfirmFrame, this, 520, minWidth: 360);
        }
        else
        {
            ShellGrid.ColumnDefinitions = new ColumnDefinitions("Auto,*");
            Grid.SetColumn(Sidebar, 0);
            Grid.SetColumnSpan(Sidebar, 1);
            Grid.SetColumn(PageArea, 1);
            Grid.SetColumnSpan(PageArea, 1);
            Grid.SetColumn(DrawerScrim, 0);
            Grid.SetColumnSpan(DrawerScrim, 2);
            DrawerScrim.IsVisible = false;
            Sidebar.IsVisible = true;
            Sidebar.Classes.Remove("bo_sidebar_drawer");
            // Repor alinhamento do sidebar quando volta a landscape.
            Sidebar.HorizontalAlignment = HorizontalAlignment.Stretch;
            Sidebar.VerticalAlignment = VerticalAlignment.Stretch;
            Sidebar.Width = double.NaN;
            _drawerOpen = false;
            PortraitChrome.FitToContent(QuitFrame, this, 480, minWidth: 360);
            PortraitChrome.FitToContent(SaftConfirmFrame, this, 520, minWidth: 360);
        }
    }

    private void SetDrawerOpen(bool open)
    {
        _drawerOpen = open;
        if (_portrait == false)
        {
            return;
        }

        Sidebar.Classes.Set("bo_sidebar_drawer", true);
        // Forçar alinhamento esquerdo e stretch vertical em código para garantir drawer lateral.
        Sidebar.HorizontalAlignment = HorizontalAlignment.Left;
        Sidebar.VerticalAlignment = VerticalAlignment.Stretch;
        Sidebar.Width = 280;
        Sidebar.IsVisible = open;
        DrawerScrim.IsVisible = open;
    }

    private void OnDrawerToggleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        => SetDrawerOpen(_drawerOpen == false);

    private void OnDrawerScrimPressed(object? sender, PointerPressedEventArgs e)
        => SetDrawerOpen(false);

    private void SyncPortraitChrome(string? title = null)
    {
        if (PortraitTitle is null)
        {
            return;
        }

        var text = title ?? PageTitle.Text;
        PortraitTitle.Text = string.IsNullOrWhiteSpace(text) ? AppText.Get("Painel") : text;
        PortraitSession.Text = TerminalLabel.Text;
        if (_portrait)
        {
            SetDrawerOpen(false);
        }
    }

    private void ApplyBranding(string productVersion)
    {
        try
        {
            SidebarLogo.Source = AppBranding.LoadSimpleLogo();
            AboutLogo.Source = AppBranding.LoadSimpleLogo();
            var reseller = AppComposition.Services?.GetService<ILicenseModule>()?.Reseller;
            if (string.IsNullOrWhiteSpace(reseller) == false &&
                reseller.Contains("LogicPulse", StringComparison.OrdinalIgnoreCase) == false)
            {
                CopyrightLabel.Text = AppBranding.FormatPoweredBy(productVersion);
            }
        }
        catch
        {
            // Keep XAML defaults.
        }
    }

    public void UseBackOfficeOnly()
    {
        _backOfficeOnly = true;
        ApplyFrontOfficeGate();
    }

    public event EventHandler? ReturnToPos;

    public event EventHandler? LoggedOut;

    public async Task ShowNewDocumentAsync(
        Guid? draftId = null,
        Guid? copyFromDocumentId = null,
        Guid? originFromDocumentId = null,
        string? originTargetType = null)
    {
        NewDocumentOverlay.IsVisible = true;
        var created = await NewDocumentHost.ShowAsync(draftId, copyFromDocumentId, originFromDocumentId, originTargetType);
        NewDocumentOverlay.IsVisible = false;
        if (created)
        {
            await DocumentsHost.ReloadAsync();
            if (string.IsNullOrEmpty(NewDocumentHost.CreatedPdfPath) == false)
            {
                await ShowPdfAsync(NewDocumentHost.CreatedPdfPath, NewDocumentHost.CreatedPdfTitle);
            }
        }
    }

    public async Task ShowPdfAsync(string path, string? title, Guid? documentId = null, string? preferredPrinter = null)
    {
        var reportsOpen = ReportsOverlay.IsVisible;
        var newDocOpen = NewDocumentOverlay.IsVisible;
        ReportsOverlay.IsVisible = false;
        // PDF por cima do Novo documento (senão o preview fica tapado).
        NewDocumentOverlay.IsVisible = false;
        PdfOverlay.IsVisible = true;
        await PdfHost.ShowAsync(path, title, documentId, preferredPrinter);
        PdfOverlay.IsVisible = false;
        NewDocumentOverlay.IsVisible = newDocOpen;
        ReportsOverlay.IsVisible = reportsOpen;
    }

    /// <summary>Shows a BackOffice entity page and opens the editor for <paramref name="id"/>.</summary>
    public async Task EditEntityAsync(string title, Guid id)
    {
        if (id == Guid.Empty)
        {
            return;
        }

        ShowPage(title);
        if (EntityHost.IsVisible == false)
        {
            return;
        }

        await EntityHost.ShowAsync(title);
        await EntityHost.OpenEditEntityAsync(id);
    }

    public void SetSessionLabel(string sessionLabel)
    {
        TerminalLabel.Text = sessionLabel;
        if (PortraitSession is not null)
        {
            PortraitSession.Text = sessionLabel;
        }
    }

    public void RefreshSession()
    {
        ClockLabel.Text = DateTime.Now.ToString(ClockFormat);
        _clock.Start();
        // Diferir carregamentos pesados para não bloquear o UI thread (evita ANR no Android).
        Dispatcher.UIThread.Post(() => _ = ApplyMenuAccessAsync(), DispatcherPriority.Background);
        Dispatcher.UIThread.Post(() => _ = RefreshHomeAfterSetupAsync(), DispatcherPriority.Background);
    }

    private async Task ReloadDashboardSafeAsync()
    {
        try
        {
            await DashboardHost.ReloadAsync();
        }
        catch (Exception exception)
        {
            App.WriteCrash("DashboardHost.ReloadAsync", exception);
        }
    }

    private async Task RefreshHomeAfterSetupAsync()
    {
        EntityHost.SetOverlayOnly(false);
        await RefreshOnboardingAsync();
        if (_onboardingIncomplete == false)
        {
            await RefreshSetupAlertsAsync();
        }
    }

    private async Task RefreshOnboardingAsync()
    {
        var service = AppComposition.Services?.GetService<IOnboardingService>();
        if (service is null)
        {
            // Cloud/Host: onboarding local não está registado — ainda assim carregar o Painel.
            _onboardingIncomplete = false;
            ApplyFrontOfficeGate();
            ShowDashboardHome();
            await ReloadDashboardSafeAsync();
            return;
        }

        OnboardingStatus status;
        try
        {
            status = await service.GetStatusAsync();
        }
        catch (Exception exception)
        {
            App.WriteCrash("RefreshOnboardingAsync", exception);
            return;
        }

        _onboardingIncomplete = status.IsComplete == false;
        ApplyFrontOfficeGate();

        if (_onboardingIncomplete)
        {
            SetupAlerts.Children.Clear();
            SetupAlerts.IsVisible = false;
            await ShowOnboardingHomeAsync();
            return;
        }

        var wasOnboarding = OnboardingHost.IsVisible;
        OnboardingHost.IsVisible = false;

        if (HandoffToFrontOfficeWhenReady && _backOfficeOnly == false)
        {
            HandoffToFrontOfficeWhenReady = false;
            FrontOfficeHandoffRequested?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (wasOnboarding || DashboardHost.IsVisible || PageHost.IsVisible == false)
        {
            ShowDashboardHome();
            await ReloadDashboardSafeAsync();
        }
    }

    private async Task ShowOnboardingHomeAsync()
    {
        SelectMenuItem(DashboardButton);
        PageTitle.Text = string.Empty;
        SyncPortraitChrome(AppText.Get("Configuração inicial"));
        DashboardHost.IsVisible = false;
        ArticleDashboardHost.IsVisible = false;
        PageHost.IsVisible = false;
        DocumentsHost.IsVisible = false;
        EntityHost.IsVisible = false;
        StockHost.IsVisible = false;
        OnboardingHost.IsVisible = true;
        await OnboardingHost.ReloadAsync();
    }

    private void ShowDashboardHome()
    {
        SelectMenuItem(DashboardButton);
        PageTitle.Text = string.Empty;
        SyncPortraitChrome(AppText.Get("Painel"));
        OnboardingHost.IsVisible = false;
        DashboardHost.IsVisible = true;
        ArticleDashboardHost.IsVisible = false;
        PageHost.IsVisible = false;
        DocumentsHost.IsVisible = false;
        EntityHost.IsVisible = false;
        StockHost.IsVisible = false;
    }

    private void ApplyFrontOfficeGate()
    {
        if (_backOfficeOnly)
        {
            FrontOfficeButton.IsVisible = false;
            FrontOfficeButton.IsEnabled = false;
            return;
        }

        FrontOfficeButton.IsVisible = true;
        FrontOfficeButton.IsEnabled = _onboardingIncomplete == false;
    }

    private async void OnOnboardingStepRequested(object? sender, OnboardingStep step)
    {
        if (step.OpensCompanySetup)
        {
            await OpenCompanySetupAsync();
            return;
        }

        if (step.Key == "fiscal-year"
            || string.Equals(step.ActionPage, "Abertura de ano fiscal", StringComparison.OrdinalIgnoreCase))
        {
            await OpenFiscalYearWizardFromOnboardingAsync();
            return;
        }

        if (step.Key == "article"
            || string.Equals(step.ActionPage, "Artigos", StringComparison.OrdinalIgnoreCase))
        {
            await OpenArticleSetupWizardFromOnboardingAsync();
            return;
        }

        if (step.Key == "final-consumer"
            || string.Equals(step.ActionPage, "Clientes", StringComparison.OrdinalIgnoreCase))
        {
            await OpenFinalConsumerSetupWizardFromOnboardingAsync();
            return;
        }

        if (step.Key == "table"
            || step.ActionPage is "Mesas" or "Locais")
        {
            await OpenTableSetupWizardFromOnboardingAsync();
            return;
        }

        if (IsOnboardingCreateStep(step))
        {
            await OpenEntityCreateFromOnboardingAsync(step.ActionPage!);
            return;
        }

        if (string.IsNullOrWhiteSpace(step.ActionPage) == false)
        {
            ShowPage(step.ActionPage);
        }
    }

    private static bool IsOnboardingCreateStep(OnboardingStep step)
        => step.Key is "payment-method"
            || step.ActionPage is "Métodos de pagamento";

    private async Task OpenFiscalYearWizardFromOnboardingAsync()
    {
        await PrepareOnboardingOverlayAsync("Abertura de ano fiscal");
        await EntityHost.OpenFiscalYearWizardAsync();
    }

    private async Task OpenArticleSetupWizardFromOnboardingAsync()
    {
        await PrepareOnboardingOverlayAsync("Artigos");
        await EntityHost.OpenArticleSetupWizardAsync();
    }

    private async Task OpenFinalConsumerSetupWizardFromOnboardingAsync()
    {
        await PrepareOnboardingOverlayAsync("Clientes");
        await EntityHost.OpenFinalConsumerSetupWizardAsync();
    }

    private async Task OpenTableSetupWizardFromOnboardingAsync()
    {
        await PrepareOnboardingOverlayAsync("Mesas");
        await EntityHost.OpenTableSetupWizardAsync();
    }

    private async Task OpenEntityCreateFromOnboardingAsync(string title)
    {
        await PrepareOnboardingOverlayAsync(title);
        await EntityHost.OpenNewEntityAsync();
    }

    /// <summary>
    /// Keeps the onboarding panel visible and only mounts EntityListing overlays on top.
    /// </summary>
    private async Task PrepareOnboardingOverlayAsync(string title)
    {
        _articleDashboardRequest++;
        ReportsOverlay.IsVisible = false;
        DashboardHost.IsVisible = false;
        ArticleDashboardHost.IsVisible = false;
        DocumentsHost.IsVisible = false;
        ReceiptHost.IsVisible = false;
        StockHost.IsVisible = false;
        SettingsHost.IsVisible = false;
        PlaceholderHost.IsVisible = false;

        OnboardingHost.IsVisible = true;
        PageHost.IsVisible = true;
        EntityHost.IsVisible = true;
        EntityHost.SetOverlayOnly(true);

        await EntityHost.ShowAsync(title);
    }

    private void RestoreOnboardingAfterOverlay()
    {
        EntityHost.SetOverlayOnly(false);
        EntityHost.IsVisible = false;
        PageHost.IsVisible = false;
        OnboardingHost.IsVisible = true;
        PageTitle.Text = string.Empty;
        SyncPortraitChrome(AppText.Get("Configuração inicial"));
        _ = OnboardingHost.ReloadAsync();
    }

    private async Task OpenCompanySetupAsync()
    {
        var setup = AppComposition.Services?.GetService<ICompanySetupService>();
        var owner = HostWindow;
        if (setup is null || owner is null)
        {
            return;
        }

        await new CompanySetupWindow(setup).ShowDialog(owner);
        await RefreshHomeAfterSetupAsync();
    }

    public async Task RefreshSetupAlertsAsync()
    {
        var service = AppComposition.Services?.GetService<IStartupReadinessService>();
        if (service is null)
        {
            SetupAlerts.Children.Clear();
            SetupAlerts.IsVisible = false;
            return;
        }

        IReadOnlyList<StartupReadinessIssue> issues;
        try
        {
            issues = await service.GetIssuesAsync();
        }
        catch (Exception exception)
        {
            App.WriteCrash("RefreshSetupAlertsAsync", exception);
            return;
        }

        SetupAlerts.Children.Clear();
        var visible = new List<StartupReadinessIssue>();
        foreach (var issue in issues)
        {
            if (_dismissedSetupAlerts.Contains(issue.Key))
            {
                continue;
            }

            visible.Add(issue);
            SetupAlerts.Children.Add(BuildSetupAlert(issue));
        }

        SetupAlerts.IsVisible = SetupAlerts.Children.Count > 0;

        // Same idea as web ShowAlertBox: toast warnings at the top on first open.
        if (_setupToastsShown == false && visible.Count > 0)
        {
            _setupToastsShown = true;
            foreach (var issue in visible)
            {
                Toast.Warning(this, issue.Message);
            }
        }
    }

    private Border BuildSetupAlert(StartupReadinessIssue issue)
    {
        var row = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto,Auto") };
        row.Children.Add(new TextBlock
        {
            Classes = { "bo_setup_alert_text" },
            Text = issue.Message
        });

        if (string.IsNullOrWhiteSpace(issue.ActionPage) == false)
        {
            var action = new Button
            {
                Classes = { "bo_setup_alert_action" },
                Content = string.IsNullOrWhiteSpace(issue.ActionLabel) ? "Abrir" : issue.ActionLabel,
                Tag = issue.ActionPage
            };
            Grid.SetColumn(action, 1);
            action.Click += (_, _) =>
            {
                if (action.Tag is string page)
                {
                    ShowPage(page);
                }
            };
            row.Children.Add(action);
        }

        var dismiss = new Button
        {
            Classes = { "bo_setup_alert_dismiss" },
            Content = "×",
            Tag = issue.Key
        };
        Grid.SetColumn(dismiss, 2);
        ToolTip.SetTip(dismiss, "Dispensar");
        dismiss.Click += (_, _) =>
        {
            if (dismiss.Tag is string key)
            {
                _dismissedSetupAlerts.Add(key);
            }

            _ = RefreshSetupAlertsAsync();
        };
        row.Children.Add(dismiss);

        return new Border
        {
            Classes = { "bo_setup_alert" },
            Child = row
        };
    }

    private void BuildMenu()
    {
        var sections = new (string Title, string Icon, (string Title, string? Icon, bool Logout)[] Items)[]
        {
            ("Documentos", "Menu/menu_dashboard_documentos_b.svg", new[]
            {
                ("Novo Doc.", "Menu/menu_dashboard_novo_documento.svg", false),
                ("Documentos", "Menu/menu_dashboard_documentos_b.svg", false),
                ("Emissão Recibos", "Menu/menu_dashboard_emissao_recibos.svg", false),
                ("Recibos", "Menu/menu_dashboard_recibos.svg", false),
                ("Conta.Corr.", "Menu/menu_dashboard_conta_corrente.svg", false),
                ("Sessões de Trab.", (string?)null, false)
            }),
            ("Relatórios", "Menu/menu_relatorios_relatorios.svg", new[]
            {
                ("Relatórios", "Menu/menu_relatorios_relatorios.svg", false)
            }),
            ("Artigos", "Menu/menu_artigos_artigos.svg", new[]
            {
                ("Famílias", "Menu/menu_artigos_familas.svg", false),
                ("Subfamílias", "Menu/menu_artigos_subfamilias.svg", false),
                ("Artigos", "Menu/menu_artigos_artigos.svg", false),
                ("Tipo de artigos", "Menu/menu_artigos_tipos_artigo.svg", false),
                ("Classe do artigo", (string?)null, false),
                ("Tipo de Preço", "Menu/menu_artigos_tipos_preco.svg", false),
                ("Gestão de Stocks", "Menu/menu_artigos_gestao_stocks.svg", false),
                ("Importar Artigos", "Menu/menu_importacao_importar_artigos.svg", false),
                ("Exportar Artigos", "Menu/menu_exportacao_exportar_artigos.svg", false)
            }),
            ("Informação fiscal", "Menu/menu_informacoes_fiscais.svg", new[]
            {
                ("Abertura de ano fiscal", "Menu/menu_informacoes_fiscais_anos_fiscais.svg", false),
                ("Séries", "Menu/menu_informacoes_fiscais_series_documentos.svg", false),
                ("Tipo de documento", "Menu/menu_informacoes_fiscais_tipos_documentos.svg", false),
                ("Taxas de imposto", "Menu/menu_informacoes_fiscais_taxas_iva.svg", false),
                ("Motivo de isenção de IVA", "Menu/menu_informacoes_fiscais_movimentos_isencao_iva.svg", false),
                ("Cond. de Pagamento", "Menu/menu_informacoes_fiscais_condicoes_pagamento.svg", false),
                ("Métodos de pagamento", "Menu/menu_informacoes_fiscais_metodo_pagamento.svg", false)
            }),
            ("Clientes", "Menu/menu_clientes.svg", new[]
            {
                ("Clientes", "Menu/menu_clientes.svg", false),
                ("Tipo de clientes", "Menu/menu_clientes_tipo_clientes.svg", false),
                ("Grupo de descontos", "Menu/menu_clientes_grupo_desconto.svg", false),
                ("Importar Clientes", "Menu/menu_importacao_importar_clientes.svg", false),
                ("Exportar Clientes", "Menu/menu_exportacao_exportar_clientes.svg", false)
            }),
            ("Utilizadores", "Menu/menu_utilizadores.svg", new[]
            {
                ("Utilizadores", "Menu/menu_utilizadores.svg", false),
                ("Permissões", "Menu/menu_utilizadores_permicoes.svg", false),
                ("Grupo de comissões", "Menu/menu_utilizadores_grupo_comicao.svg", false)
            }),
            ("Dispositivos", "Menu/menu_dispositivos.svg", new[]
            {
                ("Tipos de impressora", "Menu/menu_dispositivos_tipos_impressoras.svg", false),
                ("Impressoras", "Menu/menu_dispositivos_impressoras.svg", false),
                ("Dispositivos de Entrada", "Menu/menu_dispositivos_leitores.svg", false),
                ("Display de Cliente", "Menu/menu_dispositivos_display.svg", false),
                ("Balanças", "Menu/menu_dispositivos_balancas.svg", false)
            }),
            ("Outras Tabelas", "Menu/menu_outras_tabelas.svg", new[]
            {
                ("País", "Menu/menu_outras_tabelas_menu_dispositivos_paises.svg", false),
                ("Moeda", "Menu/menu_outras_tabelas_moedas.svg", false),
                ("Locais", "Menu/menu_outras_tabelas_locais.svg", false),
                ("Mesas", "Menu/menu_outras_tabelas_mesas.svg", false),
                ("Tipo de Movimento", "Menu/menu_outras_tabelas_tipo_movimento.svg", false),
                ("Unidades de medida", "Menu/menu_outras_tabelas_unidade_medida.svg", false),
                ("Unidades de tamanho", "Menu/menu_outras_tabelas_unidade_tamanho.svg", false),
                ("Feriados", "Menu/menu_outras_tabelas_feriados.svg", false),
                ("Armazém", "Menu/menu_outras_tabelas_armazens.svg", false)
            }),
            ("Configuração", "Menu/menu_configuracoes.svg", new[]
            {
                ("Parâmetros da Empresa", "Menu/menu_configuracoes_preferencias_empresa.svg", false),
                ("Parâmetros de Sistema", "Menu/menu_configuracoes_preferencias_sistema.svg", false),
                ("Terminais", "Menu/menu_configuracoes_terminais.svg", false)
            }),
            ("SAFT", "Menu/menu_exportacao.svg", new[]
            {
                ("SAF-T ano", "Menu/menu_exportacao_saft_anual.svg", false),
                ("SAF-T último mês", "Menu/menu_exportacao_saft_ultimo_mes.svg", false),
                ("SAF-T período", "Menu/menu_exportacao_saft_personalizado.svg", false)
            }),
            ("Sistema", "Menu/menu_sistema.svg", new[]
            {
                ("Notificações", "Menu/menu_sistema_notificacoes.svg", false),
                ("Registro de alterações (Changelog)", "Menu/menu_sistema_change_logs.svg", false),
                ("Backup DB", "Menu/menu_sistema_backup.svg", false),
                ("Restaurar DB", "Menu/menu_sistema_restauro.svg", false),
                ("Restaurar de ficheiro", "Menu/menu_sistema_restauro.svg", false),
                ("Atualizar", (string?)null, false),
                ("Sair da Sessão", "Menu/menu_sistema_logout.svg", true)
            })
        };

        var menu = sections.ToList();
        if (IsAngola())
        {
            var exportIndex = menu.FindIndex(section => section.Title == "SAFT");
            // Keep the separate AGT Séries/Documentos section; only rename the fiscal export accordion.
            if (exportIndex >= 0)
            {
                menu[exportIndex] = ("AGT", "Menu/menu_exportacao.svg", new[]
                {
                    ("AGT ano", "Menu/menu_exportacao_saft_anual.svg", false),
                    ("AGT último mês", "Menu/menu_exportacao_saft_ultimo_mes.svg", false),
                    ("AGT período", "Menu/menu_exportacao_saft_personalizado.svg", false)
                });
            }

            menu.Insert(exportIndex < 0 ? menu.Count : exportIndex, ("AGT", "Menu/menu_informacoes_fiscais.svg", new[]
            {
                ("Séries AGT", "Menu/menu_informacoes_fiscais_series_documentos.svg", false),
                ("Documentos AGT", "Menu/menu_informacoes_fiscais_tipos_documentos.svg", false)
            }));
        }

        foreach (var section in menu)
        {
            BackOfficePageIcons.Register(section.Title, section.Icon);
            var direct = section.Items.Length == 1
                && section.Items[0].Logout == false
                && section.Items[0].Title == section.Title;
            var children = new StackPanel { IsVisible = false, Classes = { "bo_menu_children" } };
            if (direct == false)
            {
                foreach (var item in section.Items)
                {
                    BackOfficePageIcons.Register(item.Title, item.Icon, section.Icon);
                    var logout = item.Logout;
                    var title = item.Title;
                    var label = AppText.Get(title);
                    var child = new Button
                    {
                        Classes = { "bo_menu_child" },
                        Content = string.IsNullOrEmpty(item.Icon)
                            ? label
                            : CreateRow($"{IconRoot}{item.Icon}", label),
                        Tag = title
                    };
                    child.Click += (_, _) =>
                    {
                        SelectMenuItem(child);
                        if (logout)
                        {
                            RaiseLogout();
                            return;
                        }

                        if (title == "Atualizar")
                        {
                            ShowUpdate();
                            return;
                        }

                        // SAF-T menu items stay on the current listing and open dialogs.
                        if (SaftExportUi.IsMenuTitle(title))
                        {
                            OpenSaftFromMenu(title);
                            return;
                        }

                        ShowPage(title);
                    };
                    children.Children.Add(child);
                }
            }

            var parent = new Button
            {
                Classes = { "bo_menu_parent" },
                Content = CreateRow($"{IconRoot}{section.Icon}", AppText.Get(section.Title)),
                Tag = direct ? section.Title : null
            };
            ToolTip.SetTip(parent, AppText.Get(section.Title));
            parent.Click += (_, _) =>
            {
                if (Sidebar.Classes.Contains("bo_sidebar_collapsed"))
                {
                    SetSidebarCollapsed(false);
                }

                if (direct)
                {
                    if (IsMenuAllowed(section.Title) == false)
                    {
                        return;
                    }

                    OpenReports();
                    return;
                }

                var opening = children.IsVisible == false;
                ToggleSection(children, parent);
                if (opening && section.Title == "Artigos")
                {
                    _ = ShowArticleDashboardAsync();
                }

                if (opening && section.Title == "Documentos")
                {
                    var documentsChild = children.Children.OfType<Button>()
                        .FirstOrDefault(button => button.Tag as string == "Documentos");
                    if (documentsChild is not null)
                    {
                        SelectMenuItem(documentsChild);
                    }

                    ShowPage("Documentos");
                }
            };
            MenuHost.Children.Add(parent);
            if (direct == false)
            {
                MenuHost.Children.Add(children);
            }

            _sectionButtons.Add(parent);
            _sectionPanels.Add(children);
        }

        _ = ApplyMenuAccessAsync();
    }

    public async Task ApplyMenuAccessAsync()
    {
        var service = AppComposition.Services?.GetService<IBackOfficeListingService>();
        if (service is null)
        {
            return;
        }

        IReadOnlyCollection<string>? allowed;
        try
        {
            allowed = await service.AllowedMenuTitlesAsync();
        }
        catch (Exception)
        {
            return;
        }

        // null = unrestricted owner; empty = no gated menus; otherwise only listed titles.
        _allowedMenu = allowed;
        for (var index = 0; index < _sectionPanels.Count; index++)
        {
            var panel = _sectionPanels[index];
            var parent = index < _sectionButtons.Count ? _sectionButtons[index] : null;
            var anyChild = false;
            var childCount = 0;
            foreach (var child in panel.Children.OfType<Button>())
            {
                if (child.Tag is not string title)
                {
                    continue;
                }

                childCount++;
                var enabled = IsMenuAllowed(title);
                child.IsEnabled = enabled;
                anyChild |= enabled;
            }

            if (parent is null)
            {
                continue;
            }

            if (parent.Tag is string directTitle)
            {
                parent.IsEnabled = IsMenuAllowed(directTitle);
            }
            else if (childCount > 0)
            {
                // Keep the section visible but muted when every child is denied.
                parent.IsEnabled = anyChild;
            }
        }
    }

    private bool IsMenuAllowed(string title)
    {
        if (ProfileMenuAccess.IsRestricted(title) == false)
        {
            return true;
        }

        // null => unrestricted; empty => deny all gated titles.
        return _allowedMenu is null || _allowedMenu.Contains(title);
    }

    private static Control CreateRow(string iconUri, string text)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Classes = { "bo_menu_row" }
        };
        if (string.IsNullOrWhiteSpace(iconUri) == false)
        {
            var glyph = new Border { Classes = { "bo_menu_glyph" } };
            if (iconUri.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            {
                glyph.Classes.Add("bo_menu_glyph_svg");
                glyph.Child = new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
                {
                    Classes = { "bo_menu_glyph_icon" },
                    Path = iconUri
                };
            }
            else
            {
                glyph.OpacityMask = new ImageBrush(new Bitmap(AssetLoader.Open(new Uri(iconUri))))
                {
                    Stretch = Stretch.Uniform
                };
            }

            panel.Children.Add(glyph);
        }

        panel.Children.Add(new TextBlock
        {
            Classes = { "bo_menu_label" },
            Text = text,
            VerticalAlignment = VerticalAlignment.Center
        });
        return panel;
    }

    private void SelectMenuItem(Button button)
    {
        DashboardButton.Classes.Remove("bo_menu_on");
        _selectedItem?.Classes.Remove("bo_menu_on");
        button.Classes.Add("bo_menu_on");
        _selectedItem = button;
    }

    private void CloseSections()
    {
        for (var index = 0; index < _sectionPanels.Count; index++)
        {
            _sectionPanels[index].IsVisible = false;
            _sectionButtons[index].Classes.Remove("open");
        }
    }

    private void ToggleSection(StackPanel children, Button parent)
    {
        var open = children.IsVisible == false;
        CloseSections();

        if (open)
        {
            children.IsVisible = true;
            parent.Classes.Add("open");
        }
    }

    public void OpenPage(string title)
    {
        if (SaftExportUi.IsMenuTitle(title))
        {
            OpenSaftFromMenu(title);
            return;
        }

        ShowPage(title);
    }

    private void OpenReports()
    {
        ReportsOverlay.IsVisible = true;
        _ = ReportsHost.ShowAsync();
    }

    private void OpenSaftFromMenu(string title)
    {
        var action = SaftExportUi.ActionForMenu(title);
        if (action == "saft-period")
        {
            ShowSaftPeriodDialog();
            return;
        }

        _pendingSaftAction = action;
        SaftConfirmTitle.Text = SaftExportUi.ExportDialogTitle();
        SaftConfirmMessage.Text = action == "saft-month"
            ? SaftExportUi.ConfirmMonthMessage()
            : SaftExportUi.ConfirmYearMessage();
        SaftConfirmOverlay.IsVisible = true;
    }

    private void ShowSaftPeriodDialog()
    {
        _pendingSaftAction = "saft-period";
        SaftPeriodTitle.Text = SaftExportUi.CustomPeriodTitle();
        var yearStart = new DateTime(DateTime.Today.Year, 1, 1);
        SaftPeriod.SetRange(yearStart, DateTime.Today.AddDays(1).AddSeconds(-1));
        SaftPeriodOverlay.IsVisible = true;
    }

    private async void OnSaftConfirmYesClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SaftConfirmOverlay.IsVisible = false;
        var action = _pendingSaftAction;
        _pendingSaftAction = null;
        if (string.IsNullOrWhiteSpace(action))
        {
            return;
        }

        await RunSaftExportAsync(action, null);
    }

    private void OnSaftConfirmNoClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _pendingSaftAction = null;
        SaftConfirmOverlay.IsVisible = false;
    }

    private async void OnSaftPeriodOkClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SaftPeriodOverlay.IsVisible = false;
        var start = SaftPeriod.StartDate;
        var end = SaftPeriod.EndDate;
        if (end < start)
        {
            (start, end) = (end, start);
        }

        _pendingSaftAction = null;
        await RunSaftExportAsync("saft-period", $"{start:yyyy-MM-dd}|{end:yyyy-MM-dd}");
    }

    private void OnSaftPeriodCancelClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _pendingSaftAction = null;
        SaftPeriodOverlay.IsVisible = false;
    }

    private void OnSaftMessageCloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        => SaftMessageOverlay.IsVisible = false;

    private async Task RunSaftExportAsync(string action, string? extra)
    {
        var service = AppComposition.Services?.GetService<IBackOfficeListingService>();
        if (service is null)
        {
            ShowSaftMessage(SaftExportUi.ExportErrorText());
            return;
        }

        SaftBusy.Show("A exportar...");
        await Task.Yield();

        ListingSaveResult result;
        try
        {
            result = await SaftExportUi.RunExportAsync(service, action, extra);
        }
        finally
        {
            SaftBusy.Hide();
        }

        var message = await SaftExportUi.FormatResultAsync(result, TopLevel.GetTopLevel(this));
        ShowSaftMessage(message);
    }

    private void ShowSaftMessage(string message)
    {
        SaftMessageTitle.Text = SaftExportUi.ExportDialogTitle();
        SaftMessageText.Text = message;
        SaftMessageOverlay.IsVisible = true;
    }

    private static bool IsAngola()
    {
        var country = AppComposition.Services?.GetService<IConfiguration>()?["Country"];
        return string.Equals(country, "AO", StringComparison.OrdinalIgnoreCase)
            || string.Equals(country, "Angola", StringComparison.OrdinalIgnoreCase);
    }

    private void ShowPage(string title)
    {
        var service = AppComposition.Services?.GetService<IBackOfficeListingService>();
        title = service?.ResolveTitle(title) ?? title;
        if (IsMenuAllowed(title) == false)
        {
            return;
        }

        if (SaftExportUi.IsMenuTitle(title))
        {
            OpenSaftFromMenu(title);
            return;
        }

        if (title is "Relatórios")
        {
            OpenReports();
            return;
        }

        PageTitle.Text = title;
        SyncPortraitChrome(title);
        _articleDashboardRequest++;
        ReportsOverlay.IsVisible = false;

        OnboardingHost.IsVisible = false;
        DashboardHost.IsVisible = false;
        ArticleDashboardHost.IsVisible = false;
        PageHost.IsVisible = true;
        DocumentsHost.IsVisible = false;
        ReceiptHost.IsVisible = false;
        StockHost.IsVisible = false;
        EntityHost.IsVisible = false;
        SettingsHost.IsVisible = false;
        PlaceholderHost.IsVisible = false;

        if (title is "Novo Doc.")
        {
            PageTitle.Text = "Documentos";
            DocumentsHost.IsVisible = true;
            _ = DocumentsHost.ReloadAsync();
            _ = ShowNewDocumentAsync();
            return;
        }

        if (title is "Documentos")
        {
            DocumentsHost.IsVisible = true;
            _ = DocumentsHost.ReloadAsync();
            return;
        }

        if (title is "Emissão Recibos")
        {
            ReceiptHost.IsVisible = true;
            _ = ReceiptHost.ReloadAsync();
            return;
        }

        if (title is "Gestão de Stocks")
        {
            EnsureStockView();
            StockHost.IsVisible = true;
            if (_stockView is not null)
            {
                _ = _stockView.ReloadAsync();
            }

            return;
        }

        if (title is "Parâmetros da Empresa" or "Parâmetros de Sistema")
        {
            SettingsHost.IsVisible = true;
            EntityHost.IsVisible = false;
            _ = SettingsHost.ShowAsync(title);
            return;
        }

        if (service is not null && service.HasPage(title))
        {
            EntityHost.SetOverlayOnly(false);
            EntityHost.IsVisible = true;
            _ = EntityHost.ShowAsync(title);
            return;
        }

        PlaceholderHost.IsVisible = true;
        PageMessage.Text = $"{title}{Environment.NewLine}{Environment.NewLine}Esta área ainda está em desenvolvimento.";
    }

    private void EnsureStockView()
    {
        if (_stockView is not null)
        {
            return;
        }

        try
        {
            _stockView = new StockManagementView();
            StockHost.Children.Clear();
            StockHost.Children.Add(_stockView);
        }
        catch (Exception exception)
        {
            App.WriteCrash("EnsureStockView", exception);
            StockHost.Children.Clear();
            StockHost.Children.Add(new TextBlock
            {
                Text = $"Não foi possível abrir Gestão de Stocks.{Environment.NewLine}{exception.Message}",
                TextWrapping = TextWrapping.Wrap
            });
        }
    }

    private void OnFlagClick(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border flag || flag.Tag is not string culture)
        {
            return;
        }

        UiCulture.Apply(culture);
        MarkFlag(culture);
        MenuHost.Children.Clear();
        _sectionPanels.Clear();
        _sectionButtons.Clear();
        BuildMenu();
        ApplySidebarTexts();
    }

    private void MarkFlag(string culture)
    {
        foreach (var child in FlagBar.Children.OfType<Border>())
        {
            child.Classes.Remove("bo_sidebar_flag_on");
            if (string.Equals(child.Tag as string, culture, StringComparison.OrdinalIgnoreCase))
            {
                child.Classes.Add("bo_sidebar_flag_on");
            }
        }
    }

    /// <summary>
    /// Atualiza textos fixos do sidebar (Dashboard, FrontOffice, Sair, Acerca) conforme a cultura ativa.
    /// </summary>
    private void ApplySidebarTexts()
    {
        var dashLabel = AppText.Get("Painel");
        SetMenuButtonText(DashboardButton, dashLabel);
        ToolTip.SetTip(DashboardButton, dashLabel);

        var foLabel = AppText.Get("FrontOffice");
        SetMenuButtonText(FrontOfficeButton, foLabel);
        ToolTip.SetTip(FrontOfficeButton, foLabel);

        var exitLabel = AppText.Get("Sair da Aplicação");
        SetMenuButtonText(ExitButton, exitLabel);
        ToolTip.SetTip(ExitButton, exitLabel);

        var aboutLabel = AppText.Get("Acerca");
        SetMenuButtonText(AboutButton, aboutLabel);
        ToolTip.SetTip(AboutButton, aboutLabel);

        if (_portrait && string.IsNullOrWhiteSpace(PageTitle.Text))
        {
            PortraitTitle.Text = dashLabel;
        }
    }

    private static void SetMenuButtonText(Button button, string text)
    {
        // O conteúdo é um StackPanel com bo_menu_row contendo um TextBlock com bo_menu_label.
        if (button.Content is StackPanel row)
        {
            var label = row.Children.OfType<TextBlock>()
                .FirstOrDefault(t => t.Classes.Contains("bo_menu_label"));
            if (label is not null)
            {
                label.Text = text;
            }
        }
        else if (button.GetVisualDescendants().OfType<TextBlock>()
                     .FirstOrDefault(t => t.Classes.Contains("bo_menu_label")) is { } nested)
        {
            nested.Text = text;
        }
    }

    private void OnSidebarToggleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SetSidebarCollapsed(Sidebar.Classes.Contains("bo_sidebar_collapsed") == false);
    }

    private void SetSidebarCollapsed(bool collapsed)
    {
        if (collapsed)
        {
            CloseSections();
            Sidebar.Classes.Add("bo_sidebar_collapsed");
            Grid.SetColumn(SidebarToggle, 0);
            Grid.SetColumnSpan(SidebarToggle, 2);
            ToolTip.SetTip(SidebarToggle, "Expandir menu");
            return;
        }

        Sidebar.Classes.Remove("bo_sidebar_collapsed");
        Grid.SetColumn(SidebarToggle, 1);
        Grid.SetColumnSpan(SidebarToggle, 1);
        ToolTip.SetTip(SidebarToggle, "Recolher menu");
    }

    private int _articleDashboardRequest;

    private async Task ShowArticleDashboardAsync()
    {
        var request = ++_articleDashboardRequest;
        if (await ArticleDashboardHost.EnsureModuleAsync() == false || request != _articleDashboardRequest)
        {
            return;
        }

        DashboardButton.Classes.Remove("bo_menu_on");
        _selectedItem?.Classes.Remove("bo_menu_on");
        _selectedItem = null;
        PageTitle.Text = "Artigos";
        SyncPortraitChrome("Artigos");
        ReportsOverlay.IsVisible = false;
        OnboardingHost.IsVisible = false;
        DashboardHost.IsVisible = false;
        PageHost.IsVisible = false;
        ArticleDashboardHost.IsVisible = true;
        _ = ArticleDashboardHost.ReloadAsync();
    }

    private void OnDashboardClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _articleDashboardRequest++;
        _ = RefreshHomeAfterSetupAsync();
    }

    private void OnPosClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_backOfficeOnly || _onboardingIncomplete)
        {
            return;
        }

        SetDrawerOpen(false);
        _clock.Stop();
        ReturnToPos?.Invoke(this, EventArgs.Empty);
    }

    private void OnMinimizeClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (HostWindow is Window window)
        {
            window.WindowState = WindowState.Minimized;
        }
    }

    private void OnMaximizeClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (HostWindow is not Window window)
        {
            return;
        }

        window.WindowDecorations = WindowDecorations.None;
        window.CanResize = false;
        window.WindowState = WindowState.Maximized;
    }

    private void OnRestoreClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (HostWindow is not Window window)
        {
            return;
        }

        window.WindowDecorations = WindowDecorations.BorderOnly;
        window.CanResize = true;
        window.WindowState = WindowState.Normal;
        window.Width = 1400;
        window.Height = 860;
        CenterHostOnScreen(window);
    }

    private void OnWindowCloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_backOfficeOnly || _onboardingIncomplete)
        {
            QuitOverlay.IsVisible = true;
            return;
        }

        _clock.Stop();
        ReturnToPos?.Invoke(this, EventArgs.Empty);
    }

    private void OnStatusPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (HostWindow is not Window window
            || window.WindowState != WindowState.Normal
            || e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == false)
        {
            return;
        }

        if (e.Source is Visual source && source.FindAncestorOfType<Button>() is not null)
        {
            return;
        }

        window.BeginMoveDrag(e);
    }

    private void CenterHostOnScreen(Window window)
    {
        var screen = window.Screens.ScreenFromVisual(window);
        if (screen is null)
        {
            return;
        }

        var area = screen.WorkingArea;
        var scale = screen.Scaling;
        var width = (int)(window.Width * scale);
        var height = (int)(window.Height * scale);
        window.Position = new PixelPoint(
            area.X + Math.Max(0, (area.Width - width) / 2),
            area.Y + Math.Max(0, (area.Height - height) / 2));
    }

    private void ShowUpdate()
    {
        UpdateStatus.Text = "A configuração (appsettings) e a base de dados local não são substituídas.";
        UpdateProgress.IsVisible = false;
        UpdateProgress.Value = 0;
        UpdateConfirm.IsEnabled = true;
        UpdateDismiss.IsEnabled = true;
        UpdateOverlay.IsVisible = true;
    }

    private void OnUpdateDismissClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (UpdateConfirm.IsEnabled == false)
        {
            return;
        }

        UpdateOverlay.IsVisible = false;
    }

    private async void OnUpdateConfirmClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        UpdateConfirm.IsEnabled = false;
        UpdateDismiss.IsEnabled = false;
        UpdateProgress.IsVisible = true;
        UpdateProgress.IsIndeterminate = true;
        UpdateProgress.Value = 0;
        var progress = new Progress<string>(text =>
        {
            UpdateStatus.Text = text;
            var percent = ParseUpdatePercent(text);
            if (percent is int value)
            {
                UpdateProgress.IsIndeterminate = false;
                UpdateProgress.Value = value;
            }
            else if (text.Contains("extrair", StringComparison.OrdinalIgnoreCase)
                     || text.Contains("preparar", StringComparison.OrdinalIgnoreCase)
                     || text.Contains("fechar", StringComparison.OrdinalIgnoreCase))
            {
                UpdateProgress.IsIndeterminate = true;
            }
        });
        try
        {
            await ApplicationUpdater.DownloadAndScheduleAsync(progress);
        }
        catch (Exception exception)
        {
            // Any failure before the apply script is running must return to the confirmation
            // buttons. Once the script is scheduled, that reset would start the download again.
            UpdateStatus.Text = exception.Message;
            UpdateProgress.IsVisible = false;
            UpdateConfirm.IsEnabled = true;
            UpdateDismiss.IsEnabled = true;
            return;
        }

        UpdateStatus.Text = "A fechar para aplicar a atualização...";
        UpdateProgress.IsIndeterminate = false;
        UpdateProgress.Value = 100;
        _allowClose = true;
        _clock.Stop();
        // Leave the UI thread before Exit. Shutdown() calls Dispatcher.InvokeShutdown on
        // this thread; doing that inside the download try/catch brought the confirmation
        // dialog back whenever shutdown threw, and the apply script never saw the process end.
        var exit = new Thread(() =>
        {
            Thread.Sleep(800);
            Environment.Exit(0);
        })
        {
            IsBackground = false,
            Name = "logicpos-update-exit"
        };
        exit.Start();
        try
        {
            if (global::Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }
        catch (Exception exception)
        {
            AppLogging.Warning(exception, "Updater shutdown failed after the apply script was scheduled");
        }
    }

    private static int? ParseUpdatePercent(string text)
    {
        var start = text.IndexOf('%');
        if (start <= 0)
        {
            return null;
        }

        var cursor = start - 1;
        while (cursor >= 0 && char.IsDigit(text[cursor]))
        {
            cursor--;
        }

        var digits = text.Substring(cursor + 1, start - cursor - 1);
        return int.TryParse(digits, out var value) ? Math.Clamp(value, 0, 100) : null;
    }

    private void OnExitClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        QuitOverlay.IsVisible = true;
    }

    private void OnAboutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        RefreshAboutDetails();
        AboutOverlay.IsVisible = true;
    }

    private void OnAboutCloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AboutOverlay.IsVisible = false;
    }

    private void RefreshAboutDetails()
    {
        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        var number = string.IsNullOrWhiteSpace(version)
            ? Assembly.GetExecutingAssembly().GetName().Version?.ToString(3)
            : version.Split('+')[0];
        VersionLabel.Text = string.IsNullOrWhiteSpace(number) ? "Versão" : $"Versão {number}";
        AboutCopyright.Text = $"{DateTime.Now.Year} Copyright © Logicpulse";

        var hardwareId = AppComposition.Services?.GetService<ILicenseModule>()?.HardwareId;
        if (string.IsNullOrWhiteSpace(hardwareId))
        {
            hardwareId = MachineIdentity.HardwareId;
        }

        AboutHardwareId.Text = string.IsNullOrWhiteSpace(hardwareId) ? "—" : hardwareId;
        AboutHardwareId.IsVisible = true;
    }

    private void OnQuitDismissClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        QuitOverlay.IsVisible = false;
    }

    private async void OnQuitConfirmClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await ReleaseLicenceSeatAsync().ConfigureAwait(true);
        _allowClose = true;
        if (global::Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
            return;
        }

        // Android: sem janela — usar callback registado pelo MainActivity.
        App.ExitApplication?.Invoke();
    }

    private async void RaiseLogout()
    {
        await ReleaseLicenceSeatAsync().ConfigureAwait(true);
        _allowClose = true;
        _clock.Stop();
        LoggedOut?.Invoke(this, EventArgs.Empty);
        RequestHostClose();
    }

    /// <summary>
    /// Releases the concurrent licence seat on the API so Web/other clients can claim it immediately.
    /// </summary>
    private static async Task ReleaseLicenceSeatAsync()
    {
        var login = AppComposition.Services?.GetService<ILoginService>();
        if (login is null)
        {
            return;
        }

        try
        {
            await login.SignOutAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Licence seat release failed: {ex}");
        }
    }

}
