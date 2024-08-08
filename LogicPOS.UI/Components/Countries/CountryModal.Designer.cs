
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using LogicPOS.Globalization;
using LogicPOS.Utility;
using LogicPOS.UI.Components.InputFieds;
using LogicPOS.Domain.Entities;
using LogicPOS.Settings;
using System.IO;
using LogicPOS.UI.Buttons;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos;
using System.Collections.Generic;
using System;
using System.Linq;

namespace LogicPOS.UI.Components
{
    internal partial class CountryModal
    {
        #region Components
        private TextBox _txtOrder = new TextBox("global_record_order", true);
        private TextBox _txtCode = new TextBox("global_record_code", true);
        private TextBox _txtDesignation = new TextBox("global_designation", true);
        private TextBox _txtCapital = new TextBox("global_country_capital", true);
        private TextBox _txtCurrency = new TextBox("global_currency", true);
        private TextBox _txtCode2 = new TextBox("global_country_code2", true);
        private TextBox _txtCode3 = new TextBox("global_country_code3", true);
        private TextBox _txtCurrencyCode = new TextBox("global_currency_code");
        private TextBox _txtFiscalNumberRegex = new TextBox("global_regex_fiscal_number");
        private TextBox _txtZipCodeRegex = new TextBox("global_regex_postal_code");
        private MultilineTextBox _txtNotes = new MultilineTextBox();
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        private Notebook _notebook;
        protected HBox _statusBar;
        private IconButtonWithText _buttonOk;
        private IconButtonWithText _buttonCancel;
        private List<Widget> _fields = new List<Widget>();
        #endregion

        protected int _boxSpacing = 5;
        private DialogMode _dialogMode;

        private void SetWindowTitle()
        {
            Title = Utils.GetWindowTitle(GeneralUtils.GetResourceByName("window_title_edit_dialog_configuration_country"));
        }

        private void DesignUI(DialogMode dialogMode)
        {
            SetWindowTitle();
            SetWindowIcon();

            _dialogMode = dialogMode;
            WindowPosition = WindowPosition.CenterAlways;
            GrabFocus();
            SetSizeRequest(500, 500);

            this.Decorated = true;
            this.Resizable = false;
            this.WindowPosition = WindowPosition.Center;

            AccelGroup accelGroup = new AccelGroup();
            AddAccelGroup(accelGroup);

            HandleDialogMode();
            AddNotebook();
            AddStatusBar();
            AddActionButtons();
        }

        private void HandleDialogMode()
        {
            if(_fields.Any() == false)
            {
               RegisterFields();
            }

            if(_dialogMode == DialogMode.View)
            {
                _fields.ForEach(f => f.Sensitive = false);
            }
        }

        private void RegisterFields()
        {
            _fields.Add(_txtOrder.Entry);
            _fields.Add(_txtCode.Entry);
            _fields.Add(_txtDesignation.Entry);
            _fields.Add(_txtCapital.Entry);
            _fields.Add(_txtCurrency.Entry);
            _fields.Add(_txtCode2.Entry);
            _fields.Add(_txtCode3.Entry);
            _fields.Add(_txtCurrencyCode.Entry);
            _fields.Add(_txtFiscalNumberRegex.Entry);
            _fields.Add(_txtZipCodeRegex.Entry);
            _fields.Add(_txtNotes.TextView);
            _fields.Add(_checkDisabled);
        }

        private void AddStatusBar()
        {
            _statusBar = CreateStatusBar();
            VBox.PackStart(_statusBar, false, false, 0);
            this.AddActionWidget(_statusBar, -1);
        }

        private void AddNotebook()
        {
            _notebook = CreateNoteBook();
            VBox.PackStart(_notebook, true, true, 0);
        }

        private void SetWindowIcon()
        {
            string fileImageAppIcon = string.Format("{0}{1}", PathsSettings.ImagesFolderLocation, POSSettings.AppIcon);
            if (File.Exists(fileImageAppIcon))
            {
                Icon = logicpos.Utils.ImageToPixbuf(System.Drawing.Image.FromFile(fileImageAppIcon));
            }
        }

        private HBox CreateStatusBar()
        {
            var statusBar = new HBox(true, 0);
            statusBar.BorderWidth = 3;

            //UpdatedBy
            VBox vboxUpdatedBy = new VBox(true, 0);
            Label labelUpdatedBy = new Label(GeneralUtils.GetResourceByName("global_record_user_update"));
            Label labelUpdatedByValue = new Label(string.Empty);
            labelUpdatedBy.SetAlignment(0.0F, 0.5F);
            labelUpdatedByValue.SetAlignment(0.0F, 0.5F);
            vboxUpdatedBy.PackStart(labelUpdatedBy);
            vboxUpdatedBy.PackStart(labelUpdatedByValue);

            //CreatedAt
            VBox vboxCreatedAt = new VBox(true, 0);
            Label labelCreatedAt = new Label(GeneralUtils.GetResourceByName("global_record_date_created"));
            Label labelCreatedAtValue = new Label(string.Empty);
            labelCreatedAt.SetAlignment(0.5F, 0.5F);
            labelCreatedAtValue.SetAlignment(0.5F, 0.5F);
            vboxCreatedAt.PackStart(labelCreatedAt);
            vboxCreatedAt.PackStart(labelCreatedAtValue);

            //UpdatedAt
            VBox vboxUpdatedAt = new VBox(true, 0);
            Label labelUpdatedAt = new Label(GeneralUtils.GetResourceByName("global_record_date_updated_for_base_dialog"));
            Label labelUpdatedAtValue = new Label(string.Empty);
            labelUpdatedAt.SetAlignment(1.0F, 0.5F);
            labelUpdatedAtValue.SetAlignment(1.0F, 0.5F);
            vboxUpdatedAt.PackStart(labelUpdatedAt);
            vboxUpdatedAt.PackStart(labelUpdatedAtValue);

            statusBar.PackStart(vboxUpdatedBy);
            statusBar.PackStart(vboxCreatedAt);
            statusBar.PackStart(vboxUpdatedAt);

            return statusBar;
        }

        private VBox CreateTab1()
        {
            var tab1 =  new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if(_dialogMode != DialogMode.Insert)
            {
                tab1.PackStart(_txtOrder.Component, false, false, 0);
                tab1.PackStart(_txtCode.Component, false, false, 0);
            }

            tab1.PackStart(_txtDesignation.Component, false, false, 0);
            tab1.PackStart(_txtCapital.Component, false, false, 0);
            tab1.PackStart(_txtCurrency.Component, false, false, 0);
            tab1.PackStart(_checkDisabled, false, false, 0);

            return tab1;
        }

        private VBox CreateTab2()
        {
            var tab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            tab2.PackStart(_txtCode2.Component, false, false, 0);
            tab2.PackStart(_txtCode3.Component, false, false, 0);
            tab2.PackStart(_txtCurrencyCode.Component, false, false, 0);
            tab2.PackStart(_txtFiscalNumberRegex.Component, false, false, 0);
            tab2.PackStart(_txtZipCodeRegex.Component, false, false, 0);

            return tab2;
        }

        private Notebook CreateNoteBook()
        {
            Notebook notebook = new Notebook();
            notebook.BorderWidth = 3;

            notebook.AppendPage(CreateTab1(), new Label(GeneralUtils.GetResourceByName("global_record_main_detail")));
            notebook.AppendPage(CreateTab2(), new Label(GeneralUtils.GetResourceByName("global_others")));
            notebook.AppendPage(CreateNotesTab(), new Label(GeneralUtils.GetResourceByName("global_notes")));
            return notebook;
        }

        private void AddActionButtons()
        {
            string fontBaseDialogActionAreaButton = AppSettings.Instance.fontBaseDialogActionAreaButton;
            string tmpFileActionOK = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
            string tmpFileActionCancel = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";
            System.Drawing.Size sizeBaseDialogActionAreaButtonIcon = AppSettings.Instance.sizeBaseDialogActionAreaButtonIcon;
            System.Drawing.Size sizeBaseDialogActionAreaButton = AppSettings.Instance.sizeBaseDialogActionAreaButton;
            System.Drawing.Color colorBaseDialogActionAreaButtonBackground = AppSettings.Instance.colorBaseDialogActionAreaButtonBackground;
            System.Drawing.Color colorBaseDialogActionAreaButtonFont = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;

            if (GlobalApp.ScreenSize.Width == 800 && GlobalApp.ScreenSize.Height == 600)
            {
                sizeBaseDialogActionAreaButton.Height -= 10;
                sizeBaseDialogActionAreaButtonIcon.Width -= 10;
                sizeBaseDialogActionAreaButtonIcon.Height -= 10;
            };

            var buttonOkSettings = new ButtonSettings
            {
                Name = "touchButtonOk_DialogActionArea",
                BackgroundColor = colorBaseDialogActionAreaButtonBackground,
                Text = GeneralUtils.GetResourceByName("global_button_label_ok"),
                Font = fontBaseDialogActionAreaButton,
                FontColor = colorBaseDialogActionAreaButtonFont,
                Icon = tmpFileActionOK,
                IconSize = sizeBaseDialogActionAreaButtonIcon,
                ButtonSize = sizeBaseDialogActionAreaButton
            };

            _buttonOk = new IconButtonWithText(buttonOkSettings);
            _buttonOk.Clicked += ButtonOk_Clicked;

            var buttonCancelSettings = buttonOkSettings.Clone();

            buttonCancelSettings.Text = GeneralUtils.GetResourceByName("global_button_label_cancel");
            buttonCancelSettings.Name = "touchButtonCancel_DialogActionArea";
            buttonCancelSettings.Icon = tmpFileActionCancel;


            _buttonCancel = new IconButtonWithText(buttonCancelSettings);

            if (_dialogMode != DialogMode.View)
            {
                this.AddActionWidget(_buttonOk, ResponseType.Ok);
            }

            this.AddActionWidget(_buttonCancel, ResponseType.Cancel);
        }

        private VBox CreateNotesTab()
        {
            VBox box = new VBox(true, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
            _txtNotes.ScrolledWindow.BorderWidth = 0;
            box.PackStart(_txtNotes, true, true, 0);
            return box;
        }
    }
}
