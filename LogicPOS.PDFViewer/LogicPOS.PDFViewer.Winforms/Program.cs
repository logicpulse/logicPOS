using System;
using System.Windows.Forms;

namespace LogicPOS.PDFViewer.Winforms
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var file = args.Length > 0 ? args[0] : "index.pdf";

            Application.Run(new PDFViewer(file));
        }
    }
}
