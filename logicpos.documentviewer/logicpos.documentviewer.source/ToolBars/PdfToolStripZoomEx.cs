using System;
using System.Drawing;
using System.Windows.Forms;

namespace Patagames.Pdf.Net.Controls.WinForms.ToolBars
{
	/// <summary>
	/// Provides a container for Windows toolbar objects with predefined functionality for zooming
	/// </summary>
	public class PdfToolStripZoomEx : PdfToolStrip
	{
		#region Private fields
		private int _trackBarWidth = 104;
		private int _trackBarHeight = 16;
		private int _currentZoomLevel = 0;
		private float[] _zoomLevel = { 8.33f, 12.5f, 25, 33.33f, 50, 66.67f, 75, 100, 125, 150, 200, 300, 400, 600, 800 };
		#endregion

		#region Public properties
		/// <summary>
		/// Gets or sets the array with zoom values for ComboBox or TrackBar
		/// </summary>
		public float[] ZoomLevel
		{
			get
			{
				return _zoomLevel;
			}
			set
			{
				if (value != null && value.Length > 0)
				{
					_zoomLevel = value;
					this.Items.Clear();
					InitializeButtons();
					UpdateButtons();
				}
			}
		}

		/// <summary>
		/// Calculate the current zoom
		/// </summary>
		public float Zoom
		{
			get
			{
				if (PdfViewer == null || PdfViewer.Document == null || PdfViewer.Document.Pages.Count == 0)
					return 0;
				var page = PdfViewer.Document.Pages.CurrentPage;
				switch (PdfViewer.SizeMode)
				{
					case SizeModes.Zoom:
						return PdfViewer.Zoom * 100;
					default:
						return PdfViewer.CalcActualRect(PdfViewer.CurrentIndex).Width * 100 / ((float)page.Width / 72.0f * 96);
				}
			}
		}
		#endregion

		#region Constructor, Destructor, Initialisation

		private ToolStripItem CreateZoomDropDown()
		{
			var btn = new ToolStripDropDownButton(Properties.PdfToolStrip.btnZoomComboText);
			btn.Name = "btnDropDownZoomEx";
			btn.ToolTipText = Properties.PdfToolStrip.btnZoomComboToolTipText;
			btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			btn.DropDownOpening += ZoomLevel_DropDownOpening;

			ToolStripMenuItem item = null;
			for (int i = ZoomLevel.Length - 1; i >= 0; i--)
			{
				item = new ToolStripMenuItem(string.Format("{0:0.00}%", ZoomLevel[i]));
				item.Name = "btnZoomLevel_" + ZoomLevel[i].ToString().Replace(".", "_").Replace(",", "_");
				item.Tag = i;
				item.Click += ZoomLevel_Click;
				btn.DropDownItems.Add(item);
			}
			btn.DropDownItems.Add(new ToolStripSeparator());

			item = new ToolStripMenuItem(
				Properties.PdfToolStrip.btnExportDoc.Replace("\r\n", " "),
				Properties.PdfToolStrip.btnActualSize16Image,
				btn_ActualSizeClick,
				"btnActualSizeEx");
			item.ToolTipText = Properties.PdfToolStrip.btnActualSizeToolTipText;
			btn.DropDownItems.Add(item);

			item = new ToolStripMenuItem(
				Properties.PdfToolStrip.btnFitPageText.Replace("\r\n", " "),
				Properties.PdfToolStrip.btnFitPage16Image,
				btn_FitPageClick,
				"btnFitPageEx");
			item.ToolTipText = Properties.PdfToolStrip.btnFitPageToolTipText;
			btn.DropDownItems.Add(item);

			item = new ToolStripMenuItem(
				Properties.PdfToolStrip.btnFitWidthText.Replace("\r\n", " "),
				Properties.PdfToolStrip.btnFitWidth16Image,
				btn_FitWidthClick,
				"btnFitWidthEx");
			item.ToolTipText = Properties.PdfToolStrip.btnFitWidthToolTipText;
			btn.DropDownItems.Add(item);

			item = new ToolStripMenuItem(
				Properties.PdfToolStrip.btnFitHeightText.Replace("\r\n", " "),
				Properties.PdfToolStrip.btnFitHeight16Image,
				btn_FitHeightClick,
				"btnFitHeightEx");
			item.ToolTipText = Properties.PdfToolStrip.btnFitHeightToolTipText;
			btn.DropDownItems.Add(item);

			return btn;
		}

		private ToolStripItem CreateTrackBar()
		{
			var btn = new ToolStripTrackBar();
			btn.Name = "btnTrackBar";
			btn.AutoSize = false;
			btn.Size = new System.Drawing.Size(_trackBarWidth, _trackBarHeight);
			var tb = btn.TrackBar;
			tb.AutoSize = false;
			tb.TickStyle = TickStyle.None;
			tb.Maximum = ZoomLevel.Length - 1;
			tb.Minimum = 0;
			tb.LargeChange = 1;
			tb.SmallChange = 1;
			tb.ValueChanged += TrackBar_ValueChanged;
			return btn;
		}

		#endregion

		#region Overriding
		/// <summary>
		/// Create all buttons and add its into toolbar. Override this method to create custom buttons
		/// </summary>
		protected override void InitializeButtons()
		{
			//var btn = CreateZoomDropDown();
			//this.Items.Add(btn);

			//btn = CreateButton("btnZoomExOut",
			//	Properties.PdfToolStrip.btnZoomOutText,
			//	Properties.PdfToolStrip.btnZoomOutToolTipText,
			//	Properties.PdfToolStrip.btnZoomExOutImage,
			//	btn_ZoomExOutClick,
			//	System.Windows.Forms.ToolStripItemDisplayStyle.Image);
			//btn.Padding = new Padding(0);
			//this.Items.Add(btn);

			//btn = CreateTrackBar();
			//this.Items.Add(btn);

			//btn = CreateButton("btnZoomExIn",
			//	Properties.PdfToolStrip.btnZoomInText,
			//	Properties.PdfToolStrip.btnZoomInToolTipText,
			//	Properties.PdfToolStrip.btnZoomExInImage,
			//	btn_ZoomExInClick,
			//	System.Windows.Forms.ToolStripItemDisplayStyle.Image);
			//btn.Padding = new Padding(0);
			//this.Items.Add(btn);
		}

		/// <summary>
		/// Called when the ToolStrip's items need to change its states
		/// </summary>
		protected override void UpdateButtons()
		{
			var tsi = this.Items["btnDropDownZoomEx"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnTrackBar"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnZoomExOut"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnZoomExIn"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			if (PdfViewer == null || PdfViewer.Document == null)
				return;


			var zoom = Zoom;
			tsi = this.Items["btnDropDownZoomEx"];
			if (tsi != null)
				tsi.Text = string.Format("{0:.00}%", zoom);

			CalcCurrentZoomLevel();

			var tstb = this.Items["btnTrackBar"] as ToolStripTrackBar;
			if (tstb == null)
				return;
			tstb.TrackBar.ValueChanged -= TrackBar_ValueChanged;
            tstb.TrackBar.Value = this.LayoutStyle== ToolStripLayoutStyle.VerticalStackWithOverflow ? _currentZoomLevel*-1 : _currentZoomLevel;
			tstb.TrackBar.ValueChanged += TrackBar_ValueChanged;
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

		/// <summary>
		/// Raises the LayoutCompleted event.
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
		protected override void OnLayoutCompleted(EventArgs e)
		{
			base.OnLayoutCompleted(e);

			if (this.LayoutStyle == ToolStripLayoutStyle.VerticalStackWithOverflow)
			{
				var tstb = (this.Items["btnTrackBar"] as ToolStripTrackBar);
				if (tstb != null)
				{
					tstb.TrackBar.Orientation = Orientation.Vertical;
                    tstb.Size = new Size(_trackBarHeight, _trackBarWidth);

					tstb.TrackBar.Minimum = (ZoomLevel.Length - 1)*-1;
					tstb.TrackBar.Maximum = 0;
				}
			}
			else
			{
				var tstb = (this.Items["btnTrackBar"] as ToolStripTrackBar);
				if (tstb != null)
				{
					tstb.TrackBar.Orientation = Orientation.Horizontal;
					tstb.Size = new Size(_trackBarWidth, _trackBarHeight);

					tstb.TrackBar.Maximum = (ZoomLevel.Length - 1);
					tstb.TrackBar.Minimum = 0;
				}
			}
			UpdateButtons();
		}

		#endregion

		#region Event handlers for PdfViewer
		private void PdfViewer_SomethingChanged(object sender, EventArgs e)
		{
			UpdateButtons();
		}
		#endregion

		#region Event handlers for buttons
		private void ZoomLevel_DropDownOpening(object sender, EventArgs e)
		{
			var btn = this.Items["btnDropDownZoomEx"] as ToolStripDropDownButton;
			if (btn == null)
				return;

			var tsmiFitHeight = btn.DropDownItems["btnFitHeightEx"] as ToolStripMenuItem;
			var tsmiFitWidth = btn.DropDownItems["btnFitWidthEx"] as ToolStripMenuItem;
			var tsmiFitSize = btn.DropDownItems["btnFitPageEx"] as ToolStripMenuItem;
			var tsmiFitActual = btn.DropDownItems["btnActualSizeEx"] as ToolStripMenuItem;

			if(tsmiFitHeight!= null)
				tsmiFitHeight.Checked = (PdfViewer.SizeMode == SizeModes.FitToHeight);

			if (tsmiFitWidth!= null)
				tsmiFitWidth.Checked = (PdfViewer.SizeMode == SizeModes.FitToWidth);

			if (tsmiFitSize != null)
				tsmiFitSize.Checked = (PdfViewer.SizeMode == SizeModes.FitToSize);

			if (tsmiFitActual != null)
				tsmiFitActual.Checked = ((PdfViewer.SizeMode == SizeModes.Zoom) && (PdfViewer.Zoom >= 1 -0.00004 && PdfViewer.Zoom <= 1 + 0.00004));
		}

		private void TrackBar_ValueChanged(object sender, EventArgs e)
		{
			OnTrackBarValueChanged(this.Items["btnTrackBar"] as ToolStripTrackBar);
		}

		private void ZoomLevel_Click(object sender, EventArgs e)
		{
			OnZoomLevelClick(sender as ToolStripMenuItem, (float)ZoomLevel[(int)(sender as ToolStripMenuItem).Tag]);
		}

		private void btn_ZoomExOutClick(object sender, System.EventArgs e)
		{
			OnZoomExOutClick(sender as ToolStripButton);
		}
		private void btn_ZoomExInClick(object sender, System.EventArgs e)
		{
			OnZoomExInClick(sender as ToolStripButton);
		}
		private void btn_ActualSizeClick(object sender, System.EventArgs e)
		{
			OnActualSizeClick(sender as ToolStripMenuItem);
		}
		private void btn_FitPageClick(object sender, System.EventArgs e)
		{
			OnFitPageClick(sender as ToolStripMenuItem);
		}
		private void btn_FitWidthClick(object sender, System.EventArgs e)
		{
			OnFitWidthClick(sender as ToolStripMenuItem);
		}
		private void btn_FitHeightClick(object sender, System.EventArgs e)
		{
			OnFitHeightClick(sender as ToolStripMenuItem);
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// Sets specified zoom level for Pdf document
		/// </summary>
		/// <param name="zoomIndex">Index of the zoom in ZoomLevel</param>
		protected void SetZoom(int zoomIndex)
		{
			SetZoom(ZoomLevel[zoomIndex] / 100);
		}

		/// <summary>
		/// Sets specified zoom for Pdf document
		/// </summary>
		/// <param name="zoom">zoom value</param>
		protected virtual void SetZoom(float zoom)
		{
			UnsubscribePdfViewEvents(PdfViewer);
			PdfViewer.SizeMode = SizeModes.Zoom;
			PdfViewer.Zoom = zoom;
			SubscribePdfViewEvents(PdfViewer);
			CalcCurrentZoomLevel();
		}

		/// <summary>
		/// Calculate zoom level for current <see cref="Zoom"/> and store it in internal field
		/// </summary>
		protected void CalcCurrentZoomLevel()
		{
			var zoom = Zoom;
			float min = float.MaxValue;
			_currentZoomLevel = 0;
			for (int i = 0; i < ZoomLevel.Length; i++)
			{
				float m = ZoomLevel[i] - zoom;
				if (m < 0) m = -m;
				if (min > m)
				{
					min = m;
					_currentZoomLevel = i;
				}
			}
		}

		/// <summary>
		/// Occurs when the Value property of a track bar changes, either by movement of the scroll box or by manipulation in code.
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnTrackBarValueChanged(ToolStripControlHost item)
		{
			var val = (item as ToolStripTrackBar).TrackBar.Value;
			SetZoom(this.LayoutStyle == ToolStripLayoutStyle.VerticalStackWithOverflow ? val * -1 : val);
			UpdateButtons();
		}

		/// <summary>
		/// Occurs when the any item with zoom level clicked in ZoomDropDown button
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		/// <param name="zoom">The zoom value of item that was clicked</param>
		protected virtual void OnZoomLevelClick(ToolStripMenuItem item, float zoom)
		{
			SetZoom(zoom/100);
			UpdateButtons();
		}

		/// <summary>
		/// Occurs when the Zoom In button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnZoomExInClick(ToolStripButton item)
		{
			if (_currentZoomLevel < ZoomLevel.Length-1)
			{
				_currentZoomLevel++;
				SetZoom(_currentZoomLevel);
				UpdateButtons();
			}
		}

		/// <summary>
		/// Occurs when the Zoom Out button is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnZoomExOutClick(ToolStripButton item)
		{
			if (_currentZoomLevel > 0)
			{
				_currentZoomLevel--;
				SetZoom(_currentZoomLevel);
				UpdateButtons();
			}

		}

		/// <summary>
		/// Occurs when the Actual Size item is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnActualSizeClick(ToolStripMenuItem item)
		{
			SetZoom(1.0f);
			UpdateButtons();
		}

		/// <summary>
		/// Occurs when the Fit To Page item is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnFitPageClick(ToolStripMenuItem item)
		{
			PdfViewer.SizeMode = SizeModes.FitToSize;
		}

		/// <summary>
		/// Occurs when the Fit To Width item is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnFitWidthClick(ToolStripMenuItem item)
		{
			PdfViewer.SizeMode = SizeModes.FitToWidth;
		}

		/// <summary>
		/// Occurs when the Fit To Height item is clicked
		/// </summary>
		/// <param name="item">The item that has been clicked</param>
		protected virtual void OnFitHeightClick(ToolStripMenuItem item)
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
			oldValue.ViewModeChanged -= PdfViewer_SomethingChanged;
			oldValue.SizeModeChanged -= PdfViewer_SomethingChanged;
			oldValue.ZoomChanged -= PdfViewer_SomethingChanged;
			oldValue.CurrentPageChanged -= PdfViewer_SomethingChanged;
		}

		private void SubscribePdfViewEvents(PdfViewer newValue)
		{
			newValue.AfterDocumentChanged += PdfViewer_SomethingChanged;
			newValue.DocumentLoaded += PdfViewer_SomethingChanged;
			newValue.DocumentClosed += PdfViewer_SomethingChanged;
			newValue.ViewModeChanged += PdfViewer_SomethingChanged;
			newValue.SizeModeChanged += PdfViewer_SomethingChanged;
			newValue.ZoomChanged += PdfViewer_SomethingChanged;
			newValue.CurrentPageChanged += PdfViewer_SomethingChanged;
		}

		#endregion

	}
}
