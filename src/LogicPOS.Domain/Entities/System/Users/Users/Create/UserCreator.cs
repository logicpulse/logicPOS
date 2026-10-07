using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;
using LogicPOS.Domain.Services;

namespace LogicPOS.Domain.Entities.Utilities;

internal class UserCreator : EntityCreator<User>
{
    private readonly CreateUserDto _dto;
    private readonly IUserRepository _userRepository;
    private readonly UserReferences _references;
    private readonly IPasswordHasher _passwordHasher;

    public UserCreator(CreateUserDto dto,
                       IUserRepository userRepository,
                       IPasswordHasher passwordHasher,
                       UserReferences references) : base(new())
    {
        _userRepository = userRepository;
        _references = references;
        _passwordHasher = passwordHasher;
        _dto = dto;
    }

    protected override async Task AssignDataAsync(CancellationToken ct = default)
    {
        _entity.Order = _dto.Imported ? 0 : await _userRepository.GetNextOrderAsync(ct);
        _entity.Code = _dto.Imported ? "?" : await _userRepository.GetNextCodeAsync(ct);
        _entity.ProfileId = _dto.ProfileId;
        _entity.Name = _dto.Name;
        _entity.Login = _dto.Login;
        _entity.AccessPin = _passwordHasher.HashPassword(User.DefaultAccessPin);
        _entity.PasswordReset = true;
        _entity.Need2FA = _dto.Need2FA;
        if (string.IsNullOrWhiteSpace(_dto.PortalPassword) == false)
            _entity.Password = _passwordHasher.HashPassword(_dto.PortalPassword.Trim());
        _entity.CommissionGroupId = _dto.CommissionGroupId;
        _entity.Residence = _dto.Residence;
        _entity.Locality = _dto.Locality;
        _entity.ZipCpde = _dto.ZipCode;
        _entity.City = _dto.City;
        _entity.DateOfContract = _dto.DateOfContract;
        _entity.Phone = _dto.Phone;
        _entity.MobilePhone = _dto.MobilePhone;
        _entity.Email = _dto.Email;
        _entity.FiscalNumber = _dto.FiscalNumber;
        _entity.Language = _dto.Language;
        _entity.AssignedSeating = _dto.AssignedSeating;
        _entity.BaseConsumption = _dto.BaseConsumption;
        _entity.BaseOffers = _dto.BaseOffers;
        _entity.PVPOffers = _dto.PVPOffers;
        _entity.Remarks = _dto.Remarks;
        _entity.ButtonImage = _dto.ButtonImage;
        _entity.Notes = _dto.Notes;
        _entity.IsDeleted = _dto.IsDeleted ?? false;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (_dto.Imported)
        {
            return Result.Success();
        }

        if (!string.IsNullOrWhiteSpace(_dto.Login) && await _userRepository.LoginExistsAsync(_dto.Login!, ct))
        {
            return Result.Conflict("Login já existe.");
        }

        var nameExists = await _userRepository.NameExistsAsync(_entity.Name, ct);

        if (nameExists)
        {
            return Result.Conflict("Nome já existe.");
        }

        if (!string.IsNullOrWhiteSpace(_dto.Email) && await _userRepository.EmailExistsAsync(_entity.Email!, ct))
        {
            return Result.Conflict("Email já existe.");
        }

        return Result.Success();
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        var profileExists = await _references.UserProfileExistsAsync(_dto.ProfileId, ct);

        if (profileExists == false)
        {
            return Result.NotFound(nameof(UserProfile), _dto.ProfileId.ToString());
        }

        if (_dto.CommissionGroupId is not null)
        {
            var commissionGroupExists = await _references.CommissionGroupExistsAsync(_dto.CommissionGroupId.Value, ct);

            if (commissionGroupExists == false)
            {
                return Result.NotFound(nameof(CommissionGroup), _dto.CommissionGroupId.ToString());
            }
        }

        return Result.Success();
    }
}