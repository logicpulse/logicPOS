namespace LogicPOS.Domain.Entities.Dtos;

public record CreateWeighingMachineDto(string Designation,
                                       string PortName,
                                       uint BaudRate,
                                       string Parity,
                                       string StopBits,
                                       uint DataBits,
                                       string? Notes,
                                       bool? IsDeleted);