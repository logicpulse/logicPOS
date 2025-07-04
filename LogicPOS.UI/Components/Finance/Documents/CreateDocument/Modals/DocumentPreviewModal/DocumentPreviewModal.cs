using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Documents.CreateDocument;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;
using Microsoft.FSharp.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Color = System.Drawing.Color;
using Item = LogicPOS.UI.Components.Documents.CreateDocument.Item;

namespace LogicPOS.UI.Components.Finance.Documents.CreateDocument.Modals.CreateDocumentModal.DocumentPreviewModal
{
    internal class DocumentPreviewModal : Modal
    {
        Pango.FontDescription TitleFontStyle = Pango.FontDescription.FromString("Bold 11");
        Pango.FontDescription ValueFontStyle = Pango.FontDescription.FromString("11");

        private IconButtonWithText BtnOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        Label LabelDesignation = new Label(GeneralUtils.GetResourceByName("global_designation"));
        Label LabelBaseValue = new Label(GeneralUtils.GetResourceByName("global_total_tax_base"));
        Label LabelValue = new Label(GeneralUtils.GetResourceByName("global_vat_rate"));
        Label LabelTaxTotal = new Label(GeneralUtils.GetResourceByName("global_documentfinance_totaltax_acronym"));
        Label labelDiscountCustomer = new Label(GeneralUtils.GetResourceByName("global_documentfinance_discount_customer") + " (%)"); /* IN009206 */
        Label labelTotalNet = new Label(GeneralUtils.GetResourceByName("global_totalnet"));
        Label labelTotalGross = new Label(GeneralUtils.GetResourceByName("global_documentfinance_totalgross"));
        Label labelDiscountTotal = new Label(GeneralUtils.GetResourceByName("global_documentfinance_total_discount"));
        Label LabelTaxTotalFinal = new Label(GeneralUtils.GetResourceByName("global_documentfinance_totaltax"));
        private Label labelFinalTotal = new Label(GeneralUtils.GetResourceByName("global_documentfinance_totalfinal"));
        private readonly List<Item> _items= new List<Item>();
        private readonly decimal _customerDiscount;
        ListStore listModel = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string));
        private Label labelDiscountCustomerValue = new Label();
        private Label labelDiscountTotalValue = new Label();
        private Label labelTotalGrossValue = new Label();
        private Label labelTotalNetValue = new Label();
        private Label LabelTaxTotalFinalValue = new Label();
        private Label labelTotalFinalValue = new Label();

        public DocumentPreviewModal(Window parent, List<Item> items, decimal customerDiscount):base(parent: parent,
                                                 title: GeneralUtils.GetResourceByName("window_title_dialog_documentfinance_preview_totals_mode_preview"),
                                                 size: new Size(700, 360),
                                                 icon: PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_preview.png")
        {
            _items = items;
            _customerDiscount = customerDiscount;
            InitilizeData();
            Initialize();
        }

        private void InitilizeData()
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


        protected override ActionAreaButtons CreateActionAreaButtons()
        {

            var areaButon = new ActionAreaButtons
             {
                new ActionAreaButton(BtnOk, ResponseType.Ok)
             };

            return areaButon;
        }

        protected override Widget CreateBody()
        {
            CellRendererText valuesRenderers = new CellRendererText();
            valuesRenderers.Font = "11";

            LabelDesignation.ModifyFont(TitleFontStyle);
            LabelBaseValue.ModifyFont(TitleFontStyle);
            LabelTaxTotal.ModifyFont(TitleFontStyle);
            LabelValue.ModifyFont(TitleFontStyle);

            LabelDesignation.Show();
            LabelBaseValue.Show();
            LabelTaxTotal.Show();
            LabelValue.Show();

            
           

            TreeView treeView = new TreeView(listModel);

            TreeViewColumn columnDesignation = new TreeViewColumn { Spacing = 20, Expand = true };
            columnDesignation.PackStart(valuesRenderers, true);
            columnDesignation.AddAttribute(columnDesignation.CellRenderers[0], "text", 0);
            columnDesignation.Widget = LabelDesignation;
            treeView.AppendColumn(columnDesignation);

            TreeViewColumn columnValue = new TreeViewColumn { Spacing = 20, Expand = true };
            columnValue.PackStart(valuesRenderers, true);
            columnValue.AddAttribute(columnValue.CellRenderers[0], "text", 1);
            columnValue.Widget = LabelValue;
            treeView.AppendColumn(columnValue);

            TreeViewColumn columnValueBase = new TreeViewColumn { Spacing = 20, Expand = true };
            columnValueBase.PackStart(valuesRenderers, true);
            columnValueBase.AddAttribute(columnValueBase.CellRenderers[0], "text", 2);
            columnValueBase.Widget = LabelBaseValue;
            treeView.AppendColumn(columnValueBase);


            TreeViewColumn columnTaxTotal = new TreeViewColumn { Spacing = 20, Expand = true };
            columnTaxTotal.PackStart(valuesRenderers, true);
            columnTaxTotal.AddAttribute(columnTaxTotal.CellRenderers[0], "text", 3);
            columnTaxTotal.Widget = LabelTaxTotal;

            treeView.AppendColumn(columnTaxTotal);



            Gtk.Table TotalTable = new Gtk.Table(6, 2, false);



            labelTotalGross.Xalign = 1;
            labelTotalGross.ModifyFont(TitleFontStyle);

            labelDiscountCustomer.Xalign = 1;
            labelDiscountCustomer.ModifyFont(TitleFontStyle);

            labelDiscountTotal.Xalign = 1;
            labelDiscountTotal.ModifyFont(TitleFontStyle);

            labelTotalNet.Xalign = 1;
            labelTotalNet.ModifyFont(TitleFontStyle);

            LabelTaxTotalFinal.Xalign=1;
            LabelTaxTotalFinal.ModifyFont(TitleFontStyle);

            labelFinalTotal.Xalign = 1;
            labelFinalTotal.ModifyFont(TitleFontStyle);

            labelTotalGrossValue.ModifyFont(ValueFontStyle);
            labelDiscountCustomerValue.ModifyFont(ValueFontStyle);
            labelDiscountTotalValue.ModifyFont(ValueFontStyle);
            labelTotalNetValue.ModifyFont(ValueFontStyle);
            LabelTaxTotalFinalValue.ModifyFont(ValueFontStyle);
            labelTotalFinalValue.ModifyFont(ValueFontStyle);

            labelTotalGrossValue.Xalign=1;
            labelDiscountCustomerValue.Xalign=1;
            labelDiscountTotalValue.Xalign=1;
            labelTotalNetValue.Xalign=1;
            LabelTaxTotalFinalValue.Xalign=1;
            labelTotalFinalValue.Xalign = 1;



            // Attach widgets na TotalTable: (left, right, top, bottom)
            TotalTable.Attach(labelTotalGross, 0, 1, 0, 1);
            TotalTable.Attach(labelTotalGrossValue, 1, 2, 0, 1);
            TotalTable.Attach(labelDiscountCustomer, 0, 1, 1, 2);
            TotalTable.Attach(labelDiscountCustomerValue, 1, 2, 1, 2);
            TotalTable.Attach(labelDiscountTotal, 0, 1, 2, 3);
            TotalTable.Attach(labelDiscountTotalValue, 1, 2, 2, 3);
            TotalTable.Attach(labelTotalNet, 0, 1, 3, 4);
            TotalTable.Attach(labelTotalNetValue, 1, 2, 3, 4);
            TotalTable.Attach(LabelTaxTotalFinal, 0, 1, 4, 5);
            TotalTable.Attach(LabelTaxTotalFinalValue, 1, 2, 4, 5);
            TotalTable.Attach(labelFinalTotal, 0, 1, 5, 6);
            TotalTable.Attach(labelTotalFinalValue, 1, 2, 5, 6);
            TotalTable.ShowAll();

            HBox mainBox = new HBox();

            mainBox.PackStart(treeView, true, true, 0);
            mainBox.PackStart(TotalTable, true,true, 5);

            mainBox.ShowAll();

            return mainBox;

        }



    }
}
