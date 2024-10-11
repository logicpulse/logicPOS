﻿using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;

namespace LogicPOS.UI.Components
{
    public class SaleOptionsPanel : Box
    {
        public SaleOptionsPanelSettings PanelSettings { get; }

        #region Components
        public IconButtonWithText BtnPrevious { get; set; }
        public IconButtonWithText BtnNext { get; set; }
        public IconButtonWithText BtnDecrease { get; set; }
        public IconButtonWithText BtnIncrease { get; set; }
        public IconButtonWithText BtnDelete { get; set; }
        public IconButtonWithText BtnQuantity { get; set; }
        public IconButtonWithText BtnPrice { get; set; }
        public IconButtonWithText BtnListMode { get; set; }
        public IconButtonWithText BtnListOrder { get; set; }
        public IconButtonWithText BtnSplitAccount { get; set; }
        public IconButtonWithText BtnMessages { get; set; }
        public IconButtonWithText BtnWeight { get; set; }
        public IconButtonWithText BtnGifts { get; set; }
        public IconButtonWithText BtnChangeTable { get; set; }
        public IconButtonWithText BtnFinishOrder { get; set; }
        public IconButtonWithText BtnPayments { get; set; }
        public IconButtonWithText BtnBarcode { get; set; }
        public IconButtonWithText BtnCardCode { get; set; }
        public IconButtonWithText BtnSelectTable { get; set; }
        #endregion

        public Window SourceWindow { get; set; }


        public SaleOptionsPanel(dynamic buttonsTheme)
        {
            PanelSettings = new SaleOptionsPanelSettings(buttonsTheme);
            InitializeButtons();
            SetButtonsVisibility();
            Add(CreateButtonsBox());
        }
        private Fixed CreateButtonsBox()
        {
            Fixed box = new Fixed() { BorderWidth = 10 };

            if (BtnSelectTable.Visible) box.Put(BtnSelectTable, PanelSettings.BtnSelectTablePosition.X, PanelSettings.BtnSelectTablePosition.Y);
            if (BtnPrevious.Visible) box.Put(BtnPrevious, PanelSettings.BtnPreviousPosition.X, PanelSettings.BtnPreviousPosition.Y);
            if (BtnNext.Visible) box.Put(BtnNext, PanelSettings.BtnNextPosition.X, PanelSettings.BtnNextPosition.Y);
            if (BtnDecrease.Visible) box.Put(BtnDecrease, PanelSettings.BtnDecreasePosition.X, PanelSettings.BtnDecreasePosition.Y);
            if (BtnIncrease.Visible) box.Put(BtnIncrease, PanelSettings.BtnIncreasePosition.X, PanelSettings.BtnIncreasePosition.Y);
            if (BtnDelete.Visible) box.Put(BtnDelete, PanelSettings.BtnDeletePosition.X, PanelSettings.BtnDeletePosition.Y);
            if (BtnQuantity.Visible) box.Put(BtnQuantity, PanelSettings.BtnQuantityPosition.X, PanelSettings.BtnQuantityPosition.Y);
            if (BtnPrice.Visible) box.Put(BtnPrice, PanelSettings.BtnPricePosition.X, PanelSettings.BtnPricePosition.Y);
            if (BtnListMode.Visible) box.Put(BtnListMode, PanelSettings.BtnListModePosition.X, PanelSettings.BtnListModePosition.Y);
            if (BtnListOrder.Visible) box.Put(BtnListOrder, PanelSettings.BtnListOrderPosition.X, PanelSettings.BtnListOrderPosition.Y);
            if (BtnWeight.Visible) box.Put(BtnWeight, PanelSettings.BtnWeightPosition.X, PanelSettings.BtnWeightPosition.Y);
            if (BtnCardCode.Visible) box.Put(BtnCardCode, PanelSettings.BtnCardCodePosition.X, PanelSettings.BtnCardCodePosition.Y);
            if (BtnGifts.Visible) box.Put(BtnGifts, PanelSettings.BtnGiftsPosition.X, PanelSettings.BtnGiftsPosition.Y);
            if (BtnChangeTable.Visible) box.Put(BtnChangeTable, PanelSettings.BtnChangeTablePosition.X, PanelSettings.BtnChangeTablePosition.Y);
            if (BtnFinishOrder.Visible) box.Put(BtnFinishOrder, PanelSettings.BtnFinishOrderPosition.X, PanelSettings.BtnFinishOrderPosition.Y);
            if (BtnPayments.Visible) box.Put(BtnPayments, PanelSettings.BtnPaymentsPosition.X, PanelSettings.BtnPaymentsPosition.Y);
            if (BtnBarcode.Visible) box.Put(BtnBarcode, PanelSettings.BtnBarCodePosition.X, PanelSettings.BtnBarCodePosition.Y);
            if (BtnSplitAccount.Visible) box.Put(BtnSplitAccount, PanelSettings.BtnSplitAccountPosition.X, PanelSettings.BtnSplitAccountPosition.Y);
            if (BtnMessages.Visible) box.Put(BtnMessages, PanelSettings.BtnMessagesPosition.X, PanelSettings.BtnMessagesPosition.Y);

            return box;
        }

        private void InitializeButtons()
        {
            BtnPrevious = PanelSettings.CreateBtnPrevious();
            BtnNext = PanelSettings.CreateBtnNext();
            BtnDecrease = PanelSettings.CreateBtnDecrease();
            BtnIncrease = PanelSettings.CreateBtnIncrease();
            BtnDelete = PanelSettings.CreateBtnDelete();
            BtnQuantity = PanelSettings.CreateBtnQuantity();
            BtnPrice = PanelSettings.CreateBtnPrice();
            BtnListMode = PanelSettings.CreateBtnListMode();
            BtnListOrder = PanelSettings.CreateBtnListOrder();
            BtnSplitAccount = PanelSettings.CreateBtnSplitAccount();
            BtnMessages = PanelSettings.CreateBtnMessages();
            BtnWeight = PanelSettings.CreateBtnWeight();
            BtnGifts = PanelSettings.CreateBtnGifts();
            BtnChangeTable = PanelSettings.CreateBtnChangeTable();
            BtnSelectTable = PanelSettings.CreateBtnSelectTable();
            BtnFinishOrder = PanelSettings.CreateBtnFinishOrder();
            BtnPayments = PanelSettings.CreateBtnPayments();
            BtnBarcode = PanelSettings.CreateBtnBarcode();
            BtnCardCode = PanelSettings.CreateBtnCardCode();
        }

        private void SetButtonsVisibility()
        {
            if (GeneralSettings.AppUseParkingTicketModule)
            {
                BtnCardCode.Visible = true;
                BtnWeight.Visible = false;
                BtnBarcode.Visible = false;
            }
            else
            {
                BtnCardCode.Visible = false;
                BtnWeight.Visible = true;
                BtnBarcode.Visible = true;
            }
        }


    }
}