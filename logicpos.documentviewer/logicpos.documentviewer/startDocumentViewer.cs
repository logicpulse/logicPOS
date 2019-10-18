using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Patagames.Pdf.Net.Controls.WinForms;


namespace logicpos.documentviewer
{  
    public partial class startDocumentViewer : Form
    {
        
        public startDocumentViewer(string docPath, int widthPDF, int heightPDF, bool export)
        {
           
            InitializeComponent(docPath, widthPDF, heightPDF, export);
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
