using Patagames.Pdf.Enums;
using System.Drawing;
using System;

namespace Patagames.Pdf.Net.Controls.WinForms
{
	internal class PRItem : IDisposable
	{
		public ProgressiveRenderingStatuses status;
        public PdfBitmap Bitmap;

        public PRItem(ProgressiveRenderingStatuses status, Size canvasSize)
        {
            this.status = status;
            if(canvasSize.Width>0 && canvasSize.Height>0)
                Bitmap = new PdfBitmap(canvasSize.Width, canvasSize.Height, true);
        }

        public void Dispose()
        {
            if (Bitmap != null)
                Bitmap.Dispose();
            Bitmap = null;
        }

    }
}
