using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PermissionsPage
    {
        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            var path = new TreePath(args.Path);

            if (!PermissionItemsGridViewSettings.Filter.GetIter(out TreeIter filterIter, path))
            {
                return;
            }

            TreeIter childIter = PermissionItemsGridViewSettings.Filter.ConvertIterToChildIter(filterIter);

            var model = PermissionItemsGridViewSettings.Model; 

            var currentValue = (bool)model.GetValue(childIter, 1);
            model.SetValue(childIter, 1, !currentValue);

            var permissionItem = model.GetValue(childIter, 0) as PermissionItem;

            if (permissionItem != null)
            {
                if (currentValue)
                {
                    DeleteUserProfilePermission(permissionItem);
                }
                else
                {
                    AddUserProfilePermission(permissionItem);
                }
            }

            LoadEntities();
            ClearPermissionitemsGridView();
            AddPermissionItemsToModel();
            ShowUserProfilePermissions(SelectedEntity);
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

        protected override DeleteCommand GetDeleteCommand()
        {
            return null;
        }

    }
}
