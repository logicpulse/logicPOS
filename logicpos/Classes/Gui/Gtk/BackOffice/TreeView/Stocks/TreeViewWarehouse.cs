using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.BackOffice.Dialogs.Configuration;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using System;
using System.Collections.Generic;
using LogicPOS.Settings.Extensions;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewWarehouse : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewWarehouse() { }

        [Obsolete]
        public TreeViewWarehouse(Window pSourceWindow)
            : this(pSourceWindow, null, null, null, GenericTreeViewMode.Default, GenericTreeViewNavigatorMode.Default) { }

        //XpoMode
        [Obsolete]
        public TreeViewWarehouse(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.CheckBox, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_warehouse);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_warehouse defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_warehouse : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogConfigurationWarehouse);

            //Config
            int fontGenericTreeViewColumn = Convert.ToInt16(LogicPOS.Settings.GeneralSettings.Settings["fontGenericTreeViewColumn"]);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("Code") { Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_article_code"), Expand = false },
                new GenericTreeViewColumnProperty("Designation") { Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_designation"), Expand = true },
                new GenericTreeViewColumnProperty("UpdatedAt") { Title = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            //Configure Criteria/XPCollection/Model
            //Default Criteria with XpoOidUndefinedRecord
            CriteriaOperator criteria;
            // Override Criteria adding XpoOidHiddenRecordsFilter
            if (pXpoCriteria != null)
            {
                criteria = CriteriaOperator.Parse($"({pXpoCriteria}) AND (DeletedAt IS NULL)");
            }
            else
            {
                criteria = CriteriaOperator.Parse($"(DeletedAt IS NULL)");
            }
            //Custom Criteria hidding all Hidden Oids
            //CriteriaOperator criteria = CriteriaOperator.Parse($"(Oid = '{SettingsApp.XpoOidUndefinedRecord}' OR Oid NOT LIKE '{SettingsApp.XpoOidHiddenRecordsFilter}')");
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria);
            var sortingCollection = new SortingCollection
            {
                new SortProperty("Code", DevExpress.Xpo.DB.SortingDirection.Ascending)
            };
            xpoCollection.Sorting = sortingCollection;

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
