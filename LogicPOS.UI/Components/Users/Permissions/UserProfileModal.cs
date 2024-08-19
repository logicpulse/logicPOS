using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Users.Profiles.AddUserProfile;
using LogicPOS.Api.Features.Users.Profiles.UpdateUserProfile;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UserProfileModal : EntityModal
    {
        public UserProfileModal(
            EntityModalMode modalMode,
            UserProfile userProfile = null) : base(modalMode, userProfile)
        {
        }

        private AddUserProfileCommand CreateAddCommand()
        {
            return new AddUserProfileCommand
            {
                Designation = _txtDesignation.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateUserProfileCommand CreateUpdateCommand()
        {
            return new UpdateUserProfileCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void ShowEntityData()
        {
            var userProfile = _entity as UserProfile;
            _txtOrder.Text = userProfile.Order.ToString();
            _txtCode.Text = userProfile.Code;
            _txtDesignation.Text = userProfile.Designation;
            _txtNotes.Value.Text = userProfile.Notes;
            _checkDisabled.Active = userProfile.IsDeleted;
        }

        protected override void UpdateEntity()
        {
            var command = CreateUpdateCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
            }
        }

        protected override void AddEntity()
        {
            var command = CreateAddCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
            }
        }
    }
}
