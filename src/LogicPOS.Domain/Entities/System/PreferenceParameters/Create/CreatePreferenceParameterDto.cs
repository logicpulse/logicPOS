namespace LogicPOS.Domain.Entities.Dtos;

public record CreatePreferenceParameterDto(string Token,
                                           string? Value,
                                           string? ValueTip,
                                           bool Required,
                                           string RegEx,
                                           string ResourceString,
                                           string? ResourceStringInfo,
                                           int FormType,
                                           int FormPageNo,
                                           int InputType,
                                           string? Notes,
                                           bool? IsDeleted);