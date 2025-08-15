using Gtk;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using Pango;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Documents;
using Color = System.Drawing.Color;

namespace LogicPOS.UI.Buttons
{
    public class SplitAccountCustomerButton: CustomButton
    {
        //Ui
        public static string Customer { get; set; }
        public static string PaymentMethod { get; set; }
        public static decimal Total { get; set; }
        public int splittersNumber { get; set; }
        public bool Paid { get; set; } 
        private readonly Window _window;
        string paymentDetailsString { get; set; } = "";
        Label LabelPaymentDetails = new Label();
        public SplitAccountCustomerButton(String name, Color color, String labelText, String font, Window window, int splittersNumber)
            : base(new ButtonSettings
            {
                Name = name,
                Text = labelText,
                Font =font,
                ButtonSize = new Size(0, AppSettings.Instance.IntSplitPaymentTouchButtonSplitPaymentHeight),
                
            })
        {
            _settings.BackgroundColor = color;
            _window = window;
            if (splittersNumber == 0)
            {
            this.splittersNumber = splittersNumber;

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
            CustomerLabel.Text = _settings.Text;
            
            updatePaymentDetails();

            LabelPaymentDetails.Text = paymentDetailsString;
            vbox.PackStart(CustomerLabel, false, true, 5);
            vbox.PackStart(LabelPaymentDetails, false, true, 5);
            _settings.Widget = vbox;
            _settings.Widget.ShowAll();
            Clicked += SplitAccountCustomerButton_Clicked;
        }

        private void updatePaymentDetails()
        {
            if (Paid)
            {
                paymentDetailsString="{0} : {1} : {2}";
                LabelPaymentDetails.Text = paymentDetailsString = string.Format(paymentDetailsString, Customer, PaymentMethod, Total);
            }

        }

        private void SplitAccountCustomerButton_Clicked(object sender, EventArgs e)
        {
            Customer =  CustomersService.DefaultCustomer.Name; 

            var modal = new PaymentsModal(_window);
            modal.SplitAccount(splittersNumber);
            ResponseType response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                Paid = true;
                Total = SaleContext.CurrentOrder.TotalFinal;
                PaymentMethod = modal.paymentMethodDesignation;
                updatePaymentDetails();
                Sensitive = false;
            }
            modal.Destroy();
        }
    }
}
