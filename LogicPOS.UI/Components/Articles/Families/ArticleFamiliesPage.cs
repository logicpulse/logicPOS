using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Families.DeleteArticleFamily;
using LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class ArticleFamiliesPage : Page<ArticleFamily>
    {
        public ArticleFamiliesPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
        }

        protected override IRequest<ErrorOr<IEnumerable<ArticleFamily>>> GetAllQuery => new GetAllArticleFamiliesQuery();

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new ArticleFamilyModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(2));
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddUpdatedAtSorting(3);
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteArticleFamilyCommand(SelectedEntity.Id);
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLEFAMILY_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLEFAMILY_EDIT");
            this.Navigator.BtnDelete.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLEFAMILY_DELETE");
            this.Navigator.BtnView.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLEFAMILY_VIEW");
        }

        #region Singleton
        private static ArticleFamiliesPage _instance;
        public static ArticleFamiliesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ArticleFamiliesPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }
}
