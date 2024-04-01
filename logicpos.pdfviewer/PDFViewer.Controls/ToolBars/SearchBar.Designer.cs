namespace Patagames.Pdf.Net.Controls.WinForms.ToolBars
{
	partial class SearchBar
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchBar));
			this.picMenu = new System.Windows.Forms.PictureBox();
			this.pnlBorder = new System.Windows.Forms.Panel();
			this.pnlHostBar = new System.Windows.Forms.Panel();
			this.pnlHostTextBox = new System.Windows.Forms.Panel();
			this.tbSearch = new System.Windows.Forms.TextBox();
			this.lblInfo = new System.Windows.Forms.Label();
			this.picUp = new System.Windows.Forms.PictureBox();
			this.picDown = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.picMenu)).BeginInit();
			this.pnlBorder.SuspendLayout();
			this.pnlHostBar.SuspendLayout();
			this.pnlHostTextBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picUp)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picDown)).BeginInit();
			this.SuspendLayout();
			// 
			// picMenu
			// 
			this.picMenu.Dock = System.Windows.Forms.DockStyle.Left;
			this.picMenu.Image = ((System.Drawing.Image)(resources.GetObject("picMenu.Image")));
			this.picMenu.Location = new System.Drawing.Point(0, 0);
			this.picMenu.MaximumSize = new System.Drawing.Size(32, 32);
			this.picMenu.MinimumSize = new System.Drawing.Size(32, 32);
			this.picMenu.Name = "picMenu";
			this.picMenu.Size = new System.Drawing.Size(32, 32);
			this.picMenu.TabIndex = 0;
			this.picMenu.TabStop = false;
			this.picMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
			this.picMenu.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
			this.picMenu.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
			this.picMenu.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Button_MouseMove);
			this.picMenu.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
			// 
			// pnlBorder
			// 
			this.pnlBorder.Controls.Add(this.pnlHostBar);
			this.pnlBorder.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlBorder.Location = new System.Drawing.Point(0, 0);
			this.pnlBorder.Name = "pnlBorder";
			this.pnlBorder.Padding = new System.Windows.Forms.Padding(1);
			this.pnlBorder.Size = new System.Drawing.Size(220, 34);
			this.pnlBorder.TabIndex = 1;
			// 
			// pnlHostBar
			// 
			this.pnlHostBar.AutoSize = true;
			this.pnlHostBar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.pnlHostBar.BackColor = System.Drawing.SystemColors.Window;
			this.pnlHostBar.Controls.Add(this.pnlHostTextBox);
			this.pnlHostBar.Controls.Add(this.lblInfo);
			this.pnlHostBar.Controls.Add(this.picMenu);
			this.pnlHostBar.Controls.Add(this.picUp);
			this.pnlHostBar.Controls.Add(this.picDown);
			this.pnlHostBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlHostBar.Location = new System.Drawing.Point(1, 1);
			this.pnlHostBar.Name = "pnlHostBar";
			this.pnlHostBar.Size = new System.Drawing.Size(218, 32);
			this.pnlHostBar.TabIndex = 4;
			// 
			// pnlHostTextBox
			// 
			this.pnlHostTextBox.Controls.Add(this.tbSearch);
			this.pnlHostTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlHostTextBox.Location = new System.Drawing.Point(32, 0);
			this.pnlHostTextBox.Name = "pnlHostTextBox";
			this.pnlHostTextBox.Size = new System.Drawing.Size(75, 32);
			this.pnlHostTextBox.TabIndex = 6;
			this.pnlHostTextBox.Click += new System.EventHandler(this.pnlHostTextBox_Click);
			// 
			// tbSearch
			// 
			this.tbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.tbSearch.BackColor = System.Drawing.SystemColors.Window;
			this.tbSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbSearch.Location = new System.Drawing.Point(3, 9);
			this.tbSearch.MaxLength = 255;
			this.tbSearch.Name = "tbSearch";
			this.tbSearch.Size = new System.Drawing.Size(69, 13);
			this.tbSearch.TabIndex = 5;
			this.tbSearch.TextChanged += new System.EventHandler(this.tbSearch_TextChanged);
			// 
			// lblInfo
			// 
			this.lblInfo.Dock = System.Windows.Forms.DockStyle.Right;
			this.lblInfo.Location = new System.Drawing.Point(107, 0);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(47, 32);
			this.lblInfo.TabIndex = 4;
			this.lblInfo.Text = "0 of 0";
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// picUp
			// 
			this.picUp.Dock = System.Windows.Forms.DockStyle.Right;
			this.picUp.Location = new System.Drawing.Point(154, 0);
			this.picUp.MaximumSize = new System.Drawing.Size(32, 32);
			this.picUp.MinimumSize = new System.Drawing.Size(32, 32);
			this.picUp.Name = "picUp";
			this.picUp.Size = new System.Drawing.Size(32, 32);
			this.picUp.TabIndex = 1;
			this.picUp.TabStop = false;
			this.picUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
			this.picUp.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
			this.picUp.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
			this.picUp.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Button_MouseMove);
			this.picUp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
			// 
			// picDown
			// 
			this.picDown.Dock = System.Windows.Forms.DockStyle.Right;
			this.picDown.Location = new System.Drawing.Point(186, 0);
			this.picDown.MaximumSize = new System.Drawing.Size(32, 32);
			this.picDown.MinimumSize = new System.Drawing.Size(32, 32);
			this.picDown.Name = "picDown";
			this.picDown.Size = new System.Drawing.Size(32, 32);
			this.picDown.TabIndex = 3;
			this.picDown.TabStop = false;
			this.picDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
			this.picDown.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
			this.picDown.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
			this.picDown.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Button_MouseMove);
			this.picDown.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
			// 
			// SearchBar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlBorder);
			this.MinimumSize = new System.Drawing.Size(220, 32);
			this.Name = "SearchBar";
			this.Size = new System.Drawing.Size(220, 34);
			((System.ComponentModel.ISupportInitialize)(this.picMenu)).EndInit();
			this.pnlBorder.ResumeLayout(false);
			this.pnlBorder.PerformLayout();
			this.pnlHostBar.ResumeLayout(false);
			this.pnlHostTextBox.ResumeLayout(false);
			this.pnlHostTextBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picUp)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picDown)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox picMenu;
		private System.Windows.Forms.Panel pnlBorder;
		private System.Windows.Forms.Panel pnlHostBar;
		private System.Windows.Forms.TextBox tbSearch;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.PictureBox picUp;
		private System.Windows.Forms.Panel pnlHostTextBox;
		private System.Windows.Forms.PictureBox picDown;
	}
}
