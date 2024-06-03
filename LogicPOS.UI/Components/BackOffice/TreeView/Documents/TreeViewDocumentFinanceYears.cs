using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewDocumentFinanceYears : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewDocumentFinanceYears() { }

        public TreeViewDocumentFinanceYears(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewDocumentFinanceYears(Window pSourceWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(fin_documentfinanceyears);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_documentfinanceyears defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_documentfinanceyears : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogDocumentFinanceYears);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("FiscalYear") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_fiscal_year") },
                new GenericTreeViewColumnProperty("Designation") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true },
                new GenericTreeViewColumnProperty("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            //Configure Criteria/XPCollection/Model : Use Default Filter
            CriteriaOperator criteria = (ReferenceEquals(pXpoCriteria, null)) ? CriteriaOperator.Parse("(Disabled = 0 OR Disabled IS NULL)") : pXpoCriteria;
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria);

            //Protection to Disable Series Before Creating Year
            this.RecordBeforeInsert += TreeViewDocumentFinanceYears_RecordBeforeInsert;
            this.RecordAfterInsert += TreeViewDocumentFinanceYears_RecordAfterInsert;

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

        private void TreeViewDocumentFinanceYears_RecordBeforeInsert(object sender, EventArgs e)
        {
            try
            {
                //Get Current Active FinanceYear
                fin_documentfinanceyears currentDocumentFinanceYear = DocumentProcessingSeriesUtils.GetCurrentDocumentFinanceYear();

                //If has Active FiscalYear, Show Warning Request to Close/Open
                if (currentDocumentFinanceYear != null)
                {
                    ResponseType responseType = logicpos.Utils.ShowMessageBox(
                        GlobalApp.StartupWindow,
                        DialogFlags.Modal,
                        new Size(600, 400),
                        MessageType.Question,
                        ButtonsType.YesNo,
                        CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_series_fiscal_year_close_current"),
                        string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_series_fiscal_year_close_current"), currentDocumentFinanceYear.Designation)
                    );

                    //Override Insert CRUD ShowDialog using SkipRecordInsert, this prevent create Record
                    _skipRecordInsert = (responseType == ResponseType.No);

                    if (responseType == ResponseType.Yes)
                    {
                        //Disable All Related Year Series and TerminalSeries
                        bool resultDisableFiscalYear = DocumentProcessingSeriesUtils.DisableActiveYearSeriesAndTerminalSeries(currentDocumentFinanceYear);
                        //Now we can disable currentDocumentFinanceYear
                        if (resultDisableFiscalYear)
                        {
                            //Disable Object
                            currentDocumentFinanceYear.Disabled = true;
                            currentDocumentFinanceYear.Save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void TreeViewDocumentFinanceYears_RecordAfterInsert(object sender, EventArgs e)
        {
            try
            {
                //Get References to TreeViewDocumentFinanceSeries and TreeViewDocumentFinanceYearSerieTerminal
                TreeViewDocumentFinanceSeries treeViewDocumentFinanceSeries = ((_sourceWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Childs["DocumentFinanceSeries"].Content as TreeViewDocumentFinanceSeries);
                //Store Reference to BackOffice TreeViewDocumentFinanceYearSerieTerminal
                TreeViewDocumentFinanceYearSerieTerminal treeViewDocumentFinanceYearSerieTerminal =
                    ((_sourceWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Childs.ContainsKey("DocumentFinanceYearSerieTerminal"))
                    ? ((_sourceWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Childs["DocumentFinanceYearSerieTerminal"].Content as TreeViewDocumentFinanceYearSerieTerminal)
                    : null;
                //Refresh TreeViews after Insert Record, and Hide old Series
                Refresh();
                treeViewDocumentFinanceSeries.Refresh();
                if (treeViewDocumentFinanceYearSerieTerminal != null) treeViewDocumentFinanceYearSerieTerminal.Refresh();
                //Apply Permissions ButtonCreateDocumentFinanceSeries
                treeViewDocumentFinanceSeries.ButtonCreateDocumentFinanceSeries.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCESERIES_MANAGE_SERIES");

                //Request Create Series for all Type of Finance Documents
                ResponseType responseType = logicpos.Utils.ShowMessageBox(
                    _sourceWindow,
                    DialogFlags.Modal,
                    new Size(600, 400),
                    MessageType.Question,
                    ButtonsType.YesNo,
                    CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_series_create_series"),
                    CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_series_create_document_type_series")
                );

                if (responseType == ResponseType.Yes)
                {
                    //Get Current Active FinanceYear
                    fin_documentfinanceyears currentDocumentFinanceYear = DocumentProcessingSeriesUtils.GetCurrentDocumentFinanceYear();
                    bool result = TreeViewDocumentFinanceSeries.UICreateDocumentFinanceYearSeriesTerminal(_sourceWindow, currentDocumentFinanceYear);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
