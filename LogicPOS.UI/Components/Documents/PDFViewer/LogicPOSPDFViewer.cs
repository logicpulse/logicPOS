using System.Windows.Forms;

namespace LogicPOS.UI.PDFViewer
{
    public partial class LogicPOSPDFViewer : Form
    {
        private readonly string _pdfLocation;

        public LogicPOSPDFViewer(string pdfLocation)
        {
            InitializeComponent();
            _pdfLocation = pdfLocation;
        }

        private void LogicPOSPDFViewer_Load(object sender, System.EventArgs e)
        {
            var document = PdfiumViewer.PdfDocument.Load(_pdfLocation);
            pdfViewer.Document = document;
        }
    }
}
