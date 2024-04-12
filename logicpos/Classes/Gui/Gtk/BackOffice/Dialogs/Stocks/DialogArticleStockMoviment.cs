using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.BackOffice.Dialogs.Articles
{
    internal class DialogArticleStockMoviment : BOBaseDialog
    {
        //UI
        private readonly VBox vboxTab1;
        private readonly VBox vboxTab2;
        private VBox vboxTab3;
        private readonly VBox vboxTab4;
        private readonly fin_article _selectedArticle;
        private XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber> _entryBoxArticleSerialNumber;
        private readonly XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber> _entryBoxArticleSerialNumberToChange;
        private readonly XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber> _entryBoxArticleSerialNumberCompositionArticles;
        private XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectSupplier;
        private readonly XPOEntryBoxSelectRecordValidation<fin_documentfinancemaster, TreeViewDocumentFinanceMaster> _entryBoxSelectDocumentOut;
        private EntryBoxValidation _entryBoxDocumentNumber;
        private EntryBoxValidation _entryBoxPrice1;
        private EntryBoxValidation _entryBoxQuantity;
        private EntryBoxValidationDatePickerDialog _entryBoxDocumentDateIn;
        private readonly EntryBoxValidationDatePickerDialog _entryBoxDocumentDateOut;
        private readonly XPGuidObject _xPGuidObject;
        private readonly Window _sourceWindow;
        private readonly Viewport _viewport;
        private readonly TouchButtonIconWithText _buttonChange;
        private readonly TouchButtonIconWithText _buttonArticleOut;
        private byte[] AttachedFile;

        private TouchButtonIconWithText _buttonInsert;
        public TouchButtonIconWithText ButtonInsert
        {
            get { return _buttonInsert; }
            set { _buttonInsert = value; }
        }
        protected GenericTreeViewNavigator<fin_article, TreeViewArticle> _navigator;
        public GenericTreeViewNavigator<fin_article, TreeViewArticle> Navigator
        {
            get { return _navigator; }
            set { _navigator = value; }
        }

        private EntryBoxValidation _entryBoxSerialNumber1;
        public EntryBoxValidation EntryBoxSerialNumber1
        {
            get { return _entryBoxSerialNumber1; }
            set { _entryBoxSerialNumber1 = value; }
        }


        public DialogArticleStockMoviment(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pDialogFlags, XPGuidObject pXPGuidObject)
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
                _entryBoxSelectSupplier = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_supplier"), "Name", "Oid", (_dataSourceRow as fin_articlestock).Customer, criteriaOperatorSupplier, SettingsApp.RegexGuid, true, true);
                _entryBoxSelectSupplier.EntryValidation.IsEditable = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.PopupCompletion = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.InlineCompletion = false;
                _entryBoxSelectSupplier.EntryValidation.Completion.PopupSingleMatch = true;
                _entryBoxSelectSupplier.EntryValidation.Completion.InlineSelection = true;
                _entryBoxSelectSupplier.Sensitive = ((_dataSourceRow as fin_articlestock).DocumentMaster == null);

                vboxTab3.PackStart(_entryBoxSelectSupplier, false, false, 0);

                //_entryBoxSelectSupplier.EntryValidation.Changed += delegate { ValidateDialog(); };

                //DocumentDate
                _entryBoxDocumentDateIn = new EntryBoxValidationDatePickerDialog(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), (_dataSourceRow as fin_articlestock).Date, SettingsApp.RegexDate, true, SettingsApp.DateFormat, true);
                //_entryBoxDocumentDate.EntryValidation.Sensitive = true;
                _entryBoxDocumentDateIn.EntryValidation.Text = (_dataSourceRow as fin_articlestock).Date.ToString(SettingsApp.DateFormat);
                _entryBoxDocumentDateIn.EntryValidation.Validate();
                _entryBoxDocumentDateIn.Sensitive = ((_dataSourceRow as fin_articlestock).DocumentMaster == null);
                //_entryBoxDocumentDate.ClosePopup += delegate { ValidateDialog(); };

                vboxTab3.PackStart(_entryBoxDocumentDateIn, false, false, 0);


                //DocumentNumber
                Color colorBaseDialogEntryBoxBackground = GlobalFramework.Settings["colorBaseDialogEntryBoxBackground"].StringToColor();
                string _fileIconListFinanceDocuments = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_toolbar_finance_document.png");
                HBox hBoxDocument = new HBox(false, 0);
                _entryBoxDocumentNumber = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_document_number"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false, true);
                if ((_dataSourceRow as fin_articlestock).DocumentNumber != string.Empty) _entryBoxDocumentNumber.EntryValidation.Text = (_dataSourceRow as fin_articlestock).DocumentNumber;
                //_entryBoxDocumentNumber.EntryValidation.Changed += delegate { ValidateDialog(); };
                TouchButtonIcon attachPDFButton = new TouchButtonIcon("attachPDFButton", colorBaseDialogEntryBoxBackground, _fileIconListFinanceDocuments, new Size(20, 20), 30, 30);
                attachPDFButton.Clicked += AttachPDFButton_Clicked;
                ((_entryBoxDocumentNumber.Children[0] as VBox).Children[1] as HBox).PackEnd(attachPDFButton, false, false, 0);
                _entryBoxDocumentNumber.Sensitive = ((_dataSourceRow as fin_articlestock).DocumentMaster == null);
                vboxTab3.PackStart(_entryBoxDocumentNumber, false, false, 0);

                //Quantity
                _entryBoxQuantity = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_quantity"), KeyboardMode.None, SettingsApp.RegexDecimal, false, true);
                _entryBoxQuantity.WidthRequest = 40;
                _entryBoxQuantity.EntryValidation.Text = (_dataSourceRow as fin_articlestock).Quantity.ToString();
                _entryBoxQuantity.Sensitive = ((_dataSourceRow as fin_articlestock).DocumentMaster == null);
                //_entryBoxPrice1.EntryValidation.Changed += EntryPurchasedPriceValidation_Changed;

                vboxTab3.PackStart(_entryBoxQuantity, false, false, 0);

                //Price
                _entryBoxPrice1 = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_price"), KeyboardMode.None, SettingsApp.RegexDecimal, false, true);
                _entryBoxPrice1.WidthRequest = 40;
                _entryBoxPrice1.EntryValidation.Text = (_dataSourceRow as fin_articlestock).PurchasePrice.ToString();
                _entryBoxPrice1.Sensitive = ((_dataSourceRow as fin_articlestock).DocumentMaster == null);
                //_entryBoxPrice1.EntryValidation.Changed += EntryPurchasedPriceValidation_Changed;

                vboxTab3.PackStart(_entryBoxPrice1, false, false, 0);


                //SerialNumber 
                _entryBoxArticleSerialNumber = new XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>(this, "Número de série", "SerialNumber", "Oid", (_dataSourceRow as fin_articlestock).ArticleSerialNumber, null, SettingsApp.RegexGuid, true, true);
                _entryBoxArticleSerialNumber.EntryValidation.IsEditable = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.PopupCompletion = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.InlineCompletion = false;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.PopupSingleMatch = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.InlineSelection = true;
                _entryBoxArticleSerialNumber.Sensitive = false;
                vboxTab3.PackStart(_entryBoxArticleSerialNumber, false, false, 0);


                _notebook.AppendPage(vboxTab3, new Label("Movimento de Entrada"));

                base.buttonOk.Clicked += ButtonOk_Clicked;

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
                    (_dataSourceRow as fin_articlestock).PurchasePrice = FrameworkUtils.StringToDecimal(_entryBoxPrice1.EntryValidation.Text);
                    (_dataSourceRow as fin_articlestock).Date = _entryBoxDocumentDateIn.Value;
                    (_dataSourceRow as fin_articlestock).Quantity = FrameworkUtils.StringToDecimal(_entryBoxQuantity.EntryValidation.Text);
                    if (AttachedFile != null) (_dataSourceRow as fin_articlestock).AttachedFile = AttachedFile;
                    (_dataSourceRow as fin_articlestock).Save();
                    _logger.Debug("Sock Moviment In Changed with sucess");

                    ResponseType responseType = logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_operation_successfully"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentticket_type_title_cs_short"), GlobalFramework.ServerVersion));
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
