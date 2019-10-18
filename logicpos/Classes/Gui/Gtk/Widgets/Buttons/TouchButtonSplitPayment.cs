using Gtk;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.Classes.Finance;
using logicpos.shared.Classes.Finance;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonSplitPayment : TouchButtonText
    {
        //Colors
        private Color _buttonColor;
        //Ui
        private EventBox _eventBoxPaymentDetails;
        //Public
        private Label _labelPaymentDetails;
        public Label LabelPaymentDetails { get => _labelPaymentDetails; set => _labelPaymentDetails = value; }
        private ArticleBag _articleBag;
        public ArticleBag ArticleBag { get => _articleBag; set => _articleBag = value; }
        //Store Values from PosPaymentWindow
        //ProcessFinanceDocumentParameter
        private ProcessFinanceDocumentParameter _processFinanceDocumentParameter;
        public ProcessFinanceDocumentParameter ProcessFinanceDocumentParameter { get => _processFinanceDocumentParameter; set => _processFinanceDocumentParameter = value; }
        //Store PersistFinanceDocument Result DocumentFinanceMaster Documents
        private fin_documentfinancemaster _documentFinanceMaster;
        public fin_documentfinancemaster DocumentFinanceMaster { get => _documentFinanceMaster; set => _documentFinanceMaster = value; }
        //PaymentMethodButton
        private string _selectedPaymentMethodButtonName;
        public string SelectedPaymentMethodButtonName { get => _selectedPaymentMethodButtonName; set => _selectedPaymentMethodButtonName = value; }

        public TouchButtonSplitPayment(String pName)
            : base(pName)
        {
        }

        public TouchButtonSplitPayment(String pName, Color pColor, String pLabelText, String pFont, int pWidth, int pHeight)
            : base(pName)
        {
            InitObject(pName, pColor, pLabelText, pFont);
            base.InitObject(pName, pColor, _widget, pWidth, pHeight);
        }

        public void InitObject(String pName, Color pColor, String pLabelText, String pFont)
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
            _labelPaymentDetails = new Label(string.Empty);
            _labelPaymentDetails.Text = string.Empty;;
            _eventBoxPaymentDetails = new EventBox() { VisibleWindow = false };
            _eventBoxPaymentDetails.Add(_labelPaymentDetails);
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
