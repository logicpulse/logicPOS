using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewConfigurationPreferenceParameter : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewConfigurationPreferenceParameter() { }

        public TreeViewConfigurationPreferenceParameter(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewConfigurationPreferenceParameter(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(CFG_ConfigurationPreferenceParameter);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            CFG_ConfigurationPreferenceParameter defaultValue = (pDefaultValue != null) ? pDefaultValue as CFG_ConfigurationPreferenceParameter : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogConfigurationPreferenceParameter);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("ResourceString") { Title = Resx.global_designation });
            columnProperties.Add(new GenericTreeViewColumnProperty("Value") { Title = Resx.global_value });

            //Configure Criteria/XPCollection/Model
            CriteriaOperator criteria = CriteriaOperator.Parse("Token <> 'COMPANY_COUNTRY_OID' AND Token <> 'SYSTEM_CURRENCY_OID'");
            XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria);

            //Call Base Initializer
            base.InitObject(
              pSourceWindow,                //Pass parameter 
              defaultValue,                 //Pass parameter
              pGenericTreeViewMode,         //Pass parameter
              pGenericTreeViewNavigatorMode,//Pass parameter
              columnProperties,             //Created Here
              xpoCollection,                //Created Here
              typeDialogClass               //Created Here
            );
        }
    }
}
