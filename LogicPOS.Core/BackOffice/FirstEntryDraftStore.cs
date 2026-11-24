using System.Text.Json;

namespace LogicPOS.Core.BackOffice;

/// <summary>
/// File-backed draft the local installer leaves for the first entry: <c>first-entry-draft.json</c>
/// next to <c>appsettings.json</c>. A draft is a preselection, never a confirmation.
/// </summary>
public static class FirstEntryDraftStore
{
    public const string FileName = "first-entry-draft.json";

    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static string PathFor(string baseDirectory) => Path.Combine(baseDirectory, FileName);

    /// <summary>The draft, or null when there is none or the file is not a valid draft.</summary>
    public static FirstEntryDraft? Read(string baseDirectory)
    {
        var path = PathFor(baseDirectory);
        if (File.Exists(path) == false)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<FirstEntryDraft>(File.ReadAllText(path));
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Writes the draft unconditionally. For the installer that has just created the database;
    /// a re-run against a database that may be confirmed should use
    /// <see cref="IFirstEntryService.TrySaveDraftAsync"/> instead.
    /// </summary>
    public static void Write(string baseDirectory, FirstEntryDraft draft)
    {
        var path = PathFor(baseDirectory);
        var temporary = path + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(draft, Options));
        File.Move(temporary, path, overwrite: true);
    }
}
