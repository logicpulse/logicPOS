using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Subfamilies.DeleteArticleSubfamily;
using LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class ArticleSubfamiliesPage : Page<ArticleSubfamily>
    {
        public ArticleSubfamiliesPage(Window parent) : base(parent)
        {
        }

        protected override IRequest<ErrorOr<IEnumerable<ArticleSubfamily>>> GetAllQuery => new GetAllArticleSubfamiliesQuery();

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new ArticleSubfamilyModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateFamilyColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

        private TreeViewColumn CreateFamilyColumn()
        {
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var family = (ArticleSubfamily)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = family.Family?.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_article_family");
            return Columns.CreateColumn(title, 2, RenderMonth);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddFamilySorting();
            AddUpdatedAtSorting(3);
        }

        private void AddFamilySorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftArticleSubfamily = (ArticleSubfamily)model.GetValue(left, 0);
                var rightArticleSubfamily = (ArticleSubfamily)model.GetValue(right, 0);

                if (leftArticleSubfamily == null || rightArticleSubfamily == null)
                {
                    return 0;
                }

                return leftArticleSubfamily.Family?.Designation.CompareTo(rightArticleSubfamily.Family?.Designation) ?? 0;
            });
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteArticleSubfamilyCommand(SelectedEntity.Id);
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLESUBFAMILY_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLESUBFAMILY_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLESUBFAMILY_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLESUBFAMILY_VIEW");
        }

        #region Singleton
        private static ArticleSubfamiliesPage _instance;
        public static ArticleSubfamiliesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ArticleSubfamiliesPage(null);
                }

                return _instance;
            }
        }
        #endregion
    }
}
