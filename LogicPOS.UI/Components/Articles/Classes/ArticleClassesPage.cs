using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Classes.GetAllArticleClasses;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class ArticleClassesPage : Page<ArticleClass>
    {
        public ArticleClassesPage(Window parent) : base(parent)
        {
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }
        protected override IRequest<ErrorOr<IEnumerable<ArticleClass>>> GetAllQuery => new GetAllArticleClassesQuery();

        public override void RunModal(EntityEditionModalMode mode)
        {
            var modal = new ArticleClassModal(mode, SelectedEntity as ArticleClass);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateAcronymColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

        private TreeViewColumn CreateAcronymColumn()
        {
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var articleClass = (ArticleClass)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = articleClass.Acronym.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_acronym");
            return Columns.CreateColumn(title, 2, RenderMonth);
        }

        protected override void InitializeSort()
        {

            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddAcronymSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddAcronymSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftArticleClass = (ArticleClass)model.GetValue(left, 0);
                var rightArticleClass = (ArticleClass)model.GetValue(right, 0);

                if (leftArticleClass == null || rightArticleClass == null)
                {
                    return 0;
                }

                return leftArticleClass.Acronym.CompareTo(rightArticleClass.Acronym);
            });
        }

        #region Singleton   
        private static ArticleClassesPage _instance;
        public static ArticleClassesPage Instance(Window parent)
        {
            if (_instance == null)
            {
                _instance = new ArticleClassesPage(parent);
            }

            return _instance;
        }
        #endregion
    }
}
