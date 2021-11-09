using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    public abstract class EntryBoxBase : EventBox
    {
        //Log4Net
        protected static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
        public EntryBoxBase(String pLabelText)
            : this(null, pLabelText)
        {
        }
        protected TouchButtonIcon _buttonKeyBoard;
        public TouchButtonIcon ButtonKeyBoard
        {
            get { return _buttonKeyBoard; }
            set { _buttonKeyBoard = value; }
        }

        public EntryBoxBase(Window pSourceWindow, String pLabelText)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            //Defaults
            Color colorBaseDialogEntryBoxBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogEntryBoxBackground"]);
            String fontLabel = GlobalFramework.Settings["fontEntryBoxLabel"];
            String fontEntry = GlobalFramework.Settings["fontEntryBoxValue"];
            int padding = 2;
            //This
            this.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(colorBaseDialogEntryBoxBackground));
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
        public EntryBoxBase(Window pSourceWindow, String pLabelText, bool pBOsource=false)
        {
            if (!pBOsource)
            {
                //Parameters
                _sourceWindow = pSourceWindow;
                //Defaults
                Color colorBaseDialogEntryBoxBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogEntryBoxBackground"]);
                String fontLabel = GlobalFramework.Settings["fontEntryBoxLabel"];
                String fontEntry = GlobalFramework.Settings["fontEntryBoxValue"];
                int padding = 2;
                //This
                this.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(colorBaseDialogEntryBoxBackground));
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
            else if ((pSourceWindow.GetType().Name == "DialogArticleStockMoviment" || pSourceWindow.GetType().Name == "DialogAddArticleStock" || (pSourceWindow.GetType().Name == "DialogArticleCompositionSerialNumber") && pLabelText != "Artigo") || pSourceWindow.GetType().Name == "DialogArticleWarehouse" || pLabelText != "Número do Doc.")
            {
                //Parameters
                _sourceWindow = pSourceWindow;
                //Defaults
                Color colorBaseDialogEntryBoxBackground = FrameworkUtils.StringToColor("240, 240, 240");
                Color validLabel = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationValidFont"]);

                String fontLabel = "10";
                String fontEntry = "9";
                int padding = 2;
                if (pSourceWindow.GetType() == typeof(PosArticleStockDialog))
                {
                    colorBaseDialogEntryBoxBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogEntryBoxBackground"]);
                    fontLabel = GlobalFramework.Settings["fontEntryBoxLabel"];
                    fontEntry = GlobalFramework.Settings["fontEntryBoxValue"];
                }

                this.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(colorBaseDialogEntryBoxBackground));
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
                _sourceWindow = pSourceWindow;
                //Defaults
                Color colorBaseDialogEntryBoxBackground = FrameworkUtils.StringToColor("240, 240, 240");
                Color validLabel = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationValidFont"]);
     
                String fontLabel = "10";
                String fontEntry = "9";
                int padding = 2;
                if (pSourceWindow.GetType() == typeof(PosArticleStockDialog))
                {
                    colorBaseDialogEntryBoxBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogEntryBoxBackground"]);
                    fontLabel = GlobalFramework.Settings["fontEntryBoxLabel"];
                    fontEntry = GlobalFramework.Settings["fontEntryBoxValue"];
                }


                this.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(colorBaseDialogEntryBoxBackground));
                //this.BorderWidth = (uint)padding;
                //VBox
                _vbox = new VBox(false, padding);
                _vbox.BorderWidth = (uint)padding;
                //Label
                Pango.FontDescription fontDescriptionLabel = Pango.FontDescription.FromString(fontLabel);
                _label = new Label(pLabelText);
                _label.ModifyFont(fontDescriptionLabel);
                _label.SetAlignment(0, 2.5F);
                _label2 = new Label(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_article_code") + "   ");
                _label2.ModifyFont(fontDescriptionLabel);
                _label2.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(validLabel));
                _label2.SetAlignment(0, 0.5F);
                _label3 = new Label("                                                         " + resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "pos_ticketlist_label_quantity"));
                _label3.ModifyFont(fontDescriptionLabel);
                _label3.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(validLabel));
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

        public TouchButtonIcon GetButton(string pFileNameIcon)
        {
            return GetButton("Unknown", pFileNameIcon);
        }

        public TouchButtonIcon GetButton(string pObjectName, string pFileNameIcon)
        {
            return new TouchButtonIcon(string.Format("touchButton", pObjectName), Color.Transparent, pFileNameIcon, new Size(20, 20), 30, 30);
        }

        public TouchButtonIcon AddButton(string pObjectName, string pFileNameIcon)
        {
            string icon = FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], pFileNameIcon));
            TouchButtonIcon result = GetButton(pObjectName, icon);
            _hbox.PackStart(result, false, false, 0);
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Keyboard

        public void InitKeyboard(object pBoxObject)
        {
            KeyboardMode keyboardMode = KeyboardMode.None;

            if (pBoxObject.GetType() == typeof(EntryValidation))
            {
                keyboardMode = (pBoxObject as EntryValidation).KeyboardMode;
            }
            else if (pBoxObject.GetType() == typeof(EntryMultiline))
            {
                keyboardMode = (pBoxObject as EntryMultiline).KeyboardMode;
            }

            //Prepare KeyBoard
            if (keyboardMode != KeyboardMode.None)
            {
                string iconKeyboard = FrameworkUtils.OSSlash(string.Format("{0}{1}", GlobalFramework.Path["images"], @"Icons/Windows/icon_window_keyboard.png"));
                _buttonKeyBoard = GetButton(iconKeyboard);
                _hbox.PackStart(_buttonKeyBoard, false, false, 0);
                _buttonKeyBoard.Clicked += delegate
                {
                    CallKeyboard(pBoxObject);
                };
                //Required to assign true, used only with custom buttons, when we inited widget without Keyboard, and Init it after, first time is assigned false
                if (pBoxObject.GetType() == typeof(EntryValidation))
                {
                    (pBoxObject as EntryValidation).IsEditable = true;
                }
                else if (pBoxObject.GetType() == typeof(EntryMultiline))
                {
                    (pBoxObject as EntryMultiline).TextView.Editable = true;
                }
            }
            //Make input Text ReadOnly
            else
            {
                if (pBoxObject.GetType() == typeof(EntryValidation))
                {
                    (pBoxObject as EntryValidation).IsEditable = false;
                }
                else if (pBoxObject.GetType() == typeof(EntryMultiline))
                {
                    (pBoxObject as EntryMultiline).TextView.Editable = false;
                }
            }
        }

        public void CallKeyboard(object pBoxObject)
        {
            KeyboardMode keyboardMode = KeyboardMode.None;
            string text = string.Empty;
            string rule = string.Empty;
            int position;

            if (pBoxObject.GetType() == typeof(EntryValidation))
            {
                keyboardMode = (pBoxObject as EntryValidation).KeyboardMode;
                text = (pBoxObject as EntryValidation).Text;
                rule = (pBoxObject as EntryValidation).Rule;
                position = (pBoxObject as EntryValidation).Position;
            }
            else if (pBoxObject.GetType() == typeof(EntryMultiline))
            {
                keyboardMode = (pBoxObject as EntryMultiline).KeyboardMode;
                text = (pBoxObject as EntryMultiline).TextView.Buffer.Text;
                position = (pBoxObject as EntryMultiline).TextView.Buffer.CursorPosition;
            }

            if (keyboardMode == KeyboardMode.AlfaNumeric || keyboardMode == KeyboardMode.Alfa || keyboardMode == KeyboardMode.Numeric)
            {
                string input = Utils.GetVirtualKeyBoardInput(_sourceWindow, keyboardMode, text, rule);

                if (input != null)
                {
                    if (pBoxObject.GetType() == typeof(EntryValidation))
                    {
                        (pBoxObject as EntryValidation).Text = input;
                        (pBoxObject as EntryValidation).GrabFocus();
                        int end = (pBoxObject as EntryValidation).Text.Length;
                        (pBoxObject as EntryValidation).SelectRegion(end, end);
                    }
                    else if (pBoxObject.GetType() == typeof(EntryMultiline))
                    {
                        (pBoxObject as EntryMultiline).TextView.Buffer.Text = input;
                        (pBoxObject as EntryMultiline).TextView.GrabFocus();
                    }
                }
            }
            else if (keyboardMode == KeyboardMode.Money)
            {
                PosMoneyPadDialog dialog = new PosMoneyPadDialog(_sourceWindow, DialogFlags.DestroyWithParent, FrameworkUtils.StringToDecimal(text));
                int response = dialog.Run();
                if (response == (int)ResponseType.Ok)
                {
                    string input = FrameworkUtils.DecimalToString(dialog.Amount);
                    if (input != null) 
                    { 
                        if (pBoxObject.GetType() == typeof(EntryValidation))
                        {
                            (pBoxObject as EntryValidation).Text = input;
                            (pBoxObject as EntryValidation).GrabFocus();
                        }
                        else if (pBoxObject.GetType() == typeof(EntryMultiline))
                        {
                            (pBoxObject as EntryMultiline).TextView.Buffer.Text = input;
                            (pBoxObject as EntryMultiline).TextView.GrabFocus();
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
