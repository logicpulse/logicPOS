using CryptographyUtils;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.Widgets.Entrys;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Logic.Others;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.Classes.Finance;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace logicpos
{
    class Utils : logicpos.shared.Classes.Utils.Utils
    {

        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //private static DatabaseType pDatabaseType;
        Hashtable commands = new Hashtable();

        public Dictionary<string, AccordionNode> _accordionChildDocumentsTemp = new Dictionary<string, AccordionNode>();
        public Dictionary<string, AccordionNode> _accordionChildArticlesTemp = new Dictionary<string, AccordionNode>();
        public Dictionary<string, AccordionNode> _accordionChildCostumersTemp = new Dictionary<string, AccordionNode>();
        public Dictionary<string, AccordionNode> _accordionChildUsersTemp = new Dictionary<string, AccordionNode>();
        public Dictionary<string, AccordionNode> _accordionChildOtherTablesTemp = new Dictionary<string, AccordionNode>();


        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Colours

        //Convert Color to HexColor
        public static string Color2Hex(Color pRgb)
        {
            string red = Convert.ToString(pRgb.R, 16);
            if (red.Length < 2) red = "0" + red;
            string green = Convert.ToString(pRgb.G, 16);
            if (green.Length < 2) green = "0" + green;
            string blue = Convert.ToString(pRgb.B, 16);
            if (blue.Length < 2) blue = "0" + blue;

            return red.ToUpper() + green.ToUpper() + blue.ToUpper();
        }

        //Convert HexColor to Color
        /*
        public static Color Hex2Color(string pHexData)
        {
          if (pHexData.Length != 6)
            return Color.Black;

          string r_text, g_text, b_text;
          int r, g, b;

          r_text = pHexData.Substring(0, 2);
          g_text = pHexData.Substring(2, 2);
          b_text = pHexData.Substring(4, 2);

          r = int.Parse(r_text, System.Globalization.NumberStyles.HexNumber);
          g = int.Parse(g_text, System.Globalization.NumberStyles.HexNumber);
          b = int.Parse(b_text, System.Globalization.NumberStyles.HexNumber);

          return Color.FromArgb(r, g, b);
        }
        */

        public static string GetCurrentDirectory()
        {
            string currentDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //if (!currentDir.EndsWith(@"\"))
            //{
            //    currentDir = currentDir + @"/";
            //}

            return currentDir;
        }


        //2015-05-13 apmuga para suportar alfa e não devolver sempre preto
        public static Color Hex2Color(string pHexData)
        {
            int tmpCutFirstChars = 0;
            string tmpAlfa, tmpRed, tmpGreen, tmpBlue;
            int tmpAlfaValue = 0, tmpRedValue, tmpGreenValue, tmpBlueValue;

            if (pHexData.StartsWith("#"))
            {
                tmpCutFirstChars++;
            }

            if (pHexData.Length == 9)
            {
                tmpAlfa = pHexData.Substring(tmpCutFirstChars, 2);

                tmpAlfaValue = int.Parse(tmpAlfa, System.Globalization.NumberStyles.HexNumber);
                tmpCutFirstChars += 2;
            }

            tmpRed = pHexData.Substring(0 + tmpCutFirstChars, 2);
            tmpGreen = pHexData.Substring(2 + tmpCutFirstChars, 2);
            tmpBlue = pHexData.Substring(4 + tmpCutFirstChars, 2);

            tmpRedValue = int.Parse(tmpRed, System.Globalization.NumberStyles.HexNumber);
            tmpGreenValue = int.Parse(tmpGreen, System.Globalization.NumberStyles.HexNumber);
            tmpBlueValue = int.Parse(tmpBlue, System.Globalization.NumberStyles.HexNumber);

            return Color.FromArgb(tmpAlfaValue, tmpRedValue, tmpGreenValue, tmpBlueValue);
        }

        //Ligth color by correctionFactor
        public static Color Lighten(Color pInColor, float pCorrectionFactor = 0.25f)
        {
            try
            {
                float red = (255 - pInColor.R) * pCorrectionFactor + pInColor.R;
                float green = (255 - pInColor.G) * pCorrectionFactor + pInColor.G;
                float blue = (255 - pInColor.B) * pCorrectionFactor + pInColor.B;
                //check
                if (red > 255) red = 0;
                if (green > 255) green = 0;
                if (blue > 255) blue = 0;
                return Color.FromArgb(pInColor.A, (int)red, (int)green, (int)blue);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return pInColor;
            }
        }

        //Dark color by correctionFactor
        public static Color Darken(Color pInColor, float pCorrectionFactor = 0.25f)
        {
            try
            {
                float red = -(255 - pInColor.R) * pCorrectionFactor + pInColor.R;
                float green = -(255 - pInColor.G) * pCorrectionFactor + pInColor.G;
                float blue = -(255 - pInColor.B) * pCorrectionFactor + pInColor.B;
                //check
                if (red < 0) red = 0;
                if (green < 0) green = 0;
                if (blue < 0) blue = 0;
                return Color.FromArgb(pInColor.A, (int)red, (int)green, (int)blue);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return pInColor;
            }
        }

        //Converts System.Drawing.Color to Gdk.Color
        public static Gdk.Color ColorToGdkColor(System.Drawing.Color pColor)
        {
            return new Gdk.Color(pColor.R, pColor.G, pColor.B);
        }

        //Converts String to Gdk.Color
        public static Gdk.Color StringToGdkColor(string pColor)
        {
            return Utils.ColorToGdkColor(FrameworkUtils.StringToColor(pColor));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DataBase

        public static void AssignConnectionStringToSettings(string pConnectionString)
        {
            Dictionary<string, string> connectionStringToDictionary = ConnectionStringToDictionary(pConnectionString);

            switch (GlobalFramework.DatabaseType)
            {
                case DatabaseType.SQLite:
                case DatabaseType.MonoLite:
                    break;
                case DatabaseType.MSSqlServer:
                    GlobalFramework.DatabaseServer = connectionStringToDictionary["Data Source"];
                    GlobalFramework.DatabaseUser = connectionStringToDictionary["User ID"];
                    GlobalFramework.DatabasePassword = connectionStringToDictionary["Password"];
                    break;
                case DatabaseType.MySql:
                    GlobalFramework.DatabaseServer = connectionStringToDictionary["server"];
                    GlobalFramework.DatabaseUser = connectionStringToDictionary["user id"];
                    GlobalFramework.DatabasePassword = connectionStringToDictionary["password"];
                    break;
            }
        }

        public static Dictionary<string, string> ConnectionStringToDictionary(string pConnectionString)
        {
            try
            {
                Dictionary<string, string> connStringParts = pConnectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                  .Select(t => t.Split(new char[] { '=' }, 2))
                  .ToDictionary(t => t[0].Trim(), t => t[1].Trim(), StringComparer.InvariantCultureIgnoreCase);

                return connStringParts;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                throw;
            }
        }

        
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ShowMessage Non Touch

        //Call with : Utils.ShowMessage(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Message Test", "Error");
        public static Gtk.ResponseType ShowMessageNonTouch(Gtk.Window pSourceWindow, Gtk.DialogFlags pDialogFlags, MessageType pMsgType, ButtonsType pButtonsType, string pMessage, String pWindowTitle)
        {
            MessageDialog messageDialog = new MessageDialog(pSourceWindow, pDialogFlags, pMsgType, pButtonsType, pMessage);
            messageDialog.Title = pWindowTitle;
            ResponseType responseType = (Gtk.ResponseType)messageDialog.Run();
            messageDialog.Destroy();

            return responseType;
        }

        public static void ShowMessageUnderConstruction()
        {
            _log.Warn(string.Format("ShowMessageUnderConstruction(): {0} {1} ", MessageType.Error, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_under_construction_function")));
            ShowMessageNonTouch(null, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_under_construction_function"), "Error");
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ShowMessage Touch

        //Touch Dialog
        public static ResponseType ShowMessageTouch(Window pSourceWindow, Gtk.DialogFlags pDialogFlags, MessageType pMessageType, ButtonsType pButtonsType, string pWindowTitle, string pMessage)
        {
            //Default Size
            Size size = new Size(600, 400);
            return ShowMessageTouch(pSourceWindow, pDialogFlags, size, pMessageType, pButtonsType, pWindowTitle, pMessage);
        }

        public static ResponseType ShowMessageTouch(Window pSourceWindow, Gtk.DialogFlags pDialogFlags, Size pSize, MessageType pMessageType, ButtonsType pButtonsType, string pWindowTitle, string pMessage)
        {
            //Settings
            Color colorBaseDialogActionAreaButtonBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogActionAreaButtonBackground"]);
            Color colorBaseDialogActionAreaButtonFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogActionAreaButtonFont"]);
            Size sizeBaseDialogActionAreaButtonIcon = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogActionAreaButtonIcon"]);
            Size sizeBaseDialogActionAreaButton = Utils.StringToSize(GlobalFramework.Settings["sizeBaseDialogActionAreaButton"]);
            String fontBaseDialogActionAreaButton = FrameworkUtils.OSSlash(GlobalFramework.Settings["fontBaseDialogActionAreaButton"]);
            //Images
            String fileImageDialogBaseMessageTypeImage = FrameworkUtils.OSSlash(GlobalFramework.Settings["fileImageDialogBaseMessageTypeImage"]);
            String fileImageDialogBaseMessageTypeIcon = FrameworkUtils.OSSlash(GlobalFramework.Settings["fileImageDialogBaseMessageTypeIcon"]);
            //Files
            String fileActionOK = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_ok.png");
            String fileActionCancel = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png");
            String fileActionYes = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_yes.png");
            String fileActionNo = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_no.png");
            String fileActionClose = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Dialogs\icon_pos_dialog_action_close.png");
            //Init Local Vars
            String fileImageDialog, fileImageWindowIcon;
            String messageType = string.Empty;
            ResponseType resultResponse = ResponseType.None;

            //Prepara ActionArea and Buttons
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            //Init Buttons
            TouchButtonIconWithText buttonOk = new TouchButtonIconWithText("touchButtonOk_DialogActionArea", colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_ok"), fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, fileActionOK, sizeBaseDialogActionAreaButtonIcon, sizeBaseDialogActionAreaButton.Width, sizeBaseDialogActionAreaButton.Height);
            TouchButtonIconWithText buttonCancel = new TouchButtonIconWithText("touchButtonCancel_DialogActionArea", colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_cancel"), fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, fileActionCancel, sizeBaseDialogActionAreaButtonIcon, sizeBaseDialogActionAreaButton.Width, sizeBaseDialogActionAreaButton.Height);
            TouchButtonIconWithText buttonYes = new TouchButtonIconWithText("touchButtonYes_DialogActionArea", colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_yes"), fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, fileActionYes, sizeBaseDialogActionAreaButtonIcon, sizeBaseDialogActionAreaButton.Width, sizeBaseDialogActionAreaButton.Height);
            TouchButtonIconWithText buttonNo = new TouchButtonIconWithText("touchButtonNo_DialogActionArea", colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_no"), fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, fileActionNo, sizeBaseDialogActionAreaButtonIcon, sizeBaseDialogActionAreaButton.Width, sizeBaseDialogActionAreaButton.Height);
            TouchButtonIconWithText buttonClose = new TouchButtonIconWithText("touchButtonClose_DialogActionArea", colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_close"), fontBaseDialogActionAreaButton, colorBaseDialogActionAreaButtonFont, fileActionClose, sizeBaseDialogActionAreaButtonIcon, sizeBaseDialogActionAreaButton.Width, sizeBaseDialogActionAreaButton.Height);

            //Perpare ActionAreaButtons
            switch (pButtonsType)
            {
                case ButtonsType.Ok:
                    actionAreaButtons.Add(new ActionAreaButton(buttonOk, ResponseType.Ok));
                    break;
                case ButtonsType.Cancel:
                    actionAreaButtons.Add(new ActionAreaButton(buttonCancel, ResponseType.Cancel));
                    break;
                case ButtonsType.OkCancel:
                    actionAreaButtons.Add(new ActionAreaButton(buttonOk, ResponseType.Ok));
                    actionAreaButtons.Add(new ActionAreaButton(buttonCancel, ResponseType.Cancel));
                    break;
                case ButtonsType.YesNo:
                    actionAreaButtons.Add(new ActionAreaButton(buttonYes, ResponseType.Yes));
                    actionAreaButtons.Add(new ActionAreaButton(buttonNo, ResponseType.No));
                    break;
                case ButtonsType.Close:
                    actionAreaButtons.Add(new ActionAreaButton(buttonClose, ResponseType.Close));
                    break;
                case ButtonsType.None:
                    break;
                default:
                    break;
            }

            //Prepare Images
            messageType = Enum.GetName(typeof(MessageType), pMessageType).ToLower();

            if (messageType != string.Empty)
            {
                fileImageDialog = string.Format(fileImageDialogBaseMessageTypeImage, messageType);
                fileImageWindowIcon = string.Format(fileImageDialogBaseMessageTypeIcon, messageType);
            }
            else
            {
                fileImageDialog = string.Empty;
                fileImageWindowIcon = string.Empty;
            }

            //Prepare Dialog
            PosMessageDialog dialog = new PosMessageDialog(pSourceWindow, pDialogFlags, pSize, pWindowTitle, pMessage, actionAreaButtons, fileImageWindowIcon, fileImageDialog);
            //Hide WindowTitleCloseButton, Force user to use buttons
            dialog.WindowTitleCloseButton = false;

            //Call Dialog and Return Result
            try
            {
                resultResponse = (ResponseType)dialog.Run();
                return resultResponse;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return resultResponse;
            }
            finally
            {
                dialog.Destroy();
            }
        }

        public static ResponseType ShowMessageTouchUnderConstruction(Window pSourceWindow)
        {
            ResponseType responseType = ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_under_construction_function"));
            _log.Debug(string.Format("ShowMessageUnderConstruction(): {0} {1} ", MessageType.Error, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_under_construction_function")));
            _log.Debug(string.Format("responseType: [{0}]", responseType));
            return responseType;
        }

        internal static ResponseType ShowMessageTouchErrorPrintingTicket(Gtk.Window pSourceWindow, sys_configurationprinters pPrinter, Exception pEx)
        {
            //Protection when Printer is Null, ex printing Ticket Articles (Printer is Assign in Article)
            string printerDesignation = (pPrinter != null) ? pPrinter.Designation : "NULL";
            string printerNetworkName = (pPrinter != null) ? pPrinter.NetworkName : "NULL";
            return ShowMessageTouch(pSourceWindow, Gtk.DialogFlags.Modal, new Size(800, 400), Gtk.MessageType.Error, Gtk.ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_error_printing_ticket"), printerDesignation, printerNetworkName, pEx.Message));
        }

        public static bool ShowMessageTouchRequiredValidPrinter(Window pSourceWindow)
        {
            bool result = (GlobalFramework.LoggedTerminal.Printer != null) ? false : true;

            if (result)
            {
                Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_required_valid_printer"));
            }

            return result;
        }

        public static void ShowMessageTouchTerminalWithoutAssociatedPrinter(Window pSourceWindow, string pDocumentType)
        {
            if (
                (
                    GlobalApp.Notifications != null && GlobalApp.Notifications.ContainsKey("SHOW_PRINTER_UNDEFINED")
                )
                && GlobalApp.Notifications["SHOW_PRINTER_UNDEFINED"] == true
            )
            {
                ResponseType responseType = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(550, 400), MessageType.Question, ButtonsType.YesNo, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information")
                    , string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_show_printer_undefined_on_print"), pDocumentType)
                );
                if (responseType == ResponseType.No) GlobalApp.Notifications["SHOW_PRINTER_UNDEFINED"] = false;
            }
        }

        public static void ShowMessageTouchErrorRenderTheme(Window pSourceWindow, string pErrorMessage)
        {
            string errorMessage = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "app_error_rendering_theme"), SettingsApp.FileTheme, pErrorMessage);
            Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(600, 500), MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), errorMessage);
            Environment.Exit(0);
        }

        public static void ShowMessageTouchErrorUnlicencedFunctionDisabled(Window pSourceWindow, string pErrorMessage)
        {
            string errorMessage = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "app_error_application_unlicenced_function_disabled"), pErrorMessage);
            Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(600, 300), MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), errorMessage);
        }

        public static ResponseType ShowMessageTouchCheckIfFinanceDocumentHasValidDocumentDate(Window pSourceWindow, ProcessFinanceDocumentParameter pParameters)
        {
            //Default is Yes
            ResponseType result = ResponseType.Yes;

            try
            {
                fin_documentfinanceyearserieterminal documentFinanceYearSerieTerminal = null;
                fin_documentfinanceseries documentFinanceSerie = null;
                if (GlobalFramework.LoggedTerminal != null)
                {
                    documentFinanceYearSerieTerminal = ProcessFinanceDocumentSeries.GetDocumentFinanceYearSerieTerminal(pParameters.DocumentType);
                    if (documentFinanceYearSerieTerminal != null) documentFinanceSerie = documentFinanceYearSerieTerminal.Serie;
                }

                //If Dont Have Series Throw Exception to Caller Method
                if (documentFinanceSerie == null)
                {
                    throw new Exception("ERROR_MISSING_SERIE");
                }
                //Get Reference
                else
                {
                    documentFinanceSerie = documentFinanceYearSerieTerminal.Serie;
                }

                //WARNING_RULE_SYSTEM_DATE_SERIE
                DateTime dateLastDocumentFromSerie = DateTime.MaxValue;
                if (documentFinanceSerie != null)
                {
                    dateLastDocumentFromSerie = ProcessFinanceDocument.GetLastDocumentDateTime(string.Format("DocumentSerie = '{0}'", documentFinanceSerie.Oid)).Date;
                }

                //Check if DocumentDate is greater than dateLastDocumentFromSerie (If Defined) else if is First Document in Series Skip
                if (pParameters.DocumentDateTime < dateLastDocumentFromSerie && dateLastDocumentFromSerie != DateTime.MinValue)
                {
                    result = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Question, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_warning"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_systementry_is_less_than_last_finance_document_series"));
                }
                else
                {
                    //WARNING_RULE_SYSTEM_DATE_GLOBAL
                    DateTime dateTimeLastDocument = ProcessFinanceDocument.GetLastDocumentDateTime();
                    //Check if DocumentDate is greater than dateLastDocument (If Defined) else if is First Document in Series Skip
                    if (pParameters.DocumentDateTime < dateTimeLastDocument && dateTimeLastDocument != DateTime.MinValue)
                    {
                        result = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Question, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_warning"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_systementry_is_less_than_last_finance_document_series"));
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return result;
        }

        public static void ShowMessageTouchSimplifiedInvoiceMaxValueExceedForFinalConsumer(Window pSourceWindow, decimal pCurrentTotal, decimal pMaxTotal)
        {
            Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(550, 480), MessageType.Info, ButtonsType.Close,
                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_warning"),
                string.Format(
                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_value_exceed_simplified_invoice_for_final_or_annonymous_consumer")
                    , string.Format("{0}: {1}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total"), FrameworkUtils.DecimalToStringCurrency(pCurrentTotal))
                    , string.Format("{0}: {1}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_maximum"), FrameworkUtils.DecimalToStringCurrency(pMaxTotal))
                )
            );
        }

        public static ResponseType ShowMessageTouchSimplifiedInvoiceMaxValueExceed(Window pSourceWindow, ShowMessageTouchSimplifiedInvoiceMaxValueExceedMode pMode, decimal pCurrentTotal, decimal pMaxTotal, decimal pCurrentTotalServices, decimal pMaxTotalServices)
        {
            ResponseType result = ResponseType.No;

            string message = string.Empty;
            string messageMaxExceed = string.Empty;
            string messageMaxExceedServices = string.Empty;
            string messageMode = string.Empty;
            MessageType messageType = MessageType.Other;
            ButtonsType buttonsType = ButtonsType.None;

            switch (pMode)
            {
                case ShowMessageTouchSimplifiedInvoiceMaxValueExceedMode.PaymentsDialog:
                    messageMode = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_value_exceed_simplified_invoice_max_value_mode_paymentdialog");
                    messageType = MessageType.Question;
                    buttonsType = ButtonsType.YesNo;
                    break;
                case ShowMessageTouchSimplifiedInvoiceMaxValueExceedMode.DocumentFinanceDialog:
                    messageMode = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_value_exceed_simplified_invoice_max_value_mode_paymentdialog_documentfinancedialog");
                    messageType = MessageType.Info;
                    buttonsType = ButtonsType.Close;
                    break;
            }

            try
            {
                if (pCurrentTotal > pMaxTotal)
                {
                    messageMaxExceed = string.Format(
                        "{1}: {2}{0}{3}: {4}"
                        , Environment.NewLine
                        , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_total")
                        , FrameworkUtils.DecimalToStringCurrency(pCurrentTotal)
                        , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_maximum")
                        , FrameworkUtils.DecimalToStringCurrency(pMaxTotal)
                    );
                }

                if (pCurrentTotalServices > pMaxTotalServices)
                {
                    messageMaxExceedServices = string.Format(
                        "{1}: {2}{0}{3}: {4}"
                        , Environment.NewLine
                        , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_services")
                        , FrameworkUtils.DecimalToStringCurrency(pCurrentTotalServices)
                        , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_maximum")
                        , FrameworkUtils.DecimalToStringCurrency(pMaxTotalServices)
                    );
                }

                if (pCurrentTotal > pMaxTotal || pCurrentTotalServices > pMaxTotalServices)
                {
                    if (messageMaxExceed != string.Empty) message += messageMaxExceed;
                    if (messageMaxExceedServices != string.Empty)
                    {
                        //Add Seperate Lines if message is not Empty
                        if (message != string.Empty) message += string.Format("{0}{0}", Environment.NewLine);
                        message += messageMaxExceedServices;
                    }

                    result = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(550, 440), messageType, buttonsType,
                        resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_warning"),
                        string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_value_exceed_simplified_invoice_max_value"), message, messageMode
                        )
                    );
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public static void ShowMessageTouchProtectedDeleteRecordMessage(Window pSourceWindow)
        {
            ShowMessageTouch(
                pSourceWindow,
                DialogFlags.DestroyWithParent | DialogFlags.Modal,
                MessageType.Error,
                ButtonsType.Ok,
                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_delete_record"),
                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_delete_record_show_protected_record"))
            ;
        }

        public static void ShowMessageTouchProtectedUpdateRecordMessage(Window pSourceWindow)
        {
            ShowMessageTouch(
                pSourceWindow,
                DialogFlags.DestroyWithParent | DialogFlags.Modal,
                MessageType.Error,
                ButtonsType.Ok,
                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_update_record"),
                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_update_record_show_protected_record"))
            ;
        }

        public static void ShowMessageTouchUnsupportedResolutionDetectedAndExit(Window pSourceWindow, int width, int height)
        {
            string message = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "app_error_unsupported_resolution_detected"), width, height);
            Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(600, 300), MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), message);
            Environment.Exit(Environment.ExitCode);
        }

        /// <summary>
        /// Responsible for the dialogbox for screen resolution issues.
        /// It takes the control of application when user opt to do not follow with low-resolution app startup, closing the application.
        /// <para>See also "IN008023: apply "800x600" settings as default."</para>
        /// </summary>
        /// <param name="pSourceWindow"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void ShowMessageTouchUnsupportedResolutionDetectedDialogbox(Window pSourceWindow, int width, int height)
        {
            string message = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "app_error_unsupported_resolution_detected"), width, height, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_treeview_true"));
            ResponseType dialogResponse = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(600, 300), MessageType.Question, ButtonsType.YesNo, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), message);
            if (dialogResponse == ResponseType.No)
            {
                Environment.Exit(Environment.ExitCode);
            }
        }

        public static void ShowMessageTouchErrorTryToIssueACreditNoteExceedingSourceDocumentArticleQuantities(Window pSourceWindow, decimal currentQuantity, decimal maxPossibleQuantity)
        {
            string message = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_error_try_to_issue_a_credit_note_exceeding_source_document_article_quantities"), currentQuantity, maxPossibleQuantity);
            Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, new Size(700, 400), MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), message);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Request Text Dialog

        //Helper Response Object
        public class ResponseText
        {
            public ResponseType ResponseType;
            public string Text;

            public ResponseText() { }
            public ResponseText(ResponseType pResponseType, string pText)
            {
                ResponseType = pResponseType;
                Text = pText;
            }
        }

        public static ResponseText GetInputText(Window pSourceWindow, DialogFlags pDialogFlags, string pWindowIcon, string pEntryLabel, string pDefaultValue, string pRule, bool pRequired)
        {
            return GetInputText(pSourceWindow, pDialogFlags, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_default_input_text_dialog"), pWindowIcon, pEntryLabel, pDefaultValue, pRule, pRequired);
        }

        public static ResponseText GetInputText(Window pSourceWindow, DialogFlags pDialogFlags, string pWindowTitle, string pWindowIcon, string pEntryLabel, string pDefaultValue, string pRule, bool pRequired)
        {
            ResponseText result = new ResponseText();
            result.ResponseType = ResponseType.Cancel;

            //Prepare Dialog
            PosInputTextDialog dialog = new PosInputTextDialog(pSourceWindow, pDialogFlags, pWindowTitle, pWindowIcon, pEntryLabel, pDefaultValue, pRule, pRequired);

            //Call Dialog
            try
            {
                result.ResponseType = (ResponseType)dialog.Run();
                result.Text = dialog.Value;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            finally
            {
                dialog.Destroy();
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Images 

        public static Gdk.Pixmap FileToPixmap(String pFilename)
        {
            if (pFilename != null && File.Exists(pFilename))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    Gdk.Pixbuf image = new Gdk.Pixbuf(pFilename);
                    Gdk.Pixmap pixmap, pixmap_mask;
                    image.RenderPixmapAndMask(out pixmap, out pixmap_mask, 255);
                    //this.Style.SetBgPixmap(StateType.Normal, pixmap);
                    return pixmap;
                }
            }
            else
            {
                return null;
            }
        }

        public static Gdk.Pixmap PixbufToPixmap(Gdk.Pixbuf pFilename)
        {
            if (pFilename != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    Gdk.Pixbuf image = pFilename;
                    Gdk.Pixmap pixmap, pixmap_mask;
                    image.RenderPixmapAndMask(out pixmap, out pixmap_mask, 255);
                    //this.Style.SetBgPixmap(StateType.Normal, pixmap);
                    return pixmap;
                }
            }
            else
            {
                return null;
            }
        }

        //Convert System.Drawing.Image to Gdk.Image
        public static Gdk.Pixbuf ImageToPixbuf(System.Drawing.Image pImage)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                pImage.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(stream);
                return pixbuf;
            }
        }

        //Returns a PixBuf from File Path
        public static Gdk.Pixbuf FileToPixBuf(String pFilename)
        {
            if (pFilename != null && File.Exists(pFilename))
            {
                var buffer = System.IO.File.ReadAllBytes(pFilename);
                return new Gdk.Pixbuf(buffer);
            }
            else
            {
                return null;
            }
        }

        //Returns a Resized PixBuf from File Path - Usefull for FileChooserButtons
        public static Gdk.Pixbuf ResizeAndCropFileToPixBuf(String pFilename, Size pSize)
        {
            if (pFilename != null && File.Exists(pFilename))
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(pFilename);
                image = Utils.ResizeAndCrop(image, new System.Drawing.Size(pSize.Width, pSize.Height));
                return Utils.ImageToPixbuf(image);
            }
            else
            {
                return null;
            }
        }

        //ResizeImage
        public static System.Drawing.Image ResizeImage(System.Drawing.Image pImage, Size pSize)
        {
            try
            {
                //skip dont require processing
                if (pImage.Width == pSize.Width && pImage.Height == pSize.Height) return pImage;

                int sourceWidth = pImage.Width;
                int sourceHeight = pImage.Height;

                Bitmap bitmap = new Bitmap(pSize.Width, pSize.Height);
                Graphics graphics = Graphics.FromImage((System.Drawing.Image)bitmap);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(pImage, 0, 0, pSize.Width, pSize.Height);
                graphics.Dispose();

                return (System.Drawing.Image)bitmap;
            }
            catch
            {
                return pImage;
            }
        }

        //Crop
        public static System.Drawing.Image CropImage(System.Drawing.Image pImage, Rectangle pCropArea)
        {
            if (pImage.Width == pCropArea.Width && pImage.Height == pCropArea.Height)
                return pImage;

            try
            {
                Bitmap bmpImage = new Bitmap(pImage);

                //_log.Debug(
                //  "CropImage():" +
                //  ": image.Width:" + pImage.Width +
                //  ", image.Height:" + pImage.Height +
                //  ", bmpImage.Width:" + bmpImage.Width +
                //  ", bmpImage.Height:" + bmpImage.Height +
                //  ", cropArea.X:" + pCropArea.X +
                //  ", cropArea.Y:" + pCropArea.Y +
                //  ", cropArea.Width:" + pCropArea.Width +
                //  ", cropArea.Height:" + pCropArea.Height
                //);

                Bitmap bmpCrop = bmpImage.Clone(pCropArea, bmpImage.PixelFormat);
                return (System.Drawing.Image)(bmpCrop);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return pImage;
            }
        }

        //Shared function ResizeAndCrop to reuse both ResizeImage and CropImage
        public static System.Drawing.Image ResizeAndCrop(System.Drawing.Image pImage, Size pSize)
        {
            //store target resize size
            Size resizeImageSize = new Size();
            int iCropOffsetX, iCropOffsetY;

            //find optimal ResizeSize
            resizeImageSize = ImageFindResizeSize(pImage.Width, pImage.Height, pSize.Width, pSize.Height);

            //ResizeImage
            pImage = ResizeImage(pImage, resizeImageSize);
            //drawingOverlay.Save(image + ".resized.png");
            //crop image
            iCropOffsetX = (resizeImageSize.Width - pSize.Width) / 2;
            iCropOffsetY = (resizeImageSize.Height - pSize.Height) / 2;
            Rectangle rectCropArea = new Rectangle(iCropOffsetX, iCropOffsetY, pSize.Width, pSize.Height);
            pImage = CropImage(pImage, rectCropArea);
            //drawingImage.Save(image + ".croped.png");

            //_log.Debug(
            //  "ResizeAndCrop()" +
            //  ": resizeImageSize.Width:" + resizeImageSize.Width +
            //  ", resizeImageSize.Height:" + resizeImageSize.Height +
            //  ", iCropOffsetX:" + iCropOffsetX +
            //  ", iCropOffsetY:" + iCropOffsetY +
            //  ", Final image.Width:" + pImage.Width +
            //  ", Final image.Height:" + pImage.Height
            //);

            return pImage;
        }

        //Find Target Resize size based on Image Size and Target Size, used before Crop
        public static Size ImageFindResizeSize(int pImageWidth, int pImageHeight, int pTargetImageWidth, int pTargetImageHeight)
        {
            int targetResizeImageWidth, targetResizeImageHeight, iHelper1, iHelper2, iHelper3;

            if (pImageWidth > pImageHeight)
            {
                iHelper1 = pImageWidth;
                iHelper2 = pTargetImageHeight;
                iHelper3 = pImageHeight;
                targetResizeImageWidth = (int)((float)iHelper1 * (float)iHelper2 / (float)iHelper3);
                targetResizeImageHeight = pTargetImageHeight;
                //required PROTECTION, para nunca calacular abaixo da targetImageWight, senao qnd as imagens sao mais pequenas da problemas
                if (targetResizeImageWidth < pTargetImageWidth)
                {
                    targetResizeImageWidth = pTargetImageWidth;
                    targetResizeImageHeight = (int)((float)pImageHeight * (float)pTargetImageWidth / (float)pImageWidth);
                }
            }
            else
            {
                iHelper1 = pImageHeight;
                iHelper2 = pTargetImageWidth;
                iHelper3 = pImageWidth;
                targetResizeImageWidth = pTargetImageWidth;
                targetResizeImageHeight = (int)((float)iHelper1 * (float)iHelper2 / (float)iHelper3);
                //required PROTECTION, para nunca calacular abaixo da targetImageHeight, senao qnd as imagens sao mais pequenas da problemas
                if (targetResizeImageHeight < pTargetImageHeight)
                {
                    targetResizeImageWidth = (int)((float)pImageWidth * (float)pTargetImageHeight / (float)pImageHeight);
                    targetResizeImageHeight = pTargetImageHeight;
                }
            };

            //_log.Debug(
            //  "ImageFindResizeSize()" +
            //  ": imageWidth=" + pImageWidth +
            //  ", imageHeight=" + pImageHeight +
            //  ", targetImageWidth:" + pTargetImageWidth +
            //  ", targetImageHeight:" + pTargetImageHeight +
            //  ", targetResizeImageWidth:" + targetResizeImageWidth +
            //  ", targetResizeImageHeight:" + targetResizeImageHeight +
            //  ", iHelper1:" + iHelper1 +
            //  ", iHelper2:" + iHelper2 +
            //  ", iHelper3:" + iHelper3
            //);

            return new Size(targetResizeImageWidth, targetResizeImageHeight);
        }

        //Render text over Image 
        public static System.Drawing.Image ImageTextOverlay(System.Drawing.Image pImage, String pLabel, Rectangle pTranspRectangle, Color pFontColor, String pFontName = "Tahoma", int pFontSize = 12, int pTransparentLevel = 128)
        {
            Graphics g = Graphics.FromImage(pImage);
            SolidBrush semiTransBrushWhite = new SolidBrush(Color.FromArgb(pTransparentLevel, 255, 255, 255));
            SolidBrush semiTransBrushBlack = new SolidBrush(Color.FromArgb(255, pFontColor.R, pFontColor.G, pFontColor.B));

            g.FillRectangle(semiTransBrushWhite, pTranspRectangle);
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;
            strFormat.LineAlignment = StringAlignment.Center;
            Font font = new Font(pFontName, pFontSize, FontStyle.Regular);
            g.DrawString(pLabel, font, semiTransBrushBlack, pTranspRectangle, strFormat);

            return pImage;
        }

        //Draw RoundedCornerRectangles in graphics
        public static void DrawRoundedRectangle(Graphics pGraphics, Rectangle pBounds, int pCornerRadius, Pen pDrawPen, Color pFillColor)
        {
            int strokeOffset = Convert.ToInt32(Math.Ceiling(pDrawPen.Width));
            pBounds = Rectangle.Inflate(pBounds, -strokeOffset, -strokeOffset);

            pDrawPen.EndCap = pDrawPen.StartCap = LineCap.Round;

            GraphicsPath graphicsPath = new GraphicsPath();

            if (pCornerRadius > 0)
            {
                graphicsPath.AddArc(pBounds.X, pBounds.Y, pCornerRadius, pCornerRadius, 180, 90);
                graphicsPath.AddArc(pBounds.X + pBounds.Width - pCornerRadius, pBounds.Y, pCornerRadius, pCornerRadius, 270, 90);
                graphicsPath.AddArc(pBounds.X + pBounds.Width - pCornerRadius, pBounds.Y + pBounds.Height - pCornerRadius, pCornerRadius, pCornerRadius, 0, 90);
                graphicsPath.AddArc(pBounds.X, pBounds.Y + pBounds.Height - pCornerRadius, pCornerRadius, pCornerRadius, 90, 90);
            }
            graphicsPath.CloseAllFigures();

            pGraphics.FillPath(new SolidBrush(pFillColor), graphicsPath);
            pGraphics.DrawPath(pDrawPen, graphicsPath);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Multimedia

        public static void ButtonSoundClick()
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = "aplay";
            //TODO: Put Sound in config
            proc.StartInfo.Arguments = "-t wav " + FrameworkUtils.OSSlash(GlobalFramework.Path["sounds"] + @"Clicks\button2.wav");
            proc.Start();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // TypeConverter

        public static Gdk.Color StringToGTKColor(string pColor)
        {
            Gdk.Color result = new Gdk.Color(255, 0, 0);
            try
            {
                string[] splitted = pColor.Split(',');

                try
                {
                    result = new Gdk.Color(Convert.ToByte(splitted[0]), Convert.ToByte(splitted[1]), Convert.ToByte(splitted[2]));
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return (result);
        }

        //Converts a Size to String using TypeConverter, used to Store values in Appsettings
        public static String SizeToString(Size pSize)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Size));
                return converter.ConvertToInvariantString(pSize);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return "0, 0";
            }
        }

        //Converts a String to Size using TypeConverter, used to Store values in Appsettings
        public static Size StringToSize(String pSize)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Size));
                return (Size)converter.ConvertFromInvariantString(pSize);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return new Size();
            }
        }

        public static Position StringToPosition(String pPosition)
        {
            string[] splitted = pPosition.Split(',');
            Position resultPosition = new Position();
            try
            {
                resultPosition = new Position(Convert.ToInt16(splitted[0]), Convert.ToInt16(splitted[1]));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return resultPosition;
            }
            return resultPosition;
        }

        public static TableConfig StringToTableConfig(String pTableConfig)
        {
            string[] splitted = pTableConfig.Split(',');
            TableConfig resultTableConfig = new TableConfig();
            try
            {
                resultTableConfig = new TableConfig(Convert.ToUInt16(splitted[0]), Convert.ToUInt16(splitted[1]));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return resultTableConfig;
            }
            return resultTableConfig;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Screen Shoots

        public static Gdk.Pixbuf ScreenCapture()
        {
            String tempPath = Convert.ToString(GlobalFramework.Path["temp"]);

            Gdk.Window window = Gdk.Global.DefaultRootWindow;
            if (window != null)
            {
                Gdk.Pixbuf pixBuf = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 8, window.Screen.Width, window.Screen.Height);
                pixBuf.GetFromDrawable(window, Gdk.Colormap.System, 0, 0, 0, 0, window.Screen.Width, window.Screen.Height);
                pixBuf.SaturateAndPixelate(pixBuf, 0.025F, false);
                pixBuf.ScaleSimple(400, 300, Gdk.InterpType.Bilinear);
                pixBuf.Save(tempPath + "screenshoot.png", "png");
                return pixBuf;
            }
            else
            {
                return null;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //UnicodeHexadecimal 00C1

        public static Char UnicodeHexadecimalStringToChar(String pInput)
        {
            char result = (char)int.Parse(pInput, NumberStyles.HexNumber);
            return result;
        }

        //UnicodeJavascript \u00C1
        public static Char UnicodeJavascriptStringToChar(String pInput)
        {
            char result = (char)int.Parse(pInput.Substring(2), NumberStyles.HexNumber);
            return result;
        }

        //Check if is letter with RegEx, better than Char.IsLetter that returns º and ª too
        public static bool IsLetter(Char input)
        {
            string pattern = @"^[a-zA-ZçÇÁáÉéÍóÚúÀàÈèÌìÒòÙùÃãÕõÂâÊêÎîÔôÛû]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(Convert.ToString(input));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Reflection - Not Used but Maybe Usefull

        public static FieldInfo[] GetFieldInfosIncludingBaseClasses(Type pType)
        {
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public;
            return GetFieldInfosIncludingBaseClasses(pType, bindingFlags);
        }

        public static FieldInfo[] GetFieldInfosIncludingBaseClasses(Type pType, BindingFlags pBindingFlags)
        {
            FieldInfo[] fieldInfo = pType.GetFields(pBindingFlags);

            // If this class doesn't have a base, don't waste any time
            if (pType.BaseType == typeof(object))
            {
                return fieldInfo;
            }
            else
            { // Otherwise, collect all types up to the furthest base class
                var fieldInfoList = new List<FieldInfo>(fieldInfo);
                while (pType.BaseType != typeof(object))
                {
                    pType = pType.BaseType;
                    fieldInfo = pType.GetFields(pBindingFlags);

                    // Look for fields we do not have listed yet and merge them into the main list
                    for (int index = 0; index < fieldInfo.Length; ++index)
                    {
                        bool found = false;

                        for (int searchIndex = 0; searchIndex < fieldInfoList.Count; ++searchIndex)
                        {
                            bool match =
                                (fieldInfoList[searchIndex].DeclaringType == fieldInfo[index].DeclaringType) &&
                                (fieldInfoList[searchIndex].Name == fieldInfo[index].Name);

                            if (match)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            fieldInfoList.Add(fieldInfo[index]);
                        }
                    }
                }
                return fieldInfoList.ToArray();
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FileFilters

        public static FileFilter GetFileFilterImages()
        {
            FileFilter filter = new FileFilter();
            filter.Name = "PNG and JPEG images";
            filter.AddMimeType("image/png");
            filter.AddPattern("*.png");
            filter.AddMimeType("image/jpeg");
            filter.AddPattern("*.jpg");
            return filter;
        }

        public static FileFilter GetFileFilterTemplates()
        {
            FileFilter filter = new FileFilter();
            filter.Name = "Printing Templates";
            filter.AddMimeType("Template/xml");
            filter.AddPattern("*.poson");
            return filter;
        }

        public static FileFilter GetFileFilterBackups()
        {
            string databaseType = GlobalFramework.Settings["databaseType"];
            FileFilter filter = new FileFilter();

            filter.Name = "Database Backups";
            /* OLD Method With diferent Extension, Now we use .bak for all BAckup Types
            switch (databaseType)
            {
                case "MSSqlServer":
                    filter.AddMimeType("application/octet-stream");
                    filter.AddPattern("*.bak");
                    break;
                case "SQLite":
                    filter.AddMimeType("application/octet-stream");
                    filter.AddPattern("*.db");
                    break;
                case "MySql":
                    filter.AddMimeType("text/sql");
                    filter.AddPattern("*.sql");
                    break;
                default:
                    break;
            }
            */

            //New: Shared for All database Types
            filter.AddMimeType("application/octet-stream");
            filter.AddPattern("*.bak");

            return filter;
        }

        public static FileFilter GetFileFilterImportExport()
        {

            FileFilter filter = new FileFilter();

            filter.Name = "Import/export xls";
            /* OLD Method With diferent Extension, Now we use .bak for all BAckup Types
            switch (databaseType)
            {
                case "MSSqlServer":
                    filter.AddMimeType("application/octet-stream");
                    filter.AddPattern("*.bak");
                    break;
                case "SQLite":
                    filter.AddMimeType("application/octet-stream");
                    filter.AddPattern("*.db");
                    break;
                case "MySql":
                    filter.AddMimeType("text/sql");
                    filter.AddPattern("*.sql");
                    break;
                default:
                    break;
            }
            */

            //New: Shared for All database Types
            filter.AddMimeType("application/octet-stream");
            filter.AddPattern("*.xls");
            filter.AddPattern("*.xlsx");
            return filter;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Localization

        //Test with Utils.ShowCultureInfo(GlobalFramework.CurrentCulture.ToString());
        public static void ShowCultureInfo(String pCulture)
        {
            // Creates and initializes the CultureInfo which uses the international sort.
            CultureInfo myCIintl = new CultureInfo(pCulture, false);

            // Creates and initializes the CultureInfo which uses the traditional sort.
            CultureInfo myCItrad = new CultureInfo(0x040A, false);

            // Displays the properties of each culture.

            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "PROPERTY", "INTERNATIONAL", "TRADITIONAL"));
            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "CompareInfo", myCIintl.CompareInfo, myCItrad.CompareInfo));
            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "DisplayName", myCIintl.DisplayName, myCItrad.DisplayName));
            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "EnglishName", myCIintl.EnglishName, myCItrad.EnglishName));
            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "IsNeutralCulture", myCIintl.IsNeutralCulture, myCItrad.IsNeutralCulture));
            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "IsReadOnly", myCIintl.IsReadOnly, myCItrad.IsReadOnly));
            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "LCID", myCIintl.LCID, myCItrad.LCID));
            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "Name", myCIintl.Name, myCItrad.Name));
            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "NativeName", myCIintl.NativeName, myCItrad.NativeName));
            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "Parent", myCIintl.Parent, myCItrad.Parent));
            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "TextInfo", myCIintl.TextInfo, myCItrad.TextInfo));
            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "ThreeLetterISOLanguageName", myCIintl.ThreeLetterISOLanguageName, myCItrad.ThreeLetterISOLanguageName));
            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "ThreeLetterWindowsLanguageName", myCIintl.ThreeLetterWindowsLanguageName, myCItrad.ThreeLetterWindowsLanguageName));
            _log.Debug(string.Format("{0,-33}{1,-25}{2,-25}", "TwoLetterISOLanguageName", myCIintl.TwoLetterISOLanguageName, myCItrad.TwoLetterISOLanguageName));
            _log.Debug("");
            // Compare two strings using myCIintl.
            _log.Debug("Comparing \"llegar\" and \"lugar\"");
            _log.Debug(string.Format("   With myCIintl.CompareInfo.Compare: {0}", myCIintl.CompareInfo.Compare("llegar", "lugar")));
            _log.Debug(string.Format("   With myCItrad.CompareInfo.Compare: {0}", myCItrad.CompareInfo.Compare("llegar", "lugar")));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Windows

        public static string GetWindowTitle(String pTitle)
        {
            //return string.Format("{0} {1} : {2}", SettingsApp.AppName, FrameworkUtils.ProductVersion, pTitle);
            return string.Format("{0} : {1}", SettingsApp.AppName, pTitle);
        }

        /// <summary>
        /// This method is responsible for dinamic dialog title creation, based on option chosen by user on BackOffice action menu.
        /// Related to #IN009039.
        /// </summary>
        /// <param name="pTitle"></param>
        /// <param name="dialogMode"></param>
        /// <returns></returns>
        public static string GetWindowTitle(String dialogWindowTitle, logicpos.Classes.Enums.Dialogs.DialogMode dialogMode)
        {
            string action = string.Empty;

            switch (dialogMode)
            {
                case logicpos.Classes.Enums.Dialogs.DialogMode.Insert:
                    action = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_generictreeviewnavigator_insert");
                    break;
                case logicpos.Classes.Enums.Dialogs.DialogMode.Update:
                    action = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_generictreeviewnavigator_update");
                    break;
                case logicpos.Classes.Enums.Dialogs.DialogMode.View:
                    action = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "widget_generictreeviewnavigator_view");
                    break;
                default:
                    break;
            }

            return string.Format("{0} :: {1} {2}", SettingsApp.AppName, action, dialogWindowTitle); // FrameworkUtils.ProductVersion
        }

        public static Size GetScreenSize()
        {
            Size result = new Size();

            try
            {
                // Moke Window only to extract its Resolution
                Window window = new Window("");
                Gdk.Screen screen = window.Screen;
                Gdk.Rectangle monitorGeometry = screen.GetMonitorGeometry(string.IsNullOrEmpty(GlobalFramework.Settings["appScreen"])
                    ? 0
                    : Convert.ToInt32(GlobalFramework.Settings["appScreen"]));
                result = new Size(monitorGeometry.Width, monitorGeometry.Height);
                // CleanUp
                window.Dispose();
                screen.Dispose();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        // Used to get the final Resolution for Render template, it uses some stuff from config and detected ScreenSize to get guest best resolution for themes

        public static Size GetThemeScreenSize()
        {
            return GetThemeScreenSize(Utils.GetScreenSize());
        }

        public static Size GetThemeScreenSize(Size screenSize)
        {
            Size result = screenSize;

            /* IN008023: apply "1024x768" settings as default. */
            ScreenSize screenSizeEnum = GetSupportedScreenResolution(screenSize);
            /* Implemented resolution settings: 800,600 | 1024,768 | 1280,768 | 1366,768 | 1280,1024 | 1680,1050 | 1920,1080 | 1920,1280*/

            switch (screenSizeEnum)
            {
                // Implemented
                case ScreenSize.res800x600:
                case ScreenSize.res1024x768:
                case ScreenSize.res1280x768:
                case ScreenSize.res1280x1024:
                case ScreenSize.res1366x768:
                case ScreenSize.res1680x1050:
                case ScreenSize.res1920x1080:
                    // Use Detected Value
                    break;
                case ScreenSize.res1024x600:
                case ScreenSize.res1280x720:
                    // Override Default
                    result = new Size(800, 600);
                    break;
                case ScreenSize.res1152x864:
                    // Override Default
                    //result = new Size(1280, 1024);
                    result = new Size(1024, 768);
                    break;
                case ScreenSize.res1280x800:
                case ScreenSize.res1360x768:
                    // Override Default
                    result = new Size(1280, 768);
                    break;
                //// Override Default
                //result = new Size(1366, 768);
                //break;
                case ScreenSize.res1440x900:
                case ScreenSize.res1536x864:
                case ScreenSize.res1600x900:
                    // Override Default
                    //result = new Size(1680, 1050);
                    result = new Size(1366, 768);
                    break;
                case ScreenSize.res1920x1200:
                case ScreenSize.res1920x1280:
                /* IN009047 */
                case ScreenSize.res2160x1440:
                case ScreenSize.res2560x1080:
                case ScreenSize.res2560x1440:
                case ScreenSize.res3440x1440:
                case ScreenSize.res3840x2160:
                    // Override Default
                    result = new Size(1920, 1080);
                    break;
                default:
                    /* IN008023: apply "1024x768" settings as default. */
                    //Default 1024*768
                    result = new Size(1024, 768);
                    break;
            }
            return result;
        }

        /// <summary>
        /// Method responsible for screen resolution validation and if necessary, sets the default resolution settings.
        /// <para>See also "IN008023: apply "800x600" settings as default"</para>
        /// </summary>
        /// <param name="screenSize"></param>
        /// <returns></returns>
        private static ScreenSize GetSupportedScreenResolution(Size screenSize)
        {
            ScreenSize supportedScreenSizeEnum;
            string screenSizeForEnum = string.Format("res{0}x{1}", screenSize.Width, screenSize.Height);

            try
            {
                supportedScreenSizeEnum = (ScreenSize)Enum.Parse(typeof(ScreenSize), screenSizeForEnum, true);
            }
            catch (Exception ex)
            {
                /* This block of code log the message informing that the resolution is not supported and asks user to define or not the default one */
                _log.Error("ScreenSize GetSupportedScreenResolution(Size screenSize) :: " + ex.Message, ex);

                /* IN009034 */
                GlobalApp.DialogThreadNotify.WakeupMain();

                string message = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "app_error_unsupported_resolution_detected"), screenSize.Width, screenSize.Height, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_treeview_true"));
                Utils.ShowMessageTouchUnsupportedResolutionDetectedDialogbox(GlobalApp.WindowStartup, screenSize.Width, screenSize.Height);

                supportedScreenSizeEnum = ScreenSize.resDefault;
            }
            return supportedScreenSizeEnum;
        }

        public static EventBox GetMinimizeEventBox()
        {

            string _fileDefaultWindowIconMinimize = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_window_minimize.png");
            EventBox result = null;

            try
            {
                Gdk.Pixbuf pixbufIconWindowMinimize = new Gdk.Pixbuf(_fileDefaultWindowIconMinimize);
                Gtk.Image gtkimageIconWindowMinimize = new Gtk.Image(pixbufIconWindowMinimize);
                EventBox eventBoxMinimize = new EventBox();
                eventBoxMinimize.WidthRequest = pixbufIconWindowMinimize.Width;
                eventBoxMinimize.HeightRequest = pixbufIconWindowMinimize.Height;
                eventBoxMinimize.Add(gtkimageIconWindowMinimize);
                //eventBoxMinimize.VisibleWindow = true;
                //eventBoxMinimize.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(Color.Red));
                result = eventBoxMinimize;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Loading Dialog / HourGlass GTK

        //Hourglass (Windows) or watch (Linux).
        public static void SetHourGlass(Window pSourceWindow, bool pShowHourGlass)
        {
            if (pShowHourGlass == true)
            {
                pSourceWindow.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Watch);
            }
            else
            {
                pSourceWindow.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.LeftPtr);
            }
        }

        //Create and return Loading Dialog
        //http://preloaders.net/en/search/circle
        //http://www.mono-project.com/docs/gui/gtksharp/responsive-applications/
        public static Dialog GetThreadDialog(Window pSourceWindow, bool dbExists)
        {
            string fileWorking = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Other\working.gif");          
  
            Dialog dialog = new Dialog("Working", pSourceWindow, DialogFlags.Modal | DialogFlags.DestroyWithParent);
            dialog.WindowPosition = WindowPosition.Center;
            //dialog.Display = 0;
            //Mensagem alternativa para primeira instalação e versao com DB criada
            Label labelBoot;            
            if (dbExists) labelBoot = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_load")); 
            else labelBoot = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_load_first_time"));
            labelBoot.ModifyFont(Pango.FontDescription.FromString("Trebuchet MS 10 Bold"));
            labelBoot.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(Color.DarkSlateGray));
            dialog.Decorated = false;
            //dialog.BorderWidth = 5;
            dialog.SetSizeRequest(194, 220);
            dialog.ActionArea.Destroy();
            Gtk.Image imageWorking = new Gtk.Image(fileWorking);
            dialog.VBox.PackStart(imageWorking);
            dialog.VBox.PackStart(labelBoot);
            dialog.ShowAll();

            return dialog;
        }

        public static void ThreadDialogReadyEvent()
        {
            /* IN008011: avoid issues when "silent mode" backup process is called during POS initial settings window is opened (POS first run) */
            if (GlobalApp.DialogThreadWork != null)
            {
                GlobalApp.DialogThreadWork.Destroy();
            }
        }

        //Get Cultures from OS
        public static bool getCultureFromOS(string culture)
        {
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {

                if (culture == System.Globalization.CultureInfo.CreateSpecificCulture(ci.Name).Name)
                {
                    return true;
                }
                
            }
            return false;
        }



        //Get Windows Version
        public static string getOSInfo()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else if (vs.Minor == 1)
                            operatingSystem = "7";
                        else if (vs.Minor == 2)
                            operatingSystem = "8";
                        else
                            operatingSystem = "8.1";
                        break;
                    case 10:
                        operatingSystem = "10";
                        break;
                    default:
                        break;
                }
            }
            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.
            if (operatingSystem != "")
            {
                //Got something.  Let's prepend "Windows" and get more info.
                operatingSystem = "Windows " + operatingSystem;
                //See if there's a service pack installed.
                if (os.ServicePack != "")
                {
                    //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                    operatingSystem += " " + os.ServicePack;
                }
                //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
            }
            //Return the information we've gathered.
            return operatingSystem;
        }

        public static Session SessionXPO()
        {
            string configDatabaseName = GlobalFramework.Settings["databaseName"];
            GlobalFramework.DatabaseName = (string.IsNullOrEmpty(configDatabaseName)) ? SettingsApp.DatabaseName : configDatabaseName;
            string xpoConnectionString = string.Format(GlobalFramework.Settings["xpoConnectionString"], GlobalFramework.DatabaseName.ToLower());
            AutoCreateOption xpoAutoCreateOption = AutoCreateOption.None;
            XpoDefault.DataLayer = XpoDefault.GetDataLayer(xpoConnectionString, xpoAutoCreateOption);
            Session LocalSessionXpo = new Session(XpoDefault.DataLayer) { LockingOption = LockingOption.None };

            return LocalSessionXpo;
        }

        public static bool checkIfDbExists()
        {
            try
            {
                string configDatabaseName = GlobalFramework.Settings["databaseName"];
                GlobalFramework.DatabaseName = (string.IsNullOrEmpty(configDatabaseName)) ? SettingsApp.DatabaseName : configDatabaseName;
                string xpoConnectionString = string.Format(GlobalFramework.Settings["xpoConnectionString"], GlobalFramework.DatabaseName.ToLower());
                AutoCreateOption xpoAutoCreateOption = AutoCreateOption.None;
                XpoDefault.DataLayer = XpoDefault.GetDataLayer(xpoConnectionString, xpoAutoCreateOption);
                Session LocalSessionXpo = new Session(XpoDefault.DataLayer) { LockingOption = LockingOption.None };


                bool databaseExists = false;
                string databaseType = System.Configuration.ConfigurationManager.AppSettings["databaseType"];

                switch (databaseType)
                {
                    case "SQLite":
                    case "MonoLite":
                        string filename = string.Format("{0}.db", GlobalFramework.DatabaseName);
                        databaseExists = (File.Exists(filename) && new FileInfo(filename).Length > 0);
                        if (databaseExists) return true;
                        else break;
                    case "MySql":
                        string sql = string.Format("SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{0}';", GlobalFramework.DatabaseName);
                        var resultCmd = LocalSessionXpo.ExecuteScalar(sql);
                        if (resultCmd != null)
                        {
                            databaseExists = (resultCmd.ToString() == GlobalFramework.DatabaseName);
                            if (databaseExists) return true;
                            else break;
                        }
                        else
                            return false;
                    case "MSSqlServer":
                    default:
                        sql = string.Format("SELECT name FROM sys.databases WHERE name = '{0}' AND name NOT IN ('master', 'tempdb', 'model', 'msdb');", GlobalFramework.DatabaseName);
                        resultCmd = LocalSessionXpo.ExecuteScalar(sql);

                        if (resultCmd != null)
                        {
                            databaseExists = (resultCmd.ToString() == GlobalFramework.DatabaseName);
                            if (databaseExists) return true;
                            else break;
                        }
                        else
                            return false;

                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        //Call With
        //Thread thread = new Thread(new ThreadStart(LARGE_COMPUTATION_METHOD));
        //GlobalApp.DialogThreadNotify = new ThreadNotify (new ReadyEvent (Utils.ThreadDialogReadyEvent));
        //thread.Start();
        //GlobalApp.DialogThreadWork = Utils.GetThreadDialog(this);
        //GlobalApp.DialogThreadWork.Run();
        //
        //With Arguments
        //Thread thread = new Thread(() => LARGE_COMPUTATION_METHOD(ARGUMENTS));
        //
        //Important Note: Dont Forget to call WakeupMain() in METHOD end of LARGE_COMPUTATION_METHOD else it never wakes
        //GlobalApp.DialogThreadNotify.WakeupMain();
        //In the end of LARGE_COMPUTATION_METHOD
        //ex
        //...
        //    return true;
        //}
        //catch (Exception ex)
        //{
        //    _log.Error(ex.Message, ex);
        //    //Notify WakeupMain and Call ReadyEvent
        //    GlobalApp.DialogThreadNotify.WakeupMain();
        //    return false;
        //}
        //
        //Quick Use
        //Thread thread = new Thread(() => result = Utils.ThreadRoutine(3000));
        //Utils.ThreadStart(pSourceWindow, thread);
        //GlobalApp.DialogThreadNotify.WakeupMain();
        //_log.Debug(String.Format("Result: [{0}]", result));
        //
        //Links
        //How to pass parameters to ThreadStart method in Thread?
        //http://stackoverflow.com/questions/3360555/how-to-pass-parameters-to-threadstart-method-in-thread

        public static void ThreadStart(Window pSourceWindow, Thread pThread)
        {
            try
            {
                /* ERR201810#15 - Database backup issues */
                GlobalApp.DialogThreadNotify = new ThreadNotify(new ReadyEvent(Utils.ThreadDialogReadyEvent));
                pThread.Start();

                // Proptection for Startup Windows and Backup, If dont have a valid window, dont show loading (BackGround Thread)
                if (pSourceWindow != null)
                {
                    GlobalApp.DialogThreadWork = GetThreadDialog(pSourceWindow, Utils.checkIfDbExists());
                    GlobalApp.DialogThreadWork.Run();
                }
                /* END: ERR201810#15 */
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public static void ThreadLoading (Window pSourceWindow)
        {
            try
            {
                // Proptection for Startup Windows and Backup, If dont have a valid window, dont show loading (BackGround Thread)
                if (pSourceWindow != null)
                {
                    GlobalApp.DialogThreadWork = GetThreadDialog(pSourceWindow, false);
                    GlobalApp.DialogThreadWork.Run();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //Sample Test Routines : Used in Startup Window
        public static bool ThreadRoutine()
        {
            return ThreadRoutine(2500);
        }

        public static bool ThreadRoutine(int pMilliseconds)
        {
            LargeComputation(pMilliseconds);
            //Notify WakeupMain and Call ReadyEvent
            GlobalApp.DialogThreadNotify.WakeupMain();

            return true;
        }

        //Sample Test Routines : Used in Startup Window
        public static void LargeComputation(int pMilliseconds)
        {
            System.Threading.Thread.Sleep(pMilliseconds);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Settings

        //Require to use Dictionary to Save Batch of Value/Keys
        public static void AddUpdateSettings(Dictionary<string, string> pValues)
        {
            bool debug = false;
            string configFileName = GetApplicationConfigFileName();

            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                foreach (var item in pValues)
                {
                    if (settings[item.Key] == null)
                    {
                        settings.Add(item.Key, item.Value);
                        //Debug
                        if (debug) _log.Debug(string.Format("AddOrUpdateAppSettings: Add Key: [{0}] = [{1}]", item.Key, settings[item.Key].Value));
                    }
                    else
                    {
                        settings[item.Key].Value = item.Value;
                        //Debug
                        if (debug) _log.Debug(string.Format("AddOrUpdateAppSettings: Modified Key: [{0}] = [{1}]", item.Key, settings[item.Key].Value));
                    }

                    //Assign to Memory
                    ConfigurationManager.AppSettings.Set(item.Key, item.Value);
                }

                //Save
                if (Debugger.IsAttached == true)
                {
                    if (debug) _log.Debug("Save AppSettings with debugger");
                    configFile.SaveAs(configFileName, ConfigurationSaveMode.Modified);
                }
                else
                {
                    if (debug) _log.Debug("Save AppSettings without debugger");
                    configFile.Save(ConfigurationSaveMode.Modified);
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //Used when working with Debugger
        public static string GetApplicationConfigFileName()
        {
            string result = string.Empty;

            try
            {
                Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                result = cfg.FilePath.ToLower().Replace("vshost.", string.Empty);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //<add key="errorMailFirst" value="test@test.no"/>
        //<add key="errorMailSeond" value="krister@tets.no"/>
        //Then in my configuration wrapper class, I add a method to search keys.

        //Get Settings Keys List
        //public static List<string> AppSearchKeys(string searchTerm)
        //{
        //    var keys = ConfigurationManager.AppSettings.Keys;

        //    return keys.Cast<object>()
        //               .Where(key => key.ToString().ToLower()
        //               .Contains(searchTerm.ToLower()))
        //               .Select(key => ConfigurationManager.AppSettings.Get(key.ToString())).ToList();
        //}

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Terminals

        //public static ConfigurationPlaceTerminal GetTerminal_OLD_WORKING_REPLACED_BY_HARDWAREID()
        //{
        //  string terminalIdFile = FrameworkUtils.OSSlash(GlobalFramework.Settings["appTerminalIdConfigFile"]);
        //  string terminalIdString = string.Empty;
        //  Guid terminalIdGuid = new Guid();
        //  ConfigurationPlaceTerminal terminalXpo = null;

        //  if (File.Exists(terminalIdFile))
        //  {
        //    terminalIdString = RemoveCarriageReturnAndExtraWhiteSpaces(File.ReadAllText(terminalIdFile));
        //    try
        //    {
        //      terminalIdGuid = new Guid(terminalIdString);
        //    }
        //    catch (Exception ex)
        //    {
        //      _log.Error(ex.Message, ex);
        //    }
        //  }

        //  if (terminalIdGuid != new Guid())
        //  {
        //    try
        //    {
        //      //Get TerminalID from Database
        //      terminalXpo = (ConfigurationPlaceTerminal)FrameworkUtils.GetXPGuidObjectFromSession(typeof(ConfigurationPlaceTerminal), terminalIdGuid);
        //    }
        //    catch (Exception ex)
        //    {
        //      _log.Error(ex.Message, ex);
        //    }
        //  }

        //  //Create a new db terminal
        //  if (terminalXpo == null)
        //  {
        //    //Persist Terminal in DB
        //    terminalXpo = new ConfigurationPlaceTerminal(GlobalFramework.SessionXpo)
        //    {
        //      Ord = FrameworkUtils.GetNextTableFieldID("pos_configurationplaceterminal", "Ord"),
        //      Code = FrameworkUtils.GetNextTableFieldID("pos_configurationplaceterminal", "Code"),
        //      Designation = "Terminal #" + FrameworkUtils.GetNextTableFieldID("pos_configurationplaceterminal", "Code")
        //      //Fqdn = GetFQDN()
        //    };
        //    terminalXpo.Save();

        //    _log.Debug(string.Format("Registered a new Terminal in Database and Config Settings. TerminalId: [{0}] ]", terminalXpo.Oid));
        //    //Save to Text
        //    File.WriteAllText(terminalIdFile, terminalXpo.Oid.ToString());
        //  }

        //  return terminalXpo;
        //}

        public static pos_configurationplaceterminal GetTerminal()
        {
            pos_configurationplaceterminal configurationPlaceTerminal = null;

            try
            {
                // Use HardwareId from Settings, must be added manually, its a hack and its not there, in setuo, only in debug config
                if (!string.IsNullOrEmpty(GlobalFramework.Settings["appHardwareId"]))
                {
                    GlobalFramework.LicenceHardwareId = GlobalFramework.Settings["appHardwareId"];
                }
                //Debug Directive disabled by Mario, if enabled we cant force HardwareId in Release, 
                //if we want to ignore appHardwareId from config we just delete it
                //If assigned in Config use it, else does nothing and use default ####-####-####-####-####-####
                else if (SettingsApp.AppHardwareId != null && SettingsApp.AppHardwareId != string.Empty)
                {
                    GlobalFramework.LicenceHardwareId = SettingsApp.AppHardwareId;
                }

                try
                {
                    //Try TerminalID from Database
                    _log.Debug("pos_configurationplaceterminal GetTerminal() :: Try TerminalID from Database");
                    configurationPlaceTerminal = (pos_configurationplaceterminal)FrameworkUtils.GetXPGuidObjectFromField(typeof(pos_configurationplaceterminal), "HardwareId", GlobalFramework.LicenceHardwareId);
                }
                catch (Exception ex)
                {
                    _log.Error("pos_configurationplaceterminal GetTerminal() :: Try TerminalID from Database: " + ex.Message, ex);
                }

                //Create a new db terminal
                if (configurationPlaceTerminal == null)
                {
                    try
                    {
                        //Persist Terminal in DB
                        configurationPlaceTerminal = new pos_configurationplaceterminal(GlobalFramework.SessionXpo)
                        {
                            Ord = FrameworkUtils.GetNextTableFieldID("pos_configurationplaceterminal", "Ord"),
                            Code = FrameworkUtils.GetNextTableFieldID("pos_configurationplaceterminal", "Code"),
                            Designation = "Terminal #" + FrameworkUtils.GetNextTableFieldID("pos_configurationplaceterminal", "Code"),
                            HardwareId = GlobalFramework.LicenceHardwareId
                            //Fqdn = GetFQDN()
                        };
                        _log.Debug("pos_configurationplaceterminal GetTerminal() :: configurationPlaceTerminal.Save()");
                        configurationPlaceTerminal.Save();
                    }
                    catch (Exception ex)
                    {
                        /* IN009034 */
                        GlobalApp.DialogThreadNotify.WakeupMain();

                        _log.Error(string.Format("pos_configurationplaceterminal GetTerminal() :: Error! Can't Register a new TerminalId [{0}] with HardwareId: [{1}], Error: [2]", configurationPlaceTerminal.Oid, configurationPlaceTerminal.HardwareId, ex.Message), ex);
                        ShowMessageTouch(null, DialogFlags.Modal, new Size(600, 300), MessageType.Error, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_error_register_new_terminal"), configurationPlaceTerminal.HardwareId));
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("pos_configurationplaceterminal GetTerminal() :: " + ex.Message, ex);
            }

            return configurationPlaceTerminal;
        }

        public static bool CloseAllOpenTerminals(Window pSourceWindow, Session pSession)
        {
            bool result = false;

            try
            {
                // SELECT Oid, PeriodType, SessionStatus, Designation FROM pos_worksessionperiod WHERE PeriodType = 1 AND SessionStatus = 0;
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse("PeriodType = 1 AND SessionStatus = 0");
                SortProperty[] sortProperty = new SortProperty[2];
                sortProperty[0] = new SortProperty("CreatedAt", SortingDirection.Ascending);
                XPCollection xpcWorkingSessionPeriod = new XPCollection(pSession, typeof(pos_worksessionperiod), criteriaOperator, sortProperty);
                DateTime dateTime = FrameworkUtils.CurrentDateTimeAtomic();
                if (xpcWorkingSessionPeriod.Count > 0)
                {
                    foreach (pos_worksessionperiod item in xpcWorkingSessionPeriod)
                    {
                        item.SessionStatus = WorkSessionPeriodStatus.Close;
                        item.DateEnd = dateTime;
                        item.Save();
                    }                    
                }

                result = true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Windows

        //Helpers
        public static void ShowFrontOffice(Window pHideWindow)
        {
            _log.Debug("void ShowFrontOffice(Window pHideWindow) :: Starting..."); /* IN009008 */
            try
            {
                if (GlobalApp.WindowPos == null)
                {
                    //Init Theme Object
                    Predicate<dynamic> predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "PosMainWindow");
                    dynamic themeWindow = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);
                    try
                    {
                        /* IN008024 */
                        CustomAppOperationMode customAppOperationMode = SettingsApp.CustomAppOperationMode;
                        //_log.Debug(string.Format("fileImageBackgroundWindowPos: [{0}]", GlobalFramework.Settings["fileImageBackgroundWindowPos"]));
                        string windowImageFileName = string.Format(themeWindow.Globals.ImageFileName, customAppOperationMode.AppOperationTheme, GlobalApp.ScreenSize.Width, GlobalApp.ScreenSize.Height);
                        GlobalApp.WindowPos = new PosMainWindow(windowImageFileName);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message, ex);
                    }
                }
                else
                {
                    //Update POS Components if Window was previously Created, Required to Reflect Outside Changes Like BackOffice
                    GlobalApp.WindowPos.UpdateUI();
                    //Now Show Updated Window
                    GlobalApp.WindowPos.Show();
                };
                pHideWindow.Hide();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public static void ShowBackOffice(Window pHideWindow)
        {
            try
            {
                //TODO: PRIVILEGIOS Exemplo de bloquear funcionalidade
                /* 
                if (!GlobalFramework.CheckCurrentUserPermission("BACKOFFICE_ACCESS"))
                {
                  //TODO: adicionar msg para indicar que nao tem acesso 
                  Utils.ShowMessageUnderConstruction();
                }
                else
                */
                {
                    //FrameworkUtils.ClearCache();

                    //FrameworkUtils.ShowWaitingCursor();

                    if (GlobalApp.WindowBackOffice == null)
                    {
                        GlobalApp.WindowBackOffice = new BackOfficeMainWindow();
                    }
                    else
                    {                        
                        GlobalApp.WindowBackOffice.Show();
                    }
                   
                   pHideWindow.Hide();
                    
                    
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public static void ShowReports(Window pHideWindow)
        {
            if (GlobalApp.WindowReports == null)
            {
                GlobalApp.WindowReports = new BackOfficeReportWindow();
            }
            else
            {
                GlobalApp.WindowReports.Show();
            };
            pHideWindow.Hide();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Themes and Backgrounds

        public static string GetThemeFileLocation(string pFile)
        {
            string pathThemes = GlobalFramework.Path["themes"].ToString();
            /* IN008024 */
            return FrameworkUtils.OSSlash(string.Format(@"{0}{1}\{2}", pathThemes, SettingsApp.AppTheme, pFile));
        }

        public static Gtk.Style GetThemeStyleBackground(string pFile)
        {
            /* IN008024 */
            string fileImageBackground = GetThemeFileLocation(pFile);

            if (fileImageBackground != null && File.Exists(fileImageBackground))
            {
                Gdk.Pixmap pixmap = Utils.FileToPixmap(fileImageBackground);

                if (pixmap != null)
                {
                    Gtk.Style style = new Style();
                    style.SetBgPixmap(StateType.Normal, pixmap);
                    return style;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                _log.Error(string.Format("Missing Theme[{0}] Image: [{1}]", SettingsApp.AppTheme, fileImageBackground));
                return null;
            }
        }

        public static Gtk.Style GetImageBackground(string pFile)
        {
            /* IN008024 */
            string fileImageBackground = GetThemeFileLocation(pFile);

            if (fileImageBackground != null && File.Exists(fileImageBackground))
            {
                Gdk.Pixmap pixmap = Utils.FileToPixmap(fileImageBackground);

                if (pixmap != null)
                {
                    Gtk.Style style = new Style();
                    style.SetBgPixmap(StateType.Normal, pixmap);
                    return style;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                _log.Error(string.Format("Missing Theme[{0}] Image: [{1}]", SettingsApp.AppTheme, fileImageBackground));
                return null;
            }
        }

        public static Gtk.Style GetImageBackgroundDash(string pFile)
        {
            
            if (pFile != null && File.Exists(pFile))
            {
                Gdk.Pixmap pixmap = Utils.FileToPixmap(pFile);

                if (pixmap != null)
                {
                    Gtk.Style style = new Style();
                    style.SetBgPixmap(StateType.Normal, pixmap);
                    return style;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                _log.Error(string.Format("Missing Theme[{0}] Image: [{1}]", SettingsApp.AppTheme, pFile));
                return null;
            }
        }



        public static Gtk.Style GetImageBackgroundDashboard(Gdk.Pixbuf pFilename)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (pFilename != null)
                {
                    Gdk.Pixmap pixmap = Utils.PixbufToPixmap(pFilename);

                    if (pixmap != null)
                    {
                        Gtk.Style style = new Style();
                        style.SetBgPixmap(StateType.Normal, pixmap);
                        return style;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    _log.Error(string.Format("Missing Theme[{0}] Image: [{1}]", SettingsApp.AppTheme, pFilename.ToString()));
                    return null;
                }                
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Notifications UI

        public static void ShowNotifications(Window pSourceWindow)
        {
            ShowNotifications(pSourceWindow, GlobalFramework.SessionXpo);
        }

        public static void ShowNotifications(Window pSourceWindow, Session pSession)
        {
            ShowNotifications(pSourceWindow, pSession, Guid.Empty);
        }

        public static void ShowNotifications(Window pSourceWindow, Guid pLoggedUser)
        {
            ShowNotifications(pSourceWindow, GlobalFramework.SessionXpo, pLoggedUser);
        }

        /// <summary>
        /// When user wish to show notifications again.
        /// Please see #IN006001 for further details.
        /// </summary>
        /// <param name="pSourceWindow"></param>
        /// <param name="showNotificationOnDemand"></param>
        public static void ShowNotifications(Window pSourceWindow, bool showNotificationOnDemand)
        {
            ShowNotifications(pSourceWindow, GlobalFramework.SessionXpo, Guid.Empty, showNotificationOnDemand);
        }

        /// <summary>
        /// Shows notifications created by "void FrameworkUtils.SystemNotification(Session pSession)" method.
        /// More information about changes on this, please see #IN006001.
        /// </summary>
        /// <param name="pSourceWindow"></param>
        /// <param name="pSession"></param>
        /// <param name="pLoggedUser"></param>
        /// <param name="showNotificationOnDemand"></param>
        public static void ShowNotifications(Window pSourceWindow, Session pSession, Guid pLoggedUser, bool showNotificationOnDemand = false)
        {
            _log.Debug("void Utils.ShowNotifications(Window pSourceWindow, Session pSession, Guid pLoggedUser) :: Source Window: " + pSourceWindow.Name);

            string message = string.Empty;
            string extraFilter = (pLoggedUser != Guid.Empty) ? string.Format("AND (UserTarget = '{0}' OR UserTarget IS NULL) ", pLoggedUser) : string.Empty;

            /* IN006001 */
            try
            {
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("(TerminalTarget = '{0}' OR TerminalTarget IS NULL){1}", GlobalFramework.LoggedTerminal.Oid, extraFilter));

                /* IN006001 - for "on demand" notification flow */
                if (showNotificationOnDemand)
                {
                    /* IN006001 - get date for filtering notifications that were created 'n' days before Today */
                    //Get Date Back DaysBackToFilter (Without WeekEnds and Holidays)
                    DateTime dateFilter = FrameworkUtils.GetDateTimeBackUtilDays(
                        FrameworkUtils.CurrentDateTimeAtomicMidnight(),
                        SettingsApp.XpoOidSystemNotificationDaysBackWhenFiltering,
                        true);
                    criteriaOperator = GroupOperator.And(criteriaOperator, CriteriaOperator.Parse(string.Format("[CreatedAt] > '{0} 23:59:59'", dateFilter.ToString(SettingsApp.DateFormat))));

                    JoinOperand joinOperand = new JoinOperand();
                    joinOperand.JoinTypeName = "sys_systemnotificationtype";
                    CriteriaOperator criteriaJoin = CriteriaOperator.Parse("^.NotificationType") == new OperandProperty("Oid");
                    /*
                     * Notification Type for "on demand" notifications flow:
                     * '06319D46-E7B5-4CCA-8257-55EFF4CFE0FA': Documentos de conta-corrente por faturar
                     * 'A567578B-53E9-4B5C-848F-183C65194971': Documentos de faturas de consignação por faturar
                     * '80A03838-0937-4AE3-921F-75A1E358F7BF': Documentos de transporte por faturar
                     * 
                     */
                    criteriaJoin = GroupOperator.And(criteriaJoin, new InOperator("Oid", new Guid[]{
                            SettingsApp.XpoOidSystemNotificationTypeCurrentAccountDocumentsToInvoice,
                            SettingsApp.XpoOidSystemNotificationTypeConsignationInvoiceDocumentsToInvoice,
                            SettingsApp.XpoOidSystemNotificationTypeSaftDocumentTypeMovementOfGoods
                        }));
                    joinOperand.Condition = criteriaJoin;

                    criteriaOperator = GroupOperator.And(criteriaOperator, joinOperand);
                }
                else
                {/* keep the criteria for original flow */
                    criteriaOperator = GroupOperator.And(criteriaOperator, CriteriaOperator.Parse("[Readed] = 0"));
                }

                SortProperty[] sortProperty = new SortProperty[2];
                sortProperty[0] = new SortProperty("CreatedAt", SortingDirection.Ascending);
                sortProperty[1] = new SortProperty("Ord", SortingDirection.Ascending);
                XPCollection xpcSystemNotification = new XPCollection(pSession, typeof(sys_systemnotification), criteriaOperator, sortProperty);

                if (xpcSystemNotification.Count > 0)
                {
                    foreach (sys_systemnotification item in xpcSystemNotification)
                    {
                        message = string.Format("{1}{0}{0}{2}", Environment.NewLine, item.CreatedAt, item.Message);
                        ResponseType response = Utils.ShowMessageTouch(
                          pSourceWindow,
                          DialogFlags.DestroyWithParent | DialogFlags.Modal,
                          new Size(700, 480),
                          MessageType.Info,
                          ButtonsType.Ok,
                          resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_notification"),
                          message
                        );

                        //Always OK
                        if (response == ResponseType.Ok)
                        {
                            item.DateRead = FrameworkUtils.CurrentDateTimeAtomic();
                            item.Readed = true;
                            item.UserLastRead = GlobalFramework.LoggedUser;
                            item.TerminalLastRead = GlobalFramework.LoggedTerminal;
                            item.Save();

                            //Call ProcessNotificationsActions
                            ProcessNotificationsActions(pSourceWindow, item);
                        }
                    }
                }
                else if (showNotificationOnDemand)
                {/* IN006001 - when "on demand" request returns no results */
                    message = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_no_notification"), SettingsApp.XpoOidSystemNotificationDaysBackWhenFiltering);
                    ResponseType response = Utils.ShowMessageTouch(
                      pSourceWindow,
                      DialogFlags.DestroyWithParent | DialogFlags.Modal,
                      new Size(700, 480),
                      MessageType.Info,
                      ButtonsType.Ok,
                      resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_notification"),
                      message
                    );
                }
            }
            catch (Exception ex)
            {
                _log.Error("void Utils.ShowNotifications(Window pSourceWindow, Session pSession, Guid pLoggedUser) :: " + ex.Message, ex);
                ShowMessageTouch(null, DialogFlags.Modal, new Size(600, 300), MessageType.Error, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), "There is an error when checking for notifications. Please contact the helpdesk");
            }
        }

        private static void ProcessNotificationsActions(Window pSourceWindow, sys_systemnotification pSystemNotification)
        {
            //NotificationTypeEditConfigurationParameters
            //if (pSystemNotification.NotificationType.Oid == SettingsApp.XpoOidSystemNotificationTypeEditConfigurationParameters)
            //{
            //    PosEditCompanyDetails dialog = new PosEditCompanyDetails(pSourceWindow, DialogFlags.DestroyWithParent | DialogFlags.Modal);
            //    ResponseType response = (ResponseType)dialog.Run();
            //    //if (response == ResponseType.Ok)
            //    //{
            //    dialog.Destroy();
            //    //}
            //    //else
            //    //{
            //    //    dialog.Run();
            //    //}
            //}
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Virtual KeyBoard

        public static string GetVirtualKeyBoardInput(Window pSourceWindow, KeyboardMode pMode, string pInitialValue, string pRegExRule)
        {
            Boolean useBaseDialogWindowMask = Convert.ToBoolean(GlobalFramework.Settings["useBaseDialogWindowMask"]);
            string result = string.Empty;

            //if (GlobalApp.DialogPosKeyboard == null)
            //{
            //Chama teclado certo na janela de artigos
            switch (pMode)
            {
                case KeyboardMode.Alfa:
                case KeyboardMode.AlfaNumeric:
                    //On Create SourceWindow is always GlobalApp.WindowPos else if its a Dialog, when it is destroyed, in Memory Keyboard is Destroyed too, this way we keep it in Memory
                    GlobalApp.DialogPosKeyboard = new PosKeyboardDialog(GlobalApp.WindowPos, Gtk.DialogFlags.DestroyWithParent, KeyboardMode.AlfaNumeric, pInitialValue, pRegExRule);
                    break;

                case KeyboardMode.Numeric:
                    GlobalApp.DialogPosKeyboard = new PosKeyboardDialog(GlobalApp.WindowPos, Gtk.DialogFlags.DestroyWithParent, KeyboardMode.Numeric, pInitialValue, pRegExRule);
                    break;
                default: break;
            }
            //}
            //else
            //{
            //pInitialValue, pRegExRule
            GlobalApp.DialogPosKeyboard.Text = pInitialValue;
            GlobalApp.DialogPosKeyboard.Rule = pRegExRule;

            //Fix TransientFor, ALT+TABS
            if (useBaseDialogWindowMask)
            {
                GlobalApp.DialogPosKeyboard.TransientFor = GlobalApp.DialogPosKeyboard.WindowMaskBackground;
                GlobalApp.DialogPosKeyboard.WindowMaskBackground.TransientFor = pSourceWindow;
                GlobalApp.DialogPosKeyboard.WindowMaskBackground.Show();
            }
            else
            {
                //Now we can change its TransientFor
                GlobalApp.DialogPosKeyboard.TransientFor = pSourceWindow;
            }
            //}

            //Always Start Validated, else Only Construct start Validated
            GlobalApp.DialogPosKeyboard.TextEntry.Validate();
            //Put Cursor in End
            GlobalApp.DialogPosKeyboard.TextEntry.Position = GlobalApp.DialogPosKeyboard.TextEntry.Text.Length;
            GlobalApp.DialogPosKeyboard.TextEntry.GrabFocus();
            int response = GlobalApp.DialogPosKeyboard.Run();
            if (response == (int)ResponseType.Ok)
            {
                result = GlobalApp.DialogPosKeyboard.Text;
            }
            else
            {
                result = null;
            }

            //Fix Keyboard White Bug when useBaseDialogWindowMask = false
            //Required to assign TransientFor to a non Destroyed Window/Dialog like GlobalApp.WindowPos, 
            //else Keyboard is destroyed when TransientFor Windows/Dialog is Destroyed ex when pSourceWindow = PosInputTextDialog
            GlobalApp.DialogPosKeyboard.TransientFor = GlobalApp.WindowPos;

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Cache

        internal static bool UseCache()
        {
            bool result = false;

            try
            {
                result = Convert.ToBoolean(GlobalFramework.PreferenceParameters["USE_CACHED_IMAGES"]);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return (result);
        }

        internal static bool UseVatAutocomplete()
        {
            bool result = false;

            try
            {
                result = Convert.ToBoolean(GlobalFramework.PreferenceParameters["USE_EUROPEAN_VAT_AUTOCOMPLETE"]); 
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return (result);
        }

        internal static bool UseCCDailyTicket()
        {
            bool result = false;

            try
            {
                result = Convert.ToBoolean(GlobalFramework.PreferenceParameters["USE_CC_DAILY_TICKET"]);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return (result);
        }


        internal static bool UsePosPDFViewer()
        {
            bool result = false;

            try
            {
                result = Convert.ToBoolean(GlobalFramework.PreferenceParameters["USE_POS_PDF_VIEWER"]);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return (result);
        }

        //Commented by Mario because its not used in Project
        //internal static void DeleteWidgetChilds(Fixed pxedWindow)
        //{
        //  try
        //  {
        //    if (pxedWindow.Children.Length > 0)
        //    {
        //      for (int i = 0; i < pxedWindow.Children.Length; i++)
        //      {
        //        pxedWindow.Remove(pxedWindow.Children[i]);
        //      }
        //    }
        //  }
        //  catch (Exception ex)
        //  {
        //    _log.Error(ex.Message, ex);
        //  }
        //}

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //GenericTreeView

        //Helper Static Method to Return a GenericTreeView from T
        public static T GetGenericTreeViewXPO<T>(Window pSourceWindow)
          where T : IGenericTreeView, new()
        {
            return GetGenericTreeViewXPO<T>(pSourceWindow, null);
        }

        public static T GetGenericTreeViewXPO<T>(Window pSourceWindow, CriteriaOperator pCriteria)
          where T : IGenericTreeView, new()
        {
            T genericTreeView = default(T);

            try
            {
                // Add default Criteria to Hide Undefined Records
                string undefinedFilter = string.Format("Oid <> '{0}'", SettingsApp.XpoOidUndefinedRecord);

#pragma warning disable CS0618 // Type or member is obsolete
                if (pCriteria == null)
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    pCriteria = CriteriaOperator.Parse(undefinedFilter);
                }
                else
                {
                    pCriteria = CriteriaOperator.Parse(string.Format("{0} AND ({1})", pCriteria, undefinedFilter));
                }

                object[] constructor = new object[]
                {
                      pSourceWindow,
                      null,         //Default Value
                      pCriteria,    //Criteria
                      null,         //DialogType
                      GenericTreeViewMode.Default,
                      GenericTreeViewNavigatorMode.Default
                };
                genericTreeView = (T)Activator.CreateInstance(typeof(T), constructor);
                //Cast to Box to use ShowAll for all T(ypes) of GenericTreeView
                (genericTreeView as Box).ShowAll();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return genericTreeView;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Network

        public static bool DownloadFileFromUrl(string pUrl, string pTargetFile)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadFile(pUrl, pTargetFile);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    return false;
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //License

        public static bool AssignLicence(string pFileName, bool pDebug = false)
        {
            bool result = false;

            try
            {
                StreamReader sr = null;
                try
                {
                    INIFile iNIFile = new INIFile(pFileName);

                    //Load
                    GlobalFramework.LicenceHardwareId = CryptorEngine.Decrypt(iNIFile.GetValue("Licence", "HardwareId", "Empresa Demonstração"), true);
                    GlobalFramework.LicenceCompany = CryptorEngine.Decrypt(iNIFile.GetValue("Licence", "Company", "NIF Demonstração"), true);
                    GlobalFramework.LicenceNif = CryptorEngine.Decrypt(iNIFile.GetValue("Licence", "Nif", "Morada Demonstração"), true);
                    GlobalFramework.LicenceAddress = CryptorEngine.Decrypt(iNIFile.GetValue("Licence", "Address", "mail@demonstracao.tld"), true);
                    GlobalFramework.LicenceEmail = CryptorEngine.Decrypt(iNIFile.GetValue("Licence", "Email", string.Empty), true);
                    GlobalFramework.LicenceTelephone = CryptorEngine.Decrypt(iNIFile.GetValue("Licence", "Telephone", "Telefone Demonstração"), true);
                    GlobalFramework.LicenceReseller = CryptorEngine.Decrypt(iNIFile.GetValue("Licence", "Reseller", "LogicPulse"), true);
                    //Test
                    if (pDebug)
                    {
                        _log.Debug(string.Format("{0}:{1}", "HardwareId", GlobalFramework.LicenceHardwareId));
                        _log.Debug(string.Format("{0}:{1}", "Company", GlobalFramework.LicenceCompany));
                        _log.Debug(string.Format("{0}:{1}", "Nif", GlobalFramework.LicenceNif));
                        _log.Debug(string.Format("{0}:{1}", "Address", GlobalFramework.LicenceAddress));
                        _log.Debug(string.Format("{0}:{1}", "Email", GlobalFramework.LicenceEmail));
                        _log.Debug(string.Format("{0}:{1}", "Telephone", GlobalFramework.LicenceTelephone));
                        _log.Debug(string.Format("{0}:{1}", "Reseller", GlobalFramework.LicenceReseller));
                    }
                    iNIFile.Flush();

                    result = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (sr != null) sr.Close();
                    sr = null;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Customers

        //Used to Save or Update Customers when Process Finance Documents
        public static object SaveOrUpdateCustomer(Window pSourceWindow, string pName, string pAddress, string pLocality, string pZipCode, string pCity, string pPhone, string pEmail, cfg_configurationcountry pCountry, string pFiscalNumber, string pCardNumber, decimal pDiscount, string pNotes)
        {
            return SaveOrUpdateCustomer(pSourceWindow, null, pName, pAddress, pLocality, pZipCode, pCity, pPhone, pEmail, pCountry, pFiscalNumber, pCardNumber, pDiscount, pNotes);
        }

        public static object SaveOrUpdateCustomer(Window pSourceWindow, erp_customer pCustomer, string pName, string pAddress, string pLocality, string pZipCode, string pCity, string pPhone, string pEmail, cfg_configurationcountry pCountry, string pFiscalNumber, string pCardNumber, decimal pDiscount, string pNotes)
        {
            bool changed = false;
            bool customerExists = false;
            erp_customer result;
            erp_customer finalConsumerEntity = (erp_customer)FrameworkUtils.GetXPGuidObject(typeof(erp_customer), SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity);
            fin_configurationpricetype configurationPriceType = (fin_configurationpricetype)FrameworkUtils.GetXPGuidObject(typeof(fin_configurationpricetype), SettingsApp.XpoOidConfigurationPriceTypeDefault);
                        
            SortingCollection sortCollection = new SortingCollection();
            sortCollection.Add(new SortProperty("Oid", DevExpress.Xpo.DB.SortingDirection.Ascending));
            CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
            ICollection collectionCustomers = GlobalFramework.SessionXpo.GetObjects(GlobalFramework.SessionXpo.GetClassInfo(typeof(erp_customer)), criteria, sortCollection, int.MaxValue, false, true);

            foreach (erp_customer item in collectionCustomers)
            {

                if (item.FiscalNumber == pFiscalNumber || item.Name == pName) customerExists = true;               
            }

            //insert new Customer before Process Finance Document
            //se for consumidor final, nao altera, cria novo registo
            if (!customerExists)
            {
                changed = true;
                result = new erp_customer(GlobalFramework.SessionXpo)
                {
                    Ord = (pFiscalNumber != string.Empty) ? FrameworkUtils.GetNextTableFieldID("erp_customer", "Ord") : 0,
                    Code = (pFiscalNumber != string.Empty) ? FrameworkUtils.GetNextTableFieldID("erp_customer", "Code") : 0,
                    Name = pName,
                    Address = pAddress,
                    Locality = pLocality,
                    ZipCode = pZipCode,
                    City = pCity,
                    Country = pCountry,
                    FiscalNumber = (pFiscalNumber != string.Empty) ? pFiscalNumber : finalConsumerEntity.FiscalNumber,
                    CardNumber = (pCardNumber != string.Empty) ? pCardNumber : null,
                    Discount = pDiscount,
                    PriceType = configurationPriceType,
                    Notes = pNotes,
                    Hidden = (pFiscalNumber != string.Empty) ? false : true
                };
                if (pPhone != null)
                {
                    result.Phone = pPhone;
                };
                if (pEmail != null)
                {
                    result.Email = pEmail;
                };
            }
            //If customer Exists, check for modifications and UPDATE Details
            else
            {
                changed = false;
                //Require to get a Fresh Object
                result = (erp_customer)FrameworkUtils.GetXPGuidObject(typeof(erp_customer), pCustomer.Oid);

                if (CheckIfFieldChanged(result.Name, pName)) { result.Name = pName; changed = true; };
                if (CheckIfFieldChanged(result.Address, pAddress)) { result.Address = pAddress; changed = true; };
                if (CheckIfFieldChanged(result.Locality, pLocality)) { result.Locality = pLocality; changed = true; };
                if (CheckIfFieldChanged(result.ZipCode, pZipCode)) { result.ZipCode = pZipCode; changed = true; };
                if (CheckIfFieldChanged(result.City, pCity)) { result.City = pCity; changed = true; };
                if (result.Country != pCountry) { result.Country = pCountry; changed = true; };
                if (CheckIfFieldChanged(result.FiscalNumber, pFiscalNumber)) { result.FiscalNumber = pFiscalNumber; changed = true; };
                if (CheckIfFieldChanged(result.CardNumber, pCardNumber)) { result.CardNumber = pCardNumber; changed = true; };
                if (CheckIfFieldChanged(result.Discount, pDiscount)) { result.Discount = pDiscount; changed = true; };
                if (CheckIfFieldChanged(result.Notes, pNotes)) { result.Notes = pNotes; changed = true; };
                // Only used in DocumentFinanceDialogPage2
                if (pPhone != null && CheckIfFieldChanged(result.Phone, pPhone)) { result.Phone = pPhone; changed = true; };
                if (pEmail != null && CheckIfFieldChanged(result.Email, pEmail)) { result.Email = pEmail; changed = true; };

                if (changed)
                {
                    ResponseType responseType = Utils.ShowMessageTouch(pSourceWindow, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_record_modified"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_customer_updated_save_changes"));
                    if (responseType == ResponseType.No)
                    {
                        changed = false;
                        //Require to Revert Changes from XPO Session Memory
                        result.Reload();
                    }
                }
            }

            //Shared Save for Insert and Update
            try
            {
                if (changed) result.Save();
                return result;
            }
            catch (Exception ex)
            {
                //Dont Show in Log, we process error showing alert to user
                //_log.Error(ex.Message, ex);
                result.Reload();
                return ex;
            }
        }

        //Method to Check Equallity from Db Fields to Input Fields, Required to compare null with "" etc
        public static bool CheckIfFieldChanged(object pFieldDb, object pFieldInput)
        {
            bool result = false;
            object fieldDb = (pFieldDb == null || pFieldDb.ToString() == string.Empty) ? string.Empty : pFieldDb;

            //Decimal
            if (fieldDb.GetType() == typeof(decimal))
            {
                result = (!fieldDb.Equals(pFieldInput));
            }
            //all Other
            else
            {
                result = (fieldDb.ToString() != pFieldInput.ToString());
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Collections

        public static bool ContainsKey(NameValueCollection pCollection, string pKey)
        {
            return pCollection.AllKeys.Contains(pKey);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //SessionApp

        public static String GetSessionFileName()
        {
            string result = Path.Combine(GlobalFramework.Path["temp"].ToString(), string.Format(SettingsApp.AppSessionFile, GlobalFramework.LicenceHardwareId));
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Change Widgets Validation Colors

        public static void ValidateUpdateColors(Widget pWidget, Label pLabel, bool pValidated)
        {
            //Use source widget or child Widget based on Type 
            Widget target = (pWidget.GetType() != typeof(EntryBoxValidationMultiLine))
                ? pWidget
                : (pWidget as EntryBoxValidationMultiLine).EntryMultiline.TextView;

            try
            {
                Color colorEntryValidationValidFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationValidFont"]);
                Color colorEntryValidationInvalidFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationInvalidFont"]);
                Color colorEntryValidationValidBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationValidBackground"]);
                Color colorEntryValidationInvalidBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationInvalidBackground"]);

                if (pValidated)
                {
                    target.ModifyText(StateType.Normal, Utils.ColorToGdkColor(colorEntryValidationValidFont));
                    target.ModifyText(StateType.Active, Utils.ColorToGdkColor(colorEntryValidationValidFont));
                    target.ModifyBase(StateType.Normal, Utils.ColorToGdkColor(colorEntryValidationValidBackground));
                    if (pLabel != null) pLabel.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(colorEntryValidationValidFont));
                }
                else
                {
                    target.ModifyText(StateType.Normal, Utils.ColorToGdkColor(colorEntryValidationInvalidFont));
                    target.ModifyText(StateType.Active, Utils.ColorToGdkColor(colorEntryValidationInvalidFont));
                    target.ModifyBase(StateType.Normal, Utils.ColorToGdkColor(colorEntryValidationInvalidBackground));
                    if (pLabel != null) pLabel.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(colorEntryValidationInvalidFont));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DataTable

        //Init DataTable Structure/Columns from Dictionary
        //Use like
        //Dictionary<string, Type> dtColumns = new Dictionary<string, Type>();
        //dtColumns.Add("VatRate", typeof(string));
        public static DataTable InitDataTable(Dictionary<string, Type> pDataTableColumns)
        {
            DataTable result = new DataTable();

            try
            {
                foreach (var item in pDataTableColumns)
                {
                    DataColumn dataColumn = new DataColumn(item.Key, item.Value);
                    result.Columns.Add(dataColumn);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Protection

        //UNDER CONSTRUCTION
        //Required to add other Parameters to be Full Protected, Initialized Date etc
        public static bool SaveSystemProtection(cfg_configurationcountry pConfigurationCountry, cfg_configurationcurrency pConfigurationCurrency)
        {
            bool result = false;

            try
            {
                //COMPANY_NAME
                cfg_configurationpreferenceparameter configurationPreferenceParameterCompanyName = (FrameworkUtils.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_NAME")) as cfg_configurationpreferenceparameter);
                //COMPANY_FISCALNUMBER
                cfg_configurationpreferenceparameter configurationPreferenceParameterCompanyFiscalNumber = (FrameworkUtils.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_FISCALNUMBER")) as cfg_configurationpreferenceparameter);
                //COMPANY_CAE
                cfg_configurationpreferenceparameter configurationPreferenceParameterCompanyCAE = (FrameworkUtils.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_CAE")) as cfg_configurationpreferenceparameter);
                //COMPANY_CIVIL_REGISTRATION
                cfg_configurationpreferenceparameter configurationPreferenceParameterCompanyCivilRegistration = (FrameworkUtils.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_CIVIL_REGISTRATION")) as cfg_configurationpreferenceparameter);
                //COMPANY_CIVIL_REGISTRATION_ID
                cfg_configurationpreferenceparameter configurationPreferenceParameterCompanyCivilRegistrationID = (FrameworkUtils.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_CIVIL_REGISTRATION_ID")) as cfg_configurationpreferenceparameter);
                //COMPANY_COUNTRY
                //Assign and Save Country and Country Code 2 From entryBoxSelectCustomerCountry
                cfg_configurationpreferenceparameter configurationPreferenceParameterCompanyCountry = (FrameworkUtils.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_COUNTRY")) as cfg_configurationpreferenceparameter);
                configurationPreferenceParameterCompanyCountry.Value = pConfigurationCountry.Designation;
                configurationPreferenceParameterCompanyCountry.Save();
                //COMPANY_COUNTRY_CODE2
                cfg_configurationpreferenceparameter configurationPreferenceParameterCompanyCountryCode2 = (FrameworkUtils.GetXPGuidObjectFromCriteria(typeof(cfg_configurationpreferenceparameter), string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Token = '{0}')", "COMPANY_COUNTRY_CODE2")) as cfg_configurationpreferenceparameter);
                configurationPreferenceParameterCompanyCountryCode2.Value = pConfigurationCountry.Code2;
                configurationPreferenceParameterCompanyCountryCode2.Save();

                //Lock Country creating Hash
                string systemProtected = string.Format(
                    "{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}",
                    //Config
                    pConfigurationCountry.Oid.ToString(),
                    pConfigurationCountry.Code2,
                    pConfigurationCurrency.Oid.ToString(),
                    //Database
                    configurationPreferenceParameterCompanyName.Value,
                    configurationPreferenceParameterCompanyFiscalNumber.Value,
                    configurationPreferenceParameterCompanyCAE.Value,
                    configurationPreferenceParameterCompanyCivilRegistration.Value,
                    configurationPreferenceParameterCompanyCivilRegistrationID.Value
                );

                string systemProtectedSalted = SaltedString.GenerateSaltedString(systemProtected);
                _log.Debug(String.Format("systemProtected: [{0}]", systemProtected));
                _log.Debug(String.Format("systemProtectedSalted: [{0}]", systemProtectedSalted));

                //Change Configuration
                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("appSystemProtection", systemProtectedSalted);
                values.Add("xpoOidConfigurationCountrySystemCountry", pConfigurationCountry.Oid.ToString());
                values.Add("xpoOidConfigurationCountrySystemCountryCountryCode2", pConfigurationCountry.Code2);
                values.Add("xpoOidConfigurationCurrencySystemCurrency)", pConfigurationCurrency.Oid.ToString());
                Utils.AddUpdateSettings(values);

                result = true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

		//TK016235 BackOffice - Mode
        public static void startReportsMenuFromBackOffice(Window pSourceWindow)        
        {
            try
            {
                PosReportsDialog dialog = new PosReportsDialog(pSourceWindow, Gtk.DialogFlags.DestroyWithParent);
                int response = dialog.Run();
                dialog.Destroy();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }            
        }

        public static void openWorkSessionFromBackOffice(Window pSourceWindow)
        {
            try
            {
                PosCashDialog dialog = new PosCashDialog(pSourceWindow, Gtk.DialogFlags.DestroyWithParent);
                int response = dialog.Run();
                dialog.Destroy();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }


        //TK016235 BackOffice - Mode
        public static void startNewDocumentFromBackOffice(Window pSourceWindow)
        {
            try
            {
                //Call New DocumentFinance Dialog
                PosDocumentFinanceDialog dialogNewDocument = new PosDocumentFinanceDialog(pSourceWindow, DialogFlags.DestroyWithParent);
                ResponseType responseNewDocument = (ResponseType)dialogNewDocument.Run();
                if (responseNewDocument == ResponseType.Ok)
                {
                }
                dialogNewDocument.Destroy();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //TK016235 BackOffice - Mode
        public static void startDocumentsMenuFromBackOffice(Window pSourceWindow, int docChoice)
        {
            try
            {
                PosDocumentFinanceSelectRecordDialog dialog = new PosDocumentFinanceSelectRecordDialog(pSourceWindow, Gtk.DialogFlags.DestroyWithParent, docChoice);
                if (docChoice == 0)
                {
                    ResponseType response = (ResponseType)dialog.Run();
                }                
                dialog.Destroy();

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }


        public static void startReportFromBackOffice(Window pSourceWindow)
        {
            //CustomReport.ProcessReportDocumentMasterList(displayMode
            //           , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], token.ToString().ToLower())
            //           , "[DocumentFinanceMaster.DocumentType.Ord]"
            //           , "([DocumentFinanceMaster.DocumentType.Code]) [DocumentFinanceMaster.DocumentType.Designation]",/* IN009066 */
            //           reportFilter,
            //           reportFilterHumanReadable
            //           );
        }


        public static void startTreeViewFromBackOffice(Dictionary<string, AccordionNode> _accordionChildDocumentsTemp)
        {
            try
            {
                

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //UNDER CONSTRUCTION
        public static bool CheckValidSystemProtection()
        {
            bool result = false;

            try
            {
                result = true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

    }
}