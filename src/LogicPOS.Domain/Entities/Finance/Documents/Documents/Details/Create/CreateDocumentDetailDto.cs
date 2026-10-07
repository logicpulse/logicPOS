namespace LogicPOS.Domain.Entities.Dtos;

public record CreateDocumentDetailDto(Guid ArticleId,
                                      decimal Quantity,
                                      decimal? Price,
                                      Guid? VatExemptionId,
                                      Guid? VatRateId,
                                      decimal Discount,
                                      Enums.PriceType? PriceType,
                                      string? Notes,
                                      string Country2Code,
                                      string? SerialNumber,
                                      bool UseAgtRounding = false);