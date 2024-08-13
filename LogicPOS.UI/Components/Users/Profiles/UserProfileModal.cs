using ErrorOr;
using LogicPOS.Api.Features.Users.Profiles;
using LogicPOS.Api.Features.Users.Profiles.AddUserProfile;
using LogicPOS.UI.Alerts;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UserProfileModal : EntityModal
    {
        public UserProfileModal(
            EntityModalMode modalMode,
            UserProfile userProfile = null) : base(modalMode, userProfile)
        {
        }

        protected override void ButtonOk_Clicked(object sender, EventArgs e)
        {
            switch (_modalMode)
            {
                case EntityModalMode.Insert:
                    AddUserProfile();
                    break;
                case EntityModalMode.Update:
                    break;
            }
        }

        private void HandleAddResult(ErrorOr<Guid> addUserProfileResult)
        {
            if (addUserProfileResult.IsError)
            {
                HandleError(addUserProfileResult.FirstError);
                return;
            }

            Destroy();
        }

        private AddUserProfileCommand CreateAddCommand()
        {
            return new AddUserProfileCommand
            {
                Designation = _txtDesignation.Text,
                Notes = _txtNotes.Value.Text
            };
        }
        private void AddUserProfile()
        {
            var command = CreateAddCommand();
            var result = _mediator.Send(command).Result;
            HandleAddResult(result);
        }
        protected override void ShowEntity()
        {
            var userProfile = _entity as UserProfile;
            _txtOrder.Text = userProfile.Order.ToString();
            _txtCode.Text = userProfile.Code;
            _txtDesignation.Text = userProfile.Designation;
            _txtNotes.Value.Text = userProfile.Notes;
            _checkDisabled.Active = userProfile.IsDeleted;
        }
    }
}
