using Gtk;
using logicpos;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System.Drawing;

namespace LogicPOS.UI.Components.Documents.CreateDocumentModal
{
    public class DocumentTypePage : ModalPage
    {
        public string BtnSelectRecordIcon => $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_select_record.png"}";
        public IconButton BtnSelectDocumentType { get; set; }
        public TextBox TxtDocumentType { get; set; }

        public DocumentTypePage(Window parent) : base(parent: parent,
                                                      name: GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page1"),
                                                      icon: PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_1_new_document.png")
        {
            InitializeButtons();
            Design();
        }

        private void InitializeButtons()
        {     
            BtnSelectDocumentType = new IconButton(new ButtonSettings { Name = "touchButtonIcon", Icon = BtnSelectRecordIcon, IconSize = new Size(20, 20), ButtonSize = new Size(30, 30) });
            BtnSelectDocumentType.Clicked += BtnSelectDocumentType_Clicked;
        }

        private void BtnSelectDocumentType_Clicked(object sender, System.EventArgs e)
        {
            var page = new DocumentTypesPage(null, PageOptions.SelectionPageOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<DocumentType>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtDocumentType.Text = page.SelectedEntity.Designation;
            }
        }

        private void Design()
        {
            var vbox = new VBox(false, 2);
            vbox.PackStart(CreateDocumentTypeField(), false, false, 0);
            PackStart(vbox);
        }

        private EventBox CreateGrayLine(Widget content)
        {
            var eventBox = new EventBox();
            eventBox.ModifyBg(StateType.Normal, AppSettings.Instance.colorBaseDialogEntryBoxBackground.ToGdkColor());
            eventBox.BorderWidth = 2;
            eventBox.Add(content);
            return eventBox;
        }

        private Widget CreateDocumentTypeField()
        {

            string labelFont = AppSettings.Instance.fontEntryBoxLabel;
            string txtFont = AppSettings.Instance.fontEntryBoxValue;
            int padding = 2;

            var rightButtons = new HBox(false, 2);
            rightButtons.PackStart(BtnSelectDocumentType, false, false, 0);


           
            TxtDocumentType = new TextBox(labelResourceName: "global_documentfinanceseries_documenttype",
                                      isRequired: true,
                                      buttonsArea: rightButtons);

            TxtDocumentType.Entry.IsEditable = false;

            TxtDocumentType.Label.ModifyFont(Pango.FontDescription.FromString(labelFont));
            TxtDocumentType.Entry.ModifyFont(Pango.FontDescription.FromString(txtFont));

            VBox box = new VBox(false, padding);
            box.BorderWidth = (uint)padding;
            box.PackStart(TxtDocumentType.Component, false, false, 0);

            return CreateGrayLine(box);
        }

        private HBox CreateTextBoxRightButtons(bool includeSelect = false,
                                               bool includeKeyboard = false)
        {
            var hbox = new HBox(false, 2);

            if (includeSelect)
            {
                hbox.PackStart(BtnSelectDocumentType, false, false, 0);
            }

            if (includeKeyboard)
            {
                hbox.PackStart(CreateKeyBoardButton(), false, false, 0);
            }

            return hbox;
        }

        public IconButton CreateKeyBoardButton()
        {
            var button = new IconButton(
               new ButtonSettings
               {
                   Icon = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_keyboard.png"}",
                   IconSize = new Size(20, 20),
                   ButtonSize = new Size(30, 30)
               });

            button.Clicked += (sender, args) => CallKeyboard(TxtDocumentType);

            return button;
        }

        private void CallKeyboard(TextBox txt)
        {
            KeyboardMode keyboardMode = KeyboardMode.AlfaNumeric;
            string text = txt.Text;
            string rule = string.Empty;
           
            if (keyboardMode == KeyboardMode.AlfaNumeric
                || keyboardMode == KeyboardMode.Alfa
                || keyboardMode == KeyboardMode.Numeric)
            {
                string input = Utils.GetVirtualKeyBoardInput(this.SourceWindow,
                                                             keyboardMode,
                                                             text,
                                                             rule);

                if (input != null)
                {
                    txt.Text = input;
                    txt.Entry.GrabFocus();
                }
            }
            else if (keyboardMode == KeyboardMode.Money)
            {
                PosMoneyPadDialog dialog = new PosMoneyPadDialog(this.SourceWindow,
                                                                 DialogFlags.DestroyWithParent,
                                                                 DataConversionUtils.StringToDecimal(text));
                int response = dialog.Run();
                if (response == (int)ResponseType.Ok)
                {
                    string input = DataConversionUtils.DecimalToString(dialog.Amount);
                    if (input != null)
                    {
                        txt.Entry.GrabFocus();
                    }
                }
                dialog.Destroy();
            }
        }
    }
}
