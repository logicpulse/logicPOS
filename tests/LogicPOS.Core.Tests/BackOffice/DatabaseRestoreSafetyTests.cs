using LogicPOS.Core.BackOffice;

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
            File.WriteAllText(live, "live-bytes");
            var stamp = new DateTime(2026, 11, 23, 13, 5, 9);
            var expected = Path.Combine(root, "logicpos.db.20261123-130509.bak");
            Assert.Equal(
                DatabaseRestoreSafety.NormalizePath(expected),
                DatabaseRestoreSafety.BuildBakPath(live, stamp));

            var error = DatabaseRestoreSafety.TryCreateLiveBak(live, stamp);
            Assert.Null(error);
            Assert.True(File.Exists(expected));
            Assert.Equal("live-bytes", File.ReadAllText(expected));
        }
        finally
        {
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
            File.WriteAllText(live, "live");
            var error = DatabaseRestoreSafety.TryCreateLiveBak(
                live,
                DateTime.UtcNow,
                copy: (_, _) => throw new IOException("disk full"));
            Assert.NotNull(error);
            Assert.Contains(".bak", error, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("disk full", error, StringComparison.Ordinal);
        }
        finally
        {
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
}
