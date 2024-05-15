namespace Patagames.Pdf.Net.Controls.WinForms
{
	/// <summary>
	/// Specifies how an pdf page is positioned within a PdfView control.
	/// </summary>
	public enum SizeModes
	{
		/// <summary>
		/// The page is scaled to fit the page to worksheet.
		/// </summary>
		FitToSize,

		/// <summary>
		/// Phe page is scaled to fit the width to worksheet.
		/// </summary>
		FitToWidth,

		/// <summary>
		/// The page is scaled to fit the height to worksheet.
		/// </summary>
		FitToHeight,

		/// <summary>
		/// The page is scaled with specified coefficient.
		/// </summary>
		Zoom
	}
}
