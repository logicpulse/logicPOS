using ErrorOr;
using Gtk;
using LogicPOS.Api.Errors;
using LogicPOS.Globalization;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Drawing;
using System.Text;

namespace LogicPOS.UI.Alerts
{
    public static class CustomAlerts
    {
        public static CustomAlert Information(Window parent = null)
        {
            return new CustomAlert(parent)
                       .WithTitleResource("global_information")
                       .WithMessageType(MessageType.Info)
                       .WithButtonsType(ButtonsType.Ok);
        }

        public static CustomAlert Error(Window parent = null)
        {
            return new CustomAlert(parent)
                       .WithTitleResource("global_error")
                       .WithMessageType(MessageType.Error)
                       .WithButtonsType(ButtonsType.Ok);
        }

        public static CustomAlert Warning(Window parent = null)
        {
            return new CustomAlert(parent)
                       .WithTitleResource("global_warning")
                       .WithMessageType(MessageType.Warning)
                       .WithButtonsType(ButtonsType.Ok);
        }

        public static CustomAlert Question(Window parent = null)
        {
            return new CustomAlert(parent)
                       .WithMessageType(MessageType.Question)
                       .WithButtonsType(ButtonsType.YesNo);
        }

        public static void ShowCannotDeleteEntityErrorAlert(Window parentWindow)
        {
            Error(parentWindow)
                .WithTitleResource("window_title_dialog_delete_record")
                .WithMessageResource("dialog_message_delete_record_show_referenced_record_message")
                .ShowAlert();
        }

        public static ResponseType ShowDeleteConfirmationAlert(Window parent = null)
        {
            return Question(parent)
                .WithTitleResource("window_title_dialog_delete_record")
                .WithMessageResource("dialog_message_delete_record")
                .ShowAlert();
        }

        public static void ShowOperationSucceededAlert(Window parent = null)
        {
            Information(parent)
                .WithMessageResource("dialog_message_operation_successfully")
                .ShowAlert();
        }

        public static void ShowUnderConstructionAlert(Window parent = null)
        {
            Error(parent)
                .WithMessageResource("dialog_message_under_construction_function")
                .ShowAlert();
        }

        public static void ShowOperationSucceededAlert(string titleResource, Window parent = null)
        {
            Information(parent)
                .WithTitleResource(titleResource)
                .WithMessageResource("dialog_message_operation_successfully")
                .ShowAlert();
        }


        public static void ShowErrorPrintingTicketAlert(Window parent,
                                                        string printerName,
                                                        string printerNetworkName,
                                                        string message)
        {
            Error(parent).WithTitleResource("global_error")
                .WithMessageResource(string.Format(GeneralUtils.GetResourceByName("dialog_message_error_printing_ticket"), printerName, printerNetworkName, message))
                .WithSize(new System.Drawing.Size(800, 400))
                .ShowAlert();
        }

        public static void ShowRequiredValidPrinterAlert(Window parent)
        {
            Information(parent)
                .WithTitleResource("global_error")
                .WithMessageResource("dialog_message_required_valid_printer")
                .ShowAlert();
        }

        public static void ShowTerminalWithoutPrinterAlert(Window parent)
        {
            Question(parent)
                .WithTitleResource("global_information")
                .WithMessageResource("dialog_message_show_printer_undefined_on_print")
                .WithSize(new System.Drawing.Size(550, 400))
                .ShowAlert();
        }

        public static void ShowThemeRenderingErrorAlert(string message, Window parent = null)
        {
            string errorMessage = string.Format(LocalizedString.Instance[ResourceNames.APP_ERROR_RENDERING_THEME],
                                                AppSettings.ThemeFile,
                                                message);

            Error(parent).WithTitleResource("global_error")
                .WithMessage(errorMessage)
                .WithSize(new System.Drawing.Size(600, 500))
                .ShowAlert();

            Environment.Exit(0);
        }

        public static void ShowInvalidDocumentDateErrorAlert(Window parent)
        {
            Error(parent)
                .WithTitleResource("global_error")
                .WithMessageResource("dialog_message_systementry_is_less_than_last_finance_document_series")
                .ShowAlert();
        }

        public static void ShowSimplifiedInvoiceMaxValueExceedForFinalConsumerErrorAlert(decimal currentTotal,
                                                                                         decimal maxTotal,
                                                                                         Window parent = null)
        {
            var message = string.Format(
                    GeneralUtils.GetResourceByName("dialog_message_value_exceed_simplified_invoice_for_final_or_annonymous_consumer")
                    , $"{GeneralUtils.GetResourceByName("global_total")}: {currentTotal}"
                    , $"{GeneralUtils.GetResourceByName("global_maximum")}: {maxTotal}");

            Warning(parent)
                .WithTitleResource("global_warning")
                .WithMessage(message)
                .ShowAlert();

        }

        public static void ShowCannotDeleteProtectedEntityErrorAlert(Window parentWindow)
        {
            Error(parentWindow)
                .WithTitleResource("window_title_dialog_delete_record")
                .WithMessageResource("dialog_message_delete_record_show_protected_record")
                .ShowAlert();
        }

        public static void ShowCannotUpdateProtectedEntityErrorAlert(Window parentWindow)
        {
            Error(parentWindow)
                .WithTitleResource("window_title_dialog_update_record")
                .WithMessageResource("dialog_message_update_record_show_protected_record")
                .ShowAlert();
        }

        public static void ShowUnsupportedResolutionErrorAlert(Window parentWindow, int width, int height)
        {
            string message = string.Format(GeneralUtils.GetResourceByName("app_error_unsupported_resolution_detected"), width, height);

            Error(parentWindow)
                .WithTitleResource("global_error")
                .WithMessage(message)
                .ShowAlert();

            Environment.Exit(Environment.ExitCode);
        }

        public static void ShowCreditNoteExceedingDocumentArticleQuantitiesAlert(Window parentWindow,
                                                                                 decimal currentQuantity,
                                                                                 decimal maxPossibleQuantity)
        {
            string message = string.Format(GeneralUtils.GetResourceByName("dialog_message_error_try_to_issue_a_credit_note_exceeding_source_document_article_quantities"),
                                           currentQuantity,
                                           maxPossibleQuantity);

            Information(parentWindow)
                .WithTitleResource("global_information")
                .WithMessage(message)
                .ShowAlert();
        }

        public static bool ShowMinimumStockAlert(Window parentWindow,
                                                 Guid pArticleOid,
                                                 decimal pNewQuantity,
                                                 out bool showMessage)
        {
            showMessage = false;
            //Size size = new Size(500, 350);
            //fin_article article = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), pArticleOid);
            //decimal articleStock;

            //string stockQuery = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;", article.Oid);
            //articleStock = Convert.ToDecimal(XPOSettings.Session.ExecuteScalar(stockQuery));

            //string childStockMessage = Environment.NewLine + Environment.NewLine + "Stock de artigos associados: " + Environment.NewLine;
            ////Composite article Messages
            //int childStockAlertCount = 0;
            //if (article.IsComposed)
            //{
            //    foreach (fin_articlecomposition item in article.ArticleComposition)
            //    {
            //        fin_article child = item.ArticleChild;
            //        decimal childStock = 0;

            //        stockQuery = string.Format("SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = '{0}' AND (Disabled = 0 OR Disabled is NULL) GROUP BY Article;", item.ArticleChild.Oid);
            //        childStock = Convert.ToDecimal(XPOSettings.Session.ExecuteScalar(stockQuery));

            //        var childStockAfterChanged = childStock - (pNewQuantity * item.Quantity);
            //        if (childStockAfterChanged <= child.MinimumStock)
            //        {
            //            childStockMessage += Environment.NewLine + GeneralUtils.GetResourceByName("global_article") + ": " + child.Designation + Environment.NewLine + GeneralUtils.GetResourceByName("global_total_stock") + ": " + DataConversionUtils.DecimalToString(Convert.ToDecimal(childStock), "0.00") + Environment.NewLine + GeneralUtils.GetResourceByName("global_minimum_stock") + ": " + DataConversionUtils.DecimalToString(Convert.ToDecimal(child.MinimumStock), "0.00") + Environment.NewLine;
            //            childStockAlertCount++;
            //        }
            //    }
            //}
            //var stockQuantityAfterChanged = articleStock - pNewQuantity;
            ////Mensagem de stock apenas para artigos da classe Produtos
            //if ((stockQuantityAfterChanged <= article.MinimumStock || childStockAlertCount > 0) && article.Class.Oid == Guid.Parse("6924945d-f99e-476b-9c4d-78fb9e2b30a3"))
            //{
            //    if (article.IsComposed)
            //    {
            //        size = new Size(650, 480);

            //        var response = Question()
            //            .WithTitleResource("global_stock_movements")
            //            .WithMessage(GeneralUtils.GetResourceByName("window_check_stock_question") + Environment.NewLine + Environment.NewLine + GeneralUtils.GetResourceByName("global_article") + ": " + article.Designation + Environment.NewLine + GeneralUtils.GetResourceByName("global_total_stock") + ": " + DataConversionUtils.DecimalToString(Convert.ToDecimal(articleStock), "0.00") + Environment.NewLine + GeneralUtils.GetResourceByName("global_minimum_stock") + ": " + DataConversionUtils.DecimalToString(Convert.ToDecimal(article.MinimumStock), "0.00") + childStockMessage)
            //            .WithSize(size)
            //            .ShowAlert();

            //        if (response == ResponseType.Yes)
            //        {
            //            showMessage = true;
            //            return true;
            //        }
            //        else
            //        {
            //            showMessage = true;
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        var response = Question()
            //            .WithSize(size)
            //            .WithTitle(GeneralUtils.GetResourceByName("global_stock_movements"))
            //            .WithMessage(GeneralUtils.GetResourceByName("window_check_stock_question") + Environment.NewLine + Environment.NewLine + GeneralUtils.GetResourceByName("global_article") + ": " + article.Designation + Environment.NewLine + GeneralUtils.GetResourceByName("global_total_stock") + ": " + DataConversionUtils.DecimalToString(Convert.ToDecimal(articleStock), "0.00") + Environment.NewLine + GeneralUtils.GetResourceByName("global_minimum_stock") + ": " + DataConversionUtils.DecimalToString(Convert.ToDecimal(article.MinimumStock), "0.00"))
            //            .ShowAlert();

            //        if (response == ResponseType.Yes)
            //        {
            //            showMessage = true;
            //            return true;
            //        }
            //        else
            //        {
            //            showMessage = true;
            //            return false;
            //        }
            //    }
            //}
            //showMessage = false;
            return false;
        }


        public static void ShowContactSupportErrorAlert(Window parentWindow, string additionalInformation = "")
        {
            Error(parentWindow)
                .WithSize(new Size(500, 240))
                .WithTitleResource("global_error")
                .WithMessage($"{GeneralUtils.GetResourceByName("app_error_contact_support")}\n\n{additionalInformation}")
                .ShowAlert();
        }

        public static void ShowApiErrorAlert(Window sourceWindow,
                                             Error error)
        {
            var errorMessage = new StringBuilder();
            errorMessage.AppendLine("Code: " + error.Code);
            errorMessage.AppendLine("Description: " + error.Description);


            var metadata = error.Metadata;

            if (metadata != null)
            {
                var problemDetails = (ProblemDetails)metadata["problem"];

                errorMessage.AppendLine("\nProblem Details:");
                errorMessage.AppendLine("Title: " + problemDetails.Title);
                errorMessage.AppendLine($"Detail: {problemDetails.Detail}");
                errorMessage.AppendLine("Status: " + problemDetails.Status);
                errorMessage.AppendLine("Type: " + problemDetails.Type);
                errorMessage.AppendLine("TraceId: " + problemDetails.TraceId);
                errorMessage.AppendLine("Instance: " + problemDetails.Instance);

                if (problemDetails.Errors != null)
                {
                    foreach (var problemDetailsError in problemDetails.Errors)
                    {
                        errorMessage.AppendLine("\nError:");
                        errorMessage.AppendLine("Name: " + problemDetailsError.Name);
                        errorMessage.AppendLine("Reson: " + problemDetailsError.Reason);
                    }
                }
            }

            var messageDialog = new CustomAlert(sourceWindow)
                                .WithMessage(errorMessage.ToString())
                                .WithMessageType(MessageType.Error)
                                .WithButtonsType(ButtonsType.Ok)
                                .WithTitle("Erro")

                                .ShowAlert();
        }


        public static bool ShowQuitConfirmationAlert(Window parentWindow)
        {
            ResponseType responseType = new CustomAlert(parentWindow)
                                            .WithMessageResource("global_quit_message")
                                            .WithSize(new Size(400, 300))
                                            .WithMessageType(MessageType.Question)
                                            .WithButtonsType(ButtonsType.YesNo)
                                            .WithTitleResource("global_quit_title")
                                            .ShowAlert();

            return responseType == ResponseType.Yes;
        }
    }
}
