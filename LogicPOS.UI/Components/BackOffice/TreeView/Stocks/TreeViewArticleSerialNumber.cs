using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Formatters;
using System;
using System.Collections.Generic;
using LogicPOS.Globalization;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewArticleSerialNumber : XpoGridView
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewArticleSerialNumber() { }

        [Obsolete]
        public TreeViewArticleSerialNumber(Window parentWindow)
            : this(parentWindow, null, null, null, GridViewMode.Default, GridViewNavigatorMode.Default) { }

        //XpoMode
        [Obsolete]
        public TreeViewArticleSerialNumber(Window parentWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.CheckBox, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_articleserialnumber);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_articleserialnumber defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_articleserialnumber : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(LogicPOS.Settings.AppSettings.Instance.fontGenericTreeViewColumn);

            //Configure columnProperties
            List<GridViewColumnProperty> columnProperties = new List<GridViewColumnProperty>
            {
                new GridViewColumnProperty("Article") { ChildName = "Designation", Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true, MinWidth = 200 },
                new GridViewColumnProperty("SerialNumber") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_serial_number"), Expand = true, MinWidth = 200 },
                new GridViewColumnProperty("IsSold") { Title = "Vendido", Expand = false },
                new GridViewColumnProperty("Status") { Title = "Estado", Expand = true, MinWidth = 150 },
                new GridViewColumnProperty("Article") { ChildName = "IsComposed", Title = "Artigo Composto", Expand = false },
                new GridViewColumnProperty("StockMovimentIn") { ChildName = "Date", Title = "Data de Compra", Expand = true, FormatProvider = new DateFormatter() },
                new GridViewColumnProperty("StockMovimentIn")
                {
                    Query = "SELECT Name as Result FROM erp_customer WHERE Oid = '{0}';",
                    Title = "Fornecedor",
                    DecryptValue = true,
                    MinWidth = 100
                },
                new GridViewColumnProperty("StockMovimentIn") { ChildName = "PurchasePrice", Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_purchase_price"), Expand = false },
                new GridViewColumnProperty("StockMovimentIn") { ChildName = "DocumentNumber", Title = "Documento Origem", Expand = true },
                new GridViewColumnProperty("StockMovimentOut")
                {
                    Query = "SELECT DocumentNumber as Result FROM fin_documentfinancemaster WHERE Oid = '{0}';",
                    Title = "Documento Venda",
                    DecryptValue = false,
                    MinWidth = 100
                },
                new GridViewColumnProperty("ArticleWarehouse") { Query = "SELECT Designation as Result FROM fin_warehouse WHERE Oid = (SELECT Warehouse FROM fin_warehouselocation WHERE Oid = '{0}');", ChildName = "Warehouse", Title = "Armazem", Expand = true },
                new GridViewColumnProperty("ArticleWarehouse") { Query = "SELECT Designation as Result FROM fin_warehouselocation WHERE Oid = '{0}';", ChildName = "Location", Title = "Localização", Expand = true },
                new GridViewColumnProperty("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            //Configure Criteria/XPCollection/Model
            //Default Criteria with XpoOidUndefinedRecord
            CriteriaOperator criteria;


            // Override Criteria adding XpoOidHiddenRecordsFilter
            if (pXpoCriteria != null)
            {
                criteria = CriteriaOperator.Parse($"({pXpoCriteria}) AND (DeletedAt IS NULL)");
            }
            else
            {
                criteria = CriteriaOperator.Parse($"(DeletedAt IS NULL)");
            }
            //Custom Criteria hidding all Hidden Oids
            //CriteriaOperator criteria = CriteriaOperator.Parse($"(Oid = '{SettingsApp.XpoOidUndefinedRecord}' OR Oid NOT LIKE '{SettingsApp.XpoOidHiddenRecordsFilter}')");

            var sortingCollection = new SortingCollection
            {
                new SortProperty("UpdatedAt", DevExpress.Xpo.DB.SortingDirection.Descending)
            };

            int TopReturnedObj = 50;
            if (parentWindow.Title != logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_document_finance_page3") + " - " + CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_stock_movements")))
            {
                TopReturnedObj = 100000000;
            }

            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria) { TopReturnedObjects = TopReturnedObj, Sorting = sortingCollection };

            //Call Base Initializer
            base.InitObject(
              parentWindow,                  //Pass parameter 
              defaultValue,                   //Pass parameter
              pGenericTreeViewMode,           //Pass parameter
              navigatorMode,  //Pass parameter
              columnProperties,               //Created Here
              xpoCollection,                  //Created Here
              typeDialogClass                 //Created Here
            );
        }
    }
}
