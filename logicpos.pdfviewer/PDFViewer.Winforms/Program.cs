using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var width = 1980 / 2;
            var height = 1000;

            Application.Run(new PDFViewer("index.pdf", width,height));
        }
    }
}
