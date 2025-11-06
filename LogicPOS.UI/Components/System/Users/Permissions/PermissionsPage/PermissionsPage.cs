using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Users.Permissions.PermissionItems.GetAllPermissionItems;
using LogicPOS.Api.Features.Users.Permissions.Profiles.AddPermissionProfile;
using LogicPOS.Api.Features.Users.Permissions.Profiles.DeletePermissionProfile;
using LogicPOS.Api.Features.Users.Permissions.Profiles.GetAllPermissionProfiles;
using LogicPOS.Api.Features.Users.Profiles.GetAllUserProfiles;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Components.Windows;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Web.ApplicationServices;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PermissionsPage : Page<UserProfile>
    {
        private TreeView _gridPermissionItems;
        private GridViewSettings PermissionItemsGridViewSettings { get; } = new GridViewSettings
        {
            Model = new ListStore(typeof(PermissionItem), typeof(bool))
        };
        private List<PermissionProfile> _permissionProfiles = new List<PermissionProfile>();
        private List<PermissionItem> _permissionItems = new List<PermissionItem>();
        protected override IRequest<ErrorOr<IEnumerable<UserProfile>>> GetAllQuery => new GetAllUserProfilesQuery();

        public PermissionsPage(Window parentWindow) : base(parentWindow)
        {
        }

        protected override void LoadEntities()
        {
            base.LoadEntities();
            if (_permissionItems.Count == 0)
            {
                LoadPermissionItems();
            }
            LoadPermissionProfiles();
        }

        public override void Refresh()
        {
            LoadEntities();

            ClearGridViewModel();
            ClearPermissionitemsGridView();

            AddEntitiesToModel(_entities);
        }


        public override void Search(string searchText)
        {
           PermissionItemsGridViewSettings.Filter.Refilter();
            this.Navigator.Update();
        }

        private void ClearPermissionitemsGridView()
        {
            var model = (ListStore)PermissionItemsGridViewSettings.Model;
            model.Clear();
        }

        private void LoadPermissionItems()
        {
            var getPermissionItemsResult = _mediator.Send(new GetAllPermissionItemsQuery()).Result;

            if (getPermissionItemsResult.IsError)
            {
                HandleErrorResult(getPermissionItemsResult);
                return;
            }

            _permissionItems.Clear();
            _permissionItems.AddRange(getPermissionItemsResult.Value);
        }

        private void LoadPermissionProfiles()
        {
            var getPermissionProfiles = _mediator.Send(new GetAllPermissionProfilesQuery()).Result;

            if (getPermissionProfiles.IsError)
            {
                HandleErrorResult(getPermissionProfiles);
                return;
            }

            _permissionProfiles.Clear();
            _permissionProfiles.AddRange(getPermissionProfiles.Value);
        }

        private void AddPermissionItemsToModel()
        {
            var model = (ListStore)PermissionItemsGridViewSettings.Model;
            _permissionItems.ForEach(item => model.AppendValues(item, false));
        }

        public void ShowUserProfilePermissions(UserProfile userProfile)
        {
            var permissionItems = (ListStore)PermissionItemsGridViewSettings.Model;

            permissionItems.Foreach((model, path, iterator) =>
            {
                var permissionItem = (PermissionItem)model.GetValue(iterator, 0);
                var granted = _permissionProfiles.Any(x => x.UserProfileId == userProfile.Id && x.PermissionItemId == permissionItem.Id);
                model.SetValue(iterator, 1, granted);
                return false;
            });
        }
      
        private void DeleteUserProfilePermission(PermissionItem permissionItem)
        {
            var userProfile = SelectedEntity;

            if (userProfile is null)
            {
                return;
            }

            var permissionProfile = _permissionProfiles.FirstOrDefault(x => x.UserProfileId == userProfile.Id && x.PermissionItemId == permissionItem.Id);

            if (permissionProfile is null)
            {
                return;
            }

            var deleteResult = _mediator.Send(new DeletePermissionProfileCommand
            {
                Id = permissionProfile.Id
            }).Result;

            if (deleteResult.IsError)
            {
                HandleErrorResult(deleteResult);
            }
            Users.AuthenticationService.RefreshPermissions();
        }

        private void AddUserProfilePermission(PermissionItem permissionItem)
        {
            var userProfile = SelectedEntity;

            if (userProfile is null)
            {
                return;
            }

            var addResult = _mediator.Send(new AddPermissionProfileCommand
            {
                PermissionItemId = permissionItem.Id,
                UserProfileId = userProfile.Id
            }).Result;

            if (addResult.IsError)
            {
                HandleErrorResult(addResult);
            }
            Users.AuthenticationService.RefreshPermissions();
        }
        
        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new UserProfileModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void InitializeFilter()
        {
            GridViewSettings.Filter = new TreeModelFilter(GridViewSettings.Model, null);
            GridViewSettings.Filter.VisibleFunc = (model, iterator) => true;
        }

        protected override void InitializeGridView()
        {
            InitializeUserProfilesGridView();
            InitializePermissionItemsGrid();
        }

        protected override void AddEntitiesToModel(IEnumerable<UserProfile> profiles)
        {
            base.AddEntitiesToModel(profiles);
            AddPermissionItemsToModel();
        }


        #region Singleton
        private static PermissionsPage _instance;
        public static PermissionsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PermissionsPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }
}
