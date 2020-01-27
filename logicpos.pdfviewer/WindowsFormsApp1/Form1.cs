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



namespace WindowsFormsApp1
{  
    public partial class Form1 : Form
    {
        
        public Form1(string docPath, int widthPDF, int heightPDF)
        {
           
            InitializeComponent(docPath, widthPDF, heightPDF);
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
