using System.Windows.Forms;

namespace Patagames.Pdf.Net.Controls.WinForms
{
    /// <summary>
    /// Represents a node of a BookmarksViewer.
    /// </summary>
	public class BookmarksViewerNode : TreeNode
	{
        /// <summary>
        /// Gets bookmark under this node
        /// </summary>
		public PdfBookmark Bookmark { get; private set; }

        /// <summary>
        /// Create a new instance of a BookmarksViewerNode class
        /// </summary>
        /// <param name="bookmark">The bookmark</param>
		public BookmarksViewerNode(PdfBookmark bookmark)
			: base(bookmark.Title)
		{
			Bookmark = bookmark;
		}
	}

}
