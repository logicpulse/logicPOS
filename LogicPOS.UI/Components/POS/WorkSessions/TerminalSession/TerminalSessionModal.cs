using Gtk;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.POS
{
    public partial class TerminalSessionModal : Modal
    {
        #region Components
        private IconButtonWithText BtnOk { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private IconButtonWithText BtnPrint { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Print);
        private IconButtonWithText BtnOpening { get; set; }
        private IconButtonWithText BtnClosing { get; set; }
        private IconButtonWithText BtnIn { get; set; }
        private IconButtonWithText BtnOut { get; set; }
        private List<IconButtonWithText> _movementTypeButtons { get; set; } = new List<IconButtonWithText>();
        private TextBox TxtAmount { get; set; }
        private TextBox TxtDescription { get; set; }
        private readonly decimal _totalCashInrawer = WorkSessionsService.GetTotalCashInCashDrawer();
        #endregion

        private bool _cashDrawerIsOpen = WorkSessionsService.TerminalIsOpen();
        public WorkSessionMovementType MovementType { get; private set; } = WorkSessionMovementType.CashDrawerOpen;

        public TerminalSessionModal(Window parentWindow)
            : base(parentWindow,
                   string.Format(LocalizedString.Instance["window_title_dialog_cashdrawer"],
                                 WorkSessionsService.GetTotalCashInCashDrawer().ToString("C")),
                   new Size(462, 310),
                   AppSettings.Paths.Images + @"Icons\Windows\icon_window_cash_drawer.png")
        {
            UpdateBtnPrint();
        }

        private void InitializeTextBoxes()
        {
            TxtAmount = new TextBox(this,
                                        LocalizedString.Instance["global_money"],
                                        true,
                                        true,
                                        RegularExpressions.Money,
                                        false,
                                        true);

            TxtAmount.Text = _totalCashInrawer.ToString("0.00");

            TxtDescription = new TextBox(sourceWindow: this,
                                             labelText: LocalizedString.Instance["global_description"],
                                             includeKeyBoardButton: true,
                                             includeSelectButton: false);

            UpdateValidatableFields();
        }

        private void InitialzeButtons()
        {
            MovementType = _cashDrawerIsOpen ? WorkSessionMovementType.CashDrawerClose : WorkSessionMovementType.CashDrawerOpen;

            BtnOpening = CreateOpeningButton();
            BtnClosing = CreateClosingButton();
            BtnIn = CreateInButton();
            BtnOut = CreateOutButton();

            _movementTypeButtons.Add(BtnOpening);
            _movementTypeButtons.Add(BtnClosing);
            _movementTypeButtons.Add(BtnIn);
            _movementTypeButtons.Add(BtnOut);

            AddEventHandlers();
            UpdateButtonsColors();
            UpdateButtonsSensitivity();
        }

        private IconButtonWithText CreateOpeningButton()
        {
            return CreateMovementTypeButton(LocalizedString.Instance["pos_button_label_cashdrawer_open"],
                                            AppSettings.Paths.Images + "Icons\\icon_pos_cashdrawer_open.png");
        }

        private IconButtonWithText CreateClosingButton()
        {
            return CreateMovementTypeButton(LocalizedString.Instance["pos_button_label_cashdrawer_close"],
                                            AppSettings.Paths.Images + "Icons\\icon_pos_cashdrawer_close.png");
        }

        private IconButtonWithText CreateInButton()
        {
            return CreateMovementTypeButton(LocalizedString.Instance["pos_button_label_cashdrawer_in"],
                                            AppSettings.Paths.Images + "Icons\\icon_pos_cashdrawer_in.png");
        }

        private IconButtonWithText CreateOutButton()
        {
            return CreateMovementTypeButton(LocalizedString.Instance["pos_button_label_cashdrawer_out"],
                                            AppSettings.Paths.Images + "Icons\\icon_pos_cashdrawer_out.png");
        }

        private IconButtonWithText CreateMovementTypeButton(string text, string icon)
        {
            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButton_Green",
                    Text = text,
                    Font = AppSettings.Instance.FontBaseDialogButton,
                    FontColor = AppSettings.Instance.ColorBaseDialogDefaultButtonFont,
                    Icon = icon,
                    IconSize = AppSettings.Instance.SizeBaseDialogDefaultButtonIcon,
                    ButtonSize = AppSettings.Instance.SizeBaseDialogDefaultButton
                });

        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            InitialzeButtons();

            return new ActionAreaButtons
                {
                    new ActionAreaButton(BtnPrint, ResponseType.None),
                    new ActionAreaButton(BtnOk, ResponseType.Ok),
                    new ActionAreaButton(BtnCancel, ResponseType.Cancel)
                };
        }

        protected override Widget CreateBody()
        {
            HBox topButtonsBox = new HBox(true, 5);
            topButtonsBox.PackStart(BtnOpening, true, true, 0);
            topButtonsBox.PackStart(BtnClosing, true, true, 0);
            topButtonsBox.PackStart(BtnIn, true, true, 0);
            topButtonsBox.PackStart(BtnOut, true, true, 0);

            InitializeTextBoxes();

            VBox vbox = new VBox(false, 0);
            vbox.PackStart(topButtonsBox, false, false, 0);
            vbox.PackStart(TxtAmount.Component, false, false, 0);
            vbox.PackStart(TxtDescription.Component, false, false, 0);

            Fixed fixedContent = new Fixed();
            fixedContent.Put(vbox, 0, 0);

            return fixedContent;
        }
    }
}
