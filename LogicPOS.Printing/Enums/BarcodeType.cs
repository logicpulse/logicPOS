namespace LogicPOS.Printing.Enums
{
    /// <summary>
    /// List of supported barcode types.
    /// </summary>
    public enum BarcodeType
    {
        /// <summary>
        /// UPC-A
        /// </summary>
        upc_a = 0,
        /// <summary>
        /// UPC-E
        /// </summary>
        upc_e = 1,
        /// <summary>
        /// EAN13
        /// </summary>
        ean13 = 2,
        /// <summary>
        /// EAN8
        /// </summary>
        ean8 = 3,
        /// <summary>
        /// CODE 39
        /// </summary>
        code39 = 4,
        /// <summary>
        /// I25
        /// </summary>
        i25 = 5,
        /// <summary>
        /// CODEBAR
        /// </summary>
        codebar = 6,
        /// <summary>
        /// CODE 93
        /// </summary>
        code93 = 7,
        /// <summary>
        /// CODE 128
        /// </summary>
        code128 = 8,
        /// <summary>
        /// CODE 11
        /// </summary>
        code11 = 9,
        /// <summary>
        /// MSI
        /// </summary>
        msi = 10,
        /// <summary>
        /// QRCode
        /// </summary>
        qrcode = 12
    }
}

