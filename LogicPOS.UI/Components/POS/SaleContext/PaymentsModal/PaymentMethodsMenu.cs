using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PaymentMethods.GetAllPaymentMethods;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Settings;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public class PaymentMethodsMenu : Menu<PaymentMethod>
    {
        private readonly ISender _mediator = DependencyInjection.Mediator;
        public uint Rows { get; set; } = 1;
        public uint Columns { get; set; } = 4;
        public Size ButtonSize { get; set; } = AppSettings.Instance.SizeBaseDialogDefaultButton;

        public PaymentMethodsMenu(CustomButton btnPrevious,
                                  CustomButton btnNext,
                                  Window sourceWindow) : base(rows: 1,
                                                              columns: 4,
                                                              AppSettings.Instance.SizeBaseDialogDefaultButton,
                                                              "touchButton_Green",
                                                              btnPrevious,
                                                              btnNext,
                                                              sourceWindow)
        {
            LoadEntities();
            ListEntities(Entities);
        }


        private IconButtonWithText CreatePaymentMethodButton(string text, string iconPath)
        {
            var font = AppSettings.Instance.FontBaseDialogButton;
            var fontColor = AppSettings.Instance.ColorBaseDialogDefaultButtonFont;
            var buttonIconSize = AppSettings.Instance.SizeBaseDialogDefaultButtonIcon;
            var buttonSize = AppSettings.Instance.SizeBaseDialogDefaultButton;

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
                    //return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_comecial_paper.png";
                    return AppSettings.Paths.Images + @"Icons\no_image_icon.png";
                case "CD":
                    return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_debit_card.png";
                case "DE":
                    //return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_virtual_money.png";
                    return AppSettings.Paths.Images + @"Icons\no_image_icon.png";
                case "CA":
                    return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_customer_card.png";
                case "OU":
                    //return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_other.png";
                    return AppSettings.Paths.Images + @"Icons\no_image_icon.png";
                case "TR":
                    //return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_transfer.png";
                    return AppSettings.Paths.Images + @"Icons\no_image_icon.png";
                case "MB":
                    return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_cash_machine.png";
                case "NU":
                    return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_money.png";
                case "TB":
                    //return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_bank_transfer.png";
                    return AppSettings.Paths.Images + @"Icons\no_image_icon.png";
                case "CC":
                    return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_visa.png";
                case "CS":
                    //return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_netting_of_balance.png";
                    return AppSettings.Paths.Images + @"Icons\no_image_icon.png";
                case "CO":
                    //return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_bank_check_or_offer_card.png";
                    return AppSettings.Paths.Images + @"Icons\no_image_icon.png";
                case "CH":
                    return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_bank_check.png";
                case "PR":
                    //return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_exchange_of_property.png";
                    return AppSettings.Paths.Images + @"Icons\no_image_icon.png";
                default:
                    //return AppSettings.Paths.Images + @"Icons\icon_pos_payment_type_other.png";
                    return AppSettings.Paths.Images + @"Icons\no_image_icon.png";
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

        protected override IEnumerable<PaymentMethod> FilterEntities(IEnumerable<PaymentMethod> entities)
        {
            return entities;
        }
    }
}
