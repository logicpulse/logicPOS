using System;
using System.Windows.Forms;

namespace Patagames.Pdf.Net.Controls.WinForms.ToolBars
{
	/// <summary>
	/// Provides a container for Windows toolbar objects with predefined functionality for zooming
	/// </summary>
	public class PdfToolStripZoom : PdfToolStripZoomEx
	{
		#region Constructor, Destructor, Initialisation
		private ToolStripItem CreateZoomCombo()
		{
			var btn = new ToolStripComboBox(Properties.PdfToolStrip.btnZoomComboText);
			btn.Name = "btnComboBox";
			btn.ToolTipText = Properties.PdfToolStrip.btnZoomComboToolTipText;
			btn.KeyDown += ComboBox_KeyDown;
			btn.ComboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
			btn.ComboBox.Width = 70;
			for (int i = 0; i<  ZoomLevel.Length; i++)
				btn.Items.Add(string.Format("{0}%", ZoomLevel[i]));
			return btn;
		}

		#endregion

		#region Overriding
		/// <summary>
		/// Create all buttons and add its into toolbar. Override this method to create custom buttons
		/// </summary>
		protected override void InitializeButtons()
		{
			var btn = CreateButton("btnZoomOut",
				Properties.PdfToolStrip.btnZoomOutText,
				Properties.PdfToolStrip.btnZoomOutToolTipText,
				Properties.PdfToolStrip.btnZoomOutImage,
				btn_ZoomOutClick);
			this.Items.Add(btn);
			
			btn = CreateButton("btnZoomIn",
				Properties.PdfToolStrip.btnZoomInText,
				Properties.PdfToolStrip.btnZoomInToolTipText,
				Properties.PdfToolStrip.btnZoomInImage,
				btn_ZoomInClick);
			this.Items.Add(btn);

            //btn = CreateZoomCombo();
            //this.Items.Add(btn);

        }

		/// <summary>
		/// Called when the ToolStrip's items need to change its states
		/// </summary>
		protected override void UpdateButtons()
		{
			var tsi = this.Items["btnComboBox"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnZoomOut"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			tsi = this.Items["btnZoomIn"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

			if (PdfViewer == null || PdfViewer.Document == null)
				return;

			CalcCurrentZoomLevel();

			var tscb = this.Items["btnComboBox"] as ToolStripComboBox;
			if (tscb == null)
				return;
			tscb.ComboBox.Text = string.Format("{0:.00}%", Zoom);

		}
		#endregion

		#region Event handlers for buttons
		private void btn_ZoomOutClick(object sender, System.EventArgs e)
		{
			OnZoomExOutClick(this.Items["btnZoomOut"] as ToolStripButton);
        }
		private void btn_ZoomInClick(object sender, System.EventArgs e)
		{
			OnZoomExInClick(this.Items["btnZoomIn"] as ToolStripButton);
		}

		private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnComboBoxSelectedIndexChanged(this.Items["btnComboBox"] as ToolStripComboBox, (sender as ComboBox).SelectedIndex);
		}

		private void ComboBox_KeyDown(object sender, KeyEventArgs e)
		{
			OnComboBoxKeyDown(this.Items["btnComboBox"] as ToolStripComboBox, e);
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// Occurs the the selected index changed
		/// </summary>
		/// <param name="item">ComboBox</param>
		/// <param name="selectedIndex">Selected index</param>
		protected virtual void OnComboBoxSelectedIndexChanged(ToolStripComboBox item, int selectedIndex)
		{
			SetZoom(selectedIndex);
		}

		/// <summary>
		/// Occurs when a key is pressed and held down while the ComboBox has focus.
		/// </summary>
		/// <param name="item">ComboBox</param>
		/// <param name="e">Key event args</param>
		protected virtual void OnComboBoxKeyDown(ToolStripComboBox item, KeyEventArgs e)
		{
			if (item == null)
				return;
			if(e.KeyCode == Keys.Enter)
			{
				float zoom = 0;
				string text = item.ComboBox.Text.Replace("%", "").Replace(" ", "");
				var t = text;
				if(!float.TryParse(t, out zoom))
				{
					t = text.Replace(".", ",");
					if(!float.TryParse(t, out zoom))
					{
						t = text.Replace(",", ".");
						if (!float.TryParse(t, out zoom))
						{
							return;
						}
					}
				}
				if (zoom < ZoomLevel[0])
					zoom = ZoomLevel[0];
				else if (zoom > ZoomLevel[ZoomLevel.Length-1])
					zoom = ZoomLevel[ZoomLevel.Length-1];
				SetZoom(zoom/100.0f);
				item.ComboBox.Text = string.Format("{0:.00}%", zoom);
			}
		}
		#endregion

	}
}
