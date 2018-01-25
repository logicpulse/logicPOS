namespace logicpos.printer.genericwindows
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains native methods invoked via P/Invoke to the underlying Windows
    /// operating system. Only supported on NT platforms.
    /// </summary>
    public static class NativeMethods
    {
        /// <summary>
        /// An enumeration of GetDeviceCaps parameters.
        /// </summary>
        public enum DeviceCap : int
        {
            /// <summary>
            /// Device driver version
            /// </summary>
            DRIVERVERSION = 0,

            /// <summary>
            /// Device classification
            /// </summary>
            TECHNOLOGY = 2,

            /// <summary>
            /// Horizontal size in millimeters
            /// </summary>
            HORZSIZE = 4,

            /// <summary>
            /// Vertical size in millimeters
            /// </summary>
            VERTSIZE = 6,

            /// <summary>
            /// Horizontal width in pixels
            /// </summary>
            HORZRES = 8,

            /// <summary>
            /// Vertical height in pixels
            /// </summary>
            VERTRES = 10,

            /// <summary>
            /// Number of bits per pixel
            /// </summary>
            BITSPIXEL = 12,

            /// <summary>
            /// Number of planes
            /// </summary>
            PLANES = 14,

            /// <summary>
            /// Number of brushes the device has
            /// </summary>
            NUMBRUSHES = 16,

            /// <summary>
            /// Number of pens the device has
            /// </summary>
            NUMPENS = 18,

            /// <summary>
            /// Number of markers the device has
            /// </summary>
            NUMMARKERS = 20,

            /// <summary>
            /// Number of fonts the device has
            /// </summary>
            NUMFONTS = 22,

            /// <summary>
            /// Number of colors the device supports
            /// </summary>
            NUMCOLORS = 24,

            /// <summary>
            /// Size required for device descriptor
            /// </summary>
            PDEVICESIZE = 26,

            /// <summary>
            /// Curve capabilities
            /// </summary>
            CURVECAPS = 28,

            /// <summary>
            /// Line capabilities
            /// </summary>
            LINECAPS = 30,

            /// <summary>
            /// Polygonal capabilities
            /// </summary>
            POLYGONALCAPS = 32,

            /// <summary>
            /// Text capabilities
            /// </summary>
            TEXTCAPS = 34,

            /// <summary>
            /// Clipping capabilities
            /// </summary>
            CLIPCAPS = 36,

            /// <summary>
            /// Bitblt capabilities
            /// </summary>
            RASTERCAPS = 38,

            /// <summary>
            /// Length of the X leg
            /// </summary>
            ASPECTX = 40,

            /// <summary>
            /// Length of the Y leg
            /// </summary>
            ASPECTY = 42,

            /// <summary>
            /// Length of the hypotenuse
            /// </summary>
            ASPECTXY = 44,

            /// <summary>
            /// Shading and Blending caps
            /// </summary>
            SHADEBLENDCAPS = 45,

            /// <summary>
            /// Logical pixels inch in X
            /// </summary>
            LOGPIXELSX = 88,

            /// <summary>
            /// Logical pixels inch in Y
            /// </summary>
            LOGPIXELSY = 90,

            /// <summary>
            /// Number of entries in physical palette
            /// </summary>
            SIZEPALETTE = 104,

            /// <summary>
            /// Number of reserved entries in palette
            /// </summary>
            NUMRESERVED = 106,

            /// <summary>
            /// Actual color resolution
            /// </summary>
            COLORRES = 108,

            /// <summary>
            /// Physical Width in device units
            /// </summary>
            PHYSICALWIDTH = 110,

            /// <summary>
            /// Physical Height in device units
            /// </summary>
            PHYSICALHEIGHT = 111,

            /// <summary>
            /// Physical Printable Area x margin
            /// </summary>
            PHYSICALOFFSETX = 112,

            /// <summary>
            /// Physical Printable Area y margin
            /// </summary>
            PHYSICALOFFSETY = 113,

            /// <summary>
            /// Scaling factor x
            /// </summary>
            SCALINGFACTORX = 114,

            /// <summary>
            /// Scaling factor y
            /// </summary>
            SCALINGFACTORY = 115,

            /// <summary>
            /// Current vertical refresh rate of the display device (for displays only) in Hz
            /// </summary>
            VREFRESH = 116,

            /// <summary>
            /// Horizontal width of entire desktop in pixels
            /// </summary>
            DESKTOPVERTRES = 117,

            /// <summary>
            /// Vertical height of entire desktop in pixels
            /// </summary>
            DESKTOPHORZRES = 118,

            /// <summary>
            /// Preferred blt alignment
            /// </summary>
            BLTALIGNMENT = 119
        }

        /// <summary>
        /// The CreateDC function creates a device context (DC) for a device 
        /// using the specified name.
        /// </summary>
        /// <param name="lpszDriver">Pointer to a null-terminated character
        /// string that specifies either DISPLAY or the name of a specific 
        /// display device or the name of a print provider, which is usually WINSPOOL.</param>
        /// <param name="lpszDevice">Pointer to a null-terminated character string 
        /// that specifies the name of the specific output device being used, 
        /// as shown by the Print Manager (for example, Epson FX-80). It is not 
        /// the printer model name. The lpszDevice parameter must be used.</param>
        /// <param name="lpszOutput">This parameter is ignored and should be set
        /// to NULL. It is provided only for compatibility with 16-bit Windows.</param>
        /// <param name="lpInitData">Pointer to a DEVMODE structure containing 
        /// device-specific initialization data for the device driver. The 
        /// DocumentProperties function retrieves this structure filled in for
        /// a specified device. The lpInitData parameter must be NULL if the
        /// device driver is to use the default initialization (if any) specified
        /// by the user.</param>
        /// <returns>If the function succeeds, the return value is the handle
        /// to a DC for the specified device. If the function fails, the 
        /// return value is NULL. The function will return NULL for a DEVMODE
        /// structure other than the current DEVMODE.</returns>
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateDC(
            string lpszDriver,
            string lpszDevice,
            string lpszOutput,
            IntPtr lpInitData);

        /// <summary>
        /// The DeleteDC function deletes the specified device context (DC).
        /// </summary>
        /// <param name="hdc">Handle to the device context.</param>
        /// <returns>If the function succeeds, the return value is nonzero. 
        /// If the function fails, the return value is zero.</returns>
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteDC(IntPtr hdc);

        [DllImport(
            "winspool.drv",
            EntryPoint = "OpenPrinterW",
            SetLastError = true,
            CharSet = CharSet.Unicode,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool OpenPrinter(
            [MarshalAs(UnmanagedType.LPWStr)] string szPrinter,
            out IntPtr hPrinter,
            IntPtr pd);

        /// <summary>
        /// Closes the specified printer object.
        /// </summary>
        /// <param name="hPrinter">Handle to the printer object to be closed.
        /// This handle is returned by the OpenPrinter or AddPrinter function.</param>
        /// <returns>If the function succeeds, the return value is a nonzero value.
        /// If the function fails, the return value is zero</returns>
        [DllImport(
            "winspool.drv",
            EntryPoint = "ClosePrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ClosePrinter(IntPtr hPrinter);

        /// <summary>
        /// The StartDoc function starts a print job.
        /// </summary>
        /// <param name="hdc">Handle to the device context for the print job.</param>
        /// <param name="lpdi">Pointer to a DOCINFO structure containing the name 
        /// of the document file and the name of the output file.</param>
        /// <returns>If the function succeeds, the return value is greater than
        /// zero. This value is the print job identifier for the document.</returns>
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int StartDoc(IntPtr hdc, DOCINFO lpdi);

        /// <summary>
        /// The EndDoc function ends a print job.
        /// </summary>
        /// <param name="hdc">Handle to the device context for the print job.</param>
        /// <returns>If the function succeeds, the return value is greater than zero.
        /// If the function fails, the return value is less than or equal
        /// to zero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern int EndDoc(IntPtr hdc);

        /// <summary>
        /// The GetDeviceCaps function retrieves device-specific information 
        /// for the specified device.
        /// </summary>
        /// <param name="hdc">Handle to the DC.</param>
        /// <param name="capindex">Specifies the item to return.</param>
        /// <returns>The return value specifies the value of the desired item.</returns>
        [DllImport("gdi32.dll")]
        internal static extern int GetDeviceCaps(IntPtr hdc, DeviceCap capindex);

        /// <summary>
        /// The StartPage function prepares the printer driver to accept data.
        /// </summary>
        /// <param name="hdc">Handle to the device context for the print job.</param>
        /// <returns>If the function succeeds, the return value is greater than zero.
        /// If the function fails, the return value is less than or equal to zero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern int StartPage(IntPtr hdc);

        /// <summary>
        /// The EndPage function notifies the device that the application has
        /// finished writing to a page. This function is typically used to 
        /// direct the device driver to advance to a new page.
        /// </summary>
        /// <param name="hdc">Handle to the device context for the print job.</param>
        /// <returns>If the function succeeds, the return value is greater than zero.
        /// If the function fails, the return value is less than or equal to zero.</returns>
        [DllImport("gdi32.dll")]
        internal static extern int EndPage(IntPtr hdc);

        /// <summary>
        /// The StartDocPrinter function notifies the print spooler
        /// that a document is to be spooled for printing.
        /// </summary>
        /// <param name="hPrinter">Handle to the printer. Use the OpenPrinter or
        /// AddPrinter function to retrieve a printer handle.</param>
        /// <param name="level">Specifies the version of the structure to 
        /// which pDocInfo points. On WIndows NT/2000/XP, the value must be 1.</param>
        /// <param name="di">Pointer to a structure that describes the document to print.</param>
        /// <returns>If the function succeeds, the return value identifies the print job.
        /// If the function fails, the return value is zero. </returns>
        [DllImport(
            "winspool.drv",
            EntryPoint = "StartDocPrinterW",
            SetLastError = true,
            CharSet = CharSet.Unicode,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool StartDocPrinter(
            IntPtr hPrinter,
            int level,
            [In, MarshalAs(UnmanagedType.LPStruct)] DOC_INFO_1 di);

        [DllImport(
            "winspool.drv",
            EntryPoint = "EndDocPrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport(
            "winspool.drv",
            EntryPoint = "StartPagePrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport(
            "winspool.drv",
            EntryPoint = "EndPagePrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport(
            "winspool.drv",
            EntryPoint = "WritePrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WritePrinter(
            IntPtr hPrinter,
            IntPtr pBytes,
            int dwCount,
            out int dwWritten);

        /// <summary>
        /// The DOCINFO structure contains the input and output file names and 
        /// other information used by the StartDoc function.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal class DOCINFO
        {
            /// <summary>
            /// The size, in bytes, of the structure.
            /// </summary>
            public int cbSize = 20;

            /// <summary>
            /// Pointer to a null-terminated string that specifies the name
            /// of the document.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszDocName;

            /// <summary>
            /// Pointer to a null-terminated string that specifies the name of 
            /// an output file. If this pointer is NULL, the output will be 
            /// sent to the device identified by the device context handle that 
            /// was passed to the StartDoc function.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszOutput;

            /// <summary>
            /// Pointer to a null-terminated string that specifies the type of 
            /// data used to record the print job. The legal values for this 
            /// member can be found by calling EnumPrintProcessorDatatypes and 
            /// can include such values as raw, emf, or XPS_PASS. This member 
            /// can be NULL. Note that the requested data type might be ignored.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszDatatype;

            /// <summary>
            /// Specifies additional information about the print job. This 
            /// member must be zero or one of the following values.
            /// </summary>
            public int fwType;
        }

        /// <summary>
        /// The DOC_INFO_1 structure describes a document that will be printed.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal class DOC_INFO_1
        {
            /// <summary>
            /// Pointer to a null-terminated string that specifies the name of
            /// the document.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDocName;

            /// <summary>
            /// Pointer to a null-terminated string that specifies the name of
            /// an output file. To print to a printer, set this to NULL.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pOutputFile;

            /// <summary>
            /// Pointer to a null-terminated string that identifies the type 
            /// of data used to record the document.
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDataType;
        }
    }
}
