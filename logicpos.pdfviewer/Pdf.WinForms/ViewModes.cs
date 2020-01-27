namespace Patagames.Pdf.Net.Controls.WinForms
{
	/// <summary>
	/// Specifies how the PdfViewer will display pages
	/// </summary>
	public enum ViewModes
	{
        /// <summary>
        /// View pages continuously with scrolling enabled
        /// </summary>
        Vertical,

        /// <summary>
        /// View pages continuously with scrolling at horizontal dimension
        /// </summary>
        Horizontal,

        /// <summary>
        /// View pages side-by-side with continuous scrolling enabled
        /// </summary>
        TilesVertical,

        /// <summary>
        /// View pages side-by-side with continuous scrolling at horizontal dimension
        /// </summary>
        TilesHorizontal,

        /// <summary>
        /// View one page at a time.
        /// </summary>
        SinglePage,

        /// <summary>
        /// View pages side-by-side
        /// </summary>
        TilesLine
    }
}
