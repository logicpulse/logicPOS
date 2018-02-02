namespace logicpos.reports.Forms
{
    partial class FormSelectTerminal
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSelectTerminal));
            this.lb_reportData = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.customPanel1 = new logicpos.reports.CustomPanel();
            this.lb_terminal = new DevExpress.XtraEditors.ListBoxControl();
            this.cb_allTerminal = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lb_endDate = new System.Windows.Forms.Label();
            this.dt_endDate = new System.Windows.Forms.DateTimePicker();
            this.lb_startDate = new System.Windows.Forms.Label();
            this.dt_startDate = new System.Windows.Forms.DateTimePicker();
            this.customPanel2 = new logicpos.reports.CustomPanel();
            this.customPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lb_terminal)).BeginInit();
            this.customPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_reportData
            // 
            this.lb_reportData.AutoSize = true;
            this.lb_reportData.Font = new System.Drawing.Font("Arial", 18F);
            this.lb_reportData.ForeColor = System.Drawing.Color.White;
            this.lb_reportData.Location = new System.Drawing.Point(1, 9);
            this.lb_reportData.Name = "lb_reportData";
            this.lb_reportData.Size = new System.Drawing.Size(174, 27);
            this.lb_reportData.TabIndex = 44;
            this.lb_reportData.Text = "Dados relatório";
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.FlatAppearance.BorderSize = 0;
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCancel.Font = new System.Drawing.Font("Arial", 8.25F);
            this.buttonCancel.ForeColor = System.Drawing.Color.White;
            this.buttonCancel.Image = ((System.Drawing.Image)(resources.GetObject("buttonCancel.Image")));
            this.buttonCancel.Location = new System.Drawing.Point(219, 312);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Padding = new System.Windows.Forms.Padding(2);
            this.buttonCancel.Size = new System.Drawing.Size(70, 60);
            this.buttonCancel.TabIndex = 42;
            this.buttonCancel.Text = "Voltar";
            this.buttonCancel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.FlatAppearance.BorderSize = 0;
            this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOk.Font = new System.Drawing.Font("Arial", 8.25F);
            this.buttonOk.ForeColor = System.Drawing.Color.White;
            this.buttonOk.Image = ((System.Drawing.Image)(resources.GetObject("buttonOk.Image")));
            this.buttonOk.Location = new System.Drawing.Point(143, 312);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Padding = new System.Windows.Forms.Padding(2);
            this.buttonOk.Size = new System.Drawing.Size(70, 60);
            this.buttonOk.TabIndex = 41;
            this.buttonOk.Text = "OK";
            this.buttonOk.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonOk.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonOk.UseVisualStyleBackColor = false;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // customPanel1
            // 
            this.customPanel1.Controls.Add(this.lb_terminal);
            this.customPanel1.Controls.Add(this.cb_allTerminal);
            this.customPanel1.Controls.Add(this.label1);
            this.customPanel1.Location = new System.Drawing.Point(6, 41);
            this.customPanel1.Name = "customPanel1";
            this.customPanel1.Size = new System.Drawing.Size(283, 196);
            this.customPanel1.TabIndex = 37;
            // 
            // lb_terminal
            // 
            this.lb_terminal.Appearance.Font = new System.Drawing.Font("Arial", 10F);
            this.lb_terminal.Appearance.Options.UseFont = true;
            this.lb_terminal.Location = new System.Drawing.Point(12, 30);
            this.lb_terminal.Name = "lb_terminal";
            this.lb_terminal.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lb_terminal.Size = new System.Drawing.Size(259, 133);
            this.lb_terminal.TabIndex = 48;
            // 
            // cb_allTerminal
            // 
            this.cb_allTerminal.AutoSize = true;
            this.cb_allTerminal.Font = new System.Drawing.Font("Arial", 9F);
            this.cb_allTerminal.Location = new System.Drawing.Point(12, 169);
            this.cb_allTerminal.Name = "cb_allTerminal";
            this.cb_allTerminal.Size = new System.Drawing.Size(60, 19);
            this.cb_allTerminal.TabIndex = 47;
            this.cb_allTerminal.Text = "Todos";
            this.cb_allTerminal.UseVisualStyleBackColor = true;
            this.cb_allTerminal.CheckedChanged += new System.EventHandler(this.cb_allTerminal_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 18);
            this.label1.TabIndex = 45;
            this.label1.Text = "Terminal:";
            // 
            // lb_endDate
            // 
            this.lb_endDate.AutoSize = true;
            this.lb_endDate.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lb_endDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_endDate.Location = new System.Drawing.Point(6, 41);
            this.lb_endDate.Name = "lb_endDate";
            this.lb_endDate.Size = new System.Drawing.Size(78, 18);
            this.lb_endDate.TabIndex = 48;
            this.lb_endDate.Text = "Data final:";
            // 
            // dt_endDate
            // 
            this.dt_endDate.CalendarFont = new System.Drawing.Font("Arial", 9F);
            this.dt_endDate.CustomFormat = "dd/MM/yyyy";
            this.dt_endDate.Font = new System.Drawing.Font("Arial", 9F);
            this.dt_endDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_endDate.Location = new System.Drawing.Point(101, 38);
            this.dt_endDate.Name = "dt_endDate";
            this.dt_endDate.Size = new System.Drawing.Size(170, 21);
            this.dt_endDate.TabIndex = 47;
            // 
            // lb_startDate
            // 
            this.lb_startDate.AutoSize = true;
            this.lb_startDate.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lb_startDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_startDate.Location = new System.Drawing.Point(6, 12);
            this.lb_startDate.Name = "lb_startDate";
            this.lb_startDate.Size = new System.Drawing.Size(89, 18);
            this.lb_startDate.TabIndex = 46;
            this.lb_startDate.Text = "Data inicial:";
            // 
            // dt_startDate
            // 
            this.dt_startDate.CalendarFont = new System.Drawing.Font("Arial", 9F);
            this.dt_startDate.CustomFormat = "dd/MM/yyyy";
            this.dt_startDate.Font = new System.Drawing.Font("Arial", 9F);
            this.dt_startDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_startDate.Location = new System.Drawing.Point(101, 9);
            this.dt_startDate.Name = "dt_startDate";
            this.dt_startDate.Size = new System.Drawing.Size(170, 21);
            this.dt_startDate.TabIndex = 45;
            // 
            // customPanel2
            // 
            this.customPanel2.Controls.Add(this.lb_endDate);
            this.customPanel2.Controls.Add(this.dt_endDate);
            this.customPanel2.Controls.Add(this.dt_startDate);
            this.customPanel2.Controls.Add(this.lb_startDate);
            this.customPanel2.Location = new System.Drawing.Point(6, 238);
            this.customPanel2.Name = "customPanel2";
            this.customPanel2.Size = new System.Drawing.Size(283, 68);
            this.customPanel2.TabIndex = 49;
            // 
            // FormSelectTerminal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(88)))), ((int)(((byte)(88)))));
            this.ClientSize = new System.Drawing.Size(301, 385);
            this.Controls.Add(this.customPanel2);
            this.Controls.Add(this.lb_reportData);
            this.Controls.Add(this.customPanel1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSelectTerminal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New";
            this.customPanel1.ResumeLayout(false);
            this.customPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lb_terminal)).EndInit();
            this.customPanel2.ResumeLayout(false);
            this.customPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_reportData;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label1;
        private CustomPanel customPanel1;
        private DevExpress.XtraEditors.ListBoxControl lb_terminal;
        private System.Windows.Forms.CheckBox cb_allTerminal;
        private System.Windows.Forms.Label lb_endDate;
        private System.Windows.Forms.DateTimePicker dt_endDate;
        private System.Windows.Forms.Label lb_startDate;
        private System.Windows.Forms.DateTimePicker dt_startDate;
        private CustomPanel customPanel2;
    }
}