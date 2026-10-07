namespace LogicPOS.Domain.Entities.Dtos;

public record CreateDocumentSeriesDto(int NextNumber,
                                      int NumberRangeBegin,
                                      int NumberRangeEnd,
                                      string Designation,
                                      string Acronym,
                                      Guid DocumentTypeId,
                                      Guid FiscalYearId,
                                      string? AtValidationCode,
                                      string? Notes,
                                      bool Imported = false);