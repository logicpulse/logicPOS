using System;
using System.Net.Sockets;

//NetWork Name ex : IP:PORT | 192.168.1.30:9100

namespace logicpos.printer.genericsocket
{
    public static class Print
    {
        private static int timeOut = 1000;

        public static void SocketPrint(string printerName, byte[] docToPrint)
        {
            try
            {
                string[] printParameters = printerName.Split(':');
                string address = printParameters[0];
                int port = Convert.ToInt16(printParameters[1]);
                System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();

                if (clientSocket.ConnectAsync(address, port).Wait(timeOut))
                {
                    //clientSocket.Connect(address, port);
                    NetworkStream serverStream = clientSocket.GetStream();
                    byte[] outStream = docToPrint;
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();
                    serverStream.Close();
                }
                else
                {
                    throw new Exception(string.Format("SOCKET_PRINT_TIMEOUT on {0}:{1}", address, port));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
