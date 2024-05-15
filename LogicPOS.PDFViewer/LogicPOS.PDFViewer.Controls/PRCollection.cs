using System;
using System.Collections.Generic;
using System.Drawing;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net.EventArguments;

namespace Patagames.Pdf.Net.Controls.WinForms
{
	internal class PRCollection : Dictionary<PdfPage, PRItem>
	{
        internal event EventHandler<EventArgs<Rectangle>> PaintBackground = null;
		private PdfBitmap _canvasBitmap = null;
		private int _waitTime;
		private long _prevTicks;

		public PdfBitmap CanvasBitmap { get { return _canvasBitmap; } }
		public Size CanvasSize { get; private set; }

        public void InitCanvas(Size size)
		{
			if (_canvasBitmap == null)
			{
				_canvasBitmap = new PdfBitmap(size.Width, size.Height, false, true);
				CanvasSize = size;
			}

			_waitTime = 70;
			_prevTicks = DateTime.Now.Ticks;
		}

		public void ReleaseCanvas()
		{
			foreach (var i in this)
				ReleasePage(i.Key);
			this.Clear();

			if (_canvasBitmap != null)
				_canvasBitmap.Dispose();
			_canvasBitmap = null;
		}

		/// <summary>
		/// Checks whether all visible pages are rendered
		/// </summary>
		/// <value>True if they still need to be painted, False otherwise.</value>
		public bool IsNeedContinuePaint
		{
			get
			{
				foreach (var item in this)
					if (item.Value.status == ProgressiveRenderingStatuses.RenderTobeContinued
						|| item.Value.status == ProgressiveRenderingStatuses.RenderReader)
						return true;
				return false;
			}
		}

		/// <summary>
		/// Checks whether rendering should be paused and control should be returned to UI thread.
		/// </summary>
		/// <returns>True means that application shoul continue painting.</returns>
		internal bool IsNeedPause(PdfPage page)
		{
			if (!this.ContainsKey(page))
				return false;

			var currentTicks = DateTime.Now.Ticks;
			var ms = TimeSpan.FromTicks(currentTicks - _prevTicks).Milliseconds;
			var ret = ms > _waitTime;
			return ret;
		}

		/// <summary>
		/// Start/Continue progressive rendering for specified page
		/// </summary>
		/// <param name="page">Pdf page object</param>
		/// <param name="pageRect">Actual page's rectangle. </param>
		/// <param name="pageRotate">Page orientation: 0 (normal), 1 (rotated 90 degrees clockwise), 2 (rotated 180 degrees), 3 (rotated 90 degrees counter-clockwise).</param>
		/// <param name="renderFlags">0 for normal display, or combination of flags defined above.</param>
		/// <param name="useProgressiveRender">True for use progressive render</param>
		/// <returns>Null if page still painting, PdfBitmap object if page successfully rendered.</returns>
		internal bool RenderPage(PdfPage page, Rectangle pageRect, PageRotate pageRotate, RenderFlags renderFlags, bool useProgressiveRender)
		{
            if (!this.ContainsKey(page))
            {
                ProcessNew(page); //Add new page into collection
                if (PaintBackground != null)
                    PaintBackground(this, new EventArgs<Rectangle>(pageRect));
            }

			if ((renderFlags & (RenderFlags.FPDF_THUMBNAIL | RenderFlags.FPDF_HQTHUMBNAIL)) != 0)
				this[page].status = ProgressiveRenderingStatuses.RenderDone + 4;
			else if (!useProgressiveRender)
				this[page].status = ProgressiveRenderingStatuses.RenderDone + 3;

            PdfBitmap bitmap = this[page].Bitmap;
            bool ret = ProcessExisting(bitmap ?? CanvasBitmap, page, pageRect, pageRotate, renderFlags);
            if (bitmap != null)
                Pdfium.FPDFBitmap_CompositeBitmap(CanvasBitmap.Handle, pageRect.X, pageRect.Y, pageRect.Width, pageRect.Height, bitmap.Handle, pageRect.X, pageRect.Y, BlendTypes.FXDIB_BLEND_NORMAL);
            return ret;
		}

		/// <summary>
		/// Process existing pages
		/// </summary>
		/// <returns>Null if page still painting, PdfBitmap object if page successfully rendered.</returns>
		private bool ProcessExisting(PdfBitmap bitmap, PdfPage page, Rectangle pageRect, PageRotate pageRotate, RenderFlags renderFlags)
		{
			switch (this[page].status)
			{
				case ProgressiveRenderingStatuses.RenderReader:
                    this[page].status = page.StartProgressiveRender(bitmap, pageRect.X, pageRect.Y, pageRect.Width, pageRect.Height, pageRotate, renderFlags, null);
					if (this[page].status == ProgressiveRenderingStatuses.RenderDone)
						return true;
					return false; //Start rendering. Return nothing.

				case ProgressiveRenderingStatuses.RenderDone:
					page.CancelProgressiveRender();
					this[page].status = ProgressiveRenderingStatuses.RenderDone + 2;
					return true; //Stop rendering. Return image.

				case ProgressiveRenderingStatuses.RenderDone + 2:
					return true; //Rendering already stoped. return image

				case ProgressiveRenderingStatuses.RenderDone + 3:
					this[page].status = ProgressiveRenderingStatuses.RenderDone + 2;
                    page.RenderEx(bitmap, pageRect.X, pageRect.Y, pageRect.Width, pageRect.Height, pageRotate, renderFlags);
					return true; //Rendering in non progressive mode

				case ProgressiveRenderingStatuses.RenderDone + 4:
					this[page].status = ProgressiveRenderingStatuses.RenderDone + 2;
					DrawThumbnail(bitmap, page, pageRect, pageRotate, renderFlags);
					return true; //Rendering thumbnails

				case ProgressiveRenderingStatuses.RenderTobeContinued:
					this[page].status = page.ContinueProgressiveRender();
					return false; //Continue rendering. Return nothing.

				case ProgressiveRenderingStatuses.RenderFailed:
				default:
                    bitmap.FillRectEx(pageRect.X, pageRect.Y, pageRect.Width, pageRect.Height, Color.Red.ToArgb());
                    bitmap.FillRectEx(pageRect.X + 5, pageRect.Y + 5, pageRect.Width - 10, pageRect.Height - 10, Color.White.ToArgb());
					page.CancelProgressiveRender();
					this[page].status = ProgressiveRenderingStatuses.RenderDone + 2;
					return true; //An error has occurred. Stop rendering. return special image
            }
		}

		private void DrawThumbnail(PdfBitmap bitmap, PdfPage page, Rectangle pageRect, PageRotate pageRotate, RenderFlags renderFlags)
		{
            int pw = (int)page.Width;
			int ph = (int)page.Height;
			int w = Math.Max(pw, (renderFlags & RenderFlags.FPDF_HQTHUMBNAIL) == RenderFlags.FPDF_HQTHUMBNAIL ? pageRect.Width : pw);
			int h = Math.Max(ph, (renderFlags & RenderFlags.FPDF_HQTHUMBNAIL) == RenderFlags.FPDF_HQTHUMBNAIL ? pageRect.Height : ph);
			using (var bmp = new PdfBitmap(w, h, true))
			{
				page.RenderEx(bmp, 0, 0, w, h, pageRotate, renderFlags);
				using (var g = Graphics.FromImage(bitmap.Image))
				{
					g.DrawImage(bmp.Image, pageRect.X, pageRect.Y, pageRect.Width, pageRect.Height);
				}
			}
		}

		/// <summary>
		/// Adds page into collection
		/// </summary>
		private void ProcessNew(PdfPage page)
		{
            var item = new PRItem(ProgressiveRenderingStatuses.RenderReader, page.IsTransparency ? CanvasSize : new Size(0,0));
			this.Add(page, item);
			page.Disposed += Page_Disposed;
		}

		/// <summary>
		/// Delete page from colection
		/// </summary>
		private void PageRemove(PdfPage page)
		{
			if (this.ContainsKey(page))
			{
				ReleasePage(page);
				this.Remove(page);
			}
		}

		/// <summary>
		/// Stops progressive rendering if need and unsubscribe events
		/// </summary>
		private void ReleasePage(PdfPage page)
		{
			if (this[page].status == ProgressiveRenderingStatuses.RenderTobeContinued)
				page.CancelProgressiveRender();
            this[page].Dispose();
			page.Disposed -= Page_Disposed;
		}

		/// <summary>
		/// Occurs then the page is disposed
		/// </summary>
		private void Page_Disposed(object sender, EventArgs e)
		{
			PageRemove(sender as PdfPage);
		}

	}
}
