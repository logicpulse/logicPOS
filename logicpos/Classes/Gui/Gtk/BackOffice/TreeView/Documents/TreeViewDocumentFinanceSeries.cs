using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Results;
using logicpos.shared.App;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewDocumentFinanceSeries : GenericTreeViewXPO
    {
        public TouchButtonIconWithText ButtonCreateDocumentFinanceSeries { get; set; }

        //Public Parametless Constructor Required by Generics
        public TreeViewDocumentFinanceSeries() { }

        public TreeViewDocumentFinanceSeries(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewDocumentFinanceSeries(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Assign Parameters to Members
            _sourceWindow = pSourceWindow;

            //Init Vars
            Type xpoGuidObjectType = typeof(fin_documentfinanceseries);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_documentfinanceseries defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_documentfinanceseries : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogDocumentFinanceSeries);

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("FiscalYear") { Title = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_fiscal_year"), ChildName = "Designation", MinWidth = 160 },
                new GenericTreeViewColumnProperty("DocumentType") { Title = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_documentfinanceseries_documenttype"), ChildName = "Designation", Expand = true },
                new GenericTreeViewColumnProperty("Designation") { Title = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_designation"), Expand = true },
                new GenericTreeViewColumnProperty("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            //Configure Criteria/XPCollection/Model : Use Default Filter
            CriteriaOperator criteria = (ReferenceEquals(pXpoCriteria, null))
                // Generate Default Criteria
                ? CriteriaOperator.Parse("(Disabled = 0 OR Disabled IS NULL)")
                // Add to Parameter Criteria
                : CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND ({0})", pXpoCriteria.ToString()))
            ;
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria);

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

            //Add Extra Button to Navigator
            ButtonCreateDocumentFinanceSeries = Navigator.GetNewButton("touchButtonCreateDocumentFinanceSeries_DialogActionArea", CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "pos_button_create_series"), @"Icons/icon_pos_nav_new.png");
            //_buttonCreateDocumentFinanceSeries.WidthRequest = 110;
            //Check if Has an Active Year Open before apply Permissions
            fin_documentfinanceyears currentDocumentFinanceYear = ProcessFinanceDocumentSeries.GetCurrentDocumentFinanceYear();
            //Apply Permissions 
            ButtonCreateDocumentFinanceSeries.Sensitive = (currentDocumentFinanceYear != null && SharedUtils.HasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCESERIES_MANAGE_SERIES"));
            //Event
            ButtonCreateDocumentFinanceSeries.Clicked += buttonCreateDocumentFinanceSeries_Clicked;
            //Add to Extra Slot
            Navigator.ExtraSlot.PackStart(ButtonCreateDocumentFinanceSeries, false, false, 0);
        }

        private void buttonCreateDocumentFinanceSeries_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Refresh Terminal XPO Object
                XPCollection xpcDocumentFinanceYears = new XPCollection(XPOSettings.Session, typeof(fin_documentfinanceyears));
                xpcDocumentFinanceYears.Reload();

                //Store Reference to BackOffice TreeViewDocumentFinanceYearSerieTerminal
                TreeViewDocumentFinanceYearSerieTerminal treeViewDocumentFinanceYearSerieTerminal =
                    ((_sourceWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Childs.ContainsKey("DocumentFinanceYearSerieTerminal"))
                    ? ((_sourceWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Childs["DocumentFinanceYearSerieTerminal"].Content as TreeViewDocumentFinanceYearSerieTerminal)
                    : null;

                //Internal Shared Action to Refresh Components
                var internalMethodRefreshComponents = new System.Action(() =>
                {
                    //Refresh Current Tree Model 
                    Refresh();
                    //Refresh TreeViewDocumentFinanceYearSerieTerminal (If Visible/Enabled)
                    if (treeViewDocumentFinanceYearSerieTerminal != null) treeViewDocumentFinanceYearSerieTerminal.Refresh();
                });

                //Get Current Active FinanceYear
                fin_documentfinanceyears currentDocumentFinanceYear = ProcessFinanceDocumentSeries.GetCurrentDocumentFinanceYear();

                //If has Active FiscalYear, Show Warning Request to Close/Open
                if (currentDocumentFinanceYear != null)
                {
                    //Call Create Series UI
                    bool result = UICreateDocumentFinanceYearSeriesTerminal(_sourceWindow, currentDocumentFinanceYear);
                    //Refresh Current Tree Model 
                    if (result)
                    {
                        //Refresh Current Tree Model 
                        Refresh();
                        //Refresh TreeViewDocumentFinanceYearSerieTerminal (If Visible/Enabled)
                        if (treeViewDocumentFinanceYearSerieTerminal != null) treeViewDocumentFinanceYearSerieTerminal.Refresh();
                    }
                }
                else
                {
                    ResponseType responseType = logicpos.Utils.ShowMessageTouch(
                        _sourceWindow,
                        DialogFlags.Modal,
                        new Size(600, 400),
                        MessageType.Error,
                        ButtonsType.Close,
                        CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "window_title_series_create_series"),
                        CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_message_series_create_document_type_series_miss_year")
                    );
                    //Disable Button, Extra protection for deleted year outside App
                    ButtonCreateDocumentFinanceSeries.Sensitive = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Shared UI Helper to Request Selected Terminals to send to ProcessFinanceDocumentSeries (Framework)

        public static bool UICreateDocumentFinanceYearSeriesTerminal(Window pSourceWindow, fin_documentfinanceyears pDocumentFinanceYear)
        {
            bool result = false;

            try
            {
                //Refresh Terminal XPO Object
                XPCollection xpcConfigurationPlaceTerminal = new XPCollection(XPOSettings.Session, typeof(pos_configurationplaceterminal));
                xpcConfigurationPlaceTerminal.Reload();

                //Get Terminals
                DataTable dataTableSelectedTerminals = PosSelectRecordDialog<DataTable, DataRow, TreeViewTerminalSeries>.GetSelected(pSourceWindow);

                //Store Reference to BackOffice TreeViewDocumentFinanceYearSerieTerminal
                TreeViewDocumentFinanceSeries treeViewDocumentFinanceSeries =
                    ((pSourceWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Childs.ContainsKey("DocumentFinanceSeries"))
                    ? ((pSourceWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Childs["DocumentFinanceSeries"].Content as TreeViewDocumentFinanceSeries)
                    : null;

                //Store Reference to BackOffice TreeViewDocumentFinanceYearSerieTerminal
                TreeViewDocumentFinanceYearSerieTerminal treeViewDocumentFinanceYearSerieTerminal =
                    ((pSourceWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Childs.ContainsKey("DocumentFinanceYearSerieTerminal"))
                    ? ((pSourceWindow as BackOfficeMainWindow).Accordion.Nodes["TopMenuDocuments"].Childs["DocumentFinanceYearSerieTerminal"].Content as TreeViewDocumentFinanceYearSerieTerminal)
                    : null;

                if (dataTableSelectedTerminals != null && dataTableSelectedTerminals.Rows.Count > 0)
                {
                    //Request Acronym
                    //string initialValue = string.Format("{0}{1}", pDocumentFinanceYear.Acronym, "01");
                    string initialValue = pDocumentFinanceYear.Acronym;
                    logicpos.Utils.ResponseText resultAcronym = PosConfirmAcronymSeriesDialog(pSourceWindow, pDocumentFinanceYear, dataTableSelectedTerminals, initialValue);

                    // Protect to Skip Cancel
                    if (resultAcronym.ResponseType == ResponseType.Ok)
                    {
                        int sqlCheckExistingSeriesResultInt = 0;
                        string sqlCheckExistingSeries = "SELECT COUNT(*) AS Count FROM fin_documentfinanceseries WHERE (Disabled = 0 OR Disabled IS NULL);";
                        object sqlCheckExistingSeriesResult = XPOSettings.Session.ExecuteScalar(sqlCheckExistingSeries);
                        if (sqlCheckExistingSeriesResult != null) sqlCheckExistingSeriesResultInt = Convert.ToInt16(sqlCheckExistingSeriesResult);

                        //Request User Confirmation if already has working Series
                        ResponseType responseType = ResponseType.No;
                        if (resultAcronym.ResponseType == ResponseType.Ok && sqlCheckExistingSeriesResultInt > 0)
                        {
                            responseType = logicpos.Utils.ShowMessageTouch(
                                GlobalApp.StartupWindow,
                                DialogFlags.Modal,
                                MessageType.Question,
                                ButtonsType.YesNo,
                                CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "window_title_series_create_series"),
                                CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_message_series_create_confirmation_text")
                            );
                        }
                        else
                        {
                            //Auto Yes, if dont have series, assumes Yes 
                            responseType = ResponseType.Yes;
                        }

                        if (responseType == ResponseType.Yes)
                        {
                            //Get Result
                            FrameworkCallsResult frameworkCallsResult = ProcessFinanceDocumentSeries.CreateDocumentFinanceYearSeriesTerminal(pDocumentFinanceYear, dataTableSelectedTerminals, resultAcronym.Text, false);
                            //Prepare Result
                            result = frameworkCallsResult.Result;
                            //Refresh Child Trees DocumentFinanceYearSerieTerminal
                            if (result)
                            {
                                //Refresh TreeViewDocumentFinanceSeries (If Visible/Enabled)
                                if (treeViewDocumentFinanceSeries != null) treeViewDocumentFinanceSeries.Refresh();
                                //Refresh TreeViewDocumentFinanceYearSerieTerminal (If Visible/Enabled)
                                if (treeViewDocumentFinanceYearSerieTerminal != null) treeViewDocumentFinanceYearSerieTerminal.Refresh();
                            }
                            //Show Error to User, Outside of Framework (Non UI)
                            else
                            {
                                logicpos.Utils.ShowMessageTouch(
                                    pSourceWindow,
                                    DialogFlags.Modal,
                                    MessageType.Error,
                                    ButtonsType.Ok,
                                    CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_error"),
                                    string.Format("{0}{1}{1}{2}",
                                        string.Format(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_message_series_create_error"), resultAcronym.Text),
                                        Environment.NewLine,
                                        frameworkCallsResult.Exception.InnerException.Message
                                    )
                                 );
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Helpers

        //UI Helper to Request Acronym for Terminal Series

        public static logicpos.Utils.ResponseText PosConfirmAcronymSeriesDialog(Window pSourceWindow, fin_documentfinanceyears pDocumentFinanceYear, DataTable pTerminals, string pInitialValue)
        {
            logicpos.Utils.ResponseText result = new logicpos.  Utils.ResponseText();
            FrameworkCallsResult frameworkCallsResult;

            try
            {
                string fileWindowIcon = GeneralSettings.Paths["images"] + @"Icons\Windows\icon_window_input_text_default.png";

                PosInputTextDialog dialog = new PosInputTextDialog(
                    pSourceWindow,
                    DialogFlags.Modal,
                    new System.Drawing.Size(800, 600),
                    CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "window_title_series_create_series"),
                    fileWindowIcon,
                    CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_acronym"),
                    pInitialValue,
                    LogicPOS.Utility.RegexUtils.RegexDocumentSeriesAcronym,
                    true
                    );

                //Initialize EntryBoxValidationMultiLine
                EntryBoxValidationMultiLine entryBoxValidationMultiLine = new EntryBoxValidationMultiLine(pSourceWindow, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_preview"));
                entryBoxValidationMultiLine.HeightRequest = 420;
                entryBoxValidationMultiLine.EntryMultiline.TextView.WrapMode = WrapMode.Word;
                entryBoxValidationMultiLine.EntryMultiline.TextView.Sensitive = false;

                //Start with Preview
                frameworkCallsResult = ProcessFinanceDocumentSeries.CreateDocumentFinanceYearSeriesTerminal(pDocumentFinanceYear, pTerminals, dialog.EntryBoxValidation.EntryValidation.Text, true);
                entryBoxValidationMultiLine.EntryMultiline.TextView.Buffer.Text = frameworkCallsResult.Output;

                //Pack Widgets
                dialog.VBoxContent.PackStart(entryBoxValidationMultiLine, true, true, 0);
                dialog.VBoxContent.ShowAll();
                dialog.EntryBoxValidation.EntryValidation.Changed += delegate
                {
                    if (dialog.EntryBoxValidation.EntryValidation.Validated)
                    {
                        frameworkCallsResult = ProcessFinanceDocumentSeries.CreateDocumentFinanceYearSeriesTerminal(pDocumentFinanceYear, pTerminals, dialog.EntryBoxValidation.EntryValidation.Text, true);
                        entryBoxValidationMultiLine.EntryMultiline.TextView.Buffer.Text = frameworkCallsResult.Output;
                    }
                };
                ResponseType responseType = (ResponseType)dialog.Run();
                result.ResponseType = responseType;

                if (responseType == ResponseType.Ok)
                {
                    result.Text = dialog.Value;
                }
                dialog.Destroy();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
