using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Documents
{
    public class RePrintDocumentModal : Modal
    {
        private readonly string _documentNumber;
        public IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        public IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        public TextBox TxtMotive { get; set; }
        public uint CopyNumber { get; set; } = 2;
        private CheckButtonExtended BtnOriginal { get; } = new CheckButtonExtended(LocalizedString.Instance["global_print_copy_title1"]);
        private CheckButtonExtended BtnCopy2 { get; } = new CheckButtonExtended(LocalizedString.Instance["global_print_copy_title2"]);
        private CheckButtonExtended BtnCopy3 { get; } = new CheckButtonExtended(LocalizedString.Instance["global_print_copy_title3"]);
        private CheckButtonExtended BtnCopy4 { get; } = new CheckButtonExtended(LocalizedString.Instance["global_print_copy_title4"]);
        private List<CheckButtonExtended> Buttons { get; set; }
        private CheckButtonBox CheckSecondCopy { get; } = new CheckButtonBox(GeneralUtils.GetResourceByName("global_second_copy"), true);

        public RePrintDocumentModal(Window parent,
                                    string documentNumber) : base(parent,
                                                                  GeneralUtils.GetResourceByName("window_title_dialog_document_finance_print"),
                                                                  new Size(500, 400),
                                                                  PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_document_new.png")
        {
            _documentNumber = documentNumber;
            SetTitle();
        }

        private void SetTitle()
        {
            WindowSettings.Title.Text = string.Format(GeneralUtils.GetResourceByName("window_title_dialog_document_finance_print"),
                                                      _documentNumber);
        }

        private void InitializeButtons()
        {
            Buttons = new List<CheckButtonExtended>
            {
                BtnOriginal,
                BtnCopy2,
                BtnCopy3,
                BtnCopy4
            };
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

            var checkBoxes = new CheckButtonBoxGroup(GeneralUtils.GetResourceByName("global_print_copies"),
                                                     Buttons);


            verticalLayout.PackStart(checkBoxes);

            verticalLayout.PackStart(CheckSecondCopy);
            verticalLayout.PackStart(TxtMotive.Component, false, false, 0);

            return verticalLayout;
        }

        private void Initialize()
        {
            InitializeButtons();
            InitializeTxtMotive();
            AddEventHandlers();
        }

        private void CheckButton(CheckButtonExtended button)
        {
            foreach (var btn in Buttons)
            {    
                if(btn != button)
                {
                    btn.Active = false;
                }
            }
        }

        private void AddEventHandlers()
        {
            BtnOriginal.Toggled += (sender, args) =>
            {
                if (BtnOriginal.Active)
                {
                    CheckButton(BtnOriginal);
                    CopyNumber = 1;
                }

                TxtMotive.Component.Sensitive = BtnOriginal.Active;
            };

            BtnCopy2.Toggled += (sender, args) =>
                {
                    if (BtnCopy2.Active)
                    {
                        CheckButton(BtnCopy2);
                        CopyNumber = 2;
                    }
            };

            BtnCopy3.Toggled += (sender, args) =>
            {
                if (BtnCopy3.Active)
                {
                    CheckButton(BtnCopy3);
                    CopyNumber = 3;
                }
            };

            BtnCopy4.Toggled += (sender, args) =>
            {
                if (BtnCopy4.Active)
                {
                    CheckButton(BtnCopy4);
                    CopyNumber = 4;
                }
            };
        }

        private void InitializeTxtMotive()
        {
            TxtMotive = new TextBox(WindowSettings.Source,
                                       GeneralUtils.GetResourceByName("global_reprint_original_motive"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);

            TxtMotive.Component.Sensitive = false;
        }
    }
}
