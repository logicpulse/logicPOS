using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class NumberPad : Box
    {
        //Private Members
        private Table _table;
        public Table Table
        {
            get { return _table; }
            set { _table = value; }
        }

        //Public Properties
        private EntryValidation _textEntry;
        public EntryValidation TextEntry
        {
            get { return _textEntry; }
            set { _textEntry = value; }
        }

        //Public EventHandlers
        public event EventHandler Clicked;

        public NumberPad(String name, Color color, String font, byte buttonWidth, byte buttonHeight, byte padding = 0)
        {
            this.Name = name;

            Color colorFont = Color.White;
            char decimalSeparator = (char)GlobalFramework.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

            EventBox eventbox = new EventBox() { VisibleWindow = false };
            _table = new Table(4, 3, true);
            _table.Homogeneous = true;

            TouchButtonText buttonKey1 = new TouchButtonText("touchButtonKey1_DarkGrey", color, "1", font, colorFont, buttonWidth, buttonHeight);
            TouchButtonText buttonKey2 = new TouchButtonText("touchButtonKey2_DarkGrey", color, "2", font, colorFont, buttonWidth, buttonHeight);
            TouchButtonText buttonKey3 = new TouchButtonText("touchButtonKey3_DarkGrey", color, "3", font, colorFont, buttonWidth, buttonHeight);
            TouchButtonText buttonKey4 = new TouchButtonText("touchButtonKey4_DarkGrey", color, "4", font, colorFont, buttonWidth, buttonHeight);
            TouchButtonText buttonKey5 = new TouchButtonText("touchButtonKey5_DarkGrey", color, "5", font, colorFont, buttonWidth, buttonHeight);
            TouchButtonText buttonKey6 = new TouchButtonText("touchButtonKey6_DarkGrey", color, "6", font, colorFont, buttonWidth, buttonHeight);
            TouchButtonText buttonKey7 = new TouchButtonText("touchButtonKey7_DarkGrey", color, "7", font, colorFont, buttonWidth, buttonHeight);
            TouchButtonText buttonKey8 = new TouchButtonText("touchButtonKey8_DarkGrey", color, "8", font, colorFont, buttonWidth, buttonHeight);
            TouchButtonText buttonKey9 = new TouchButtonText("touchButtonKey9_DarkGrey", color, "9", font, colorFont, buttonWidth, buttonHeight);
            TouchButtonText buttonKey0 = new TouchButtonText("touchButtonKey0_DarkGrey", color, "0", font, colorFont, buttonWidth, buttonHeight);
            TouchButtonText buttonKeyDot = new TouchButtonText("touchButtonKeyDOT_DarkGrey", color, decimalSeparator.ToString(), font, colorFont, buttonWidth, buttonHeight);
            TouchButtonText buttonKeyCE = new TouchButtonText("touchButtonKeyCE_DarkGrey", color, "CE", font, colorFont, buttonWidth, buttonHeight);

            //prepare _table
            //row0
            _table.Attach(buttonKey7, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKey8, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKey9, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //row1
            _table.Attach(buttonKey4, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKey5, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKey6, 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //row2
            _table.Attach(buttonKey1, 0, 1, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKey2, 1, 2, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKey3, 2, 3, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //row3
            _table.Attach(buttonKey0, 0, 1, 3, 4, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKeyDot, 1, 2, 3, 4, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            _table.Attach(buttonKeyCE, 2, 3, 3, 4, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            //PackIt
            eventbox.Add(_table);
            this.Add(eventbox);

            //Events
            buttonKey1.Clicked += buttonKey_Clicked;
            buttonKey2.Clicked += buttonKey_Clicked;
            buttonKey3.Clicked += buttonKey_Clicked;
            buttonKey4.Clicked += buttonKey_Clicked;
            buttonKey5.Clicked += buttonKey_Clicked;
            buttonKey6.Clicked += buttonKey_Clicked;
            buttonKey7.Clicked += buttonKey_Clicked;
            buttonKey8.Clicked += buttonKey_Clicked;
            buttonKey9.Clicked += buttonKey_Clicked;
            buttonKey0.Clicked += buttonKey_Clicked;
            buttonKeyDot.Clicked += buttonKey_Clicked;
            buttonKeyCE.Clicked += buttonKey_Clicked;
        }

        void buttonKey_Clicked(object sender, EventArgs e)
        {
            if (Clicked != null) Clicked(sender, e);
        }
    }
}