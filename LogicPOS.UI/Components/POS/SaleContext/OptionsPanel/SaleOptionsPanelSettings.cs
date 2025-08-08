using LogicPOS.UI.Buttons;
using LogicPOS.UI.Extensions;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components
{
    public class SaleOptionsPanelSettings
    {
        public dynamic Theme { get; }

        public SaleOptionsPanelSettings(dynamic theme)
        {
            Theme = theme;
        }

        public Size ButtonIconSize => (Theme.IconSize as string).ToSize();
        public string ButtonFont => Theme.Font;
        public Color ButtonFontColor => (Theme.FontColor as string).StringToColor();

        #region BtnPrevious
        public string BtnPreviousName => Theme.ButtonPrev.Name;
        public string BtnPreviousText => Theme.ButtonPrev.Text;
        public Point BtnPreviousPosition => logicpos.Utils.StringToPosition(Theme.ButtonPrev.Position);
        public Size BtnPreviousSize => (Theme.ButtonPrev.Size as string).ToSize();
        public string BtnPreviousImageFileName => Theme.ButtonPrev.ImageFileName;
        public bool BtnPreviousVisible => Convert.ToBoolean(Theme.ButtonPrev.Visible);
        #endregion

        #region BtnNext
        public string BtnNextName => Theme.ButtonNext.Name;
        public string BtnNextText => Theme.ButtonNext.Text;
        public Point BtnNextPosition => logicpos.Utils.StringToPosition(Theme.ButtonNext.Position);
        public Size BtnNextSize => (Theme.ButtonNext.Size as string).ToSize();
        public string BtnNextImageFileName => Theme.ButtonNext.ImageFileName;
        public bool BtnNextVisible => Convert.ToBoolean(Theme.ButtonNext.Visible);
        #endregion

        #region BtnDecrease
        public string BtnDecreaseName => Theme.ButtonDecrease.Name;
        public string BtnDecreaseText => Theme.ButtonDecrease.Text;
        public Point BtnDecreasePosition => logicpos.Utils.StringToPosition(Theme.ButtonDecrease.Position);
        public Size BtnDecreaseSize => (Theme.ButtonDecrease.Size as string).ToSize();
        public string BtnDecreaseImageFileName => Theme.ButtonDecrease.ImageFileName;
        public bool BtnDecreaseVisible => Convert.ToBoolean(Theme.ButtonDecrease.Visible);
        #endregion

        #region BtnIncrease
        public string BtnIncreaseName => Theme.ButtonIncrease.Name;
        public string BtnIncreaseText => Theme.ButtonIncrease.Text;
        public Point BtnIncreasePosition => logicpos.Utils.StringToPosition(Theme.ButtonIncrease.Position);
        public Size BtnIncreaseSize => (Theme.ButtonIncrease.Size as string).ToSize();
        public string BtnIncreaseImageFileName => Theme.ButtonIncrease.ImageFileName;
        public bool BtnIncreaseVisible => Convert.ToBoolean(Theme.ButtonIncrease.Visible);
        #endregion

        #region BtnDelete
        public string BtnDeleteName => Theme.ButtonDelete.Name;
        public string BtnDeleteText => Theme.ButtonDelete.Text;
        public Point BtnDeletePosition => logicpos.Utils.StringToPosition(Theme.ButtonDelete.Position);
        public Size BtnDeleteSize => (Theme.ButtonDelete.Size as string).ToSize();
        public string BtnDeleteImageFileName => Theme.ButtonDelete.ImageFileName;
        public bool BtnDeleteVisible => Convert.ToBoolean(Theme.ButtonDelete.Visible);
        #endregion

        #region BtnQuantity
        public string BtnQuantityName => Theme.ButtonChangeQuantity.Name;
        public string BtnQuantityText => Theme.ButtonChangeQuantity.Text;
        public Point BtnQuantityPosition => logicpos.Utils.StringToPosition(Theme.ButtonChangeQuantity.Position);
        public Size BtnQuantitySize => (Theme.ButtonChangeQuantity.Size as string).ToSize();
        public string BtnQuantityImageFileName => Theme.ButtonChangeQuantity.ImageFileName;
        public bool BtnQuantityVisible => Convert.ToBoolean(Theme.ButtonChangeQuantity.Visible);
        #endregion

        #region BtnPrice
        public string BtnPriceName => Theme.ButtonChangePrice.Name;
        public string BtnPriceText => Theme.ButtonChangePrice.Text;
        public Point BtnPricePosition => logicpos.Utils.StringToPosition(Theme.ButtonChangePrice.Position);
        public Size BtnPriceSize => (Theme.ButtonChangePrice.Size as string).ToSize();
        public string BtnPriceImageFileName => Theme.ButtonChangePrice.ImageFileName;
        public bool BtnPriceVisible => Convert.ToBoolean(Theme.ButtonChangePrice.Visible);
        #endregion

        #region BtnListMode
        public string BtnListModeName => Theme.ButtonListMode.Name;
        public string BtnListModeText => Theme.ButtonListMode.Text;
        public Point BtnListModePosition => logicpos.Utils.StringToPosition(Theme.ButtonListMode.Position);
        public Size BtnListModeSize => (Theme.ButtonListMode.Size as string).ToSize();
        public string BtnListModeImageFileName => Theme.ButtonListMode.ImageFileName;
        public bool BtnListModeVisible => Convert.ToBoolean(Theme.ButtonListMode.Visible);
        #endregion

        #region BtnListOrder
        public string BtnListOrderName => Theme.ButtonListOrder.Name;
        public string BtnListOrderText => Theme.ButtonListOrder.Text;
        public Point BtnListOrderPosition => logicpos.Utils.StringToPosition(Theme.ButtonListOrder.Position);
        public Size BtnListOrderSize => (Theme.ButtonListOrder.Size as string).ToSize();
        public string BtnListOrderImageFileName => Theme.ButtonListOrder.ImageFileName;
        public bool BtnListOrderVisible => Convert.ToBoolean(Theme.ButtonListOrder.Visible);
        #endregion

        #region BtnSplitAccount
        public string BtnSplitAccountName => Theme.ButtonSplitAccount.Name;
        public string BtnSplitAccountText => Theme.ButtonSplitAccount.Text;
        public Point BtnSplitAccountPosition => logicpos.Utils.StringToPosition(Theme.ButtonSplitAccount.Position);
        public Size BtnSplitAccountSize => (Theme.ButtonSplitAccount.Size as string).ToSize();
        public string BtnSplitAccountImageFileName => Theme.ButtonSplitAccount.ImageFileName;
        public bool BtnSplitAccountVisible => Convert.ToBoolean(Theme.ButtonSplitAccount.Visible);
        #endregion

        #region BtnMessages
        public string BtnMessagesName => Theme.ButtonMessages.Name;
        public string BtnMessagesText => Theme.ButtonMessages.Text;
        public Point BtnMessagesPosition => logicpos.Utils.StringToPosition(Theme.ButtonMessages.Position);
        public Size BtnMessagesSize => (Theme.ButtonMessages.Size as string).ToSize();
        public string BtnMessagesImageFileName => Theme.ButtonMessages.ImageFileName;
        public bool BtnMessagesVisible => Convert.ToBoolean(Theme.ButtonMessages.Visible);
        #endregion

        #region BtnWeight
        public string BtnWeightName => Theme.ButtonWeight.Name;
        public string BtnWeightText => Theme.ButtonWeight.Text;
        public Point BtnWeightPosition => logicpos.Utils.StringToPosition(Theme.ButtonWeight.Position);
        public Size BtnWeightSize => (Theme.ButtonWeight.Size as string).ToSize();
        public string BtnWeightImageFileName => Theme.ButtonWeight.ImageFileName;
        public bool BtnWeightVisible => Convert.ToBoolean(Theme.ButtonWeight.Visible);
        #endregion

        #region BtnGifts
        public string BtnGiftsName => Theme.ButtonGifts.Name;
        public string BtnGiftsText => Theme.ButtonGifts.Text;
        public Point BtnGiftsPosition => logicpos.Utils.StringToPosition(Theme.ButtonGifts.Position);
        public Size BtnGiftsSize => (Theme.ButtonGifts.Size as string).ToSize();
        public string BtnGiftsImageFileName => Theme.ButtonGifts.ImageFileName;
        public bool BtnGiftsVisible => Convert.ToBoolean(Theme.ButtonGifts.Visible);
        #endregion

        #region BtnChangeTable
        public string BtnChangeTableName => Theme.ButtonChangeTable.Name;
        public string BtnChangeTableText => Theme.ButtonChangeTable.Text;
        public Point BtnChangeTablePosition => logicpos.Utils.StringToPosition(Theme.ButtonChangeTable.Position);
        public Size BtnChangeTableSize => (Theme.ButtonChangeTable.Size as string).ToSize();
        public string BtnChangeTableImageFileName => Theme.ButtonChangeTable.ImageFileName;
        public bool BtnChangeTableVisible => Convert.ToBoolean(Theme.ButtonChangeTable.Visible);
        #endregion

        #region BtnSelectTable
        public string BtnSelectTableName => Theme.ButtonSelectTable.Name;
        public string BtnSelectTableText => Theme.ButtonSelectTable.Text;
        public Point BtnSelectTablePosition => logicpos.Utils.StringToPosition(Theme.ButtonSelectTable.Position);
        public Size BtnSelectTableSize => (Theme.ButtonSelectTable.Size as string).ToSize();
        public string BtnSelectTableImageFileName => Theme.ButtonSelectTable.ImageFileName;
        public bool BtnSelectTableVisible => Convert.ToBoolean(Theme.ButtonSelectTable.Visible);
        #endregion

        #region BtnFinishOrder
        public string BtnFinishOrderName => Theme.ButtonFinishOrder.Name;
        public string BtnFinishOrderText => Theme.ButtonFinishOrder.Text;
        public Point BtnFinishOrderPosition => logicpos.Utils.StringToPosition(Theme.ButtonFinishOrder.Position);
        public Size BtnFinishOrderSize => (Theme.ButtonFinishOrder.Size as string).ToSize();
        public string BtnFinishOrderImageFileName => Theme.ButtonFinishOrder.ImageFileName;
        public bool BtnFinishOrderVisible => Convert.ToBoolean(Theme.ButtonFinishOrder.Visible);
        #endregion

        #region BtnPayments
        public string BtnPaymentsName => Theme.ButtonPayments.Name;
        public string BtnPaymentsText => Theme.ButtonPayments.Text;
        public Point BtnPaymentsPosition => logicpos.Utils.StringToPosition(Theme.ButtonPayments.Position);
        public Size BtnPaymentsSize => (Theme.ButtonPayments.Size as string).ToSize();
        public string BtnPaymentsImageFileName => Theme.ButtonPayments.ImageFileName;
        public bool BtnPaymentsVisible => Convert.ToBoolean(Theme.ButtonPayments.Visible);
        #endregion

        #region BtnBarcode
        public string BtnBarCodeName => Theme.ButtonBarCode.Name;
        public string BtnBarCodeText => Theme.ButtonBarCode.Text;
        public Point BtnBarCodePosition => logicpos.Utils.StringToPosition(Theme.ButtonBarCode.Position);
        public Size BtnBarCodeSize => (Theme.ButtonBarCode.Size as string).ToSize();
        public string BtnBarCodeImageFileName => Theme.ButtonBarCode.ImageFileName;
        public bool BtnBarCodeVisible => Convert.ToBoolean(Theme.ButtonBarCode.Visible);
        #endregion

        #region BtnCardCode
        public string BtnCardCodeName => Theme.ButtonCardCode.Name;
        public string BtnCardCodeText => Theme.ButtonCardCode.Text;
        public Point BtnCardCodePosition => logicpos.Utils.StringToPosition(Theme.ButtonBarCode.Position);
        public Size BtnCardCodeSize => (Theme.ButtonPayments.Size as string).ToSize();
        public string BtnCardCodeImageFileName => Theme.ButtonCardCode.ImageFileName;
        public bool BtnCardCodeVisible => Convert.ToBoolean(Theme.ButtonCardCode.Visible);
        #endregion


        private IconButtonWithText CreateButton(string name,
                                                string text,
                                                Size size,
                                                string imageFileName)

        {
            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = name,
                    Text = text,
                    Font = ButtonFont,
                    FontColor = ButtonFontColor,
                    Icon = imageFileName,
                    IconSize = ButtonIconSize,
                    ButtonSize = size
                });
        }

        public IconButtonWithText CreateBtnPrevious()
        {
            var button = CreateButton("buttonUserId",
                                      BtnPreviousText,
                                      BtnPreviousSize,
                                      BtnPreviousImageFileName);

            button.Visible = BtnPreviousVisible;

            return button;
        }

        public IconButtonWithText CreateBtnNext()
        {
            var button = CreateButton("buttonUserId",
                                      BtnNextText,
                                      BtnNextSize,
                                      BtnNextImageFileName);

            button.Visible = BtnNextVisible;

            return button;
        }

        public IconButtonWithText CreateBtnDecrease()
        {
            var button = CreateButton("buttonUserId",
                                      BtnDecreaseText,
                                      BtnDecreaseSize,
                                      BtnDecreaseImageFileName);

            button.Visible = BtnDecreaseVisible;

            return button;
        }

        public IconButtonWithText CreateBtnIncrease()
        {
            var button = CreateButton("buttonUserId",
                                      BtnIncreaseText,
                                      BtnIncreaseSize,
                                      BtnIncreaseImageFileName);

            button.Visible = BtnIncreaseVisible;

            return button;
        }

        public IconButtonWithText CreateBtnDelete()
        {
            var button = CreateButton("buttonUserId",
                                      BtnDeleteText,
                                      BtnDeleteSize,
                                      BtnDeleteImageFileName);

            button.Visible = BtnDeleteVisible;

            return button;
        }

        public IconButtonWithText CreateBtnQuantity()
        {
            var button = CreateButton("buttonUserId",
                                      BtnQuantityText,
                                      BtnQuantitySize,
                                      BtnQuantityImageFileName);

            button.Visible = BtnQuantityVisible;

            return button;
        }

        public IconButtonWithText CreateBtnPrice()
        {
            var button = CreateButton("buttonUserId",
                                      BtnPriceText,
                                      BtnPriceSize,
                                      BtnPriceImageFileName);

            button.Visible = BtnPriceVisible;

            return button;
        }

        public IconButtonWithText CreateBtnListMode()
        {
            var button = CreateButton("buttonUserId",
                                      BtnListModeText,
                                      BtnListModeSize,
                                      BtnListModeImageFileName);

            button.Visible = BtnListModeVisible;

            return button;
        }

        public IconButtonWithText CreateBtnListOrder()
        {
            var button = CreateButton("buttonUserId",
                                      BtnListOrderText,
                                      BtnListOrderSize,
                                      BtnListOrderImageFileName);

            button.Visible = BtnListOrderVisible;
            button.Sensitive = false;
            return button;
        }

        public IconButtonWithText CreateBtnSplitAccount()
        {
            var button = CreateButton(BtnSplitAccountName,
                                      BtnSplitAccountText,
                                      BtnSplitAccountSize,
                                      BtnSplitAccountImageFileName);

            button.Visible = BtnSplitAccountVisible;

            return button;
        }

        public IconButtonWithText CreateBtnMessages()
        {
            var button = CreateButton(BtnMessagesName,
                                      BtnMessagesText,
                                      BtnMessagesSize,
                                      BtnMessagesImageFileName);

            button.Visible = BtnMessagesVisible;

            return button;
        }

        public IconButtonWithText CreateBtnWeight()
        {
            var button = CreateButton("buttonUserId",
                                      BtnWeightText,
                                      BtnWeightSize,
                                      BtnWeightImageFileName);

            button.Visible = BtnWeightVisible;

            return button;
        }

        public IconButtonWithText CreateBtnGifts()
        {
            var button = CreateButton(BtnGiftsName,
                                      BtnGiftsText,
                                      BtnGiftsSize,
                                      BtnGiftsImageFileName);

            button.Visible = BtnGiftsVisible;

            return button;
        }

        public IconButtonWithText CreateBtnChangeTable()
        {
            var button = CreateButton("buttonUserId",
                                      BtnChangeTableText,
                                      BtnChangeTableSize,
                                      BtnChangeTableImageFileName);

            button.Visible = BtnChangeTableVisible;

            return button;
        }

        public IconButtonWithText CreateBtnSelectTable()
        {
            var button = CreateButton(BtnSelectTableName,
                                      BtnSelectTableText,
                                      BtnSelectTableSize,
                                      BtnSelectTableImageFileName);

            button.Visible = BtnSelectTableVisible;

            return button;
        }

        public IconButtonWithText CreateBtnFinishOrder()
        {
            var button = CreateButton(BtnFinishOrderName,
                                      BtnFinishOrderText,
                                      BtnFinishOrderSize,
                                      BtnFinishOrderImageFileName);

            button.Visible = BtnFinishOrderVisible;

            return button;
        }

        public IconButtonWithText CreateBtnPayments()
        {
            var button = CreateButton(BtnPaymentsName,
                                      BtnPaymentsText,
                                      BtnPaymentsSize,
                                      BtnPaymentsImageFileName);

            button.Visible = BtnPaymentsVisible;

            return button;
        }

        public IconButtonWithText CreateBtnBarcode()
        {
            var button = CreateButton("buttonUserId",
                                      BtnBarCodeText,
                                      BtnBarCodeSize,
                                      BtnBarCodeImageFileName);

            button.Visible = BtnBarCodeVisible;

            return button;
        }

        public IconButtonWithText CreateBtnCardCode()
        {
            var button = CreateButton(BtnCardCodeName,
                                      BtnCardCodeText,
                                      BtnCardCodeSize,
                                      BtnCardCodeImageFileName);

            button.Visible = BtnCardCodeVisible;

            return button;
        }
    }
}
