using LogicPOS.Core.BackOffice;
using Microsoft.Data.Sqlite;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class DatabaseRestoreSafetyTests
{
    [Fact]
    public void Cancel_and_wrong_confirm_text_do_not_match()
    {
        Assert.False(DatabaseRestoreSafety.IsConfirmWord(null));
        Assert.False(DatabaseRestoreSafety.IsConfirmWord(""));
        Assert.False(DatabaseRestoreSafety.IsConfirmWord("restaurar"));
        Assert.False(DatabaseRestoreSafety.IsConfirmWord("RESTAURA"));
        Assert.True(DatabaseRestoreSafety.IsConfirmWord("RESTAURAR"));
    }

    [Fact]
    public void Refuses_candidate_equal_to_live_database()
    {
        var root = Path.Combine(Path.GetTempPath(), "logicpos-restore-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var live = Path.Combine(root, "logicpos.db");
            File.WriteAllText(live, "live");
            var same = Path.Combine(root, ".", "logicpos.db");
            var error = DatabaseRestoreSafety.ValidateCandidateFile(live, same);
            Assert.NotNull(error);
            Assert.Contains("ativa", error, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Accepts_different_backup_file()
    {
        var root = Path.Combine(Path.GetTempPath(), "logicpos-restore-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var live = Path.Combine(root, "logicpos.db");
            var bak = Path.Combine(root, "backup.db");
            File.WriteAllText(live, "live");
            File.WriteAllText(bak, "bak");
            Assert.Null(DatabaseRestoreSafety.ValidateCandidateFile(live, bak));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Creates_timestamped_bak_beside_live_file()
    {
        var root = Path.Combine(Path.GetTempPath(), "logicpos-restore-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var live = Path.Combine(root, "logicpos.db");
            CreateSqliteWithPayload(live, "live-bytes");
            var stamp = new DateTime(2026, 11, 23, 13, 5, 9);
            var expected = Path.Combine(root, "logicpos.db.20261123-130509.bak");
            Assert.Equal(
                DatabaseRestoreSafety.NormalizePath(expected),
                DatabaseRestoreSafety.BuildBakPath(live, stamp));

            var error = DatabaseRestoreSafety.TryCreateLiveBak(live, stamp);
            Assert.Null(error);
            Assert.True(File.Exists(expected));
            Assert.Equal("live-bytes", ReadSqlitePayload(expected));
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Bak_copy_failure_blocks_restore()
    {
        var root = Path.Combine(Path.GetTempPath(), "logicpos-restore-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var live = Path.Combine(root, "logicpos.db");
            CreateSqliteWithPayload(live, "live");
            var error = DatabaseRestoreSafety.TryCreateLiveBak(
                live,
                DateTime.UtcNow,
                backup: (_, _) => throw new IOException("disk full"));
            Assert.NotNull(error);
            Assert.Contains(".bak", error, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("disk full", error, StringComparison.Ordinal);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Bak_includes_uncheckpointed_wal_pages()
    {
        var root = Path.Combine(Path.GetTempPath(), "logicpos-restore-wal-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        var live = Path.Combine(root, "logicpos.db");
        var stamp = new DateTime(2026, 11, 23, 14, 0, 0);
        var bakPath = DatabaseRestoreSafety.BuildBakPath(live, stamp);
        const string payload = "only-in-wal";

        try
        {
            // Keep the writer open so the checkpoint cannot flush the WAL into the main file.
            using var writer = new SqliteConnection(
                new SqliteConnectionStringBuilder { DataSource = live, Pooling = false }.ToString());
            writer.Open();
            Execute(writer, "PRAGMA journal_mode=WAL;");
            Execute(writer, "PRAGMA wal_autocheckpoint=0;");
            Execute(writer, "CREATE TABLE restore_probe(v TEXT NOT NULL);");
            Execute(writer, "INSERT INTO restore_probe(v) VALUES ('" + payload + "');");

            var walPath = live + "-wal";
            Assert.True(File.Exists(walPath), "Expected a -wal file after WAL inserts.");
            Assert.True(new FileInfo(walPath).Length > 0, "Expected -wal to hold uncheckpointed pages.");

            var error = DatabaseRestoreSafety.TryCreateLiveBak(live, stamp);
            Assert.Null(error);
            Assert.True(File.Exists(bakPath));
            Assert.Equal(payload, ReadSqlitePayload(bakPath));
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Prompt_label_includes_full_path()
    {
        var path = Path.Combine(Path.GetTempPath(), "copy.db");
        var label = DatabaseRestoreSafety.FileRestorePromptLabel(path);
        Assert.Contains(path, label, StringComparison.Ordinal);
        Assert.Contains("RESTAURAR", label, StringComparison.Ordinal);
    }

    private static void CreateSqliteWithPayload(string path, string payload)
    {
        using var connection = new SqliteConnection(
            new SqliteConnectionStringBuilder { DataSource = path, Pooling = false }.ToString());
        connection.Open();
        Execute(connection, "CREATE TABLE restore_probe(v TEXT NOT NULL);");
        Execute(connection, "INSERT INTO restore_probe(v) VALUES ('" + payload.Replace("'", "''") + "');");
        connection.Close();
        SqliteConnection.ClearAllPools();
    }

    private static string ReadSqlitePayload(string path)
    {
        using var connection = new SqliteConnection(
            new SqliteConnectionStringBuilder
            {
                DataSource = path,
                Mode = SqliteOpenMode.ReadOnly,
                Pooling = false
            }.ToString());
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT v FROM restore_probe LIMIT 1;";
        return Convert.ToString(command.ExecuteScalar()) ?? string.Empty;
    }

    private static void Execute(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}
