using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PaymentMethods.GetAllPaymentMethods;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public class PaymentMethodsMenu : Menu<PaymentMethod>
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public uint Rows { get; set; } = 1;
        public uint Columns { get; set; } = 4;
        public Size ButtonSize { get; set; } = AppSettings.Instance.sizeBaseDialogDefaultButton;

        public PaymentMethodsMenu(CustomButton btnPrevious,
                                  CustomButton btnNext,
                                  Window sourceWindow) : base(rows: 1,
                                                              columns: 4,
                                                              AppSettings.Instance.sizeBaseDialogDefaultButton,
                                                              "touchButton_Green",
                                                              btnPrevious,
                                                              btnNext,
                                                              sourceWindow)
        {
            PresentEntities();
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

        protected override CustomButton CreateMenuButton(PaymentMethod entity)
        {
            return CreatePaymentMethodButton(ButtonLabel, GetIconByAcronym(entity.Acronym));
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

        protected override string GetButtonLabel(PaymentMethod entity)
        {
            return entity.Designation;
        }

        protected override string GetButtonImage(PaymentMethod entity)
        {
            return null;
        }

        protected override void LoadEntities()
        {
            Entities.Clear();

            var paymentMethods = _mediator.Send(new GetAllPaymentMethodsQuery()).Result;

            if (paymentMethods.IsError != false)
            {
                return;
            }

            Entities.AddRange(paymentMethods.Value);
        }
    }
}
