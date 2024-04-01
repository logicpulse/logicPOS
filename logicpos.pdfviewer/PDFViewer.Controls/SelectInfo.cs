namespace Patagames.Pdf.Net.Controls.WinForms
{
	/// <summary>
	/// Represents the information about the selected text in the control
	/// </summary>
	public struct SelectInfo
	{
		/// <summary>
		/// Zero-based index of a starting page.
		/// </summary>
		public int StartPage;
		
		/// <summary>
		/// Zero-based char index on a startPage.
		/// </summary>
		public int StartIndex;

		/// <summary>
		/// Zero-based index of a ending page.
		/// </summary>
		public int EndPage;

		/// <summary>
		/// Zero-based char index on a endPage.
		/// </summary>
		public int EndIndex;
	}
}
