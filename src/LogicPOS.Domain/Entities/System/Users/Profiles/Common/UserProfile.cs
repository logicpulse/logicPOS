using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities;

//[Table("sys_userprofile")]
public class UserProfile : Entity.WithCode.AndOrder.AndDesignation
{
    public string? AccessPassword { get; set; }

    public Task<Result> UpdateAsync(IUserProfileRepository repository,
                                    UpdateUserProfileDto dto,
                                    CancellationToken cancellationToken = default)
    {
        var updater = new UserProfileUpdater(this, dto, repository);
        return updater.UpdateAsync(cancellationToken);
    }

    public static Task<Result<UserProfile>> CreateAsync(IUserProfileRepository repository,
                                                        CreateUserProfileDto dto,
                                                        CancellationToken cancellationToken = default)
    {
        var creator = new UserProfileCreator(dto, repository);
        return creator.CreateAsync(cancellationToken);
    }
}
