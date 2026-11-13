namespace LogicPOS.Persistence.Services.DemoData;

/// <summary>Preference tokens mirroring qtrack <c>demoDataEnabled</c> / <c>demoDataLastGeneratedDate</c>.</summary>
public static class DemoDataParameterTokens
{
    public const string Enabled = "DEMO_DATA_ENABLED";
    public const string LastGeneratedDate = "DEMO_DATA_LAST_GENERATED_DATE";

    /// <summary>Marker stored in <c>Document.Notes</c> so demo rows can be replaced safely.</summary>
    public const string DocumentNotesMarker = "[DEMO_DATA]";
}
