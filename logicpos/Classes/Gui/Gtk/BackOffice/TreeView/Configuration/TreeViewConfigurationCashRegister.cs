using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using logicpos.Classes.Enums.GenericTreeView;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewConfigurationCashRegister : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewConfigurationCashRegister() { }

        public TreeViewConfigurationCashRegister(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewConfigurationCashRegister(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(POS_ConfigurationCashRegister);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            POS_ConfigurationCashRegister defaultValue = (pDefaultValue != null) ? pDefaultValue as POS_ConfigurationCashRegister : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogConfigurationCashRegister);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("Code") { Title = Resx.global_record_code, MinWidth = 100 });
            columnProperties.Add(new GenericTreeViewColumnProperty("Designation") { Title = Resx.global_designation_Drawer, Expand = true });
            columnProperties.Add(new GenericTreeViewColumnProperty("Drawer") { Title = Resx.global_ConfigurationCashRegister });
            columnProperties.Add(new GenericTreeViewColumnProperty("AutomaticDrawer") { Title = Resx.global_ConfigurationCashRegister_AutomaticDrawer });
            columnProperties.Add(new GenericTreeViewColumnProperty("ActiveSales") { Title = Resx.global_ConfigurationCashRegister_ActiveSales });
            columnProperties.Add(new GenericTreeViewColumnProperty("AllowChargeBacks") { Title = Resx.global_ConfigurationCashRegister_AllowChargeBacks });
            columnProperties.Add(new GenericTreeViewColumnProperty("UpdatedAt") { Title = Resx.global_record_date_updated, MinWidth = 150, MaxWidth = 150 });

            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");
            CriteriaOperator criteria = pXpoCriteria;
            XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria);

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
