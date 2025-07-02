using Gtk;
using logicpos;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Documents.CreateDocument;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Diagnostics;
using System.Drawing;

namespace LogicPOS.UI.Components.Finance.Documents.CreateDocument.Modals.CreateDocumentModal.DocumentPreviewModal
{
    internal class DocumentPreviewModal : Modal
    {

        private ModalTabsNavigator Navigator { get; set; }
        private CreateDocumentDocumentTab DocumentTab { get; set; }
        private CreateDocumentCustomerTab CustomerTab { get; set; }
        private CreateDocumentArticlesTab ArticlesTab { get; set; }
        private CreateDocumentShipToTab ShipToTab { get; set; }
        private CreateDocumentShipFromTab ShipFromTab { get; set; }
        private CreateDocumentPaymentMethodsTab PaymentMethodsTab { get; set; }


        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        Label lbDesignation{ get; set; } = new Label(GeneralUtils.GetResourceByName("global_designation"));
        public TreeView GridView { get; set; } = new TreeView();
        public GridViewSettings GridViewSettings { get; } = new GridViewSettings();
        public DocumentPreviewModal(Window parent) : base(parent: parent,
                                                 title: GeneralUtils.GetResourceByName("window_title_dialog_documentfinance_preview_totals_mode_preview"),
                                                 size: new Size(700, 360),
                                                 icon: PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_preview.png")
        {
            Initialize();
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
            Gtk.Table table = new Gtk.Table(4,4,false);
            table.Attach(lbDesignation, 0, 1, 0, 0, AttachOptions.Fill, AttachOptions.Fill, 1, 1);
            GridView.AppendColumn(Columns.CreateDesignationColumn(0));
            GridView.AppendColumn(CreateValueColumn());
            GridView.AppendColumn(CreateBaseValueColumn());
            GridView.AppendColumn(CreateAcronymColumn());

            EventBox eventboxTax = new EventBox();
            eventboxTax.ModifyBg(StateType.Normal,(Color.LightGray.ToGdkColor()));
            eventboxTax.Add(table);
            GridView.ShowAll();
            HBox mainBox = new HBox();
            mainBox.PackStart(eventboxTax, true, true, 0);
            mainBox.PackStart(GridView, true, true, 0);
            return mainBox;

        }

        #region Columns
        private TreeViewColumn CreateValueColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var vatRate = (VatRate)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = vatRate.Value.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_vat_rate");
            return Columns.CreateColumn(title, 1, RenderValue);
        }

        private TreeViewColumn CreateBaseValueColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var vatRate = (VatRate)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = vatRate.Value.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_total_tax_base");
            return Columns.CreateColumn(title, 2, RenderValue);
        }

        private TreeViewColumn CreateAcronymColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var vatRate = (VatRate)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = vatRate.Value.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_documentfinance_totaltax_acronym");
            return Columns.CreateColumn(title, 3, RenderValue);
        }

        public static void ShowModal(Window parent)
        {
            var modal = new DocumentPreviewModal(parent);
            modal.Run();
            modal.Destroy();
        }
        #endregion

    }
}
