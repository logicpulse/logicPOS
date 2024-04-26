using System;
using System.Windows.Forms;

namespace logicpos.documentviewer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new startDocumentViewer("C://Suporte POS//Novo Documento 2019-09-18 12.03.32.pdf", 1980/2,1000, false));
        }
    }
}
