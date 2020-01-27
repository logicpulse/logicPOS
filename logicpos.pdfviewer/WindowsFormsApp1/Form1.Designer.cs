namespace WindowsFormsApp1
{
    partial class Form1
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent(string docPath, int widthPDF, int heightPDF)
        {
            //this.TopLevel = true;
            this.TopMost = true;
            //this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            
            this.pdfToolStripZoom1 = new Patagames.Pdf.Net.Controls.WinForms.ToolBars.PdfToolStripZoom();
            this.pdfViewer1 = new Patagames.Pdf.Net.Controls.WinForms.PdfViewer();
            //this.pdfToolStripViewModes1 = new Patagames.Pdf.Net.Controls.WinForms.ToolBars.PdfToolStripViewModes();
            this.pdfToolStripPages1 = new Patagames.Pdf.Net.Controls.WinForms.ToolBars.PdfToolStripPages();
            this.pdfToolStripMain1 = new Patagames.Pdf.Net.Controls.WinForms.ToolBars.PdfToolStripMain();
            this.pdfToolStrip1 = new Patagames.Pdf.Net.Controls.WinForms.ToolBars.PdfToolStrip();
            this.SuspendLayout();
            // 
            // pdfToolStripZoom1
            // 
            this.pdfToolStripZoom1.Dock = System.Windows.Forms.DockStyle.None;
            this.pdfToolStripZoom1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.pdfToolStripZoom1.Location = new System.Drawing.Point(0, 0);
            this.pdfToolStripZoom1.Name = "pdfToolStripZoom1";
            this.pdfToolStripZoom1.PdfViewer = this.pdfViewer1;
            this.pdfToolStripZoom1.Size = new System.Drawing.Size(194, 73);
            this.pdfToolStripZoom1.TabIndex = 8;
            this.pdfToolStripZoom1.Text = "pdfToolStripZoom1";
            this.pdfToolStripZoom1.ZoomLevel = new float[] {
        8.33F,
        12.5F,
        25F,
        33.33F,
        50F,
        66.67F,
        75F,
        100F,
        125F,
        150F,
        200F,
        300F,
        400F,
        600F,
        800F};
            // 
            // pdfViewer1
            // 
            this.pdfViewer1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pdfViewer1.CurrentIndex = -1;
            this.pdfViewer1.CurrentPageHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.pdfViewer1.Document = null;
            this.pdfViewer1.FormHighlightColor = System.Drawing.Color.Transparent;
            this.pdfViewer1.FormsBlendMode = Patagames.Pdf.Enums.BlendTypes.FXDIB_BLEND_MULTIPLY;
            this.pdfViewer1.LoadingIconText = "Loading...";
            this.pdfViewer1.Location = new System.Drawing.Point(0, 76);
            this.pdfViewer1.MouseMode = Patagames.Pdf.Net.Controls.WinForms.MouseModes.Default;
            this.pdfViewer1.Name = "pdfViewer1";
            this.pdfViewer1.OptimizedLoadThreshold = 1000;
            this.pdfViewer1.Padding = new System.Windows.Forms.Padding(10);
            this.pdfViewer1.PageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.pdfViewer1.PageAutoDispose = true;
            this.pdfViewer1.PageBackColor = System.Drawing.Color.White;
            this.pdfViewer1.PageBorderColor = System.Drawing.Color.Black;
            this.pdfViewer1.PageMargin = new System.Windows.Forms.Padding(10);
            this.pdfViewer1.PageSeparatorColor = System.Drawing.Color.Gray;
            this.pdfViewer1.RenderFlags = ((Patagames.Pdf.Enums.RenderFlags)((Patagames.Pdf.Enums.RenderFlags.FPDF_LCD_TEXT | Patagames.Pdf.Enums.RenderFlags.FPDF_NO_CATCH)));
            this.pdfViewer1.ShowCurrentPageHighlight = true;
            this.pdfViewer1.ShowLoadingIcon = true;
            this.pdfViewer1.ShowPageSeparator = true;
            this.pdfViewer1.Size = new System.Drawing.Size((widthPDF-20), (heightPDF-100));
            this.pdfViewer1.SizeMode = Patagames.Pdf.Net.Controls.WinForms.SizeModes.FitToHeight;
            this.pdfViewer1.TabIndex = 0;
            this.pdfViewer1.TextSelectColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.pdfViewer1.TilesCount = 2;
            this.pdfViewer1.UseProgressiveRender = true;
            this.pdfViewer1.ViewMode = Patagames.Pdf.Net.Controls.WinForms.ViewModes.Vertical;
            this.pdfViewer1.Zoom = 0.5F;
            this.pdfViewer1.LoadDocument(docPath);
            this.pdfViewer1.DocPath = docPath;
            // 
            // pdfToolStripViewModes1
            // 
            //this.pdfToolStripViewModes1.Dock = System.Windows.Forms.DockStyle.None;
            //this.pdfToolStripViewModes1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            //this.pdfToolStripViewModes1.Location = new System.Drawing.Point(0, 0);
            //this.pdfToolStripViewModes1.Name = "pdfToolStripViewModes1";
            //this.pdfToolStripViewModes1.PdfViewer = null;
            //this.pdfToolStripViewModes1.Size = new System.Drawing.Size(168, 27);
            //this.pdfToolStripViewModes1.TabIndex = 9;
            // 
            // pdfToolStripPages1
            // 
            int heightpdfToolStripPages1 = (widthPDF - (widthPDF / 2));
            heightpdfToolStripPages1 -= 160;
            this.pdfToolStripPages1.Dock = System.Windows.Forms.DockStyle.None;
            this.pdfToolStripPages1.Location = new System.Drawing.Point(heightpdfToolStripPages1, 0);
            this.pdfToolStripPages1.Name = "pdfToolStripPages1";
            this.pdfToolStripPages1.PdfViewer = this.pdfViewer1;
            this.pdfToolStripPages1.Size = new System.Drawing.Size((widthPDF - 700), 75);
            this.pdfToolStripPages1.TabIndex = 4;
            this.pdfToolStripPages1.Text = "pdfToolStripPages1";
            // 
            // pdfToolStripMain1 Print/Close/save buttons
            // 
            this.pdfToolStripMain1.Dock = System.Windows.Forms.DockStyle.None;
            this.pdfToolStripMain1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.pdfToolStripMain1.Location = new System.Drawing.Point((widthPDF - 165), 0);
            this.pdfToolStripMain1.Name = "pdfToolStripMain1";
            this.pdfToolStripMain1.PdfViewer = this.pdfViewer1;
            this.pdfToolStripMain1.Size = new System.Drawing.Size(107, 58);
            this.pdfToolStripMain1.TabIndex = 3;
            this.pdfToolStripMain1.Text = "pdfToolStripMain1";
            this.pdfToolStripMain1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.pdfToolStripMain1_ItemClicked);
            
            // 
            // pdfToolStrip1
            // 
            this.pdfToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.pdfToolStrip1.Name = "pdfToolStrip1";
            this.pdfToolStrip1.PdfViewer = null;
            this.pdfToolStrip1.Size = new System.Drawing.Size((widthPDF - 150), 25);
            this.pdfToolStrip1.TabIndex = 2;
            this.pdfToolStrip1.Text = "pdfToolStrip1";
            // 
            // Form1
            //            
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(widthPDF, (heightPDF-50));
            this.Controls.Add(this.pdfToolStripZoom1);
            this.Controls.Add(this.pdfToolStripPages1);
            //this.Controls.Add(this.pdfToolStripViewModes1);
            this.Controls.Add(this.pdfToolStripMain1);
            this.Controls.Add(this.pdfToolStrip1);
            this.Controls.Add(this.pdfViewer1);
            this.ShowIcon = false;
            this.ControlBox = false;
            this.Name = "POS::PDF Viewer";
            this.Text = "POS::PDF Viewer";
            this.ResumeLayout(true);
            this.PerformLayout();

        }

        #endregion

        private Patagames.Pdf.Net.Controls.WinForms.PdfViewer pdfViewer1;
        private Patagames.Pdf.Net.Controls.WinForms.ToolBars.PdfToolStrip pdfToolStrip1;
        private Patagames.Pdf.Net.Controls.WinForms.ToolBars.PdfToolStripMain pdfToolStripMain1;
        private Patagames.Pdf.Net.Controls.WinForms.ToolBars.PdfToolStripPages pdfToolStripPages1;
        //private Patagames.Pdf.Net.Controls.WinForms.ToolBars.PdfToolStripViewModes pdfToolStripViewModes1;
        private Patagames.Pdf.Net.Controls.WinForms.ToolBars.PdfToolStripZoom pdfToolStripZoom1;
    }
}

