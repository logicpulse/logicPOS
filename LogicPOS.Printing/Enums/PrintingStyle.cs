namespace LogicPOS.Printing.Enums
{

    public enum PrintingStyle
    {
        /// <summary>
        /// White on black.
        /// </summary>
        Reverse = 1 << 1,
        /// <summary>
        /// Updown characters.
        /// </summary>
        Updown = 1 << 2,
        /// <summary>
        /// Bold characters.
        /// </summary>
        Bold = 1 << 3,
        /// <summary>
        /// Double height characters.
        /// </summary>
        DoubleHeight = 1 << 4,
        /// <summary>
        /// Double width characters.
        /// </summary>
        DoubleWidth = 1 << 5,
        /// <summary>
        /// Strikes text.
        /// </summary>
        DeleteLine = 1 << 6,
        /// <summary>
        /// Thin underline.
        /// </summary>
        Underline = 1 << 0,
        /// <summary>
        /// Thick underline.
        /// </summary>
        ThickUnderline = 1 << 7
    }


}

