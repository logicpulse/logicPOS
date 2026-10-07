using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("sys_usercommissiongroup")]
public class CommissionGroup : Entity.WithCode.AndOrder.AndDesignation
{
    [Column(TypeName = "decimal(18,6)")]
    public decimal Commission { get; set; }

    public Task<Result> UpdateAsync(UpdateUserCommissionGroupDto dto,
                                    ICommissionGroupRepository repository,
                                    CancellationToken cancellationToken = default)
    {
        var updater = new CommissionGroupUpdater(this, dto, repository);
        return updater.UpdateAsync(cancellationToken);
    }   

       public static Task<Result<CommissionGroup>> CreateAsync(ICommissionGroupRepository repository,
                                                               CreateUserCommissionGroupDto dto,
                                                               CancellationToken cancellationToken = default)
    {
        var creator = new CommissionGroupCreator(repository, dto);
        return creator.CreateAsync(cancellationToken);
    }

}
