using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.GetAllArticles;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class ArticlesPage : Page<Article>
    {
        public ArticlesPage(Window parent, Dictionary<string,string> options = null) : base(parent, options)
        {
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        protected override IRequest<ErrorOr<IEnumerable<Article>>> GetAllQuery => new GetAllArticlesQuery();

        public override void RunModal(EntityEditionModalMode mode)
        {
            var modal = new ArticleModal(mode, SelectedEntity);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreatesTotalStockColumn());
            GridView.AppendColumn(CreateCompositeColumn());
            GridView.AppendColumn(CreateFamilyColumn());
            GridView.AppendColumn(CreateSubfamilyColumn());
            GridView.AppendColumn(CreateTypeColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(7));
        }

        private TreeViewColumn CreateCompositeColumn()
        {
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (Article)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.IsComposed ? GeneralUtils.GetResourceByName("global_treeview_true") : GeneralUtils.GetResourceByName("global_treeview_false");
            }

            var title = GeneralUtils.GetResourceByName("global_composite_article");
            return Columns.CreateColumn(title, 3, RenderMonth);
        }

        private TreeViewColumn CreateFamilyColumn()
        {
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (Article)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.Subfamily.Family.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_article_family");
            return Columns.CreateColumn(title, 4, RenderMonth);
        }

        private TreeViewColumn CreateSubfamilyColumn()
        {
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (Article)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.Subfamily.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_article_subfamily");
            return Columns.CreateColumn(title, 5, RenderMonth);
        }

        private TreeViewColumn CreatesTotalStockColumn()
        {
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (Article)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = 0.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_total_stock");
            return Columns.CreateColumn(title, 2, RenderMonth);
        }

        private TreeViewColumn CreateTypeColumn()
        {
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var article = (Article)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = article.Type.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_article_type");
            return Columns.CreateColumn(title, 6, RenderMonth);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddTotalStockSorting();
            AddCompositeSorting();
            AddFamilySorting();
            AddSubfamilySorting();
            AddTypeSorting();
            AddUpdatedAtSorting(7);
        }

        private void AddTotalStockSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftArticle = (Article)model.GetValue(left, 0);
                var rightArticle = (Article)model.GetValue(right, 0);

                if (leftArticle == null || rightArticle == null)
                {
                    return 0;
                }

                return 0;
            });
        }

        private void AddCompositeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftArticle = (Article)model.GetValue(left, 0);
                var rightArticle = (Article)model.GetValue(right, 0);

                if (leftArticle == null || rightArticle == null)
                {
                    return 0;
                }

                return leftArticle.IsComposed.CompareTo(rightArticle.IsComposed);
            });
        }

        private void AddFamilySorting()
        {
            GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
            {
                var leftArticle = (Article)model.GetValue(left, 0);
                var rightArticle = (Article)model.GetValue(right, 0);

                if (leftArticle == null || rightArticle == null)
                {
                    return 0;
                }

                return leftArticle.Subfamily?.Family?.Designation.CompareTo(rightArticle.Subfamily?.Family?.Designation) ?? 0;
            });
        }

        private void AddSubfamilySorting()
        {
            GridViewSettings.Sort.SetSortFunc(5, (model, left, right) =>
            {
                var leftArticle = (Article)model.GetValue(left, 0);
                var rightArticle = (Article)model.GetValue(right, 0);

                if (leftArticle == null || rightArticle == null)
                {
                    return 0;
                }

                return leftArticle.Subfamily?.Designation.CompareTo(rightArticle.Subfamily?.Designation) ?? 0;
            });
        }

        private void AddTypeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(6, (model, left, right) =>
            {
                var leftArticle = (Article)model.GetValue(left, 0);
                var rightArticle = (Article)model.GetValue(right, 0);

                if (leftArticle == null || rightArticle == null)
                {
                    return 0;
                }

                return leftArticle.Type?.Designation.CompareTo(rightArticle.Type?.Designation) ?? 0;
            });
        }
    }
}
