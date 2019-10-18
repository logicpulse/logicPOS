namespace Patagames.Pdf.Net.Controls.WinForms
{
	/// <summary>
	/// Specifies how the PdfViewer will process mouse events
	/// </summary>
	public enum MouseModes
	{
		/// <summary>
		/// By default. Select text, process links
		/// </summary>
		Default,

		/// <summary>
		/// Any action is missing
		/// </summary>
		None,

		/// <summary>
		/// Select text only
		/// </summary>
		SelectTextTool,

		/// <summary>
		/// Move the page
		/// </summary>
		PanTool,
	}
}
