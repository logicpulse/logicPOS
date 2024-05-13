using Gtk;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.Classes.Finance;
using LogicPOS.Shared.Article;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonSplitPayment : TouchButtonText
    {
        //Colors
        private Color _buttonColor;
        //Ui
        private EventBox _eventBoxPaymentDetails;

        public Label LabelPaymentDetails { get; set; }
        public ArticleBag ArticleBag { get; set; }
        public ProcessFinanceDocumentParameter ProcessFinanceDocumentParameter { get; set; }
        public fin_documentfinancemaster DocumentFinanceMaster { get; set; }
        public string SelectedPaymentMethodButtonName { get; set; }

        public TouchButtonSplitPayment(string pName)
            : base(pName)
        {
        }

        public TouchButtonSplitPayment(string pName, Color pColor, string pLabelText, string pFont, int pWidth, int pHeight)
            : base(pName)
        {
            InitObject(pName, pColor, pLabelText, pFont);
            InitObject(pName, pColor, _widget, pWidth, pHeight);
        }

        public void InitObject(string pName, Color pColor, string pLabelText, string pFont)
        {
            //Init Parameters
            _buttonColor = pColor;

            //Initialize UI Components
            VBox vbox = new VBox(true, 5) { BorderWidth = 5 };
            //Button base Label
            _labelText = pLabelText;
            _label = new Label(pLabelText);
            SetFont(string.Format("Bold {0}", pFont));
            //Label for PaymentDetails
            LabelPaymentDetails = new Label(string.Empty);
            LabelPaymentDetails.Text = string.Empty; ;
            _eventBoxPaymentDetails = new EventBox() { VisibleWindow = false };
            _eventBoxPaymentDetails.Add(LabelPaymentDetails);
            //_eventBoxPaymentDetails.CanFocus = false;
            //If click in EventBox call button Click Event
            _eventBoxPaymentDetails.ButtonPressEvent += delegate { Click(); };

            //Pack VBox
            vbox.PackStart(_label);
            vbox.PackStart(_eventBoxPaymentDetails);
            //Pack Final Widget
            _widget = vbox;
        }
    }
}
