using Patagames.Pdf.Net.EventArguments;
using Patagames.Pdf.Net.Exceptions;
using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Patagames.Pdf.Net.Controls.WinForms.ToolBars
{
	/// <summary>
	/// Provides a container for Windows toolbar objects with predefined functionality for opening and printing
	/// </summary>
	public class PdfToolStripMain : PdfToolStrip
	{
		#region private members
		delegate void ShowPrintDialogDelegate(PrintDialog dlg);
		#endregion

		#region Public events
		/// <summary>
		/// Occurs when the loaded document protected by password. Application should return the password through Value property
		/// </summary>
		public event EventHandler<EventArgs<string>> PasswordRequired = null;

		/// <summary>
		/// Occurs after an instance of PdfPrintDocument class is created and before printing is started.
		/// </summary>
		/// <remarks>
		/// You can use this event to get access to PdfPrintDialog which is used in printing routine.
		/// For example, the printing routine shows the standard dialog with printing progress. 
		/// If you want to suppress it you can write in event handler the following:
		/// <code>
		/// private void ToolbarMain1_PdfPrintDocumentCreated(object sender, EventArgs&lt;PdfPrintDocument&gt; e)
		/// {
		///		e.Value.PrintController = new StandardPrintController();
		/// }
		/// </code>
		/// </remarks>
		public event EventHandler<EventArgs<PdfPrintDocument>> PdfPrintDocumentCreated = null;
        #endregion

        #region Overriding    
        /// <summary>
        /// Create all buttons and add its into toolbar. Override this method to create custom buttons
        /// </summary>
        /// 
        protected override void InitializeButtons()
        {
            var btn = CreateButton("btnPrintDoc",
            Properties.PdfToolStrip.btnPrintText,
            Properties.PdfToolStrip.btnPrintToolTipText,
            Properties.PdfToolStrip.btnPrintImage,
            btn_PrintDocClick);
            this.Items.Add(btn);

            if (HighlightInfo.export)
            {
                btn = CreateButton("btnExportDoc",
            Properties.PdfToolStrip.btnExportDoc,
            Properties.PdfToolStrip.btnExportToolTipText,
            Properties.PdfToolStrip.btnExportImage,
            btn_ExportDocClick);
             this.Items.Add(btn); }
            

            btn = CreateButton("btnSaveDoc",
            Properties.PdfToolStrip.btnSaveDoc,
            Properties.PdfToolStrip.btnSaveToolTipText,
            Properties.PdfToolStrip.btnSaveImage,
            btn_SaveDocClick);
            this.Items.Add(btn);

            btn = CreateButton("btnCloseDoc",
            Properties.PdfToolStrip.btnCloseDoc,
            Properties.PdfToolStrip.btnCloseToolTipText,
            Properties.PdfToolStrip.btnCloseImage,
            btn_CloseDocClick);
            this.Items.Add(btn);
        }

        /// <summary>
        /// Called when the ToolStrip's items need to change its states
        /// </summary>
        protected override void UpdateButtons()
		{
			var tsi = this.Items["btnCloseDoc"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null);

			tsi = this.Items["btnPrintDoc"];
			if (tsi != null)
				tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

            tsi = this.Items["btnSaveDoc"];
            if (tsi != null)
                tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

            tsi = this.Items["btnExportDoc"];
            if (tsi != null)
                tsi.Enabled = (PdfViewer != null) && (PdfViewer.Document != null);

            if (PdfViewer == null || PdfViewer.Document == null)
				return;
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

		#endregion

		#region Event handlers for PdfViewer
		private void PdfViewer_SomethingChanged(object sender, EventArgs e)
		{
			UpdateButtons();
		}
		#endregion

		#region Event handlers for buttons
		private void btn_CloseDocClick(object sender, EventArgs e)
		{
			OnCloseClick(this.Items["btnCloseDoc"] as ToolStripButton);
		}
		private void btn_PrintDocClick(object sender, EventArgs e)
		{
			OnPrintClick(this.Items["btnPrintDoc"] as ToolStripButton);
		}
        private void btn_SaveDocClick(object sender, EventArgs e)
        {
            OnSaveClick(this.Items["btnSaveDoc"] as ToolStripButton);
        }
        private void btn_ExportDocClick(object sender, EventArgs e)
        {
            OnExportClick(this.Items["btnExportDoc"] as ToolStripButton);
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Occurs when the Open button is clicked
        /// </summary>
        /// <param name="item">The item that has been clicked</param>
        protected virtual void OnOpenClick(ToolStripButton item)
		{
			var dlg = new OpenFileDialog();
			dlg.Multiselect = false;
			dlg.Filter = Properties.PdfToolStrip.OpenDialogFilter;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				try
				{
					PdfViewer.LoadDocument(dlg.FileName);
				}
				catch (InvalidPasswordException)
				{
					string password = OnPasswordRequired();
					try
					{
						PdfViewer.LoadDocument(dlg.FileName, password);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, Properties.Error.ErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}

        public virtual void OnCloseClick(ToolStripButton item)
        {
            //Application.Exit();
        }        

        /// <summary>
        /// Occurs when the Loaded document protected by password. Application should return the password
        /// </summary>
        /// <returns>Password to the document must be returned.</returns>
        protected virtual string OnPasswordRequired()
		{
			var args = new EventArgs<string>(null);
			if (PasswordRequired != null)
				PasswordRequired(this, args);
			return args.Value;
		}

        /// <summary>
        /// Save documents
        /// </summary>
        /// <param name="item"></param>
        public virtual void OnSaveClick(ToolStripButton item)
        {
            // Displays a SaveFileDialog so the user can save the Doc  
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PDF File|*.pdf";
            saveFileDialog1.Title = "Gravar documento como Ficheiro";
            string separator = "/";
            string name = PdfViewer.DocPath;
            string[] names = name.Split(separator.ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries);
            saveFileDialog1.FileName = names[names.Length - 1];
            //Initialize a new thread for print dialog
            Thread thread2 = new Thread(() =>
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        
                        string newDirectory = saveFileDialog1.FileName;
                        System.IO.File.Copy(PdfViewer.DocPath, newDirectory, true);   

                    }
                    catch (Win32Exception)
                    {
                        //Printing was canceled
                    }
                }
            });
            thread2.SetApartmentState(ApartmentState.STA);
            thread2.Start();
            thread2.Join();
        }

        public virtual void OnExportClick(ToolStripButton item)
        {
            // Displays a SaveFileDialog so the user can save the Doc  
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Spreadsheet File|*.xlsx";
            saveFileDialog1.Title = "Save document as File";
            //Initialize a new thread for print dialog
            Thread thread2 = new Thread(() =>
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string xlsPath = Path.ChangeExtension(PdfViewer.DocPath, ".xlsx");
                        string newDirectory = saveFileDialog1.FileName;
                        System.IO.File.Copy(xlsPath, newDirectory, true);
                        System.Diagnostics.Process.Start(newDirectory);
                        File.Delete(xlsPath);
                        //saveFileDialog1.FileName = PdfViewer.DocPath;

                    }
                    catch (Win32Exception)
                    {
                        //Printing was canceled
                    }
                }
            });
            thread2.SetApartmentState(ApartmentState.STA);
            thread2.Start();
            thread2.Join();
        }

        /// <summary>
        /// Occurs when the Print button is clicked
        /// </summary>
        /// <param name="item">The item that has been clicked</param>
        public virtual void OnPrintClick(ToolStripButton item)
		{           

            if (PdfViewer.Document.FormFill != null)
				PdfViewer.Document.FormFill.ForceToKillFocus();

			//Show standard print dialog
			var printDoc = new PdfPrintDocument(PdfViewer.Document);
			var dlg = new PrintDialog();
            dlg.AllowCurrentPage = true;
			dlg.AllowSomePages = true;
			dlg.UseEXDialog = true;
			dlg.Document = printDoc;
            
            OnPdfPrinDocumentCreaded(new EventArgs<PdfPrintDocument>(printDoc));
			ShowPrintDialogDelegate showprintdialog = ShowPrintDialog;

            //Initialize a new thread for print dialog
            Thread thread3 = new Thread(() => {                
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        this.Focus();
                        this.BringToFront();
                        Application.Exit();
                        dlg.Document.Print();
                        this.DestroyHandle();
                    }
                    catch (Win32Exception)
                    {
                        //Printing was canceled
                    }
                }                
            });
            thread3.SetApartmentState(ApartmentState.STA);
            thread3.Start();       
            thread3.Join();

        }

        /// <summary>
        /// Occurs after an instance of PdfPrintDocument class is created and before printing is started.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnPdfPrinDocumentCreaded(EventArgs<PdfPrintDocument> e)
		{
			if (PdfPrintDocumentCreated != null)
				PdfPrintDocumentCreated(this, e);
		}
		#endregion

		#region Private methods
		private void UnsubscribePdfViewEvents(PdfViewer oldValue)
		{
			oldValue.AfterDocumentChanged -= PdfViewer_SomethingChanged;
			oldValue.DocumentLoaded -= PdfViewer_SomethingChanged;
			oldValue.DocumentClosed -= PdfViewer_SomethingChanged;
		}

		private void SubscribePdfViewEvents(PdfViewer newValue)
		{
			newValue.AfterDocumentChanged += PdfViewer_SomethingChanged;
			newValue.DocumentLoaded += PdfViewer_SomethingChanged;
			newValue.DocumentClosed += PdfViewer_SomethingChanged;
		}

		private static void ShowPrintDialog(PrintDialog dlg)
		{
            
        }
		#endregion

	}
}
