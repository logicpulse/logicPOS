﻿using DevExpress.Data.Filtering;
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
using logicpos.Classes.Gui.Gtk.BackOffice.Dialogs.Articles;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewArticleWarehouse : GenericTreeViewXPO
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
            int fontGenericTreeViewColumn = Convert.ToInt16(GlobalFramework.Settings["fontGenericTreeViewColumn"]);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();

            columnProperties.Add(new GenericTreeViewColumnProperty("Warehouse") { ChildName = "Designation", Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_warehouse"), Expand = true });


            columnProperties.Add(new GenericTreeViewColumnProperty("Location") { ChildName = "Designation", Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_ConfigurationDevice_PlaceTerminal"), Expand = true });


            columnProperties.Add(new GenericTreeViewColumnProperty("Article") { ChildName = "Designation", Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), Expand = true });


            columnProperties.Add(new GenericTreeViewColumnProperty("ArticleSerialNumber") {ChildName = "SerialNumber", Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_serial_number"), Expand = true });

            columnProperties.Add(new GenericTreeViewColumnProperty("Quantity") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_quantity"), Expand = true });

            columnProperties.Add(new GenericTreeViewColumnProperty("UpdatedAt") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 });

            //Configure Criteria/XPCollection/Model
            //Default Criteria with XpoOidUndefinedRecord
            CriteriaOperator criteria = pXpoCriteria;
            // Override Criteria adding XpoOidHiddenRecordsFilter
            criteria = CriteriaOperator.Parse($"({pXpoCriteria.ToString()}) AND (Disabled = 0 OR Disabled IS NULL) AND (Quantity <> 0) AND (Oid NOT LIKE '{SettingsApp.XpoOidHiddenRecordsFilter}')");
            //Custom Criteria hidding all Hidden Oids
            //CriteriaOperator criteria = CriteriaOperator.Parse($"(Oid = '{SettingsApp.XpoOidUndefinedRecord}' OR Oid NOT LIKE '{SettingsApp.XpoOidHiddenRecordsFilter}')");

            int TopReturnedObj = SettingsApp.PaginationRowsPerPage;
            if (pSourceWindow.Title != Utils.GetWindowTitle(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_document_finance_page3") + " - " + resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_stock_movements")))
            {
                TopReturnedObj = 100000000;
            }

            XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria) { TopReturnedObjects = TopReturnedObj * base.CurrentPageNumber };
            var sortingCollection = new SortingCollection();
            sortingCollection.Add(new SortProperty("Warehouse", DevExpress.Xpo.DB.SortingDirection.Ascending));
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
