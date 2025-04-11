using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Settings;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class RePrintDocumentModal : Modal
    {
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

    }
}
