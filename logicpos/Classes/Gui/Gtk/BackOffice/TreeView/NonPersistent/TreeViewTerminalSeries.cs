using Gtk;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.App;
using logicpos.shared.App;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewTerminalSeries : GenericTreeViewDataTable
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewTerminalSeries() { }

        public TreeViewTerminalSeries(Window pSourceWindow)
            : this(pSourceWindow, null, null) { }

        //DataTable Mode
        public TreeViewTerminalSeries(Window pSourceWindow, DataRow pDefaultValue, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            DataRow defaultValue = (pDefaultValue != null) ? pDefaultValue : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                /*00*/
                new GenericTreeViewColumnProperty("Oid") { Type = typeof(Guid), Visible = false },
                /*01*/
                new GenericTreeViewColumnProperty("Code") { Type = typeof(uint), Title = resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_code") },
                /*02*/
                new GenericTreeViewColumnProperty("Designation") { Type = typeof(string), Title = resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_designation"), Expand = true },
                /*03*/
                new GenericTreeViewColumnProperty("HardwareId") { Type = typeof(string), Title = resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_hardware_id"), MinWidth = 200 }
            };

            //init DataTable
            DataTable dataTable = GetDataTable(columnProperties);

            //Call Base Initializer
            base.InitObject(
              pSourceWindow,                    //Pass parameter 
              pDefaultValue,                    //Pass parameter 
              pGenericTreeViewMode,             //Pass parameter 
              pGenericTreeViewNavigatorMode,    //Pass parameter 
              columnProperties,                 //Created Here
              dataTable,                        //Created Here
              typeDialogClass                   //Created Here
            );
        }

        private DataTable GetDataTable(List<GenericTreeViewColumnProperty> pColumnProperties)
        {
            //Init Local Vars
            string sql = @"
                SELECT 
                    Oid,Code,Designation,HardwareId
                FROM 
                    pos_configurationplaceterminal 
                WHERE 
                    (Disabled IS NULL OR Disabled  <> 1)
                ORDER BY 
                    Ord;
            ";
            DataTable resultDataTable = SharedUtils.GetDataTableFromQuery(sql);

            return resultDataTable;
        }
    }
}
