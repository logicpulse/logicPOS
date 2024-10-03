using Gtk;
using logicpos.App;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Documents
{
    public class DocumentsModal : Modal
    {
        private DocumentsPage Page { get; set; } 
        private IconButtonWithText BtnPrintDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Print, "btnPrintDocument");
        private IconButtonWithText BtnOpenDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument, "btnOpenDocument");
        private IconButtonWithText BtnClose { get; set; } =  ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
        private IconButtonWithText BtnPrintDocumentAs { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.PrintAs, "btnPrintDocumentAs");
        private IconButtonWithText BtnSendDocumentEmail { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.SendEmailDocument, "btnSendDocumentEmail");
        private IconButtonWithText BtnCancelDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("btnCancelDocument",
                                                                                         GeneralUtils.GetResourceByName("global_button_label_cancel_document"),
                                                                                         PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png");

        public DocumentsModal(Window parent) : base(parent,
                                                    GeneralUtils.GetResourceByName("window_title_select_finance_document"),
                                                    GlobalApp.MaxWindowSize,
                                                    $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_select_record.png"}")
        {

        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            ColorButtons();

            AddButtonsEventHandlers();

            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(BtnPrintDocument, ResponseType.Ok),
                new ActionAreaButton(BtnPrintDocumentAs, ResponseType.Ok),
                new ActionAreaButton(BtnCancelDocument, ResponseType.Ok),
                new ActionAreaButton(BtnOpenDocument, ResponseType.Ok),
                new ActionAreaButton(BtnSendDocumentEmail, ResponseType.Ok),
                new ActionAreaButton(BtnClose, ResponseType.Close),
            };
            return actionAreaButtons;
        }

        private void ColorButtons()
        {
            var greenColor = Color.FromArgb(140, 187, 59);
            BtnPrintDocument.SetBackgroundColor(greenColor);
            BtnPrintDocumentAs.SetBackgroundColor(greenColor);
            BtnOpenDocument.SetBackgroundColor(greenColor);
            BtnSendDocumentEmail.SetBackgroundColor(greenColor);
            BtnCancelDocument.SetBackgroundColor(greenColor);
        }

        private void AddButtonsEventHandlers()
        {
            BtnOpenDocument.Clicked += BtnOpenDocument_Clicked;
            BtnPrintDocumentAs.Clicked += BtnPrintDocumentAs_Clicked;
        }

        private void BtnPrintDocumentAs_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity != null)
            {
                var pdfLocation = DocumentPrintingUtils.GetPdfFile(Page.SelectedEntity.Id);

                if (pdfLocation == null)
                {
                    return;
                }

                DocumentPrintingUtils.PrintWithNativeDialog(pdfLocation);
            }
        }

        private void BtnOpenDocument_Clicked(object sender, EventArgs e)
        {
            if(Page.SelectedEntity != null)
            {
                DocumentPrintingUtils.ShowPdf(this,Page.SelectedEntity.Id);
            }
        }

        protected override void OnResponse(ResponseType response)
        {
            if(response != ResponseType.Close)
            {
                Run();
            }

            base.OnResponse(response);
        }


        protected override Widget CreateBody()
        {
            var page = new DocumentsPage(this, PageOptions.SelectionPageOptions);
            page.SetSizeRequest(WindowSettings.Size.Width - 14, WindowSettings.Size.Height - 124);
            Fixed fixedContent = new Fixed();
            fixedContent.Put(page, 0, 0);
            Page = page;
            return fixedContent;
        }

        protected override Widget CreateLeftContent()
        {
            return CreateSearchBox();
        }

        private Widget CreateSearchBox()
        {
            var searchBox = new PageSearchBox(Page.SourceWindow, true);
            searchBox.TxtSearch.EntryValidation.Changed += delegate
            {
                Page.Navigator.SearchBox.TxtSearch.EntryValidation.Text = searchBox.TxtSearch.EntryValidation.Text;
            };

            return searchBox;
        }
    }
}
