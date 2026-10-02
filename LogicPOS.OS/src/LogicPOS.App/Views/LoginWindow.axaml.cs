using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LogicPOS.Core;
using LogicPOS.Core.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow()
    {
        InitializeComponent();
        TouchFields.Attach(this);

        var loginService = AppComposition.Services?.GetService<ILoginService>();
        _viewModel = new LoginViewModel(loginService, AppComposition.StartupError);
        _viewModel.QuitConfirmed += (_, _) => Close();
        _viewModel.TerminalChoiceRequested += (_, _) => ShowTerminalChoice();
        _viewModel.SessionStarted += (_, userName) => _ = ShowSessionAsync(userName);
        DataContext = _viewModel;

        AddHandler(KeyDownEvent, OnWindowKeyDown, RoutingStrategies.Tunnel);
        Opened += async (_, _) =>
        {
            ApplyScreenLayout();
            await _viewModel.LoadAsync();
            PinEntryFocus();
        };
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

    private void ApplyScreenLayout()
    {
        var screen = Screens?.ScreenFromVisual(this) ?? Screens?.Primary;
        var height = (int)Math.Round(screen?.Bounds.Height ?? 1080d);
        var rows = (height - (120 * 2) - (60 * 2)) / 102;
        _viewModel.SetPageSize(rows < 1 ? 1 : rows);
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

        Hide();
        try
        {
            if (backOfficeOnly)
            {
                var office = new BackOfficeWindow(await SessionLabelAsync(userName));
                office.UseBackOfficeOnly();
                office.LoggedOut += (_, _) =>
                {
                    _viewModel.ClearSession();
                    Show();
                    PinEntryFocus();
                };
                office.Show();
                return;
            }

            var pos = new PosWindow(userName);
            pos.LoggedOut += (_, _) =>
            {
                _viewModel.ClearSession();
                Show();
                PinEntryFocus();
            };
            pos.Show();
        }
        catch (Exception exception)
        {
            App.WriteCrash("ShowSessionAsync", exception);
            Show();
            PinEntryFocus();
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
            box.Show();
        }
    }

    private static async Task<string> SessionLabelAsync(string userName)
    {
        var login = AppComposition.Services?.GetService<ILoginService>();
        if (login is null)
        {
            return userName;
        }

        try
        {
            var terminalName = await login.GetFirstTerminalNameAsync();
            return string.IsNullOrWhiteSpace(terminalName) ? userName : $"{terminalName} : {userName}";
        }
        catch (Exception)
        {
            return userName;
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

    private void OnMinimizeClick(object? sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
}
