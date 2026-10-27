using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using LogicPOS.App.Branding;
using LogicPOS.Core;
using LogicPOS.Core.Authentication;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.Licensing;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

/// <summary>
/// Login UI as UserControl so Android (no Window platform) and desktop shell can host the same view.
/// </summary>
public partial class LoginWindow : UserControl
{
    private readonly LoginViewModel _viewModel;
    private bool _registrationPrompted;
    private bool _activated;
    private bool _sessionOpening;
    private UiViewport _viewport;

    public LoginWindow()
    {
        InitializeComponent();
        TouchFields.Attach(this);

        var loginService = AppComposition.Services?.GetService<ILoginService>();
        _viewModel = new LoginViewModel(loginService, AppComposition.StartupError);
        _viewModel.QuitConfirmed += (_, _) => RequestClose();
        _viewModel.TerminalChoiceRequested += (_, _) => ShowTerminalChoice();
        _viewModel.SessionStarted += (_, userName) =>
        {
            _sessionOpening = true;
            _ = ShowSessionAsync(userName);
        };
        DataContext = _viewModel;

        AddHandler(KeyDownEvent, OnWindowKeyDown, RoutingStrategies.Tunnel);
        SizeChanged += (_, _) => ApplyViewportLayout();
    }

    /// <summary>
    /// Shared startup for desktop shell and Android single-view host.
    /// </summary>
    public async Task ActivateHostedAsync()
    {
        if (_activated)
        {
            return;
        }

        _activated = true;
        ApplyBranding();
        ApplyViewportLayout();
        await _viewModel.LoadAsync();
        PinEntryFocus();
        // Defer so the host finishes attaching before a modal dialog.
        Dispatcher.UIThread.Post(() => _ = PromptStartupDialogsAsync(), DispatcherPriority.Background);
    }

    private void RequestClose()
    {
        if (ResolveDialogOwner() is { } owner)
        {
            owner.Close();
            return;
        }

        if (global::Avalonia.Application.Current?.ApplicationLifetime is IControlledApplicationLifetime life)
        {
            life.Shutdown();
            return;
        }

        // Android: sem janela nem desktop lifetime — usar callback registado pelo MainActivity.
        App.ExitApplication?.Invoke();
    }

    private async Task PromptStartupDialogsAsync()
    {
        await PromptCompanySetupIfNeededAsync();
        await PromptRegistrationIfNeededAsync();
    }

    private async Task PromptCompanySetupIfNeededAsync()
    {
        var setup = AppComposition.Services?.GetService<ICompanySetupService>();
        if (setup is null)
        {
            return;
        }

        try
        {
            if (await setup.NeedsSetupAsync() == false)
            {
                return;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Company setup check failed: {ex}");
            return;
        }

        var owner = ResolveDialogOwner();
        if (owner is null)
        {
            return;
        }

        await new CompanySetupWindow(setup).ShowDialog(owner);
    }

    /// <summary>
    /// Desktop: LoginShellWindow. Android: no Window owner (modals skipped).
    /// </summary>
    private Window? ResolveDialogOwner()
    {
        if (TopLevel.GetTopLevel(this) is Window top)
        {
            return top;
        }

        return this.GetVisualAncestors().OfType<Window>().FirstOrDefault();
    }

    private void ApplyBranding()
    {
        try
        {
            _viewModel.VersionText = FormatLoginVersion();
            var logo = AppBranding.LoadLoginLogo();
            LoginLogo.Source = logo;
            PortraitLogo.Source = logo;
        }
        catch
        {
            // Keep XAML defaults when branding assets are missing.
        }
    }

    private static string FormatLoginVersion()
    {
        var version = ReadAssemblyVersion();
        return $"Powered by LogicPulse Technologies © Vers. v{version}";
    }

    private static string ReadAssemblyVersion()
    {
        // Prefer stamped informational version (e.g. "1.6.2+retail"); keep only Major.Minor.Patch.
        var informational = typeof(LoginWindow).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
        var raw = string.IsNullOrWhiteSpace(informational)
            ? (typeof(LoginWindow).Assembly.GetName().Version?.ToString(3) ?? "1.6.0")
            : informational.Trim();

        var plus = raw.IndexOf('+');
        if (plus >= 0)
        {
            raw = raw[..plus];
        }

        raw = raw.TrimStart('v', 'V');
        var parts = raw.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length >= 3)
        {
            return $"{parts[0]}.{parts[1]}.{parts[2]}";
        }

        return string.IsNullOrWhiteSpace(raw) ? "1.6.0" : raw;
    }

    private async Task PromptRegistrationIfNeededAsync()
    {
        if (_registrationPrompted)
        {
            return;
        }

        _registrationPrompted = true;
        var license = AppComposition.Services?.GetService<ILicenseModule>();
        if (license is null)
        {
            return;
        }

        bool needsRegistration;
        try
        {
            await Task.Run(async () => await license.RefreshAsync().ConfigureAwait(false)).ConfigureAwait(true);
            needsRegistration = await Task.Run(() => license.RegistrationRequired).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Licence check failed: {ex}");
            return;
        }

        if (needsRegistration == false)
        {
            return;
        }

        var owner = ResolveDialogOwner();
        if (owner is null)
        {
            return;
        }

        await new RegistrationWindow(license, requireRegistration: true).ShowDialog(owner);
        ApplyBranding();

        if (license.RegistrationRequired)
        {
            RequestClose();
        }
    }

    private Guid? _selectedTerminalId;
    private bool _terminalSearchHooked;

    private void ShowTerminalChoice()
    {
        if (_terminalSearchHooked == false)
        {
            TerminalSearch.TextChanged += (_, _) => RenderTerminalChoices();
            _terminalSearchHooked = true;
        }

        RenderTerminalChoices();
        TerminalOverlay.IsVisible = true;
    }

    private static void AddTerminalCell(Grid grid, string text, int column)
    {
        var label = new TextBlock
        {
            Classes = { "pos_pay_label" },
            Text = text,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            TextTrimming = Avalonia.Media.TextTrimming.CharacterEllipsis
        };
        Grid.SetColumn(label, column);
        grid.Children.Add(label);
    }

    private void RenderTerminalChoices()
    {
        TerminalHost.Children.Clear();
        var query = TerminalSearch.Text?.Trim() ?? string.Empty;
        var rows = _viewModel.TerminalChoices.Where(terminal =>
            query.Length == 0
            || terminal.Code.Contains(query, StringComparison.OrdinalIgnoreCase)
            || terminal.Designation.Contains(query, StringComparison.OrdinalIgnoreCase)
            || terminal.HardwareId.Contains(query, StringComparison.OrdinalIgnoreCase));

        foreach (var terminal in rows)
        {
            var choice = terminal;
            var columns = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("72,*,88,2*,168"),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            AddTerminalCell(columns, choice.Code, 0);
            AddTerminalCell(columns, choice.Designation, 1);
            AddTerminalCell(columns, choice.IsDefault ? "Sim" : "Não", 2);
            AddTerminalCell(columns, choice.HardwareId, 3);
            AddTerminalCell(columns, choice.UpdatedAt?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty, 4);
            var button = new Button
            {
                Classes = { "bo_report_row" },
                Content = columns,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            if (_selectedTerminalId == choice.Id)
            {
                button.Classes.Add("bo_report_row_on");
            }

            button.Click += (_, _) =>
            {
                _selectedTerminalId = choice.Id;
                _viewModel.SelectTerminal(choice.Id);
                RenderTerminalChoices();
            };
            TerminalHost.Children.Add(button);
        }
    }

    private async void OnTerminalConfirmClick(object? sender, RoutedEventArgs e)
    {
        var isAndroid = OperatingSystem.IsAndroid();
        if (isAndroid)
        {
            MobileHostView.ShowLoadingOverlay("A associar terminal…");
            await MobileHostView.YieldForPaintAsync();
        }

        _sessionOpening = false;
        var error = await _viewModel.ConfirmTerminalAsync();
        TerminalNotice.Text = error ?? string.Empty;
        if (string.IsNullOrWhiteSpace(error) == false)
        {
            if (isAndroid)
            {
                MobileHostView.HideLoadingOverlay();
            }

            return;
        }

        TerminalOverlay.IsVisible = false;
        if (isAndroid && _sessionOpening == false)
        {
            MobileHostView.HideLoadingOverlay();
        }

        PinEntryFocus();
    }

    private void OnTerminalCancelClick(object? sender, RoutedEventArgs e)
    {
        TerminalOverlay.IsVisible = false;
    }

    private bool? _lastPortrait;

    private void ApplyViewportLayout()
    {
        _viewport = UiViewport.From(this);

        if (_viewport.IsPortrait)
        {
            LoginArtboard.Width = _viewport.Width;
            LoginArtboard.Height = _viewport.Height;
        }
        else
        {
            LoginArtboard.Width = _viewport.DesignWidth;
            LoginArtboard.Height = _viewport.DesignHeight;
        }

        Classes.Set("login_portrait", _viewport.IsPortrait);
        Classes.Set("login_landscape", _viewport.IsLandscape);

        // Alternar painéis landscape / portrait — única fonte de verdade para visibilidade.
        LandscapePanel.IsVisible = !_viewport.IsPortrait;
        PortraitLoginPanel.IsVisible = _viewport.IsPortrait;

        if (_lastPortrait != _viewport.IsPortrait)
        {
            _lastPortrait = _viewport.IsPortrait;

            // Landscape: restaurar orientação vertical dos utilizadores.
            if (_viewport.IsLandscape && LoginUsers.ItemsPanelRoot is StackPanel usersPanel)
            {
                usersPanel.Orientation = Avalonia.Layout.Orientation.Vertical;
            }
        }

        if (_viewport.IsPortrait)
        {
            ApplyPortraitChrome();
            TerminalFrame.Width = Math.Min(_viewport.Width - 40, 700);
            TerminalFrame.Height = Math.Min(_viewport.Height - 120, 900);
            // Limitar alertas ao viewport em portrait para nunca transbordar o ecrã.
            AlertFrame.MaxWidth = Math.Max(240, _viewport.Width - 32);
            AlertFrame.MaxHeight = Math.Max(200, _viewport.Height - 80);
        }
        else
        {
            ApplyLandscapeChrome();
            TerminalFrame.Width = 980;
            TerminalFrame.Height = 520;
            AlertFrame.ClearValue(Avalonia.Layout.Layoutable.MaxWidthProperty);
            AlertFrame.ClearValue(Avalonia.Layout.Layoutable.MaxHeightProperty);
        }

        var pageSize = _viewport.IsPortrait
            ? 4
            : (_viewport.DesignHeight - (120 * 2) - (60 * 2)) / 102;
        _viewModel.SetPageSize(pageSize < 1 ? 1 : pageSize);
    }

    /// <summary>
    /// Portrait: dimensionar o cartão e logo proporcionalmente ao ecrã; centrado verticalmente.
    /// </summary>
    private void ApplyPortraitChrome()
    {
        var w = _viewport.Width;

        var logoWidth = Math.Min(300, w * 0.42);
        PortraitLogo.Width = logoWidth;
        PortraitLogo.Height = double.NaN;

        var cardWidth = Math.Min(520, w - 40);
        PortraitCard.Width = cardWidth;
        PortraitCard.Margin = default;
    }

    /// <summary>
    /// Landscape: restaurar posições do artboard fixo 1920×1080.
    /// </summary>
    private void ApplyLandscapeChrome()
    {
        LoginLogo.Width = 460;
        LoginLogo.Height = 157;
        LoginLogo.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
        LoginLogo.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
        LoginLogo.Margin = new Avalonia.Thickness(0, 16, 48, 0);

        LoginUsersFrame.Width = 148;
        LoginUsersFrame.Height = 861;
        LoginUsersFrame.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
        LoginUsersFrame.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
        LoginUsersFrame.Margin = new Avalonia.Thickness(108, 105, 0, 0);

        LoginUsersColumn.Width = 120;
        LoginUsersColumn.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
        LoginUsersColumn.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
        LoginUsersColumn.Margin = new Avalonia.Thickness(120, 120, 0, 0);

        LoginUsers.Width = 120;
        LoginUsers.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
        LoginUsersPrev.Width = 120;
        LoginUsersPrev.Height = 60;
        LoginUsersPrev.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
        LoginUsersNext.Width = 120;
        LoginUsersNext.Height = 60;
        LoginUsersNext.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;

        LoginPinpadFrame.Width = 344;
        LoginPinpadFrame.Height = 466;
        LoginPinpadFrame.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
        LoginPinpadFrame.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        LoginPinpadFrame.Margin = new Avalonia.Thickness(0, 0, 104, 0);

        LoginPinpad.Width = 315;
        LoginPinpad.Height = 466;
        LoginPinpad.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
        LoginPinpad.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        LoginPinpad.Margin = new Avalonia.Thickness(0, 0, 120, 0);
    }

    private async Task ShowSessionAsync(string userName)
    {
        var isAndroid = OperatingSystem.IsAndroid();
        if (isAndroid)
        {
            // Overlay por cima do login (não limpar o Host) — já pode estar visível desde o Entrar.
            MobileHostView.ShowLoadingOverlay("A preparar o ecrã…");
            await MobileHostView.YieldForPaintAsync();
        }

        var backOfficeOnly = false;
        var mode = AppComposition.Services?.GetService<IOperationMode>();
        if (mode is not null)
        {
            try
            {
                backOfficeOnly = await Task.Run(async () => await mode.IsBackOfficeOnlyAsync())
                    .ConfigureAwait(true);
            }
            catch (Exception)
            {
                backOfficeOnly = false;
            }
        }

        try
        {
            if (backOfficeOnly)
            {
                if (isAndroid)
                {
                    MobileHostView.UpdateLoadingMessage("A abrir o BackOffice…");
                    await MobileHostView.YieldForPaintAsync();
                }

                var label = await Task.Run(async () => await SessionLabelAsync(userName))
                    .ConfigureAwait(true);
                var office = new BackOfficeWindow(label);
                office.UseBackOfficeOnly();
                void OnBoLoggedOut()
                {
                    _viewModel.ClearSession();
                    IsVisible = true;
                    PinEntryFocus();
                }

                if (isAndroid)
                {
                    MobileHostView.ShowSession(office, OnBoLoggedOut);
                    await office.ActivateHostedAsync();
                    MobileHostView.HideLoadingOverlay();
                    return;
                }

                office.LoggedOut += (_, _) => OnBoLoggedOut();
                var boShell = new BoShellWindow(office);
                boShell.Show();
                return;
            }

            if (isAndroid)
            {
                MobileHostView.UpdateLoadingMessage("A construir o ecrã POS…");
                await MobileHostView.YieldForPaintAsync();
            }

            // Construtor pesado (InitializeComponent) — overlay já pintado por cima.
            var pos = new PosWindow(userName);

            if (isAndroid)
            {
                await MobileHostView.YieldForPaintAsync();
                MobileHostView.UpdateLoadingMessage("A carregar o catálogo…");
            }

            void OnLoggedOut()
            {
                _viewModel.ClearSession();
                IsVisible = true;
                PinEntryFocus();
            }

            if (isAndroid)
            {
                // POS fica por baixo do overlay até ActivateHostedAsync terminar.
                MobileHostView.ShowSession(pos, OnLoggedOut);
                await pos.ActivateHostedAsync();
                MobileHostView.HideLoadingOverlay();
                return;
            }

            pos.LoggedOut += (_, _) => OnLoggedOut();
            var shell = new PosShellWindow(pos);
            shell.Show();
        }
        catch (Exception exception)
        {
            App.WriteCrash("ShowSessionAsync", exception);
            if (isAndroid)
            {
                MobileHostView.HideLoadingOverlay();
            }

            IsVisible = true;
            PinEntryFocus();
            var owner = ResolveDialogOwner();
            if (owner is null)
            {
                return;
            }

            var box = new Window
            {
                Title = "Erro após login",
                Width = 720,
                Height = 420,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new ScrollViewer
                {
                    Content = new TextBox
                    {
                        Text = exception.ToString(),
                        IsReadOnly = true,
                        AcceptsReturn = true,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    }
                }
            };
            await box.ShowDialog(owner);
        }
    }

    /// <summary>
    /// Entrar / tecla Enter: loading imediato no Android, depois auth.
    /// </summary>
    private async Task SubmitLoginAsync()
    {
        var isAndroid = OperatingSystem.IsAndroid();
        if (isAndroid)
        {
            MobileHostView.ShowLoadingOverlay("A autenticar…");
            await MobileHostView.YieldForPaintAsync();
        }

        _sessionOpening = false;
        await _viewModel.SubmitAsync();

        // Falha de login / PIN errado: esconder overlay. Sucesso: ShowSessionAsync trata.
        if (isAndroid && _sessionOpening == false)
        {
            MobileHostView.HideLoadingOverlay();
        }
    }

    private static async Task<string> SessionLabelAsync(string userName)
    {
        var login = AppComposition.Services?.GetService<ILoginService>();
        if (login is null)
        {
            return SessionCaption.Format(null, userName);
        }

        try
        {
            var terminalName = await login.GetFirstTerminalNameAsync();
            return SessionCaption.Format(terminalName, userName);
        }
        catch (Exception)
        {
            return SessionCaption.Format(null, userName);
        }
    }

    private void PinEntryFocus()
    {
        var name = _viewport.IsPortrait ? "PortraitPinEntry" : "PinEntry";
        var entry = this.FindControl<TextBox>(name);
        if (entry is null)
        {
            return;
        }

        entry.IsReadOnly = true;
        entry.Focus();
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (_viewModel.AlertVisible)
        {
            if (e.Key == Key.Enter)
            {
                _viewModel.ConfirmAlert();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape && _viewModel.AlertShowsCancel)
            {
                _viewModel.DismissAlert();
                e.Handled = true;
            }

            return;
        }

        var digit = DigitFromKey(e.Key);
        if (digit is not null)
        {
            _viewModel.AppendDigit(digit);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Back)
        {
            _viewModel.Backspace();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            _ = SubmitLoginAsync();
            e.Handled = true;
        }
    }

    private static string? DigitFromKey(Key key) => key switch
    {
        Key.D0 or Key.NumPad0 => "0",
        Key.D1 or Key.NumPad1 => "1",
        Key.D2 or Key.NumPad2 => "2",
        Key.D3 or Key.NumPad3 => "3",
        Key.D4 or Key.NumPad4 => "4",
        Key.D5 or Key.NumPad5 => "5",
        Key.D6 or Key.NumPad6 => "6",
        Key.D7 or Key.NumPad7 => "7",
        Key.D8 or Key.NumPad8 => "8",
        Key.D9 or Key.NumPad9 => "9",
        _ => null
    };

    private void OnDigitClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string digit })
        {
            _viewModel.AppendDigit(digit);
        }
    }

    private void OnTogglePinClick(object? sender, RoutedEventArgs e)
    {
        _viewModel.TogglePinVisibility();
        PinEntryFocus();
    }

    private void OnClearClick(object? sender, RoutedEventArgs e) => _viewModel.ClearPin();

    private async void OnSubmitClick(object? sender, RoutedEventArgs e) => await SubmitLoginAsync();

    private async void OnResetPasswordClick(object? sender, RoutedEventArgs e)
    {
        if (OperatingSystem.IsAndroid())
        {
            MobileHostView.ShowLoadingOverlay("A autenticar…");
            await MobileHostView.YieldForPaintAsync();
        }

        try
        {
            await _viewModel.BeginPasswordResetAsync();
        }
        finally
        {
            // Alterar PIN fica no ecrã de login — esconder overlay (excepto se sessão já arrancou).
            if (OperatingSystem.IsAndroid() && _sessionOpening == false)
            {
                MobileHostView.HideLoadingOverlay();
            }
        }
    }

    private void OnQuitClick(object? sender, RoutedEventArgs e) => _viewModel.RequestQuit();

    private void OnAlertConfirmClick(object? sender, RoutedEventArgs e) => _viewModel.ConfirmAlert();

    private void OnAlertDismissClick(object? sender, RoutedEventArgs e) => _viewModel.DismissAlert();

    private void OnPreviousUsersClick(object? sender, RoutedEventArgs e) => _viewModel.ShowPreviousPage();

    private void OnNextUsersClick(object? sender, RoutedEventArgs e) => _viewModel.ShowNextPage();

    private void OnUserClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: UserSlot slot })
        {
            _viewModel.SelectSlot(slot);
        }
    }

    private void OnMinimizeClick(object? sender, RoutedEventArgs e)
    {
        if (ResolveDialogOwner() is { } owner)
        {
            owner.WindowState = Avalonia.Controls.WindowState.Minimized;
        }
    }
}
