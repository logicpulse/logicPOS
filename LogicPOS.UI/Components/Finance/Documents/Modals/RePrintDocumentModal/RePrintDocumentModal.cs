using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LogicPOS.Globalization;

namespace LogicPOS.UI.Components.Modals
{
    public partial class RePrintDocumentModal : Modal
    {
        private readonly int _printCopies;
        private readonly bool _printRequestMotive;

        private const int WindowWidth = 400;
        private const int WindowHeight = 259;
        private const int WindowHeightWithMotiveExtra = 118; // legacy: 42 + 76

        public RePrintDocumentModal(
            Window parent,
            string documentNumber,
            int printCopies = 4,
            bool printRequestMotive = false)
            : base(parent,
                   LocalizedString.Instance["window_title_dialog_document_finance_print"],
                   GetWindowSize(printRequestMotive),
                   AppSettings.Paths.Images + @"Icons\Windows\icon_window_document_new.png",
                   render: false)
        {
            _documentNumber = documentNumber;
            _printCopies = Math.Max(1, Math.Min(4, printCopies));
            _printRequestMotive = printRequestMotive;
            SetTitle();
            Render();
            InitializeDefaultCopyNumber();
        }

        private static Size GetWindowSize(bool printRequestMotive) =>
            new Size(
                WindowWidth,
                WindowHeight + (printRequestMotive ? WindowHeightWithMotiveExtra : 0));

        private void InitializeDefaultCopyNumber()
        {
            if (_printRequestMotive)
            {
                CheckSecondCopy.Active = true;
            }

            ApplyDefaultCopySelection();
        }

        private void ApplyDefaultCopySelection()
        {
            BtnOriginal.Active = true;
            BtnCopy2.Active = _printCopies >= 2;
            BtnCopy3.Active = _printCopies >= 3;
            BtnCopy4.Active = _printCopies >= 4;

            Copies = new List<int>();
            for (var index = 0; index < Buttons.Count; index++)
            {
                if (Buttons[index].Active)
                {
                    Copies.Add(index + 1);
                }
            }

            if (Copies.Count == 0)
            {
                Copies.Add(1);
                BtnOriginal.Active = true;
            }
        }

        private void SetTitle()
        {
            WindowSettings.Title.Text = string.Format(
                LocalizedString.Instance["window_title_dialog_document_finance_print"],
                _documentNumber);
        }

        protected override Widget CreateBody()
        {
            Initialize();

            var verticalLayout = new VBox(false, 0);

            var checkBoxes = new CheckButtonBoxGroup(
                LocalizedString.Instance["global_print_copies"],
                Buttons);

            verticalLayout.PackStart(checkBoxes, false, false, 0);

            if (_printRequestMotive)
            {
                verticalLayout.PackStart(CheckSecondCopy, false, false, 0);
                verticalLayout.PackStart(TxtMotive.Component, false, false, 0);
            }

            return verticalLayout;
        }

        private void Initialize()
        {
            InitializeButtons();
            if (_printRequestMotive)
            {
                InitializeTxtMotive();
            }

            AddEventHandlers();
            UpdateBtnOkSensitive();
        }

        private void AddEventHandlers()
        {
            BtnOriginal.Toggled += (_, __) =>
            {
                if (BtnOriginal.Active)
                {
                    if (!Copies.Contains(1))
                    {
                        Copies.Add(1);
                    }
                }
                else if (_printRequestMotive && !CheckSecondCopy.Active)
                {
                    BtnOriginal.Active = true;
                    return;
                }
                else
                {
                    Copies.Remove(1);
                }

                UpdateBtnOkSensitive();
            };

            BtnCopy2.Toggled += (_, __) => ToggleCopy(2, BtnCopy2.Active);
            BtnCopy3.Toggled += (_, __) => ToggleCopy(3, BtnCopy3.Active);
            BtnCopy4.Toggled += (_, __) => ToggleCopy(4, BtnCopy4.Active);

            if (_printRequestMotive)
            {
                CheckSecondCopy.Clicked += CheckSecondCopy_Clicked;
                CheckSecondCopy.StateChanged += CheckSecondCopy_Clicked;
                TxtMotive.Entry.Changed += (_, __) => UpdateBtnOkSensitive();
            }
        }

        private void ToggleCopy(int copyNumber, bool active)
        {
            if (active)
            {
                if (!Copies.Contains(copyNumber))
                {
                    Copies.Add(copyNumber);
                }
            }
            else
            {
                Copies.Remove(copyNumber);
            }

            UpdateBtnOkSensitive();
        }

        private void UpdateBtnOkSensitive()
        {
            if (Copies.Count == 0)
            {
                BtnOk.Sensitive = false;
                return;
            }

            if (!_printRequestMotive || SecondPrint)
            {
                BtnOk.Sensitive = true;
                return;
            }

            BtnOk.Sensitive = !string.IsNullOrWhiteSpace(TxtMotive.Text);
        }

        private void CheckSecondCopy_Clicked(object sender, EventArgs e)
        {
            if (CheckSecondCopy.Active)
            {
                TxtMotive.Component.Sensitive = false;

                if (Copies.Count == 0)
                {
                    Copies.Add(1);
                    BtnOriginal.Active = true;
                }
            }
            else
            {
                TxtMotive.Component.Sensitive = true;
                BtnOriginal.Active = true;
                CheckButtonOnly(BtnOriginal);
            }

            UpdateBtnOkSensitive();
        }
    }
}
