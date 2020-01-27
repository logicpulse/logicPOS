using System.Windows.Forms;

namespace Patagames.Pdf.Net.Controls.WinForms.ToolBars
{
	internal class ToolStripTrackBar : ToolStripControlHost
	{
		public ToolStripTrackBar() : base(new TrackBar()) { }

		public TrackBar TrackBar
		{
			get
			{
				return (Control as TrackBar);
			}
		}

		// Subscribe and unsubscribe the control events you wish to expose.
		protected override void OnSubscribeControlEvents(Control c)
		{
			// Call the base so the base events are connected.
			base.OnSubscribeControlEvents(c);

			//var tb = c as TrackBar;
			////tb.Scroll
			////tb.ValueChanged
		}

		protected override void OnUnsubscribeControlEvents(Control c)
		{
			// Call the base method so the basic events are unsubscribed.
			base.OnUnsubscribeControlEvents(c);

		}
	}
}
