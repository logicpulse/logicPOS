using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.Api.Features.Users.AddUser;
using LogicPOS.Api.Features.Users.Profiles.GetAllUserProfiles;
using LogicPOS.Api.Features.Users.UpdateUser;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UserModal : EntityEditionModal<User>
    {
        public UserModal(EntityEditionModalMode modalMode, User entity = null) : base(modalMode, entity)
        {

        }

        protected override void ShowEntityData()
        {
            _txtName.Text = _entity.Name;
            _txtLogin.Text = _entity.Login;
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
                Order = uint.Parse(_txtOrder.Text),
                Code = _txtCode.Text,
                Name = _txtName.Text,
                Login = _txtLogin.Text,
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
                Notes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override bool UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand()).IsError == false;

        private AddUserCommand CreateAddCommand()
        {
            return new AddUserCommand
            {
                Name = _txtName.Text,
                Login = _txtLogin.Text,
                ProfileId = _comboProfiles.SelectedEntity.Id,
                CommissionGroupId = _comboCommissionGroups.SelectedEntity?.Id,
                DateOfContract = _txtContractDate.Text,
                Residence = _txtAddress.Text,
                Locality = _txtLocality.Text,
                ZipCode = _txtPostalCode.Text,
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
        
        protected override bool AddEntity() => ExecuteAddCommand(CreateAddCommand()).IsError == false;

        private IEnumerable<UserProfile> GetProfiles() => ExecuteGetEntitiesQuery(new GetAllUserProfilesQuery());

        private IEnumerable<CommissionGroup> GetCommissionGroups()=>ExecuteGetEntitiesQuery(new GetAllCommissionGroupsQuery());
    }
}
