using Patagames.Pdf.Enums;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Patagames.Pdf.Net.Controls.WinForms.ToolBars
{
	internal class ToolStripSearchBar : ToolStripControlHost
	{
		public ToolStripSearchBar() : base(new SearchBar()) { }

		internal SearchBar SearchBar
		{
			get
			{
				return (Control as SearchBar);
			}
		}

		// Subscribe and unsubscribe the control events you wish to expose.
		protected override void OnSubscribeControlEvents(Control c)
		{
			// Call the base so the base events are connected.
			base.OnSubscribeControlEvents(c);
		}

		protected override void OnUnsubscribeControlEvents(Control c)
		{
			// Call the base method so the basic events are unsubscribed.
			base.OnUnsubscribeControlEvents(c);
		}
	}
}
