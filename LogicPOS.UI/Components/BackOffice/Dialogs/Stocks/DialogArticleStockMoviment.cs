using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.Extensions;
using System;
using System.Drawing;
using System.IO;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Utility;

namespace logicpos.Classes.Gui.Gtk.BackOffice.Dialogs.Articles
{
    internal class DialogArticleStockMoviment : BOBaseDialog
    {
        private VBox vboxTab3;
        private XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber> _entryBoxArticleSerialNumber;
        private XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectSupplier;
        private EntryBoxValidation _entryBoxDocumentNumber;
        private EntryBoxValidation _entryBoxPrice1;
        private EntryBoxValidation _entryBoxQuantity;
        private EntryBoxValidationDatePickerDialog _entryBoxDocumentDateIn;
        private readonly Entity _xPGuidObject;
        private readonly Window _sourceWindow;
        private byte[] AttachedFile;

        public TouchButtonIconWithText ButtonInsert { get; set; }
        protected GenericTreeViewNavigator<fin_article, TreeViewArticle> _navigator;
        public GenericTreeViewNavigator<fin_article, TreeViewArticle> Navigator
        {
            get { return _navigator; }
            set { _navigator = value; }
        }

        public EntryBoxValidation EntryBoxSerialNumber1 { get; set; }


        public DialogArticleStockMoviment(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pDialogFlags, Entity pXPGuidObject)
            : base(pSourceWindow, pTreeView, pDialogFlags, DialogMode.Update, pXPGuidObject)
        {
            _sourceWindow = pSourceWindow;
            _xPGuidObject = pXPGuidObject;
            this.Title = "Editar Movimento";
            if (GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600)
            {
                SetSizeRequest(320, 250);
            }
            else
            {
                SetSizeRequest(450, 520);
            }
            InitUI();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //Moviment In

                vboxTab3 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Supplier
                CriteriaOperator criteriaOperatorSupplier = CriteriaOperator.Parse("(Supplier = 1)");
                _entryBoxSelectSupplier = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(this, GeneralUtils.GetResourceByName("global_supplier"), "Name", "Oid", (_dataSourceRow as fin_articlestock).Customer, criteriaOperatorSupplier, LogicPOS.Utility.RegexUtils.RegexGuid, true, true);
                _entryBoxSelectSupplier.EntryValidation.IsEditable = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.PopupCompletion = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.InlineCompletion = false;
                _entryBoxSelectSupplier.EntryValidation.Completion.PopupSingleMatch = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.InlineSelection = true;
                _entryBoxSelectSupplier.Sensitive = ((_dataSourceRow as fin_articlestock).DocumentMaster == null);

                vboxTab3.PackStart(_entryBoxSelectSupplier, false, false, 0);

                //_entryBoxSelectSupplier.EntryValidation.Changed += delegate { ValidateDialog(); };

                //DocumentDate
                _entryBoxDocumentDateIn = new EntryBoxValidationDatePickerDialog(this, GeneralUtils.GetResourceByName("global_date"), GeneralUtils.GetResourceByName("global_date"), (_dataSourceRow as fin_articlestock).Date, LogicPOS.Utility.RegexUtils.RegexDate, true, CultureSettings.DateFormat, true);
                //_entryBoxDocumentDate.EntryValidation.Sensitive = true;
                _entryBoxDocumentDateIn.EntryValidation.Text = (_dataSourceRow as fin_articlestock).Date.ToString(CultureSettings.DateFormat);
                _entryBoxDocumentDateIn.EntryValidation.Validate();
                _entryBoxDocumentDateIn.Sensitive = ((_dataSourceRow as fin_articlestock).DocumentMaster == null);
                //_entryBoxDocumentDate.ClosePopup += delegate { ValidateDialog(); };

                vboxTab3.PackStart(_entryBoxDocumentDateIn, false, false, 0);


                //DocumentNumber
                Color colorBaseDialogEntryBoxBackground = GeneralSettings.Settings["colorBaseDialogEntryBoxBackground"].StringToColor();
                string _fileIconListFinanceDocuments = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_finance_document.png";
                HBox hBoxDocument = new HBox(false, 0);
                _entryBoxDocumentNumber = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_document_number"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false, true);
                if ((_dataSourceRow as fin_articlestock).DocumentNumber != string.Empty) _entryBoxDocumentNumber.EntryValidation.Text = (_dataSourceRow as fin_articlestock).DocumentNumber;
                //_entryBoxDocumentNumber.EntryValidation.Changed += delegate { ValidateDialog(); };
                TouchButtonIcon attachPDFButton = new TouchButtonIcon("attachPDFButton", colorBaseDialogEntryBoxBackground, _fileIconListFinanceDocuments, new Size(20, 20), 30, 30);
                attachPDFButton.Clicked += AttachPDFButton_Clicked;
                ((_entryBoxDocumentNumber.Children[0] as VBox).Children[1] as HBox).PackEnd(attachPDFButton, false, false, 0);
                _entryBoxDocumentNumber.Sensitive = ((_dataSourceRow as fin_articlestock).DocumentMaster == null);
                vboxTab3.PackStart(_entryBoxDocumentNumber, false, false, 0);

                //Quantity
                _entryBoxQuantity = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_quantity"), KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexDecimal, false, true);
                _entryBoxQuantity.WidthRequest = 40;
                _entryBoxQuantity.EntryValidation.Text = (_dataSourceRow as fin_articlestock).Quantity.ToString();
                _entryBoxQuantity.Sensitive = ((_dataSourceRow as fin_articlestock).DocumentMaster == null);
                //_entryBoxPrice1.EntryValidation.Changed += EntryPurchasedPriceValidation_Changed;

                vboxTab3.PackStart(_entryBoxQuantity, false, false, 0);

                //Price
                _entryBoxPrice1 = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_price"), KeyboardMode.None, LogicPOS.Utility.RegexUtils.RegexDecimal, false, true);
                _entryBoxPrice1.WidthRequest = 40;
                _entryBoxPrice1.EntryValidation.Text = (_dataSourceRow as fin_articlestock).PurchasePrice.ToString();
                _entryBoxPrice1.Sensitive = ((_dataSourceRow as fin_articlestock).DocumentMaster == null);
                //_entryBoxPrice1.EntryValidation.Changed += EntryPurchasedPriceValidation_Changed;

                vboxTab3.PackStart(_entryBoxPrice1, false, false, 0);


                //SerialNumber 
                _entryBoxArticleSerialNumber = new XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>(this, "Número de série", "SerialNumber", "Oid", (_dataSourceRow as fin_articlestock).ArticleSerialNumber, null, LogicPOS.Utility.RegexUtils.RegexGuid, true, true);
                _entryBoxArticleSerialNumber.EntryValidation.IsEditable = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.PopupCompletion = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.InlineCompletion = false;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.PopupSingleMatch = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.InlineSelection = true;
                _entryBoxArticleSerialNumber.Sensitive = false;
                vboxTab3.PackStart(_entryBoxArticleSerialNumber, false, false, 0);


                _notebook.AppendPage(vboxTab3, new Label("Movimento de Entrada"));

                buttonOk.Clicked += ButtonOk_Clicked;

            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void ButtonOk_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Edit stock moviment IN
                if (_entryBoxSelectSupplier.EntryValidation.Validated && _entryBoxDocumentNumber.EntryValidation.Validated && _entryBoxPrice1.EntryValidation.Validated && _entryBoxDocumentDateIn.EntryValidation.Validated)
                {
                    (_dataSourceRow as fin_articlestock).Customer = _entryBoxSelectSupplier.Value;
                    (_dataSourceRow as fin_articlestock).DocumentNumber = _entryBoxDocumentNumber.EntryValidation.Text;
                    (_dataSourceRow as fin_articlestock).PurchasePrice = LogicPOS.Utility.DataConversionUtils.StringToDecimal(_entryBoxPrice1.EntryValidation.Text);
                    (_dataSourceRow as fin_articlestock).Date = _entryBoxDocumentDateIn.Value;
                    (_dataSourceRow as fin_articlestock).Quantity = LogicPOS.Utility.DataConversionUtils.StringToDecimal(_entryBoxQuantity.EntryValidation.Text);
                    if (AttachedFile != null) (_dataSourceRow as fin_articlestock).AttachedFile = AttachedFile;
                    (_dataSourceRow as fin_articlestock).Save();
                    _logger.Debug("Sock Moviment In Changed with sucess");

                    ResponseType responseType = logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, GeneralUtils.GetResourceByName("dialog_message_operation_successfully"), string.Format(GeneralUtils.GetResourceByName("global_documentticket_type_title_cs_short"), GeneralSettings.ServerVersion));
                }

            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void AttachPDFButton_Clicked(object sender, EventArgs e)
        {
            FileFilter fileFilterPDF = logicpos.Utils.GetFileFilterPDF();
            Pos.Dialogs.PosFilePickerDialog dialog = new Pos.Dialogs.PosFilePickerDialog(this, DialogFlags.DestroyWithParent, fileFilterPDF, FileChooserAction.Open);
            ResponseType response = (ResponseType)dialog.Run();
            if (response == ResponseType.Ok)
            {
                string fileNamePacked = dialog.FilePicker.Filename;
                string fileName = string.Format("{0}/", System.IO.Path.GetFileNameWithoutExtension(fileNamePacked));
                AttachedFile = File.ReadAllBytes(fileNamePacked);
                _entryBoxDocumentNumber.EntryValidation.Text = fileName.Replace("/", "");
                dialog.Destroy();
            }
            else { dialog.Destroy(); }
        }


    }

}
