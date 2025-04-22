using Gtk;
using LogicPOS.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            }
        }

        protected override void GridViewRow_Changed(object sender, EventArgs e)
        {
            base.GridViewRow_Changed(sender, e);
            ShowUserProfilePermissions(SelectedEntity);
        }
        public override void Refresh()
        {
            LoadEntities();
            var model = (ListStore)GridViewSettings.Model;
            model.Clear();
            base.AddEntitiesToModel();
        }

    }
}
