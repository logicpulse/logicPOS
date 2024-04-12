using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Formatters;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.datalayer.Enums;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewArticleSerialNumber : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewArticleSerialNumber() { }

        public TreeViewArticleSerialNumber(Window pSourceWindow)
            : this(pSourceWindow, null, null, null, GenericTreeViewMode.Default, GenericTreeViewNavigatorMode.Default) { }

        //XpoMode
        public TreeViewArticleSerialNumber(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.CheckBox, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_articleserialnumber);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_articleserialnumber defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_articleserialnumber : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(GlobalFramework.Settings["fontGenericTreeViewColumn"]);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            
            columnProperties.Add(new GenericTreeViewColumnProperty("Article") { ChildName = "Designation", Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), Expand = true, MinWidth = 200 });

            columnProperties.Add(new GenericTreeViewColumnProperty("SerialNumber") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_serial_number"), Expand = true, MinWidth = 200 });

            columnProperties.Add(new GenericTreeViewColumnProperty("IsSold") { Title = "Vendido", Expand = false });

            columnProperties.Add(new GenericTreeViewColumnProperty("Status") { Title = "Estado", Expand = true, MinWidth = 150 });

            columnProperties.Add(new GenericTreeViewColumnProperty("Article") { ChildName = "IsComposed", Title = "Artigo Composto", Expand = false });

            columnProperties.Add(new GenericTreeViewColumnProperty("StockMovimentIn") { ChildName = "Date", Title = "Data de Compra", Expand = true, FormatProvider = new FormatterDate() });

            columnProperties.Add(new GenericTreeViewColumnProperty("StockMovimentIn")
            {
                Query = "SELECT Name as Result FROM erp_customer WHERE Oid = '{0}';",                
                Title = "Fornecedor",
                DecryptValue = true,
                MinWidth = 100
            });
            
            columnProperties.Add(new GenericTreeViewColumnProperty("StockMovimentIn") { ChildName = "PurchasePrice", Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_purchase_price"), Expand = false });

            columnProperties.Add(new GenericTreeViewColumnProperty("StockMovimentIn") { ChildName = "DocumentNumber", Title = "Documento Origem", Expand = true });

            columnProperties.Add(new GenericTreeViewColumnProperty("StockMovimentOut")
            {
                Query = "SELECT DocumentNumber as Result FROM fin_documentfinancemaster WHERE Oid = '{0}';",
                Title = "Documento Venda",
                DecryptValue = false,
                MinWidth = 100
            });

            columnProperties.Add(new GenericTreeViewColumnProperty("ArticleWarehouse") { Query = "SELECT Designation as Result FROM fin_warehouse WHERE Oid = (SELECT Warehouse FROM fin_warehouselocation WHERE Oid = '{0}');", ChildName = "Warehouse",  Title = "Armazem", Expand = true });

            columnProperties.Add(new GenericTreeViewColumnProperty("ArticleWarehouse") { Query = "SELECT Designation as Result FROM fin_warehouselocation WHERE Oid = '{0}';", ChildName = "Location", Title = "Localização", Expand = true });

            columnProperties.Add(new GenericTreeViewColumnProperty("UpdatedAt") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 });

            //Configure Criteria/XPCollection/Model
            //Default Criteria with XpoOidUndefinedRecord
            CriteriaOperator criteria = pXpoCriteria;


            // Override Criteria adding XpoOidHiddenRecordsFilter
            if (pXpoCriteria != null)
            {
                criteria = CriteriaOperator.Parse($"({pXpoCriteria.ToString()}) AND (DeletedAt IS NULL)");
            }
            else
            {
                criteria = CriteriaOperator.Parse($"(DeletedAt IS NULL)");
            }
            //Custom Criteria hidding all Hidden Oids
            //CriteriaOperator criteria = CriteriaOperator.Parse($"(Oid = '{SettingsApp.XpoOidUndefinedRecord}' OR Oid NOT LIKE '{SettingsApp.XpoOidHiddenRecordsFilter}')");

            var sortingCollection = new SortingCollection();
            sortingCollection.Add(new SortProperty("UpdatedAt", DevExpress.Xpo.DB.SortingDirection.Descending));

            int TopReturnedObj = 50;
            if(pSourceWindow.Title != logicpos.Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_document_finance_page3") + " - " + resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_stock_movements")))
            {
                TopReturnedObj = 100000000;
            }

            XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria) { TopReturnedObjects = TopReturnedObj, Sorting = sortingCollection };         

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
