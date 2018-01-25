namespace logicpos.printer.generic
{
    /// <summary>
    /// A listing of byte control characters for readability.
    /// </summary>
    public static class AsciiControlChars
    {
        /// <summary>
        /// Usually indicates the end of a string.
        /// </summary>
        public const byte Nul = 0;

        /// <summary>
        /// Meant to be used for printers. When receiving this code the 
        /// printer moves to the next sheet of paper.
        /// </summary>
        public const byte FormFeed = 12;

        /// <summary>
        /// Starts an extended sequence of control codes.
        /// </summary>
        public const byte Escape = 27;

        /// <summary>
        /// Advances to the next line.
        /// </summary>
        public const byte Newline = 10;

        /// <summary>
        /// Defined to separate tables or different sets of data in a serial
        /// data storage system.
        /// </summary>
        public const byte GroupSeparator = 29;

        /// <summary>
        /// A horizontal tab.
        /// </summary>
        public const byte HorizontalTab = 09;

        /// <summary>
        /// Returns the carriage to the start of the line.
        /// </summary>
        public const byte CarriageReturn = 13;

        /// <summary>
        /// Cancels the operation.
        /// </summary>
        public const byte Cancel = 24;

        /// <summary>
        /// Indicates that control characters present in the stream should
        /// be passed through as transmitted and not interpreted as control
        /// characters.
        /// </summary>
        public const byte DataLinkEscape = 16;

        /// <summary>
        /// Signals the end of a transmission.
        /// </summary>
        public const byte EndOfTransmission = 04;

        /// <summary>
        /// In serial storage, signals the separation of two files.
        /// </summary>
        public const byte FileSeparator = 28;
        public const byte PrintAndLineFeed = 10;
        public const byte SelectCharacterSize = 29;
        public const byte ReversePrintingMode = 66;
        public const byte CharacterSpacing = 32;
        public const byte SelectPrintMode = 33;
        public const byte TurnUnderline = 45;
        public const byte TurnEmphasized = 69;
        public const byte PrintAndFeedNLines = 100;
        public const byte SetLineSpacing = 51;
        public const byte SelectJustification = 97;
        public const byte InitializePrinter = 64;
        public const byte PrintBarcode = 107;
        public const byte SetBarcodeWidth = 119;
        public const byte SetBarcodeHeight = 105;
        public const byte SelectCutModeAndCutPaper = 86;
        public const byte SelectBitImageMode = 42;
        public const byte SetPeripheralDevice = 61;
        public const byte PrintAndFeedPaper = 74;
        public const byte SelectCharacterCodeTable = 116;
        public const byte SelectInternationalCharacterSet = 82;
        public const byte DLE = 16;
        public const byte GeneratePulseRealtime = 20;
        public const byte GeneratePulse = 112;

        
     
    }
}