using Gtk;
using LogicPOS.Api.Entities;
using System;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PermissionsPage
    {
        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            TreeIter iterator;
            var path = new TreePath(args.Path);

            if (_gridPermissionItems.Model.GetIter(out iterator, path))
            {
                var currentValue = (bool)_gridPermissionItems.Model.GetValue(iterator, 1);
                _gridPermissionItems.Model.SetValue(iterator, 1, !currentValue);

                var permissionItem = _gridPermissionItems.Model.GetValue(iterator, 0) as PermissionItem;

                if (currentValue)
                {
                    DeleteUserProfilePermission(permissionItem);
                }
                else
                {
                    AddUserProfilePermission(permissionItem);
                }

                Refresh();
            }
        }

        protected override void GridViewRow_Changed(object sender, EventArgs e)
        {
            base.GridViewRow_Changed(sender, e);
            ShowUserProfilePermissions(SelectedEntity);
        }

        public override void UpdateButtonPrevileges()
        {
            Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERPERMISSIONPROFILE_CREATE");
            Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERPERMISSIONPROFILE_EDIT");
            Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERPERMISSIONPROFILE_DELETE");
            Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_USERPERMISSIONPROFILE_VIEW");
        }

    }
}
