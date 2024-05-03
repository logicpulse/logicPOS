using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using logicpos.Extensions;
using logicpos.shared.App;
using logicpos.shared.Classes.Finance;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosSplitPaymentsDialog : PosBaseDialog
    {
        private readonly ArticleBag _articleBag;
        private readonly TicketList _ticketList;

        private readonly TouchButtonIconWithText _buttonOk;
        private readonly TouchButtonIconWithText _buttonCancel;
        private readonly TouchButtonIconWithText _buttonTableRemoveSplit;
        private readonly TouchButtonIconWithText _buttonTableAddSplit;
        private readonly ResponseType _responseTypeRemoveSplit = (ResponseType)11;
        private readonly ResponseType _responseTypeAddSplit = (ResponseType)10;
        // Strore Total per Split
        private decimal _totalPerSplit;
        // UI
        private readonly VBox _vbox;
        private readonly List<TouchButtonSplitPayment> _splitPaymentButtons = new List<TouchButtonSplitPayment>();
        // Settings
        private string _fontSplitPaymentTouchButtonSplitPayment;
        private int _intSplitPaymentTouchButtonSplitPaymentHeight;
        private Color _colorSplitPaymentTouchButtonFilledDataBackground;
        private int _intSplitPaymentStartClients;
        private int _intSplitPaymentMinClients;
        private int _intSplitPaymentMaxClients;

        public PosSplitPaymentsDialog(Window pSourceWindow, DialogFlags pDialogFlags, ArticleBag articleBag, TicketList ticketList)
            : base(pSourceWindow, pDialogFlags)
        {
            // Parameters
            _articleBag = articleBag;
            _ticketList = ticketList;

            // initSettingsValues
            initSettingsValues();

            //Init Local Vars
            // Title will be Overrided in CalculateTotalPerSplit
            string windowTitle = resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "window_title_dialog_split_payment");
            Size windowSize = new Size(600, 460);
            string fileDefaultWindowIcon = DataLayerFramework.Path["images"] + @"Icons\Windows\icon_window_split_payments.png";
            string fileAddSplitIcon = DataLayerFramework.Path["images"] + @"Icons\icon_pos_nav_new.png";
            string fileRemoveSplitIcon = DataLayerFramework.Path["images"] + @"Icons\icon_pos_nav_delete.png";

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
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            _buttonOk.Sensitive = false;

            _buttonTableAddSplit = new TouchButtonIconWithText("touchButtonTableIncrementSplit_DialogActionArea", _colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "global_add"), _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, fileAddSplitIcon, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height)
            { Sensitive = true };
            _buttonTableRemoveSplit = new TouchButtonIconWithText("touchButtonTableDecrementSplit_DialogActionArea", _colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "global_remove"), _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, fileRemoveSplitIcon, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height)
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
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, _scrolledWindow, actionAreaButtons);

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
                _fontSplitPaymentTouchButtonSplitPayment = LogicPOS.Settings.GeneralSettings.Settings["fontSplitPaymentTouchButtonSplitPayment"];
            }
            catch (Exception)
            {
                _logger.Debug(string.Format("Error in Config value fontSplitPaymentTouchButtonSplitPayment: [{0}]", _fontSplitPaymentTouchButtonSplitPayment));
                _fontSplitPaymentTouchButtonSplitPayment = "Bold 12";
            }

            // Settings : intSplitPaymentTouchButtonSplitPaymentHeight
            try
            {
                _intSplitPaymentTouchButtonSplitPaymentHeight = Convert.ToInt16(LogicPOS.Settings.GeneralSettings.Settings["intSplitPaymentTouchButtonSplitPaymentHeight"]);
            }
            catch (Exception)
            {
                _logger.Debug(string.Format("Error in Config value intSplitPaymentTouchButtonSplitPaymentHeight: [{0}]", _intSplitPaymentTouchButtonSplitPaymentHeight));
                _intSplitPaymentTouchButtonSplitPaymentHeight = 72;
            }

            // Settings : colorSplitPaymentTouchButtonFilledDataBackground
            try
            {
                _colorSplitPaymentTouchButtonFilledDataBackground = LogicPOS.Settings.GeneralSettings.Settings["colorSplitPaymentTouchButtonFilledDataBackground"].StringToColor();
            }
            catch (Exception)
            {
                _logger.Debug(string.Format("Error in Config value colorSplitPaymentTouchButtonFilledDataBackground: [{0}]", _colorSplitPaymentTouchButtonFilledDataBackground));
                _colorSplitPaymentTouchButtonFilledDataBackground = ("72,  84,  96").StringToColor();
            }

            // Settings : intSplitPaymentStartClients
            try
            {
                _intSplitPaymentStartClients = Convert.ToInt16(LogicPOS.Settings.GeneralSettings.PreferenceParameters["SPLIT_PAYMENT_START_CLIENTS"]);
            }
            catch (Exception)
            {
                _logger.Debug(string.Format("Error in Config value SPLIT_PAYMENT_START_CLIENTS: [{0}]", Convert.ToInt16(LogicPOS.Settings.GeneralSettings.PreferenceParameters["SPLIT_PAYMENT_START_CLIENTS"])));
                // Use Defaults
                _intSplitPaymentStartClients = 2;
            }

            // Settings : intSplitPaymentMinClients
            try
            {
                _intSplitPaymentMinClients = Convert.ToInt16(LogicPOS.Settings.GeneralSettings.PreferenceParameters["SPLIT_PAYMENT_MIN_CLIENTS"]);
            }
            catch (Exception)
            {
                _logger.Debug(string.Format("Error in Config value SPLIT_PAYMENT_MIN_CLIENTS: [{0}]", Convert.ToInt16(LogicPOS.Settings.GeneralSettings.PreferenceParameters["SPLIT_PAYMENT_MIN_CLIENTS"])));
                // Use Defaults
                _intSplitPaymentMinClients = 2;
            }

            // Settings : intSplitPaymentMaxClients
            try
            {
                _intSplitPaymentMaxClients = Convert.ToInt16(LogicPOS.Settings.GeneralSettings.PreferenceParameters["SPLIT_PAYMENT_MAX_CLIENTS"]);
            }
            catch (Exception)
            {
                _logger.Debug(string.Format("Error in Config value SPLIT_PAYMENT_MAX_CLIENTS: [{0}]", Convert.ToInt16(LogicPOS.Settings.GeneralSettings.PreferenceParameters["SPLIT_PAYMENT_MAX_CLIENTS"])));
                // Use Defaults
                _intSplitPaymentMaxClients = 10;
            }
        }
    }
}