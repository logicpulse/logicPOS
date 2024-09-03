using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using System;
using System.Collections.Generic;
using LogicPOS.Globalization;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewConfigurationPreferenceParameter : XpoGridView
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewConfigurationPreferenceParameter() { }

        [Obsolete]
        public TreeViewConfigurationPreferenceParameter(Window parentWindow)
            : this(parentWindow, null, null, null) { }

        //XpoMode
        [Obsolete]
        public TreeViewConfigurationPreferenceParameter(Window parentWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.Default, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(cfg_configurationpreferenceparameter);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            cfg_configurationpreferenceparameter defaultValue = (pDefaultValue != null) ? pDefaultValue as cfg_configurationpreferenceparameter : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogConfigurationPreferenceParameter);

            //Configure columnProperties
            List<GridViewColumn> columnProperties = new List<GridViewColumn>
            {
                new GridViewColumn("ResourceString") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true, ResourceString = true },
                new GridViewColumn("Value") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_value"), Expand = true },
                new GridViewColumn("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            //Configure Criteria/XPCollection/Model : pXpoCriteria Parameter sent by BO
            CriteriaOperator criteria = pXpoCriteria;
            if (pXpoCriteria != null)
            {
                criteria = CriteriaOperator.Parse($"({pXpoCriteria}) AND (DeletedAt IS NULL)");
            }
            else
            {
                criteria = CriteriaOperator.Parse($"(DeletedAt IS NULL)");
            }
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria);

            this.RecordAfterUpdate += TreeViewConfigurationPreferenceParameter_RecordAfterUpdate;

            //Call Base Initializer
            base.InitObject(
              parentWindow,                //Pass parameter 
              defaultValue,                 //Pass parameter
              pGenericTreeViewMode,         //Pass parameter
              navigatorMode,//Pass parameter
              columnProperties,             //Created Here
              xpoCollection,                //Created Here
              typeDialogClass               //Created Here
            );
        }

        private void TreeViewConfigurationPreferenceParameter_RecordAfterUpdate(object sender, EventArgs e)
        {
            cfg_configurationpreferenceparameter configurationPreferenceParameter = (Entity as cfg_configurationpreferenceparameter);

            if (LogicPOS.Settings.GeneralSettings.PreferenceParameters[configurationPreferenceParameter.Token] == null ||
                !LogicPOS.Settings.GeneralSettings.PreferenceParameters[configurationPreferenceParameter.Token].Equals(configurationPreferenceParameter.Value)
                )
            {
                LogicPOS.Settings.GeneralSettings.PreferenceParameters[configurationPreferenceParameter.Token] = configurationPreferenceParameter.Value;
            }

        }
    }
}
