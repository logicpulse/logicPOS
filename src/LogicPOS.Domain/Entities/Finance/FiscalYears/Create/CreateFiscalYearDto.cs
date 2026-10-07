namespace LogicPOS.Domain.Entities.Dtos;

public record CreateFiscalYearDto(string Designation,
                                  string Acronym,
                                  int Year,
                                  bool SeriesForEachTerminal,
                                  string? Notes);
