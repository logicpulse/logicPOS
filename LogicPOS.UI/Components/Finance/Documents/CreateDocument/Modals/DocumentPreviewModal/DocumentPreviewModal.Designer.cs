using Gtk;
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
using Item = LogicPOS.UI.Components.Documents.CreateDocument.DocumentDetail;

namespace LogicPOS.UI.Components.Finance.Documents.CreateDocument.Modals.CreateDocumentModal.DocumentPreviewModal
{
    internal partial class DocumentPreviewModal : Modal
    {

        protected override ActionAreaButtons CreateActionAreaButtons()
        {

            var areaButon = new ActionAreaButtons
             {
                new ActionAreaButton(BtnYes, ResponseType.Yes),
                new ActionAreaButton(BtnNo, ResponseType.No)
             };

            return areaButon;
        }

        private Table DesignTotalTable()
        {
            uint padding = 5;
            Gtk.Table TotalTable = new Gtk.Table(6, 2, true);
            TotalTable.Attach(labelTotalGross, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            TotalTable.Attach(labelTotalGrossValue, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            TotalTable.Attach(labelDiscountCustomer, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            TotalTable.Attach(labelDiscountCustomerValue, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            TotalTable.Attach(labelDiscountTotal, 0, 1, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            TotalTable.Attach(labelDiscountTotalValue, 1, 2, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            TotalTable.Attach(labelTotalNet, 0, 1, 3, 4, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            TotalTable.Attach(labelTotalNetValue, 1, 2, 3, 4, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            TotalTable.Attach(LabelTaxTotalFinal, 0, 1, 4, 5, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            TotalTable.Attach(LabelTaxTotalFinalValue, 1, 2, 4, 5, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            TotalTable.Attach(labelFinalTotal, 0, 1, 5, 6, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            TotalTable.Attach(labelTotalFinalValue, 1, 2, 5, 6, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            TotalTable.ShowAll();
            return TotalTable;
        }

        private TreeView DesignTreeView()
        {
            CellRendererText valuesRenderers = new CellRendererText();
            valuesRenderers.Font = "11";
            valuesRenderers.Xalign = 1.0F;
            TreeView treeView = new TreeView(listModel);

            TreeViewColumn columnDesignation = new TreeViewColumn { Spacing = 20, Expand = true};
            columnDesignation.Alignment = 1.0F;
            columnDesignation.PackStart(valuesRenderers, true);
            columnDesignation.AddAttribute(columnDesignation.CellRenderers[0], "text", 0);
            columnDesignation.Widget = labelDesignation;
            treeView.AppendColumn(columnDesignation);

            TreeViewColumn columnValue = new TreeViewColumn { Spacing = 20, Expand = true };
            columnValue.Alignment = 1.0F;
            columnValue.PackStart(valuesRenderers, true);
            columnValue.AddAttribute(columnValue.CellRenderers[0], "text", 1);
            columnValue.Widget = labelValue;
            treeView.AppendColumn(columnValue);

            TreeViewColumn columnValueBase = new TreeViewColumn { Spacing = 20, Expand = true };
            columnValueBase.Alignment = 1.0F;
            columnValueBase.PackStart(valuesRenderers, true);
            columnValueBase.AddAttribute(columnValueBase.CellRenderers[0], "text", 2);
            columnValueBase.Widget = labelBaseValue;
            treeView.AppendColumn(columnValueBase);


            TreeViewColumn columnTaxTotal = new TreeViewColumn { Spacing = 20, Expand = true };
            columnTaxTotal.Alignment = 1.0F;
            columnTaxTotal.PackStart(valuesRenderers, true);
            columnTaxTotal.AddAttribute(columnTaxTotal.CellRenderers[0], "text", 3);
            columnTaxTotal.Widget = labelTaxTotal;

            treeView.AppendColumn(columnTaxTotal);
            treeView.CanFocus = false;
            treeView.Selection.Mode = SelectionMode.None;
            return treeView;
        }

        private void DesignLabels()
        {
            labelDesignation.ModifyFont(TitleFontStyle);
            labelBaseValue.ModifyFont(TitleFontStyle);
            labelTaxTotal.ModifyFont(TitleFontStyle);
            labelValue.ModifyFont(TitleFontStyle);

            labelTotalGross.Xalign = 1;
            labelTotalGross.ModifyFont(TitleFontStyle);

            labelDiscountCustomer.Xalign = 1;
            labelDiscountCustomer.ModifyFont(TitleFontStyle);

            labelDiscountTotal.Xalign = 1;
            labelDiscountTotal.ModifyFont(TitleFontStyle);

            labelTotalNet.Xalign = 1;
            labelTotalNet.ModifyFont(TitleFontStyle);

            LabelTaxTotalFinal.Xalign = 1;
            LabelTaxTotalFinal.ModifyFont(TitleFontStyle);

            labelFinalTotal.Xalign = 1;
            labelFinalTotal.ModifyFont(TitleFontStyle);

            labelTotalGrossValue.ModifyFont(ValueFontStyle);
            labelDiscountCustomerValue.ModifyFont(ValueFontStyle);
            labelDiscountTotalValue.ModifyFont(ValueFontStyle);
            labelTotalNetValue.ModifyFont(ValueFontStyle);
            LabelTaxTotalFinalValue.ModifyFont(ValueFontStyle);
            labelTotalFinalValue.ModifyFont(ValueFontStyle);

            labelTotalGrossValue.Xalign = 1;
            labelDiscountCustomerValue.Xalign = 1;
            labelDiscountTotalValue.Xalign = 1;
            labelTotalNetValue.Xalign = 1;
            LabelTaxTotalFinalValue.Xalign = 1;
            labelTotalFinalValue.Xalign = 1;


            labelDesignation.Show();
            labelBaseValue.Show();
            labelTaxTotal.Show();
            labelValue.Show();
        }

        protected override Widget CreateBody()
        {
            DesignLabels();
            TreeView treeView = DesignTreeView();
            Table totalsTable = DesignTotalTable();
            HBox hbox = new HBox();
            hbox.PackStart(treeView, true, true, 20);
            hbox.PackStart(totalsTable, true, true, 20);
            hbox.ShowAll();

            VBox vbox = new VBox();
            vbox.PackStart(hbox, false, false, 20);
            return vbox;
        }

    }
}
