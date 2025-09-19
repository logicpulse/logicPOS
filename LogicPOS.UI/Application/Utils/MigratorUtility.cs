using System.IO;
using System.Windows;

namespace LogicPOS.UI.Application.Utils
{
    public static class MigratorUtility
    {
        private const string MigratorExecutablePath = "migrator\\win-x64\\lpmigrator.exe";
        public static bool MigratorExists => File.Exists(MigratorExecutablePath);

        public static void LaunchMigrator()
        {
            var response = MessageBox.Show("Deseja migrar os dados da versão anterior?", "LogicPOS", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (response != MessageBoxResult.Yes)
            {
                return;
            }

            if (MigratorExists == false)
            {
                Serilog.Log.Warning("Migrator executable not found at path: {Path}", MigratorExecutablePath);
                MessageBox.Show("Aplicação de migração não encontrada.", "LogicPOS", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = MigratorExecutablePath,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal,
                };

                using (var process = System.Diagnostics.Process.Start(processInfo))
                {
                    process.WaitForExit();
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("Erro ao executar a aplicação de migração", "LogicPOS", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                Serilog.Log.Error(ex, "Failed to launch migrator: {Message}", ex.Message);
            }
        }

    }
}
