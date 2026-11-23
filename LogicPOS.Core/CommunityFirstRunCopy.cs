namespace LogicPOS.Core;

/// <summary>
/// First-run Community copy. The public edition has no fiscal plugin; the LogicPulse edition does.
/// Business and Cloud cards stay the certified choices already written in the window.
/// </summary>
public static class CommunityFirstRunCopy
{
    public const string PublicIntro =
        "O LogicPOS é um ponto de venda e back-office moderno e flexível. A Community inclui faturação local sem certificação AT integrada; Business e Cloud acrescentam certificação LogicPulse.";

    public const string LogicPulseIntro =
        "O LogicPOS é um ponto de venda e back-office moderno e flexível. A Community inclui faturação local com certificação AT; Business e Cloud acrescentam certificação LogicPulse.";

    public const string PublicFeature =
        "✓  Faturação e documentos PDF no posto (sem certificação AT integrada)";

    public const string LogicPulseFeature =
        "✓  Faturação e documentos PDF no posto (com certificação AT)";

    public static string Intro(bool logicPulseEdition)
        => logicPulseEdition ? LogicPulseIntro : PublicIntro;

    public static string Feature(bool logicPulseEdition)
        => logicPulseEdition ? LogicPulseFeature : PublicFeature;
}
