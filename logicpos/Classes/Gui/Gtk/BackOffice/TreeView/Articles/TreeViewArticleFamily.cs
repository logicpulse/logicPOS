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
    class TreeViewArticleFamily : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewArticleFamily() { }

        public TreeViewArticleFamily(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewArticleFamily(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_articlefamily);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_articlefamily defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_articlefamily : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogArticleFamily);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("Code") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_code"), MinWidth = 100 });
            columnProperties.Add(new GenericTreeViewColumnProperty("Designation") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), Expand = true });
            columnProperties.Add(new GenericTreeViewColumnProperty("Printer") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_device_printer"), ChildName = "Designation" });
            columnProperties.Add(new GenericTreeViewColumnProperty("UpdatedAt") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 });

            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");
            CriteriaOperator criteria = pXpoCriteria;
            XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria);

            //TODO: Clean Comments : Moved Events to XPOWidget - Capture Events - VIEW DialogArticleFamily _crudWidgetList.BeforeUpdate and _crudWidgetList.AfterUpdate
            //this.RecordBeforeInsert += delegate { 
            //  _log.Debug(string.Format("[{0}] [{1}]", this.GetType(), "RecordBeforeInsert")); 
            //  _log.Debug(string.Format("Dialog.DataSourceRow: [{0}]", (_dialog as DialogArticleFamily).DataSourceRow));
            //  _log.Debug(string.Format("Dialog.CrudWidgetList: [{0}]", (_dialog as DialogArticleFamily).CrudWidgetList));
            //};

            //TODO: Test Events Here
            //this.RecordBeforeUpdate += delegate { _log.Debug(string.Format("[{0}] [{1}]", this.GetType(), "RecordBeforeUpdate"));};
            //this.RecordBeforeDelete += delegate { _log.Debug(string.Format("[{0}] [{1}]", this.GetType(), "RecordBeforeDelete"));};
            //this.RecordAfterInsert += delegate { _log.Debug(string.Format("[{0}] [{1}]", this.GetType(), "RecordAfterInsert")); };
            //this.RecordAfterUpdate += delegate { _log.Debug(string.Format("[{0}] [{1}]", this.GetType(), "RecordAfterUpdate"));};
            //this.RecordAfterDelete += delegate { _log.Debug(string.Format("[{0}] [{1}]", this.GetType(), "RecordAfterDelete"));};
            //this.RecordBeforeConfirm += delegate { _log.Debug(string.Format("[{0}] [{1}]", this.GetType(), "RecordBeforeConfirm"));};

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
        }
    }
}
