using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentsPage
    {
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Model);

            AddDateSorting();
            AddNumberSorting();
            AddStatusSorting();
            AddEntitySorting();
            AddFiscalNumberSorting();
            AddTotalFinalSorting();
        }

        #region Creators
        private TreeViewColumn CreateRelatedDocumentsColumn()
        {
            void RenderRelatedDocuments(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var doc = (DocumentViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = string.Join(",", doc.RelatedDocuments);
            }

            var title = GeneralUtils.GetResourceByName("window_title_dialog_document_finance_column_related_doc");
            return Columns.CreateColumn(title, 10, RenderRelatedDocuments);
        }

        private TreeViewColumn CreateTotalToPayColumn()
        {
            void RenderTotalToPay(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = ((DocumentViewModel)model.GetValue(iter, 0));
                (cell as CellRendererText).Text = document.TotalToPay.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("global_debit");
            return Columns.CreateColumn(title, 9, RenderTotalToPay);
        }

        private TreeViewColumn CreateTotalPaidColumn()
        {
            void RenderTotalPaid(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = ((DocumentViewModel)model.GetValue(iter, 0));
                (cell as CellRendererText).Text = document.TotalPaid.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("window_title_dialog_document_finance_column_total_credit_rc_nc_based");
            return Columns.CreateColumn(title, 8, RenderTotalPaid);
        }

        private TreeViewColumn CreateTotalFinalColumn()
        {
            void RenderTotalFinal(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (DocumentViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.TotalFinal.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("global_total_final");
            return Columns.CreateColumn(title, 7, RenderTotalFinal);
        }

        private TreeViewColumn CreateFiscalNumberColumn()
        {
            void RenderFiscalNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (DocumentViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.Customer.FiscalNumber;
            }

            var title = GeneralUtils.GetResourceByName("global_fiscal_number");
            return Columns.CreateColumn(title, 6, RenderFiscalNumber);
        }

        private TreeViewColumn CreateEntityColumn()
        {
            void RenderEntity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (DocumentViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.Customer.Name;
            }

            var title = GeneralUtils.GetResourceByName("global_entity");
            return Columns.CreateColumn(title, 5, RenderEntity);
        }

        private TreeViewColumn CreateStatusColumn()
        {
            void RenderStatus(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (DocumentViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.IsDraft ? "Rascunho" : document.Status;

                if (document.IsDraft)
                {
                    (cell as CellRendererText).ForegroundGdk = new Gdk.Color(255, 0, 0);
                }
                else
                {
                    (cell as CellRendererText).ForegroundGdk = new Gdk.Color(0, 0, 0);
                }
            }

            var title = GeneralUtils.GetResourceByName("global_document_status");
            return Columns.CreateColumn(title, 4, RenderStatus);
        }

        private TreeViewColumn CreateDateColumn()
        {
            void RenderDate(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (DocumentViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.CreatedAt.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_document_date");
            return Columns.CreateColumn(title, 2, RenderDate);
        }

        private TreeViewColumn CreateNumberColumn()
        {
            void RenderNumber(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var document = (DocumentViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = document.Number;
                
                if (document.IsDraft)
                {
                    (cell as CellRendererText).ForegroundGdk = new Gdk.Color(255, 0, 0);
                } else
                {
                    (cell as CellRendererText).ForegroundGdk = new Gdk.Color(0, 0, 0);
                }
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
                var document = (DocumentViewModel)model.GetValue(iter, 0);
                (cell as CellRendererToggle).Active = SelectedDocuments.Contains(document);
            }

            selectColumn.SetCellDataFunc(selectCellRenderer, RenderSelect);

            return selectColumn;
        }
        #endregion

        #region Sorting
        private void AddTotalFinalSorting()
        {
            GridViewSettings.Sort.SetSortFunc(7, (model, left, right) =>
            {
                var a = (DocumentViewModel)model.GetValue(left, 0);
                var b = (DocumentViewModel)model.GetValue(right, 0);

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
                var a = (DocumentViewModel)model.GetValue(left, 0);
                var b = (DocumentViewModel)model.GetValue(right, 0);

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
                var a = (DocumentViewModel)model.GetValue(left, 0);
                var b = (DocumentViewModel)model.GetValue(right, 0);

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
                var a = (DocumentViewModel)model.GetValue(left, 0);
                var b = (DocumentViewModel)model.GetValue(right, 0);

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
                var a = (DocumentViewModel)model.GetValue(left, 0);
                var b = (DocumentViewModel)model.GetValue(right, 0);

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
                var a = (DocumentViewModel)model.GetValue(left, 0);
                var b = (DocumentViewModel)model.GetValue(right, 0);

                if (a == null || b == null)
                {
                    return 0;
                }

                return a.Number.CompareTo(b.Number);
            });
        }

        #endregion

    }
}
