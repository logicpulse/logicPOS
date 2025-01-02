using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PaymentMethods.GetAllPaymentMethods;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Menus
{
    public class PaymentMethodsMenu : Gtk.Table
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public int ScrollerHeight { get; set; } = 0;
        public int ButtonFontSize = Convert.ToInt16(AppSettings.Instance.fontPosBaseButtonSize);
        public int MaxCharsPerButtonLabel { get; set; } = AppSettings.Instance.posBaseButtonMaxCharsPerLabel;
        public string ButtonOverlay { get; set; } = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_overlay.png";
        public List<(PaymentMethod PaymentMethod, CustomButton Button)> Buttons { get; set; } = new List<(PaymentMethod, CustomButton)>();
        public string ButtonImage { get; set; }
        public string ButtonLabel { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CustomButton BtnPrevious { get; set; }
        public CustomButton BtnNext { get; set; }
        public uint Rows { get; set; } = 1;
        public uint Columns { get; set; } = 4;
        public Size ButtonSize { get; set; } = AppSettings.Instance.sizeBaseDialogDefaultButton;
        public Window SourceWindow { get; set; }
        public PaymentMethod SelectedPaymentMethod { get; set; }
        public CustomButton SelectedButton { get; set; }

        public event Action<PaymentMethod> PaymentMethodSelected;

        public PaymentMethodsMenu(CustomButton btnPrevious, CustomButton btnNext) : base(1, 4, true)
        {
            BtnPrevious = btnPrevious;
            BtnNext = btnNext;
            AddEventHandlers();
            LoadEntities();
        }

        private void AddEventHandlers()
        {
            BtnPrevious.Clicked += BtnPrevious_Clicked;
            BtnNext.Clicked += BtnNext_Clicked;
        }

        private IconButtonWithText CreatePaymentMethodButton(string text, string iconPath)
        {
            var font = AppSettings.Instance.fontBaseDialogButton;
            var fontColor = AppSettings.Instance.colorBaseDialogDefaultButtonFont;
            var buttonIconSize = AppSettings.Instance.sizeBaseDialogDefaultButtonIcon;
            var buttonSize = AppSettings.Instance.sizeBaseDialogDefaultButton;

            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "touchButton_Green",
                    Text = text,
                    Font = font,
                    FontColor = fontColor,
                    Icon = iconPath,
                    IconSize = buttonIconSize,
                    ButtonSize = buttonSize
                });
        }

        public void Update()
        {
            RemoveOldButtons();
            AddItems();
            UpdateButtonsState();
        }

        private void UpdateButtonsState()
        {
            if (CurrentPage == 1)
            {
                BtnPrevious.Sensitive = false;
            }
            else
            {
                BtnPrevious.Sensitive = true;
            }

            if (CurrentPage == TotalPages)
            {
                BtnNext.Sensitive = false;
            }
            else
            {
                if (TotalPages > 1) BtnNext.Sensitive = true;
            }
        }

        private void AddItems()
        {
            if (Buttons.Count <= 0)
            {
                return;
            }

            uint currentRow = 0, currentColumn = 0;
            int startItem = (CurrentPage * ItemsPerPage) - ItemsPerPage;
            int endItem = startItem + ItemsPerPage - 1;
            for (int i = startItem; i <= endItem; i++)
            {
                if (i < TotalItems)
                {
                    this.Attach(Buttons[i].Button, currentColumn, currentColumn + 1, currentRow, currentRow + 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
                }

                if (currentColumn == this.NColumns - 1)
                {
                    ++currentRow;
                    currentColumn = 0;
                }
                else
                {
                    ++currentColumn;
                }
            }

        }

        private void RemoveOldButtons()
        {
            int childrenLength = this.Children.Length;
            for (int i = 0; i < childrenLength; i++)
            {
                this.Remove(this.Children[0]);
            }
        }

        private void BtnPrevious_Clicked(object obj, EventArgs args)
        {
            CurrentPage -= 1;
            Update();
        }

        private void BtnNext_Clicked(object obj, EventArgs args)
        {
            CurrentPage += 1;
            Update();
        }

        public void LoadEntities()
        {
            if (AppSettings.Instance.useImageOverlay == false)
            {
                ButtonOverlay = null;
            }

            CurrentPage = 1;
            CustomButton currentButton = null;

            if (Buttons.Count > 0)
            {
                Buttons.Clear();
            }

            IEnumerable<PaymentMethod> paymentMethods = Enumerable.Empty<PaymentMethod>();
            var getPaymentMethodsResult = _mediator.Send(new GetAllPaymentMethodsQuery()).Result;

            if (getPaymentMethodsResult.IsError == false)
            {
                paymentMethods = getPaymentMethodsResult.Value.Where(m => m.IsDeleted == false);
            }

            if (paymentMethods.Any())
            {
                foreach (var paymentMethod in paymentMethods)
                {
 
                    ButtonLabel = paymentMethod.Designation;

                    ButtonImage = null;

                    if (ButtonLabel.Length > MaxCharsPerButtonLabel)
                    {
                        ButtonLabel = ButtonLabel.Substring(0, MaxCharsPerButtonLabel) + ".";
                    }

                    var icon = GetIconByAcronym(paymentMethod.Acronym);
                    currentButton = CreatePaymentMethodButton(ButtonLabel, icon);
                    currentButton.Clicked += Button_Clicked;
                    Buttons.Add((paymentMethod, currentButton));
                }

                TotalItems = Buttons.Count;
                ItemsPerPage = Convert.ToInt16(Rows * Columns);
                TotalPages = (int)Math.Ceiling((float)TotalItems / (float)ItemsPerPage);
                Update();
            }
            else
            {

                Update();
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            CustomButton button = (CustomButton)sender;

            if(SelectedButton != null)
            {
                SelectedButton.Sensitive = true;
            }
    
            SelectedButton = button;
            SelectedButton.Sensitive = false;
            SelectedPaymentMethod = Buttons.Find(x => x.Button == SelectedButton).PaymentMethod;
            PaymentMethodSelected?.Invoke(SelectedPaymentMethod);
        }

        private string GetIconByAcronym(string acronym)
        {
            switch (acronym)
            {
                case "LS":
                    //return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_comecial_paper.png";
                    return PathsSettings.ImagesFolderLocation + @"Icons\no_image_icon.png";
                case "CD":
                    return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_debit_card.png";
                case "DE":
                    //return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_virtual_money.png";
                    return PathsSettings.ImagesFolderLocation + @"Icons\no_image_icon.png";
                case "CA":
                    return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_customer_card.png";
                case "OU":
                    //return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_other.png";
                    return PathsSettings.ImagesFolderLocation + @"Icons\no_image_icon.png";
                case "TR":
                    //return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_transfer.png";
                    return PathsSettings.ImagesFolderLocation + @"Icons\no_image_icon.png";
                case "MB":
                    return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_cash_machine.png";
                case "NU":
                    return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_money.png";
                case "TB":
                    //return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_bank_transfer.png";
                    return PathsSettings.ImagesFolderLocation + @"Icons\no_image_icon.png";
                case "CC":
                    return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_visa.png";
                case "CS":
                    //return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_netting_of_balance.png";
                    return PathsSettings.ImagesFolderLocation + @"Icons\no_image_icon.png";
                case "CO":
                    //return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_bank_check_or_offer_card.png";
                    return PathsSettings.ImagesFolderLocation + @"Icons\no_image_icon.png";
                case "CH":
                    return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_bank_check.png";
                case "PR":
                    //return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_exchange_of_property.png";
                    return PathsSettings.ImagesFolderLocation + @"Icons\no_image_icon.png";
                default:
                    //return PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_payment_type_other.png";
                    return PathsSettings.ImagesFolderLocation + @"Icons\no_image_icon.png";
            }

        }
    }
}
