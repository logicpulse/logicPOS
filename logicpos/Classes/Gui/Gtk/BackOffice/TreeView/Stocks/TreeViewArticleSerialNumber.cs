using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Formatters;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.datalayer.Xpo;
using System;
using System.Collections.Generic;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewArticleSerialNumber : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewArticleSerialNumber() { }

        [Obsolete]
        public TreeViewArticleSerialNumber(Window pSourceWindow)
            : this(pSourceWindow, null, null, null, GenericTreeViewMode.Default, GenericTreeViewNavigatorMode.Default) { }

        //XpoMode
        [Obsolete]
        public TreeViewArticleSerialNumber(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.CheckBox, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_articleserialnumber);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_articleserialnumber defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_articleserialnumber : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(LogicPOS.Settings.GeneralSettings.Settings["fontGenericTreeViewColumn"]);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("Article") { ChildName = "Designation", Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true, MinWidth = 200 },
                new GenericTreeViewColumnProperty("SerialNumber") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_serial_number"), Expand = true, MinWidth = 200 },
                new GenericTreeViewColumnProperty("IsSold") { Title = "Vendido", Expand = false },
                new GenericTreeViewColumnProperty("Status") { Title = "Estado", Expand = true, MinWidth = 150 },
                new GenericTreeViewColumnProperty("Article") { ChildName = "IsComposed", Title = "Artigo Composto", Expand = false },
                new GenericTreeViewColumnProperty("StockMovimentIn") { ChildName = "Date", Title = "Data de Compra", Expand = true, FormatProvider = new FormatterDate() },
                new GenericTreeViewColumnProperty("StockMovimentIn")
                {
                    Query = "SELECT Name as Result FROM erp_customer WHERE Oid = '{0}';",
                    Title = "Fornecedor",
                    DecryptValue = true,
                    MinWidth = 100
                },
                new GenericTreeViewColumnProperty("StockMovimentIn") { ChildName = "PurchasePrice", Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_purchase_price"), Expand = false },
                new GenericTreeViewColumnProperty("StockMovimentIn") { ChildName = "DocumentNumber", Title = "Documento Origem", Expand = true },
                new GenericTreeViewColumnProperty("StockMovimentOut")
                {
                    Query = "SELECT DocumentNumber as Result FROM fin_documentfinancemaster WHERE Oid = '{0}';",
                    Title = "Documento Venda",
                    DecryptValue = false,
                    MinWidth = 100
                },
                new GenericTreeViewColumnProperty("ArticleWarehouse") { Query = "SELECT Designation as Result FROM fin_warehouse WHERE Oid = (SELECT Warehouse FROM fin_warehouselocation WHERE Oid = '{0}');", ChildName = "Warehouse", Title = "Armazem", Expand = true },
                new GenericTreeViewColumnProperty("ArticleWarehouse") { Query = "SELECT Designation as Result FROM fin_warehouselocation WHERE Oid = '{0}';", ChildName = "Location", Title = "Localização", Expand = true },
                new GenericTreeViewColumnProperty("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
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
            if (pSourceWindow.Title != logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_document_finance_page3") + " - " + CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_stock_movements")))
            {
                TopReturnedObj = 100000000;
            }

            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria) { TopReturnedObjects = TopReturnedObj, Sorting = sortingCollection };

            //Call Base Initializer
            base.InitObject(
              pSourceWindow,                  //Pass parameter 
              defaultValue,                   //Pass parameter
              pGenericTreeViewMode,           //Pass parameter
              pGenericTreeViewNavigatorMode,  //Pass parameter
              columnProperties,               //Created Here
              xpoCollection,                  //Created Here
              typeDialogClass                 //Created Here
            );
        }
    }
}
