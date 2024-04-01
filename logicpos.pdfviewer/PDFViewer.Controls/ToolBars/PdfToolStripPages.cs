using System;
using System.Windows.Forms;

namespace Patagames.Pdf.Net.Controls.WinForms.ToolBars
{
	/// <summary>
	/// Provides a container for Windows toolbar objects with predefined functionality for working with pages
	/// </summary>
	public class PdfToolStripPages : PdfToolStrip
	{
		#region Constructor, Destructor, Initialisation
		private ToolStripItem CreateTextBox()
		{
			var btn = new ToolStripTextBox("btnPageNumber");
			btn.TextBox.Width = 70;
			btn.TextBox.TextAlign = HorizontalAlignment.Center;
			btn.TextBox.KeyDown += btnPageNumber_KeyDown;
			return btn;
		}

		#endregion

		#region Overriding
		/// <summary>
		/// Create all buttons and add its into toolbar. Override this method to create custom buttons
		/// </summary>
		protected override void InitializeButtons()
		{
			var btn = CreateButton("btnFirstPage",
				Properties.PdfToolStrip.btnFirstPageText,
				Properties.PdfToolStrip.btnFirstPageToolTipText,
				Properties.PdfToolStrip.btnFirstPageImage,
				btn_FirstPageClick,
				System.Windows.Forms.ToolStripItemDisplayStyle.Image);
			this.Items.Add(btn);

			btn = CreateButton("btnPreviousPage",
				Properties.PdfToolStrip.btnPreviousPageText,
				Properties.PdfToolStrip.btnPreviousPageToolTipText,
				Properties.PdfToolStrip.btnPreviousPageImage,
				btn_PreviousPageClick,
				System.Windows.Forms.ToolStripItemDisplayStyle.Image);
			this.Items.Add(btn);

			btn = CreateTextBox();
			this.Items.Add(btn);

			btn = CreateButton("btnNextPage",
				Properties.PdfToolStrip.btnNextPageText,
				Properties.PdfToolStrip.btnNextPageToolTipText,
				Properties.PdfToolStrip.btnNextPageImage,
				btn_NextPageClick,
				System.Windows.Forms.ToolStripItemDisplayStyle.Image);
			this.Items.Add(btn);

			btn = CreateButton("btnLastPage",
				Properties.PdfToolStrip.btnLastPageText,
				Properties.PdfToolStrip.btnLastPageToolTipText,
				Properties.PdfToolStrip.btnLastPageImage,
				btn_LastPageClick,
				System.Windows.Forms.ToolStripItemDisplayStyle.Image);
			this.Items.Add(btn);
		}

		/// <summary>
		/// Called when the ToolStrip's items need to change its states
		/// </summary>
		protected override void UpdateButtons()
		{
			var tsi = this.Items["btnFirstPage"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnPreviousPage"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnNextPage"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnLastPage"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnPageNumber"];
			if (tsi == null)
				return;
			tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			var tb = (tsi as ToolStripTextBox).TextBox;
			if (tb == null)
				return;

			if (PdfViewer == null || PdfViewer.Document == null)
				tb.Text = "";
			else
				tb.Text = string.Format("{0} / {1}", PdfViewer.Document.Pages.CurrentIndex + 1, PdfViewer.Document.Pages.Count);
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
		private void btnPageNumber_KeyDown(object sender, KeyEventArgs e)
		{
			OnPageNumberKeyDown(this.Items["btnPageNumber"] as ToolStripTextBox, e);
		}

		private void btn_FirstPageClick(object sender, EventArgs e)
		{
			OnToBeginClick(this.Items["btnFirstPage"] as ToolStripButton);
		}

		private void btn_PreviousPageClick(object sender, System.EventArgs e)
		{
			OnToLeftClick(this.Items["btnPreviousPage"] as ToolStripButton);
		}
		private void btn_NextPageClick(object sender, System.EventArgs e)
		{
			OnToRightClick(this.Items["btnNextPage"] as ToolStripButton);
		}
		private void btn_LastPageClick(object sender, System.EventArgs e)
		{
			OnToEndClick(this.Items["btnLastPage"] as ToolStripButton);
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// Occurs when a key is pressed and held down while the PageNumber textbox has focus.
		/// </summary>
		/// <param name="item">PageNumber item</param>
		/// <param name="e">Key event args</param>
		protected virtual void OnPageNumberKeyDown(ToolStripTextBox item, KeyEventArgs e)
		{
			if (item == null)
				return;
			if (e.KeyCode == Keys.Enter)
			{
				int pn = 0;
				string text = item.TextBox.Text;
				char[] chs = { ' ', '/', '\\' };
				int i = text.LastIndexOfAny(chs);
				if (i > 0)
					text = text.Substring(0, i-1);

				if (!int.TryParse(text, out pn))
					return;
				if (pn < 1)
					pn = 1;
				else if (pn > PdfViewer.Document.Pages.Count )
					pn = PdfViewer.Document.Pages.Count;

				PdfViewer.ScrollToPage(pn-1);
				PdfViewer.CurrentIndex = pn-1;
				item.TextBox.Text = string.Format("{0} / {1}", pn, PdfViewer.Document.Pages.Count);
			}
		}

		/// <summary>
		/// Occurs when the First Page button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnToBeginClick(ToolStripButton item)
		{
			PdfViewer.ScrollToPage(0);
			PdfViewer.CurrentIndex = 0;
		}


		/// <summary>
		/// Occurs when the Previous Page button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnToLeftClick(ToolStripButton item)
		{
			int ci = PdfViewer.CurrentIndex;
			if (ci > 0)
				ci--;
            PdfViewer.ScrollToPage(ci);
			PdfViewer.CurrentIndex = ci;
		}

		/// <summary>
		/// Occurs when the Next Page button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnToRightClick(ToolStripButton item)
		{
			int ci = PdfViewer.CurrentIndex;
			if (ci < PdfViewer.Document.Pages.Count-1)
				ci++;
			PdfViewer.ScrollToPage(ci);
			PdfViewer.CurrentIndex = ci;
		}

		/// <summary>
		/// Occurs when the Last Page button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnToEndClick(ToolStripButton item)
		{
			int ci = PdfViewer.Document.Pages.Count - 1;
			PdfViewer.ScrollToPage(ci);
			PdfViewer.CurrentIndex = ci;
		}

		#endregion

		#region Private methods
		private void UnsubscribePdfViewEvents(PdfViewer oldValue)
		{
			if (oldValue.Document != null)
			{
				oldValue.Document.Pages.PageInserted -= PdfViewer_SomethingChanged;
				oldValue.Document.Pages.PageDeleted -= PdfViewer_SomethingChanged;
			}
			oldValue.BeforeDocumentChanged -= Subscribe_BeforeDocumentChanged;
			oldValue.AfterDocumentChanged -= Subscribe_AfterDocumentChanged;
			oldValue.AfterDocumentChanged -= PdfViewer_SomethingChanged;
			oldValue.DocumentLoaded -= PdfViewer_SomethingChanged;
			oldValue.DocumentClosed -= PdfViewer_SomethingChanged;
			oldValue.CurrentPageChanged -= PdfViewer_SomethingChanged;
		}

		private void SubscribePdfViewEvents(PdfViewer newValue)
		{
			if (newValue.Document != null)
			{
				newValue.Document.Pages.PageInserted -= PdfViewer_SomethingChanged;
				newValue.Document.Pages.PageDeleted -= PdfViewer_SomethingChanged;
			}
			newValue.BeforeDocumentChanged += Subscribe_BeforeDocumentChanged;
			newValue.AfterDocumentChanged += Subscribe_AfterDocumentChanged;
			newValue.AfterDocumentChanged += PdfViewer_SomethingChanged;
			newValue.DocumentLoaded += PdfViewer_SomethingChanged;
			newValue.DocumentClosed += PdfViewer_SomethingChanged;
			newValue.CurrentPageChanged += PdfViewer_SomethingChanged;
		}

		private void Subscribe_AfterDocumentChanged(object sender, EventArgs e)
		{
			if (PdfViewer.Document != null)
			{
				PdfViewer.Document.Pages.PageInserted += PdfViewer_SomethingChanged;
				PdfViewer.Document.Pages.PageDeleted += PdfViewer_SomethingChanged;
			}
		}

		private void Subscribe_BeforeDocumentChanged(object sender, EventArguments.DocumentClosingEventArgs e)
		{
			if (PdfViewer.Document != null)
			{
				PdfViewer.Document.Pages.PageInserted -= PdfViewer_SomethingChanged;
				PdfViewer.Document.Pages.PageDeleted -= PdfViewer_SomethingChanged;
			}
		}

		#endregion
	}
}
