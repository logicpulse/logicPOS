using DevExpress.Data.Filtering;
using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using LogicPOS.Domain.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components;
using LogicPOS.UI.Components.Pickers;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Drawing;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.BackOffice.Dialogs.Articles
{
    internal class DialogArticleStockMoviment : EditDialog
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

        public IconButtonWithText ButtonInsert { get; set; }
        protected GridViewNavigator<fin_article, TreeViewArticle> _navigator;
        public GridViewNavigator<fin_article, TreeViewArticle> Navigator
        {
            get { return _navigator; }
            set { _navigator = value; }
        }

        public EntryBoxValidation EntryBoxSerialNumber1 { get; set; }


        public DialogArticleStockMoviment(Window parentWindow, XpoGridView pTreeView, DialogFlags pDialogFlags, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pDialogFlags, DialogMode.Update, pXPGuidObject)
        {
            _sourceWindow = parentWindow;
            _xPGuidObject = pXPGuidObject;
            this.Title = "Editar Movimento";
            if (LogicPOSAppContext.ScreenSize.Width == 800 && LogicPOSAppContext.ScreenSize.Height == 600)
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
                _entryBoxSelectSupplier = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(this, GeneralUtils.GetResourceByName("global_supplier"), "Name", "Oid", (Entity as fin_articlestock).Customer, criteriaOperatorSupplier, RegexUtils.RegexGuid, true, true);
                _entryBoxSelectSupplier.EntryValidation.IsEditable = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.PopupCompletion = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.InlineCompletion = false;
                _entryBoxSelectSupplier.EntryValidation.Completion.PopupSingleMatch = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.InlineSelection = true;
                _entryBoxSelectSupplier.Sensitive = ((Entity as fin_articlestock).DocumentMaster == null);

                vboxTab3.PackStart(_entryBoxSelectSupplier, false, false, 0);

                //_entryBoxSelectSupplier.EntryValidation.Changed += delegate { ValidateDialog(); };

                //DocumentDate
                _entryBoxDocumentDateIn = new EntryBoxValidationDatePickerDialog(this, GeneralUtils.GetResourceByName("global_date"), GeneralUtils.GetResourceByName("global_date"), (Entity as fin_articlestock).Date, RegexUtils.RegexDate, true, CultureSettings.DateFormat, true);
                //_entryBoxDocumentDate.EntryValidation.Sensitive = true;
                _entryBoxDocumentDateIn.EntryValidation.Text = (Entity as fin_articlestock).Date.ToString(CultureSettings.DateFormat);
                _entryBoxDocumentDateIn.EntryValidation.Validate();
                _entryBoxDocumentDateIn.Sensitive = ((Entity as fin_articlestock).DocumentMaster == null);
                //_entryBoxDocumentDate.ClosePopup += delegate { ValidateDialog(); };

                vboxTab3.PackStart(_entryBoxDocumentDateIn, false, false, 0);


                //DocumentNumber
                Color colorBaseDialogEntryBoxBackground = AppSettings.Instance.colorBaseDialogEntryBoxBackground;
                string _fileIconListFinanceDocuments = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_toolbar_finance_document.png";
                HBox hBoxDocument = new HBox(false, 0);
                _entryBoxDocumentNumber = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_document_number"), KeyboardMode.Alfa, RegexUtils.RegexAlfaNumericExtended, false, true);
                if ((Entity as fin_articlestock).DocumentNumber != string.Empty) _entryBoxDocumentNumber.EntryValidation.Text = (Entity as fin_articlestock).DocumentNumber;
                //_entryBoxDocumentNumber.EntryValidation.Changed += delegate { ValidateDialog(); };
                IconButton attachPDFButton = new IconButton(
                    new ButtonSettings
                    {
                        Name = "attachPDFButton",
                        BackgroundColor = colorBaseDialogEntryBoxBackground,
                        Icon = _fileIconListFinanceDocuments,
                        IconSize = new Size(20, 20),
                        ButtonSize = new Size(30, 30)
                    });

                attachPDFButton.Clicked += AttachPDFButton_Clicked;
                ((_entryBoxDocumentNumber.Children[0] as VBox).Children[1] as HBox).PackEnd(attachPDFButton, false, false, 0);
                _entryBoxDocumentNumber.Sensitive = ((Entity as fin_articlestock).DocumentMaster == null);
                vboxTab3.PackStart(_entryBoxDocumentNumber, false, false, 0);

                //Quantity
                _entryBoxQuantity = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_quantity"), KeyboardMode.None, RegexUtils.RegexDecimal, false, true);
                _entryBoxQuantity.WidthRequest = 40;
                _entryBoxQuantity.EntryValidation.Text = (Entity as fin_articlestock).Quantity.ToString();
                _entryBoxQuantity.Sensitive = ((Entity as fin_articlestock).DocumentMaster == null);
                //_entryBoxPrice1.EntryValidation.Changed += EntryPurchasedPriceValidation_Changed;

                vboxTab3.PackStart(_entryBoxQuantity, false, false, 0);

                //Price
                _entryBoxPrice1 = new EntryBoxValidation(this, GeneralUtils.GetResourceByName("global_price"), KeyboardMode.None, RegexUtils.RegexDecimal, false, true);
                _entryBoxPrice1.WidthRequest = 40;
                _entryBoxPrice1.EntryValidation.Text = (Entity as fin_articlestock).PurchasePrice.ToString();
                _entryBoxPrice1.Sensitive = ((Entity as fin_articlestock).DocumentMaster == null);
                //_entryBoxPrice1.EntryValidation.Changed += EntryPurchasedPriceValidation_Changed;

                vboxTab3.PackStart(_entryBoxPrice1, false, false, 0);


                //SerialNumber 
                _entryBoxArticleSerialNumber = new XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>(this, "Número de série", "SerialNumber", "Oid", (Entity as fin_articlestock).ArticleSerialNumber, null, RegexUtils.RegexGuid, true, true);
                _entryBoxArticleSerialNumber.EntryValidation.IsEditable = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.PopupCompletion = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.InlineCompletion = false;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.PopupSingleMatch = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.InlineSelection = true;
                _entryBoxArticleSerialNumber.Sensitive = false;
                vboxTab3.PackStart(_entryBoxArticleSerialNumber, false, false, 0);


                _notebook.AppendPage(vboxTab3, new Label("Movimento de Entrada"));

                ButtonOk.Clicked += ButtonOk_Clicked;

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
                    (Entity as fin_articlestock).Customer = _entryBoxSelectSupplier.Value;
                    (Entity as fin_articlestock).DocumentNumber = _entryBoxDocumentNumber.EntryValidation.Text;
                    (Entity as fin_articlestock).PurchasePrice = DataConversionUtils.StringToDecimal(_entryBoxPrice1.EntryValidation.Text);
                    (Entity as fin_articlestock).Date = _entryBoxDocumentDateIn.Value;
                    (Entity as fin_articlestock).Quantity = DataConversionUtils.StringToDecimal(_entryBoxQuantity.EntryValidation.Text);
                    if (AttachedFile != null) (Entity as fin_articlestock).AttachedFile = AttachedFile;
                    (Entity as fin_articlestock).Save();
                    _logger.Debug("Sock Moviment In Changed with sucess");


                    var alertTitle = GeneralUtils.GetResourceByName("global_documentticket_type_title_cs_short");
                    SimpleAlerts.ShowOperationSucceededAlert(alertTitle);

                }

            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void AttachPDFButton_Clicked(object sender, EventArgs e)
        {
            FileFilter fileFilterPDF = FilePicker.GetFileFilterPDF();
            FilePicker dialog = new FilePicker(this, DialogFlags.DestroyWithParent, fileFilterPDF, FileChooserAction.Open);
            ResponseType response = (ResponseType)dialog.Run();
            if (response == ResponseType.Ok)
            {
                string fileNamePacked = dialog.FileChooser.Filename;
                string fileName = string.Format("{0}/", System.IO.Path.GetFileNameWithoutExtension(fileNamePacked));
                AttachedFile = File.ReadAllBytes(fileNamePacked);
                _entryBoxDocumentNumber.EntryValidation.Text = fileName.Replace("/", "");
                dialog.Destroy();
            }
            else { dialog.Destroy(); }
        }


    }

}
