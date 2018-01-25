using System;
using System.Drawing;

namespace logicpulsePosOn
{
	public static class ImageExtensions
	{
		public static Image ScaleToFit(this Image helper, double Height, double Width)
		{
			double percentWidth  = (double) Width  / (double)helper.Width;
			double percentHeight = (double) Height / (double)helper.Height;

			double multiplier = 0;

			if (percentWidth > percentHeight)
				multiplier = percentHeight;
			else
				multiplier = percentWidth;

			return helper.GetThumbnailImage((int)(helper.Width * multiplier), (int)(helper.Height * multiplier), null, IntPtr.Zero);
		}
	}
}

