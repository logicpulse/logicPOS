using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.DocumentTypes.GetAllDocumentTypes;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;


namespace LogicPOS.UI.Components.Pages
{
    public class DocumentTypesPage : Page<DocumentType>
    {
        public DocumentTypesPage(Window parent, Dictionary<string,string> options = null) : base(parent,options)
        {
        }

        protected override IRequest<ErrorOr<IEnumerable<DocumentType>>> GetAllQuery => new GetAllDocumentTypesQuery();

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityEditionModalMode mode)
        {
            if (mode == EntityEditionModalMode.Insert)
            {
                return;
            }

            var modal = new DocumentTypeModal(mode, SelectedEntity);
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
                var documentType = (DocumentType)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = documentType.Acronym.ToString();
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
                var leftType = (DocumentType)model.GetValue(left, 0);
                var rightType = (DocumentType)model.GetValue(right, 0);

                if (leftType == null || rightType == null)
                {
                    return 0;
                }

                return leftType.Acronym.CompareTo(rightType.Acronym);
            });
        }
    }

}
