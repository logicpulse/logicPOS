using System;
using System.Windows.Forms;

namespace Patagames.Pdf.Net.Controls.WinForms.ToolBars
{
	/// <summary>
	/// Provides a container for Windows toolbar objects with predefined functionality for changing pages size mode
	/// </summary>
	public class PdfToolStripSizes : PdfToolStrip
	{
		#region Overriding
		/// <summary>
		/// Create all buttons and add its into toolbar. Override this method to create custom buttons
		/// </summary>
		protected override void InitializeButtons()
		{
			var btn = CreateButton("btnActualSize",
				Properties.PdfToolStrip.btnActualSizeText,
				Properties.PdfToolStrip.btnActualSizeToolTipText,
				Properties.PdfToolStrip.btnActualSizeImage,
				btn_ActualSizeClick);
			this.Items.Add(btn);

			btn = CreateButton("btnFitPage",
				Properties.PdfToolStrip.btnFitPageText,
				Properties.PdfToolStrip.btnFitPageToolTipText,
				Properties.PdfToolStrip.btnFitPageImage,
				btn_FitPageClick);
			this.Items.Add(btn);

			btn = CreateButton("btnFitWidth",
				Properties.PdfToolStrip.btnFitWidthText,
				Properties.PdfToolStrip.btnFitWidthToolTipText,
				Properties.PdfToolStrip.btnFitWidthImage,
				btn_FitWidthClick);
			this.Items.Add(btn);

			btn = CreateButton("btnFitHeight",
				Properties.PdfToolStrip.btnFitHeightText,
				Properties.PdfToolStrip.btnFitHeightToolTipText,
				Properties.PdfToolStrip.btnFitHeightImage,
				btn_FitHeightClick);
			this.Items.Add(btn);
		}

		/// <summary>
		/// Called when the ToolStrip's items need to change its states
		/// </summary>
		protected override void UpdateButtons()
		{
			var tsi = this.Items["btnActualSize"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnFitPage"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnFitWidth"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnFitHeight"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			if (PdfViewer == null || PdfViewer.Document == null)
				return;

			var tsb = this.Items["btnActualSize"] as ToolStripButton;
			if (tsb != null)
				tsb.Checked = ((PdfViewer.SizeMode == SizeModes.Zoom) && (PdfViewer.Zoom >= 1 - 0.00004 && PdfViewer.Zoom <= 1 + 0.00004));

			tsb = this.Items["btnFitPage"] as ToolStripButton;
			if (tsb != null)
				tsb.Checked = (PdfViewer.SizeMode == SizeModes.FitToSize);

			tsb = this.Items["btnFitWidth"] as ToolStripButton;
			if (tsb != null)
				tsb.Checked = (PdfViewer.SizeMode == SizeModes.FitToWidth);

			tsb = this.Items["btnFitHeight"] as ToolStripButton;
			if (tsb != null)
				tsb.Checked = (PdfViewer.SizeMode == SizeModes.FitToHeight);

		}

		/// <summary>
		/// Called when the current PdfViewer control associated with the ToolStrip is changing.
		/// </summary>
		/// <param name="oldValue">PdfViewer control of which was associated with the ToolStrip.</param>
		/// <param name="newValue">PdfViewer control of which will be associated with the ToolStrip.</param>
		protected override void OnPdfViewerChanging(PdfViewer oldValue, PdfViewer newValue)
		{
			base.OnPdfViewerChanging(oldValue, newValue);
			if (oldValue != null)
				UnsubscribePdfViewEvents(oldValue);
			if (newValue != null)
				SubscribePdfViewEvents(newValue);
		}

		#endregion

		#region Event handlers for PdfViewer
		private void PdfViewer_SomethingChanged(object sender, EventArgs e)
		{
			UpdateButtons();
		}
		#endregion

		#region Event handlers for buttons
		private void btn_ActualSizeClick(object sender, System.EventArgs e)
		{
			OnActualSizeClick(this.Items["btnActualSize"] as ToolStripButton);
		}
		private void btn_FitPageClick(object sender, System.EventArgs e)
		{
			OnFitPageClick(this.Items["btnFitPage"] as ToolStripButton);
		}
		private void btn_FitWidthClick(object sender, System.EventArgs e)
		{
			OnFitWidthClick(this.Items["btnFitWidth"] as ToolStripButton);
		}
		private void btn_FitHeightClick(object sender, System.EventArgs e)
		{
			OnFitHeightClick(this.Items["btnFitHeight"] as ToolStripButton);
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// Occurs when the Actual Size button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnActualSizeClick(ToolStripButton item)
		{
			UnsubscribePdfViewEvents(PdfViewer);
			PdfViewer.SizeMode = SizeModes.Zoom;
			PdfViewer.Zoom = 1;
			SubscribePdfViewEvents(PdfViewer);
			UpdateButtons();
		}

		/// <summary>
		/// Occurs when the Fit Page button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnFitPageClick(ToolStripButton item)
		{
			PdfViewer.SizeMode = SizeModes.FitToSize;
		}

		/// <summary>
		/// Occurs when the Fit Width button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnFitWidthClick(ToolStripButton item)
		{
			PdfViewer.SizeMode = SizeModes.FitToWidth;
		}

		/// <summary>
		/// Occurs when the Fit Height button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnFitHeightClick(ToolStripButton item)
		{
			PdfViewer.SizeMode = SizeModes.FitToHeight;
		}

		#endregion

		#region Private methods
		private void UnsubscribePdfViewEvents(PdfViewer oldValue)
		{
			oldValue.AfterDocumentChanged -= PdfViewer_SomethingChanged;
			oldValue.DocumentLoaded -= PdfViewer_SomethingChanged;
			oldValue.DocumentClosed -= PdfViewer_SomethingChanged;
			oldValue.SizeModeChanged -= PdfViewer_SomethingChanged;
			oldValue.ZoomChanged -= PdfViewer_SomethingChanged;
		}

		private void SubscribePdfViewEvents(PdfViewer newValue)
		{
			newValue.AfterDocumentChanged += PdfViewer_SomethingChanged;
			newValue.DocumentLoaded += PdfViewer_SomethingChanged;
			newValue.DocumentClosed += PdfViewer_SomethingChanged;
			newValue.SizeModeChanged += PdfViewer_SomethingChanged;
			newValue.ZoomChanged += PdfViewer_SomethingChanged;
		}

		#endregion
	}
}
