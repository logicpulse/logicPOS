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
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Results;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{

    class TreeViewDocumentFinanceSeries : GenericTreeViewXPO
    {
        private TouchButtonIconWithText _buttonCreateDocumentFinanceSeries;
        public TouchButtonIconWithText ButtonCreateDocumentFinanceSeries
        {
            get { return _buttonCreateDocumentFinanceSeries; }
            set { _buttonCreateDocumentFinanceSeries = value; }
        }

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
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("FiscalYear") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_fiscal_year"), ChildName = "Designation", MinWidth = 160 });
            columnProperties.Add(new GenericTreeViewColumnProperty("DocumentType") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinanceseries_documenttype"), ChildName = "Designation", Expand = true });
            columnProperties.Add(new GenericTreeViewColumnProperty("Designation") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_designation"), Expand = true });
            columnProperties.Add(new GenericTreeViewColumnProperty("UpdatedAt") { Title = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 });

            //Configure Criteria/XPCollection/Model : Use Default Filter
            CriteriaOperator criteria = (ReferenceEquals(pXpoCriteria, null))
                // Generate Default Criteria
                ? CriteriaOperator.Parse("(Disabled = 0 OR Disabled IS NULL)")
                // Add to Parameter Criteria
                : CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL) AND ({0})", pXpoCriteria.ToString()))
            ;
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

            //Add Extra Button to Navigator
            _buttonCreateDocumentFinanceSeries = Navigator.GetNewButton("touchButtonCreateDocumentFinanceSeries_DialogActionArea", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_button_create_series"), @"Icons/icon_pos_nav_new.png");
            //_buttonCreateDocumentFinanceSeries.WidthRequest = 110;
            //Check if Has an Active Year Open before apply Permissions
            fin_documentfinanceyears currentDocumentFinanceYear = ProcessFinanceDocumentSeries.GetCurrentDocumentFinanceYear();
            //Apply Permissions 
            _buttonCreateDocumentFinanceSeries.Sensitive = (currentDocumentFinanceYear != null && FrameworkUtils.HasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCESERIES_MANAGE_SERIES"));
            //Event
            _buttonCreateDocumentFinanceSeries.Clicked += buttonCreateDocumentFinanceSeries_Clicked;
            //Add to Extra Slot
            Navigator.ExtraSlot.PackStart(_buttonCreateDocumentFinanceSeries, false, false, 0);
        }

        private void buttonCreateDocumentFinanceSeries_Clicked(object sender, EventArgs e)
        {
            try
            {
                //Refresh Terminal XPO Object
                XPCollection xpcDocumentFinanceYears = new XPCollection(GlobalFramework.SessionXpo, typeof(fin_documentfinanceyears));
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
                    ResponseType responseType = Utils.ShowMessageTouch(
                        _sourceWindow,
                        DialogFlags.Modal,
                        new Size(600, 400),
                        MessageType.Error,
                        ButtonsType.Close,
                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_series_create_series"),
                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_series_create_document_type_series_miss_year")
                    );
                    //Disable Button, Extra protection for deleted year outside App
                    _buttonCreateDocumentFinanceSeries.Sensitive = false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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
                XPCollection xpcConfigurationPlaceTerminal = new XPCollection(GlobalFramework.SessionXpo, typeof(pos_configurationplaceterminal));
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
                    logicpos.Utils.ResponseText resultAcronym = TreeViewDocumentFinanceSeries.PosConfirmAcronymSeriesDialog(pSourceWindow, pDocumentFinanceYear, dataTableSelectedTerminals, initialValue);

                    // Protect to Skip Cancel
                    if (resultAcronym.ResponseType == ResponseType.Ok)
                    {
                        int sqlCheckExistingSeriesResultInt = 0;
                        string sqlCheckExistingSeries = "SELECT COUNT(*) AS Count FROM fin_documentfinanceseries WHERE (Disabled = 0 OR Disabled IS NULL);";
                        object sqlCheckExistingSeriesResult = GlobalFramework.SessionXpo.ExecuteScalar(sqlCheckExistingSeries);
                        if (sqlCheckExistingSeriesResult != null) sqlCheckExistingSeriesResultInt = Convert.ToInt16(sqlCheckExistingSeriesResult);

                        //Request User Confirmation if already has working Series
                        ResponseType responseType = ResponseType.No;
                        if (resultAcronym.ResponseType == ResponseType.Ok && sqlCheckExistingSeriesResultInt > 0)
                        {
                            responseType = Utils.ShowMessageTouch(
                                GlobalApp.WindowStartup,
                                DialogFlags.Modal,
                                MessageType.Question,
                                ButtonsType.YesNo,
                                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_series_create_series"),
                                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_series_create_confirmation_text")
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
                                Utils.ShowMessageTouch(
                                    pSourceWindow,
                                    DialogFlags.Modal,
                                    MessageType.Error,
                                    ButtonsType.Ok,
                                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"),
                                    string.Format("{0}{1}{1}{2}",
                                        string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_series_create_error"), resultAcronym.Text),
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
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Helpers

        //UI Helper to Request Acronym for Terminal Series

        public static logicpos.Utils.ResponseText PosConfirmAcronymSeriesDialog(Window pSourceWindow, fin_documentfinanceyears pDocumentFinanceYear, DataTable pTerminals, string pInitialValue)
        {
            logicpos.Utils.ResponseText result = new Utils.ResponseText();
            FrameworkCallsResult frameworkCallsResult;

            try
            {
                string fileWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_input_text_default.png");

                PosInputTextDialog dialog = new PosInputTextDialog(
                    pSourceWindow,
                    DialogFlags.Modal,
                    new System.Drawing.Size(800, 600),
                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_series_create_series"),
                    fileWindowIcon,
                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_acronym"),
                    pInitialValue,
                    SettingsApp.RegexDocumentSeriesAcronym,
                    true
                    );

                //Initialize EntryBoxValidationMultiLine
                EntryBoxValidationMultiLine entryBoxValidationMultiLine = new EntryBoxValidationMultiLine(pSourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_preview"));
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
                _log.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
