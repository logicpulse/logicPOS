using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.Api.Features.Users.AddUser;
using LogicPOS.Api.Features.Users.Profiles.GetAllUserProfiles;
using LogicPOS.Api.Features.Users.UpdateUser;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UserDetailsModal : EntityModal<UserDetail>
    {
        public UserDetailsModal(EntityModalMode modalMode, UserDetail entity = null) : base(modalMode, entity)
        {

        }

        protected override void ShowEntityData()
        {
            _txtName.Text = _entity.Name;
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtContractDate.Text = _entity.DateOfContract;
            _txtAddress.Text = _entity.Residence;
            _txtLocality.Text = _entity.Locality;
            _txtCity.Text = _entity.City;
            _txtPostalCode.Text = _entity.ZipCpde;
            _txtPhone.Text = _entity.Phone;
            _txtMobile.Text = _entity.MobilePhone;
            _txtEmail.Text = _entity.Email;
            _txtFiscalNumber.Text = _entity.FiscalNumber;
            _txtLanguage.Text = _entity.Language;
            _txtAssignedSeating.Text = _entity.AssignedSeating;
            _txtAccessCardNumber.Text = _entity.AccessCardNumber;
            _txtConsumptionBase.Text = _entity.BaseConsumption;
            _txtBaseOffers.Text = _entity.BaseOffers;
            _txtPVPOffers.Text = _entity.PVPOffers;
            _checkDisabled.Active = _entity.IsDeleted;     
            _txtNotes.Value.Text = _entity.Notes;
        }

        private UpdateUserCommand CreateUpdateCommand()
        {
            return new UpdateUserCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewName = _txtName.Text,
                NewProfileId = _comboProfiles.SelectedEntity.Id,
                NewCommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id,
                NewDateOfContract = _txtContractDate.Text,
                NewResidence = _txtAddress.Text,
                NewLocality = _txtLocality.Text,
                NewZipCpde = _txtPostalCode.Text,
                NewCity = _txtCity.Text,
                NewPhone = _txtPhone.Text,
                NewMobilePhone = _txtMobile.Text,
                NewEmail = _txtEmail.Text,
                NewFiscalNumber = _txtFiscalNumber.Text,
                NewLanguage = _txtLanguage.Text,
                NewAssignedSeating = _txtAssignedSeating.Text,
                NewBaseConsumption = _txtConsumptionBase.Text,
                NewBaseOffers = _txtBaseOffers.Text,
                NewPVPOffers = _txtPVPOffers.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

        private AddUserCommand CreateAddCommand()
        {
            return new AddUserCommand
            {
                Name = _txtName.Text,
                ProfileId = _comboProfiles.SelectedEntity.Id,
                CommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id,
                DateOfContract = _txtContractDate.Text,
                Residence = _txtAddress.Text,
                Locality = _txtLocality.Text,
                ZipCpde = _txtPostalCode.Text,
                City = _txtCity.Text,
                Phone = _txtPhone.Text,
                MobilePhone = _txtMobile.Text,
                Email = _txtEmail.Text,
                FiscalNumber = _txtFiscalNumber.Text,
                Language = _txtLanguage.Text,
                AssignedSeating = _txtAssignedSeating.Text,
                BaseConsumption = _txtConsumptionBase.Text,
                BaseOffers = _txtBaseOffers.Text,
                PVPOffers = _txtPVPOffers.Text,
                Notes = _txtNotes.Value.Text
            };
        }
        
        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());

        private IEnumerable<UserProfile> GetProfiles() => ExecuteGetAllQuery(new GetAllUserProfilesQuery());

        private IEnumerable<CommissionGroup> GetCommissionGroups()=>ExecuteGetAllQuery(new GetAllCommissionGroupsQuery());
    }
}
