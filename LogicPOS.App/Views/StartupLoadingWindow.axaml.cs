using Avalonia.Controls;
using LogicPOS.Core;

namespace LogicPOS.App.Views;

public partial class StartupLoadingWindow : Window
{
    public StartupLoadingWindow()
    {
        InitializeComponent();
        ApplyMessage(DatabaseStartup.SqliteDatabaseExists());
    }

    public void ApplyMessage(bool databaseExists)
    {
        StatusText.Text = databaseExists
            ? PreferenceLabels.Text("global_load", "A carregar POS...")
            : PreferenceLabels.Text("global_load_first_time", "A criar base de dados...");
    }
}
