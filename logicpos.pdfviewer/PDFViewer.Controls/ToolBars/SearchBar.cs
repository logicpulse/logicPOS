using System;
using System.Drawing;
using System.Windows.Forms;
using Patagames.Pdf.Enums;

namespace Patagames.Pdf.Net.Controls.WinForms.ToolBars
{
	internal partial class SearchBar : UserControl
	{
		#region Private fields
		private int _totalRecords = 0;
		private int _currentRecord = 0;
		private Timer _onsearchTimer;
		#endregion

		#region Public events and properties
		public event EventHandler CurrentRecordChanged = null;
		public event EventHandler NeedSearch = null;

		public Color BorderColor {
			get
			{
				return pnlBorder.BackColor;
			}
			set
			{
				pnlBorder.BackColor = value;
			}
		}
		public FindFlags FindFlags { get; set; }

		public int TotalRecords{
			get
			{
				return _totalRecords;
			}
			set
			{
				_totalRecords = value;
				if (_totalRecords < 0)
					_totalRecords = 0;
				if (_currentRecord > _totalRecords)
					_currentRecord = _totalRecords;
				if (_totalRecords == 0)
					lblInfo.BackColor = Color.PaleVioletRed;
				else
					lblInfo.BackColor = Color.Transparent;
				SetInfoText();
				EnableButton(picUp, _totalRecords > 0);
				EnableButton(picDown, _totalRecords > 0);

				if (_totalRecords > 0 && _currentRecord == 0)
					CurrentRecord = 1;
			}
		}
		public int CurrentRecord
		{
			get
			{
				return _currentRecord;
			}
			set
            {
				if (_currentRecord != value)
				{
					_currentRecord = value;
					SetInfoText();
					if (CurrentRecordChanged != null)
						CurrentRecordChanged(this, EventArgs.Empty);
				}
			}
		}

		private void SetInfoText()
		{
			lblInfo.Text = string.Format(Properties.PdfToolStrip.searchLblnfo, CurrentRecord, TotalRecords);
			using (var g = Graphics.FromHwnd(lblInfo.Handle))
			{
				var sz = g.MeasureString(lblInfo.Text, lblInfo.Font);
				lblInfo.Width = (int)sz.Width + 10;
			}
		}

		public string SearchText
		{
			get
			{
				return tbSearch.Text;
			}
			set
			{
				if(SearchText!= value)
					tbSearch.Text = value;
			}
		}
		#endregion
		
		#region Construxtors
		public SearchBar()
		{
			InitializeComponent();
			pnlBorder.BackColor = BorderColor;
			EnableButton(picUp, false);
			EnableButton(picDown, false);
			lblInfo.Visible = false;

			_onsearchTimer = new Timer();
			_onsearchTimer.Interval = 50;
			_onsearchTimer.Tick += _onsearchTimer_Tick;
        }
		#endregion
		
		#region Buttons and context menu reaction
		private void Button_MouseEnter(object sender, EventArgs e)
		{
			SetButtonHover(sender as Control);
		}

		private void Button_MouseLeave(object sender, EventArgs e)
		{
			SetButtonUnpressed(sender as Control);
		}

		private void Button_MouseMove(object sender, MouseEventArgs e)
		{
			if (((sender as Control).Tag as string) == "pressed")
			{
				if ((sender as Control).ClientRectangle.Contains(e.Location))
					SetButtonPressed(sender as Control);
				else
					SetButtonUnpressed(sender as Control);
			}
		}

		private void Button_MouseDown(object sender, MouseEventArgs e)
		{
			(sender as Control).Tag = "pressed";
			SetButtonPressed(sender as Control);
        }

		private void Button_MouseUp(object sender, MouseEventArgs e)
		{
			(sender as Control).Tag = null;
			SetButtonHover(sender as Control);
			if ((sender as Control).ClientRectangle.Contains(e.Location))
			{
				switch((sender as Control).Name)
				{
					case "picMenu":
						ProcessMenuClick();
						break;
					case "picDown":
						ProcessDownClick();
						break;
					case "picUp":
						ProcessUpClick();
						break;
				}
            }

		}

		private void ProcessDownClick()
		{
			if (CurrentRecord < TotalRecords)
				CurrentRecord++;
			else
				CurrentRecord = 1;
		}

		private void ProcessUpClick()
		{
			if (CurrentRecord > 1)
				CurrentRecord--;
			else
				CurrentRecord = TotalRecords;

		}

		private void ProcessMenuClick()
		{
			var cm = new ContextMenu();
			var item = new MenuItem(Properties.PdfToolStrip.menuItemMatchCase);
			item.Tag = FindFlags.MatchCase;
			item.Checked = ((FindFlags & FindFlags.MatchCase) == FindFlags.MatchCase);
			item.Click += searchMenuItem_Click;
			cm.MenuItems.Add(item);

			item = new MenuItem(Properties.PdfToolStrip.menuItemMatchWholeWord);
			item.Tag = FindFlags.MatchWholeWord;
			item.Checked = ((FindFlags & FindFlags.MatchWholeWord) == FindFlags.MatchWholeWord);
			item.Click += searchMenuItem_Click;
			cm.MenuItems.Add(item);

			cm.Show(picMenu, new Point(-1, picMenu.Height));
		}

		private void searchMenuItem_Click(object sender, EventArgs e)
		{
			var flag = (FindFlags)(sender as MenuItem).Tag;
			FindFlags ^= flag;
			OnSearch();
		}

		private void SetButtonHover(Control button)
		{
			button.BackColor = Color.WhiteSmoke;
			button.Padding = new Padding(0, 0, 0, 0);
		}

		private void SetButtonPressed(Control button)
		{
			button.BackColor = Color.Gray;
			button.Padding = new Padding(1, 1, 0, 0);
		}

		private void SetButtonUnpressed(Control button)
		{

			button.BackColor = SystemColors.Window;
			button.Padding = new Padding(0, 0, 0, 0);
		}

		#endregion

		#region Text changed and timer
		private void tbSearch_TextChanged(object sender, EventArgs e)
		{
			lblInfo.Visible = (tbSearch.Text != "");
			EnableButton(picUp, (tbSearch.Text != ""));
			EnableButton(picDown, (tbSearch.Text != ""));
			_onsearchTimer.Stop();
			//if (tbSearch.Text != "")
				_onsearchTimer.Start();
			//else
			//{
			//	_totalRecords = 0;
			//	_currentRecord = 0;
			//}
		}

		private void _onsearchTimer_Tick(object sender, EventArgs e)
		{
			_onsearchTimer.Stop();
			OnSearch();
		}
		#endregion

		#region Event handlers
		private void pnlHostTextBox_Click(object sender, EventArgs e)
		{
			tbSearch.Focus();
		}
		#endregion

		#region Private methods
		private void EnableButton(PictureBox button, bool enabled)
		{
			switch (button.Name)
			{
				case "picUp":
					picUp.Enabled = enabled;
					picUp.Image = enabled ? Properties.PdfToolStrip.searchPrevImage : Properties.PdfToolStrip.searchPrevDisabledImage;
					break;
				case "picDown":
					picDown.Enabled = enabled;
					picDown.Image = enabled ? Properties.PdfToolStrip.searchNextImage : Properties.PdfToolStrip.searchNextDisabledImage;
					break;
			}
		}

		private void OnSearch()
		{
			if (NeedSearch != null)
				NeedSearch(this, EventArgs.Empty);
        }
		#endregion
	}
}
