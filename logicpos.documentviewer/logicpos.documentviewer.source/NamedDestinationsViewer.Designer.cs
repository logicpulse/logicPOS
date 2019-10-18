namespace Patagames.Pdf.Net.Controls.WinForms
{
	partial class NamedDestinationsViewer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Title = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// Title
			// 
			this.Title.Text = "Title";
			// 
			// NamedDestinationsViewer
			// 
			this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Title});
			this.MultiSelect = false;
			this.View = System.Windows.Forms.View.Details;
			this.VirtualMode = true;
			this.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.NamedDestinationsViewer_RetrieveVirtualItem);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ColumnHeader Title;
	}
}
