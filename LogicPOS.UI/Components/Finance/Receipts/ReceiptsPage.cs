using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Documents.GetDocuments;
using LogicPOS.Api.Features.Receipts.GetReceipts;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Errors;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class ReceiptsPage : Page<Receipt>
    {
        public GetReceiptsQuery Query { get; private set; } = GetDefaultQuery();
        public PaginatedResult<Receipt> Receipts { get; private set; }
        public List<Receipt> SelectedReceipts { get; private set; } = new List<Receipt>();
        public decimal SelectedReceiptsTotalAmount { get; private set; }
        public event EventHandler PageChanged;

        public ReceiptsPage(Window parent,
                            Dictionary<string, string> options = null) : base(parent, options)
        {
        }

        protected override void LoadEntities()
        {
            var getReceiptsResult = _mediator.Send(Query).Result;

            if (getReceiptsResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getReceiptsResult,
                                                    source: SourceWindow);
                return;
            }

            Receipts = getReceiptsResult.Value;

            _entities.Clear();
            _entities.AddRange(Receipts.Items);
        }

        public void MoveToNextPage()
        {
            Query.Page = Receipts.Page + 1;
            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }

        public void MoveToPreviousPage()
        {
            Query.Page = Receipts.Page - 1;
            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RunFilter()
        {
            var filterModal = new DocumentsFilterModal(SourceWindow);
            var response = (ResponseType)filterModal.Run();
            var query = filterModal.GetReceiptsQuery();
            filterModal.Destroy();

            if (response != ResponseType.Ok)
            {
                return;
            }

            Query = query;
            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
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

                var receipt = model.GetValue(iterator, 0) as Receipt;

                if (receipt != null && receipt.RefNo.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
        }

        public override int RunModal(EntityEditionModalMode mode) => (int)ResponseType.None;

        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateSelectColumn());
            GridView.AppendColumn(CreateDateColumn());
            GridView.AppendColumn(CreateNumberColumn());
            GridView.AppendColumn(CreateStatusColumn());
            GridView.AppendColumn(CreateEntityColumn());
            GridView.AppendColumn(CreateFiscalNumberColumn());
            GridView.AppendColumn(CreateTotalColumn());
            GridView.AppendColumn(CreateRelatedReceiptsColumn());
        }

        private TreeViewColumn CreateRelatedReceiptsColumn()
        {
            void RenderRelatedReceipts(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = ((Receipt)model.GetValue(iter, 0));
                (cell as CellRendererText).Text = receipt.Notes;
            }

            var title = GeneralUtils.GetResourceByName("window_title_dialog_document_finance_column_related_doc");
            return Columns.CreateColumn(title, 8, RenderRelatedReceipts);
        }

        private TreeViewColumn CreateTotalColumn()
        {
            void RenderAmountColumn(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = ((Receipt)model.GetValue(iter, 0));
                (cell as CellRendererText).Text = receipt.Amount.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("global_total");
            return Columns.CreateColumn(title, 7, RenderAmountColumn);
        }

        private TreeViewColumn CreateFiscalNumberColumn()
        {
            void RenderFiscalNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = (Receipt)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = receipt.CustomerFiscalNumber;
            }

            var title = GeneralUtils.GetResourceByName("global_fiscal_number");
            return Columns.CreateColumn(title, 6, RenderFiscalNumber);
        }

        private TreeViewColumn CreateEntityColumn()
        {
            void RenderEntity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = (Receipt)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = receipt.Customer;
            }

            var title = GeneralUtils.GetResourceByName("global_entity");
            return Columns.CreateColumn(title, 5, RenderEntity);
        }

        private TreeViewColumn CreateStatusColumn()
        {
            void RenderStatus(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = (Receipt)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = receipt.Status;
            }

            var title = GeneralUtils.GetResourceByName("global_document_status");
            return Columns.CreateColumn(title, 4, RenderStatus);
        }

        private TreeViewColumn CreateDateColumn()
        {
            void RenderDate(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = (Receipt)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = receipt.CreatedAt.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_document_date");
            return Columns.CreateColumn(title, 2, RenderDate);
        }

        private TreeViewColumn CreateNumberColumn()
        {
            void RenderNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var receipt = (Receipt)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = receipt.RefNo;
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
                var receipt = (Receipt)model.GetValue(iter, 0);
                (cell as CellRendererToggle).Active = SelectedReceipts.Contains(receipt);
            }

            selectColumn.SetCellDataFunc(selectCellRenderer, RenderSelect);

            return selectColumn;
        }

        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            if (GridView.Model.GetIter(out TreeIter iterator, new TreePath(args.Path)))
            {
                var receipt = (Receipt)GridView.Model.GetValue(iterator, 0);

                if (SelectedReceipts.Contains(receipt))
                {
                    SelectedReceipts.Remove(receipt);
                    SelectedReceiptsTotalAmount -= receipt.Amount;
                }
                else
                {
                    SelectedReceipts.Add(receipt);
                    SelectedReceiptsTotalAmount += receipt.Amount;
                }

                PageChanged?.Invoke(this, EventArgs.Empty);
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
        }

        private void AddFiscalNumberSorting()
        {
            GridViewSettings.Sort.SetSortFunc(6, (model, left, right) =>
            {
                var a = (Receipt)model.GetValue(left, 0);
                var b = (Receipt)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return 0;
            });
        }

        private void AddEntitySorting()
        {
            GridViewSettings.Sort.SetSortFunc(5, (model, left, right) =>
            {
                var a = (Receipt)model.GetValue(left, 0);
                var b = (Receipt)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return 0;
            });
        }

        private void AddStatusSorting()
        {
            GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
            {
                var a = (Receipt)model.GetValue(left, 0);
                var b = (Receipt)model.GetValue(right, 0);

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
                var a = (Receipt)model.GetValue(left, 0);
                var b = (Receipt)model.GetValue(right, 0);

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
                var a = (Receipt)model.GetValue(left, 0);
                var b = (Receipt)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.RefNo.CompareTo(b.RefNo);
            });
        }

        protected override void InitializeGridView()
        {
            GridViewSettings.Model = new ListStore(typeof(Receipt), typeof(bool));

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
            _entities.ForEach(entity => model.AppendValues(entity, false));
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return null;
        }

        private static GetReceiptsQuery GetDefaultQuery()
        {
            var query = new GetReceiptsQuery
            {
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                EndDate = DateTime.Now,
            };

            return query;
        }
    }
}
