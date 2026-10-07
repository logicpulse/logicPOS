namespace LogicPOS.Domain.Entities.Dtos;

public record CreateTerminalDto(string Designation,
                                string HardwareId,
                                uint TimerInterval,
                                Guid? PlaceId,
                                Guid? PrinterId,
                                Guid? ThermalPrinterId,
                                Guid? PoleDisplayId,
                                Guid? WeighingMachineId,
                                Guid? BarcodeReaderId,
                                Guid? CardReaderId,
                                string? Notes,
                                bool Imported = false);