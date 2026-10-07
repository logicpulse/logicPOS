namespace LogicPOS.Domain.Entities.Articles.Articles.Common;

public static class SdrConstants
{
    public const string SdrArticleCode = "SDRVDEP";
    public const decimal SdrArticlePrice = 0.10M;
    public const string SdrArticleDesignation = "Valor de Depósitivo Volta";

    public const string SdrFamilyCode = "SDRV";
    public const string SdrFamilyDesignation = "SDR Volta";

    public const string SdrSubfamilyCode = "SDRV";
    public const string SdrSubfamilyDesignation = "Depósito Volta";

    public static bool IsDepositArticleCode(string? code) =>
        string.Equals(code, SdrArticleCode, StringComparison.OrdinalIgnoreCase);
}