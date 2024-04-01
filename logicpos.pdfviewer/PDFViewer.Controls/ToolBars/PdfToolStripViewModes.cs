using System;
using System.Windows.Forms;

namespace Patagames.Pdf.Net.Controls.WinForms.ToolBars
{
	/// <summary>
	/// Provides a container for Windows toolbar objects with predefined functionality for changing view modes
	/// </summary>
	public class PdfToolStripViewModes : PdfToolStrip
	{
        int _tilesCount = -1;
		#region Overriding
		/// <summary>
		/// Create all buttons and add its into toolbar. Override this method to create custom buttons
		/// </summary>
		protected override void InitializeButtons()
		{
			//var btn = CreateButton("btnModeSingle",
			//	Properties.PdfToolStrip.btnModeSingleText,
			//	Properties.PdfToolStrip.btnModeSingleToolTipText,
			//	Properties.PdfToolStrip.btnModeSingleImage,
			//	btn_ModeSingleClick,
			//	System.Windows.Forms.ToolStripItemDisplayStyle.Image);
			//this.Items.Add(btn);

			//btn = CreateButton("btnModeVertical",
			//	Properties.PdfToolStrip.btnModeVerticalText,
			//	Properties.PdfToolStrip.btnModeVerticalToolTipText,
			//	Properties.PdfToolStrip.btnModeVerticalImage,
			//	btn_ModeVerticalClick,
			//	System.Windows.Forms.ToolStripItemDisplayStyle.Image);
			//this.Items.Add(btn);

			//btn = CreateButton("btnModeHorizontal",
			//	Properties.PdfToolStrip.btnModeHorizontalText,
			//	Properties.PdfToolStrip.btnModeHorizontalToolTipText,
			//	Properties.PdfToolStrip.btnModeHorizontalImage,
			//	btn_ModeHorizontalClick,
			//	System.Windows.Forms.ToolStripItemDisplayStyle.Image);
			//this.Items.Add(btn);

			//btn = CreateButton("btnModeTiles",
			//	Properties.PdfToolStrip.btnModeTilesText,
			//	Properties.PdfToolStrip.btnModeTilesToolTipText,
			//	Properties.PdfToolStrip.btnModeTilesImage,
			//	btn_ModeTilesClick,
			//	System.Windows.Forms.ToolStripItemDisplayStyle.Image);
			//this.Items.Add(btn);

   //         btn = CreateButton("btnModeTwoPage",
   //             Properties.PdfToolStrip.btnModeTwoPageText,
   //             Properties.PdfToolStrip.btnModeTwoPageToolTipText,
   //             Properties.PdfToolStrip.btnModeTwoPage,
   //             btn_ModeTwoPageClick,
   //             System.Windows.Forms.ToolStripItemDisplayStyle.Image);
   //         this.Items.Add(btn);
        }

        /// <summary>
        /// Called when the ToolStrip's items need to change its states
        /// </summary>
        protected override void UpdateButtons()
		{
			var tsi = this.Items["btnModeSingle"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnModeVertical"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnModeHorizontal"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnModeTiles"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

            tsi = this.Items["btnModeTwoPage"];
            if (tsi != null)
                tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);


            if (PdfViewer == null || PdfViewer.Document == null)
				return;

			var tsb = this.Items["btnModeSingle"] as ToolStripButton;
			if (tsb != null)
				tsb.Checked = (PdfViewer.ViewMode == ViewModes.SinglePage);

			tsb = this.Items["btnModeVertical"] as ToolStripButton;
			if (tsb != null)
				tsb.Checked = (PdfViewer.ViewMode == ViewModes.Vertical);

			tsb = this.Items["btnModeHorizontal"] as ToolStripButton;
			if (tsb != null)
				tsb.Checked = (PdfViewer.ViewMode == ViewModes.Horizontal);

			tsb = this.Items["btnModeTiles"] as ToolStripButton;
			if (tsb != null)
				tsb.Checked = (PdfViewer.ViewMode == ViewModes.TilesVertical);

            tsb = this.Items["btnModeTwoPage"] as ToolStripButton;
            if (tsb != null)
                tsb.Checked = (PdfViewer.ViewMode == ViewModes.TilesLine);

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
		private void btn_ModeSingleClick(object sender, System.EventArgs e)
		{
			OnModeSingleClick(this.Items["btnModeSingle"] as ToolStripButton);
		}
		private void btn_ModeVerticalClick(object sender, System.EventArgs e)
		{
			OnModeVerticalClick(this.Items["btnModeVertical"] as ToolStripButton);
		}
		private void btn_ModeHorizontalClick(object sender, System.EventArgs e)
		{
			OnModeHorizontalClick(this.Items["btnModeHorizontal"] as ToolStripButton);
		}
		private void btn_ModeTilesClick(object sender, System.EventArgs e)
		{
			OnModeTilesClick(this.Items["btnModeTiles"] as ToolStripButton);
		}
        private void btn_ModeTwoPageClick(object sender, System.EventArgs e)
        {
            OnModeTwoPageClick(this.Items["btnModeTwoPage"] as ToolStripButton);
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Occurs when the Single Page button is clicked
        /// </summary>
        /// <param name="item">The item that has been clicked</param>
        protected virtual void OnModeSingleClick(ToolStripButton item)
		{
			PdfViewer.ViewMode = ViewModes.SinglePage;
		}

		/// <summary>
		/// Occurs when the Continuous Vertical button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnModeVerticalClick(ToolStripButton item)
		{
			PdfViewer.ViewMode = ViewModes.Vertical;
		}

		/// <summary>
		/// Occurs when the Continuous Horizontal button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnModeHorizontalClick(ToolStripButton item)
		{
			PdfViewer.ViewMode = ViewModes.Horizontal;
		}

		/// <summary>
		/// Occurs when the Continuous Facing button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnModeTilesClick(ToolStripButton item)
		{
            if(_tilesCount!= -1)
                PdfViewer.TilesCount = _tilesCount;
            PdfViewer.ViewMode = ViewModes.TilesVertical;
		}

        /// <summary>
        /// Occurs when the Facing button is clicked
        /// </summary>
        /// <param name="item">The item that has been clicked</param>
        protected virtual void OnModeTwoPageClick(ToolStripButton item)
        {
            _tilesCount = PdfViewer.TilesCount;
            PdfViewer.TilesCount = 2;
            PdfViewer.ViewMode = ViewModes.TilesLine;
        }
        #endregion

        #region Private methods
        private void UnsubscribePdfViewEvents(PdfViewer oldValue)
		{
			oldValue.AfterDocumentChanged -= PdfViewer_SomethingChanged;
			oldValue.DocumentLoaded -= PdfViewer_SomethingChanged;
			oldValue.DocumentClosed -= PdfViewer_SomethingChanged;
			oldValue.ViewModeChanged -= PdfViewer_SomethingChanged;
		}

		private void SubscribePdfViewEvents(PdfViewer newValue)
		{
			newValue.AfterDocumentChanged += PdfViewer_SomethingChanged;
			newValue.DocumentLoaded += PdfViewer_SomethingChanged;
			newValue.DocumentClosed += PdfViewer_SomethingChanged;
			newValue.ViewModeChanged += PdfViewer_SomethingChanged;
		}

		#endregion


	}
}
