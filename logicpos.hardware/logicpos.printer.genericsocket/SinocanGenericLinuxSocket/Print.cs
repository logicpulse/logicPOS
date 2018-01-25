using System;
using System.Net.Sockets;

namespace SinocanGenericSocket
{
    public class Print
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void LinuxPrint(string printerName, byte[] docToPrint)
        {
            try
            {
                string[] printParameters = printerName.Split(':');
                string address = printParameters[0];
                int port = Convert.ToInt16(printParameters[1]);

                System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
                clientSocket.Connect(address, port);
                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = docToPrint;
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
