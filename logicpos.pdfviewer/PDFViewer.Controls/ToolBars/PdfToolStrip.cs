using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;



namespace Patagames.Pdf.Net.Controls.WinForms.ToolBars
{
    /// <summary>
    /// Represents the base functionality for all PdfToolStrips
    /// </summary>

    

    public class PdfToolStrip : ToolStrip
	{
		#region Private fields
		private PdfViewer _pdfViewer = null;
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets PdfViewer control associated with this PdfToolStrip control
        /// </summary>
        /// 
  

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

		#region Constructors, destructors, initialisation
		/// <summary>
		/// Initialize the new instance of PdfToolStrip class
		/// </summary>
		public PdfToolStrip()
		{
			InitializeButtons();
			UpdateButtons();
        }
		#endregion

		#region Protected methods
		/// <summary>
		/// Create all buttons and add its into toolbar. Override this method to create custom buttons
		/// </summary>
		protected virtual void InitializeButtons()
		{

		}

		/// <summary>
		/// Called when the ToolStrip's items need to change its states
		/// </summary>
		protected virtual void UpdateButtons()
		{

		}

		/// <summary>
		/// Called when the current PdfViewer control associated with the ToolStrip is changing.
		/// </summary>
		/// <param name="oldValue">PdfViewer control of which was associated with the ToolStrip.</param>
		/// <param name="newValue">PdfViewer control of which will be associated with the ToolStrip.</param>
		protected virtual void OnPdfViewerChanging(PdfViewer oldValue, PdfViewer newValue)
		{
			_pdfViewer = newValue;
			UpdateButtons();
		}

		/// <summary>
		/// Create a new instance of ToolStripButton class with the specified name that displays the specified text and image and that raises the Click event.
		/// </summary>
		/// <param name="name">The name of the ToolStripButton.</param>
		/// <param name="text">The text to display on the ToolStripButton.</param>
		/// <param name="toolTipText">Specify the text that appears as a ToolTip for a control.</param>
		/// <param name="image">The image to display on the ToolStripButton.</param>
		/// <param name="onClick">An event handler that raises the Click event.</param>
		/// <param name="displayStyle">Specify whether text and images are displayed on a ToolStripItem.</param>
		/// <returns>Tool strip item</returns>
		protected virtual ToolStripItem CreateButton(string name, string text, string toolTipText, Image image, EventHandler onClick, ToolStripItemDisplayStyle displayStyle = ToolStripItemDisplayStyle.ImageAndText)
		{
			ToolStripButton btn = new ToolStripButton(text, image, onClick, name);
			btn.Padding = new Padding(7, 2, 7, 2);
			btn.ToolTipText = toolTipText;
			btn.ImageScaling = ToolStripItemImageScaling.None;
			btn.TextImageRelation = TextImageRelation.ImageAboveText;
			btn.DisplayStyle = displayStyle;
			return btn;
		}
		#endregion
	}
}
