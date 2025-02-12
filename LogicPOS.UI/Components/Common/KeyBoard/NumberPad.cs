using Gtk;
using LogicPOS.UI.Buttons;
using System;
using System.Drawing;
using System.Globalization;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class NumberPad : Box
    {
        public Table Table { get; set; }

        public ValidatableTextBox TextEntry { get; set; }

        //Public EventHandlers
        public event EventHandler Clicked;

        public NumberPad(string name, Color color, string font, byte buttonWidth, byte buttonHeight, byte padding = 0)
        {
            this.Name = name;

            Color colorFont = Color.White;
            char decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

            EventBox eventbox = new EventBox() { VisibleWindow = false };
            Table = new Table(4, 3, true);

            Table.Homogeneous = true;

            TextButton buttonKey1 = CreateNumberedButton("1");
            TextButton buttonKey2 = CreateNumberedButton("2");
            TextButton buttonKey3 = CreateNumberedButton("3");
            TextButton buttonKey4 = CreateNumberedButton("4");
            TextButton buttonKey5 = CreateNumberedButton("5");
            TextButton buttonKey6 = CreateNumberedButton("6");
            TextButton buttonKey7 = CreateNumberedButton("7");
            TextButton buttonKey8 = CreateNumberedButton("8");
            TextButton buttonKey9 = CreateNumberedButton("9");
            TextButton buttonKey0 = CreateNumberedButton("0");

            TextButton buttonKeyDot = new TextButton(
                new ButtonSettings
                {
                    Name = "touchButtonKeyDOT_DarkGrey",
                    BackgroundColor = color,
                    Text = decimalSeparator.ToString(),
                    Font = font,
                    FontColor = colorFont,
                    ButtonSize = new Size(buttonWidth, buttonHeight)
                });

            TextButton buttonKeyCE = CreateNumberedButton("CE");

            //prepare _table
            //row0
            Table.Attach(buttonKey7, 0, 1, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKey8, 1, 2, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKey9, 2, 3, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //row1
            Table.Attach(buttonKey4, 0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKey5, 1, 2, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKey6, 2, 3, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //row2
            Table.Attach(buttonKey1, 0, 1, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKey2, 1, 2, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKey3, 2, 3, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //row3
            Table.Attach(buttonKey0, 0, 1, 3, 4, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKeyDot, 1, 2, 3, 4, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKeyCE, 2, 3, 3, 4, AttachOptions.Fill, AttachOptions.Fill, padding, padding);

            //PackIt
            eventbox.Add(Table);
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

            TextButton CreateNumberedButton(string number)
            {
                return new TextButton(
                    new ButtonSettings
                    {
                        Name = $"touchButtonKey{number}_DarkGrey",
                        BackgroundColor = color,
                        Text = number,
                        Font = font,
                        FontColor = colorFont,
                        ButtonSize = new Size(buttonWidth, buttonHeight)
                    });
            }
        }

        private void buttonKey_Clicked(object sender, EventArgs e)
        {
            Clicked?.Invoke(sender, e);
        }
    }
}