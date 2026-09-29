using Serilog;
using System;
using System.IO;
using System.Windows;

namespace LogicPOS.UI.Application.Services
{
    public static class MigratorService
    {
        private const string MigratorExecutablePath = "migrator\\lpmigrator.exe";
        public static bool MigratorExists => File.Exists(MigratorExecutablePath);

        public static bool HasOldPosSqliteDatabase() => File.Exists("logicposdb.db");
        
        private static void HideOldPosSQliteDatabase()
        {
            Log.Information("Hiding old pos sqlite database");
            if(!File.Exists("logicposdb.db"))
            {
                Log.Warning("Could not find old pos sqlite database");
                return;
            }

            try
            {
                File.Move("logicposdb.db", "logicposdb.db.old");
            }
            catch (Exception ex) {
                Log.Error(ex, "Could not rename the file logicposdb.bd to hide it");
                MessageBox.Show("Ocorreu um erro ao renomear o ficheiro logicposdb.bd: \n" + ex.Message);
            }
        }

        public static void LaunchMigrator()
        {
            var response = MessageBox.Show("Deseja migrar os dados da versão anterior?", "LogicPOS", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (response != MessageBoxResult.Yes)
            {
                Log.Information("User declined to run the migrator.");
                return;
            }

            if (MigratorExists == false)
            {
                Log.Warning("Migrator executable not found at path: {Path}", MigratorExecutablePath);
                MessageBox.Show("Aplicação de migração não encontrada.", "LogicPOS", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Log.Information("Launching migrator from path: {Path}", MigratorExecutablePath);
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

                HideOldPosSQliteDatabase();

            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("Erro ao executar a aplicação de migração", "LogicPOS", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                Serilog.Log.Error(ex, "Failed to launch migrator: {Message}", ex.Message);
            }
        }

    }
}
