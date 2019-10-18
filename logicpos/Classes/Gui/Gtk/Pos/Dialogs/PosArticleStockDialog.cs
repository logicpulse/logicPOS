using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial;
using logicpos.financial.library.Classes.Stocks;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Drawing;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    /// <summary>
    /// PosArticleStock Dialog
    /// </summary>
    public class PosArticleStockDialog : PosBaseDialog
    {
        //UI Components Dialog
        private VBox _vbox;
        TouchButtonIconWithText _buttonOk;
        TouchButtonIconWithText _buttonCancel;
        //UI Components Form
        private XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer> _entryBoxSelectSupplier;
        private EntryBoxValidationDatePickerDialog _entryBoxDocumentDate;
        private EntryBoxValidation _entryBoxDocumentNumber;
        private XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> _entryBoxSelectArticle;
        private EntryBoxValidation _entryBoxQuantity;
        private EntryBoxValidation _entryBoxNotes;
        //InitialValues
        private erp_customer _initialSupplier = null;
        private DateTime _initialDocumentDate;
        private string _initialDocumentNumber;

        //Public Methods
        public erp_customer Customer
        {
            get { return _entryBoxSelectSupplier.Value; }
        }
        public DateTime DocumentDate
        {
            get { return _entryBoxDocumentDate.Value; }
        }
        public string DocumentNumber
        {
            get { return _entryBoxDocumentNumber.EntryValidation.Text; }
        }
        public fin_article Article
        {
            get { return _entryBoxSelectArticle.Value; }
        }
        public decimal Quantity
        {
            get { return Convert.ToDecimal(_entryBoxQuantity.EntryValidation.Text); }
        }
        public string Notes
        {
            get { return _entryBoxNotes.EntryValidation.Text; }
        }

        public PosArticleStockDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_article_stock");
            Size windowSize = new Size(500, 480);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_stocks.png");

            InitUI();

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            _buttonOk.Sensitive = false;

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _vbox, actionAreaButtons);
        }

        private void InitUI()
        {
            try
            {
                //Load Initial Values
                Load();

                //Init VBOX
                _vbox = new VBox(true, 0);

                //Supplier
                CriteriaOperator criteriaOperatorSupplier = CriteriaOperator.Parse("(Disabled = 0 OR Disabled is NULL) AND (Supplier = 1)");
                _entryBoxSelectSupplier = new XPOEntryBoxSelectRecordValidation<erp_customer, TreeViewCustomer>(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_supplier"), "Name", "Oid", _initialSupplier, criteriaOperatorSupplier, SettingsApp.RegexGuid, true);
                _entryBoxSelectSupplier.EntryValidation.IsEditable = false;
                _entryBoxSelectSupplier.EntryValidation.Changed += delegate { ValidateDialog(); };

                //DocumentDate
                _entryBoxDocumentDate = new EntryBoxValidationDatePickerDialog(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_date"), _initialDocumentDate, SettingsApp.RegexDate, true, SettingsApp.DateFormat);
                //_entryBoxDocumentDate.EntryValidation.Sensitive = true;
                _entryBoxDocumentDate.EntryValidation.Text = _initialDocumentDate.ToString(SettingsApp.DateFormat);
                _entryBoxDocumentDate.EntryValidation.Validate();
                _entryBoxDocumentDate.EntryValidation.Sensitive = true;
                _entryBoxDocumentDate.ClosePopup += delegate { ValidateDialog(); };

                //DocumentNumber
                _entryBoxDocumentNumber = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_document_number"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
                if (_initialDocumentNumber != string.Empty) _entryBoxDocumentNumber.EntryValidation.Text = _initialDocumentNumber;
                _entryBoxDocumentNumber.EntryValidation.Changed += delegate { ValidateDialog(); };

                //SelectArticle
                CriteriaOperator criteriaOperatorSelectArticle = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Class = '{0}')", SettingsApp.XpoOidArticleDefaultClass));
                _entryBoxSelectArticle = new XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_article"), "Designation", "Oid", null, criteriaOperatorSelectArticle, SettingsApp.RegexGuid, true);
                _entryBoxSelectArticle.EntryValidation.IsEditable = false;
                _entryBoxSelectArticle.EntryValidation.Changed += delegate { ValidateDialog(); };

                //Quantity
                _entryBoxQuantity = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_quantity"), KeyboardMode.Numeric, SettingsApp.RegexDecimalPositiveAndNegative, true);
                _entryBoxQuantity.EntryValidation.Changed += delegate { ValidateDialog(); };

                //Notes
                _entryBoxNotes = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_notes"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
                _entryBoxNotes.EntryValidation.Changed += delegate { ValidateDialog(); };

                //Final Pack
                _vbox.PackStart(_entryBoxSelectSupplier, false, false, 0);
                _vbox.PackStart(_entryBoxDocumentDate, false, false, 0);
                _vbox.PackStart(_entryBoxDocumentNumber, false, false, 0);
                _vbox.PackStart(_entryBoxSelectArticle, false, false, 0);
                _vbox.PackStart(_entryBoxQuantity, false, false, 0);
                _vbox.PackStart(_entryBoxNotes, false, false, 0);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void ValidateDialog()
        {
            _buttonOk.Sensitive = (
                _entryBoxSelectSupplier.EntryValidation.Validated &&
                _entryBoxDocumentDate.EntryValidation.Validated &&
                _entryBoxDocumentNumber.EntryValidation.Validated &&
                _entryBoxSelectArticle.EntryValidation.Validated &&
                _entryBoxQuantity.EntryValidation.Validated &&
                _entryBoxNotes.EntryValidation.Validated
            );
        }

        public void Load()
        {
            //Get From Session if Exists
            object supplier = GlobalFramework.SessionApp.GetToken(string.Format("{0}_{1}", this.GetType().Name, "supplier").ToUpper());
            object documentDate = GlobalFramework.SessionApp.GetToken(string.Format("{0}_{1}", this.GetType().Name, "documentDate").ToUpper());
            object documentNumber = GlobalFramework.SessionApp.GetToken(string.Format("{0}_{1}", this.GetType().Name, "documentNumber").ToUpper());
            //Assign if Valid
            try
            {
                if (supplier != null) _initialSupplier = (erp_customer) GlobalFramework.SessionXpo.GetObjectByKey(typeof(erp_customer), new Guid(supplier.ToString()));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            
            try
            {
                _initialDocumentDate = (documentDate != null) ? Convert.ToDateTime(documentDate) : FrameworkUtils.CurrentDateTimeAtomic();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            try
            {
                _initialDocumentNumber = (documentNumber != null) ? Convert.ToString(documentNumber) : string.Empty;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public void Save()
        {
            try
            {
                GlobalFramework.SessionApp.SetToken(string.Format("{0}_{1}", this.GetType().Name, "supplier").ToUpper(), _entryBoxSelectSupplier.Value.Oid);
                GlobalFramework.SessionApp.SetToken(string.Format("{0}_{1}", this.GetType().Name, "documentDate").ToUpper(), _entryBoxDocumentDate.Value);
                GlobalFramework.SessionApp.SetToken(string.Format("{0}_{1}", this.GetType().Name, "documentNumber").ToUpper(), _entryBoxDocumentNumber.EntryValidation.Text);
                GlobalFramework.SessionApp.Write();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Helper Methods

        /// <summary>
        /// Get ArticleStock Response Object
        /// </summary>
        /// <param name="pSourceWindow"></param>
        /// <returns>PosArticleStockResponse</returns>
        public static ProcessArticleStockParameter GetProcessArticleStockParameter(Window pSourceWindow)
        {
            ProcessArticleStockParameter result = null;  

            PosArticleStockDialog dialog = new PosArticleStockDialog(pSourceWindow, DialogFlags.DestroyWithParent | DialogFlags.Modal);
            ResponseType response = (ResponseType) dialog.Run();
            if (response == ResponseType.Ok)
            {
                result = new ProcessArticleStockParameter(
                  dialog.Customer,
                  dialog.DocumentDate, 
                  dialog.DocumentNumber, 
                  dialog.Article, 
                  dialog.Quantity, 
                  dialog.Notes
              );
              //Save to Session
              dialog.Save();
            }
            dialog.Destroy();

            return result;
        }
    }
}
