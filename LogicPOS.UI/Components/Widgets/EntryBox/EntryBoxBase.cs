using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public abstract class EntryBoxBase : EventBox
    {
        //Log4Net
        protected static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Protected
        protected Window _sourceWindow;
        protected Label _label;
        protected Label _label2;
        protected Label _label3;

        //Public    
        protected VBox _vbox;
        public VBox Vbox
        {
            get { return _vbox; }
            set { _vbox = value; }
        }
        protected HBox _hbox;
        public HBox Hbox
        {
            get { return _hbox; }
            set { _hbox = value; }
        }
        protected Pango.FontDescription _fontDescription;
        public Label Label
        {
            get { return _label; }
            set { _label = value; }
        }
        public Label Label2
        {
            get { return _label2; }
            set { _label2 = value; }
        }
        public Label Label3
        {
            get { return _label3; }
            set { _label3 = value; }
        }
        public EntryBoxBase(string pLabelText)
            : this(null, pLabelText)
        {
        }
        protected IconButton _buttonKeyBoard;
        public IconButton ButtonKeyBoard
        {
            get { return _buttonKeyBoard; }
            set { _buttonKeyBoard = value; }
        }

        public EntryBoxBase(Window parentWindow, string pLabelText)
        {
            //Parameters
            _sourceWindow = parentWindow;
            //Defaults
            Color colorBaseDialogEntryBoxBackground = AppSettings.Instance.colorBaseDialogEntryBoxBackground;
            string fontLabel = AppSettings.Instance.fontEntryBoxLabel;
            string fontEntry = AppSettings.Instance.fontEntryBoxValue;
            int padding = 2;
            //This
            this.ModifyBg(StateType.Normal, colorBaseDialogEntryBoxBackground.ToGdkColor());
            this.BorderWidth = (uint)padding;
            //VBox
            _vbox = new VBox(false, padding);
            _vbox.BorderWidth = (uint)padding;
            //Label
            Pango.FontDescription fontDescriptionLabel = Pango.FontDescription.FromString(fontLabel);
            _label = new Label(pLabelText);
            _label.ModifyFont(fontDescriptionLabel);
            _label.SetAlignment(0, 0.5F);
            //Child Entrys
            _fontDescription = Pango.FontDescription.FromString(fontEntry);
            //HBox
            _hbox = new HBox(false, padding);
            //Pack
            _vbox.PackStart(_label, false, false, 0);
            _vbox.PackStart(_hbox, true, true, 0);
            //Finish
            Add(_vbox);
        }
        //Artigos Compostos [IN:016522]
        public EntryBoxBase(Window parentWindow, string pLabelText, bool pBOsource = false)
        {
            if (!pBOsource)
            {
                //Parameters
                _sourceWindow = parentWindow;
                //Defaults
                Color colorBaseDialogEntryBoxBackground = AppSettings.Instance.colorBaseDialogEntryBoxBackground;
                string fontLabel = AppSettings.Instance.fontEntryBoxLabel;
                string fontEntry = AppSettings.Instance.fontEntryBoxValue;
                int padding = 2;
                //This
                this.ModifyBg(StateType.Normal, colorBaseDialogEntryBoxBackground.ToGdkColor());
                this.BorderWidth = (uint)padding;
                //VBox
                _vbox = new VBox(false, padding);
                _vbox.BorderWidth = (uint)padding;
                //Label
                Pango.FontDescription fontDescriptionLabel = Pango.FontDescription.FromString(fontLabel);
                _label = new Label(pLabelText);
                _label.ModifyFont(fontDescriptionLabel);
                _label.SetAlignment(0, 0.5F);
                //Child Entrys
                _fontDescription = Pango.FontDescription.FromString(fontEntry);
                //HBox
                _hbox = new HBox(false, padding);
                //Pack
                _vbox.PackStart(_label, false, false, 0);
                _vbox.PackStart(_hbox, true, true, 0);
                //Finish
                Add(_vbox);
            }
            else if ((parentWindow.GetType().Name == "DialogArticleStockMoviment" || parentWindow.GetType().Name == "DialogAddArticleStock" || (parentWindow.GetType().Name == "DialogArticleCompositionSerialNumber") && pLabelText != "Artigo") || parentWindow.GetType().Name == "DialogArticleWarehouse" || pLabelText != "Número do Doc.")
            {
                //Parameters
                _sourceWindow = parentWindow;
                //Defaults
                Color colorBaseDialogEntryBoxBackground = "240, 240, 240".StringToColor();
                Color validLabel = AppSettings.Instance.colorEntryValidationValidFont;

                string fontLabel = "10";
                string fontEntry = "9";
                int padding = 2;

                this.ModifyBg(StateType.Normal, colorBaseDialogEntryBoxBackground.ToGdkColor());
                //this.BorderWidth = (uint)padding;
                //VBox
                _vbox = new VBox(false, padding);
                _vbox.BorderWidth = (uint)padding;
                //Label
                Pango.FontDescription fontDescriptionLabel = Pango.FontDescription.FromString(fontLabel);
                _label = new Label(pLabelText);
                _label.ModifyFont(fontDescriptionLabel);
                _label.SetAlignment(0, 2.5F);
                //Child Entrys
                _fontDescription = Pango.FontDescription.FromString(fontEntry);
                //HBox
                _hbox = new HBox(false, padding);
                //Pack
                _vbox.PackStart(_label, false, false, 0);
                _vbox.PackStart(_hbox, true, true, 0);
                //Finish
                Add(_vbox);
            }
            else
            {
                //Parameters
                _sourceWindow = parentWindow;
                //Defaults
                Color colorBaseDialogEntryBoxBackground = "240, 240, 240".StringToColor();
                Color validLabel = AppSettings.Instance.colorEntryValidationValidFont;

                string fontLabel = "10";
                string fontEntry = "9";
                int padding = 2;
                

                this.ModifyBg(StateType.Normal, colorBaseDialogEntryBoxBackground.ToGdkColor());
                //this.BorderWidth = (uint)padding;
                //VBox
                _vbox = new VBox(false, padding);
                _vbox.BorderWidth = (uint)padding;
                //Label
                Pango.FontDescription fontDescriptionLabel = Pango.FontDescription.FromString(fontLabel);
                _label = new Label(pLabelText);
                _label.ModifyFont(fontDescriptionLabel);
                _label.SetAlignment(0, 2.5F);
                _label2 = new Label(GeneralUtils.GetResourceByName("global_article_code") + "   ");
                _label2.ModifyFont(fontDescriptionLabel);
                _label2.ModifyBg(StateType.Normal, validLabel.ToGdkColor());
                _label2.SetAlignment(0, 0.5F);
                _label3 = new Label("                                                         " + GeneralUtils.GetResourceByName("pos_ticketlist_label_quantity"));
                _label3.ModifyFont(fontDescriptionLabel);
                _label3.ModifyBg(StateType.Normal, validLabel.ToGdkColor());
                _label3.SetAlignment(0, 0.5F);
                //Child Entrys
                _fontDescription = Pango.FontDescription.FromString(fontEntry);
                //HBox
                HBox _hbox1 = new HBox(false, padding);
                _hbox = new HBox(false, padding);
                //Pack
                _hbox1.PackStart(_label2, false, false, 0);
                _hbox1.PackStart(_label, false, false, 0);
                _hbox1.PackStart(_label3, false, false, 0);
                _vbox.PackStart(_hbox1, false, false, 0);
                _vbox.PackStart(_hbox, true, true, 0);
                //Finish
                Add(_vbox);
            }

        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Buttons

        public IconButton GetButton(string pFileNameIcon)
        {
            return GetButton("Unknown", pFileNameIcon);
        }

        public IconButton GetButton(string pObjectName, string pFileNameIcon)
        {
            return new IconButton(
                new ButtonSettings
                {
                    Name = "buttonUserId",
                    Icon = pFileNameIcon,
                    IconSize = new Size(20, 20),
                    ButtonSize = new Size(30, 30)
                });
        }

        public IconButton AddButton(string pObjectName, string pFileNameIcon)
        {
            string icon = string.Format("{0}{1}", PathsSettings.ImagesFolderLocation, pFileNameIcon);
            IconButton result = GetButton(pObjectName, icon);
            _hbox.PackStart(result, false, false, 0);
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Keyboard

        public void InitKeyboard(object pBoxObject)
        {
            KeyboardMode keyboardMode = KeyboardMode.None;

            if (pBoxObject.GetType() == typeof(ValidatableTextBox))
            {
                keyboardMode = (pBoxObject as ValidatableTextBox).KeyboardMode;
            }
            else if (pBoxObject.GetType() == typeof(MultilineTextBox))
            {
                keyboardMode = (pBoxObject as MultilineTextBox).KeyboardMode;
            }

            //Prepare KeyBoard
            if (keyboardMode != KeyboardMode.None)
            {
                string iconKeyboard = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_keyboard.png"}";
                _buttonKeyBoard = GetButton(iconKeyboard);
                _hbox.PackStart(_buttonKeyBoard, false, false, 0);
                _buttonKeyBoard.Clicked += delegate
                {
                    CallKeyboard(pBoxObject);
                };
                //Required to assign true, used only with custom buttons, when we inited widget without Keyboard, and Init it after, first time is assigned false
                if (pBoxObject.GetType() == typeof(ValidatableTextBox))
                {
                    (pBoxObject as ValidatableTextBox).IsEditable = true;
                }
                else if (pBoxObject.GetType() == typeof(MultilineTextBox))
                {
                    (pBoxObject as MultilineTextBox).TextView.Editable = true;
                }
            }
            //Make input Text ReadOnly
            else
            {
                if (pBoxObject.GetType() == typeof(ValidatableTextBox))
                {
                    (pBoxObject as ValidatableTextBox).IsEditable = false;
                }
                else if (pBoxObject.GetType() == typeof(MultilineTextBox))
                {
                    (pBoxObject as MultilineTextBox).TextView.Editable = false;
                }
            }
        }

        public void CallKeyboard(object pBoxObject)
        {
            KeyboardMode keyboardMode = KeyboardMode.None;
            string text = string.Empty;
            string rule = string.Empty;
            int position;

            if (pBoxObject.GetType() == typeof(ValidatableTextBox))
            {
                keyboardMode = (pBoxObject as ValidatableTextBox).KeyboardMode;
                text = (pBoxObject as ValidatableTextBox).Text;
                rule = (pBoxObject as ValidatableTextBox).Rule;
                position = (pBoxObject as ValidatableTextBox).Position;
            }
            else if (pBoxObject.GetType() == typeof(MultilineTextBox))
            {
                keyboardMode = (pBoxObject as MultilineTextBox).KeyboardMode;
                text = (pBoxObject as MultilineTextBox).TextView.Buffer.Text;
                position = (pBoxObject as MultilineTextBox).TextView.Buffer.CursorPosition;
            }

            if (keyboardMode == KeyboardMode.AlfaNumeric || keyboardMode == KeyboardMode.Alfa || keyboardMode == KeyboardMode.Numeric)
            {
                string input = logicpos.Utils.GetVirtualKeyBoardInput(_sourceWindow, keyboardMode, text, rule);

                if (input != null)
                {
                    if (pBoxObject.GetType() == typeof(ValidatableTextBox))
                    {
                        (pBoxObject as ValidatableTextBox).Text = input;
                        (pBoxObject as ValidatableTextBox).GrabFocus();
                        int end = (pBoxObject as ValidatableTextBox).Text.Length;
                        (pBoxObject as ValidatableTextBox).SelectRegion(end, end);
                    }
                    else if (pBoxObject.GetType() == typeof(MultilineTextBox))
                    {
                        (pBoxObject as MultilineTextBox).TextView.Buffer.Text = input;
                        (pBoxObject as MultilineTextBox).TextView.GrabFocus();
                    }
                }
            }
            else if (keyboardMode == KeyboardMode.Money)
            {
                InsertMoneyModal dialog = new InsertMoneyModal(_sourceWindow, DialogFlags.DestroyWithParent, decimal.Parse(text));
                int response = dialog.Run();
                if (response == (int)ResponseType.Ok)
                {
                    string input = DataConversionUtils.DecimalToString(dialog.Amount);
                    if (input != null)
                    {
                        if (pBoxObject.GetType() == typeof(ValidatableTextBox))
                        {
                            (pBoxObject as ValidatableTextBox).Text = input;
                            (pBoxObject as ValidatableTextBox).GrabFocus();
                        }
                        else if (pBoxObject.GetType() == typeof(MultilineTextBox))
                        {
                            (pBoxObject as MultilineTextBox).TextView.Buffer.Text = input;
                            (pBoxObject as MultilineTextBox).TextView.GrabFocus();
                        }
                    }
                }
                dialog.Destroy();
            }
            //Always position cursor in End
            position = text.Length;
        }
    }
}
