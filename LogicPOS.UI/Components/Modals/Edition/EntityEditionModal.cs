using ErrorOr;
using Gtk;
using logicpos;
using logicpos.App;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
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
    public abstract class EntityEditionModal<TEntity> : Dialog where TEntity : ApiEntity
    {
        protected TEntity _entity;
        protected readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<ISender>();
        protected readonly EntityEditionModalMode _modalMode;
        protected int _boxSpacing = 5;
        public abstract Size ModalSize { get; }
        public abstract string ModalTitleResourceName { get; }

        #region Components
        protected IconButtonWithText _buttonOk;
        protected IconButtonWithText _buttonCancel;
        public List<Widget> SensitiveFields { get; private set; } = new List<Widget>();
        public HashSet<IValidatableField> ValidatableFields { get; private set; } = new HashSet<IValidatableField>();

        protected HBox _statusBar;
        protected MultilineTextBox _txtNotes = new MultilineTextBox();
        #endregion

        public EntityEditionModal(EntityEditionModalMode modalMode,
                           TEntity entity = null)
        {
            _modalMode = modalMode;

            if(modalMode != EntityEditionModalMode.Insert)
            {
                _entity = entity;
            }

            BeforeDesign();
            Design();
            HandleModalMode();
            AddValidatableFields();      
            ShowAll();
        }

        protected virtual void BeforeDesign()
        {

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
            SimpleAlerts.ShowApiErrorAlert(this,error);
            this.Run();
        }

        protected bool AllFieldsAreValid()
        {
            return ValidatableFields.All(txt => txt.IsValid());
        }

        protected void ShowValidationErrors() => ValidationUtilities.ShowValidationErrors(ValidatableFields);

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

            if(_modalMode != EntityEditionModalMode.Insert)
            {
                AddStatusBar();
            }

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
            if (_entity == null && _modalMode != EntityEditionModalMode.Insert)
            {
                throw new ArgumentNullException(nameof(_entity));
            }

            if (SensitiveFields.Any() == false)
            {
                AddSensitiveFields();
            }

            SetViewMode();

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                ShowEntityData();
            }

        }

        private void SetViewMode()
        {
            if (_modalMode == EntityEditionModalMode.View)
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

            if (_modalMode != EntityEditionModalMode.View)
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
                case EntityEditionModalMode.Insert:
                    AddEntity();
                    break;
                case EntityEditionModalMode.Update:
                    UpdateEntity();
                    break;
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

        protected void ExecuteCommand<Response>(IRequest<ErrorOr<Response>> command)
        {
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }
        }

        protected void ExecuteAddCommand(IRequest<ErrorOr<Guid>> command) => ExecuteCommand(command);

        protected void ExecuteUpdateCommand(IRequest<ErrorOr<MediatR.Unit>> command) => ExecuteCommand(command);

        protected IEnumerable<TE> ExecuteGetAllQuery<TE>(IRequest<ErrorOr<IEnumerable<TE>>> query)
        {
            var result = _mediator.Send(query).Result;

            if (result.IsError)
            {
                return Enumerable.Empty<TE>();
            }

            return result.Value;
        }

        protected abstract void UpdateEntity();
        protected abstract void AddEntity();
        protected abstract void AddSensitiveFields();
        protected abstract void AddValidatableFields();
        protected abstract IEnumerable<(VBox Page, string Title)> CreateTabs();

    }
}
