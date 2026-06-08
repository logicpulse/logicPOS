using System.Drawing.Printing;
using System.Windows.Forms;

namespace LogicPOS.UI.PDFViewer
{
    public partial class LogicPOSPDFViewer : Form
    {
        private readonly string _pdfLocation;
        private readonly string _saveDefaultName;

        private LogicPOSPDFViewer(string pdfLocation, string saveDefaultName)
        {
            InitializeComponent();
            this.TopMost = false;
            _pdfLocation = pdfLocation;
            _saveDefaultName = saveDefaultName;
        }

        private void LogicPOSPDFViewer_Load(object sender, System.EventArgs e)
        {
            var document = PdfiumViewer.PdfDocument.Load(_pdfLocation);
            pdfViewer.Document = document;
            pdfViewer.ZoomMode = PdfiumViewer.PdfViewerZoomMode.FitBest;

            var toolStrip = GetPdfViewerToolStrip();

            // --- Save Button Customization (Existing) ---
            var originalSaveButton = GetSaveButton();
            var newSaveButton = CloneButton(originalSaveButton);
            newSaveButton.Click += SaveButton_Click;

            // Remove original and add new (You might want to insert it at a specific index to keep order)
            int saveIndex = toolStrip.Items.IndexOf(originalSaveButton);
            toolStrip.Items.Remove(originalSaveButton);
            toolStrip.Items.Insert(saveIndex, newSaveButton);


            // --- Print Button Customization (New) ---
            var originalPrintButton = GetPrintButton();
            if (originalPrintButton != null)
            {
                var newPrintButton = CloneButton(originalPrintButton);
                newPrintButton.Click += PrintButton_Click;

                // Maintain the position of the print button
                int printIndex = toolStrip.Items.IndexOf(originalPrintButton);
                toolStrip.Items.Remove(originalPrintButton);
                toolStrip.Items.Insert(printIndex, newPrintButton);
            }
        }

        private void SaveButton_Click(object sender, System.EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.DefaultExt = ".pdf";
                saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.Title = "Salvar Documento";
                saveFileDialog.FileName = _saveDefaultName ?? "Documento";

                if (saveFileDialog.ShowDialog(FindForm()) == DialogResult.OK)
                {
                    try
                    {
                        pdfViewer.Document.Save(saveFileDialog.FileName);
                    }
                    catch
                    {
                        MessageBox.Show(FindForm(), "Erro ao salvar documento", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void PrintButton_Click(object sender, System.EventArgs e)
        {
            using (var printDialog = new PrintDialog())
            {
                printDialog.AllowSomePages = true;
                printDialog.AllowSelection = true;
                printDialog.UseEXDialog = true;

                if (printDialog.ShowDialog(FindForm()) == DialogResult.OK)
                {
                    try
                    {
                        using (var printDocument = pdfViewer.Document.CreatePrintDocument(PdfiumViewer.PdfPrintMode.CutMargin))
                        {
                            printDocument.PrinterSettings = printDialog.PrinterSettings;
                            printDocument.OriginAtMargins = false;
                            printDocument.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);

                            printDocument.QueryPageSettings += (s, qsArgs) =>
                            {
                                var pdfPageSize = pdfViewer.Document.PageSizes[0];

                                int exactWidth = (int)(pdfPageSize.Width / 72.0 * 100.0);
                                int exactHeight = (int)(pdfPageSize.Height / 72.0 * 100.0);

                                qsArgs.PageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom PDF Size", exactWidth, exactHeight);

                                qsArgs.PageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
                            };

                            printDocument.Print();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(FindForm(), $"Erro ao imprimir: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private ToolStrip GetPdfViewerToolStrip()
        {
            var toolStripFieldInfo = typeof(PdfiumViewer.PdfViewer).GetField("_toolStrip", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return (ToolStrip)toolStripFieldInfo.GetValue(pdfViewer);
        }

        private ToolStripButton GetSaveButton()
        {
            var saveButtonFieldInfo = typeof(PdfiumViewer.PdfViewer).GetField("_saveButton", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return (ToolStripButton)saveButtonFieldInfo.GetValue(pdfViewer);
        }

        private ToolStripButton GetPrintButton()
        {
            var printButtonFieldInfo = typeof(PdfiumViewer.PdfViewer).GetField("_printButton", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return (ToolStripButton)printButtonFieldInfo.GetValue(pdfViewer);
        }

        private ToolStripButton CloneButton(ToolStripButton originalButton)
        {
            var clonedButton = new ToolStripButton
            {
                Name = originalButton.Name,
                Text = originalButton.Text,
                Image = originalButton.Image,
                DisplayStyle = originalButton.DisplayStyle,
                Enabled = originalButton.Enabled,
                Visible = originalButton.Visible,
                ToolTipText = originalButton.ToolTipText // Copied tooltip as well
            };

            return clonedButton;
        }

        public static void ShowPDF(string pdfLocation, string saveDefaultName)
        {
            var pdfViewer = new LogicPOSPDFViewer(pdfLocation, saveDefaultName);
            pdfViewer.ShowDialog();
        }
    }
}