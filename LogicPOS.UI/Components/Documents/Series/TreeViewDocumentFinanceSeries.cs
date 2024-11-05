using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.DTOs.Common;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Settings;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewDocumentFinanceSeries : XpoGridView
    {
        public IconButtonWithText ButtonCreateDocumentFinanceSeries { get; set; }

        //Public Parametless Constructor Required by Generics
        public TreeViewDocumentFinanceSeries() { }

        public TreeViewDocumentFinanceSeries(Window parentWindow)
            : this(parentWindow, null, null, null) { }

        //XpoMode
        public TreeViewDocumentFinanceSeries(Window parentWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.Default, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            //Assign Parameters to Members
            _parentWindow = parentWindow;

            //Init Vars
            Type xpoGuidObjectType = typeof(fin_documentfinanceseries);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            fin_documentfinanceseries defaultValue = (pDefaultValue != null) ? pDefaultValue as fin_documentfinanceseries : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : typeof(DialogDocumentFinanceSeries);

            //Configure columnProperties
            List<GridViewColumn> columnProperties = new List<GridViewColumn>
            {
                new GridViewColumn("FiscalYear") { Title = GeneralUtils.GetResourceByName("global_fiscal_year"), ChildName = "Designation", MinWidth = 160 },
                new GridViewColumn("DocumentType") { Title = GeneralUtils.GetResourceByName("global_documentfinanceseries_documenttype"), ChildName = "Designation", Expand = true },
                new GridViewColumn("Designation") { Title = GeneralUtils.GetResourceByName("global_designation"), Expand = true },
                new GridViewColumn("UpdatedAt") { Title = GeneralUtils.GetResourceByName("global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
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
              parentWindow,                  //Pass parameter 
              defaultValue,                   //Pass parameter
              pGenericTreeViewMode,           //Pass parameter
              navigatorMode,  //Pass parameter
              columnProperties,               //Created Here
              xpoCollection,                  //Created Here
              typeDialogClass                 //Created Here
            );

            //Add Extra Button to Navigator
            ButtonCreateDocumentFinanceSeries = Navigator.GetNewButton("touchButtonCreateDocumentFinanceSeries_DialogActionArea", GeneralUtils.GetResourceByName("pos_button_create_series"), @"Icons/icon_pos_nav_new.png");
            //_buttonCreateDocumentFinanceSeries.WidthRequest = 110;
            //Check if Has an Active Year Open before apply Permissions
            fin_documentfinanceyears currentDocumentFinanceYear = DocumentProcessingSeriesUtils.GetCurrentDocumentFinanceYear();
            //Apply Permissions 
            ButtonCreateDocumentFinanceSeries.Sensitive = (currentDocumentFinanceYear != null && GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_DOCUMENTFINANCESERIES_MANAGE_SERIES"));
            //Event
            ButtonCreateDocumentFinanceSeries.Clicked += buttonCreateDocumentFinanceSeries_Clicked;
            //Add to Extra Slot
            Navigator.ExtraSlot.PackStart(ButtonCreateDocumentFinanceSeries, false, false, 0);
        }

        private void buttonCreateDocumentFinanceSeries_Clicked(object sender, EventArgs e)
        {
            XPCollection xpcDocumentFinanceYears = new XPCollection(XPOSettings.Session, typeof(fin_documentfinanceyears));
            xpcDocumentFinanceYears.Reload();

            //Store Reference to BackOffice TreeViewDocumentFinanceYearSerieTerminal
            TreeViewDocumentFinanceYearSerieTerminal treeViewDocumentFinanceYearSerieTerminal =
                ((_parentWindow as BackOfficeWindow).Menu.Nodes["TopMenuDocuments"].Children.ContainsKey("DocumentFinanceYearSerieTerminal"))
                ? ((_parentWindow as BackOfficeWindow).Menu.Nodes["TopMenuDocuments"].Children["DocumentFinanceYearSerieTerminal"].Content as TreeViewDocumentFinanceYearSerieTerminal)
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
            fin_documentfinanceyears currentDocumentFinanceYear = DocumentProcessingSeriesUtils.GetCurrentDocumentFinanceYear();

            //If has Active FiscalYear, Show Warning Request to Close/Open
            if (currentDocumentFinanceYear != null)
            {
                //Call Create Series UI
                bool result = UICreateDocumentFinanceYearSeriesTerminal(_parentWindow, currentDocumentFinanceYear);
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
                ResponseType responseType = logicpos.Utils.ShowMessageBox(
                    _parentWindow,
                    DialogFlags.Modal,
                    new Size(600, 400),
                    MessageType.Error,
                    ButtonsType.Close,
                    GeneralUtils.GetResourceByName("window_title_series_create_series"),
                    GeneralUtils.GetResourceByName("dialog_message_series_create_document_type_series_miss_year")
                );
                //Disable Button, Extra protection for deleted year outside App
                ButtonCreateDocumentFinanceSeries.Sensitive = false;
            }

        }

        public static bool UICreateDocumentFinanceYearSeriesTerminal(Window parentWindow, fin_documentfinanceyears pDocumentFinanceYear)
        {
            bool result = false;

            XPCollection xpcConfigurationPlaceTerminal = new XPCollection(XPOSettings.Session, typeof(pos_configurationplaceterminal));
            xpcConfigurationPlaceTerminal.Reload();

            //Get Terminals
            DataTable dataTableSelectedTerminals = PosSelectRecordDialog<DataTable, DataRow, TreeViewTerminalSeries>.GetSelected(parentWindow);

            //Store Reference to BackOffice TreeViewDocumentFinanceYearSerieTerminal
            TreeViewDocumentFinanceSeries treeViewDocumentFinanceSeries =
                ((parentWindow as BackOfficeWindow).Menu.Nodes["TopMenuDocuments"].Children.ContainsKey("DocumentFinanceSeries"))
                ? ((parentWindow as BackOfficeWindow).Menu.Nodes["TopMenuDocuments"].Children["DocumentFinanceSeries"].Content as TreeViewDocumentFinanceSeries)
                : null;

            //Store Reference to BackOffice TreeViewDocumentFinanceYearSerieTerminal
            TreeViewDocumentFinanceYearSerieTerminal treeViewDocumentFinanceYearSerieTerminal =
                ((parentWindow as BackOfficeWindow).Menu.Nodes["TopMenuDocuments"].Children.ContainsKey("DocumentFinanceYearSerieTerminal"))
                ? ((parentWindow as BackOfficeWindow).Menu.Nodes["TopMenuDocuments"].Children["DocumentFinanceYearSerieTerminal"].Content as TreeViewDocumentFinanceYearSerieTerminal)
                : null;

            if (dataTableSelectedTerminals != null && dataTableSelectedTerminals.Rows.Count > 0)
            {
                //Request Acronym
                //string initialValue = string.Format("{0}{1}", pDocumentFinanceYear.Acronym, "01");
                string initialValue = pDocumentFinanceYear.Acronym;
                logicpos.Utils.ResponseText resultAcronym = PosConfirmAcronymSeriesDialog(parentWindow, pDocumentFinanceYear, dataTableSelectedTerminals, initialValue);

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
                            LogicPOSAppContext.StartupWindow,
                            DialogFlags.Modal,
                            MessageType.Question,
                            ButtonsType.YesNo,
                            GeneralUtils.GetResourceByName("window_title_series_create_series"),
                            GeneralUtils.GetResourceByName("dialog_message_series_create_confirmation_text")
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
                        FrameworkCallResult frameworkCallsResult = DocumentProcessingSeriesUtils.CreateDocumentFinanceYearSeriesTerminal(pDocumentFinanceYear, dataTableSelectedTerminals, resultAcronym.Text, false);
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
                                parentWindow,
                                DialogFlags.Modal,
                                MessageType.Error,
                                ButtonsType.Ok,
                                GeneralUtils.GetResourceByName("global_error"),
                                string.Format("{0}{1}{1}{2}",
                                    string.Format(GeneralUtils.GetResourceByName("dialog_message_series_create_error"), resultAcronym.Text),
                                    Environment.NewLine,
                                    frameworkCallsResult.Exception.InnerException.Message
                                )
                             );
                        }
                    }

                }

            }


            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Helpers

        //UI Helper to Request Acronym for Terminal Series

        public static logicpos.Utils.ResponseText PosConfirmAcronymSeriesDialog(Window parentWindow, fin_documentfinanceyears pDocumentFinanceYear, DataTable pTerminals, string pInitialValue)
        {
            logicpos.Utils.ResponseText result = new logicpos.Utils.ResponseText();
            FrameworkCallResult frameworkCallsResult;

            string fileWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_input_text_default.png";

            PosInputTextDialog dialog = new PosInputTextDialog(
                parentWindow,
                DialogFlags.Modal,
                new System.Drawing.Size(800, 600),
                GeneralUtils.GetResourceByName("window_title_series_create_series"),
                fileWindowIcon,
                GeneralUtils.GetResourceByName("global_acronym"),
                pInitialValue,
                RegexUtils.RegexDocumentSeriesAcronym,
                true
                );

            //Initialize EntryBoxValidationMultiLine
            EntryBoxValidationMultiLine entryBoxValidationMultiLine = new EntryBoxValidationMultiLine(parentWindow, GeneralUtils.GetResourceByName("global_preview"));
            entryBoxValidationMultiLine.HeightRequest = 420;
            entryBoxValidationMultiLine.EntryMultiline.TextView.WrapMode = WrapMode.Word;
            entryBoxValidationMultiLine.EntryMultiline.TextView.Sensitive = false;

            //Start with Preview
            frameworkCallsResult = DocumentProcessingSeriesUtils.CreateDocumentFinanceYearSeriesTerminal(pDocumentFinanceYear, pTerminals, dialog.EntryBoxValidation.EntryValidation.Text, true);
            entryBoxValidationMultiLine.EntryMultiline.TextView.Buffer.Text = frameworkCallsResult.Output;

            //Pack Widgets
            dialog.VBoxContent.PackStart(entryBoxValidationMultiLine, true, true, 0);
            dialog.VBoxContent.ShowAll();
            dialog.EntryBoxValidation.EntryValidation.Changed += delegate
            {
                if (dialog.EntryBoxValidation.EntryValidation.Validated)
                {
                    frameworkCallsResult = DocumentProcessingSeriesUtils.CreateDocumentFinanceYearSeriesTerminal(pDocumentFinanceYear, pTerminals, dialog.EntryBoxValidation.EntryValidation.Text, true);
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


            return result;
        }
    }
}
