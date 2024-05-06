using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using System;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.datalayer.DataLayer.Xpo.Documents;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

namespace logicpos.Classes.Gui.Gtk.BackOffice.Dialogs.Articles
{
    internal class DialogArticleWarehouse : BOBaseDialog
    {
        private fin_article _selectedArticle;
        private XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle> _articleBoxSelectRecord;
        private XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber> _entryBoxArticleSerialNumber;
        private BOWidgetBox boxQuantity;
        private Entry entryQuantity;
        private XPOComboBox xpoComboBoxWarehouse;
        private XPOComboBox xpoComboBoxWarehouseLocation;
        private bool _modifyArticle;

        public TouchButtonIconWithText ButtonInsert { get; set; }
        protected GenericTreeViewNavigator<fin_article, TreeViewArticle> _navigator;

        public DialogArticleWarehouse(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pDialogFlags, DialogMode dialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pDialogFlags, dialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_warehose_management"));

            if (GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600)
            {
                SetSizeRequest(320, 250);
            }
            else
            {
                SetSizeRequest(320, 450);
            }
            _modifyArticle = true;
            if (dialogMode == DialogMode.Update && _dataSourceRow != null && _dataSourceRow.GetType() == typeof(fin_articlewarehouse) && (_dataSourceRow as fin_articlewarehouse).ArticleSerialNumber != null)
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

                buttonOk.Sensitive = false;

                if (_dataSourceRow != null && _dataSourceRow.GetType() == typeof(fin_articleserialnumber))
                {
                    fin_articlewarehouse selectedArticleWarehouse = (_dataSourceRow as fin_articleserialnumber).ArticleWarehouse;
                    _dataSourceRow = selectedArticleWarehouse;
                    _modifyArticle = false;
                }


                //Articles
                CriteriaOperator articleCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
                _articleBoxSelectRecord = new XPOEntryBoxSelectRecordValidation<fin_article, TreeViewArticle>(this, CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_article"), "Designation", "Oid", (_dataSourceRow as fin_articlewarehouse).Article, articleCriteria, LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, true, true);
                GenericCRUDWidgetXPO genericCRUDWidgetXPO = new GenericCRUDWidgetXPO(_articleBoxSelectRecord, _dataSourceRow, "Article", "", true);
                _crudWidgetList.Add(genericCRUDWidgetXPO);
                _articleBoxSelectRecord.EntryValidation.IsEditable = true;
                _articleBoxSelectRecord.EntryValidation.Completion.PopupCompletion = true;
                _articleBoxSelectRecord.EntryValidation.Completion.InlineCompletion = false;
                _articleBoxSelectRecord.EntryValidation.Completion.PopupSingleMatch = true;
                _articleBoxSelectRecord.EntryValidation.Completion.InlineSelection = true;
                _articleBoxSelectRecord.Sensitive = _modifyArticle;
                vboxTab1.PackStart(_articleBoxSelectRecord, false, false, 0);

                //SerialNumber
                CriteriaOperator serialNumberCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
                if ((_dataSourceRow as fin_articlewarehouse).Article != null)
                {
                    serialNumberCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND Article == '{0}'", (_dataSourceRow as fin_articlewarehouse).Article.Oid));
                }

                _entryBoxArticleSerialNumber = new XPOEntryBoxSelectRecordValidation<fin_articleserialnumber, TreeViewArticleSerialNumber>(this, CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_serialnumber"), "SerialNumber", "Oid", (_dataSourceRow as fin_articlewarehouse).ArticleSerialNumber, serialNumberCriteria, LogicPOS.Utility.RegexUtils.RegexGuid, true, true);
                _entryBoxArticleSerialNumber.EntryValidation.IsEditable = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.PopupCompletion = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.InlineCompletion = false;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.PopupSingleMatch = true;
                _entryBoxArticleSerialNumber.EntryValidation.Completion.InlineSelection = true;
                //if (_entryBoxArticleSerialNumber.Value != null) _entryBoxArticleSerialNumber.EntryValidation.Changed += EntrySerialNumberValidation_Changed;
                genericCRUDWidgetXPO = new GenericCRUDWidgetXPO(_entryBoxArticleSerialNumber, _dataSourceRow, "ArticleSerialNumber", "", false);
                _crudWidgetList.Add(genericCRUDWidgetXPO);
                _entryBoxArticleSerialNumber.Sensitive = false;
                vboxTab1.PackStart(_entryBoxArticleSerialNumber, false, false, 0);

                //Warehouse
                CriteriaOperator defaultWarehouseCriteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND IsDefault == '1'"));
                fin_warehouse defaultWareHouse = ((_dataSourceRow as fin_articlewarehouse).Warehouse != null) ? (_dataSourceRow as fin_articlewarehouse).Warehouse : (fin_warehouse)_dataSourceRow.Session.FindObject(typeof(fin_warehouse), defaultWarehouseCriteria);
                xpoComboBoxWarehouse = new XPOComboBox(DataSourceRow.Session, typeof(fin_warehouse), defaultWareHouse, "Designation", null);
                BOWidgetBox boxWareHouse = new BOWidgetBox(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_warehouse"), xpoComboBoxWarehouse);
                genericCRUDWidgetXPO = new GenericCRUDWidgetXPO(boxWareHouse, _dataSourceRow, "Warehouse", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false);
                _crudWidgetList.Add(genericCRUDWidgetXPO);
                vboxTab1.PackStart(boxWareHouse, false, false, 0);

                //Location
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
                if (defaultWareHouse != null)
                {
                    criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND Warehouse == '{0}'", defaultWareHouse.Oid.ToString()));
                }
                fin_warehouselocation defaultLocation = ((_dataSourceRow as fin_articlewarehouse).Location != null) ? (_dataSourceRow as fin_articlewarehouse).Location : (fin_warehouselocation)_dataSourceRow.Session.FindObject(typeof(fin_warehouselocation), criteria);
                xpoComboBoxWarehouseLocation = new XPOComboBox(DataSourceRow.Session, typeof(fin_warehouselocation), defaultLocation, "Designation", criteria);
                BOWidgetBox boxWareHouseLocation = new BOWidgetBox(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_ConfigurationDevice_PlaceTerminal"), xpoComboBoxWarehouseLocation);
                genericCRUDWidgetXPO = new GenericCRUDWidgetXPO(boxWareHouseLocation, _dataSourceRow, "Location", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, false);
                _crudWidgetList.Add(genericCRUDWidgetXPO);
                vboxTab1.PackStart(boxWareHouseLocation, false, false, 0);


                entryQuantity = new Entry();
                boxQuantity = new BOWidgetBox(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_quantity"), entryQuantity);
                vboxTab1.PackStart(boxQuantity, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxQuantity, _dataSourceRow, "Quantity", LogicPOS.Utility.RegexUtils.RegexDecimal, false));


                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_article_location")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                if ((_dataSourceRow as fin_articlewarehouse).ArticleSerialNumber != null && ((this._dialogMode.Equals(DialogMode.View) || (_dataSourceRow as fin_articlewarehouse).ArticleSerialNumber.IsSold == true)))
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
            if (_dataSourceRow != null && _dataSourceRow.Oid != Guid.Empty && (_dataSourceRow as fin_articlewarehouse).ArticleSerialNumber != null && !string.IsNullOrEmpty((_dataSourceRow as fin_articlewarehouse).ArticleSerialNumber.SerialNumber) && _selectedArticle != null && _selectedArticle.ArticleSerialNumber.Count > 0)
            {
                SortProperty sortProperty = new SortProperty("SerialNumber", DevExpress.Xpo.DB.SortingDirection.Ascending);
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("Article = '{0}'", _selectedArticle.Oid));
                string sql = string.Format("SELECT OID FROM fin_articleserialnumber where SerialNumber = '{0}';", (_dataSourceRow as fin_articlewarehouse).ArticleSerialNumber.SerialNumber);
                string serialNumberOid = _dataSourceRow.Session.ExecuteScalar(sql).ToString();
                var articleSerialNumber = (fin_articleserialnumber)_dataSourceRow.Session.GetObjectByKey(typeof(fin_articleserialnumber), Guid.Parse(serialNumberOid));
                _entryBoxArticleSerialNumber.CriteriaOperator = criteria;/*.UpdateModel(criteria, articleSerialNumber, sortProperty);*/
                _entryBoxArticleSerialNumber.Sensitive = _modifyArticle;
        

                ValidateDialog();
            }
            if(_selectedArticle != null)
            {
                string stockQuery = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;", _selectedArticle.Oid);
                boxQuantity.TooltipText = _dataSourceRow.Session.ExecuteScalar(stockQuery).ToString();
                _selectedArticle.Accounting = Convert.ToDecimal(boxQuantity.TooltipText);
                boxQuantity.LabelComponent.Text = string.Format(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_quantity") + " :: Total em Stock: " + _selectedArticle.Accounting.ToString());
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
                    buttonOk.Sensitive = false;
                }
                else
                {
                    buttonOk.Sensitive = true;
                }

                if (_selectedArticle != null && _entryBoxArticleSerialNumber.Value != null && Convert.ToDecimal(entryQuantity.Text) > 1)
                {
                    buttonOk.Sensitive = false;
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
                EntryValidation entryValidation = (EntryValidation)sender;
                _selectedArticle = _articleBoxSelectRecord.Value;
                boxQuantity.TooltipText = _selectedArticle.Accounting.ToString();
                boxQuantity.LabelComponent.Text = string.Format(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_quantity") + " :: Total em Stock: " + _selectedArticle.Accounting.ToString());

                if (_selectedArticle != null)
                {
                    string stockQuery = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;", _selectedArticle.Oid);
                    boxQuantity.TooltipText = _dataSourceRow.Session.ExecuteScalar(stockQuery).ToString();
                    _selectedArticle.Accounting = Convert.ToDecimal(boxQuantity.TooltipText);
                    boxQuantity.LabelComponent.Text = string.Format(CultureResources.GetLanguageResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_quantity") + " :: Total em Stock: " + _selectedArticle.Accounting.ToString());
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

