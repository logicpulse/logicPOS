using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Users.GetAllUsers;
using MediatR;
using System.Collections.Generic;
using Gtk;
using System;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Users.DeleteUser;

namespace LogicPOS.UI.Components.Pages
{
    public class UsersPage : Page<UserDetail>
    {

        protected override IRequest<ErrorOr<IEnumerable<UserDetail>>> GetAllQuery => new GetAllUsersQuery();
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

        private TreeViewColumn CreateNameColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var user = (UserDetail)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = user.Name;
            }

            var title = GeneralUtils.GetResourceByName("global_users");
            return Columns.CreateColumn(title, 1, RenderValue);
        }

        private TreeViewColumn CreateProfileColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var user = (UserDetail)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = user.Profile.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_profile");
            return Columns.CreateColumn(title, 2, RenderValue);
        }

        private TreeViewColumn CreateFiscalNumberColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var user = (UserDetail)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = user.FiscalNumber;
            }

            var title = GeneralUtils.GetResourceByName("global_fiscal_number");
            return Columns.CreateColumn(title, 3, RenderValue);
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

        private void AddNameSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
            {
                var leftUser = (UserDetail)model.GetValue(left, 0);
                var rightUser = (UserDetail)model.GetValue(right, 0);

                if (leftUser == null || rightUser == null)
                {
                    return 0;
                }

                return leftUser.Name.CompareTo(rightUser.Name);
            });
        }

        private void AddProfileSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftUser = (UserDetail)model.GetValue(left, 0);
                var rightUser = (UserDetail)model.GetValue(right, 0);

                if (leftUser == null || rightUser == null)
                {
                    return 0;
                }

                return leftUser.Profile.Designation.CompareTo(rightUser.Profile.Designation);
            });
        }

        private void AddFiscalNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftUser = (UserDetail)model.GetValue(left, 0);
                var rightUser = (UserDetail)model.GetValue(right, 0);

                if (leftUser == null || rightUser == null)
                {
                    return 0;
                }

                return leftUser.FiscalNumber?.CompareTo(rightUser?.FiscalNumber) ?? 0;
            });
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
