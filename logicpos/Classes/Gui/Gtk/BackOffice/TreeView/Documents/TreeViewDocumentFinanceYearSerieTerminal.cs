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

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewDocumentFinanceYearSerieTerminal : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewDocumentFinanceYearSerieTerminal() { }

        public TreeViewDocumentFinanceYearSerieTerminal(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewDocumentFinanceYearSerieTerminal(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(FIN_DocumentFinanceYearSerieTerminal);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            FIN_DocumentFinanceYearSerieTerminal defaultValue = (pDefaultValue != null) ? pDefaultValue as FIN_DocumentFinanceYearSerieTerminal : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogDocumentFinanceYearSerieTerminal);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("FiscalYear") { Title = Resx.global_fiscal_year, ChildName = "Designation" });
            //columnProperties.Add(new GenericTreeViewColumnProperty("DocumentType") { Title = Resx.global_documentfinanceseries_documenttype, ChildName = "Designation" });
            //columnProperties.Add(new GenericTreeViewColumnProperty("Serie") { Title = Resx.global_documentfinance_series, ChildName = "Designation" });
            columnProperties.Add(new GenericTreeViewColumnProperty("Designation") { Title = Resx.global_designation });
            columnProperties.Add(new GenericTreeViewColumnProperty("Terminal") { Title = Resx.global_configurationplaceterminal, ChildName = "Designation" });
            //columnProperties.Add(new GenericTreeViewColumnProperty("Disabled") { Title = Resx.global_record_disabled });

            //Configure Criteria/XPCollection/Model : Use Default Filter
            CriteriaOperator criteria = (ReferenceEquals(pXpoCriteria, null)) ? CriteriaOperator.Parse("(Disabled = 0 OR Disabled IS NULL)") : pXpoCriteria;
            //Init Collection
            XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria);
            //Override default Sorting
            SortingCollection sortingCollection = new SortingCollection();
            sortingCollection.Add(new SortProperty("Terminal", DevExpress.Xpo.DB.SortingDirection.Ascending));
            sortingCollection.Add(new SortProperty("Ord", DevExpress.Xpo.DB.SortingDirection.Ascending));
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
