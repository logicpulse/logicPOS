using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Collections.Generic;
using logicpos.Classes.Enums.GenericTreeView;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewCustomer : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewCustomer() { }

        public TreeViewCustomer(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewCustomer(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(erp_customer);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            erp_customer defaultValue = (pDefaultValue != null) ? pDefaultValue as erp_customer : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogCustomer);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("Code") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_code"), MinWidth = 100 });
            columnProperties.Add(new GenericTreeViewColumnProperty("Name") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_name"), MinWidth = 200, Expand = true });
            columnProperties.Add(new GenericTreeViewColumnProperty("FiscalNumber") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_fiscal_number"), MinWidth = 150 });
            columnProperties.Add(new GenericTreeViewColumnProperty("CardNumber") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_card_number"), MinWidth = 150 });
            columnProperties.Add(new GenericTreeViewColumnProperty("UpdatedAt") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 });

            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");
            CriteriaOperator criteria = pXpoCriteria;
            /* IN009162 - customer sorting changes when "PosDocumentFinanceDialog" window */
            string sortedColumn = "Ord"; // Default one
            if ("PosDocumentFinanceDialog".Equals(pSourceWindow.GetType().Name))
            {
                sortedColumn = "UpdatedAt";
            }

            SortProperty sortPropertyForCustomer = new SortProperty(sortedColumn, DevExpress.Xpo.DB.SortingDirection.Descending);
            XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria, sortPropertyForCustomer);

            //Custom Events
            //WIP: this.CursorChanged += TreeViewCustomer_CursorChanged;

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

            //Protected Records
            ProtectedRecords.Add(SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity);//FinalConsumerEntity
        }
    }
}
