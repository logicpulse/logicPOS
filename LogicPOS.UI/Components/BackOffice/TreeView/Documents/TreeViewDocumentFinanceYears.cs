using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Components;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewDocumentFinanceYears : XpoGridView
    {

        public TreeViewDocumentFinanceYears() { }

        public TreeViewDocumentFinanceYears(Window parentWindow)
            : this(parentWindow, null, null, null) { }

        //XpoMode
        public TreeViewDocumentFinanceYears(Window parentWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.Default, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_documentfinanceyears);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_documentfinanceyears defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_documentfinanceyears : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogDocumentFinanceYears);

            //Configure columnProperties
            List<GridViewColumnProperty> columnProperties = new List<GridViewColumnProperty>
            {
                new GridViewColumnProperty("FiscalYear") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_fiscal_year") },
                new GridViewColumnProperty("Designation") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_designation"), Expand = true },
                new GridViewColumnProperty("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            //Configure Criteria/XPCollection/Model : Use Default Filter
            CriteriaOperator criteria = (ReferenceEquals(pXpoCriteria, null)) ? CriteriaOperator.Parse("(Disabled = 0 OR Disabled IS NULL)") : pXpoCriteria;
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria);

            //Protection to Disable Series Before Creating Year
            this.RecordBeforeInsert += TreeViewDocumentFinanceYears_RecordBeforeInsert;
            this.RecordAfterInsert += TreeViewDocumentFinanceYears_RecordAfterInsert;

            //Call Base Initializer
            base.InitObject(
              parentWindow,                  
              defaultValue,                  
              pGenericTreeViewMode,          
              navigatorMode, 
              columnProperties,              
              xpoCollection,                 
              typeDialogClass                
            );
        }

        private void TreeViewDocumentFinanceYears_RecordBeforeInsert(object sender, EventArgs e)
        {

            fin_documentfinanceyears currentDocumentFinanceYear = DocumentProcessingSeriesUtils.GetCurrentDocumentFinanceYear();

            if (currentDocumentFinanceYear != null)
            {
                ResponseType responseType = logicpos.Utils.ShowMessageBox(
                    GlobalApp.StartupWindow,
                    DialogFlags.Modal,
                    new Size(600, 400),
                    MessageType.Question,
                    ButtonsType.YesNo,
                    CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_series_fiscal_year_close_current"),
                    string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_series_fiscal_year_close_current"), currentDocumentFinanceYear.Designation)
                );

                SkipRecordInsert = (responseType == ResponseType.No);

                if (responseType == ResponseType.Yes)
                {
                    bool resultDisableFiscalYear = DocumentProcessingSeriesUtils.DisableActiveYearSeriesAndTerminalSeries(currentDocumentFinanceYear);
                    
                    if (resultDisableFiscalYear)
                    {
                        currentDocumentFinanceYear.Disabled = true;
                        currentDocumentFinanceYear.Save();
                    }
                }
            }

        }

        private void TreeViewDocumentFinanceYears_RecordAfterInsert(object sender, EventArgs e)
        {

            //Get References to TreeViewDocumentFinanceSeries and TreeViewDocumentFinanceYearSerieTerminal
            TreeViewDocumentFinanceSeries treeViewDocumentFinanceSeries = ((_parentWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Children["DocumentFinanceSeries"].Content as TreeViewDocumentFinanceSeries);
            //Store Reference to BackOffice TreeViewDocumentFinanceYearSerieTerminal
            TreeViewDocumentFinanceYearSerieTerminal treeViewDocumentFinanceYearSerieTerminal =
                ((_parentWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Children.ContainsKey("DocumentFinanceYearSerieTerminal"))
                ? ((_parentWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Children["DocumentFinanceYearSerieTerminal"].Content as TreeViewDocumentFinanceYearSerieTerminal)
                : null;
            //Refresh TreeViews after Insert Record, and Hide old Series
            Refresh();
            treeViewDocumentFinanceSeries.Refresh();
            if (treeViewDocumentFinanceYearSerieTerminal != null) treeViewDocumentFinanceYearSerieTerminal.Refresh();
            //Apply Permissions ButtonCreateDocumentFinanceSeries
            treeViewDocumentFinanceSeries.ButtonCreateDocumentFinanceSeries.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCESERIES_MANAGE_SERIES");

            //Request Create Series for all Type of Finance Documents
            ResponseType responseType = logicpos.Utils.ShowMessageBox(
                _parentWindow,
                DialogFlags.Modal,
                new Size(600, 400),
                MessageType.Question,
                ButtonsType.YesNo,
                GeneralUtils.GetResourceByName("window_title_series_create_series"),
                GeneralUtils.GetResourceByName("dialog_message_series_create_document_type_series")
            );

            if (responseType == ResponseType.Yes)
            {
                //Get Current Active FinanceYear
                fin_documentfinanceyears currentDocumentFinanceYear = DocumentProcessingSeriesUtils.GetCurrentDocumentFinanceYear();
                bool result = TreeViewDocumentFinanceSeries.UICreateDocumentFinanceYearSeriesTerminal(_parentWindow, currentDocumentFinanceYear);
            }

        }
    }
}
