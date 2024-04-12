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
    internal class TreeViewConfigurationPreferenceParameter : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewConfigurationPreferenceParameter() { }

        public TreeViewConfigurationPreferenceParameter(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewConfigurationPreferenceParameter(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(cfg_configurationpreferenceparameter);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            cfg_configurationpreferenceparameter defaultValue = (pDefaultValue != null) ? pDefaultValue as cfg_configurationpreferenceparameter : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogConfigurationPreferenceParameter);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("ResourceString") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), Expand = true, ResourceString = true },
                new GenericTreeViewColumnProperty("Value") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_value"), Expand = true },
                new GenericTreeViewColumnProperty("UpdatedAt") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
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
            XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria);

            this.RecordAfterUpdate += TreeViewConfigurationPreferenceParameter_RecordAfterUpdate;

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

        private void TreeViewConfigurationPreferenceParameter_RecordAfterUpdate(object sender, EventArgs e)
        {
            // Get ConfigurationPreferenceParameter Reference
            cfg_configurationpreferenceparameter configurationPreferenceParameter = (_dataSourceRow as cfg_configurationpreferenceparameter);

            try
            {
                // We Must Modify GlobalFramework.PreferenceParameters after user Change Value, if Value is Changed, this will Update in Memory GlobalFramework.PreferenceParameters Dictionary
                if (GlobalFramework.PreferenceParameters[configurationPreferenceParameter.Token] == null ||
                    !GlobalFramework.PreferenceParameters[configurationPreferenceParameter.Token].Equals(configurationPreferenceParameter.Value)
                    )
                {
                    if (_debug) _logger.Debug($"TreeViewConfigurationPreferenceParameter: Previous Value: [{GlobalFramework.PreferenceParameters[configurationPreferenceParameter.Token]}]");
                    GlobalFramework.PreferenceParameters[configurationPreferenceParameter.Token] = configurationPreferenceParameter.Value;
                    if (_debug) _logger.Debug($"TreeViewConfigurationPreferenceParameter: Current Value: [{GlobalFramework.PreferenceParameters[configurationPreferenceParameter.Token]}]");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
