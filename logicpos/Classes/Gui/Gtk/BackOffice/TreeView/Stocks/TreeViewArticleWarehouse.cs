using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.BackOffice.Dialogs.Articles;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewArticleWarehouse : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewArticleWarehouse() { }

        public TreeViewArticleWarehouse(Window pSourceWindow)
            : this(pSourceWindow, null, null, null, GenericTreeViewMode.Default, GenericTreeViewNavigatorMode.Default) { }

        //XpoMode
        public TreeViewArticleWarehouse(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.CheckBox, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_articlewarehouse);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_articlewarehouse defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_articlewarehouse : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogArticleWarehouse);

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(LogicPOS.Settings.GeneralSettings.Settings["fontGenericTreeViewColumn"]);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("Warehouse") { ChildName = "Designation", Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "global_warehouse"), Expand = true },
                new GenericTreeViewColumnProperty("Location") { ChildName = "Designation", Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "global_ConfigurationDevice_PlaceTerminal"), Expand = true },
                new GenericTreeViewColumnProperty("Article") { ChildName = "Designation", Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "global_designation"), Expand = true },
                new GenericTreeViewColumnProperty("ArticleSerialNumber") { ChildName = "SerialNumber", Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "global_serial_number"), Expand = true },
                new GenericTreeViewColumnProperty("Quantity") { Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "global_quantity"), Expand = true },
                new GenericTreeViewColumnProperty("UpdatedAt") { Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };
            //Configure Criteria/XPCollection/Model
            //Default Criteria with XpoOidUndefinedRecord
            // Override Criteria adding XpoOidHiddenRecordsFilter
            CriteriaOperator criteria = CriteriaOperator.Parse($"({pXpoCriteria}) AND (Disabled = 0 OR Disabled IS NULL) AND (Quantity <> 0) AND (Oid NOT LIKE '{shared.App.SharedSettings.XpoOidHiddenRecordsFilter}')");
            //Custom Criteria hidding all Hidden Oids
            //CriteriaOperator criteria = CriteriaOperator.Parse($"(Oid = '{SettingsApp.XpoOidUndefinedRecord}' OR Oid NOT LIKE '{SettingsApp.XpoOidHiddenRecordsFilter}')");

            int TopReturnedObj = POSSettings.PaginationRowsPerPage;
            if (pSourceWindow.Title != logicpos.Utils.GetWindowTitle(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "window_title_dialog_document_finance_page3") + " - " + resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "global_stock_movements")))
            {
                TopReturnedObj = 100000000;
            }

            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria) { TopReturnedObjects = TopReturnedObj * CurrentPageNumber };
            var sortingCollection = new SortingCollection
            {
                new SortProperty("Warehouse", DevExpress.Xpo.DB.SortingDirection.Ascending)
            };
            xpoCollection.Sorting = sortingCollection;

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
