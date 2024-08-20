using ErrorOr;
using Gtk;
using logicpos;
using logicpos.App;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFieds;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public abstract class EntityModal : Dialog
    {
        protected ApiEntity _entity;
        protected readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<ISender>();
        protected readonly EntityModalMode _modalMode;
        protected int _boxSpacing = 5;
        public abstract Size ModalSize { get; }
        public abstract string ModalTitleResourceName { get; }

        #region Components
        protected IconButtonWithText _buttonOk;
        protected IconButtonWithText _buttonCancel;
        public List<Widget> SensitiveFields { get; private set; } = new List<Widget>();
        public List<TextBox> ValidatableFields { get; private set; } = new List<TextBox>();

        protected HBox _statusBar;
        protected MultilineTextBox _txtNotes = new MultilineTextBox();
        #endregion

        public EntityModal(EntityModalMode modalMode, ApiEntity entity = null)
        {
            _modalMode = modalMode;
            _entity = entity;

            HandleModalMode();
            AddValidatableFields();
            Design();
            ShowAll();
        }

        protected void Validate()
        {
            if (AllFieldsAreValid())
            {
                return;
            }

            ShowValidationErrors();

            this.Run();
        }

        protected virtual void HandleApiError(Error error)
        {
            switch (error.Type)
            {
                case ErrorType.Validation:
                    var problem = error.Metadata["problem"] as ProblemDetails;
                    SimpleAlerts.Error()
                                .WithParent(this)
                                .WithMessage(problem.Errors.First().Reason)
                                .WithTitle(problem.Title)
                                .Show();
                    break;

                default:
                    SimpleAlerts.Error()
                                .WithParent(this)
                                .WithMessage(error.Description ?? "Ocorreu um erro")
                                .WithTitle("Erro inesperado")
                                .Show();
                    break;
            }

            this.Run();
        }

        protected bool AllFieldsAreValid()
        {
            return ValidatableFields.All(txt => txt.IsValid());
        }

        protected void ShowValidationErrors()
        {
            var invalidFields = string.Join(", ", ValidatableFields.Where(txt => txt.IsValid() == false).Select(txt => txt.Label.Text));

            Utils.ShowMessageBox(GlobalApp.BackOfficeMainWindow,
                                 DialogFlags.DestroyWithParent | DialogFlags.Modal,
                                 new Size(500, 500),
                                 MessageType.Error,
                                 ButtonsType.Ok,
                                 GeneralUtils.GetResourceByName("window_title_dialog_validation_error"),
                                 string.Format(GeneralUtils.GetResourceByName("dialog_message_field_validation_error"), invalidFields));
        }

        private void Design()
        {
            SetWindowTitle();
            SetWindowIcon();
            GrabFocus();
            SetSizeRequest(ModalSize.Width, ModalSize.Height);

            Decorated = true;
            Resizable = false;
            WindowPosition = WindowPosition.Center;

            AddAccelGroup(new AccelGroup());

            VBox.PackStart(CreateNoteBook(), true, true, 0);

            AddStatusBar();
            AddActionButtons();
        }

        private void AddStatusBar()
        {
            _statusBar = CreateStatusBar();
            VBox.PackStart(_statusBar, false, false, 0);
            this.AddActionWidget(_statusBar, -1);
        }

        private void HandleModalMode()
        {
            if (_entity == null && _modalMode != EntityModalMode.Insert)
            {
                throw new ArgumentNullException(nameof(_entity));
            }

            if (SensitiveFields.Any() == false)
            {
                AddSensitiveFields();
            }

            SetViewMode();

            if (_modalMode != EntityModalMode.Insert)
            {
                ShowEntityData();
            }

        }

        private void SetViewMode()
        {
            if (_modalMode == EntityModalMode.View)
            {
                SensitiveFields.ForEach(field => field.Sensitive = false);
            }
        }

        private void SetWindowTitle()
        {
            Title = GeneralUtils.GetResourceByName(ModalTitleResourceName);
        }

        private void SetWindowIcon()
        {
            string fileImageAppIcon = string.Format("{0}{1}", PathsSettings.ImagesFolderLocation, POSSettings.AppIcon);
            if (File.Exists(fileImageAppIcon))
            {
                Icon = Utils.ImageToPixbuf(System.Drawing.Image.FromFile(fileImageAppIcon));
            }
        }

        protected void AddActionButtons()
        {
            string fontBaseDialogActionAreaButton = AppSettings.Instance.fontBaseDialogActionAreaButton;
            string tmpFileActionOK = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
            string tmpFileActionCancel = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";
            Size sizeBaseDialogActionAreaButtonIcon = AppSettings.Instance.sizeBaseDialogActionAreaButtonIcon;
            Size sizeBaseDialogActionAreaButton = AppSettings.Instance.sizeBaseDialogActionAreaButton;
            Color colorBaseDialogActionAreaButtonBackground = AppSettings.Instance.colorBaseDialogActionAreaButtonBackground;
            Color colorBaseDialogActionAreaButtonFont = AppSettings.Instance.colorBaseDialogActionAreaButtonFont;

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

            if (_modalMode != EntityModalMode.View)
            {
                this.AddActionWidget(_buttonOk, ResponseType.Ok);
            }

            this.AddActionWidget(_buttonCancel, ResponseType.Cancel);
        }

        protected virtual void ButtonOk_Clicked(object sender, EventArgs e)
        {
            if(AllFieldsAreValid() == false)
            {
                Validate();
                return;
            }

            switch (_modalMode)
            {
                case EntityModalMode.Insert:
                    AddEntity();
                    break;
                case EntityModalMode.Update:
                    UpdateEntity();
                    break;
                default:
                    throw new Exception("Invalid modal mode");

            }
        }

        private HBox CreateStatusBar()
        {
            var statusBar = new HBox(true, 0);
            statusBar.BorderWidth = 3;

            //UpdatedBy
            VBox vboxUpdatedBy = new VBox(true, 0);
            Label labelUpdatedBy = new Label(GeneralUtils.GetResourceByName("global_record_user_update"));
            var lastUpdatedBy = _entity?.UpdatedBy.ToString() ?? "?";
            Label labelUpdatedByValue = new Label(lastUpdatedBy);
            labelUpdatedBy.SetAlignment(0.0F, 0.5F);
            labelUpdatedByValue.SetAlignment(0.0F, 0.5F);
            vboxUpdatedBy.PackStart(labelUpdatedBy);
            vboxUpdatedBy.PackStart(labelUpdatedByValue);

            //CreatedAt
            VBox vboxCreatedAt = new VBox(true, 0);
            Label labelCreatedAt = new Label(GeneralUtils.GetResourceByName("global_record_date_created"));
            var createdAt = _entity?.CreatedAt.ToString() ?? "?";
            Label labelCreatedAtValue = new Label(createdAt);
            labelCreatedAt.SetAlignment(0.5F, 0.5F);
            labelCreatedAtValue.SetAlignment(0.5F, 0.5F);
            vboxCreatedAt.PackStart(labelCreatedAt);
            vboxCreatedAt.PackStart(labelCreatedAtValue);

            //UpdatedAt
            VBox vboxUpdatedAt = new VBox(true, 0);
            Label labelUpdatedAt = new Label(GeneralUtils.GetResourceByName("global_record_date_updated_for_base_dialog"));
            var updatedAt = _entity?.UpdatedAt.ToString() ?? "?";
            Label labelUpdatedAtValue = new Label(updatedAt);
            labelUpdatedAt.SetAlignment(1.0F, 0.5F);
            labelUpdatedAtValue.SetAlignment(1.0F, 0.5F);
            vboxUpdatedAt.PackStart(labelUpdatedAt);
            vboxUpdatedAt.PackStart(labelUpdatedAtValue);

            statusBar.PackStart(vboxUpdatedBy);
            statusBar.PackStart(vboxCreatedAt);
            statusBar.PackStart(vboxUpdatedAt);

            return statusBar;
        }

        protected virtual VBox CreateNotesTab()
        {
            VBox box = new VBox(true, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
            _txtNotes.ScrolledWindow.BorderWidth = 0;
            box.PackStart(_txtNotes, true, true, 0);
            return box;
        }

        protected abstract void ShowEntityData();

        protected virtual Notebook CreateNoteBook()
        {
            Notebook notebook = new Notebook();
            notebook.BorderWidth = 3;

            foreach (var tab in CreateTabs())
            {
                notebook.AppendPage(tab.Page, new Label(tab.Title));
            }

            return notebook;
        }

        protected abstract void UpdateEntity();
        protected abstract void AddEntity();
        protected abstract void AddSensitiveFields();
        protected abstract void AddValidatableFields();
        protected abstract IEnumerable<(VBox Page, string Title)> CreateTabs();

    }
}
