using LogicPOS.Domain.Entities.Common.Utilities;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;

namespace LogicPOS.Domain.Entities.Utilities;

internal class UserUpdater : EntityUpdater<User>
{
    private readonly UpdateUserDto _dto;
    private readonly IUserProfileRepository _profileRepository;
    private readonly ICommissionGroupRepository _commissionGroupRepository;
    private readonly IUserRepository _userRepository;

    public UserUpdater(User entity,
        UpdateUserDto dto,
        IUserRepository repository,
        IUserProfileRepository profileRepository,
        ICommissionGroupRepository commissionGroupRepository) : base(entity,
        repository)
    {
        _dto = dto;
        _profileRepository = profileRepository;
        _commissionGroupRepository = commissionGroupRepository;
        _userRepository = repository;
    }

    protected override async Task<Result> CheckForConflictsAsync(CancellationToken ct = default)
    {
        if (_dto.Name != _entity.Name && await _userRepository.NameExistsAsync(_dto.Name, ct))
        {
            return Result.Conflict("Nome já existe.");
        }
        
        if (_dto.Login != _entity.Login && await _userRepository.LoginExistsAsync(_dto.Login, ct))
        {
            return Result.Conflict("Login já existe.");
        }

        if (!string.IsNullOrWhiteSpace(_dto.Email) && _dto.Email != _entity.Email && await _userRepository.EmailExistsAsync(_dto.Email!, ct))
        {
            return Result.Conflict("Email já existe.");
        }

        return await CheckForCodeConflictAsync(_dto.Code, ct);
    }

    protected override async Task<Result> CheckForExistenceAsync(CancellationToken ct = default)
    {
        if (_dto.ProfileId != _entity.ProfileId && await _profileRepository.ExistsAsync(_dto.ProfileId, ct) == false)
        {
            return Result.NotFound(nameof(UserProfile), _dto.ProfileId.ToString());
        }

        if (_dto.CommissionGroupId != null && _dto.CommissionGroupId != _entity.CommissionGroupId &&
            await _commissionGroupRepository.ExistsAsync(_dto.CommissionGroupId.Value, ct) == false)
        {
            return Result.NotFound(nameof(CommissionGroup), _dto.CommissionGroupId.ToString());
        }
        return Result.Success();
    }

    protected override void SetNewData()
    {
        _entity.Order = _dto.Order;
        _entity.Code = _dto.Code;
        _entity.Name = _dto.Name;
        _entity.Login = _dto.Login;
        _entity.ProfileId = _dto.ProfileId;
        _entity.CommissionGroupId = _dto.CommissionGroupId;
        _entity.Residence = _dto.Residence;
        _entity.Locality = _dto.Locality;
        _entity.ZipCpde = _dto.ZipCpde;
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
        _entity.IsDeleted = _dto.IsDeleted;
    }
}