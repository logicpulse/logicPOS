using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Patagames.Pdf.Net.Controls.WinForms
{
	/// <summary>
	/// Represents the NamedDestinationsViewer control for displaying named destinations contained in PDF document
	/// </summary>
	public partial class NamedDestinationsViewer : ListView
	{
		#region Private fields
		private PdfViewer _pdfViewer = null;
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets PdfViewer control associated with this NamedDestinationsViewer control
		/// </summary>
		public PdfViewer PdfViewer
		{
			get
			{
				return _pdfViewer;
			}
			set
			{
				if (_pdfViewer != value)
					OnPdfViewerChanging(_pdfViewer, value);
			}
		}
		#endregion

		#region Constructors and initialization
		/// <summary>
		/// Initializes a new instance of the <see cref="NamedDestinationsViewer"/> class. 
		/// </summary>
		public NamedDestinationsViewer()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NamedDestinationsViewer"/> class. 
		/// </summary>
		/// <param name="container">Container</param>
		public NamedDestinationsViewer(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Raises the System.Windows.Forms.Control.MouseDoubleClick event.
		/// </summary>
		/// <param name="e">An System.Windows.Forms.MouseEventArgs that contains the event data.</param>
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			ProcessNamedDestinationsClick();
			base.OnMouseDoubleClick(e);
		}

		/// <summary>
		/// Raises the System.Windows.Forms.Control.KeyDown event.
		/// </summary>
		/// <param name="e">A System.Windows.Forms.KeyEventArgs that contains the event data.</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				ProcessNamedDestinationsClick();
			base.OnKeyDown(e);
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// Called when the current PdfViewer control associated with the ToolStrip is changing.
		/// </summary>
		/// <param name="oldValue">PdfViewer control of which was associated with the ToolStrip.</param>
		/// <param name="newValue">PdfViewer control of which will be associated with the ToolStrip.</param>
		protected virtual void OnPdfViewerChanging(PdfViewer oldValue, PdfViewer newValue)
		{
			if (oldValue != null)
			{
				oldValue.AfterDocumentChanged -= pdfViewer_DocumentChanged;
				oldValue.DocumentClosed -= pdfViewer_DocumentClosed;
				oldValue.DocumentLoaded -= pdfViewer_DocumentLoaded;
			}
			if (newValue != null)
			{
				newValue.AfterDocumentChanged += pdfViewer_DocumentChanged;
				newValue.DocumentClosed += pdfViewer_DocumentClosed;
				newValue.DocumentLoaded += pdfViewer_DocumentLoaded;
			}

			_pdfViewer = newValue;
			RebuildList();
		}
		#endregion

		#region Private event handlers
		private void pdfViewer_DocumentChanged(object sender, EventArgs e)
		{
			RebuildList();
		}

		private void pdfViewer_DocumentLoaded(object sender, EventArgs e)
		{
			RebuildList();
		}

		private void pdfViewer_DocumentClosed(object sender, EventArgs e)
		{
			RebuildList();
		}

		private void NamedDestinationsViewer_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			e.Item = new NamedDestinationsViewerItem(_pdfViewer.Document.NamedDestinations[e.ItemIndex]);
		}
		#endregion

		#region Private methods
		private void ProcessNamedDestinationsClick()
		{
			foreach (int index in SelectedIndices)
			{
				var item = Items[index] as NamedDestinationsViewerItem;
				if (item == null)
					continue;

				if (item.Destination != null)
					ProcessDestination(item.Destination);
			}
		}

		private void ProcessDestination(PdfDestination pdfDestination)
		{
			if (_pdfViewer == null)
				return;
			_pdfViewer.ScrollToPage(pdfDestination.PageIndex);
			_pdfViewer.Invalidate();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Constructs the list of named destinations
		/// </summary>
		public void RebuildList()
		{
			if (_pdfViewer == null || _pdfViewer.Document == null || _pdfViewer.Document.NamedDestinations == null)
			{
				VirtualListSize = 0;
				Items.Clear();
			}
			else
			{
				VirtualListSize = _pdfViewer.Document.NamedDestinations.Count;
				if (!VirtualMode)
				{
					BeginUpdate();
					Items.Clear();
					foreach (var b in _pdfViewer.Document.NamedDestinations)
					{
						var item = new NamedDestinationsViewerItem(b);
						Items.Add(item);
					}
					EndUpdate();
				}
			}
		}
		#endregion
	}
}
