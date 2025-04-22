using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Users.DeleteUser;
using LogicPOS.Api.Features.Users.GetAllUsers;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class UsersPage : Page<User>
    {

        protected override IRequest<ErrorOr<IEnumerable<User>>> GetAllQuery => new GetAllUsersQuery();
        public UsersPage(Window parent) : base(parent)
        {
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new UserModal(mode, SelectedEntity);
            var resposne = modal.Run();
            modal.Destroy();
            return resposne;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(CreateNameColumn());
            GridView.AppendColumn(CreateProfileColumn());
            GridView.AppendColumn(CreateFiscalNumberColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddNameSorting();
            AddProfileSorting();
            AddFiscalNumberSorting();
            AddUpdatedAtSorting(4);
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteUserCommand(SelectedEntity.Id);
        }

        #region Singleton
        private static UsersPage _instance;
        public static UsersPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UsersPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }
}
