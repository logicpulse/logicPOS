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
        public ArticleFamiliesPage(Window parent, Dictionary<string,string> options = null) : base(parent,options)
        {
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        protected override IRequest<ErrorOr<IEnumerable<ArticleFamily>>> GetAllQuery => new GetAllArticleFamiliesQuery();

        public override void RunModal(EntityEditionModalMode mode)
        {
            var modal = new ArticleFamilyModal(mode, SelectedEntity);
            modal.Run();
            modal.Destroy();
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
    }
}
