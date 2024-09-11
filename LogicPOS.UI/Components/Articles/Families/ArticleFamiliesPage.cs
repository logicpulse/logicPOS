using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class ArticleFamiliesPage : Page<ArticleFamily>
    {
        public ArticleFamiliesPage(Window parent) : base(parent)
        {
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        protected override IRequest<ErrorOr<IEnumerable<ArticleFamily>>> GetAllQuery => new GetAllArticleFamiliesQuery();

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new ArticleFamilyModal(mode, SelectedEntity);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreatePrinterColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

        private TreeViewColumn CreatePrinterColumn()
        {
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var family = (ArticleFamily)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = family.Printer?.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_device_printer");
            return Columns.CreateColumn(title, 2, RenderMonth);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddPrinterSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddPrinterSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftArticleFamily = (ArticleFamily)model.GetValue(left, 0);
                var rightArticleFamily = (ArticleFamily)model.GetValue(right, 0);

                if (leftArticleFamily == null || rightArticleFamily == null)
                {
                    return 0;
                }

                return leftArticleFamily.Printer?.Designation.CompareTo(rightArticleFamily.Printer?.Designation) ?? 0;
            });
        }
    }
}
