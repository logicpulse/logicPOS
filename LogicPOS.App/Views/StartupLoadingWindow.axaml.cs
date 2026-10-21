using Avalonia.Controls;
using LogicPOS.Core;

namespace LogicPOS.App.Views;

public partial class StartupLoadingWindow : Window
{
    public StartupLoadingWindow()
    {
        InitializeComponent();
        ApplyMessage(DatabaseStartup.ShowFirstTimeDatabaseMessage() == false);
    }

    /// <param name="databaseReady">
    /// True = normal loading text. False = first-time local SQLite create.
    /// Cloud/online never uses the create message.
    /// </param>
    public void ApplyMessage(bool databaseReady)
    {
        StatusText.Text = databaseReady
            ? PreferenceLabels.Text("global_load", "A carregar POS...")
            : PreferenceLabels.Text("global_load_first_time", "A criar base de dados...");
    }
}
