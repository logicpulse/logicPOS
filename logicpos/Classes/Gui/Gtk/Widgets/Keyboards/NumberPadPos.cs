using System.Drawing;
using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using LogicPOS.Settings.Extensions;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class NumberPadPos : NumberPad
    {
        public NumberPadPos(string name, Color colorButton, Color colorRightButton, string fontButton, string fontRightButton, byte buttonWidth, byte rightButtonWidth, byte buttonHeight, byte padding = 0)
            : base(name, colorButton, fontButton, buttonWidth, buttonHeight, padding)
        {
            //Init Local Vars
            Size sizeIcon = new Size(28, 28);
            string icon1 = DataLayerFramework.Path["images"] + @"Icons\icon_pos_numberpad_1_splitaccount.png";
            string icon2 = DataLayerFramework.Path["images"] + @"Icons\icon_pos_numberpad_2_messages.png";
            string icon3 = DataLayerFramework.Path["images"] + @"Icons\icon_pos_numberpad_3_gifts.png";
            string icon4 = DataLayerFramework.Path["images"] + @"Icons\icon_pos_numberpad_4_weight.png";

            Color colorFont = Color.Black;

            Table.Name = name;
            Table.Homogeneous = false;
            Table.NColumns = 4;

            TouchButtonIconWithText buttonKeySplitAccount = new TouchButtonIconWithText("buttonKeySplitAccount", colorRightButton, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_split_account"), fontRightButton, colorFont, icon1, sizeIcon, rightButtonWidth, buttonHeight);
            TouchButtonIconWithText buttonKeyMessages = new TouchButtonIconWithText("buttonKeyMessages", colorRightButton, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_messages"), fontRightButton, colorFont, icon2, sizeIcon, rightButtonWidth, buttonHeight);
            TouchButtonIconWithText buttonKeyGifts = new TouchButtonIconWithText("buttonKeyGifts", colorRightButton, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_gifts"), fontRightButton, colorFont, icon3, sizeIcon, rightButtonWidth, buttonHeight);
            TouchButtonIconWithText buttonKeyWeight = new TouchButtonIconWithText("buttonKeyWeight", colorRightButton, resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_weight"), fontRightButton, colorFont, icon4, sizeIcon, rightButtonWidth, buttonHeight);
            buttonKeyMessages.Sensitive = false;
            buttonKeyGifts.Sensitive = false;

            //add column4 to base NumberPad
            Table.Attach(buttonKeySplitAccount, 3, 4, 0, 1, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKeyMessages, 3, 4, 1, 2, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKeyGifts, 3, 4, 2, 3, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            Table.Attach(buttonKeyWeight, 3, 4, 3, 4, AttachOptions.Fill, AttachOptions.Fill, padding, padding);
            //space between NumberPad and POS Keys
            Table.SetColSpacing(2, 6);
        }
    }
}
