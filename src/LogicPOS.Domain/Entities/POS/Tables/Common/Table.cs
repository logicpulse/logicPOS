using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Enums;
using LogicPOS.Domain.Errors;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("pos_configurationplacetable")]
public class Table : Entity.WithCode.AndOrder.AndDesignation
{
    public Place? Place { get; set; }
    public Guid PlaceId { get; set; }

    public string? ButtonImage { get; set; }
    [Column(TypeName = "decimal(18,6)")]
    public decimal Discount { get; set; }
    public TableStatus Status { get; set; }
    public DateTime? OpennedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public Result Open()
    {
        if (Status != TableStatus.Free)
        {
            return Result.Failure(Error.InvalidOperation("Table is not free"));
        }

        Status = TableStatus.Open;
        OpennedAt = DateTime.Now;
        return Result.Success();
    }

    public Result Free()
    {
        if (Status != TableStatus.Reserved)
        {
            return Result.Failure(Error.InvalidOperation("Table is not reserved"));
        }

        Status = TableStatus.Free;
        return Result.Success();
    }
    
    public Result Close()
    {
        if (Status == TableStatus.Free)
        {
            return Result.Failure(Error.InvalidOperation("Table is already closed"));
        }

        Status = TableStatus.Free;
        ClosedAt = DateTime.Now;
        return Result.Success();
    }

    public Result Reserve()
    {
        if (Status != TableStatus.Free)
        {
            return Result.Failure(Error.InvalidOperation("Table is not free"));
        }

        Status = TableStatus.Reserved;
        return Result.Success();
    }


    public Task<Result> UpdateAsync(ITableRepository repository,
                                    IPlaceRepository placeRepository,
                                    UpdateTableDto dto,
                                    CancellationToken cancellationToken = default)
    {
        var updater = new TableUpdater(this, dto, repository, placeRepository);
        return updater.UpdateAsync(cancellationToken);
    }


    public static Task<Result<Table>> CreateAsync(ITableRepository repository,
                                                  IPlaceRepository placeRepository,
                                                  CreateTableDto dto,
                                                  CancellationToken cancellationToken = default)
    {
        var creator = new TableCreator(dto, repository, placeRepository);
        return creator.CreateAsync(cancellationToken);
    }

}
