using Patagames.Pdf.Enums;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace Patagames.Pdf.Net.Controls.WinForms
{
	/// <summary>
	/// Represents the BookmarksViewer control for displaying bookmarks contained in PDF document.
	/// </summary>
	public partial class BookmarksViewer : TreeView
	{

		#region Private fields
		private PdfViewer _pdfViewer = null;
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets PdfViewer control associated with this BookmarkViewer control
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
		/// Initializes a new instance of the <see cref="BookmarksViewer"/> class. 
		/// </summary>
		public BookmarksViewer()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BookmarksViewer"/> class. 
		/// </summary>
		/// <param name="container">Container</param>
		public BookmarksViewer(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Raises the System.Windows.Forms.TreeView.AfterSelect event.
		/// </summary>
		/// <param name="e">A System.Windows.Forms.TreeViewEventArgs that contains the event data.</param>
		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			var node = e.Node as BookmarksViewerNode;
			if (node == null || node.Bookmark == null)
				return;

			if (node.Bookmark.Action != null)
				ProcessAction(node.Bookmark.Action);
			else if (node.Bookmark.Destination != null)
				ProcessDestination(node.Bookmark.Destination);

			base.OnAfterSelect(e);
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
			RebuildTree();
		}
		#endregion

		#region Private event handlers
		private void pdfViewer_DocumentChanged(object sender, EventArgs e)
		{
			RebuildTree();
		}

		private void pdfViewer_DocumentLoaded(object sender, EventArgs e)
		{
			RebuildTree();
		}

		private void pdfViewer_DocumentClosed(object sender, EventArgs e)
		{
			RebuildTree();
		}
		#endregion

		#region Private methods
		private void BuildTree(TreeNodeCollection nodes, PdfBookmarkCollections bookmarks)
		{
			if (bookmarks == null)
				return;

			foreach (var b in bookmarks)
			{
				var node = new BookmarksViewerNode(b);
				nodes.Add(node);
				if (b.Childs != null && b.Childs.Count > 0)
					BuildTree(node.Nodes, b.Childs);
			}
		}

		private void ProcessAction(PdfAction pdfAction)
		{
			if (pdfAction.ActionType == ActionTypes.Uri)
				Process.Start(pdfAction.ActionUrl);
			else if (pdfAction.Destination != null)
				ProcessDestination(pdfAction.Destination);
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
		/// Constructs the tree of bookmarks
		/// </summary>
		public void RebuildTree()
		{
			Nodes.Clear();
			if (_pdfViewer != null && _pdfViewer.Document != null)
				BuildTree(Nodes, _pdfViewer.Document.Bookmarks);
		}
		#endregion
	}
}
