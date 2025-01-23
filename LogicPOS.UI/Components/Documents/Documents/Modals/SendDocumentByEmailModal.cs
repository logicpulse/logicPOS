using Gtk;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public class SendDocumentByEmailModal : Modal
    {
        #region Components
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        private PageTextBox TxtSubject { get; set; }
        private PageTextBox TxtTo { get; set; }
        private PageTextBox TxtCc { get; set; }
        private PageTextBox TxtBcc { get; set; }
        #endregion

        public SendDocumentByEmailModal(Window parent) : base(parent,
                                                              LocalizedString.Instance["window_title_send_email"],
                                                              new Size(800, 640),
                                                              PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_send_email.png")
        {
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        protected override Widget CreateBody()
        {
            Initialize();
            VBox verticalLayout = new VBox(false, 0);
            verticalLayout.PackStart(TxtSubject.Component, false, false, 0);
            verticalLayout.PackStart(TxtTo.Component, false, false, 0);
            verticalLayout.PackStart(TxtCc.Component, false, false, 0);
            verticalLayout.PackStart(TxtBcc.Component, false, false, 0);

            return verticalLayout;
        }

        private void Initialize()
        {
            InitializeTxtSubject();
            InitializeTxtTo();
            InitializeTxtCc();
            InitializeTxtBcc();
        }

        private void InitializeTxtSubject()
        {
            TxtSubject = new PageTextBox(this,
                                        LocalizedString.Instance["global_email_subject"],
                                        true);
        }

        private void InitializeTxtTo()
        {
            TxtTo = new PageTextBox(this,
                                    LocalizedString.Instance["global_email_to"],
                                    true,
                                    true,
                                    RegularExpressions.Email);
        }

        private void InitializeTxtCc()
        {
            TxtCc = new PageTextBox(this,
                                    LocalizedString.Instance["global_email_cc"],
                                    true,
                                    true,
                                    RegularExpressions.Email);
        }

        private void InitializeTxtBcc()
        {
            TxtBcc = new PageTextBox(this,
                                     LocalizedString.Instance["global_email_bcc"],
                                     true,
                                     true,
                                     RegularExpressions.Email);
        }
    }
}
