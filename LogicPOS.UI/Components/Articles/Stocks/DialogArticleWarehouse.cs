using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using System;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Globalization;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components;
using LogicPOS.UI.Application;

namespace logicpos.Classes.Gui.Gtk.BackOffice.Dialogs.Articles
{
    internal class DialogArticleWarehouse : EditDialog
    {
        private fin_article _selectedArticle;
        private XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> _articleBoxSelectRecord;
        private XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber> _entryBoxArticleSerialNumber;
        private BOWidgetBox boxQuantity;
        private Entry entryQuantity;
        private XPOComboBox xpoComboBoxWarehouse;
        private XPOComboBox xpoComboBoxWarehouseLocation;
        private bool _modifyArticle;

        public IconButtonWithText ButtonInsert { get; set; }
        protected GridViewNavigator<fin_article, TreeViewArticle> _navigator;

        public DialogArticleWarehouse(Window parentWindow, XpoGridView pTreeView, DialogFlags pDialogFlags, DialogMode dialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pDialogFlags, dialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_warehose_management"));

            if (LogicPOSAppContext.ScreenSize.Width == 800 && LogicPOSAppContext.ScreenSize.Height == 600)
            {
                SetSizeRequest(320, 250);
            }
            else
            {
                SetSizeRequest(320, 450);
            }
            _modifyArticle = true;
            if (dialogMode == DialogMode.Update && Entity != null && Entity.GetType() == typeof(fin_articlewarehouse) && (Entity as fin_articlewarehouse).ArticleSerialNumber != null)
            {
                _modifyArticle = false;
            }
            InitUI();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                ButtonOk.Sensitive = false;

                if (Entity != null && Entity.GetType() == typeof(fin_articleserialnumber))
                {
                    fin_articlewarehouse selectedArticleWarehouse = (Entity as fin_articleserialnumber).ArticleWarehouse;
                    Entity = selectedArticleWarehouse;
                    _modifyArticle = false;
                }


                //Articles
                CriteriaOperator articleCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
                _articleBoxSelectRecord = new XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>(this, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_article"), "Designation", "Oid", (Entity as fin_articlewarehouse).Article, articleCriteria, LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, true, true);
                GenericCRUDWidgetXPO genericCRUDWidgetXPO = new GenericCRUDWidgetXPO(_articleBoxSelectRecord, Entity, "Article", "", true);
                InputFields.Add(genericCRUDWidgetXPO);
                _articleBoxSelectRecord.EntryValidation.IsEditable = true;
                _articleBoxSelectRecord.EntryValidation.Completion.PopupCompletion = true;
                _articleBoxSelectRecord.EntryValidation.Completion.InlineCompletion = false;
                _articleBoxSelectRecord.EntryValidation.Completion.PopupSingleMatch = true;
                _articleBoxSelectRecord.EntryValidation.Completion.InlineSelection = true;
                _articleBoxSelectRecord.Sensitive = _modifyArticle;
                vboxTab1.PackStart(_articleBoxSelectRecord, false, false, 0);

                //SerialNumber
                CriteriaOperator serialNumberCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
                if ((Entity as fin_articlewarehouse).Article != null)
                {
                    serialNumberCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND Article == '{0}'", (Entity as fin_articlewarehouse).Article.Oid));
                }

                _entryBoxArticleSerialNumber = new XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>(this, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_serialnumber"), "SerialNumber", "Oid", (Entity as fin_articlewarehouse).ArticleSerialNumber, serialNumberCriteria, LogicPOS.Utility.RegexUtils.RegexGuid, true, true);
                _entryBoxArticleSerialNumber.EntryValidation.IsEditable = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.PopupCompletion = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.InlineCompletion = false;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.PopupSingleMatch = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.InlineSelection = true;
                //if (_entryBoxArticleSerialNumber.Value != null) _entryBoxArticleSerialNumber.EntryValidation.Changed += EntrySerialNumberValidation_Changed;
                genericCRUDWidgetXPO = new GenericCRUDWidgetXPO(_entryBoxArticleSerialNumber, Entity, "ArticleSerialNumber", "", false);
                InputFields.Add(genericCRUDWidgetXPO);
                _entryBoxArticleSerialNumber.Sensitive = false;
                vboxTab1.PackStart(_entryBoxArticleSerialNumber, false, false, 0);

                //Warehouse
                CriteriaOperator defaultWarehouseCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND IsDefault == '1'"));
                fin_warehouse defaultWareHouse = ((Entity as fin_articlewarehouse).Warehouse != null) ? (Entity as fin_articlewarehouse).Warehouse : (fin_warehouse)Entity.Session.FindObject(typeof(fin_warehouse), defaultWarehouseCriteria);
                xpoComboBoxWarehouse = new XPOComboBox(Entity.Session, typeof(fin_warehouse), defaultWareHouse, "Designation", null);
                BOWidgetBox boxWareHouse = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_warehouse"), xpoComboBoxWarehouse);
                genericCRUDWidgetXPO = new GenericCRUDWidgetXPO(boxWareHouse, Entity, "Warehouse", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false);
                InputFields.Add(genericCRUDWidgetXPO);
                vboxTab1.PackStart(boxWareHouse, false, false, 0);

                //Location
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
                if (defaultWareHouse != null)
                {
                    criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND Warehouse == '{0}'", defaultWareHouse.Oid.ToString()));
                }
                fin_warehouselocation defaultLocation = ((Entity as fin_articlewarehouse).Location != null) ? (Entity as fin_articlewarehouse).Location : (fin_warehouselocation)Entity.Session.FindObject(typeof(fin_warehouselocation), criteria);
                xpoComboBoxWarehouseLocation = new XPOComboBox(Entity.Session, typeof(fin_warehouselocation), defaultLocation, "Designation", criteria);
                BOWidgetBox boxWareHouseLocation = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationDevice_PlaceTerminal"), xpoComboBoxWarehouseLocation);
                genericCRUDWidgetXPO = new GenericCRUDWidgetXPO(boxWareHouseLocation, Entity, "Location", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false);
                InputFields.Add(genericCRUDWidgetXPO);
                vboxTab1.PackStart(boxWareHouseLocation, false, false, 0);


                entryQuantity = new Entry();
                boxQuantity = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_quantity"), entryQuantity);
                vboxTab1.PackStart(boxQuantity, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxQuantity, Entity, "Quantity", LogicPOS.Utility.RegexUtils.RegexDecimal, false));


                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_article_location")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                if ((Entity as fin_articlewarehouse).ArticleSerialNumber != null && ((this._dialogMode.Equals(DialogMode.View) || (Entity as fin_articlewarehouse).ArticleSerialNumber.IsSold == true)))
                {
                    _articleBoxSelectRecord.Sensitive = false;
                    _entryBoxArticleSerialNumber.Sensitive = false;
                    xpoComboBoxWarehouse.Sensitive = false;
                    xpoComboBoxWarehouseLocation.Sensitive = false;
                    entryQuantity.Sensitive = false;
                }

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                //Events
                _selectedArticle = _articleBoxSelectRecord.Value as fin_article;
                _articleBoxSelectRecord.EntryValidation.Changed += XpoComboBoxArticle_Changed;
                xpoComboBoxWarehouseLocation.Changed += delegate { ValidateDialog(); };
                xpoComboBoxWarehouse.Changed += XpoComboBoxWarehouse_Changed;
                _entryBoxArticleSerialNumber.EntryValidation.Changed += XpoComboBoxArticleSerialNumber_Changed;
                entryQuantity.Changed += delegate { ValidateDialog(); };
                DefineInitialValues();
                ValidateDialog();

            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void XpoComboBoxWarehouse_Changed(object sender, EventArgs e)
        {
            try
            {
                if (xpoComboBoxWarehouse.Value == null)
                {
                    xpoComboBoxWarehouseLocation.Sensitive = false;
                    ValidateDialog();
                    return;
                }
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND Warehouse == '{0}'", xpoComboBoxWarehouse.Value.Oid.ToString()));
                //var xpCollection = new XPCollection(_dataSourceRow.Session, typeof(fin_warehouselocation));
                xpoComboBoxWarehouseLocation.UpdateModel(criteria, null);
                xpoComboBoxWarehouseLocation.Sensitive = true;
                ValidateDialog();
                return;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public void DefineInitialValues()
        {
            if (Entity != null && Entity.Oid != Guid.Empty && (Entity as fin_articlewarehouse).ArticleSerialNumber != null && !string.IsNullOrEmpty((Entity as fin_articlewarehouse).ArticleSerialNumber.SerialNumber) && _selectedArticle != null && _selectedArticle.ArticleSerialNumber.Count > 0)
            {
                SortProperty sortProperty = new SortProperty("SerialNumber", DevExpress.Xpo.DB.SortingDirection.Ascending);
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("Article = '{0}'", _selectedArticle.Oid));
                string sql = string.Format("SELECT OID FROM fin_articleserialnumber where SerialNumber = '{0}';", (Entity as fin_articlewarehouse).ArticleSerialNumber.SerialNumber);
                string serialNumberOid = Entity.Session.ExecuteScalar(sql).ToString();
                var articleSerialNumber = (fin_articleserialnumber)Entity.Session.GetObjectByKey(typeof(fin_articleserialnumber), Guid.Parse(serialNumberOid));
                _entryBoxArticleSerialNumber.CriteriaOperator = criteria;/*.UpdateModel(criteria, articleSerialNumber, sortProperty);*/
                _entryBoxArticleSerialNumber.Sensitive = _modifyArticle;
        

                ValidateDialog();
            }
            if(_selectedArticle != null)
            {
                string stockQuery = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;", _selectedArticle.Oid);
                boxQuantity.TooltipText = Entity.Session.ExecuteScalar(stockQuery).ToString();
                _selectedArticle.Accounting = Convert.ToDecimal(boxQuantity.TooltipText);
                boxQuantity.LabelComponent.Text = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_quantity") + " :: Total em Stock: " + _selectedArticle.Accounting.ToString());
            }
            if (xpoComboBoxWarehouse.Value == null)
            {
                xpoComboBoxWarehouseLocation.Sensitive = false;
            }
        }

        private void XpoComboBoxArticleSerialNumber_Changed(object sender, EventArgs e)
        {
            if (_entryBoxArticleSerialNumber.Value != null)
            {
                entryQuantity.Text = "1";
                entryQuantity.Sensitive = false;
            }
            else
            {
                entryQuantity.Text = "0";
                entryQuantity.Sensitive = true;
            }
            ValidateDialog();
            //CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("Family = '{0}'", currentOid));
            //_xpoComboBoxSubFamily.UpdateModel(criteria);
        }

        private void ValidateDialog()
        {
            try
            {
                if (_selectedArticle == null || entryQuantity.Text == "0" || Convert.ToDecimal(entryQuantity.Text) > _selectedArticle.Accounting
                    || xpoComboBoxWarehouseLocation.Value == null || xpoComboBoxWarehouse.Value == null)
                {
                    ButtonOk.Sensitive = false;
                }
                else
                {
                    ButtonOk.Sensitive = true;
                }

                if (_selectedArticle != null && _entryBoxArticleSerialNumber.Value != null && Convert.ToDecimal(entryQuantity.Text) > 1)
                {
                    ButtonOk.Sensitive = false;
                }

            }
            catch (System.Exception)
            {
                //_logger.Error(ex.Message, ex);
            }
        }


        private void XpoComboBoxArticle_Changed(object sender, EventArgs e)
        {
            try
            {
                ValidatableTextBox entryValidation = (ValidatableTextBox)sender;
                _selectedArticle = _articleBoxSelectRecord.Value;
                boxQuantity.TooltipText = _selectedArticle.Accounting.ToString();
                boxQuantity.LabelComponent.Text = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_quantity") + " :: Total em Stock: " + _selectedArticle.Accounting.ToString());

                if (_selectedArticle != null)
                {
                    string stockQuery = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;", _selectedArticle.Oid);
                    boxQuantity.TooltipText = Entity.Session.ExecuteScalar(stockQuery).ToString();
                    _selectedArticle.Accounting = Convert.ToDecimal(boxQuantity.TooltipText);
                    boxQuantity.LabelComponent.Text = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_quantity") + " :: Total em Stock: " + _selectedArticle.Accounting.ToString());
                }

                ValidateDialog();

                //Não é possivel adicionar serialNumbers nesta janela por enquanto
                _entryBoxArticleSerialNumber.Value = null;
                _entryBoxArticleSerialNumber.Sensitive = false;
                return;

                //if (_selectedArticle.ArticleSerialNumber.Count > 0)
                //{
                //    Guid currentOid = Guid.Empty;
                //    if (_selectedArticle != null)
                //    {
                //        currentOid = _selectedArticle.Oid;

                //        //SortProperty sortProperty = new SortProperty("Article", DevExpress.Xpo.DB.SortingDirection.Ascending);
                //        CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("Article = '{0}'", currentOid));
                //        _entryBoxArticleSerialNumber.CriteriaOperator = criteria;
                //        _entryBoxArticleSerialNumber.Sensitive = true;
                //        return;
                //    }
                //}
                //else
                //{
                //    _entryBoxArticleSerialNumber.Value = null;
                //    _entryBoxArticleSerialNumber.Sensitive = false;
                //}


            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    }
}

