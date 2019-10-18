using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewTerminalSeries : GenericTreeViewDataTable
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
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            /*00*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Oid") { Type = typeof(Guid), Visible = false });
            /*01*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Code") { Type = typeof(UInt32), Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_code") });
            /*02*/
            columnProperties.Add(new GenericTreeViewColumnProperty("Designation") { Type = typeof(String), Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), Expand = true });
            /*03*/
            columnProperties.Add(new GenericTreeViewColumnProperty("HardwareId") { Type = typeof(String), Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_hardware_id"), MinWidth = 200 });

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
            DataTable resultDataTable = FrameworkUtils.GetDataTableFromQuery(sql);

            return resultDataTable;
        }
    }
}
