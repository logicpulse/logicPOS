using Gtk;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReceiptsModal
    {
        #region Components
        private ReceiptsPage Page { get; set; }
        private string WindowTitleBase => GeneralUtils.GetResourceByName("window_title_dialog_document_finance_payment");

        private IconButtonWithText BtnPrintDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Print, "touchButton_Green");
        private IconButtonWithText BtnOpenDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument, "touchButton_Green");
        private IconButtonWithText BtnClose { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.Close);
        private IconButtonWithText BtnPrintDocumentAs { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.PrintAs, "touchButton_Green");
        private IconButtonWithText BtnSendDocumentEmail { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.SendEmailDocument, "touchButton_Green");
        private IconButtonWithText BtnCancelDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments("touchButton_Green",
                                                                                                                          GeneralUtils.GetResourceByName("global_button_label_cancel_document"),
                                                                                                                          PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png");

        private IconButtonWithText BtnPrevious { get; set; }
        private IconButtonWithText BtnNext { get; set; }
        private TextBox TxtSearch { get; set; }
        public IconButtonWithText BtnFilter { get; set; }
        #endregion

        protected override Widget CreateBody()
        {
            var page = new ReceiptsPage(this, PageOptions.SelectionPageOptions);
            page.SetSizeRequest(WindowSettings.Size.Width - 14, WindowSettings.Size.Height - 124);
            Fixed fixedContent = new Fixed();
            fixedContent.Put(page, 0, 0);
            Page = page;
            UpdateModalTitle();
            AddPageEventHandlers();
            return fixedContent;
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            InitializeButtons();
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

        protected override Widget CreateLeftContent()
        {
            HBox box = new HBox(false, 0);

            TxtSearch = new TextBox(this,
                                        LocalizedString.Instance["widget_generictreeviewsearch_search_label"],
                                        isRequired: false,
                                        isValidatable: false,
                                        includeKeyBoardButton: true,
                                        includeSelectButton: false);

            TxtSearch.Component.WidthRequest = LogicPOSAppContext.ScreenSize.Width == 800 && LogicPOSAppContext.ScreenSize.Height == 600 ? 150 : 250;

            box.PackStart(TxtSearch.Component, false, false, 0);
            box.PackStart(BtnFilter, false, false, 0);
            box.PackStart(BtnPrevious, false, false, 0);
            box.PackStart(BtnNext, false, false, 0);

            return box;

        }

        private void InitializeButtons()
        {
            IconButtonWithText CreateButton(string name,
                                            string label,
                                            string icon)
            {
                return new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = name,
                        Text = label,
                        Font = AppSettings.Instance.fontBaseDialogActionAreaButton,
                        FontColor = AppSettings.Instance.colorBaseDialogActionAreaButtonFont,
                        Icon = PathsSettings.ImagesFolderLocation + icon,
                        IconSize = AppSettings.Instance.sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon,
                        ButtonSize = AppSettings.Instance.sizeBaseDialogActionAreaBackOfficeNavigatorButton
                    });
            }

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
