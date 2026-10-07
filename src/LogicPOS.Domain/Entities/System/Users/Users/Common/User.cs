using LogicPOS.Domain.Entities.Common.Entity;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;
using LogicPOS.Domain.Services;

namespace LogicPOS.Domain.Entities;

//[Table("sys_userdetail")]
public class User : Entity.WithCode.AndOrder.AndName
{
    public const string DefaultAccessPin = "0000";
    public UserProfile? Profile { get; set; }
    public Guid ProfileId { get; set; }

    public CommissionGroup? CommissionGroup { get; set; }
    public Guid? CommissionGroupId { get; set; }

    public string? Residence { get; set; }
    public string? Locality { get; set; }
    public string? ZipCpde { get; set; }
    public string? City { get; set; }
    public string? DateOfContract { get; set; }
    public string? Phone { get; set; }
    public string? MobilePhone { get; set; }
    public string? Email { get; set; }
    public string? FiscalNumber { get; set; }
    public string? Language { get; set; }
    public string? AssignedSeating { get; set; }
    public string AccessPin { get; set; } = null!;
    public string? AccessCardNumber { get; set; }
    public string? Login { get; set; }
    public string? Password { get; set; }
    public bool Need2FA { get; set; }
    public bool PasswordReset { get; set; }
    public DateTime PasswordResetDate { get; set; }
    public string? BaseConsumption { get; set; }
    public string? BaseOffers { get; set; }
    public string? PVPOffers { get; set; }
    public string? Remarks { get; set; }
    public string? ButtonImage { get; set; }

    public Task<Result> UpdateAsync(IUserRepository repository,
                                    UpdateUserDto dto,
                                    IUserProfileRepository profileRepository,
                                    ICommissionGroupRepository commissionGroupRepository,
                                    CancellationToken cancellationToken = default)
    {
        var updater = new UserUpdater(this,
                                            dto,
                                            repository,
                                            profileRepository,
                                            commissionGroupRepository);

        return updater.UpdateAsync(cancellationToken);
    }


    public static async Task<Result<User>> CreateAsync(CreateUserDto dto,
                                                             IUserRepository detailRepository,
                                                             IPasswordHasher passwordHasher,
                                                             UserReferences references,
                                                             CancellationToken cancellationToken = default)
    {
        var creator = new UserCreator(dto,
                                      detailRepository,
                                      passwordHasher,
                                      references);

        return await creator.CreateAsync(cancellationToken);
    }

}
