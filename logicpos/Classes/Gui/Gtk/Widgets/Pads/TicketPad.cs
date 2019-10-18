using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Logic.Others;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    partial class TicketPad : Box
    {
        //Private Members
        private TicketList _ticketList;
        //UI
        private TouchButtonIconWithText _buttonPrev;
        private TouchButtonIconWithText _buttonNext;
        private TouchButtonIconWithText _buttonDecrease;
        private TouchButtonIconWithText _buttonIncrease;
        private TouchButtonIconWithText _buttonDelete;
        private TouchButtonIconWithText _buttonChangeQuantity;
        private TouchButtonIconWithText _buttonChangePrice;
        private TouchButtonIconWithText _buttonListMode;
        private TouchButtonIconWithText _buttonListOrder;
        private TouchButtonIconWithText _buttonSplitAccount;
        private TouchButtonIconWithText _buttonMessages;
        private TouchButtonIconWithText _buttonWeight;
        private TouchButtonIconWithText _buttonGifts;
        private TouchButtonIconWithText _buttonChangeTable;
        private TouchButtonIconWithText _buttonFinishOrder;
        private TouchButtonIconWithText _buttonPayments;
        private TouchButtonIconWithText _buttonBarCode;
		//IN009279 Parking ticket Service - implementar Cartão cliente
        private TouchButtonIconWithText _buttonCardCode;
        //private TouchButtonIconWithText _buttonSendTicket;
        //Public Properties
        public Window SourceWindow { get; set; }
        //Public UI
        TouchButtonIconWithText _buttonSelectTable;
        public TouchButtonIconWithText ButtonKeySelectTable
        {
            get { return _buttonSelectTable; }
            set { _buttonSelectTable = value; }
        }

        public TicketPad(string pName, TicketList pTicketList, dynamic pThemeButtons, Position pPosition)
        {
            //Init Parameters
            Name = pName;
            _ticketList = pTicketList;
            dynamic themeButtons = pThemeButtons;

            //Buttons Shared for all Buttons
            Size buttonsIconSize = Utils.StringToSize(themeButtons.IconSize);
            string buttonsFont = themeButtons.Font;
            Color buttonsFontColor = FrameworkUtils.StringToColor(themeButtons.FontColor);

            //Buttons:ButtonPrev
            string buttonPrevName = themeButtons.ButtonPrev.Name;
            string buttonPrevText = themeButtons.ButtonPrev.Text;
            Position buttonPrevPosition = Utils.StringToPosition(themeButtons.ButtonPrev.Position);
            Size buttonPrevSize = Utils.StringToSize(themeButtons.ButtonPrev.Size);
            string buttonPrevImageFileName = themeButtons.ButtonPrev.ImageFileName;
            bool buttonPrevVisible = Convert.ToBoolean(themeButtons.ButtonPrev.Visible);

            //Buttons:ButtonNext
            string buttonNextName = themeButtons.ButtonNext.Name;
            string buttonNextText = themeButtons.ButtonNext.Text;
            Position buttonNextPosition = Utils.StringToPosition(themeButtons.ButtonNext.Position);
            Size buttonNextSize = Utils.StringToSize(themeButtons.ButtonNext.Size);
            string buttonNextImageFileName = themeButtons.ButtonNext.ImageFileName;
            bool buttonNextVisible = Convert.ToBoolean(themeButtons.ButtonNext.Visible);

            //Buttons:ButtonDecrease
            string buttonDecreaseName = themeButtons.ButtonDecrease.Name;
            string buttonDecreaseText = themeButtons.ButtonDecrease.Text;
            Position buttonDecreasePosition = Utils.StringToPosition(themeButtons.ButtonDecrease.Position);
            Size buttonDecreaseSize = Utils.StringToSize(themeButtons.ButtonDecrease.Size);
            string buttonDecreaseImageFileName = themeButtons.ButtonDecrease.ImageFileName;
            bool buttonDecreaseVisible = Convert.ToBoolean(themeButtons.ButtonDecrease.Visible);

            //Buttons:ButtonIncrease
            string buttonIncreaseName = themeButtons.ButtonIncrease.Name;
            string buttonIncreaseText = themeButtons.ButtonIncrease.Text;
            Position buttonIncreasePosition = Utils.StringToPosition(themeButtons.ButtonIncrease.Position);
            Size buttonIncreaseSize = Utils.StringToSize(themeButtons.ButtonIncrease.Size);
            string buttonIncreaseImageFileName = themeButtons.ButtonIncrease.ImageFileName;
            bool buttonIncreaseVisible = Convert.ToBoolean(themeButtons.ButtonIncrease.Visible);

            //Buttons:ButtonDelete
            string buttonDeleteName = themeButtons.ButtonDelete.Name;
            string buttonDeleteText = themeButtons.ButtonDelete.Text;
            Position buttonDeletePosition = Utils.StringToPosition(themeButtons.ButtonDelete.Position);
            Size buttonDeleteSize = Utils.StringToSize(themeButtons.ButtonDelete.Size);
            string buttonDeleteImageFileName = themeButtons.ButtonDelete.ImageFileName;
            bool buttonDeleteVisible = Convert.ToBoolean(themeButtons.ButtonDelete.Visible);

            //Buttons:ButtonChangeQuantity
            string buttonChangeQuantityName = themeButtons.ButtonChangeQuantity.Name;
            string buttonChangeQuantityText = themeButtons.ButtonChangeQuantity.Text;
            Position buttonChangeQuantityPosition = Utils.StringToPosition(themeButtons.ButtonChangeQuantity.Position);
            Size buttonChangeQuantitySize = Utils.StringToSize(themeButtons.ButtonChangeQuantity.Size);
            string buttonChangeQuantityImageFileName = themeButtons.ButtonChangeQuantity.ImageFileName;
            bool buttonChangeQuantityVisible = Convert.ToBoolean(themeButtons.ButtonChangeQuantity.Visible);

            //Buttons:ButtonChangePrice
            string buttonChangePriceName = themeButtons.ButtonChangePrice.Name;
            string buttonChangePriceText = themeButtons.ButtonChangePrice.Text;
            Position buttonChangePricePosition = Utils.StringToPosition(themeButtons.ButtonChangePrice.Position);
            Size buttonChangePriceSize = Utils.StringToSize(themeButtons.ButtonChangePrice.Size);
            string buttonChangePriceImageFileName = themeButtons.ButtonChangePrice.ImageFileName;
            bool buttonChangePriceVisible = Convert.ToBoolean(themeButtons.ButtonChangePrice.Visible);

            //Buttons:ButtonListMode
            string buttonListModeName = themeButtons.ButtonListMode.Name;
            string buttonListModeText = themeButtons.ButtonListMode.Text;
            Position buttonListModePosition = Utils.StringToPosition(themeButtons.ButtonListMode.Position);
            Size buttonListModeSize = Utils.StringToSize(themeButtons.ButtonListMode.Size);
            string buttonListModeImageFileName = themeButtons.ButtonListMode.ImageFileName;
            bool buttonListModeVisible = Convert.ToBoolean(themeButtons.ButtonListMode.Visible);

            //Buttons:ButtonListOrder
            string buttonListOrderName = themeButtons.ButtonListOrder.Name;
            string buttonListOrderText = themeButtons.ButtonListOrder.Text;
            Position buttonListOrderPosition = Utils.StringToPosition(themeButtons.ButtonListOrder.Position);
            Size buttonListOrderSize = Utils.StringToSize(themeButtons.ButtonListOrder.Size);
            string buttonListOrderImageFileName = themeButtons.ButtonListOrder.ImageFileName;
            bool buttonListOrderVisible = Convert.ToBoolean(themeButtons.ButtonListOrder.Visible);

            //Buttons:ButtonSplitAccount
            string buttonSplitAccountName = themeButtons.ButtonSplitAccount.Name;
            string buttonSplitAccountText = themeButtons.ButtonSplitAccount.Text;
            Position buttonSplitAccountPosition = Utils.StringToPosition(themeButtons.ButtonSplitAccount.Position);
            Size buttonSplitAccountSize = Utils.StringToSize(themeButtons.ButtonSplitAccount.Size);
            string buttonSplitAccountImageFileName = themeButtons.ButtonSplitAccount.ImageFileName;
            bool buttonSplitAccountVisible = Convert.ToBoolean(themeButtons.ButtonSplitAccount.Visible);

            //Buttons:ButtonMessages
            string buttonMessagesName = themeButtons.ButtonMessages.Name;
            string buttonMessagesText = themeButtons.ButtonMessages.Text;
            Position buttonMessagesPosition = Utils.StringToPosition(themeButtons.ButtonMessages.Position);
            Size buttonMessagesSize = Utils.StringToSize(themeButtons.ButtonMessages.Size);
            string buttonMessagesImageFileName = themeButtons.ButtonMessages.ImageFileName;
            bool buttonMessagesVisible = Convert.ToBoolean(themeButtons.ButtonMessages.Visible);

            //Buttons:ButtonWeight
            string buttonWeightName = themeButtons.ButtonWeight.Name;
            string buttonWeightText = themeButtons.ButtonWeight.Text;
            Position buttonWeightPosition = Utils.StringToPosition(themeButtons.ButtonWeight.Position);
            Size buttonWeightSize = Utils.StringToSize(themeButtons.ButtonWeight.Size);
            string buttonWeightImageFileName = themeButtons.ButtonWeight.ImageFileName;
            bool buttonWeightVisible = Convert.ToBoolean(themeButtons.ButtonWeight.Visible);

            //Buttons:ButtonGifts
            string buttonGiftsName = themeButtons.ButtonGifts.Name;
            string buttonGiftsText = themeButtons.ButtonGifts.Text;
            Position buttonGiftsPosition = Utils.StringToPosition(themeButtons.ButtonGifts.Position);
            Size buttonGiftsSize = Utils.StringToSize(themeButtons.ButtonGifts.Size);
            string buttonGiftsImageFileName = themeButtons.ButtonGifts.ImageFileName;
            bool buttonGiftsVisible = Convert.ToBoolean(themeButtons.ButtonGifts.Visible);

            //Buttons:ButtonChangeTable
            string buttonChangeTableName = themeButtons.ButtonChangeTable.Name;
            string buttonChangeTableText = themeButtons.ButtonChangeTable.Text;
            Position buttonChangeTablePosition = Utils.StringToPosition(themeButtons.ButtonChangeTable.Position);
            Size buttonChangeTableSize = Utils.StringToSize(themeButtons.ButtonChangeTable.Size);
            string buttonChangeTableImageFileName = themeButtons.ButtonChangeTable.ImageFileName;
            bool buttonChangeTableVisible = Convert.ToBoolean(themeButtons.ButtonChangeTable.Visible);

            //Buttons:ButtonSelectTable
            string buttonSelectTableName = themeButtons.ButtonSelectTable.Name;
            string buttonSelectTableText = themeButtons.ButtonSelectTable.Text;
            Position buttonSelectTablePosition = Utils.StringToPosition(themeButtons.ButtonSelectTable.Position);
            Size buttonSelectTableSize = Utils.StringToSize(themeButtons.ButtonSelectTable.Size);
            string buttonSelectTableImageFileName = themeButtons.ButtonSelectTable.ImageFileName;
            bool buttonSelectTableVisible = Convert.ToBoolean(themeButtons.ButtonSelectTable.Visible);

            //Buttons:ButtonFinishOrder
            string buttonFinishOrderName = themeButtons.ButtonFinishOrder.Name;
            string buttonFinishOrderText = themeButtons.ButtonFinishOrder.Text;
            Position buttonFinishOrderPosition = Utils.StringToPosition(themeButtons.ButtonFinishOrder.Position);
            Size buttonFinishOrderSize = Utils.StringToSize(themeButtons.ButtonFinishOrder.Size);
            string buttonFinishOrderImageFileName = themeButtons.ButtonFinishOrder.ImageFileName;
            bool buttonFinishOrderVisible = Convert.ToBoolean(themeButtons.ButtonFinishOrder.Visible);

            //Buttons:ButtonPayments
            string buttonPaymentsName = themeButtons.ButtonPayments.Name;
            string buttonPaymentsText = themeButtons.ButtonPayments.Text;
            Position buttonPaymentsPosition = Utils.StringToPosition(themeButtons.ButtonPayments.Position);
            Size buttonPaymentsSize = Utils.StringToSize(themeButtons.ButtonPayments.Size);
            string buttonPaymentsImageFileName = themeButtons.ButtonPayments.ImageFileName;
            bool buttonPaymentsVisible = Convert.ToBoolean(themeButtons.ButtonPayments.Visible);

            //Buttons:ButtonBarCode
            string buttonBarCodeName = themeButtons.ButtonBarCode.Name;
            string buttonBarCodeText = themeButtons.ButtonBarCode.Text;
            Position buttonBarCodePosition = Utils.StringToPosition(themeButtons.ButtonBarCode.Position);
            Size buttonBarCodeSize = Utils.StringToSize(themeButtons.ButtonBarCode.Size);
            string buttonBarCodeImageFileName = themeButtons.ButtonBarCode.ImageFileName;
            bool buttonBarCodeVisible = Convert.ToBoolean(themeButtons.ButtonBarCode.Visible);
			
			//IN009279 Parking ticket Service - implementar Cartão cliente
            //Buttons:ButtonCardCode
            string buttonCardCodeName = themeButtons.ButtonCardCode.Name;
            string buttonCardCodeText = themeButtons.ButtonCardCode.Text;
            Position buttonCardCodePosition = Utils.StringToPosition(themeButtons.ButtonBarCode.Position);
            Size buttonCardCodeSize = Utils.StringToSize(themeButtons.ButtonPayments.Size);
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
            _buttonSelectTable = getButton(buttonSelectTableName, buttonSelectTableText, buttonSelectTableSize, buttonSelectTableImageFileName);
            _buttonFinishOrder = getButton(buttonFinishOrderName, buttonFinishOrderText, buttonFinishOrderSize, buttonFinishOrderImageFileName);
            _buttonPayments = getButton(buttonPaymentsName, buttonPaymentsText, buttonPaymentsSize, buttonPaymentsImageFileName);
            _buttonBarCode = getButton(buttonBarCodeName, buttonBarCodeText, buttonBarCodeSize, buttonBarCodeImageFileName);
            _buttonCardCode = getButton(buttonCardCodeName, buttonCardCodeText, buttonCardCodeSize, buttonCardCodeImageFileName);
            //Always Disabled Buttons until Implementation
            _buttonGifts.Sensitive = false;
            _buttonMessages.Sensitive = false;
            //_buttonSplitAccount.Sensitive = false;
			
			//IN009279 Parking ticket Service - implementar Cartão cliente
            if (GlobalFramework.AppUseParkingTicketModule)
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
            
            if (buttonSelectTableVisible) fix.Put(_buttonSelectTable, buttonSelectTablePosition.X, buttonSelectTablePosition.Y);
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
            _buttonSelectTable.Clicked += buttonKeySelectTable_Clicked;
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