namespace LogicPOS.Core.BackOffice;

public static class DocumentDefaultStore
{
    private static string FilePath => Path.Combine(AppContext.BaseDirectory, "default-document.txt");

    public static string? Read()
    {
        try
        {
            if (File.Exists(FilePath) == false)
            {
                return null;
            }

            var text = File.ReadAllText(FilePath).Trim();
            return string.IsNullOrWhiteSpace(text) ? null : text;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static void Write(string? acronym)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(acronym))
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }

                return;
            }

            File.WriteAllText(FilePath, acronym.Trim());
        }
        catch (Exception)
        {
            // The default document is a local preference. A write failure must not block the document type save.
        }
    }
}
