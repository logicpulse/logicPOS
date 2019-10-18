using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Classes.Finance;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosSplitPaymentsDialog : PosBaseDialog
    {
        private ArticleBag _articleBag;
        private TicketList _ticketList;

        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;
        private TouchButtonIconWithText _buttonTableRemoveSplit;
        private TouchButtonIconWithText _buttonTableAddSplit;
        private ResponseType _responseTypeRemoveSplit = (ResponseType)11;
        private ResponseType _responseTypeAddSplit = (ResponseType)10;
        // Strore Total per Split
        private decimal _totalPerSplit;
        // UI
        private VBox _vbox;
        private List<TouchButtonSplitPayment> _splitPaymentButtons = new List<TouchButtonSplitPayment>();
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
            string windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_split_payment");
            Size windowSize = new Size(600, 460);
            string fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_split_payments.png");
            string fileAddSplitIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_nav_new.png");
            string fileRemoveSplitIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_nav_delete.png");

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

            _buttonTableAddSplit = new TouchButtonIconWithText("touchButtonTableIncrementSplit_DialogActionArea", _colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_add"), _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, fileAddSplitIcon, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height)
            { Sensitive = true };
            _buttonTableRemoveSplit = new TouchButtonIconWithText("touchButtonTableDecrementSplit_DialogActionArea", _colorBaseDialogActionAreaButtonBackground, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_remove"), _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, fileRemoveSplitIcon, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height)
            { Sensitive = true };

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(_buttonTableRemoveSplit, _responseTypeRemoveSplit));
            actionAreaButtons.Add(new ActionAreaButton(_buttonTableAddSplit, _responseTypeAddSplit));
            actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

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
                _fontSplitPaymentTouchButtonSplitPayment = GlobalFramework.Settings["fontSplitPaymentTouchButtonSplitPayment"];
            }
            catch (Exception)
            {
                _log.Debug(string.Format("Error in Config value fontSplitPaymentTouchButtonSplitPayment: [{0}]", _fontSplitPaymentTouchButtonSplitPayment));
                _fontSplitPaymentTouchButtonSplitPayment = "Bold 12";
            }

            // Settings : intSplitPaymentTouchButtonSplitPaymentHeight
            try
            {
                _intSplitPaymentTouchButtonSplitPaymentHeight = Convert.ToInt16(GlobalFramework.Settings["intSplitPaymentTouchButtonSplitPaymentHeight"]);
            }
            catch (Exception)
            {
                _log.Debug(string.Format("Error in Config value intSplitPaymentTouchButtonSplitPaymentHeight: [{0}]", _intSplitPaymentTouchButtonSplitPaymentHeight));
                _intSplitPaymentTouchButtonSplitPaymentHeight = 72;
            }

            // Settings : colorSplitPaymentTouchButtonFilledDataBackground
            try
            {
                _colorSplitPaymentTouchButtonFilledDataBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorSplitPaymentTouchButtonFilledDataBackground"]);
            }
            catch (Exception)
            {
                _log.Debug(string.Format("Error in Config value colorSplitPaymentTouchButtonFilledDataBackground: [{0}]", _colorSplitPaymentTouchButtonFilledDataBackground));
                _colorSplitPaymentTouchButtonFilledDataBackground = FrameworkUtils.StringToColor("72,  84,  96");
            }

            // Settings : intSplitPaymentStartClients
            try
            {
                _intSplitPaymentStartClients = Convert.ToInt16(GlobalFramework.PreferenceParameters["SPLIT_PAYMENT_START_CLIENTS"]);
            }
            catch (Exception)
            {
                _log.Debug(string.Format("Error in Config value SPLIT_PAYMENT_START_CLIENTS: [{0}]", Convert.ToInt16(GlobalFramework.PreferenceParameters["SPLIT_PAYMENT_START_CLIENTS"])));
                // Use Defaults
                _intSplitPaymentStartClients = 2;
            }

            // Settings : intSplitPaymentMinClients
            try
            {
                _intSplitPaymentMinClients = Convert.ToInt16(GlobalFramework.PreferenceParameters["SPLIT_PAYMENT_MIN_CLIENTS"]);
            }
            catch (Exception)
            {
                _log.Debug(string.Format("Error in Config value SPLIT_PAYMENT_MIN_CLIENTS: [{0}]", Convert.ToInt16(GlobalFramework.PreferenceParameters["SPLIT_PAYMENT_MIN_CLIENTS"])));
                // Use Defaults
                _intSplitPaymentMinClients = 2;
            }

            // Settings : intSplitPaymentMaxClients
            try
            {
                _intSplitPaymentMaxClients = Convert.ToInt16(GlobalFramework.PreferenceParameters["SPLIT_PAYMENT_MAX_CLIENTS"]);
            }
            catch (Exception)
            {
                _log.Debug(string.Format("Error in Config value SPLIT_PAYMENT_MAX_CLIENTS: [{0}]", Convert.ToInt16(GlobalFramework.PreferenceParameters["SPLIT_PAYMENT_MAX_CLIENTS"])));
                // Use Defaults
                _intSplitPaymentMaxClients = 10;
            }
        }
    }
}