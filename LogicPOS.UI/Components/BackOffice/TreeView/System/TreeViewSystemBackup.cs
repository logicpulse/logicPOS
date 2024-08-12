using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using System;
using System.Collections.Generic;
using LogicPOS.Globalization;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewSystemBackup : XpoGridView
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewSystemBackup() { }

        [Obsolete]
        public TreeViewSystemBackup(Window parentWindow)
            : this(parentWindow, null, null, null) { }

        //XpoMode
        [Obsolete]
        public TreeViewSystemBackup(Window parentWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.Default, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(sys_systembackup);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            sys_systembackup defaultValue = (pDefaultValue != null) ? pDefaultValue as sys_systembackup : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Configure columnProperties
            List<GridViewColumn> columnProperties = new List<GridViewColumn>
            {
                new GridViewColumn("Version")
                {
                    Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_version"),
                    MinWidth = 60,
                    CellRenderer = new CellRendererText()
                    {
                        Alignment = Pango.Alignment.Right,
                        Xalign = 1.0F
                    }
                },
                new GridViewColumn("CreatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_date"), MinWidth = 180 },
                new GridViewColumn("FileNamePacked") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_file"), MinWidth = 240 },
                new GridViewColumn("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            //Sort Order
            SortProperty[] sortProperty = new SortProperty[2];
            sortProperty[0] = new SortProperty("Version", SortingDirection.Descending);
            sortProperty[1] = new SortProperty("CreatedAt", SortingDirection.Ascending);

            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");
            CriteriaOperator criteria;
            if (pXpoCriteria != null)
            {
                criteria = CriteriaOperator.Parse($"({pXpoCriteria}) AND (DeletedAt IS NULL)");
            }
            else
            {
                criteria = CriteriaOperator.Parse($"(DeletedAt IS NULL)");
            }
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria, sortProperty);

            //Call Base Initializer
            base.InitObject(
              parentWindow,                  //Pass parameter 
              defaultValue,                   //Pass parameter
              pGenericTreeViewMode,           //Pass parameter
              navigatorMode,  //Pass parameter
              columnProperties,               //Created Here
              xpoCollection,                  //Created Here
              typeDialogClass                 //Created Here
            );
        }
    }
}
