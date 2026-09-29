using Gtk;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Settings;
using Pango;
using System;
using System.Drawing;
using System.Linq;
using Color = System.Drawing.Color;

namespace LogicPOS.UI.Buttons
{
    public class SplitAccountCustomerButton : CustomButton
    {
        public static string Customer { get; set; }
        public static string PaymentMethod { get; set; }
        public decimal Total { get; set; }
        public static int SplittersNumber { get; set; }
        public bool Paid { get; set; }
        private readonly Window _window;
        string PaymentDetailsString { get; set; } = "";
        private Label LabelPaymentDetails = new Label();
        public SplitAccountCustomerButton(String labelText, String font, Window window, int splittersNumber)
            : base(new ButtonSettings
            {
                Name = "touchButton_Green",
                Text = labelText,
                Font = font,
                ButtonSize = new Size(0, AppSettings.Instance.IntSplitPaymentTouchButtonSplitPaymentHeight)
            })
        {
            _window = window;
            if (splittersNumber == 0)
            {
                SplittersNumber = splittersNumber;

            }
            initializeObject();
            Initialize();
        }
        public void initializeObject()
        {
            VBox vbox = new VBox();
            EventBox _eventBoxPaymentDetails = new EventBox();

            Label CustomerLabel = new Label();
            CustomerLabel.ModifyFont(FontDescription.FromString("Bold 12"));
            CustomerLabel.Text = ButtonSettings.Text;

            UpdatePaymentDetails();

            LabelPaymentDetails.Text = PaymentDetailsString;
            vbox.PackStart(CustomerLabel, false, true, 5);
            vbox.PackStart(LabelPaymentDetails, false, true, 5);
            ButtonSettings.Widget = vbox;
            ButtonSettings.Widget.ShowAll();
            Clicked += SplitAccountCustomerButton_Clicked;
        }

        private void UpdatePaymentDetails()
        {
            if (Paid)
            {
                PaymentDetailsString = "{0} : {1} : {2}";
                LabelPaymentDetails.Text = PaymentDetailsString = string.Format(PaymentDetailsString, Customer, PaymentMethod, Math.Round(Total, 2, MidpointRounding.AwayFromZero));
            }

        }

        private void SplitAccountCustomerButton_Clicked(object sender, EventArgs e)
        {

            var modal = new PaymentsModal(_window);

            modal.SplitAccount(SplittersNumber);
            ResponseType response = (ResponseType)modal.Run();
            Customer=modal.GetCustomerName();
            if (response == ResponseType.Ok && modal.IsValid)
            {
                Paid = true;
                Total = SaleContext.CurrentOrder.TotalFinal;
                PaymentMethod = modal.PaymentMethod.Designation;
                SplittersNumber--;
                UpdatePaymentDetails();
                if (SplittersNumber == 0)
                {
                    var ticket = SaleContext.CurrentOrder.Tickets.FirstOrDefault();
                    SaleContext.ItemsPage.Clear(true);
                    SaleContext.CurrentOrder.Tickets.Remove(ticket);
                    SaleContext.CurrentOrder.Close();
                }
                Sensitive = false;
            }
            modal.Destroy();
        }
    }
}
