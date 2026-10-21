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
    private UiViewport _viewport;

    public LoginWindow()
    {
        InitializeComponent();
        TouchFields.Attach(this);

        var loginService = AppComposition.Services?.GetService<ILoginService>();
        _viewModel = new LoginViewModel(loginService, AppComposition.StartupError);
        _viewModel.QuitConfirmed += (_, _) => RequestClose();
        _viewModel.TerminalChoiceRequested += (_, _) => ShowTerminalChoice();
        _viewModel.SessionStarted += (_, userName) => _ = ShowSessionAsync(userName);
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
        }
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
            LoginLogo.Source = AppBranding.LoadLoginLogo();
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
            // Re-sync with ERP (soft-delete / revoke) then read RegistrationRequired.
            // CloudApiLicenseModule uses a timeout and must not deadlock the UI.
            await Task.Run(async () => await license.RefreshAsync().ConfigureAwait(false)).ConfigureAwait(true);
            needsRegistration = await Task.Run(() => license.RegistrationRequired).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            // Soft-fail: login stays usable; CloudApiLicenseModule already treats API failure as unlicensed.
            System.Diagnostics.Debug.WriteLine($"Licence check failed: {ex}");
            return;
        }

        if (needsRegistration == false)
        {
            return;
        }

        // Mandatory: no soft-lock skip. Exit closes the login window; Register must succeed.
        var owner = ResolveDialogOwner();
        if (owner is null)
        {
            return;
        }

        await new RegistrationWindow(license, requireRegistration: true).ShowDialog(owner);
        ApplyBranding();

        if (license.RegistrationRequired)
        {
            // Still unlicensed after dismiss/exit — do not leave a usable login screen.
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
        var error = await _viewModel.ConfirmTerminalAsync();
        TerminalNotice.Text = error ?? string.Empty;
        if (string.IsNullOrWhiteSpace(error) == false)
        {
            return;
        }

        TerminalOverlay.IsVisible = false;
        PinEntryFocus();
    }

    private void OnTerminalCancelClick(object? sender, RoutedEventArgs e)
    {
        TerminalOverlay.IsVisible = false;
    }

    private bool? _usersPanelPortrait;

    private void ApplyViewportLayout()
    {
        _viewport = UiViewport.From(this);
        LoginArtboard.Width = _viewport.DesignWidth;
        LoginArtboard.Height = _viewport.DesignHeight;
        Classes.Set("login_portrait", _viewport.IsPortrait);
        Classes.Set("login_landscape", _viewport.IsLandscape);

        // Portrait chrome (logo / frames / pinpad) is in Styles.axaml via UserControl.login_portrait.
        if (_usersPanelPortrait != _viewport.IsPortrait)
        {
            _usersPanelPortrait = _viewport.IsPortrait;
            var orientation = _viewport.IsPortrait
                ? Avalonia.Layout.Orientation.Horizontal
                : Avalonia.Layout.Orientation.Vertical;

            // Reuse the XAML ItemsPanel StackPanel; only flip orientation.
            if (LoginUsers.ItemsPanelRoot is StackPanel usersPanel)
            {
                usersPanel.Orientation = orientation;
            }

            // Up/down glyphs rotated → left/right for the horizontal strip.
            LoginUsersPrevIcon.RenderTransform = _viewport.IsPortrait
                ? new Avalonia.Media.RotateTransform(-90)
                : null;
            LoginUsersNextIcon.RenderTransform = _viewport.IsPortrait
                ? new Avalonia.Media.RotateTransform(-90)
                : null;
        }

        if (_viewport.IsPortrait)
        {
            TerminalFrame.Width = 1000;
            TerminalFrame.Height = 1400;
        }
        else
        {
            TerminalFrame.Width = 980;
            TerminalFrame.Height = 520;
        }

        // Landscape: vertical column. Portrait: one horizontal strip (~5 slots).
        var pageSize = _viewport.IsPortrait
            ? 5
            : (_viewport.DesignHeight - (120 * 2) - (60 * 2)) / 102;
        _viewModel.SetPageSize(pageSize < 1 ? 1 : pageSize);
    }

    private async Task ShowSessionAsync(string userName)
    {
        var backOfficeOnly = false;
        var mode = AppComposition.Services?.GetService<IOperationMode>();
        if (mode is not null)
        {
            try
            {
                backOfficeOnly = await mode.IsBackOfficeOnlyAsync();
            }
            catch (Exception)
            {
                backOfficeOnly = false;
            }
        }

        // Avalonia Android has no Window platform — POS/BackOffice still require Window.
        if (OperatingSystem.IsAndroid())
        {
            // POS/BackOffice still use Window — host navigation comes next.
            System.Diagnostics.Debug.WriteLine("ShowSessionAsync skipped on Android (no Window platform).");
            return;
        }

        IsVisible = false;
        try
        {
            if (backOfficeOnly)
            {
                var office = new BackOfficeWindow(await SessionLabelAsync(userName));
                office.UseBackOfficeOnly();
                office.LoggedOut += (_, _) =>
                {
                    _viewModel.ClearSession();
                    IsVisible = true;
                    PinEntryFocus();
                };
                office.Show();
                return;
            }

            var pos = new PosWindow(userName);
            pos.LoggedOut += (_, _) =>
            {
                _viewModel.ClearSession();
                IsVisible = true;
                PinEntryFocus();
            };
            pos.Show();
        }
        catch (Exception exception)
        {
            App.WriteCrash("ShowSessionAsync", exception);
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
        var entry = this.FindControl<TextBox>("PinEntry");
        entry?.Focus();
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
            _ = _viewModel.SubmitAsync();
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

    private async void OnSubmitClick(object? sender, RoutedEventArgs e) => await _viewModel.SubmitAsync();

    private async void OnResetPasswordClick(object? sender, RoutedEventArgs e) => await _viewModel.BeginPasswordResetAsync();

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
