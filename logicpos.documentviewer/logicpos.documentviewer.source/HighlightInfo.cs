using System.Drawing;
namespace Patagames.Pdf.Net.Controls.WinForms
{
	/// <summary>
	/// Represents information about highlighted text in the control
	/// </summary>
	public struct HighlightInfo
	{
        public static bool export;

        private Color _color;

		/// <summary>
		/// The starting character index of the highlighted text.
		/// </summary>
		public int CharIndex;

		/// <summary>
		/// The number of highlighted characters in the text
		/// </summary>
		public int CharsCount;

		/// <summary>
		/// Text highlighted color
		/// </summary>
		public Color Color
		{
			get { return _color; }
			set
			{
				if (_color != value)
					_color = value;
			}
		}
	}
}
