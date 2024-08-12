using DevExpress.Data.Filtering;
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
    internal class TreeViewDocumentFinanceYearSerieTerminal : XpoGridView
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewDocumentFinanceYearSerieTerminal() { }

        public TreeViewDocumentFinanceYearSerieTerminal(Window parentWindow)
            : this(parentWindow, null, null, null) { }

        //XpoMode
        public TreeViewDocumentFinanceYearSerieTerminal(Window parentWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.Default, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_documentfinanceyearserieterminal);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_documentfinanceyearserieterminal defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_documentfinanceyearserieterminal : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogDocumentFinanceYearSerieTerminal);

            //Configure columnProperties
            List<GridViewColumn> columnProperties = new List<GridViewColumn>
            {
                new GridViewColumn("FiscalYear") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_fiscal_year"), ChildName = "Designation" },
                //columnProperties.Add(new GenericTreeViewColumnProperty("DocumentType") { Title = CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_documentfinanceseries_documenttype, ChildName = "Designation" });
                //columnProperties.Add(new GenericTreeViewColumnProperty("Serie") { Title = CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_documentfinance_series, ChildName = "Designation" });
                new GridViewColumn("Designation") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true },
                new GridViewColumn("Terminal") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_configurationplaceterminal"), ChildName = "Designation" },
                new GridViewColumn("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            //Configure Criteria/XPCollection/Model : Use Default Filter
            CriteriaOperator criteria = (ReferenceEquals(pXpoCriteria, null)) ? CriteriaOperator.Parse("(Disabled = 0 OR Disabled IS NULL)") : pXpoCriteria;
            //Init Collection
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria);
            //Override default Sorting
            SortingCollection sortingCollection = new SortingCollection
            {
                new SortProperty("Terminal", DevExpress.Xpo.DB.SortingDirection.Ascending),
                new SortProperty("Ord", DevExpress.Xpo.DB.SortingDirection.Ascending)
            };
            xpoCollection.Sorting = sortingCollection;

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
