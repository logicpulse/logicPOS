using System.IO;

namespace logicpos.printer.genericlinux
{
    public static class Print
    {
        public static void LinuxPrint(string printerName, byte[] docToPrint)
        {
            FileStream printerPort = new FileStream(printerName, FileMode.OpenOrCreate, FileAccess.Write);

            printerPort.Write(docToPrint, 0, docToPrint.Length);

            //Cant Close: Problems in Ubuntu-Mate (Raspberry) 
            //else "System.IO.IOException: Sharing violation on path /dev/usb/lp0"
            //After first print
            //_thermalPrinterStream.Close();
            //Fixed with Flush
            printerPort.Flush();

            printerPort.Close();
        }
    }
}
