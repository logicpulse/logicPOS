 using System;
using System.Windows.Forms;

namespace Patagames.Pdf.Net.Controls.WinForms.ToolBars
{
	/// <summary>
	/// Provides a container for Windows toolbar objects with predefined functionality for working with clipboard
	/// </summary>
	public class PdfToolStripClipboard : PdfToolStrip
	{
		#region Overriding
		/// <summary>
		/// Create all buttons and add its into toolbar. Override this method to create custom buttons
		/// </summary>
		protected override void InitializeButtons()
		{
			var btn = CreateButton("btnSelectAll",
				Properties.PdfToolStrip.btnSelectAllText,
				Properties.PdfToolStrip.btnSelectAllToolTipText,
				Properties.PdfToolStrip.btnSelectAllImage,
				btn_SelectAllClick);
			this.Items.Add(btn);

			btn = CreateButton("btnCopy",
				Properties.PdfToolStrip.btnCopyText,
				Properties.PdfToolStrip.btnCopyToolTipText,
				Properties.PdfToolStrip.btnCopyImage,
				btn_CopyClick);
			this.Items.Add(btn);
		}

		/// <summary>
		/// Called when the ToolStrip's items need to change its states
		/// </summary>
		protected override void UpdateButtons()
		{
			var tsi = this.Items["btnSelectAll"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnCopy"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			if (PdfViewer == null || PdfViewer.Document == null)
				return;

			var tsb = this.Items["btnCopy"] as ToolStripButton;
			if (tsb != null)
				tsb.Enabled = PdfViewer.SelectedText.Length > 0;


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
		private void btn_SelectAllClick(object sender, System.EventArgs e)
		{
			OnSelectAllClick(this.Items["btnSelectAll"] as ToolStripButton);
		}
		private void btn_CopyClick(object sender, System.EventArgs e)
		{
			OnCopyClick(this.Items["btnCopy"] as ToolStripButton);
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// Occurs when the Select All button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnSelectAllClick(ToolStripButton item)
		{
			PdfViewer.SelectText(0, 0, PdfViewer.Document.Pages.Count - 1, PdfViewer.Document.Pages[PdfViewer.Document.Pages.Count - 1].Text.CountChars);
		}

		/// <summary>
		/// Occurs when the Copy button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnCopyClick(ToolStripButton item)
		{
			Clipboard.SetText(PdfViewer.SelectedText);
		}

		#endregion

		#region Private methods
		private void UnsubscribePdfViewEvents(PdfViewer oldValue)
		{
			oldValue.AfterDocumentChanged -= PdfViewer_SomethingChanged;
			oldValue.DocumentLoaded -= PdfViewer_SomethingChanged;
			oldValue.DocumentClosed -= PdfViewer_SomethingChanged;
			oldValue.SelectionChanged -= PdfViewer_SomethingChanged;
		}

		private void SubscribePdfViewEvents(PdfViewer newValue)
		{
			newValue.AfterDocumentChanged += PdfViewer_SomethingChanged;
			newValue.DocumentLoaded += PdfViewer_SomethingChanged;
			newValue.DocumentClosed += PdfViewer_SomethingChanged;
			newValue.SelectionChanged += PdfViewer_SomethingChanged;
		}

		#endregion

	}
}
