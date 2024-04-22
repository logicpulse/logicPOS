using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.datalayer.App;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewWorkSessionPeriod : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewWorkSessionPeriod() { }

        public TreeViewWorkSessionPeriod(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewWorkSessionPeriod(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(pos_worksessionperiod);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            pos_worksessionperiod defaultValue = (pDefaultValue != null) ? pDefaultValue as pos_worksessionperiod : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("Designation") { Title = resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_designation") },
                new GenericTreeViewColumnProperty("DateStart") { Type = typeof(DateTime), Title = resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_date_start") },
                new GenericTreeViewColumnProperty("DateEnd") { Type = typeof(DateTime), Title = resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_date_end") }
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
            sortProperty[0] = new SortProperty("DateStart", SortingDirection.Ascending);
            XPCollection xpoCollection = new XPCollection(DataLayerFramework.SessionXpo, xpoGuidObjectType, criteria, sortProperty);

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
