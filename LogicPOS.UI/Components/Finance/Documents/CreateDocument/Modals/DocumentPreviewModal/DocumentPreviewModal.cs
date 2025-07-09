using Gtk;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Item = LogicPOS.UI.Components.Documents.CreateDocument.Item;

namespace LogicPOS.UI.Components.Finance.Documents.CreateDocument.Modals.CreateDocumentModal.DocumentPreviewModal
{
    internal partial class DocumentPreviewModal : Modal
    {
        public DocumentPreviewModal(Window parent, List<Item> items, decimal customerDiscount):base(parent: parent,
                                                 title: GeneralUtils.GetResourceByName("window_title_dialog_documentfinance_preview_totals_mode_preview"),
                                                 size: new Size(700, 360),
                                                 icon: AppSettings.Paths.Images + @"Icons\Windows\icon_window_preview.png")
        {
            _items = items;
            _customerDiscount = customerDiscount;
            LoadData();
            Initialize();
        }

        private void LoadData()
        {
            if (_items.Count > 0)
            {
                foreach (var group in _items.GroupBy(vat => vat.VatDesignation))
                {
                    listModel.AppendValues(group.Key, group.Last().Vat.ToString("F2"), group.Sum(x => x.UnitPrice).ToString("F2"), group.Sum(x => x.VatPrice).ToString("F2"));
                }
            }

            var discountTotal = _items.Sum(d => d.DiscountPrice); 
            var totalNet = _items.Sum(t => t.TotalNet) - discountTotal;
            discountTotal = discountTotal +((totalNet * _customerDiscount) / 100);
            totalNet -= discountTotal;

            labelTotalGrossValue.Text = _items.Sum(t => t.UnitPrice).ToString("F2");
            labelDiscountCustomerValue.Text = _customerDiscount.ToString("F2");
            labelDiscountTotalValue.Text = discountTotal.ToString("F2");
            labelTotalNetValue.Text = totalNet.ToString("F2");
            LabelTaxTotalFinalValue.Text = _items.Sum(t => t.VatPrice).ToString("F2");
            labelTotalFinalValue.Text = (totalNet + _items.Sum(t => t.VatPrice)).ToString("F2");
        }



        private void Initialize()
        {
            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
        }
        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            return;
        }

        protected override Widget CreateBody()
        {
            DesignLabels();
            TreeView treeView = DesignTreeView();
            Table TotalTable = DesignTotalTable();
            HBox Hbox = new HBox();

            Hbox.PackStart(treeView, true, true, 0);
            Hbox.PackStart(TotalTable, true, true, 0);

            Hbox.ShowAll();

            return Hbox;

        }
    }
}
