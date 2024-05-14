using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using System;
using System.Collections.Generic;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewDocumentFinanceYearSerieTerminal : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewDocumentFinanceYearSerieTerminal() { }

        public TreeViewDocumentFinanceYearSerieTerminal(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewDocumentFinanceYearSerieTerminal(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_documentfinanceyearserieterminal);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_documentfinanceyearserieterminal defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_documentfinanceyearserieterminal : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogDocumentFinanceYearSerieTerminal);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("FiscalYear") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_fiscal_year"), ChildName = "Designation" },
                //columnProperties.Add(new GenericTreeViewColumnProperty("DocumentType") { Title = CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_documentfinanceseries_documenttype, ChildName = "Designation" });
                //columnProperties.Add(new GenericTreeViewColumnProperty("Serie") { Title = CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_documentfinance_series, ChildName = "Designation" });
                new GenericTreeViewColumnProperty("Designation") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true },
                new GenericTreeViewColumnProperty("Terminal") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_configurationplaceterminal"), ChildName = "Designation" },
                new GenericTreeViewColumnProperty("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
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
