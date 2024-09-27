using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.GetAllDocuments;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class DocumentsPage : Page<Document>
    {
        protected override IRequest<ErrorOr<IEnumerable<Document>>> GetAllQuery => new GetAllDocumentsQuery();
        public DocumentsPage(Window parent, Dictionary<string,string> options = null) : base(parent,options)
        {
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        protected override void InitializeFilter()
        {
            GridViewSettings.Filter = new TreeModelFilter(GridViewSettings.Model, null);
            GridViewSettings.Filter.VisibleFunc = (model, iterator) =>
            {
                var search = Navigator.SearchBox.SearchText.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(search))
                {
                    return true;
                }

                var entity = model.GetValue(iterator, 0) as Document;

                if (entity != null && entity.Number.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
        }

        public override void RunModal(EntityEditionModalMode mode)
        {
          
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateDateColumn());
            GridView.AppendColumn(CreateNumberColumn());
            GridView.AppendColumn(CreateStatusColumn());
            GridView.AppendColumn(CreateEntityColumn());
            GridView.AppendColumn(CreateFiscalNumberColumn());
            GridView.AppendColumn(CreateTotalFinalColumn());
        }

        private TreeViewColumn CreateTotalFinalColumn()
        {
            void RenderTotalFinal(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (Document)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.TotalFinal.ToString("C");
            }

            var title = GeneralUtils.GetResourceByName("global_total_final");
            return Columns.CreateColumn(title, 5, RenderTotalFinal);
        }

        private TreeViewColumn CreateFiscalNumberColumn()
        {
            void RenderFiscalNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (Document)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.Customer.FiscalNumber;
            }

            var title = GeneralUtils.GetResourceByName("global_fiscal_number");
            return Columns.CreateColumn(title, 4, RenderFiscalNumber);
        }

        private TreeViewColumn CreateEntityColumn()
        {
            void RenderEntity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (Document)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.Customer.Name;
            }

            var title = GeneralUtils.GetResourceByName("global_entity");
            return Columns.CreateColumn(title, 3, RenderEntity);
        }

        private TreeViewColumn CreateStatusColumn()
        {
            void RenderStatus(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (Document)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.Status;
            }

            var title = GeneralUtils.GetResourceByName("global_document_status");
            return Columns.CreateColumn(title, 2, RenderStatus);
        }

        private TreeViewColumn CreateDateColumn()
        {
            void RenderDate(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (Document)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.Date;
            }

            var title = GeneralUtils.GetResourceByName("global_document_date");
            return Columns.CreateColumn(title, 0, RenderDate);
        }

        private TreeViewColumn CreateNumberColumn()
        {
            void RenderNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (Document)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.Number;
            }

            var title = GeneralUtils.GetResourceByName("global_document_number");
            return Columns.CreateColumn(title, 1, RenderNumber);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddDateSorting();
            AddNumberSorting();
            AddStatusSorting();
            AddEntitySorting();
            AddFiscalNumberSorting();
            AddTotalFinalSorting();
        }

        private void AddTotalFinalSorting()
        {
            GridViewSettings.Sort.SetSortFunc(5, (model, left, right) =>
            {
                var a = (Document)model.GetValue(left, 0);
                var b = (Document)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.TotalFinal.CompareTo(b.TotalFinal);
            });
        }

        private void AddFiscalNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
            {
                var a = (Document)model.GetValue(left, 0);
                var b = (Document)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Customer.FiscalNumber.CompareTo(b.Customer.FiscalNumber);
            });
        }

        private void AddEntitySorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var a = (Document)model.GetValue(left, 0);
                var b = (Document)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Customer.Name.CompareTo(b.Customer.Name);
            });
        }

        private void AddStatusSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var a = (Document)model.GetValue(left, 0);
                var b = (Document)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Status.CompareTo(b.Status);
            });
        }

        private void AddDateSorting()
        {
            GridViewSettings.Sort.SetSortFunc(0, (model, left, right) =>
            {
                var a = (Document)model.GetValue(left, 0);
                var b = (Document)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.CreatedAt.CompareTo(b.CreatedAt);
            });
        }

        private void AddNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, left, right) =>
            {
                var a = (Document)model.GetValue(left, 0);
                var b = (Document)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Number.CompareTo(b.Number);
            });
        }


    }
}
