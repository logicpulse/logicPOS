using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.GetAllDocuments;
using LogicPOS.Api.Features.Documents.GetDocumentsTotals;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public class DocumentsPage : Page<Document>
    {
        protected override IRequest<ErrorOr<IEnumerable<Document>>> GetAllQuery => new GetAllDocumentsQuery();
        private List<DocumentTotals> _documentsTotals = new List<DocumentTotals>();
        public List<Document> SelectedDocuments { get; private set; } = new List<Document>();
        public decimal SelectedDocumentsTotalFinal { get; private set; }
        public event EventHandler DocumentsSelectionChanged;

        public DocumentsPage(Window parent,
                             Dictionary<string, string> options = null) : base(parent,options)
        {
        }

        protected override void LoadEntities()
        {
            LoadDocumentsTotals();
            base.LoadEntities();
        }

        private void LoadDocumentsTotals()
        {
            var query = new GetDocumentsTotalsQuery();
            var result = _mediator.Send(query).Result;

            if (result.IsError)
            {
                ShowApiErrorAlert(result.FirstError);
                return;
            }

            if(_documentsTotals.Count > 0)
            {
                _documentsTotals.Clear();
            }

            _documentsTotals.AddRange(result.Value);
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
            GridView.AppendColumn(CreateSelectColumn());
            GridView.AppendColumn(CreateDateColumn());
            GridView.AppendColumn(CreateNumberColumn());
            GridView.AppendColumn(CreateStatusColumn());
            GridView.AppendColumn(CreateEntityColumn());
            GridView.AppendColumn(CreateFiscalNumberColumn());
            GridView.AppendColumn(CreateTotalFinalColumn());
            GridView.AppendColumn(CreateTotalPaidColumn());
            GridView.AppendColumn(CreateTotalToPayColumn());
        }

        private TreeViewColumn CreateTotalToPayColumn()
        {
            void RenderTotalToPay(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = ((Document)model.GetValue(iter, 0));
                var totalToPay = _documentsTotals.FirstOrDefault(x => x.DocumentId == document.Id)?.TotalToPay ?? document.TotalFinal;
                (cell as CellRendererText).Text = totalToPay.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("global_debit");
            return Columns.CreateColumn(title, 9, RenderTotalToPay);
        }

        private TreeViewColumn CreateTotalPaidColumn()
        {
            void RenderTotalPaid(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var documentId = ((Document)model.GetValue(iter, 0)).Id;
                var totalPaid = _documentsTotals.FirstOrDefault(x => x.DocumentId == documentId)?.TotalPaid ?? 0;
                (cell as CellRendererText).Text = totalPaid.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("window_title_dialog_document_finance_column_total_credit_rc_nc_based");
            return Columns.CreateColumn(title, 8, RenderTotalPaid);
        }

        private TreeViewColumn CreateTotalFinalColumn()
        {
            void RenderTotalFinal(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (Document)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.TotalFinal.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("global_total_final");
            return Columns.CreateColumn(title, 7, RenderTotalFinal);
        }

        private TreeViewColumn CreateFiscalNumberColumn()
        {
            void RenderFiscalNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (Document)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.Customer.FiscalNumber;
            }

            var title = GeneralUtils.GetResourceByName("global_fiscal_number");
            return Columns.CreateColumn(title, 6, RenderFiscalNumber);
        }

        private TreeViewColumn CreateEntityColumn()
        {
            void RenderEntity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (Document)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.Customer.Name;
            }

            var title = GeneralUtils.GetResourceByName("global_entity");
            return Columns.CreateColumn(title, 5, RenderEntity);
        }

        private TreeViewColumn CreateStatusColumn()
        {
            void RenderStatus(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (Document)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.Status;
            }

            var title = GeneralUtils.GetResourceByName("global_document_status");
            return Columns.CreateColumn(title, 4, RenderStatus);
        }

        private TreeViewColumn CreateDateColumn()
        {
            void RenderDate(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (Document)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.Date;
            }

            var title = GeneralUtils.GetResourceByName("global_document_date");
            return Columns.CreateColumn(title, 2, RenderDate);
        }

        private TreeViewColumn CreateNumberColumn()
        {
            void RenderNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (Document)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.Number;
            }

            var title = GeneralUtils.GetResourceByName("global_document_number");
            return Columns.CreateColumn(title, 3, RenderNumber);
        }

        private TreeViewColumn CreateSelectColumn()
        {
            TreeViewColumn selectColumn = new TreeViewColumn();
            
            var selectCellRenderer = new CellRendererToggle();
            selectColumn.PackStart(selectCellRenderer, true);

            selectCellRenderer.Toggled += CheckBox_Clicked;

            void RenderSelect(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (Document)model.GetValue(iter, 0);
                (cell as CellRendererToggle).Active = SelectedDocuments.Contains(document);
            }

            selectColumn.SetCellDataFunc(selectCellRenderer, RenderSelect);

            return selectColumn;
        }

        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            if (GridView.Model.GetIter(out TreeIter iterator, new TreePath(args.Path)))
            {
                var document = (Document)GridView.Model.GetValue(iterator, 0);

                if (SelectedDocuments.Contains(document))
                {
                    SelectedDocuments.Remove(document);
                    SelectedDocumentsTotalFinal -= document.TotalFinal;
                }
                else
                {
                    SelectedDocuments.Add(document);
                    SelectedDocumentsTotalFinal += document.TotalFinal;
                }

                DocumentsSelectionChanged?.Invoke(this, EventArgs.Empty);
            }
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
            GridViewSettings.Sort.SetSortFunc(7, (model, left, right) =>
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
            GridViewSettings.Sort.SetSortFunc(6, (model, left, right) =>
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
            GridViewSettings.Sort.SetSortFunc(5, (model, left, right) =>
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
            GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
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
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
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
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
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

        protected override void InitializeGridView()
        {
            GridViewSettings.Model = new ListStore(typeof(Document), typeof(bool));

            InitializeGridViewModel();

            GridView = new TreeView();
            GridView.Model = GridViewSettings.Sort;
            GridView.EnableSearch = true;
            GridView.SearchColumn = 1;

            GridView.RulesHint = true;
            GridView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));

            AddColumns();
            AddGridViewEventHandlers();
        }

        protected override void AddEntitiesToModel()
        {
            var model = (ListStore)GridViewSettings.Model;
            _entities.ForEach(entity => model.AppendValues(entity,false));
        }


    }
}
