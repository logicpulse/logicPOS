using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Configuration;

namespace logicpos.printer.genericusb
{
    public static class Print
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		//TK016310 Configuração Impressoras Windows 
        public static void USBPrintWindows(string printerName, byte[] document)
        {
            if (document == null)
                return;
            if (!RawPrinterHelper.SendBytesToPrinter(printerName, document))
                throw new ArgumentException("Unable to access printer : " + printerName);
        }

        public static void USBPrint(string printerName, byte[] document)
        {
            _log.Debug(String.Format("USBPrint to printerName: [{0}], document: [{1}]", printerName, document.ToString()));

            UsbDevice usbDevice;
            UsbEndpointWriter usbWriter;
            ErrorCode usbErrorCode = ErrorCode.None;

            //int vid = Convert.ToInt32(ConfigurationManager.AppSettings["printerVID"], 16);
            //int pid = Convert.ToInt32(ConfigurationManager.AppSettings["printerPID"], 16);
            //WriteEndpointID endpoint = GetEnumFromString(ConfigurationManager.AppSettings["printerEndPoint"]);
            //string printerPort = ConfigurationManager.AppSettings["printerPort"];
            //string[] printerParam = printerPort.Split('|');
            string[] printerParam = printerName.Split('|');
            int vid = Convert.ToInt32(printerParam[0], 16);
            int pid = Convert.ToInt32(printerParam[1], 16);
            WriteEndpointID endpointWrite = GetWriteEndpointFromString(printerParam[2]);
            ReadEndpointID endpointRead = GetReadEndpointFromString(printerParam[2]);

            try
            {
                //Init Usb Finder
                UsbDeviceFinder usbFinder = new UsbDeviceFinder(vid, pid);

                // Find and open the usb device.
                usbDevice = UsbDevice.OpenUsbDevice(usbFinder);

                // If the device is open and ready
                if (usbDevice == null) throw new Exception(string.Format("UsbDisplayDevice: Device NOT Found [ VID:{0}, PID:{1}, ENDPOINT:{2} ]", vid, pid, endpointWrite));

                // If this is a "whole" usb device (libusb-win32, linux libusb)
                // it will have an IUsbDevice interface. If not (WinUSB) the
                // variable will be null indicating this is an interface of a
                // device.
                IUsbDevice wholeUsbDevice = usbDevice as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    // This is a "whole" USB device. Before it can be used,
                    // the desired configuration and interface must be selected.

                    // Select config
                    wholeUsbDevice.SetConfiguration(1);

                    // Claim interface
                    wholeUsbDevice.ClaimInterface(1);
                }

                // open read endpoint
                UsbEndpointReader reader = usbDevice.OpenEndpointReader(endpointRead);
                reader.DataReceived += Reader_DataReceived;

                // open write endpoint
                usbWriter = usbDevice.OpenEndpointWriter(endpointWrite);

                //byte[] document = Encoding.ASCII.GetBytes("Hello World");

                try
                {
                    // write data, read data
                    int bytesWritten;
                    usbErrorCode = usbWriter.Write(document, 2000, out bytesWritten);

                    if (usbErrorCode != ErrorCode.None)
                    {
                        Close(usbDevice);
                        // Write that output to the console.
                        throw new Exception(UsbDevice.LastErrorString);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


            }
            catch (Exception ex)
            {
                _log.Error((usbErrorCode != ErrorCode.None ? usbErrorCode + ":" : String.Empty) + ex.Message);
            }
        }

        private static void Reader_DataReceived(object sender, EndpointDataEventArgs e)
        {
            _log.Debug(string.Format("Reader DataReceived: Count:[{0}], Buffer: [{1}]", e.Count, e.Buffer));
        }

        public static WriteEndpointID GetWriteEndpointFromString(string pValue)
        {
            return (WriteEndpointID)Enum.Parse(typeof(WriteEndpointID), pValue, true);
        }

        public static ReadEndpointID GetReadEndpointFromString(string pValue)
        {
            return (ReadEndpointID)Enum.Parse(typeof(ReadEndpointID), pValue, true);
        }

        public static void Close(UsbDevice usbDevice)
        {
            if (usbDevice != null)
            {
                if (usbDevice.IsOpen)
                {
                    // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                    // it exposes an IUsbDevice interface. If not (WinUSB) the
                    // 'wholeUsbDevice' variable will be null indicating this is
                    // an interface of a device; it does not require or support
                    // configuration and interface selection.
                    IUsbDevice wholeUsbDevice = usbDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // Release interface
                        wholeUsbDevice.ReleaseInterface(1);
                    }

                    usbDevice.Close();
                }
                usbDevice = null;

                // Free usb resources
                UsbDevice.Exit();
            }
        }
    }
}
