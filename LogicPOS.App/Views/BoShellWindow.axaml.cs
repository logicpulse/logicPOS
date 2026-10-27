using Avalonia.Controls;

namespace LogicPOS.App.Views;

/// <summary>
/// Desktop-only shell: Avalonia Android cannot create Window, so BO UI lives in BackOfficeWindow (UserControl).
/// </summary>
public partial class BoShellWindow : Window
{
    private readonly BackOfficeWindow _office;

    public BoShellWindow(BackOfficeWindow office)
    {
        InitializeComponent();
        _office = office;
        Host.Children.Add(office);
        Opened += async (_, _) => await office.ActivateHostedAsync();
        office.LoggedOut += (_, _) =>
        {
            Closing -= OnClosing;
            Close();
        };
        Closing += OnClosing;
    }

    public BackOfficeWindow Office => _office;

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_office.TryRequestClose() == false)
        {
            e.Cancel = true;
        }
    }
}
