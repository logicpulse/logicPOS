using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using Serilog;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{

    internal class KeyBoardPad : Box
    {
        //Public Properties
        public PosKeyboardDialog ParentDialog { get; set; }
        public KeyboardMode KeyboardMode { get; set; } = KeyboardMode.AlfaNumeric;

        public ValidatableTextBox TextEntry { get; set; }

        //Private Members
        private readonly string _fontKeyboardPadTextEntry = AppSettings.Instance.FontKeyboardPadTextEntry;
        private readonly VirtualKeyBoard _virtualKeyBoard;
        private readonly int _spacing = 10;
        private bool _isCapsEnabled = false;
        private string _activeDiacritical;
        private ModifierKeys _activeModifierKey = ModifierKeys.None;
        private VBox _vboxKeyboardRows;
        private VBox _vboxNumPadRows;

        //Constructor
        public KeyBoardPad(string pFile)
        {
            //ParseXML into VirtualKeyBoard Object
            _virtualKeyBoard = new VirtualKeyBoard(pFile);

            //Transform VirtualKeyBoard Object into UI VBox of Rows
            VBox keyboard = InitVirtualKeyboard(_virtualKeyBoard);

            PackStart(keyboard);

            //Update after Allocated
            _vboxNumPadRows.SizeAllocated += delegate { UpdateKeyboardMode(); };
        }

        //Initialize UI Keyboard from VirtualKeyboard
        private VBox InitVirtualKeyboard(VirtualKeyBoard pVirtualKeyboard)
        {
            List<VirtualKey> currentKeyboardRow;
            VirtualKey currentKey;//Virtual Key
            KeyboardPadKey keyboardPadKey;//UI GTK Key

            //Init Lists
            List<HBox> hboxKeyBoard = new List<HBox>();
            List<HBox> hboxNumPad = new List<HBox>();
            //Init VBoxs to strore Rows
            _vboxKeyboardRows = new VBox(true, 0);
            _vboxNumPadRows = new VBox(true, 0);

            //loop rows
            for (int i = 0; i < pVirtualKeyboard.KeyBoard.Count; i++)
            {
                //Get current VirtualKeyboard Row to Work
                currentKeyboardRow = pVirtualKeyboard.KeyBoard[i];

                //add new Hbox to hboxKeyBoard/hboxNumPad rows List
                hboxKeyBoard.Add(new HBox(false, 0));
                hboxNumPad.Add(new HBox(false, 0));

                //loop columns in a row
                for (int j = 0; j < currentKeyboardRow.Count; j++)
                {
                    //Debug
                    //Log.Debug(string.Format("InitVirtualKeyboard(): tmpKey{0}:{1}:{2}", i, j, currentKey.L1.Glyph));

                    currentKey = currentKeyboardRow[j];

                    //Create UI Key
                    keyboardPadKey = new KeyboardPadKey(currentKey);
                    keyboardPadKey.Clicked += keyboardPadKey_Clicked;
                    //Assign its UI reference to VirtualKey, usefull to have access to UI in VirtualKeyboard.VirtualKey
                    currentKey.UIKey = keyboardPadKey;

                    //If is a IsNumPad L1 key add to IsNumPad
                    if (currentKey.L1.IsNumPad)
                    {
                        hboxNumPad[i].PackStart(keyboardPadKey, false, false, 0);
                    }
                    //Else Add to KeyBoard
                    else
                    {
                        hboxKeyBoard[i].PackStart(keyboardPadKey, false, false, 0);
                    };
                }
                //In the end add row to Vbox
                _vboxKeyboardRows.PackStart(hboxKeyBoard[i]);
                _vboxNumPadRows.PackStart(hboxNumPad[i]);
            }

            //Pack KeyBoard and NumberPad into hboxResultReyboard
            HBox hboxResultReyboard = new HBox(false, 0);
            hboxResultReyboard.Spacing = _spacing;
            hboxResultReyboard.PackStart(_vboxKeyboardRows, false, false, 0);
            hboxResultReyboard.PackStart(_vboxNumPadRows, false, false, 0);
            //Init _textEntry
            Pango.FontDescription fontDescriptiontextEntry = Pango.FontDescription.FromString(_fontKeyboardPadTextEntry);
            TextEntry = new ValidatableTextBox();
            TextEntry.ModifyFont(fontDescriptiontextEntry);
            //Change Selected Text, when looses entry focus
            TextEntry.ModifyBase(StateType.Active, Color.Gray.ToGdkColor());
            //Final Pack KeyBoard + TextEntry
            VBox vboxResult = new VBox(false, _spacing);
            vboxResult.PackStart(TextEntry);
            vboxResult.PackStart(hboxResultReyboard);

            //Events
            this.KeyReleaseEvent += KeyBoardPad_KeyReleaseEvent;

            //Add Space arround Keyboards
            return vboxResult;
        }

        private void KeyBoardPad_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key.ToString().Equals("Return"))
            {
                //KeyboardKey vKey = (KeyboardKey)args.Event.Key;
                if (TextEntry.Validated)
                {
                    ParentDialog.Respond(ResponseType.Ok);
                }
                else
                {
                    CustomAlerts.Error(BackOfficeWindow.Instance)
                                .WithSize(new Size(500, 340))
                                .WithTitleResource("global_error")
                                .WithMessage(GeneralUtils.GetResourceByName("dialog_message_field_validation_error_keyboardpad"))
                                .ShowAlert();
                }
            }
        }

        //Used to Update Keyboard and Enable/Disable Keys
        private void UpdateKeyboard()
        {
            List<VirtualKey> currentKeyboardRow;
            VirtualKey currentKey;
            char charKey;
            Color _colorKeyboardPadKeyDefaultFont = AppSettings.Instance.ColorKeyboardPadKeyDefaultFont;
            Color _colorKeyboardPadKeySecondaryFont = AppSettings.Instance.ColorKeyboardPadKeySecondaryFont;

            //loop rows
            for (int i = 0; i < _virtualKeyBoard.KeyBoard.Count; i++)
            {
                //Get current VirtualKeyboard Row to Work
                currentKeyboardRow = _virtualKeyBoard.KeyBoard[i];

                //loop columns in a row
                for (int j = 0; j < currentKeyboardRow.Count; j++)
                {
                    //Get currentKey
                    currentKey = currentKeyboardRow[j];

                    switch (_activeModifierKey)
                    {
                        case ModifierKeys.None:
                            //Activate/Desactivate Keys
                            currentKey.UIKey.Active = false;
                            currentKey.UIKey.Sensitive = true;
                            //Always Redraw normal state L1
                            if (currentKey.UIKey.LabelL1 != null && currentKey.L1 != null && currentKey.L1.Glyph != null)
                            {
                                currentKey.UIKey.LabelL1.Text = currentKey.L1.Glyph;
                            }
                            //Always Redraw normal state L2
                            if (currentKey.UIKey.LabelL2 != null && currentKey.L2 != null && currentKey.L2.Glyph != null)
                            {
                                currentKey.UIKey.LabelL2.Text = currentKey.L2.Glyph;
                            }
                            //Bypass Normal with Uppercase Letters, when Caps Enabled and Key only have UI Label1
                            if (_isCapsEnabled && currentKey.L1.UnicodeId != null && currentKey.L2 != null && currentKey.L2.Glyph != null)
                            {
                                charKey = logicpos.Utils.UnicodeHexadecimalStringToChar(currentKey.L1.UnicodeId);
                                if (logicpos.Utils.IsLetter(charKey) && currentKey.UIKey.LabelL2 == null)
                                {
                                    currentKey.UIKey.LabelL1.Text = currentKey.L2.Glyph;
                                };
                            };
                            break;
                        case ModifierKeys.Shift:
                            //Activate/Desactivate Keys
                            if (currentKey.Type == "shift") { currentKey.UIKey.Active = true; } else { if (currentKey.UIKey.Active) currentKey.UIKey.Active = false; };
                            //Show Uppercase Letters, when Caps Enabled and Key only have UI Label1
                            if (currentKey.L1.UnicodeId != null && currentKey.L2 != null && currentKey.L2.Glyph != null)
                            {
                                charKey = logicpos.Utils.UnicodeHexadecimalStringToChar(currentKey.L1.UnicodeId);
                                if (logicpos.Utils.IsLetter(charKey) && currentKey.UIKey.LabelL2 == null) currentKey.UIKey.LabelL1.Text = currentKey.L2.Glyph;
                            }
                            break;
                        case ModifierKeys.Alt:
                            //Activate/Desactivate Keys
                            if (currentKey.Type == "alt") { currentKey.UIKey.Active = true; } else { if (currentKey.UIKey.Active) currentKey.UIKey.Active = false; };
                            if (currentKey.L3 != null)
                            {
                                currentKey.UIKey.LabelL1.Text = currentKey.L3.Glyph;
                                if (currentKey.UIKey.LabelL2 != null) currentKey.UIKey.LabelL2.Text = "";
                            }
                            else
                            {
                                //Disable Non Unicode Keys and Modifier Keys, Except Alt
                                if (currentKey.L1.UnicodeId != null || currentKey.Type == "caps" || currentKey.Type == "shift" || currentKey.Type == "ctrl")
                                {
                                    if (currentKey.UIKey.LabelL1 != null) currentKey.UIKey.LabelL1.Text = "";
                                    if (currentKey.UIKey.LabelL2 != null) currentKey.UIKey.LabelL2.Text = "";
                                    currentKey.UIKey.Sensitive = false;
                                }
                            }
                            break;
                        case ModifierKeys.Ctrl:
                            //Activate/Desactivate Keys
                            if (currentKey.Type == "ctrl") { currentKey.UIKey.Active = true; } else { if (currentKey.UIKey.Active) currentKey.UIKey.Active = false; };
                            //Disable Non Unicode Keys and Modifier Keys, Except Ctrl
                            if (currentKey.L1.UnicodeId != null || currentKey.Type == "caps" || currentKey.Type == "shift" || currentKey.Type == "alt")
                            {
                                if (currentKey.L1.Glyph != "v")
                                {
                                    if (currentKey.UIKey.LabelL1 != null) currentKey.UIKey.LabelL1.Text = "";
                                    if (currentKey.UIKey.LabelL2 != null) currentKey.UIKey.LabelL2.Text = "";
                                    currentKey.UIKey.Sensitive = false;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    //Always Enable/Disable Caps, After Process Modifier Keys, to Keep Caps Active State
                    if (currentKey.Type == "caps")
                    {
                        if (_isCapsEnabled) { currentKey.UIKey.Active = true; } else { currentKey.UIKey.Active = false; };
                    };

                    //Always Update L2 Keys If ModifierKey is Shift
                    if (currentKey.UIKey.LabelL2 != null)
                    {
                        if (_activeModifierKey != ModifierKeys.Shift)
                        {
                            //ExChange Font Color, Highlight L1
                            currentKey.UIKey.LabelL1.ModifyFg(StateType.Normal, _colorKeyboardPadKeyDefaultFont.ToGdkColor());
                            currentKey.UIKey.LabelL2.ModifyFg(StateType.Normal, _colorKeyboardPadKeySecondaryFont.ToGdkColor());
                        }
                        else
                        {
                            //ExChange Font Color, Highlight L2
                            currentKey.UIKey.LabelL1.ModifyFg(StateType.Normal, _colorKeyboardPadKeySecondaryFont.ToGdkColor());
                            currentKey.UIKey.LabelL2.ModifyFg(StateType.Normal, _colorKeyboardPadKeyDefaultFont.ToGdkColor());
                        };
                    };
                }
            }
        }

        //Update Keyboard Mode Default, Alfa, Numeric
        private void UpdateKeyboardMode()
        {
            switch (KeyboardMode)
            {
                case KeyboardMode.Alfa:
                    if (ParentDialog != null) ParentDialog.WidthRequest -= _vboxNumPadRows.Allocation.Width + _spacing;
                    _vboxNumPadRows.HideAll();
                    break;
                case KeyboardMode.Numeric:
                    if (ParentDialog != null) ParentDialog.WidthRequest -= _vboxKeyboardRows.Allocation.Width + _spacing;
                    _vboxKeyboardRows.HideAll();
                    break;
            }

            //Hide Numeric KeyPad if in 800x600
            //TODO:THEME
            if (KeyboardMode.Equals(KeyboardMode.AlfaNumeric) && AppSettings.Instance.AppScreenSize.Width == 800 && AppSettings.Instance.AppScreenSize.Height == 600)
            {
                if (ParentDialog != null) ParentDialog.WidthRequest -= _vboxNumPadRows.Allocation.Width + _spacing;
                _vboxNumPadRows.HideAll();
            }
        }

        //Process Keyboard Inputs
        private void keyboardPadKey_Clicked(object sender, EventArgs e)
        {
            KeyboardPadKey vKey = (KeyboardPadKey)sender;
            char _unicodeChar;
            bool _requireUpdate = false;
            bool _skipInsert = false;
            int _tempCursorPosition;
            string _stringChar;

            int selectionStart, selectionEnd;
            TextEntry.GetSelectionBounds(out selectionStart, out selectionEnd);
            if (selectionStart > 0 || selectionEnd > 0) TextEntry.DeleteSelection();

            VirtualKeyProperties vKeyProperties;
            //Get Level and Assign Level Properties to current vKeyProperties
            switch (_activeModifierKey)
            {
                case ModifierKeys.Shift:
                    vKeyProperties = vKey.Properties.L2;
                    break;
                case ModifierKeys.Alt:
                    vKeyProperties = vKey.Properties.L3;
                    break;
                default:
                    vKeyProperties = vKey.Properties.L1;
                    break;
            }

            //DeadKey
            if (vKeyProperties != null && vKeyProperties.IsDeadKey)
            {
                _requireUpdate = true;
                _skipInsert = true;
                _activeDiacritical = vKeyProperties.Diacritical;
            };

            //Process Keys, Modifiers
            switch (vKey.Properties.Type)
            {
                //Sticky Caps Lock
                case "caps":
                    _requireUpdate = true;
                    if (_isCapsEnabled == false) { _isCapsEnabled = true; } else { _isCapsEnabled = false; }
                    break;
                //Modifier Shift
                case "shift":
                    _requireUpdate = true;
                    if (_activeModifierKey == ModifierKeys.Shift) { _activeModifierKey = ModifierKeys.None; } else { _activeModifierKey = ModifierKeys.Shift; };
                    break;
                //Modifier Alt
                case "alt":
                    _requireUpdate = true;
                    if (_activeModifierKey == ModifierKeys.Alt) { _activeModifierKey = ModifierKeys.None; } else { _activeModifierKey = ModifierKeys.Alt; };
                    break;
                //Modifier Shift
                case "ctrl":
                    _requireUpdate = true;
                    if (_activeModifierKey == ModifierKeys.Ctrl) { _activeModifierKey = ModifierKeys.None; } else { _activeModifierKey = ModifierKeys.Ctrl; };
                    break;
                //Modal Cancel
                case "esc":
                    ParentDialog.Respond(ResponseType.Cancel);
                    break;
                //Modal Confirm
                case "enter":
                    if (TextEntry.Validated)
                    {
                        // This Will Crash only in Debug, if Run Outside it Wont Crash (Simply disappear without log error)
                        try
                        {
                            ParentDialog.Respond(ResponseType.Ok);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex,"Exception");
                        }
                    }
                    else
                    {
                        CustomAlerts.Error(BackOfficeWindow.Instance)
                                    .WithSize(new Size(500, 340))
                                    .WithTitleResource("global_error")
                                    .WithMessage(GeneralUtils.GetResourceByName("dialog_message_field_validation_error_keyboardpad"))
                                    .ShowAlert();
                    };
                    break;
                //Show/Hide Number Lock
                case "tab":
                    if (KeyboardMode == KeyboardMode.AlfaNumeric)
                    {
                        if (_vboxNumPadRows.Visible)
                        {
                            ParentDialog.WidthRequest -= _vboxNumPadRows.Allocation.Width + _spacing;
                            _vboxNumPadRows.HideAll();
                        }
                        else
                        {
                            ParentDialog.WidthRequest += _vboxNumPadRows.Allocation.Width + _spacing;
                            _vboxNumPadRows.ShowAll();
                        };
                    }
                    break;
                //Enable/Disable Internal Keyboard
                case "ekey":
                    break;
                //Delete
                case "back":
                    TextEntry.DeleteText(TextEntry.Position - 1, TextEntry.Position);
                    break;
                //Cursor Move to Start
                case "home":
                    TextEntry.Position = 0;
                    break;
                //Cursor Move to End
                case "end":
                    TextEntry.Position = TextEntry.Text.Length;
                    break;
                //Does Nothing
                case "up":
                    break;
                //Does Nothing
                case "down":
                    break;
                //Move Cursor to Left
                case "left":
                    if (TextEntry.Position > 0) TextEntry.Position -= 1;
                    break;
                //Move Cursor to Right
                case "right":
                    if (TextEntry.Position < TextEntry.Text.Length) TextEntry.Position += 1;
                    break;
                //All Other Keys
                default:
                    //NumberPad always work only with L1
                    if (vKey.Properties.L1.IsNumPad == true || vKey.Properties.Type == "space")
                    {
                        vKeyProperties = vKey.Properties.L1;
                    }
                    //If Caps enabled and is Letter, change to Level 2 (Uppercase)
                    else if (_isCapsEnabled && logicpos.Utils.IsLetter(logicpos.Utils.UnicodeHexadecimalStringToChar(vKeyProperties.UnicodeId)))
                    {
                        vKeyProperties = vKey.Properties.L2;
                    };
                    //Get unicodeChar from UnicodeId after Caps
                    _unicodeChar = logicpos.Utils.UnicodeHexadecimalStringToChar(vKeyProperties.UnicodeId);
                    //Modifie _unicodeChar Keys ex Culture Decimal Separator . with , [3,15 = Key NumberPad .]
                    //if (vKey.Properties.RowIndex == 3 && vKey.Properties.ColIndex == 15) _unicodeChar = (char) LogicPOS.Settings.CultureSettings.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
                    //Always Disable Modifiers Keys After Use
                    if (_activeModifierKey != ModifierKeys.None)
                    {
                        _requireUpdate = true;
                        //Process Modifiers aBefore Disable Modifier
                        switch (_activeModifierKey)
                        {
                            case ModifierKeys.Ctrl:
                                switch (Convert.ToString(_unicodeChar).ToUpper())
                                {
                                    case "X":
                                        TextEntry.CutClipboard();
                                        _skipInsert = true;
                                        break;
                                    case "C":
                                        TextEntry.CopyClipboard();
                                        _skipInsert = true;
                                        break;
                                    case "V":
                                        TextEntry.PasteClipboard();
                                        _skipInsert = true;
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        //After Process Modifiers, restore default Non Modifiers State
                        _activeModifierKey = ModifierKeys.None;
                    };

                    //Debug
                    //Log.Debug(string.Format("keyboardKey_Clicked(): L1.Glyph:[{1}] L1.UnicodeId:[{2}] L1.CharacterName:[{3}] unicodeChar[{4}]", vKey.Properties.Type, vKeyProperties.Glyph, vKeyProperties.UnicodeId, vKeyProperties.CharacterName, _unicodeChar));

                    //Add to TextEntry
                    _tempCursorPosition = TextEntry.Position;

                    if (!_skipInsert)
                    {
                        _stringChar = Convert.ToString(_unicodeChar);
                        //Diacritical
                        if (_activeDiacritical != null)
                        {
                            _stringChar += Convert.ToString(logicpos.Utils.UnicodeHexadecimalStringToChar(_activeDiacritical));
                            //Convert Diacritial chars to ISO chars to work with Validations and Peristence Database
                            _stringChar = ReplaceDiacritial(_stringChar);
                            //Reset activeDiacritical
                            _activeDiacritical = null;
                        }
                        TextEntry.InsertText(_stringChar, ref _tempCursorPosition);
                        TextEntry.Text.Normalize();
                    };
                    TextEntry.Position = _tempCursorPosition;
                    break;
            }

            //HACK: to Activate TextEntry and place Cursor
            _tempCursorPosition = TextEntry.Position;
            TextEntry.GrabFocus();
            TextEntry.Position = _tempCursorPosition;

            //Update Keyboard if ModifierKey has Changed
            if (_requireUpdate) UpdateKeyboard();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static

        public static string ReplaceDiacritial(string pInput)
        {
            string result = pInput;
            Dictionary<string, string> replace = new Dictionary<string, string>
            {
                //Important this chars are not equal, ex ("ã", "ã") in real is ("a~", "ã"), this is the trick to a good replacement

                // ´ : From VirtualKeyboard > ÁÉÍÓÚ áéíóú
                { "Á", "Á" },//A´
                { "É", "É" },//E´
                { "Í", "Í" },//I´
                { "Ó", "Ó" },//O´
                { "Ú", "Ú" },//U´
                { "á", "á" },//a´
                { "é", "é" },//e´
                { "í", "í" },//i´
                { "ó", "ó" },//o´
                { "ú", "ú" },//u´
                              //  : From VirtualKeyboard > ÀÈÌÒÙ àèìòù
                { "À", "À" },//A
                { "È", "È" },//E
                { "Ì", "Ì" },//I
                { "Ò", "Ò" },//O
                { "Ù", "Ù" },//U
                { "à", "à" },//a
                { "è", "è" },//e
                { "ì", "ì" },//i
                { "ò", "ò" },//o
                { "ù", "ù" },//u
                              // ^ : From VirtualKeyboard > ÂÊÎÔÛ âêîôû
                { "Â", "Â" },//A^
                { "Ê", "Ê" },//E^
                { "Î", "Î" },//I^
                { "Ô", "Ô" },//O^
                { "Û", "Û" },//U^
                { "â", "â" },//a^
                { "ê", "ê" },//e^
                { "î", "î" },//i^
                { "ô", "ô" },//o^
                { "û", "û" },//u^
                              // ~ : From VirtualKeyboard > ÃẼĨÕŨÑ ãẽĩõũñ
                { "Ã", "Ã" },//A~
                { "Ẽ", "E" },//E~
                { "Ĩ", "I" },//I~
                { "Õ", "Õ" },//O~
                { "Ũ", "U" },//U~
                { "Ñ", "Ñ" },//N~
                { "ã", "ã" },//a~
                { "ẽ", "e" },//e~
                { "ĩ", "i" },//i~
                { "õ", "õ" },//o~
                { "ũ", "u" },//u~
                { "ñ", "ñ" }//n~
            };

            foreach (var item in replace)
            {
                result = result.Replace(item.Key, item.Value);
            }

            return result;
        }
    }
}