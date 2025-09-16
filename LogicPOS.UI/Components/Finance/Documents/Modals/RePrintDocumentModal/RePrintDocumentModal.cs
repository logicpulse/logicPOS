using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
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
                                                                  AppSettings.Paths.Images + @"Icons\Windows\icon_window_document_new.png")
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
                    CheckButtonOnly(BtnOriginal);
                    Copies.Clear();
                    Copies.Add(1);
                }
                else
                {
                    Copies.Remove(1);
                }

                TxtMotive.Component.Sensitive = BtnOriginal.Active && !CheckSecondCopy.Active;
            };

            BtnCopy2.Toggled += (sender, args) =>
                {
                    if (BtnCopy2.Active)
                    {
                        Copies.Add(2);
                    } else
                    {
                        Copies.Remove(2);
                    }
                };

            BtnCopy3.Toggled += (sender, args) =>
            {
                if (BtnCopy3.Active)
                {
                    Copies.Add(3);
                }
                else
                {
                    Copies.Remove(3);
                }
            };

            BtnCopy4.Toggled += (sender, args) =>
            {
                if (BtnCopy4.Active)
                {
                    Copies.Add(4);
                }
                else
                {
                    Copies.Remove(4);
                }
            };
        }

    }
}
