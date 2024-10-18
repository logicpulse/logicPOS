using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.BackOffice.Dialogs.Articles;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Logic.Others;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.DTOs.Printing;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Globalization;
using LogicPOS.Modules;
using LogicPOS.Persistence.Services;
using LogicPOS.Settings;
using LogicPOS.UI;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace logicpos
{
    internal static class Utils
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ShowMessage Touch

        //Touch Dialog
        public static ResponseType ShowMessageTouch(Window parentWindow, Gtk.DialogFlags pDialogFlags, MessageType pMessageType, ButtonsType pButtonsType, string pWindowTitle, string pMessage)
        {
            //Default Size
            Size size = new Size(600, 400);
            return ShowMessageBox(parentWindow, pDialogFlags, size, pMessageType, pButtonsType, pWindowTitle, pMessage);
        }

        public static ResponseType ShowMessageBox(Window parentWindow,
                                                  DialogFlags flags,
                                                  Size size,
                                                  MessageType messageType,
                                                  ButtonsType buttonsType,
                                                  string title,
                                                  string message)
        {
            var alertSettings = new CustomAlertSettings();
            var alertButtons = new CustomAlertButtons(alertSettings);

            //Init Local Vars
            string fileImageDialog, fileImageWindowIcon;
            ResponseType dialogResponse = ResponseType.None;

            ActionAreaButtons actionAreaButtons = alertButtons.GetActionAreaButtons(buttonsType);

            fileImageDialog = alertSettings.GetDialogImage(messageType);
            fileImageWindowIcon = alertSettings.GetDialogIcon(messageType);

            CustomAlert dialog = new CustomAlert(parentWindow,
                                                 flags,
                                                 size,
                                                 title,
                                                 message,
                                                 actionAreaButtons,
                                                 fileImageWindowIcon,
                                                 fileImageDialog);

            dialog.HideCloseButton();

            dialogResponse = (ResponseType)dialog.Run();

            if (dialogResponse != ResponseType.Apply)
            {
                dialog.Destroy();
            }

            return dialogResponse;
        }

        internal static ResponseType ShowMessageTouchErrorPrintingTicket(
            Gtk.Window parentWindow,
            PrinterDto printer,
            Exception pEx)
        {
            //Protection when Printer is Null, ex printing Ticket Articles (Printer is Assign in Article)
            string printerDesignation = (printer != null) ? printer.Designation : "NULL";
            string printerNetworkName = (printer != null) ? printer.NetworkName : "NULL";
            return ShowMessageBox(parentWindow, DialogFlags.Modal, new Size(800, 400), MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_error"), string.Format(GeneralUtils.GetResourceByName("dialog_message_error_printing_ticket"), printerDesignation, printerNetworkName, pEx.Message));
        }

        public static bool ShowMessageTouchRequiredValidPrinter(Window parentWindow, sys_configurationprinters pPrinter)
        {
            bool result = pPrinter == null && TerminalSettings.LoggedTerminal.ThermalPrinter == null;

            if (result)
            {
                ShowMessageTouch(parentWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_information"), GeneralUtils.GetResourceByName("dialog_message_required_valid_printer"));
            }

            return result;
        }

        public static void ShowMessageTouchTerminalWithoutAssociatedPrinter(Window parentWindow, string pDocumentType)
        {
            if (
                (
                    GlobalApp.Notifications != null && GlobalApp.Notifications.ContainsKey("SHOW_PRINTER_UNDEFINED")
                )
                && GlobalApp.Notifications["SHOW_PRINTER_UNDEFINED"] == true
            )
            {
                ResponseType responseType = ShowMessageBox(parentWindow, DialogFlags.Modal, new Size(550, 400), MessageType.Question, ButtonsType.YesNo, GeneralUtils.GetResourceByName("global_information")
                    , string.Format(GeneralUtils.GetResourceByName("dialog_message_show_printer_undefined_on_print"), pDocumentType)
                );
                if (responseType == ResponseType.No) GlobalApp.Notifications["SHOW_PRINTER_UNDEFINED"] = false;
            }
        }

        public static void ShowMessageTouchErrorRenderTheme(Window parentWindow, string pErrorMessage)
        {
            string errorMessage = string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, ResourceNames.APP_ERROR_RENDERING_THEME), POSSettings.FileTheme, pErrorMessage);
            ShowMessageBox(parentWindow, DialogFlags.Modal, new Size(600, 500), MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_error"), errorMessage);
            Environment.Exit(0);
        }

        public static void ShowMessageBoxUnlicensedError(
            Window parentWindow,
            string pErrorMessage)
        {
            string errorMessage = string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, ResourceNames.APP_ERROR_APPLICATION_UNLICENCED_FUNCTION_DISABLED), pErrorMessage);
            ShowMessageBox(parentWindow, DialogFlags.Modal, new Size(600, 300), MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_error"), errorMessage);
        }

        public static ResponseType ShowMessageTouchCheckIfFinanceDocumentHasValidDocumentDate(Window parentWindow, DocumentProcessingParameters pParameters)
        {
            //Default is Yes
            ResponseType result = ResponseType.Yes;

            try
            {
                fin_documentfinanceyearserieterminal documentFinanceYearSerieTerminal = null;
                fin_documentfinanceseries documentFinanceSerie = null;
                if (TerminalSettings.LoggedTerminal != null)
                {
                    documentFinanceYearSerieTerminal = DocumentProcessingSeriesUtils.GetDocumentFinanceYearSerieTerminal(pParameters.DocumentType);
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
                    dateLastDocumentFromSerie = DocumentProcessingUtils.GetLastDocumentDateTime(string.Format("DocumentSerie = '{0}'", documentFinanceSerie.Oid)).Date;
                }

                //Check if DocumentDate is greater than dateLastDocumentFromSerie (If Defined) else if is First Document in Series Skip
                if (pParameters.DocumentDateTime < dateLastDocumentFromSerie && dateLastDocumentFromSerie != DateTime.MinValue)
                {
                    result = ShowMessageTouch(parentWindow, DialogFlags.Modal, MessageType.Question, ButtonsType.Close, GeneralUtils.GetResourceByName("global_warning"), GeneralUtils.GetResourceByName("dialog_message_systementry_is_less_than_last_finance_document_series"));
                }
                else
                {
                    //WARNING_RULE_SYSTEM_DATE_GLOBAL
                    DateTime dateTimeLastDocument = DocumentProcessingUtils.GetLastDocumentDateTime();
                    //Check if DocumentDate is greater than dateLastDocument (If Defined) else if is First Document in Series Skip
                    if (pParameters.DocumentDateTime < dateTimeLastDocument && dateTimeLastDocument != DateTime.MinValue)
                    {
                        result = ShowMessageTouch(parentWindow, DialogFlags.Modal, MessageType.Question, ButtonsType.Close, GeneralUtils.GetResourceByName("global_warning"), GeneralUtils.GetResourceByName("dialog_message_systementry_is_less_than_last_finance_document_series"));
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return result;
        }

        public static void ShowMessageTouchSimplifiedInvoiceMaxValueExceedForFinalConsumer(Window parentWindow, decimal pCurrentTotal, decimal pMaxTotal)
        {
            ShowMessageBox(parentWindow, DialogFlags.Modal, new Size(550, 480), MessageType.Info, ButtonsType.Close,
                GeneralUtils.GetResourceByName("global_warning"),
                string.Format(
                    GeneralUtils.GetResourceByName("dialog_message_value_exceed_simplified_invoice_for_final_or_annonymous_consumer")
                    , string.Format("{0}: {1}", GeneralUtils.GetResourceByName("global_total"), DataConversionUtils.DecimalToStringCurrency(pCurrentTotal, XPOSettings.ConfigurationSystemCurrency.Acronym))
                    , string.Format("{0}: {1}", GeneralUtils.GetResourceByName("global_maximum"), DataConversionUtils.DecimalToStringCurrency(pMaxTotal, XPOSettings.ConfigurationSystemCurrency.Acronym))
                )
            );
        }

        public static ResponseType ShowMessageTouchSimplifiedInvoiceMaxValueExceed(Window parentWindow, ShowMessageTouchSimplifiedInvoiceMaxValueExceedMode pMode, decimal pCurrentTotal, decimal pMaxTotal, decimal pCurrentTotalServices, decimal pMaxTotalServices)
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
                    messageMode = GeneralUtils.GetResourceByName("dialog_message_value_exceed_simplified_invoice_max_value_mode_paymentdialog");
                    messageType = MessageType.Question;
                    buttonsType = ButtonsType.YesNo;
                    break;
                case ShowMessageTouchSimplifiedInvoiceMaxValueExceedMode.DocumentFinanceDialog:
                    messageMode = GeneralUtils.GetResourceByName("dialog_message_value_exceed_simplified_invoice_max_value_mode_paymentdialog_documentfinancedialog");
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
                        , GeneralUtils.GetResourceByName("global_total")
                        , DataConversionUtils.DecimalToStringCurrency(pCurrentTotal, XPOSettings.ConfigurationSystemCurrency.Acronym)
                        , GeneralUtils.GetResourceByName("global_maximum")
                        , DataConversionUtils.DecimalToStringCurrency(pMaxTotal, XPOSettings.ConfigurationSystemCurrency.Acronym)
                    );
                }

                if (pCurrentTotalServices > pMaxTotalServices)
                {
                    messageMaxExceedServices = string.Format(
                        "{1}: {2}{0}{3}: {4}"
                        , Environment.NewLine
                        , GeneralUtils.GetResourceByName("global_services")
                        , DataConversionUtils.DecimalToStringCurrency(pCurrentTotalServices, XPOSettings.ConfigurationSystemCurrency.Acronym)
                        , GeneralUtils.GetResourceByName("global_maximum")
                        , DataConversionUtils.DecimalToStringCurrency(pMaxTotalServices, XPOSettings.ConfigurationSystemCurrency.Acronym)
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

                    result = ShowMessageBox(parentWindow, DialogFlags.Modal, new Size(550, 440), messageType, buttonsType,
                        GeneralUtils.GetResourceByName("global_warning"),
                        string.Format(GeneralUtils.GetResourceByName("dialog_message_value_exceed_simplified_invoice_max_value"), message, messageMode
                        )
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }
        public static bool ShowMessageMinimumStock(Window parentWindow, Guid pArticleOid, decimal pNewQuantity)
        {
            bool unusedBool;
            return ShowMessageMinimumStock(parentWindow, pArticleOid, pNewQuantity, out unusedBool);
        }

        public static bool ShowMessageMinimumStock(Window parentWindow, Guid pArticleOid, decimal pNewQuantity, out bool showMessage)
        {
            showMessage = false;
            Size size = new Size(500, 350);
            fin_article article = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), pArticleOid);
            decimal articleStock;
            try
            {
                string stockQuery = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;", article.Oid);
                articleStock = Convert.ToDecimal(XPOSettings.Session.ExecuteScalar(stockQuery));
            }
            catch
            {
                _logger.Debug("Article with stock 0 or NULL");
                articleStock = 0;
            }

            string childStockMessage = Environment.NewLine + Environment.NewLine + "Stock de artigos associados: " + Environment.NewLine;
            //Composite article Messages
            int childStockAlertCount = 0;
            if (article.IsComposed)
            {
                foreach (fin_articlecomposition item in article.ArticleComposition)
                {
                    fin_article child = item.ArticleChild;
                    decimal childStock = 0;
                    try
                    {
                        string stockQuery = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;", item.ArticleChild.Oid);
                        childStock = Convert.ToDecimal(XPOSettings.Session.ExecuteScalar(stockQuery));
                    }
                    catch
                    {
                        _logger.Debug("Article child with stock 0 or NULL");
                        childStock = 0;
                    }
                    var childStockAfterChanged = childStock - (pNewQuantity * item.Quantity);
                    if (childStockAfterChanged <= child.MinimumStock)
                    {
                        childStockMessage += Environment.NewLine + GeneralUtils.GetResourceByName("global_article") + ": " + child.Designation + Environment.NewLine + GeneralUtils.GetResourceByName("global_total_stock") + ": " + DataConversionUtils.DecimalToString(Convert.ToDecimal(childStock), "0.00") + Environment.NewLine + GeneralUtils.GetResourceByName("global_minimum_stock") + ": " + DataConversionUtils.DecimalToString(Convert.ToDecimal(child.MinimumStock), "0.00") + Environment.NewLine;
                        childStockAlertCount++;
                    }
                }
            }
            var stockQuantityAfterChanged = articleStock - pNewQuantity;
            //Mensagem de stock apenas para artigos da classe Produtos
            if ((stockQuantityAfterChanged <= article.MinimumStock || childStockAlertCount > 0) && article.Class.Oid == Guid.Parse("6924945d-f99e-476b-9c4d-78fb9e2b30a3"))
            {
                if (article.IsComposed)
                {
                    size = new Size(650, 480);
                    var response = ShowMessageBox(parentWindow, DialogFlags.DestroyWithParent, size, MessageType.Question, ButtonsType.YesNo, GeneralUtils.GetResourceByName("global_stock_movements"), GeneralUtils.GetResourceByName("window_check_stock_question") + Environment.NewLine + Environment.NewLine + GeneralUtils.GetResourceByName("global_article") + ": " + article.Designation + Environment.NewLine + GeneralUtils.GetResourceByName("global_total_stock") + ": " + DataConversionUtils.DecimalToString(Convert.ToDecimal(articleStock), "0.00") + Environment.NewLine + GeneralUtils.GetResourceByName("global_minimum_stock") + ": " + DataConversionUtils.DecimalToString(Convert.ToDecimal(article.MinimumStock), "0.00") + childStockMessage);
                    if (response == ResponseType.Yes)
                    {
                        showMessage = true;
                        return true;
                    }
                    else
                    {
                        showMessage = true;
                        return false;
                    }
                }
                else
                {
                    var response = ShowMessageBox(parentWindow,
                                                  DialogFlags.DestroyWithParent,
                                                  size,
                                                  MessageType.Question,
                                                  ButtonsType.YesNo,
                                                  GeneralUtils.GetResourceByName("global_stock_movements"),
                                                  GeneralUtils.GetResourceByName("window_check_stock_question") + Environment.NewLine + Environment.NewLine + GeneralUtils.GetResourceByName("global_article") + ": " + article.Designation + Environment.NewLine + GeneralUtils.GetResourceByName("global_total_stock") + ": " + DataConversionUtils.DecimalToString(Convert.ToDecimal(articleStock), "0.00") + Environment.NewLine + GeneralUtils.GetResourceByName("global_minimum_stock") + ": " + DataConversionUtils.DecimalToString(Convert.ToDecimal(article.MinimumStock), "0.00"));
                    
                    if (response == ResponseType.Yes)
                    {
                        showMessage = true;
                        return true;
                    }
                    else
                    {
                        showMessage = true;
                        return false;
                    }
                }
            }
            showMessage = false;
            return false;
        }


        public static void ShowMessageTouchProtectedDeleteRecordMessage(Window parentWindow)
        {
            ShowMessageTouch(
                parentWindow,
                DialogFlags.DestroyWithParent | DialogFlags.Modal,
                MessageType.Error,
                ButtonsType.Ok,
                GeneralUtils.GetResourceByName("window_title_dialog_delete_record"),
                GeneralUtils.GetResourceByName("dialog_message_delete_record_show_protected_record"))
            ;
        }

        public static void ShowMessageTouchProtectedUpdateRecordMessage(Window parentWindow)
        {
            ShowMessageTouch(
                parentWindow,
                DialogFlags.DestroyWithParent | DialogFlags.Modal,
                MessageType.Error,
                ButtonsType.Ok,
                GeneralUtils.GetResourceByName("window_title_dialog_update_record"),
                GeneralUtils.GetResourceByName("dialog_message_update_record_show_protected_record"))
            ;
        }

        public static void ShowMessageTouchUnsupportedResolutionDetectedAndExit(Window parentWindow, int width, int height)
        {
            string message = string.Format(GeneralUtils.GetResourceByName("app_error_unsupported_resolution_detected"), width, height);
            ShowMessageBox(parentWindow, DialogFlags.Modal, new Size(600, 300), MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_error"), message);
            Environment.Exit(Environment.ExitCode);
        }

        /// <summary>
        /// Responsible for the dialogbox for screen resolution issues.
        /// It takes the control of application when user opt to do not follow with low-resolution app startup, closing the application.
        /// <para>See also "IN008023: apply "800x600" settings as default."</para>
        /// </summary>
        /// <param name="parentWindow"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void ShowMessageTouchUnsupportedResolutionDetectedDialogbox(Window parentWindow, int width, int height)
        {
            string message = string.Format(GeneralUtils.GetResourceByName("app_error_unsupported_resolution_detected"), width, height, GeneralUtils.GetResourceByName("global_treeview_true"));
            ResponseType dialogResponse = ShowMessageBox(parentWindow, DialogFlags.Modal, new Size(600, 300), MessageType.Question, ButtonsType.YesNo, GeneralUtils.GetResourceByName("global_information"), message);
            if (dialogResponse == ResponseType.No)
            {
                Environment.Exit(Environment.ExitCode);
            }
        }

        public static void ShowMessageTouchErrorTryToIssueACreditNoteExceedingSourceDocumentArticleQuantities(Window parentWindow, decimal currentQuantity, decimal maxPossibleQuantity)
        {
            string message = string.Format(GeneralUtils.GetResourceByName("dialog_message_error_try_to_issue_a_credit_note_exceeding_source_document_article_quantities"), currentQuantity, maxPossibleQuantity);
            ShowMessageBox(parentWindow, DialogFlags.Modal, new Size(700, 400), MessageType.Info, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_information"), message);
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

        public static ResponseText GetInputText(Window parentWindow, DialogFlags pDialogFlags, string pWindowIcon, string pEntryLabel, string pDefaultValue, string pRule, bool pRequired)
        {
            return GetInputText(parentWindow, pDialogFlags, GeneralUtils.GetResourceByName("window_title_default_input_text_dialog"), pWindowIcon, pEntryLabel, pDefaultValue, pRule, pRequired);
        }

        public static ResponseText GetInputText(Window parentWindow, DialogFlags pDialogFlags, string pWindowTitle, string pWindowIcon, string pEntryLabel, string pDefaultValue, string pRule, bool pRequired)
        {
            ResponseText result = new ResponseText();
            result.ResponseType = ResponseType.Cancel;

            //Prepare Dialog
            PosInputTextDialog dialog = new PosInputTextDialog(parentWindow, pDialogFlags, pWindowTitle, pWindowIcon, pEntryLabel, pDefaultValue, pRule, pRequired);

            //Call Dialog
            try
            {
                result.ResponseType = (ResponseType)dialog.Run();
                result.Text = dialog.Value;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            finally
            {
                dialog.Destroy();
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Images 

        public static Gdk.Pixmap FileToPixmap(string pFilename)
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
        public static Gdk.Pixbuf FileToPixBuf(string pFilename)
        {
            if (pFilename != null && File.Exists(pFilename))
            {
                var buffer = File.ReadAllBytes(pFilename);
                return new Gdk.Pixbuf(buffer);
            }
            else
            {
                return null;
            }
        }

        //Returns a Resized PixBuf from File Path - Usefull for FileChooserButtons
        public static Gdk.Pixbuf ResizeAndCropFileToPixBuf(string pFilename, Size pSize)
        {
            if (pFilename != null && File.Exists(pFilename))
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(pFilename);
                image = ResizeAndCrop(image, new System.Drawing.Size(pSize.Width, pSize.Height));
                return ImageToPixbuf(image);
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

                //_logger.Debug(
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
                _logger.Error(ex.Message, ex);
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

            //_logger.Debug(
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

            //_logger.Debug(
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
        public static System.Drawing.Image ImageTextOverlay(System.Drawing.Image pImage, string pLabel, Rectangle pTranspRectangle, Color pFontColor, string pFontName = "Tahoma", int pFontSize = 12, int pTransparentLevel = 128)
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

        public static Point StringToPosition(string position)
        {
            string[] splitted = position.Split(',');
            short x = Convert.ToInt16(splitted[0]);
            short y = Convert.ToInt16(splitted[1]);
            return new Point(x, y);
        }

        public static TableConfig StringToTableConfig(string pTableConfig)
        {
            string[] splitted = pTableConfig.Split(',');
            TableConfig resultTableConfig = new TableConfig();
            try
            {
                resultTableConfig = new TableConfig(Convert.ToUInt16(splitted[0]), Convert.ToUInt16(splitted[1]));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return resultTableConfig;
            }
            return resultTableConfig;
        }

        public static char UnicodeHexadecimalStringToChar(string pInput)
        {
            char result = (char)int.Parse(pInput, NumberStyles.HexNumber);
            return result;
        }


        //Check if is letter with RegEx, better than Char.IsLetter that returns º and ª too
        public static bool IsLetter(char input)
        {
            string pattern = @"^[a-zA-ZçÇÁáÉéÍóÚúÀàÈèÌìÒòÙùÃãÕõÂâÊêÎîÔôÛû]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(Convert.ToString(input));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Windows

        public static string GetWindowTitle(string pTitle)
        {
            //return string.Format("{0} {1} : {2}", SettingsApp.AppName, FrameworkUtils.ProductVersion, pTitle);
            return string.Format("{0} : {1}", POSSettings.AppName, pTitle);
        }

        /// <summary>
        /// This method is responsible for dinamic dialog title creation, based on option chosen by user on BackOffice action menu.
        /// Related to #IN009039.
        /// </summary>
        /// <param name="pTitle"></param>
        /// <param name="dialogMode"></param>
        /// <returns></returns>
        public static string GetWindowTitle(string dialogWindowTitle, logicpos.Classes.Enums.Dialogs.DialogMode dialogMode)
        {
            string action = string.Empty;

            switch (dialogMode)
            {
                case Classes.Enums.Dialogs.DialogMode.Insert:
                    action = GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_insert");
                    break;
                case Classes.Enums.Dialogs.DialogMode.Update:
                    action = GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_update");
                    break;
                case Classes.Enums.Dialogs.DialogMode.View:
                    action = GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_view");
                    break;
                default:
                    break;
            }

            return string.Format("{0} :: {1} {2}", POSSettings.AppName, action, dialogWindowTitle); // FrameworkUtils.ProductVersion
        }

        public static Size GetScreenSize()
        {
            Size result = new Size();

            // Moke Window only to extract its Resolution
            Window window = new Window("");
            Gdk.Screen screen = window.Screen;
            Gdk.Rectangle monitorGeometry = screen.GetMonitorGeometry(AppSettings.Instance.appScreen);
            result = new Size(monitorGeometry.Width, monitorGeometry.Height);
            // CleanUp
            window.Dispose();
            screen.Dispose();

            return result;
        }

        // Used to get the final Resolution for Render template, it uses some stuff from config and detected ScreenSize to get guest best resolution for themes

        public static Size GetThemeScreenSize()
        {
            return GetThemeScreenSize(GetScreenSize());
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


        public static string OpenNewSerialNumberCompositePopUpWindow(Window parentWindow, Entity pXPGuidObject, out List<fin_articleserialnumber> pSelectedCollection, string pSerialNumber = "", List<fin_articleserialnumber> pSelectedCollectionToFill = null)
        {
            try
            {
                DialogArticleCompositionSerialNumber dialog = new DialogArticleCompositionSerialNumber(parentWindow, GetGenericTreeViewXPO<TreeViewArticleStock>(parentWindow), DialogFlags.DestroyWithParent, pXPGuidObject, pSelectedCollectionToFill, pSerialNumber);
                ResponseType response = (ResponseType)dialog.Run();
                if (response == ResponseType.Ok)
                {
                    string insertedSerialNumber = dialog.EntryBoxSerialNumber1.EntryValidation.Text;
                    pSelectedCollection = dialog.SelectedAssocietedArticles;
                    dialog.Destroy();
                    return insertedSerialNumber;
                }
                dialog.Destroy();
                pSelectedCollection = dialog.SelectedAssocietedArticles;
                return pSerialNumber;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                pSelectedCollection = null;
                return "";
            }
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
                _logger.Error("ScreenSize GetSupportedScreenResolution(Size screenSize) :: " + ex.Message, ex);

                /* IN009034 */
                GlobalApp.DialogThreadNotify.WakeupMain();

                string message = string.Format(GeneralUtils.GetResourceByName("app_error_unsupported_resolution_detected"), screenSize.Width, screenSize.Height, GeneralUtils.GetResourceByName("global_treeview_true"));
                ShowMessageTouchUnsupportedResolutionDetectedDialogbox(GlobalApp.StartupWindow, screenSize.Width, screenSize.Height);

                supportedScreenSizeEnum = ScreenSize.resDefault;
            }
            return supportedScreenSizeEnum;
        }



        public static Dialog CreateSplashScreen(
            Window parent,
            bool dbExists,
            string backupProcess = "")
        {
            string loadingImage = PathsSettings.ImagesFolderLocation + @"Other\working.gif";

            Dialog dialog = new Dialog(
                "Working",
                parent,
                DialogFlags.Modal | DialogFlags.DestroyWithParent);

            dialog.WindowPosition = WindowPosition.Center;

            Label labelBoot;

            if (dbExists)
            {
                labelBoot = new Label(GeneralUtils.GetResourceByName("global_load"));
            }
            else
            {
                labelBoot = new Label(GeneralUtils.GetResourceByName("global_load_first_time"));
            }

            if (backupProcess != string.Empty)
            {
                labelBoot = new Label(backupProcess);
            }

            labelBoot.ModifyFont(Pango.FontDescription.FromString("Trebuchet MS 10 Bold"));
            labelBoot.ModifyFg(StateType.Normal, Color.DarkSlateGray.ToGdkColor());
            dialog.Decorated = false;
            dialog.ActionArea.Destroy();
            dialog.Fullscreen();
            Gtk.Image imageWorking = new Gtk.Image(loadingImage);
            dialog.VBox.PackStart(imageWorking);
            dialog.VBox.PackStart(labelBoot);
            dialog.ShowAll();

            return dialog;
        }

        public static void NotifyLoadingIsDone()
        {
            if (GlobalApp.LoadingDialog != null)
            {
                GlobalApp.LoadingDialog.Destroy();
            }
        }

        public static void ThreadStart(
            Window sourceWindow,
            Thread thread,
            string backupProcess)
        {
            GlobalApp.DialogThreadNotify = new ThreadNotify(new ReadyEvent(NotifyLoadingIsDone));
            thread.Start();

            if (sourceWindow != null)
            {
                GlobalApp.LoadingDialog = CreateSplashScreen(
                    sourceWindow,
                    DatabaseService.DatabaseExists(),
                    backupProcess);

                GlobalApp.LoadingDialog.Run();
            }
        }

        //Sample Test Routines : Used in Startup Window
        public static void LargeComputation(int pMilliseconds)
        {
            Thread.Sleep(pMilliseconds);
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
                        if (debug) _logger.Debug(string.Format("AddOrUpdateAppSettings: Add Key: [{0}] = [{1}]", item.Key, settings[item.Key].Value));
                    }
                    else
                    {
                        settings[item.Key].Value = item.Value;
                        //Debug
                        if (debug) _logger.Debug(string.Format("AddOrUpdateAppSettings: Modified Key: [{0}] = [{1}]", item.Key, settings[item.Key].Value));
                    }

                    //Assign to Memory
                    ConfigurationManager.AppSettings.Set(item.Key, item.Value);
                }

                //Save
                if (Debugger.IsAttached == true)
                {
                    if (debug) _logger.Debug("Save AppSettings with debugger");
                    configFile.SaveAs(configFileName, ConfigurationSaveMode.Modified);
                }
                else
                {
                    if (debug) _logger.Debug("Save AppSettings without debugger");
                    configFile.Save(ConfigurationSaveMode.Modified);
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                _logger.Error(ex.Message, ex);
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
                _logger.Error(ex.Message, ex);
            }

            return result;
        }


        public static pos_configurationplaceterminal GetOrCreateTerminal()
        {
            pos_configurationplaceterminal configurationPlaceTerminal = null;

            try
            {

                LicenseSettings.LicenseHardwareId = AppSettings.Instance.appHardwareId;

                try
                {

                    configurationPlaceTerminal = (pos_configurationplaceterminal)XPOUtility.GetXPGuidObjectFromField(typeof(pos_configurationplaceterminal), "HardwareId", LicenseSettings.LicenseHardwareId);
                }
                catch (Exception ex)
                {
                    _logger.Error("pos_configurationplaceterminal GetTerminal() :: Try TerminalID from Database: " + ex.Message, ex);
                }

                //Create a new db terminal
                if (configurationPlaceTerminal == null)
                {
                    try
                    {
                        //Persist Terminal in DB
                        configurationPlaceTerminal = new pos_configurationplaceterminal(XPOSettings.Session)
                        {
                            Ord = XPOUtility.GetNextTableFieldID("pos_configurationplaceterminal", "Ord"),
                            Code = XPOUtility.GetNextTableFieldID("pos_configurationplaceterminal", "Code"),
                            Designation = "Terminal #" + XPOUtility.GetNextTableFieldID("pos_configurationplaceterminal", "Code"),
                            HardwareId = LicenseSettings.LicenseHardwareId
                        };
                        configurationPlaceTerminal.Save();
                    }
                    catch (Exception ex)
                    {
                        /* IN009034 */
                        GlobalApp.DialogThreadNotify.WakeupMain();

                        _logger.Error(string.Format("pos_configurationplaceterminal GetTerminal() :: Error! Can't Register a new TerminalId [{0}] with HardwareId: [{1}], Error: [2]", configurationPlaceTerminal.Oid, configurationPlaceTerminal.HardwareId, ex.Message), ex);
                        ShowMessageBox(null, DialogFlags.Modal, new Size(600, 300), MessageType.Error, ButtonsType.Close, GeneralUtils.GetResourceByName("global_error"), string.Format(GeneralUtils.GetResourceByName("dialog_message_error_register_new_terminal"), configurationPlaceTerminal.HardwareId));
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("pos_configurationplaceterminal GetTerminal() :: " + ex.Message, ex);
            }

            return configurationPlaceTerminal;
        }

        public static bool CloseAllOpenTerminals(Window parentWindow, Session pSession)
        {
            bool result = false;

            try
            {
                // SELECT Oid, PeriodType, SessionStatus, Designation FROM pos_worksessionperiod WHERE PeriodType = 1 AND SessionStatus = 0;
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse("PeriodType = 1 AND SessionStatus = 0");
                SortProperty[] sortProperty = new SortProperty[2];
                sortProperty[0] = new SortProperty("CreatedAt", SortingDirection.Ascending);
                XPCollection xpcWorkingSessionPeriod = new XPCollection(pSession, typeof(pos_worksessionperiod), criteriaOperator, sortProperty);
                DateTime dateTime = XPOUtility.CurrentDateTimeAtomic();
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
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Windows

        //Helpers
        public static void ShowFrontOffice(Window window)
        {

            if (GlobalApp.PosMainWindow == null)
            {
                Predicate<dynamic> predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "PosMainWindow");
                dynamic themeWindow = GlobalApp.Theme.Theme.Frontoffice.Window.Find(predicate);

                CustomAppOperationMode customAppOperationMode = AppOperationModeSettings.CustomAppOperationMode;
                string windowImageFileName = string.Format(themeWindow.Globals.ImageFileName, customAppOperationMode.AppOperationTheme, GlobalApp.ScreenSize.Width, GlobalApp.ScreenSize.Height);
                GlobalApp.PosMainWindow = new POSMainWindow(windowImageFileName);
            }
            else
            {
                GlobalApp.PosMainWindow.UpdateUI();
                GlobalApp.PosMainWindow.Show();
            };
            window.Hide();

        }

        public static void ShowBackOffice(Window pHideWindow)
        {
            if (GlobalApp.BackOffice == null)
            {
                GlobalApp.BackOffice = new BackOfficeWindow();
            }
            else
            {
                GlobalApp.BackOffice.Show();
            }

            pHideWindow.Hide();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Themes and Backgrounds

        public static string GetThemeFileLocation(string pFile)
        {
            string pathThemes = PathsSettings.Paths["themes"].ToString();
            /* IN008024 */
            return string.Format(@"{0}{1}\{2}", pathThemes, GeneralSettings.AppTheme, pFile);
        }

        public static Gtk.Style GetThemeStyleBackground(string pFile)
        {
            /* IN008024 */
            string fileImageBackground = GetThemeFileLocation(pFile);

            if (fileImageBackground != null && File.Exists(fileImageBackground))
            {
                Gdk.Pixmap pixmap = FileToPixmap(fileImageBackground);

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
                _logger.Error(string.Format("Missing Theme[{0}] Image: [{1}]", GeneralSettings.AppTheme, fileImageBackground));
                return null;
            }
        }

        public static Gtk.Style GetImageBackgroundDashboard(Gdk.Pixbuf image)
        {

            if (image == null)
            {
                return null;
            }

            Gdk.Pixmap pixmap = PixbufToPixmap(image);

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

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Notifications UI

        public static void ShowNotifications(Window parentWindow)
        {
            ShowNotifications(parentWindow, XPOSettings.Session);
        }

        public static void ShowNotifications(Window parentWindow, Session pSession)
        {
            ShowNotifications(parentWindow, pSession, Guid.Empty);
        }

        /// <summary>
        /// When user wish to show notifications again.
        /// Please see #IN006001 for further details.
        /// </summary>
        /// <param name="parentWindow"></param>
        /// <param name="showNotificationOnDemand"></param>
        public static void ShowNotifications(Window parentWindow, bool showNotificationOnDemand)
        {
            ShowNotifications(parentWindow, XPOSettings.Session, Guid.Empty, showNotificationOnDemand);
        }

        /// <summary>
        /// Shows notifications created by "void FrameworkUtils.SystemNotification(Session pSession)" method.
        /// More information about changes on this, please see #IN006001.
        /// </summary>
        /// <param name="parentWindow"></param>
        /// <param name="pSession"></param>
        /// <param name="pLoggedUser"></param>
        /// <param name="showNotificationOnDemand"></param>
        public static void ShowNotifications(Window parentWindow, Session pSession, Guid pLoggedUser, bool showNotificationOnDemand = false)
        {
            _logger.Debug("void Utils.ShowNotifications(Window parentWindow, Session pSession, Guid pLoggedUser) :: Source Window: " + parentWindow.Name);
            string extraFilter = (pLoggedUser != Guid.Empty) ? string.Format("AND (UserTarget = '{0}' OR UserTarget IS NULL) ", pLoggedUser) : string.Empty;

            /* IN006001 */
            try
            {
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("(TerminalTarget = '{0}' OR TerminalTarget IS NULL){1}", TerminalSettings.LoggedTerminal.Oid, extraFilter));

                /* IN006001 - for "on demand" notification flow */
                if (showNotificationOnDemand)
                {
                    /* IN006001 - get date for filtering notifications that were created 'n' days before Today */
                    //Get Date Back DaysBackToFilter (Without WeekEnds and Holidays)
                    DateTime dateFilter = XPOUtility.GetDateTimeBackUtilDays(
                        XPOUtility.CurrentDateTimeAtomicMidnight(),
                        NotificationSettings.XpoOidSystemNotificationDaysBackWhenFiltering,
                        true);
                    criteriaOperator = CriteriaOperator.And(criteriaOperator, CriteriaOperator.Parse(string.Format("[CreatedAt] > '{0} 23:59:59'", dateFilter.ToString(CultureSettings.DateFormat))));

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
                    criteriaJoin = CriteriaOperator.And(criteriaJoin, new InOperator("Oid", new Guid[]{
                            NotificationSettings.XpoOidSystemNotificationTypeCurrentAccountDocumentsToInvoice,
                            NotificationSettings.XpoOidSystemNotificationTypeConsignationInvoiceDocumentsToInvoice,
                            NotificationSettings.XpoOidSystemNotificationTypeSaftDocumentTypeMovementOfGoods
                        }));
                    joinOperand.Condition = criteriaJoin;

                    criteriaOperator = CriteriaOperator.And(criteriaOperator, joinOperand);
                }
                else
                {/* keep the criteria for original flow */
                    criteriaOperator = CriteriaOperator.And(criteriaOperator, CriteriaOperator.Parse("[Readed] = 0"));
                }

                SortProperty[] sortProperty = new SortProperty[2];
                sortProperty[0] = new SortProperty("CreatedAt", SortingDirection.Ascending);
                sortProperty[1] = new SortProperty("Ord", SortingDirection.Ascending);
                XPCollection xpcSystemNotification = new XPCollection(pSession, typeof(sys_systemnotification), criteriaOperator, sortProperty);


                string message;
                if (xpcSystemNotification.Count > 0)
                {
                    foreach (sys_systemnotification item in xpcSystemNotification)
                    {
                        message = string.Format("{1}{0}{0}{2}", Environment.NewLine, item.CreatedAt, item.Message);
                        ResponseType response = ShowMessageBox(
                          parentWindow,
                          DialogFlags.DestroyWithParent | DialogFlags.Modal,
                          new Size(700, 480),
                          MessageType.Info,
                          ButtonsType.Ok,
                          GeneralUtils.GetResourceByName("window_title_dialog_notification"),
                          message
                        );

                        //Always OK
                        if (response == ResponseType.Ok)
                        {
                            item.DateRead = XPOUtility.CurrentDateTimeAtomic();
                            item.Readed = true;
                            item.UserLastRead = XPOSettings.LoggedUser;
                            item.TerminalLastRead = TerminalSettings.LoggedTerminal;
                            item.Save();
                        }
                    }
                }
                else if (showNotificationOnDemand)
                {/* IN006001 - when "on demand" request returns no results */
                    message = string.Format(GeneralUtils.GetResourceByName("dialog_message_no_notification"), NotificationSettings.XpoOidSystemNotificationDaysBackWhenFiltering);
                    ResponseType response = ShowMessageBox(
                      parentWindow,
                      DialogFlags.DestroyWithParent | DialogFlags.Modal,
                      new Size(700, 480),
                      MessageType.Info,
                      ButtonsType.Ok,
                      GeneralUtils.GetResourceByName("window_title_dialog_notification"),
                      message
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.Error("void Utils.ShowNotifications(Window parentWindow, Session pSession, Guid pLoggedUser) :: " + ex.Message, ex);
                ShowMessageBox(null, DialogFlags.Modal, new Size(600, 300), MessageType.Error, ButtonsType.Close, GeneralUtils.GetResourceByName("global_error"), "There is an error when checking for notifications. Please contact the helpdesk");
            }
        }

        public static void ShowChangeLog(Window parentWindow)
        {
            try
            {
                string message = "";


                WebClient wc = new WebClient();
                byte[] raw = wc.DownloadData("http://box.logicpulse.com/files/changelogs/pos.txt");

                message = System.Text.Encoding.UTF8.GetString(raw);

                System.Text.Encoding iso = System.Text.Encoding.GetEncoding("ISO-8859-1");
                System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
                byte[] isoBytes = System.Text.Encoding.Convert(utf8, iso, raw);
                message = iso.GetString(isoBytes);

                ResponseType response = ShowMessageBox(
                         parentWindow,
                         DialogFlags.DestroyWithParent | DialogFlags.Modal,
                         new Size(700, 480),
                         MessageType.Info,
                         ButtonsType.Ok,
                         GeneralUtils.GetResourceByName("change_log"),
                         message
                       );
            }
            catch (Exception ex)
            {
                _logger.Error("void Utils.ShowNotifications(Window parentWindow, Session pSession, Guid pLoggedUser) :: " + ex.Message, ex);
                ShowMessageBox(null, DialogFlags.Modal, new Size(600, 300), MessageType.Error, ButtonsType.Close, GeneralUtils.GetResourceByName("global_error"), "There is an error when checking for changelog. Please contact the helpdesk");
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Virtual KeyBoard

        public static string GetVirtualKeyBoardInput(Window parentWindow, KeyboardMode pMode, string pInitialValue, string pRegExRule)
        {
            bool useBaseDialogWindowMask = AppSettings.Instance.useBaseDialogWindowMask;

            //if (GlobalApp.DialogPosKeyboard == null)
            //{
            //Chama teclado certo na janela de artigos
            switch (pMode)
            {
                case KeyboardMode.Alfa:
                case KeyboardMode.AlfaNumeric:
                    //On Create SourceWindow is always GlobalApp.WindowPos else if its a Dialog, when it is destroyed, in Memory Keyboard is Destroyed too, this way we keep it in Memory
                    GlobalApp.DialogPosKeyboard = new PosKeyboardDialog(GlobalApp.PosMainWindow, DialogFlags.DestroyWithParent, KeyboardMode.AlfaNumeric, pInitialValue, pRegExRule);
                    break;

                case KeyboardMode.Numeric:
                    GlobalApp.DialogPosKeyboard = new PosKeyboardDialog(GlobalApp.PosMainWindow, DialogFlags.DestroyWithParent, KeyboardMode.Numeric, pInitialValue, pRegExRule);
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
                GlobalApp.DialogPosKeyboard.TransientFor = GlobalApp.DialogPosKeyboard.WindowSettings.Mask;
                GlobalApp.DialogPosKeyboard.WindowSettings.Mask.TransientFor = parentWindow;
                GlobalApp.DialogPosKeyboard.WindowSettings.Mask.Show();
            }
            else
            {
                //Now we can change its TransientFor
                GlobalApp.DialogPosKeyboard.TransientFor = parentWindow;
            }
            //}

            //Always Start Validated, else Only Construct start Validated
            GlobalApp.DialogPosKeyboard.TextEntry.Validate();
            //Put Cursor in End
            GlobalApp.DialogPosKeyboard.TextEntry.Position = GlobalApp.DialogPosKeyboard.TextEntry.Text.Length;
            GlobalApp.DialogPosKeyboard.TextEntry.GrabFocus();
            int response = GlobalApp.DialogPosKeyboard.Run();
            string result;
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
            //else Keyboard is destroyed when TransientFor Windows/Dialog is Destroyed ex when parentWindow = PosInputTextDialog
            GlobalApp.DialogPosKeyboard.TransientFor = GlobalApp.PosMainWindow;

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Cache

        internal static bool UseCache()
        {
            bool result = false;

            try
            {
                result = Convert.ToBoolean(GeneralSettings.PreferenceParameters["USE_CACHED_IMAGES"]);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return (result);
        }

        internal static bool UseVatAutocomplete()
        {
            bool result = false;

            try
            {
                result = Convert.ToBoolean(GeneralSettings.PreferenceParameters["USE_EUROPEAN_VAT_AUTOCOMPLETE"]);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return (result);
        }

        internal static bool UsePosPDFViewer()
        {
            bool result = false;

            try
            {
                result = Convert.ToBoolean(GeneralSettings.PreferenceParameters["USE_POS_PDF_VIEWER"]);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return (result);
        }

        internal static bool PrintTicket()
        {
            bool result;
            try
            {
                result = Convert.ToBoolean(GeneralSettings.PreferenceParameters["TICKET_PRINT_TICKET"]);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return true;
            }

            return (result);
        }

        //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
        //Criação de variável nas configurações para imprimir ou não QRCode
        internal static bool PrintQRCode()
        {
            bool result;
            try
            {
                string query = string.Format("SELECT Value FROM cfg_configurationpreferenceparameter WHERE Token = 'PRINT_QRCODE' AND (Disabled = 0 OR Disabled is NULL);");
                result = Convert.ToBoolean(XPOSettings.Session.ExecuteScalar(query));
                PrintingSettings.PrintQRCode = result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                result = Convert.ToBoolean(GeneralSettings.PreferenceParameters["PRINT_QRCODE"]);
                PrintingSettings.PrintQRCode = result;
                return true;
            }

            return (result);
        }

        internal static bool CheckStocks()
        {
            bool result;
            try
            {
                string query = string.Format("SELECT Value FROM cfg_configurationpreferenceparameter WHERE Token = 'CHECK_STOCKS' AND (Disabled = 0 OR Disabled is NULL);");
                result = Convert.ToBoolean(XPOSettings.Session.ExecuteScalar(query));
                GeneralSettings.CheckStocks = result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                result = Convert.ToBoolean(GeneralSettings.PreferenceParameters["CHECK_STOCKS"]);
                GeneralSettings.CheckStocks = result;
                return true;
            }

            return (result);
        }

        internal static bool CheckStockMessage()
        {
            bool result;
            try
            {
                string query = string.Format("SELECT Value FROM cfg_configurationpreferenceparameter WHERE Token = 'CHECK_STOCKS_MESSAGE';");
                result = Convert.ToBoolean(XPOSettings.Session.ExecuteScalar(query));
                GeneralSettings.CheckStockMessage = result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                result = Convert.ToBoolean(GeneralSettings.PreferenceParameters["CHECK_STOCKS_MESSAGE"]);
                GeneralSettings.CheckStockMessage = result;
                return true;
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
        //    _logger.Error(ex.Message, ex);
        //  }
        //}

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //GenericTreeView

        //Helper Static Method to Return a GenericTreeView from T
        [Obsolete]
        public static T GetGenericTreeViewXPO<T>(Window parentWindow,
                                                 GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default,
                                                 GridViewMode pGenericTreeViewMode = GridViewMode.Default)
          where T : IGridView, new()
        {
            return GetGenericTreeViewXPO<T>(parentWindow, null, navigatorMode, pGenericTreeViewMode);
        }

        [Obsolete]
        public static T GetGenericTreeViewXPO<T>(Window parentWindow, CriteriaOperator pCriteria, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default, GridViewMode pGenericTreeViewMode = GridViewMode.Default)
          where T : IGridView, new()
        {
            T genericTreeView = default;

            try
            {
                // Add default Criteria to Hide Undefined Records
                string undefinedFilter = string.Format("Oid <> '{0}'", XPOSettings.XpoOidUndefinedRecord);

                if (pCriteria == null)
                {
                    pCriteria = CriteriaOperator.Parse(undefinedFilter);
                }
                else
                {
                    pCriteria = CriteriaOperator.Parse(string.Format("{0} AND ({1})", pCriteria, undefinedFilter));
                }

                object[] constructor = new object[]
                {
                      parentWindow,
                      null,         //Default Value
                      pCriteria,    //Criteria
                      null,         //DialogType
                      pGenericTreeViewMode,
                      navigatorMode
                };
                genericTreeView = (T)Activator.CreateInstance(typeof(T), constructor);
                //Cast to Box to use ShowAll for all T(ypes) of GenericTreeView
                (genericTreeView as Box).ShowAll();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return genericTreeView;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Network


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
                    IniFileParser iNIFile = new IniFileParser(pFileName);

                    //Load
                    LicenseSettings.LicenseHardwareId = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "HardwareId", "Empresa Demonstração"), true);
                    LicenseSettings.LicenseCompany = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Company", "NIF Demonstração"), true);
                    LicenseSettings.LicenseNif = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Nif", "Morada Demonstração"), true);
                    LicenseSettings.LicenseAddress = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Address", "mail@demonstracao.tld"), true);
                    LicenseSettings.LicenseEmail = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Email", string.Empty), true);
                    LicenseSettings.LicenseTelephone = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Telephone", "Telefone Demonstração"), true);
                    LicenseSettings.LicenseReseller = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Reseller", "LogicPulse"), true);
                    //Test
                    if (pDebug)
                    {
                        _logger.Debug(string.Format("{0}:{1}", "HardwareId", LicenseSettings.LicenseHardwareId));
                        _logger.Debug(string.Format("{0}:{1}", "Company", LicenseSettings.LicenseCompany));
                        _logger.Debug(string.Format("{0}:{1}", "Nif", LicenseSettings.LicenseNif));
                        _logger.Debug(string.Format("{0}:{1}", "Address", LicenseSettings.LicenseAddress));
                        _logger.Debug(string.Format("{0}:{1}", "Email", LicenseSettings.LicenseEmail));
                        _logger.Debug(string.Format("{0}:{1}", "Telephone", LicenseSettings.LicenseTelephone));
                        _logger.Debug(string.Format("{0}:{1}", "Reseller", LicenseSettings.LicenseReseller));
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
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        public static object SaveOrUpdateCustomer(Window parentWindow, erp_customer pCustomer, string pName, string pAddress, string pLocality, string pZipCode, string pCity, string pPhone, string pEmail, cfg_configurationcountry pCountry, string pFiscalNumber, string pCardNumber, decimal pDiscount, string pNotes)
        {
            bool customerExists = false;
            erp_customer result;
            erp_customer finalConsumerEntity = XPOUtility.GetEntityById<erp_customer>(InvoiceSettings.FinalConsumerId);
            fin_configurationpricetype configurationPriceType = XPOUtility.GetEntityById<fin_configurationpricetype>(XPOSettings.XpoOidConfigurationPriceTypeDefault);

            SortingCollection sortCollection = new SortingCollection
            {
                new SortProperty("Oid", SortingDirection.Ascending)
            };
            CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled IS NULL)"));
            ICollection collectionCustomers = XPOSettings.Session.GetObjects(XPOSettings.Session.GetClassInfo(typeof(erp_customer)), criteria, sortCollection, int.MaxValue, false, true);

            foreach (erp_customer item in collectionCustomers)
            {
                //Front-end - Gravação de múltiplos clientes sem nome definido [IN:014367]
                if (item.FiscalNumber == pFiscalNumber) customerExists = true;
                if (item.Oid.Equals(finalConsumerEntity)) customerExists = true;
            }

            bool changed;
            //insert new Customer before Process Finance Document
            //se for consumidor final, nao altera, cria novo registo
            if (!customerExists)
            {
                changed = true;
                //Front-end - Gravação de múltiplos clientes sem nome definido [IN:014367]
                if (string.IsNullOrEmpty(pName)) pName = GeneralUtils.GetResourceByName("saft_value_unknown");
                result = new erp_customer(XPOSettings.Session)
                {
                    Ord = (pFiscalNumber != string.Empty) ? XPOUtility.GetNextTableFieldID("erp_customer", "Ord") : 0,
                    Code = (pFiscalNumber != string.Empty) ? XPOUtility.GetNextTableFieldID("erp_customer", "Code") : 0,
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
                    Hidden = pFiscalNumber == string.Empty
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
                result = XPOUtility.GetEntityById<erp_customer>(pCustomer.Oid);

                if (result != finalConsumerEntity)
                {
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

                    //If final Consumer not save
                    if (changed && result.Oid != finalConsumerEntity.Oid)
                    {
                        ResponseType responseType = ShowMessageTouch(parentWindow, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, GeneralUtils.GetResourceByName("global_record_modified"), GeneralUtils.GetResourceByName("dialog_message_customer_updated_save_changes"));
                        if (responseType == ResponseType.No)
                        {
                            changed = false;
                            //Require to Revert Changes from XPO Session Memory
                            result.Reload();
                        }
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
                //_logger.Error(ex.Message, ex);
                result.Reload();
                return ex;
            }
        }

        //Method to Check Equallity from Db Fields to Input Fields, Required to compare null with "" etc
        public static bool CheckIfFieldChanged(object pFieldDb, object pFieldInput)
        {
            object fieldDb = (pFieldDb == null || pFieldDb.ToString() == string.Empty) ? string.Empty : pFieldDb;

            bool result;
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
        //SessionApp

        public static string GetSessionFileName()
        {
            string result = Path.Combine(
                PathsSettings.TempFolderLocation,
                GeneralSettings.POSSessionJsonFileName);

            return result;
        }



        //TK016235 BackOffice - Mode
        public static void StartReportsMenuFromBackOffice(Window parentWindow)
        {
            try
            {
                PosReportsDialog dialog = new PosReportsDialog(parentWindow, DialogFlags.DestroyWithParent);
                int response = dialog.Run();
                dialog.Destroy();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //TK016235 BackOffice - Mode
        public static void StartNewDocumentFromBackOffice(Window parentWindow)
        {
            try
            {
                //Call New DocumentFinance Dialog
                PosDocumentFinanceDialog dialogNewDocument = new PosDocumentFinanceDialog(parentWindow, DialogFlags.DestroyWithParent);
                ResponseType responseNewDocument = (ResponseType)dialogNewDocument.Run();
                if (responseNewDocument == ResponseType.Ok)
                {
                }
                dialogNewDocument.Destroy();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }


        //TK016235 BackOffice - Mode
        public static void OpenArticleStockDialog(Window parentWindow)
        {

            if (LicenseSettings.LicenseModuleStocks && ModulesSettings.StockManagementModule != null)
            {
                DialogArticleStock dialog = new DialogArticleStock(parentWindow);
                ResponseType response = (ResponseType)dialog.Run();
                dialog.Destroy();
            }
            else if (CheckStockMessage() && !LicenseSettings.LicenseModuleStocks)
            {
                var messageDialog = ShowMessageTouch(parentWindow, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.OkCancel, GeneralUtils.GetResourceByName("global_warning"), GeneralUtils.GetResourceByName("global_warning_acquire_module_stocks"));
                if (messageDialog == ResponseType.Ok)
                {
                    Process.Start("https://logic-pos.com/");
                }

                string query = string.Format("UPDATE cfg_configurationpreferenceparameter SET Value = 'False' WHERE Token = 'CHECK_STOCKS_MESSAGE';");
                XPOSettings.Session.ExecuteScalar(query);
                query = string.Format("UPDATE cfg_configurationpreferenceparameter SET Disabled = '1' WHERE Token = 'CHECK_STOCKS_MESSAGE';");
                XPOSettings.Session.ExecuteScalar(query);

                var documentsMenu = new DocumentsMenuModal(parentWindow);
                documentsMenu.Run();
                documentsMenu.Destroy();

            }
            else
            {
                var addStockModal = new AddStockModal(parentWindow);
                addStockModal.Run();
                addStockModal.Destroy();
            }

        }


        public static bool IsPortOpen(string portName)
        {
            var port = new SerialPort(portName);
            try
            {
                port.Open();
                port.Close();
                return true;
            }
            catch (Exception)
            {
                _logger.Debug(string.Format("Port already in use: [{0}]", portName));
                return false;
            }
        }

        //Generate random String
        public static string RandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}