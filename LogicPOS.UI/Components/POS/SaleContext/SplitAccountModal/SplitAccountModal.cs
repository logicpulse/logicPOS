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
using System.Linq;
using System.Windows.Forms;
using Button = Gtk.Button;

namespace LogicPOS.UI.Components.Modals
{
    public class SplitAccountModal : Modal
    {
        VBox vBox = new VBox(false, 0);
        private IconButtonWithText BtnOk;
        private IconButtonWithText BtnCancel;
        private IconButtonWithText BtnRemoveSplitter;
        private IconButtonWithText BtnAddSplitter;
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
                splitter.Clicked += Splitter_Clicked;
                splitter.splittersNumber = Splitters.Count;
                vBox.PackStart(splitter, false, true, 5);
            }
            UpdateTitle(_order);

        }

        private void InitializeButtons()
        {
            BtnAddSplitter = DesignButton(BtnAddSplitter, "touchButtonTableIncrementSplit_DialogActionArea",
                                          GeneralUtils.GetResourceByName("global_add"),
                                          AppSettings.Paths.Images + @"Icons\icon_pos_nav_new.png",
                                          AppSettings.Instance.SizeBaseDialogActionAreaButton);
            BtnAddSplitter.Clicked += BtnAddSplitter_Clicked;


            BtnRemoveSplitter = DesignButton(BtnRemoveSplitter, "touchButtonTableDecrementSplit_DialogActionArea",
                                             GeneralUtils.GetResourceByName("global_remove"),
                                             AppSettings.Paths.Images + @"Icons\icon_pos_nav_delete.png",
                                             AppSettings.Instance.SizeBaseDialogActionAreaButton);
            BtnRemoveSplitter.Clicked += BtnRemoveSplitter_Clicked;

            BtnOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok, "touchButtonOk_DialogActionArea");
            BtnCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel, "touchButtonCancel_DialogActionArea");
        }

        private void BtnRemoveSplitter_Clicked(object sender, EventArgs e)
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
        private void Splitter_Clicked(object sender, EventArgs e)
        {
            if (Splitters.Any(x => x.Paid))
            {
                BtnAddSplitter.Sensitive = false;
                BtnRemoveSplitter.Sensitive = false;
            }
        }
        private void BtnAddSplitter_Clicked(object sender, EventArgs e)
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
                    new ActionAreaButton(BtnRemoveSplitter, ResponseType.None),
                    new ActionAreaButton(BtnAddSplitter, ResponseType.None)
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
                SaleContext.ItemsPage.Clear();
                SaleContext.ItemsPage.PresentOrderItems();
                return;
            }

            base.OnResponse(response);
        }
    }
}
