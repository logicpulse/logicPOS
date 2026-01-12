using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System;
using System.Drawing;
using System.IO;

namespace LogicPOS.UI.Components.System.Users.Users.AccessKeyModal
{
    public class AccessKeyModal : Modal
    {
        private string _accessKey;

        public AccessKeyModal(Window parent) : base(parent,
                                                      "Chave de Acesso",
                                                      new Size(500, 200),
                                                      AppSettings.Paths.Images + @"Icons\Windows\icon_window_license.png")
        {
        }

        public IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        public IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        public TextBox TxtAcessKey { get; set; }


        protected override Widget CreateBody()
        {
            Initialize();

            var verticalLayout = new VBox(false, 2);
            verticalLayout.PackStart(TxtAcessKey.Component, false, false, 0);
            return verticalLayout;
        }

        private void Initialize()
        {

            BtnOk.Sensitive = false;
            BtnOk.Clicked += BtnOk_Clicked;
            InitializeTxtAccessKey();
            LoadAccessKey();
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            var result = TxtAcessKey.Text.Equals(_accessKey, StringComparison.InvariantCulture);

            if (result == false)
            {
                Respond(ResponseType.Cancel);
            }
        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        private void InitializeTxtAccessKey()
        {
            TxtAcessKey = new TextBox(WindowSettings.Source,
                                       "Chave de Acesso",
                                       isRequired: true,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);

            TxtAcessKey.Entry.Changed += (sender, args) =>
            {
                BtnOk.Sensitive = !string.IsNullOrWhiteSpace(TxtAcessKey.Text);
            };
        }

        private void LoadAccessKey()
        {
            if (!File.Exists("access.key"))
            {
                _accessKey = CreateAccessKey();
                return;
            }

            _accessKey = File.ReadAllText("access.key");
        }

        private static string CreateAccessKey()
        {
            var parts = Guid.NewGuid().ToString().ToUpper().Split('-');
            string accessKey = $"{parts[0]}-{parts[1]}";
            File.WriteAllText("access.key", accessKey);
            return accessKey;
        }
    }
}
