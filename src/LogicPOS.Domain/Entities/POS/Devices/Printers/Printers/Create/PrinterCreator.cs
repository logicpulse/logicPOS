using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class PrinterCreator : EntityCreator<Printer>
{
    private readonly CreatePrinterDto _dto;
    private readonly IPrinterRepository _repository;
    private readonly IPrinterTypeRepository _printerTypeRepository;
    public PrinterCreator(CreatePrinterDto dto,
                          IPrinterRepository repository,
                          IPrinterTypeRepository printerTypeRepository) : base(new())
    {
        _dto = dto;
        _repository = repository;
        _printerTypeRepository = printerTypeRepository;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Order = await _repository.GetNextOrderAsync(ct);
        _entity.Designation = _dto.Designation;
        _entity.Code = await _repository.GetNextCodeAsync(ct);
        _entity.TypeId = _dto.PrinterTypeId;
        _entity.Notes = _dto.Notes;
        _entity.NetworkName = _dto.NetworkName;
        _entity.ThermalEncoding = _dto.ThermalEncoding;
        _entity.ThermalCutCommand = _dto.ThermalCutCommand;
        _entity.ThermalMaxCharsPerLineSmall = _dto.ThermalMaxCharsPerLineSmall;
        _entity.ThermalMaxCharsPerLineNormal = _dto.ThermalMaxCharsPerLineNormal;
        _entity.ThermalMaxCharsPerLineNormalBold = _dto.ThermalMaxCharsPerLineNormalBold;
        _entity.ThermalOpenDrawerValueT1 = _dto.ThermalOpenDrawerValueT1;
        _entity.ThermalOpenDrawerValueT2 = _dto.ThermalOpenDrawerValueT2;
        _entity.ShowInDialog = _dto.ShowInDialog;
        _entity.ThermalOpenDrawerValueM = _dto.ThermalOpenDrawerValueM;
        _entity.ThermalPrintLogo = _dto.ThermalPrintLogo;
        _entity.ThermalImageCompanyLogo = _dto.ThermalImageCompanyLogo;
        _entity.IsDeleted = _dto.IsDeleted ?? false;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        return await CheckForDesignationConflictAsync(_repository, _dto.Designation, ct);
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (!await _printerTypeRepository.ExistsAsync(_dto.PrinterTypeId, ct))
        {
            return Result.NotFound<PrinterType>(_dto.PrinterTypeId.ToString());
        }

        return Result.Success();
    }
}