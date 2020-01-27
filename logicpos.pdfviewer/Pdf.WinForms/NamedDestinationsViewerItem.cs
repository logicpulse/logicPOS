using System.Windows.Forms;

namespace Patagames.Pdf.Net.Controls.WinForms
{
	internal class NamedDestinationsViewerItem : ListViewItem
	{
		public PdfDestination Destination { get; private set; }

		public NamedDestinationsViewerItem(PdfDestination destination)
			: base(destination.Name)
		{
			Destination = destination;
			SubItems.Add(new ListViewSubItem(this, destination.PageIndex.ToString()));
		}
	}
}
