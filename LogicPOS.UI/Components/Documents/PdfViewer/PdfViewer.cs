using System.Windows.Forms;

namespace LogicPOS.UI.Components.Documents.PdfViewer
{
    public partial class PdfViewer : Form
    {
        public PdfViewer(string pdfLocation)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PdfViewer));
            this.SuspendLayout();
            // 
            // PdfViewer
            // 
            this.ClientSize = new System.Drawing.Size(747, 748);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PdfViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Visualizador de Documento";
            this.ResumeLayout(false);

        }
    }
}
