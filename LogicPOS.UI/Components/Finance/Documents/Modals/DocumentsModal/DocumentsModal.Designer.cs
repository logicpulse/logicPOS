using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentsModal
    {
        private IconButtonWithText CreateButton(string name,
                                              string label,
                                              string icon)
        {
            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = name,
                    Text = label,
                    Font = AppSettings.Instance.FontBaseDialogActionAreaButton,
                    FontColor = AppSettings.Instance.ColorBaseDialogActionAreaButtonFont,
                    Icon = AppSettings.Paths.Images + icon,
                    IconSize = AppSettings.Instance.SizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon,
                    ButtonSize = AppSettings.Instance.SizeBaseDialogActionAreaBackOfficeNavigatorButton
                });
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            InitializeButtons();
            AddButtonsEventHandlers();

            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();

            if (SystemInformationService.SystemInformation.IsAngola && Licensing.LicensingService.Data.ModuleAgtFe)
            {
                actionAreaButtons.Add(new ActionAreaButton(BtnSendDocumentToAgt, ResponseType.Ok));
                actionAreaButtons.Add(new ActionAreaButton(BtnUpdateAgtValidationStatus, ResponseType.Ok));
                actionAreaButtons.Add(new ActionAreaButton(BtnViewAgtDocument, ResponseType.Ok));
            }
            else
            {
                actionAreaButtons.Add(new ActionAreaButton(BtnCloneDocument, ResponseType.Ok));
            }

            actionAreaButtons.Add(new ActionAreaButton(BtnNewDocument, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(BtnEditDraft, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(BtnDeleteDraft, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(BtnCancelDocument, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(BtnPayInvoice, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(BtnPrintDocument, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(BtnPrintDocumentAs, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(BtnOpenDocument, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(BtnSendDocumentEmail, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(BtnClose, ResponseType.Close));

            actionAreaButtons.Add(new ActionAreaButton(BtnOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(BtnCancel, ResponseType.Cancel));

            return actionAreaButtons;
        }

        protected override Widget CreateLeftContent()
        {
            HBox box = new HBox(false, 0);

            TxtSearch = new TextBox(this,
                                        LocalizedString.Instance["widget_generictreeviewsearch_search_label"],
                                        isRequired: false,
                                        isValidatable: false,
                                        includeKeyBoardButton: true,
                                        includeSelectButton: false);

            TxtSearch.Component.WidthRequest = AppSettings.Instance.AppScreenSize.Width == 800 && AppSettings.Instance.AppScreenSize.Height == 600 ? 150 : 250;

            box.PackStart(TxtSearch.Component, false, false, 0);
            box.PackStart(BtnFilter, false, false, 0);
            box.PackStart(BtnPrevious, false, false, 0);
            box.PackStart(BtnNext, false, false, 0);

            TxtSearch.Entry.Changed += delegate { Page.Navigator.SearchBox.TxtSearch.EntryValidation.Text = TxtSearch.Text; };

            return box;
        }

        protected override Widget CreateBody()
        {
            var pageOptions = PageOptions.SelectionPageOptions;

            if (_mode == Finance.Documents.Modals.DocumentsModal.DocumentsModalMode.UnpaidInvoices)
            {
                pageOptions = DocumentsPage.UpaidInvoicesOptions;
            }

            var page = new DocumentsPage(this, pageOptions);
            page.SetSizeRequest(WindowSettings.Size.Width - 14, WindowSettings.Size.Height - 124);
            Fixed fixedContent = new Fixed();
            fixedContent.Put(page, 0, 0);
            Page = page;
            UpdateModalTitle();
            AddPageEventHandlers();
            UpdateNavigationButtons();
            return fixedContent;
        }

        private void InitializeButtons()
        {

            BtnPrevious = CreateButton("touchButtonPrev_DialogActionArea",
                                       LocalizedString.Instance["widget_generictreeviewnavigator_record_prev"],
                                       @"Icons/icon_pos_nav_prev.png");

            BtnNext = CreateButton("touchButtonNext_DialogActionArea",
                                   LocalizedString.Instance["widget_generictreeviewnavigator_record_next"],
                                   @"Icons/icon_pos_nav_next.png");

            BtnFilter = CreateButton("touchButtonSearchAdvanced_DialogActionArea",
                                    LocalizedString.Instance["global_button_label_filter"],
                                    @"Icons\icon_pos_filter.png");
        }
    }
}
