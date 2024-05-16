using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Logic.Others;
using logicpos.Extensions;
using LogicPOS.Settings;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal partial class TicketPad : Box
    {
        //Private Members
        private readonly TicketList _ticketList;
        //UI
        private readonly TouchButtonIconWithText _buttonPrev;
        private readonly TouchButtonIconWithText _buttonNext;
        private readonly TouchButtonIconWithText _buttonDecrease;
        private readonly TouchButtonIconWithText _buttonIncrease;
        private readonly TouchButtonIconWithText _buttonDelete;
        private readonly TouchButtonIconWithText _buttonChangeQuantity;
        private readonly TouchButtonIconWithText _buttonChangePrice;
        private readonly TouchButtonIconWithText _buttonListMode;
        private readonly TouchButtonIconWithText _buttonListOrder;
        private readonly TouchButtonIconWithText _buttonSplitAccount;
        private readonly TouchButtonIconWithText _buttonMessages;
        private readonly TouchButtonIconWithText _buttonWeight;
        private readonly TouchButtonIconWithText _buttonGifts;
        private readonly TouchButtonIconWithText _buttonChangeTable;
        private readonly TouchButtonIconWithText _buttonFinishOrder;
        private readonly TouchButtonIconWithText _buttonPayments;
        private readonly TouchButtonIconWithText _buttonBarCode;
        //IN009279 Parking ticket Service - implementar Cartão cliente
        private readonly TouchButtonIconWithText _buttonCardCode;
        //private TouchButtonIconWithText _buttonSendTicket;
        //Public Properties
        public Window SourceWindow { get; set; }
        public TouchButtonIconWithText ButtonKeySelectTable { get; set; }

        public TicketPad(string pName, TicketList pTicketList, dynamic pThemeButtons, Point position)
        {
            //Init Parameters
            Name = pName;
            _ticketList = pTicketList;
            dynamic themeButtons = pThemeButtons;

            //Buttons Shared for all Buttons
            Size buttonsIconSize = logicpos.Utils.StringToSize(themeButtons.IconSize);
            string buttonsFont = themeButtons.Font;
            Color buttonsFontColor = (themeButtons.FontColor as string).StringToColor();

            //Buttons:ButtonPrev
            string buttonPrevName = themeButtons.ButtonPrev.Name;
            string buttonPrevText = themeButtons.ButtonPrev.Text;
            Point buttonPrevPosition = logicpos.Utils.StringToPosition(themeButtons.ButtonPrev.Position);
            Size buttonPrevSize = logicpos.Utils.StringToSize(themeButtons.ButtonPrev.Size);
            string buttonPrevImageFileName = themeButtons.ButtonPrev.ImageFileName;
            bool buttonPrevVisible = Convert.ToBoolean(themeButtons.ButtonPrev.Visible);

            //Buttons:ButtonNext
            string buttonNextName = themeButtons.ButtonNext.Name;
            string buttonNextText = themeButtons.ButtonNext.Text;
            Point buttonNextPosition = logicpos.Utils.StringToPosition(themeButtons.ButtonNext.Position);
            Size buttonNextSize = logicpos.Utils.StringToSize(themeButtons.ButtonNext.Size);
            string buttonNextImageFileName = themeButtons.ButtonNext.ImageFileName;
            bool buttonNextVisible = Convert.ToBoolean(themeButtons.ButtonNext.Visible);

            //Buttons:ButtonDecrease
            string buttonDecreaseName = themeButtons.ButtonDecrease.Name;
            string buttonDecreaseText = themeButtons.ButtonDecrease.Text;
            Point buttonDecreasePosition = logicpos.Utils.StringToPosition(themeButtons.ButtonDecrease.Position);
            Size buttonDecreaseSize = logicpos.Utils.StringToSize(themeButtons.ButtonDecrease.Size);
            string buttonDecreaseImageFileName = themeButtons.ButtonDecrease.ImageFileName;
            bool buttonDecreaseVisible = Convert.ToBoolean(themeButtons.ButtonDecrease.Visible);

            //Buttons:ButtonIncrease
            string buttonIncreaseName = themeButtons.ButtonIncrease.Name;
            string buttonIncreaseText = themeButtons.ButtonIncrease.Text;
            Point buttonIncreasePosition = logicpos.Utils.StringToPosition(themeButtons.ButtonIncrease.Position);
            Size buttonIncreaseSize = logicpos.Utils.StringToSize(themeButtons.ButtonIncrease.Size);
            string buttonIncreaseImageFileName = themeButtons.ButtonIncrease.ImageFileName;
            bool buttonIncreaseVisible = Convert.ToBoolean(themeButtons.ButtonIncrease.Visible);

            //Buttons:ButtonDelete
            string buttonDeleteName = themeButtons.ButtonDelete.Name;
            string buttonDeleteText = themeButtons.ButtonDelete.Text;
            Point buttonDeletePosition = logicpos.Utils.StringToPosition(themeButtons.ButtonDelete.Position);
            Size buttonDeleteSize = logicpos.Utils.StringToSize(themeButtons.ButtonDelete.Size);
            string buttonDeleteImageFileName = themeButtons.ButtonDelete.ImageFileName;
            bool buttonDeleteVisible = Convert.ToBoolean(themeButtons.ButtonDelete.Visible);

            //Buttons:ButtonChangeQuantity
            string buttonChangeQuantityName = themeButtons.ButtonChangeQuantity.Name;
            string buttonChangeQuantityText = themeButtons.ButtonChangeQuantity.Text;
            Point buttonChangeQuantityPosition = logicpos.Utils.StringToPosition(themeButtons.ButtonChangeQuantity.Position);
            Size buttonChangeQuantitySize = logicpos.Utils.StringToSize(themeButtons.ButtonChangeQuantity.Size);
            string buttonChangeQuantityImageFileName = themeButtons.ButtonChangeQuantity.ImageFileName;
            bool buttonChangeQuantityVisible = Convert.ToBoolean(themeButtons.ButtonChangeQuantity.Visible);

            //Buttons:ButtonChangePrice
            string buttonChangePriceName = themeButtons.ButtonChangePrice.Name;
            string buttonChangePriceText = themeButtons.ButtonChangePrice.Text;
            Point buttonChangePricePosition = logicpos.Utils.StringToPosition(themeButtons.ButtonChangePrice.Position);
            Size buttonChangePriceSize = logicpos.Utils.StringToSize(themeButtons.ButtonChangePrice.Size);
            string buttonChangePriceImageFileName = themeButtons.ButtonChangePrice.ImageFileName;
            bool buttonChangePriceVisible = Convert.ToBoolean(themeButtons.ButtonChangePrice.Visible);

            //Buttons:ButtonListMode
            string buttonListModeName = themeButtons.ButtonListMode.Name;
            string buttonListModeText = themeButtons.ButtonListMode.Text;
            Point buttonListModePosition = logicpos.Utils.StringToPosition(themeButtons.ButtonListMode.Position);
            Size buttonListModeSize = logicpos.Utils.StringToSize(themeButtons.ButtonListMode.Size);
            string buttonListModeImageFileName = themeButtons.ButtonListMode.ImageFileName;
            bool buttonListModeVisible = Convert.ToBoolean(themeButtons.ButtonListMode.Visible);

            //Buttons:ButtonListOrder
            string buttonListOrderName = themeButtons.ButtonListOrder.Name;
            string buttonListOrderText = themeButtons.ButtonListOrder.Text;
            Point buttonListOrderPosition = logicpos.Utils.StringToPosition(themeButtons.ButtonListOrder.Position);
            Size buttonListOrderSize = logicpos.Utils.StringToSize(themeButtons.ButtonListOrder.Size);
            string buttonListOrderImageFileName = themeButtons.ButtonListOrder.ImageFileName;
            bool buttonListOrderVisible = Convert.ToBoolean(themeButtons.ButtonListOrder.Visible);

            //Buttons:ButtonSplitAccount
            string buttonSplitAccountName = themeButtons.ButtonSplitAccount.Name;
            string buttonSplitAccountText = themeButtons.ButtonSplitAccount.Text;
            Point buttonSplitAccountPosition = logicpos.Utils.StringToPosition(themeButtons.ButtonSplitAccount.Position);
            Size buttonSplitAccountSize = logicpos.Utils.StringToSize(themeButtons.ButtonSplitAccount.Size);
            string buttonSplitAccountImageFileName = themeButtons.ButtonSplitAccount.ImageFileName;
            bool buttonSplitAccountVisible = Convert.ToBoolean(themeButtons.ButtonSplitAccount.Visible);

            //Buttons:ButtonMessages
            string buttonMessagesName = themeButtons.ButtonMessages.Name;
            string buttonMessagesText = themeButtons.ButtonMessages.Text;
            Point buttonMessagesPosition = logicpos.Utils.StringToPosition(themeButtons.ButtonMessages.Position);
            Size buttonMessagesSize = logicpos.Utils.StringToSize(themeButtons.ButtonMessages.Size);
            string buttonMessagesImageFileName = themeButtons.ButtonMessages.ImageFileName;
            bool buttonMessagesVisible = Convert.ToBoolean(themeButtons.ButtonMessages.Visible);

            //Buttons:ButtonWeight
            string buttonWeightName = themeButtons.ButtonWeight.Name;
            string buttonWeightText = themeButtons.ButtonWeight.Text;
            Point buttonWeightPosition = logicpos.Utils.StringToPosition(themeButtons.ButtonWeight.Position);
            Size buttonWeightSize = logicpos.Utils.StringToSize(themeButtons.ButtonWeight.Size);
            string buttonWeightImageFileName = themeButtons.ButtonWeight.ImageFileName;
            bool buttonWeightVisible = Convert.ToBoolean(themeButtons.ButtonWeight.Visible);

            //Buttons:ButtonGifts
            string buttonGiftsName = themeButtons.ButtonGifts.Name;
            string buttonGiftsText = themeButtons.ButtonGifts.Text;
            Point buttonGiftsPosition = logicpos.Utils.StringToPosition(themeButtons.ButtonGifts.Position);
            Size buttonGiftsSize = logicpos.Utils.StringToSize(themeButtons.ButtonGifts.Size);
            string buttonGiftsImageFileName = themeButtons.ButtonGifts.ImageFileName;
            bool buttonGiftsVisible = Convert.ToBoolean(themeButtons.ButtonGifts.Visible);

            //Buttons:ButtonChangeTable
            string buttonChangeTableName = themeButtons.ButtonChangeTable.Name;
            string buttonChangeTableText = themeButtons.ButtonChangeTable.Text;
            Point buttonChangeTablePosition = logicpos.Utils.StringToPosition(themeButtons.ButtonChangeTable.Position);
            Size buttonChangeTableSize = logicpos.Utils.StringToSize(themeButtons.ButtonChangeTable.Size);
            string buttonChangeTableImageFileName = themeButtons.ButtonChangeTable.ImageFileName;
            bool buttonChangeTableVisible = Convert.ToBoolean(themeButtons.ButtonChangeTable.Visible);

            //Buttons:ButtonSelectTable
            string buttonSelectTableName = themeButtons.ButtonSelectTable.Name;
            string buttonSelectTableText = themeButtons.ButtonSelectTable.Text;
            Point buttonSelectTablePosition = logicpos.Utils.StringToPosition(themeButtons.ButtonSelectTable.Position);
            Size buttonSelectTableSize = logicpos.Utils.StringToSize(themeButtons.ButtonSelectTable.Size);
            string buttonSelectTableImageFileName = themeButtons.ButtonSelectTable.ImageFileName;
            bool buttonSelectTableVisible = Convert.ToBoolean(themeButtons.ButtonSelectTable.Visible);

            //Buttons:ButtonFinishOrder
            string buttonFinishOrderName = themeButtons.ButtonFinishOrder.Name;
            string buttonFinishOrderText = themeButtons.ButtonFinishOrder.Text;
            Point buttonFinishOrderPosition = logicpos.Utils.StringToPosition(themeButtons.ButtonFinishOrder.Position);
            Size buttonFinishOrderSize = logicpos.Utils.StringToSize(themeButtons.ButtonFinishOrder.Size);
            string buttonFinishOrderImageFileName = themeButtons.ButtonFinishOrder.ImageFileName;
            bool buttonFinishOrderVisible = Convert.ToBoolean(themeButtons.ButtonFinishOrder.Visible);

            //Buttons:ButtonPayments
            string buttonPaymentsName = themeButtons.ButtonPayments.Name;
            string buttonPaymentsText = themeButtons.ButtonPayments.Text;
            Point buttonPaymentsPosition = logicpos.Utils.StringToPosition(themeButtons.ButtonPayments.Position);
            Size buttonPaymentsSize = logicpos.Utils.StringToSize(themeButtons.ButtonPayments.Size);
            string buttonPaymentsImageFileName = themeButtons.ButtonPayments.ImageFileName;
            bool buttonPaymentsVisible = Convert.ToBoolean(themeButtons.ButtonPayments.Visible);

            //Buttons:ButtonBarCode
            string buttonBarCodeName = themeButtons.ButtonBarCode.Name;
            string buttonBarCodeText = themeButtons.ButtonBarCode.Text;
            Point buttonBarCodePosition = logicpos.Utils.StringToPosition(themeButtons.ButtonBarCode.Position);
            Size buttonBarCodeSize = logicpos.Utils.StringToSize(themeButtons.ButtonBarCode.Size);
            string buttonBarCodeImageFileName = themeButtons.ButtonBarCode.ImageFileName;
            bool buttonBarCodeVisible = Convert.ToBoolean(themeButtons.ButtonBarCode.Visible);

            //IN009279 Parking ticket Service - implementar Cartão cliente
            //Buttons:ButtonCardCode
            string buttonCardCodeName = themeButtons.ButtonCardCode.Name;
            string buttonCardCodeText = themeButtons.ButtonCardCode.Text;
            Point buttonCardCodePosition = logicpos.Utils.StringToPosition(themeButtons.ButtonBarCode.Position);
            Size buttonCardCodeSize = logicpos.Utils.StringToSize(themeButtons.ButtonPayments.Size);
            string buttonCardCodeImageFileName = themeButtons.ButtonCardCode.ImageFileName;
            bool buttonCardCodeVisible = Convert.ToBoolean(themeButtons.ButtonCardCode.Visible);


            //Local Func to Get Shared Buttons
            Func<string, string, Size, string, TouchButtonIconWithText> getButton = (pObjectName, pText, pSize, pImageFileName)
                => new TouchButtonIconWithText(
                pObjectName,
                Color.Transparent,
                pText,
                buttonsFont,
                buttonsFontColor,
                pImageFileName,
                buttonsIconSize,
                pSize.Width,
                pSize.Height
             );
            //Create Button References with Local Func
            _buttonPrev = getButton(buttonPrevName, buttonPrevText, buttonPrevSize, buttonPrevImageFileName);
            _buttonNext = getButton(buttonNextName, buttonNextText, buttonNextSize, buttonNextImageFileName);
            _buttonDecrease = getButton(buttonDecreaseName, buttonDecreaseText, buttonDecreaseSize, buttonDecreaseImageFileName);
            _buttonIncrease = getButton(buttonIncreaseName, buttonIncreaseText, buttonIncreaseSize, buttonIncreaseImageFileName);
            _buttonDelete = getButton(buttonDeleteName, buttonDeleteText, buttonDeleteSize, buttonDeleteImageFileName);
            _buttonChangeQuantity = getButton(buttonChangeQuantityName, buttonChangeQuantityText, buttonChangeQuantitySize, buttonChangeQuantityImageFileName);
            _buttonChangePrice = getButton(buttonChangePriceName, buttonChangePriceText, buttonChangePriceSize, buttonChangePriceImageFileName);
            _buttonListMode = getButton(buttonListModeName, buttonListModeText, buttonListModeSize, buttonListModeImageFileName);
            _buttonListOrder = getButton(buttonListOrderName, buttonListOrderText, buttonListOrderSize, buttonListOrderImageFileName);
            _buttonSplitAccount = getButton(buttonSplitAccountName, buttonSplitAccountText, buttonSplitAccountSize, buttonSplitAccountImageFileName);
            _buttonMessages = getButton(buttonMessagesName, buttonMessagesText, buttonMessagesSize, buttonMessagesImageFileName);
            _buttonWeight = getButton(buttonWeightName, buttonWeightText, buttonWeightSize, buttonWeightImageFileName);
            _buttonGifts = getButton(buttonGiftsName, buttonGiftsText, buttonGiftsSize, buttonGiftsImageFileName);
            _buttonChangeTable = getButton(buttonChangeTableName, buttonChangeTableText, buttonChangeTableSize, buttonChangeTableImageFileName);
            ButtonKeySelectTable = getButton(buttonSelectTableName, buttonSelectTableText, buttonSelectTableSize, buttonSelectTableImageFileName);
            _buttonFinishOrder = getButton(buttonFinishOrderName, buttonFinishOrderText, buttonFinishOrderSize, buttonFinishOrderImageFileName);
            _buttonPayments = getButton(buttonPaymentsName, buttonPaymentsText, buttonPaymentsSize, buttonPaymentsImageFileName);
            _buttonBarCode = getButton(buttonBarCodeName, buttonBarCodeText, buttonBarCodeSize, buttonBarCodeImageFileName);
            _buttonCardCode = getButton(buttonCardCodeName, buttonCardCodeText, buttonCardCodeSize, buttonCardCodeImageFileName);
            //Always Disabled Buttons until Implementation
            _buttonGifts.Sensitive = false;
            _buttonMessages.Sensitive = false;
            //_buttonSplitAccount.Sensitive = false;

            //IN009279 Parking ticket Service - implementar Cartão cliente
            if (GeneralSettings.AppUseParkingTicketModule)
            {
                _buttonCardCode.Visible = true;
                buttonCardCodeVisible = true;
                _buttonWeight.Visible = false;
                buttonWeightVisible = false;
                _buttonBarCode.Visible = false;
                buttonBarCodeVisible = false;
            }
            else
            {
                _buttonCardCode.Visible = false;
                buttonCardCodeVisible = false;
                _buttonWeight.Visible = true;
                buttonWeightVisible = true;
                _buttonBarCode.Visible = true;
                buttonBarCodeVisible = true;
            }

            //Put Buttons/Theme
            Fixed fix = new Fixed() { BorderWidth = 10 };

            if (buttonSelectTableVisible) fix.Put(ButtonKeySelectTable, buttonSelectTablePosition.X, buttonSelectTablePosition.Y);
            if (buttonPrevVisible) fix.Put(_buttonPrev, buttonPrevPosition.X, buttonPrevPosition.Y);
            if (buttonNextVisible) fix.Put(_buttonNext, buttonNextPosition.X, buttonNextPosition.Y);
            if (buttonDecreaseVisible) fix.Put(_buttonDecrease, buttonDecreasePosition.X, buttonDecreasePosition.Y);
            if (buttonIncreaseVisible) fix.Put(_buttonIncrease, buttonIncreasePosition.X, buttonIncreasePosition.Y);
            if (buttonDeleteVisible) fix.Put(_buttonDelete, buttonDeletePosition.X, buttonDeletePosition.Y);
            if (buttonChangeQuantityVisible) fix.Put(_buttonChangeQuantity, buttonChangeQuantityPosition.X, buttonChangeQuantityPosition.Y);
            if (buttonChangePriceVisible) fix.Put(_buttonChangePrice, buttonChangePricePosition.X, buttonChangePricePosition.Y);
            if (buttonListModeVisible) fix.Put(_buttonListMode, buttonListModePosition.X, buttonListModePosition.Y);
            if (buttonListOrderVisible) fix.Put(_buttonListOrder, buttonListOrderPosition.X, buttonListOrderPosition.Y);
            if (buttonWeightVisible) fix.Put(_buttonWeight, buttonWeightPosition.X, buttonWeightPosition.Y);
            if (buttonCardCodeVisible) fix.Put(_buttonCardCode, buttonCardCodePosition.X, buttonCardCodePosition.Y);
            if (buttonGiftsVisible) fix.Put(_buttonGifts, buttonGiftsPosition.X, buttonGiftsPosition.Y);
            if (buttonChangeTableVisible) fix.Put(_buttonChangeTable, buttonChangeTablePosition.X, buttonChangeTablePosition.Y);
            if (buttonFinishOrderVisible) fix.Put(_buttonFinishOrder, buttonFinishOrderPosition.X, buttonFinishOrderPosition.Y);
            if (buttonPaymentsVisible) fix.Put(_buttonPayments, buttonPaymentsPosition.X, buttonPaymentsPosition.Y);
            if (buttonBarCodeVisible) fix.Put(_buttonBarCode, buttonBarCodePosition.X, buttonBarCodePosition.Y);
            if (buttonSplitAccountVisible) fix.Put(_buttonSplitAccount, buttonSplitAccountPosition.X, buttonSplitAccountPosition.Y);
            if (buttonMessagesVisible) fix.Put(_buttonMessages, buttonMessagesPosition.X, buttonMessagesPosition.Y);

            // Add Fix
            Add(fix);

            //Events
            ButtonKeySelectTable.Clicked += buttonKeySelectTable_Clicked;
            //Assign _ticketList Button References, EventHandlers logic in _ticketList
            _ticketList.ButtonKeyPrev = _buttonPrev;
            _ticketList.ButtonKeyNext = _buttonNext;
            _ticketList.ButtonKeyDecrease = _buttonDecrease;
            _ticketList.ButtonKeyIncrease = _buttonIncrease;
            _ticketList.ButtonKeyDelete = _buttonDelete;
            _ticketList.ButtonKeyChangeQuantity = _buttonChangeQuantity;
            _ticketList.ButtonKeyChangePrice = _buttonChangePrice;
            _ticketList.ButtonKeyListMode = _buttonListMode;
            _ticketList.ButtonKeyListOrder = _buttonListOrder;
            _ticketList.ButtonKeySplitAccount = _buttonSplitAccount;
            _ticketList.ButtonKeyWeight = _buttonWeight;
            _ticketList.ButtonKeyGifts = _buttonGifts;
            _ticketList.ButtonKeyChangeTable = _buttonChangeTable;
            _ticketList.ButtonKeyFinishOrder = _buttonFinishOrder;
            _ticketList.ButtonKeyPayments = _buttonPayments;
            _ticketList.ButtonKeyBarCode = _buttonBarCode;
            _ticketList.ButtonKeyCardCode = _buttonCardCode;
            //Miss: SplitAccount
            //Miss: Messages
        }
    }
}