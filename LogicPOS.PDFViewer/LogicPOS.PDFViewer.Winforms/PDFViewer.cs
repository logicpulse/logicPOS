using System.Windows.Forms;



namespace LogicPOS.PDFViewer.Winforms
{
    public partial class PDFViewer : Form
    {

        public PDFViewer(string docPath,
                         int width = 1980 / 2,
                         int height = 1000,
                         bool export = true)
        {

            InitializeComponent(docPath, width, height);

            pdfToolStripMain1.Enabled = false;
        }

        private void pdfToolStripMain1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.TopMost = false;
            this.DestroyHandle();
        }

        private void OnSaveClick(object sender, ToolStripItemClickedEventArgs e)
        {
            this.TopMost = false;
            this.DestroyHandle();
        }
        protected virtual void OnCloseClick(ToolStripButton item)
        {
            this.DestroyHandle();
            //Application.Exit();
        }

    }
}
