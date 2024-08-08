using Gtk;
using LogicPOS.Domain.Entities;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Shared.Article;
using System.Drawing;

namespace LogicPOS.UI.Buttons
{
    public class SplitPaymentButton : TextButton
    {
        private EventBox _eventBoxPaymentDetails;

        public Label LabelPaymentDetails { get; set; }
        public ArticleBag ArticleBag { get; set; }
        public DocumentProcessingParameters ProcessFinanceDocumentParameter { get; set; }
        public fin_documentfinancemaster DocumentFinanceMaster { get; set; }
        public string SelectedPaymentMethodButtonName { get; set; }

      
        public SplitPaymentButton(ButtonSettings settings)
            : base(settings,false)
        {
            _settings.Widget = CreateWidget();
            Initialize();
        }

        public Widget CreateWidget()
        {
            VBox vbox = new VBox(true, 5) { BorderWidth = 5 };

            SetFont(string.Format("Bold {0}", _settings.Font));
            //Label for PaymentDetails
            LabelPaymentDetails = new Label(string.Empty);
            LabelPaymentDetails.Text = string.Empty; ;
            _eventBoxPaymentDetails = new EventBox() { VisibleWindow = false };
            _eventBoxPaymentDetails.Add(LabelPaymentDetails);

            _eventBoxPaymentDetails.ButtonPressEvent += delegate { Click(); };

            vbox.PackStart(ButtonLabel);
            vbox.PackStart(_eventBoxPaymentDetails);
    
            return vbox;
        }
    }
}
