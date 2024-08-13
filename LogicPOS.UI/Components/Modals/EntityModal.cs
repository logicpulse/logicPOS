using ErrorOr;
using Gtk;
using logicpos;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public abstract class EntityModal : Dialog
    {
        protected readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<ISender>();
        protected readonly EntityModalMode _modalMode;
        #region Components
        protected IconButtonWithText _buttonOk;
        protected IconButtonWithText _buttonCancel;
        protected List<Widget> _fields = new List<Widget>();
        protected HBox _statusBar;
        #endregion
        protected int _boxSpacing = 5;
        protected ApiEntity _entity;

        public abstract System.Drawing.Size ModalSize { get; }
        public abstract string ModalTitleResourceName { get; }

        public EntityModal(EntityModalMode modalMode, ApiEntity entity = null) 
        {
            _modalMode = modalMode;
            _entity = entity;

            HandleModalMode();
            DefaultDesign();
            ShowAll();
        }

        protected virtual void HandleError(Error error)
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

        }

        private void DefaultDesign()
        {
            SetWindowTitle();
            SetWindowIcon();
            GrabFocus();
            SetSizeRequest(ModalSize.Width, ModalSize.Height);

            Decorated = true;
            Resizable = false;
            WindowPosition = WindowPosition.Center;

            AddAccelGroup(new AccelGroup());

            Design();

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

            if (_fields.Any() == false)
            {
                RegisterFields();
            }

            SetViewMode();

            if (_modalMode != EntityModalMode.Insert)
            {
                ShowEntity();
            }

        }

        private void SetViewMode()
        {
            if (_modalMode == EntityModalMode.View)
            {
                _fields.ForEach(f => f.Sensitive = false);
            }
        }

        protected abstract void ShowEntity();

        protected abstract void Design();

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

        protected  void AddActionButtons()
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

            if (_modalMode != EntityModalMode.View)
            {
                this.AddActionWidget(_buttonOk, ResponseType.Ok);
            }

            this.AddActionWidget(_buttonCancel, ResponseType.Cancel);
        }

        protected abstract void ButtonOk_Clicked(object sender, EventArgs e);

        protected abstract void RegisterFields();

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

    }
}
