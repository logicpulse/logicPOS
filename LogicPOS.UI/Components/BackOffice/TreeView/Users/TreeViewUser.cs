﻿using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.UI.Components;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewUser : XpoGridView
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewUser() { }

        [Obsolete]
        public TreeViewUser(Window parentWindow)
            : this(parentWindow, null, null, null) { }

        //XpoMode
        [Obsolete]
        public TreeViewUser(Window parentWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.Default, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            Type xpoGuidObjectType = typeof(sys_userdetail);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            sys_userdetail defaultValue = (pDefaultValue != null) ? pDefaultValue as sys_userdetail : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogUserDetail);

            // XPO column properties
            List<GridViewColumnProperty> columnProperties = new List<GridViewColumnProperty>
            {
                new GridViewColumnProperty("Code") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code"), MinWidth = 100 },
                new GridViewColumnProperty("Name") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_users"), Expand = true },
                new GridViewColumnProperty("Profile") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_profile"), ChildName = "Designation", MinWidth = 160 },
                //columnProperties.Add(new GenericTreeViewColumnProperty("MobilePhone") { Title = CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_mobile_phone });
                new GridViewColumnProperty("FiscalNumber") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_fiscal_number"), MinWidth = 100 },
                new GridViewColumnProperty("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            //configure criteria/xpcollection/model
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

            //Protection to LogOff User
            this.RecordBeforeDelete += TreeView_RecordBeforeDelete;
        }

        //LogOff User Before Delete
        private void TreeView_RecordBeforeDelete(object sender, EventArgs e)
        {
            sys_userdetail userDetail = (Entity as sys_userdetail);
            //If User deleted Force Logout in Sytem
            GlobalApp.StartupWindow.LogOutUser(false, userDetail);
        }
    }
}
