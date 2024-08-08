using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Settings;
using LogicPOS.Shared.Article;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosSplitPaymentsDialog : BaseDialog
    {
        private readonly ArticleBag _articleBag;
        private readonly TicketList _ticketList;

        private readonly IconButtonWithText _buttonOk;
        private readonly IconButtonWithText _buttonCancel;
        private readonly IconButtonWithText _buttonTableRemoveSplit;
        private readonly IconButtonWithText _buttonTableAddSplit;
        private readonly ResponseType _responseTypeRemoveSplit = (ResponseType)11;
        private readonly ResponseType _responseTypeAddSplit = (ResponseType)10;
        // Strore Total per Split
        private decimal _totalPerSplit;
        // UI
        private readonly VBox _vbox;
        private readonly List<SplitPaymentButton> _splitPaymentButtons = new List<SplitPaymentButton>();
        // Settings
        private string _fontSplitPaymentTouchButtonSplitPayment;
        private int _intSplitPaymentTouchButtonSplitPaymentHeight;
        private Color _colorSplitPaymentTouchButtonFilledDataBackground;
        private int _intSplitPaymentStartClients;
        private int _intSplitPaymentMinClients;
        private int _intSplitPaymentMaxClients;

        public PosSplitPaymentsDialog(Window parentWindow, DialogFlags pDialogFlags, ArticleBag articleBag, TicketList ticketList)
            : base(parentWindow, pDialogFlags)
        {
            // Parameters
            _articleBag = articleBag;
            _ticketList = ticketList;

            // initSettingsValues
            initSettingsValues();

            //Init Local Vars
            // Title will be Overrided in CalculateTotalPerSplit
            string windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_split_payment");
            Size windowSize = new Size(600, 460);
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_split_payments.png";
            string fileAddSplitIcon = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_new.png";
            string fileRemoveSplitIcon = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_nav_delete.png";

            //Init Content : ViewPort
            _vbox = new VBox(false, 2);
            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            viewport.Add(_vbox);
            viewport.ResizeMode = ResizeMode.Parent;
            //ScrolledWindow
            ScrolledWindow _scrolledWindow = new ScrolledWindow();
            _scrolledWindow.ShadowType = ShadowType.EtchedIn;
            _scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            _scrolledWindow.Add(viewport);
            _scrolledWindow.ResizeMode = ResizeMode.Parent;

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
            _buttonOk.Sensitive = false;

            _buttonTableAddSplit = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonTableIncrementSplit_DialogActionArea",
                    BackgroundColor = ColorSettings.ActionAreaButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_add"),
                    Font = FontSettings.ActionAreaButton,
                    FontColor = ColorSettings.ActionAreaButtonFont,
                    Icon = fileAddSplitIcon,
                    IconSize = SizeSettings.ActionAreaButtonIcon,
                    ButtonSize = SizeSettings.ActionAreaButton
                })
            { Sensitive = true };

            _buttonTableRemoveSplit = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButtonTableDecrementSplit_DialogActionArea",
                    BackgroundColor = ColorSettings.ActionAreaButtonBackground,
                    Text = GeneralUtils.GetResourceByName("global_remove"),
                    Font = FontSettings.ActionAreaButton,
                    FontColor = ColorSettings.ActionAreaButtonFont,
                    Icon = fileRemoveSplitIcon,
                    IconSize = SizeSettings.ActionAreaButtonIcon,
                    ButtonSize = SizeSettings.ActionAreaButton
                })
            { Sensitive = true };

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonTableRemoveSplit, _responseTypeRemoveSplit),
                new ActionAreaButton(_buttonTableAddSplit, _responseTypeAddSplit),
                new ActionAreaButton(_buttonOk, ResponseType.Ok),
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
            };

            // Init Start SplitButtons : After Action Buttons
            for (int i = 0; i < _intSplitPaymentStartClients; i++)
            {
                AddSplitButton(false);
            }

            //Init Object
            this.Initialize(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _scrolledWindow, actionAreaButtons);

            // CalculateSplit to Calc and Assign Title after Dialog Construction
            CalculateSplit();
            // UpdateActionButtons
            UpdateActionButtons();
        }

        private void initSettingsValues()
        {
            // Settings : fontSplitPaymentTouchButtonSplitPayment
            try
            {
                _fontSplitPaymentTouchButtonSplitPayment = AppSettings.Instance.fontSplitPaymentTouchButtonSplitPayment;
            }
            catch (Exception)
            {
                _fontSplitPaymentTouchButtonSplitPayment = "Bold 12";
            }

            // Settings : intSplitPaymentTouchButtonSplitPaymentHeight
            try
            {
                _intSplitPaymentTouchButtonSplitPaymentHeight = AppSettings.Instance.intSplitPaymentTouchButtonSplitPaymentHeight;
            }
            catch (Exception)
            {
                _intSplitPaymentTouchButtonSplitPaymentHeight = 72;
            }

            // Settings : colorSplitPaymentTouchButtonFilledDataBackground
            try
            {
                _colorSplitPaymentTouchButtonFilledDataBackground = AppSettings.Instance.colorSplitPaymentTouchButtonFilledDataBackground;
            }
            catch (Exception)
            {
                _colorSplitPaymentTouchButtonFilledDataBackground = ("72,  84,  96").StringToColor();
            }

            // Settings : intSplitPaymentStartClients
            try
            {
                _intSplitPaymentStartClients = Convert.ToInt16(GeneralSettings.PreferenceParameters["SPLIT_PAYMENT_START_CLIENTS"]);
            }
            catch (Exception)
            {
                // Use Defaults
                _intSplitPaymentStartClients = 2;
            }

            // Settings : intSplitPaymentMinClients
            try
            {
                _intSplitPaymentMinClients = Convert.ToInt16(GeneralSettings.PreferenceParameters["SPLIT_PAYMENT_MIN_CLIENTS"]);
            }
            catch (Exception)
            {
                // Use Defaults
                _intSplitPaymentMinClients = 2;
            }

            // Settings : intSplitPaymentMaxClients
            try
            {
                _intSplitPaymentMaxClients = Convert.ToInt16(GeneralSettings.PreferenceParameters["SPLIT_PAYMENT_MAX_CLIENTS"]);
            }
            catch (Exception)
            {
                // Use Defaults
                _intSplitPaymentMaxClients = 10;
            }
        }
    }
}