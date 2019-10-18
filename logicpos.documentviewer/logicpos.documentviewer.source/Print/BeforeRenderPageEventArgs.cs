using System;
using Patagames.Pdf.Enums;

namespace Patagames.Pdf.Net.Controls.WinForms
{
	/// <summary>
	/// Represents the class that contain event data for the PrintPageLoaded event.
	/// </summary>
	public class BeforeRenderPageEventArgs : EventArgs
	{
		/// <summary>
		/// The page what will be printed.
		/// </summary>
		public PdfPage Page { get; private set; }

		/// <summary>
		/// The page's width calculated to match the sheet size.
		/// </summary>
		public double Width { get; private set; }

		/// <summary>
		/// The page's height calculated to match the sheet size.
		/// </summary>
		public double Height { get; private set; }

		/// <summary>
		/// The page rotation.
		/// </summary>
		public PageRotate Rotation { get; private set; }


		/// <summary>
		/// Construct new instnace of the PrintPageLoadedEventArgs class
		/// </summary>
		/// <param name="page">The page what will be printed.</param>
		/// <param name="width">The page's width calculated to match the sheet size.</param>
		/// <param name="height">The page's height calculated to match the sheet size.</param>
		/// <param name="rotation">The page rotation.</param>
		public BeforeRenderPageEventArgs(PdfPage page, double width, double height, PageRotate rotation)
		{
			Page = page;
			Width = width;
			Height = height;
			Rotation = rotation;
		}
	}
}