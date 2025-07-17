﻿using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace LogicPOS.UI.PDFViewer
{
    public partial class LogicPOSPDFViewer : Form
    {
        private readonly string _pdfLocation;
        private readonly string _saveDefaultName;

        private LogicPOSPDFViewer(string pdfLocation, string saveDefaultName)
        {
            InitializeComponent();
            _pdfLocation = pdfLocation;
            _saveDefaultName = saveDefaultName;
        }

        private void LogicPOSPDFViewer_Load(object sender, System.EventArgs e)
        {
            var document = PdfiumViewer.PdfDocument.Load(_pdfLocation);
            pdfViewer.Document = document;

            var toolStrip = GetPdfViewerToolStrip();
            var originalSaveButton = GetSaveButton();
            var newSaveButton = CloneButton(originalSaveButton);
            newSaveButton.Click += SaveButton_Click;
            toolStrip.Items.Remove(originalSaveButton);
            toolStrip.Items.Add(newSaveButton);
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