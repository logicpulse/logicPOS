using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Button = Gtk.Button;

namespace LogicPOS.UI.Components.Modals
{
    public class SplitAccountModal : Modal
    {
        VBox vBox = new VBox(false, 0);
        private IconButtonWithText BtnOk;
        private IconButtonWithText BtnCancel;
        private IconButtonWithText BtnRemoveCustomer;
        private IconButtonWithText BtnAddCustomer;
        private static string Title="";
        private readonly PosOrder _order;
        private static int TitleNumber = 2;
        public  List<SplitAccountCustomerButton> Splitters =new List<SplitAccountCustomerButton>();
        public SplitAccountModal(Window parent, PosOrder order) : base(parent, Title, new Size(610, 460),
                                                                       AppSettings.Paths.Images + @"Icons\Windows\icon_window_split_payments.png")
        {
            _order = order;
            Splitters = new List<SplitAccountCustomerButton>()
                {
                    new SplitAccountCustomerButton("splitPaymentButton", AppSettings.Instance.ColorSplitPaymentTouchButtonFilledDataBackground, $"Cliente #{1}", AppSettings.Instance.FontSplitPaymentTouchButtonSplitPayment, this, Splitters.Count>2? Splitters.Count+1 :2),
                    new SplitAccountCustomerButton("splitPaymentButton", AppSettings.Instance.ColorSplitPaymentTouchButtonFilledDataBackground, $"Cliente #{2}", AppSettings.Instance.FontSplitPaymentTouchButtonSplitPayment, this, Splitters.Count>2? Splitters.Count+1 :2)
                };
            TitleNumber = Splitters.Count;

            UpdateTitle(_order);

            UpdateSplitters();
        }

        private void UpdateTitle(PosOrder order)
        {
            WindowSettings.Title.Text = string.Format(GeneralUtils.GetResourceByName("window_title_dialog_split_payment"), TitleNumber.ToString(), (order.TotalFinal / TitleNumber).ToString()) + PreferenceParametersService.SystemCurrency;
        }

        private void UpdateSplitters()
        {
            TitleNumber = Splitters.Count;
            
            foreach (var splitter in Splitters)
            {
                splitter.splittersNumber = Splitters.Count;
                vBox.PackStart(splitter, false, true, 5);
            }
            UpdateTitle(_order);
        }

        private void InitializeButtons()
        {
            BtnAddCustomer = DesignButton(BtnAddCustomer, "touchButtonTableIncrementSplit_DialogActionArea",
                                          GeneralUtils.GetResourceByName("global_add"),
                                          AppSettings.Paths.Images + @"Icons\icon_pos_nav_new.png",
                                          AppSettings.Instance.SizeBaseDialogActionAreaButton);
            BtnAddCustomer.Clicked += BtnAddCustomer_Clicked;


            BtnRemoveCustomer = DesignButton(BtnRemoveCustomer, "touchButtonTableDecrementSplit_DialogActionArea",
                                             GeneralUtils.GetResourceByName("global_remove"),
                                             AppSettings.Paths.Images + @"Icons\icon_pos_nav_delete.png",
                                             AppSettings.Instance.SizeBaseDialogActionAreaButton);
            BtnRemoveCustomer.Clicked += BtnRemoveCustomer_Clicked;

            BtnOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok, "touchButtonOk_DialogActionArea");
            BtnCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel, "touchButtonOk_DialogActionArea");
        }

        private void BtnRemoveCustomer_Clicked(object sender, EventArgs e)
        {

            if (Splitters.Count > 2)
            {
                if (!Splitters[Splitters.Count - 1].Paid)
                {
                    VBox.Remove(Splitters[Splitters.Count - 1]);
                    Splitters[Splitters.Count - 1].Destroy();
                    Splitters.Remove(Splitters[Splitters.Count - 1]);
                }
            }
            UpdateSplitters();
        }

        private void BtnAddCustomer_Clicked(object sender, EventArgs e)
        {
            if (Splitters.Count < 4)
            {
               Splitters.Add(new SplitAccountCustomerButton("splitPaymentButton", AppSettings.Instance.ColorSplitPaymentTouchButtonFilledDataBackground, $"Cliente #{Splitters.Count + 1}", AppSettings.Instance.FontSplitPaymentTouchButtonSplitPayment, this, Splitters.Count > 2 ? Splitters.Count+1 : 2));
            }
            UpdateSplitters();
        }

        private IconButtonWithText DesignButton(Button button, string name, string buttonLabel, string buttonIcon, Size buttonSize)
        {
            return new IconButtonWithText(
                new ButtonSettings
                {
                    Name = name,
                    Text = buttonLabel,
                    Icon = buttonIcon,
                    IconSize = AppSettings.Instance.SizeBaseDialogActionAreaButtonIcon,
                    Font = AppSettings.Instance.FontBaseDialogActionAreaButton,
                    FontColor = Color.White,
                    BackgroundColor = AppSettings.Instance.ColorBaseDialogActionAreaButtonBackground,
                    ButtonSize = AppSettings.Instance.SizeBaseDialogActionAreaButton
                });
        }


        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            InitializeButtons();
            return new ActionAreaButtons
                {
                    new ActionAreaButton(BtnOk, ResponseType.Ok),
                    new ActionAreaButton(BtnCancel, ResponseType.Cancel),
                    new ActionAreaButton(BtnRemoveCustomer, ResponseType.None),
                    new ActionAreaButton(BtnAddCustomer, ResponseType.None)
                 };
        }

        
        protected override Widget CreateBody()
        {
            return vBox;
        }
        protected override void OnResponse(ResponseType response)
        {
            if (response != ResponseType.Ok && response != ResponseType.Cancel)
            {
                Run();
                return;
            }

            base.OnResponse(response);
        }
    }
}
