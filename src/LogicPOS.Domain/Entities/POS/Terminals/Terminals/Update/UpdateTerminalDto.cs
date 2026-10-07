namespace LogicPOS.Domain.Entities.Dtos;

public record UpdateTerminalDto(uint Order,
                                string Code,
                                string Designation,
                                uint TimerInterval,
                                bool IsDefault,
                                Guid? PrinterId,
                                Guid? WeighingMachineId,
                                Guid? PlaceId,
                                Guid? ThermalPrinterId,
                                Guid? BarcodeReaderId,
                                Guid? PoleDisplayId,
                                Guid? CardReaderId,
                                string? Notes);