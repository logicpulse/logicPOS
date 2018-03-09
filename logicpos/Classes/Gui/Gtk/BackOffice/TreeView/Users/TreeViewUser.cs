using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Collections.Generic;
using logicpos.Classes.Enums.GenericTreeView;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewUser : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewUser() { }

        public TreeViewUser(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewUser(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            Type xpoGuidObjectType = typeof(SYS_UserDetail);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            SYS_UserDetail defaultValue = (pDefaultValue != null) ? pDefaultValue as SYS_UserDetail : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogUserDetail);

            // XPO column properties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("Code") { Title = Resx.global_record_code, MinWidth = 100 });
            columnProperties.Add(new GenericTreeViewColumnProperty("Name") { Title = Resx.global_users, Expand = true });
            columnProperties.Add(new GenericTreeViewColumnProperty("Profile") { Title = Resx.global_profile, ChildName = "Designation" });
            columnProperties.Add(new GenericTreeViewColumnProperty("MobilePhone") { Title = Resx.global_mobile_phone });
            columnProperties.Add(new GenericTreeViewColumnProperty("FiscalNumber") { Title = Resx.global_fiscal_number });
            //columnProperties.Add(new GenericTreeViewColumnProperty("Disabled") { Title = Resx.global_record_disabled });

            //configure criteria/xpcollection/model
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

            //Protection to LogOff User
            this.RecordBeforeDelete += TreeView_RecordBeforeDelete;
        }

        //LogOff User Before Delete
        void TreeView_RecordBeforeDelete(object sender, EventArgs e)
        {
            SYS_UserDetail userDetail = (_dataSourceRow as SYS_UserDetail);
            //If User deleted Force Logout in Sytem
            GlobalApp.WindowStartup.LogOutUser(false, userDetail);
        }
    }
}
