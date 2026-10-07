namespace LogicPOS.Domain.Entities.Finance.Documents.Documents.Details.Sdr;

public sealed record SdrDepositArticleSnapshot(
    Guid Id,
    decimal UnitPrice,
    Guid VatRateId,
    Guid? VatExemptionId);
