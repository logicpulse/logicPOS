namespace LogicPOS.Domain.Entities.Dtos;

public record CreateInputReaderDto(string Designation,
                                   string ReaderSizes,
                                   string? Notes,
                                   bool? IsDeleted);