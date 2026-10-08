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
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Core.Licensing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class BackOfficeWindow : Window, IOfficeSurface
{
    private const string ClockFormat = "dddd, dd' de 'MMMM' de 'yyyy' || 'HH:mm:ss tt";
    private const string IconRoot = "avares://LogicPOS.App/Assets/Images/BackOffice/";

    private readonly DispatcherTimer _clock = new() { Interval = TimeSpan.FromSeconds(1) };
    private readonly List<StackPanel> _sectionPanels = new();
    private readonly List<Button> _sectionButtons = new();
    private Button? _selectedItem;
    private bool _allowClose;
    private bool _backOfficeOnly;
    private StockManagementView? _stockView;

    public BackOfficeWindow()
        : this(string.Empty)
    {
    }

    public BackOfficeWindow(string sessionLabel)
    {
        InitializeComponent();
        ArticleDashboardHost.OpenStockRequested += (_, _) => ShowPage("Gestão de Stocks");
        NewDocumentHost.PreviewRequested += async (path, title) => await ShowPdfAsync(path, title);
        ReportsHost.PreviewRequested += async (path, title) => await ShowPdfAsync(path, title);
        ReportsHost.Closed += (_, _) => ReportsOverlay.IsVisible = false;
        MarkFlag(UiCulture.Read(null));
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
        _clock.Tick += (_, _) => ClockLabel.Text = DateTime.Now.ToString(ClockFormat);
        Opened += (_, _) => RefreshSession();
        Closing += (_, e) =>
        {
            if (_allowClose)
            {
                return;
            }

            e.Cancel = true;
            QuitOverlay.IsVisible = true;
        };
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
        FrontOfficeButton.IsVisible = false;
    }

    public event EventHandler? ReturnToPos;

    public event EventHandler? LoggedOut;

    public async Task ShowNewDocumentAsync(Guid? draftId = null)
    {
        NewDocumentOverlay.IsVisible = true;
        var created = await NewDocumentHost.ShowAsync(draftId);
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

    public async Task ShowPdfAsync(string path, string? title, Guid? documentId = null)
    {
        var reportsOpen = ReportsOverlay.IsVisible;
        ReportsOverlay.IsVisible = false;
        PdfOverlay.IsVisible = true;
        await PdfHost.ShowAsync(path, title, documentId);
        PdfOverlay.IsVisible = false;
        ReportsOverlay.IsVisible = reportsOpen;
    }

    public void SetSessionLabel(string sessionLabel) => TerminalLabel.Text = sessionLabel;

    public void RefreshSession()
    {
        ClockLabel.Text = DateTime.Now.ToString(ClockFormat);
        _clock.Start();
        _ = ReloadDashboardSafeAsync();
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

    private void BuildMenu()
    {
        var sections = new (string Title, string Icon, (string Title, bool Logout)[] Items)[]
        {
            ("Documentos", "Accordion/pos_backoffice_documentos.png", new[]
            {
                ("Novo Doc.", false),
                ("Documentos", false),
                ("Emissão Recibos", false),
                ("Recibos", false),
                ("Conta.Corr.", false),
                ("Sessões de Trab.", false)
            }),
            ("Relatórios", "Accordion/pos_backoffice_relatorios.png", new[]
            {
                ("Relatórios", false)
            }),
            ("Artigos", "Accordion/pos_backoffice_artigos.png", new[]
            {
                ("Famílias", false),
                ("Subfamílias", false),
                ("Artigos", false),
                ("Tipo de artigos", false),
                ("Classe do artigo", false),
                ("Tipo de Preço", false),
                ("Gestão de Stocks", false),
                ("Importar Artigos", false),
                ("Exportar Artigos", false)
            }),
            ("Informação fiscal", "Accordion/pos_backoffice_informacao_fiscal.png", new[]
            {
                ("Abertura de ano fiscal", false),
                ("Séries", false),
                ("Tipo de documento", false),
                ("Taxas de imposto", false),
                ("Motivo de isenção de IVA", false),
                ("Cond. de Pagamento", false),
                ("Métodos de pagamento", false)
            }),
            ("Clientes", "Accordion/pos_backoffice_clientes.png", new[]
            {
                ("Clientes", false),
                ("Tipo de clientes", false),
                ("Grupo de descontos", false),
                ("Importar Clientes", false),
                ("Exportar Clientes", false)
            }),
            ("Utilizadores", "Accordion/pos_backoffice_utilizadores.png", new[]
            {
                ("Utilizadores", false),
                ("Permissões", false),
                ("Grupo de comissões", false)
            }),
            ("Dispositivos", "Accordion/pos_backoffice_impressoras.png", new[]
            {
                ("Tipos de impressora", false),
                ("Impressoras", false),
                ("Dispositivos de Entrada", false),
                ("Display de Cliente", false),
                ("Balanças", false)
            }),
            ("Outras Tabelas", "Accordion/pos_backoffice_outras_tabelas.png", new[]
            {
                ("País", false),
                ("Moeda", false),
                ("Locais", false),
                ("Mesas", false),
                ("Tipo de Movimento", false),
                ("Unidades de medida", false),
                ("Unidades de tamanho", false),
                ("Feriados", false),
                ("Armazém", false)
            }),
            ("Configuração", "Accordion/pos_backoffice_configuracao.png", new[]
            {
                ("Parâmetros da Empresa", false),
                ("Parâmetros de Sistema", false),
                ("Terminais", false)
            }),
            ("Exportar", "Accordion/pos_backoffice_export.png", new[]
            {
                ("Exportar Artigos", false),
                ("Exportar Clientes", false),
                ("SAF-T ano", false),
                ("SAF-T último mês", false),
                ("SAF-T período", false)
            }),
            ("Sistema", "Accordion/pos_backoffice_sistema.png", new[]
            {
                ("Notificações", false),
                ("Registro de alterações (Changelog)", false),
                ("Backup DB", false),
                ("Restaurar DB", false),
                ("Restaurar de ficheiro", false),
                ("Atualizar", false),
                ("Sair da Sessão", true)
            })
        };

        var menu = sections.ToList();
        if (IsAngola())
        {
            var exportIndex = menu.FindIndex(section => section.Title == "Exportar");
            menu.Insert(exportIndex < 0 ? menu.Count : exportIndex, ("AGT", "Accordion/pos_backoffice_informacao_fiscal.png", new[]
            {
                ("Séries AGT", false),
                ("Documentos AGT", false)
            }));
        }

        foreach (var section in menu)
        {
            var direct = section.Items.Length == 1
                && section.Items[0].Logout == false
                && section.Items[0].Title == section.Title;
            var children = new StackPanel { IsVisible = false, Classes = { "bo_menu_children" } };
            if (direct == false)
            {
                foreach (var item in section.Items)
                {
                    var logout = item.Logout;
                    var title = item.Title;
                    var child = new Button
                    {
                        Classes = { "bo_menu_child" },
                        Content = AppText.Get(title),
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

                        ShowPage(title);
                    };
                    children.Children.Add(child);
                }
            }

            var parent = new Button
            {
                Classes = { "bo_menu_parent" },
                Content = CreateRow($"{IconRoot}{section.Icon}", AppText.Get(section.Title))
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
                    OpenReports();
                    return;
                }

                var opening = children.IsVisible == false;
                ToggleSection(children, parent);
                if (opening && section.Title == "Artigos")
                {
                    _ = ShowArticleDashboardAsync();
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

        if (allowed is null)
        {
            return;
        }

        foreach (var panel in _sectionPanels)
        {
            foreach (var child in panel.Children.OfType<Button>())
            {
                if (child.Tag is string title)
                {
                    child.IsEnabled = allowed.Contains(title);
                }
            }
        }
    }

    private static Control CreateRow(string iconUri, string text)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };
        var glyph = new Border { Classes = { "bo_menu_glyph" } };
        glyph.OpacityMask = new ImageBrush(new Bitmap(AssetLoader.Open(new Uri(iconUri))))
        {
            Stretch = Stretch.Uniform
        };
        panel.Children.Add(glyph);
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

    public void OpenPage(string title) => ShowPage(title);

    private void OpenReports()
    {
        ReportsOverlay.IsVisible = true;
        _ = ReportsHost.ShowAsync();
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
        if (title is "Relatórios")
        {
            OpenReports();
            return;
        }

        PageTitle.Text = title;
        _articleDashboardRequest++;
        ReportsOverlay.IsVisible = false;

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
        ReportsOverlay.IsVisible = false;
        DashboardHost.IsVisible = false;
        PageHost.IsVisible = false;
        ArticleDashboardHost.IsVisible = true;
        _ = ArticleDashboardHost.ReloadAsync();
    }

    private void OnDashboardClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SelectMenuItem(DashboardButton);
        _articleDashboardRequest++;
        PageTitle.Text = string.Empty;
        DashboardHost.IsVisible = true;
        ArticleDashboardHost.IsVisible = false;
        PageHost.IsVisible = false;
        DocumentsHost.IsVisible = false;
        EntityHost.IsVisible = false;
        StockHost.IsVisible = false;
        _ = DashboardHost.ReloadAsync();
    }

    private void OnPosClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_backOfficeOnly)
        {
            return;
        }

        _clock.Stop();
        ReturnToPos?.Invoke(this, EventArgs.Empty);
    }

    private void OnMinimizeClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void OnMaximizeClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowDecorations = WindowDecorations.None;
        CanResize = false;
        WindowState = WindowState.Maximized;
    }

    private void OnRestoreClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowDecorations = WindowDecorations.BorderOnly;
        CanResize = true;
        WindowState = WindowState.Normal;
        Width = 1400;
        Height = 860;
        CenterOnScreen();
    }

    private void OnWindowCloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_backOfficeOnly)
        {
            QuitOverlay.IsVisible = true;
            return;
        }

        _clock.Stop();
        ReturnToPos?.Invoke(this, EventArgs.Empty);
    }

    private void OnStatusPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (WindowState != WindowState.Normal || e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == false)
        {
            return;
        }

        if (e.Source is Visual source && source.FindAncestorOfType<Button>() is not null)
        {
            return;
        }

        BeginMoveDrag(e);
    }

    private void CenterOnScreen()
    {
        var screen = Screens.ScreenFromVisual(this);
        if (screen is null)
        {
            return;
        }

        var area = screen.WorkingArea;
        var scale = screen.Scaling;
        var width = (int)(Width * scale);
        var height = (int)(Height * scale);
        Position = new PixelPoint(
            area.X + Math.Max(0, (area.Width - width) / 2),
            area.Y + Math.Max(0, (area.Height - height) / 2));
    }

    private void ShowUpdate()
    {
        UpdateStatus.Text = "A configuração e a base de dados local não são substituídas.";
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
        var progress = new Progress<string>(text => UpdateStatus.Text = text);
        try
        {
            await ApplicationUpdater.DownloadAndScheduleAsync(progress);
            _allowClose = true;
            _clock.Stop();
            if (global::Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
                return;
            }

            Close();
        }
        catch (Exception exception)
        {
            UpdateStatus.Text = exception.Message;
            UpdateConfirm.IsEnabled = true;
            UpdateDismiss.IsEnabled = true;
        }
    }

    private void OnExitClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        QuitOverlay.IsVisible = true;
    }

    private void OnAboutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AboutOverlay.IsVisible = true;
    }

    private void OnAboutCloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AboutOverlay.IsVisible = false;
    }

    private void OnQuitDismissClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        QuitOverlay.IsVisible = false;
    }

    private void OnQuitConfirmClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _allowClose = true;
        if (global::Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
            return;
        }

        Close();
    }

    private void RaiseLogout()
    {
        _allowClose = true;
        _clock.Stop();
        LoggedOut?.Invoke(this, EventArgs.Empty);
        Close();
    }

}
