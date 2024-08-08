﻿using DevExpress.Data.Filtering;
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
    internal class TreeViewConfigurationCashRegister : XpoGridView
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewConfigurationCashRegister() { }

        [Obsolete]
        public TreeViewConfigurationCashRegister(Window parentWindow)
            : this(parentWindow, null, null, null) { }

        //XpoMode
        [Obsolete]
        public TreeViewConfigurationCashRegister(Window parentWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.Default, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(pos_configurationcashregister);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            pos_configurationcashregister defaultValue = (pDefaultValue != null) ? pDefaultValue as pos_configurationcashregister : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogConfigurationCashRegister);

            //Configure columnProperties
            List<GridViewColumnProperty> columnProperties = new List<GridViewColumnProperty>
            {
                new GridViewColumnProperty("Code") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code"), MinWidth = 100 },
                new GridViewColumnProperty("Designation") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation_Drawer"), Expand = true },
                new GridViewColumnProperty("Drawer") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister") },
                new GridViewColumnProperty("AutomaticDrawer") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister_AutomaticDrawer") },
                new GridViewColumnProperty("ActiveSales") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister_ActiveSales") },
                new GridViewColumnProperty("AllowChargeBacks") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_ConfigurationCashRegister_AllowChargeBacks") },
                new GridViewColumnProperty("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
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
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria);

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
