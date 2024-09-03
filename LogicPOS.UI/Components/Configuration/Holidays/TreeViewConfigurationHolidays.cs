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
    internal class TreeViewConfigurationHolidays : XpoGridView
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewConfigurationHolidays() { }

        [Obsolete]
        public TreeViewConfigurationHolidays(Window parentWindow)
            : this(parentWindow, null, null, null) { }

        //XpoMode
        [Obsolete]
        public TreeViewConfigurationHolidays(Window parentWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.Default, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(cfg_configurationholidays);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            cfg_configurationholidays defaultValue = (pDefaultValue != null) ? pDefaultValue as cfg_configurationholidays : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogConfigurationHolidays);

            //Configure columnProperties
            List<GridViewColumn> columnProperties = new List<GridViewColumn>
            {
                new GridViewColumn("Code") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code"), MinWidth = 100 },
                /* IN009137 - adding Year, Month and Day columns to Holidays window */
                new GridViewColumn("Day")
                { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_day"), MinWidth = 40 },
                new GridViewColumn("Month")
                { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_month"), MinWidth = 40 },
                new GridViewColumn("Year")
                { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_year"), MinWidth = 60 },
                new GridViewColumn("Designation") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true },
                new GridViewColumn("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

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
            SortProperty[] sortProperty = new SortProperty[1];
            sortProperty[0] = new SortProperty("Code", SortingDirection.Ascending);
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
