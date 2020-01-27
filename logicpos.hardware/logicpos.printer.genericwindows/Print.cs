using System;
using System.Runtime.InteropServices;

namespace logicpos.printer.genericwindows
{
    public static class Print
    {
		//TK016310 Configuração Impressoras Windows 
        public static void USBPrintWindows(string printerName, byte[] document)
        {
            if (document == null)
                return;
            if (!RawPrinterHelper.SendBytesToPrinter(printerName, document))
                throw new ArgumentException("Unable to access printer : " + printerName);
        }
        public static void WindowsPrint(string printerName, byte[] document)
        {
            NativeMethods.DOC_INFO_1 documentInfo;
            IntPtr printerHandle;

            documentInfo = new NativeMethods.DOC_INFO_1();
            documentInfo.pDataType = "RAW";
            documentInfo.pDocName = "LogicPulse";

            printerHandle = new IntPtr(0);

            if (NativeMethods.OpenPrinter(printerName.Normalize(), out printerHandle, IntPtr.Zero))
            {
                if (NativeMethods.StartDocPrinter(printerHandle, 1, documentInfo))
                {
                    int bytesWritten;
                    byte[] managedData;
                    IntPtr unmanagedData;

                    managedData = document;
                    unmanagedData = Marshal.AllocCoTaskMem(managedData.Length);
                    Marshal.Copy(managedData, 0, unmanagedData, managedData.Length);

                    if (NativeMethods.StartPagePrinter(printerHandle))
                    {
                        NativeMethods.WritePrinter(
                            printerHandle,
                            unmanagedData,
                            managedData.Length,
                            out bytesWritten);
                        NativeMethods.EndPagePrinter(printerHandle);
                    }
                    else
                    {
                        throw new System.ComponentModel.Win32Exception();
                    }

                    Marshal.FreeCoTaskMem(unmanagedData);

                    NativeMethods.EndDocPrinter(printerHandle);
                }
                else
                {
                    throw new System.ComponentModel.Win32Exception();
                }

                NativeMethods.ClosePrinter(printerHandle);
            }
            else
            {
                throw new System.ComponentModel.Win32Exception();
            }
        }
    }
}
