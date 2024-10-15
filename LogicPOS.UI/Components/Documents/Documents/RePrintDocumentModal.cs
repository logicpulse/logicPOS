using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Documents
{
    public class RePrintDocumentModal : Modal
    {
        private readonly Document _document;
        public IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        public IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        public PageTextBox TxtMotive { get; set; }

        public RePrintDocumentModal(Window parent, Document document) : base(parent,
                                                          GeneralUtils.GetResourceByName("window_title_dialog_document_finance_print"),
                                                          new Size(500, 400),
                                                          PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_document_new.png")
        {
            _document = document;
            SetTitle();
        }

        private void SetTitle()
        {
            WindowSettings.Title.Text = string.Format(GeneralUtils.GetResourceByName("window_title_dialog_document_finance_print"), _document.Number);
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

            var verticalLayout = new VBox(false, 2);

            var _printCopies = 1;

            Dictionary<string, bool> buttonGroup = new Dictionary<string, bool>
            {
                { GeneralUtils.GetResourceByName("global_print_copy_title1"), (_printCopies >= 1) },
                { GeneralUtils.GetResourceByName("global_print_copy_title2"), (_printCopies >= 2) },
                { GeneralUtils.GetResourceByName("global_print_copy_title3"), (_printCopies >= 3) },
                { GeneralUtils.GetResourceByName("global_print_copy_title4"), (_printCopies >= 4) }
            };

            var _checkButtonCopyNamesBoxGroup = new CheckButtonBoxGroup(GeneralUtils.GetResourceByName("global_print_copies"), buttonGroup);
            verticalLayout.PackStart(_checkButtonCopyNamesBoxGroup);

            var _checkButtonBoxSecondCopy = new CheckButtonBox(GeneralUtils.GetResourceByName("global_second_copy"), true);
            verticalLayout.PackStart(_checkButtonBoxSecondCopy);

            verticalLayout.PackStart(TxtMotive.Component, false, false, 0);
            return verticalLayout;
        }

        private void Initialize()
        {
            InitializeTxtMotive();
        }

        private void InitializeTxtMotive()
        {
            TxtMotive = new PageTextBox(WindowSettings.Source,
                                       GeneralUtils.GetResourceByName("global_reprint_original_motive"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }
    }
}
