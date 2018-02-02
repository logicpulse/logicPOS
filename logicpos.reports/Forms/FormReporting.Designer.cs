using System.Windows.Forms;
namespace logicpos.reports
{
    partial class FormReporting
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReporting));
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.bt_searchCustomers = new System.Windows.Forms.Button();
            this.cb_customers = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.bt_searchtypemovement = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.panel_date = new System.Windows.Forms.Panel();
            this.lb_endDate = new System.Windows.Forms.Label();
            this.dt_endDate = new System.Windows.Forms.DateTimePicker();
            this.bt_searchDate = new System.Windows.Forms.Button();
            this.lb_startDate = new System.Windows.Forms.Label();
            this.dt_startDate = new System.Windows.Forms.DateTimePicker();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tbPageNo = new System.Windows.Forms.TextBox();
            this.exportMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnLast = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrior = new System.Windows.Forms.Button();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.bt_design = new System.Windows.Forms.Button();
            this.bt_close = new System.Windows.Forms.Button();
            this.previewControl1 = new FastReport.Preview.PreviewControl();
            this.panel_defaultReport = new System.Windows.Forms.Panel();
            this.manualScrollPanel1 = new System.Windows.Forms.Panel();
            this.manualScrollPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.bt_closingBox = new System.Windows.Forms.Button();
            this.bt_closingDay = new System.Windows.Forms.Button();
            this.bt_CurrentAccount = new System.Windows.Forms.Button();
            this.bt_DetailsCurrentAccount = new System.Windows.Forms.Button();
            this.bt_totalCash = new System.Windows.Forms.Button();
            this.bt_totalday = new System.Windows.Forms.Button();
            this.bt_totalterminal = new System.Windows.Forms.Button();
            this.bt_totalfamily = new System.Windows.Forms.Button();
            this.bt_totalCustomer = new System.Windows.Forms.Button();
            this.bt_movCustomers = new System.Windows.Forms.Button();
            this.bt_recordsCustomer = new System.Windows.Forms.Button();
            this.bt_totalzone = new System.Windows.Forms.Button();
            this.bt_totalzonetable = new System.Windows.Forms.Button();
            this.bt_occupationPlace = new System.Windows.Forms.Button();
            this.bt_listArticles = new System.Windows.Forms.Button();
            this.bt_scrollup = new System.Windows.Forms.Button();
            this.bt_scrolldown = new System.Windows.Forms.Button();
            this.panel_SearchItemsClosingDay = new System.Windows.Forms.Panel();
            this.lb_ItemsClosingDay = new DevExpress.XtraEditors.ListBoxControl();
            this.lb_employee = new System.Windows.Forms.Label();
            this.cb_customer = new System.Windows.Forms.ComboBox();
            this.bt_searchEmployee = new System.Windows.Forms.Button();
            this.panel_Customers = new System.Windows.Forms.Panel();
            this.lb_zones = new System.Windows.Forms.Label();
            this.cb_zones = new System.Windows.Forms.ComboBox();
            this.bt_searchZones = new System.Windows.Forms.Button();
            this.panel_Zones = new System.Windows.Forms.Panel();
            this.lb_family = new System.Windows.Forms.Label();
            this.cb_family = new System.Windows.Forms.ComboBox();
            this.bt_searchFamily = new System.Windows.Forms.Button();
            this.panel_Family = new System.Windows.Forms.Panel();
            this.lb_typeMovement = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.comboBox5 = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.comboBox6 = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.bt_typemovement = new System.Windows.Forms.Button();
            this.cb_typemovement = new System.Windows.Forms.ComboBox();
            this.panel_Movements = new System.Windows.Forms.Panel();
            this.lb_zone = new System.Windows.Forms.Label();
            this.cb_zone = new System.Windows.Forms.ComboBox();
            this.lb_table = new System.Windows.Forms.Label();
            this.cb_table = new System.Windows.Forms.ComboBox();
            this.bt_searchTables = new System.Windows.Forms.Button();
            this.panel_Zone_Table = new System.Windows.Forms.Panel();
            this.lb_terminal = new System.Windows.Forms.Label();
            this.cb_terminal = new System.Windows.Forms.ComboBox();
            this.bt_terminal = new System.Windows.Forms.Button();
            this.panel_Terminal = new System.Windows.Forms.Panel();
            this.lb_typePayment = new System.Windows.Forms.Label();
            this.cb_typepayment = new System.Windows.Forms.ComboBox();
            this.bt_searchtypepayment = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel_Payment = new System.Windows.Forms.Panel();
            this.lb_customer = new System.Windows.Forms.Label();
            this.cb_clients = new System.Windows.Forms.ComboBox();
            this.lb_StatePay = new System.Windows.Forms.Label();
            this.bt_client = new System.Windows.Forms.Button();
            this.cb_StatePay = new System.Windows.Forms.ComboBox();
            this.panel_Clients = new System.Windows.Forms.Panel();
            this.br_searchTerminalDatesClosingBox = new System.Windows.Forms.Button();
            this.lb_ItemsClosingBox = new DevExpress.XtraEditors.ListBoxControl();
            this.panel_SearchItemsClosingBox = new System.Windows.Forms.Panel();
            this.bt_stockArticles = new System.Windows.Forms.Button();
            this.panel6.SuspendLayout();
            this.panel_date.SuspendLayout();
            this.panel3.SuspendLayout();
            this.manualScrollPanel1.SuspendLayout();
            this.manualScrollPanel2.SuspendLayout();
            this.panel_SearchItemsClosingDay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lb_ItemsClosingDay)).BeginInit();
            this.panel_Customers.SuspendLayout();
            this.panel_Zones.SuspendLayout();
            this.panel_Family.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel_Movements.SuspendLayout();
            this.panel_Zone_Table.SuspendLayout();
            this.panel_Terminal.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel_Payment.SuspendLayout();
            this.panel_Clients.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lb_ItemsClosingBox)).BeginInit();
            this.panel_SearchItemsClosingBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(0, 0);
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(100, 20);
            this.tbDescription.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.label10.Location = new System.Drawing.Point(5, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(61, 18);
            this.label10.TabIndex = 3;
            this.label10.Text = "Família:";
            // 
            // bt_searchCustomers
            // 
            this.bt_searchCustomers.Location = new System.Drawing.Point(0, 0);
            this.bt_searchCustomers.Name = "bt_searchCustomers";
            this.bt_searchCustomers.Size = new System.Drawing.Size(75, 23);
            this.bt_searchCustomers.TabIndex = 0;
            // 
            // cb_customers
            // 
            this.cb_customers.Location = new System.Drawing.Point(0, 0);
            this.cb_customers.Name = "cb_customers";
            this.cb_customers.Size = new System.Drawing.Size(121, 21);
            this.cb_customers.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.label6.Location = new System.Drawing.Point(5, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 18);
            this.label6.TabIndex = 3;
            this.label6.Text = "Empregado:";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel6.Controls.Add(this.button3);
            this.panel6.Controls.Add(this.comboBox4);
            this.panel6.Controls.Add(this.label7);
            this.panel6.Location = new System.Drawing.Point(10, 515);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(250, 84);
            this.panel6.TabIndex = 16;
            this.panel6.Visible = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.button3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button3.BackgroundImage")));
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button3.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(199, 36);
            this.button3.Name = "button3";
            this.button3.Padding = new System.Windows.Forms.Padding(2);
            this.button3.Size = new System.Drawing.Size(43, 43);
            this.button3.TabIndex = 25;
            this.button3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.button3.UseVisualStyleBackColor = false;
            // 
            // comboBox4
            // 
            this.comboBox4.Font = new System.Drawing.Font("Arial", 10.5F);
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Location = new System.Drawing.Point(72, 6);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(170, 24);
            this.comboBox4.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.label7.Location = new System.Drawing.Point(5, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 18);
            this.label7.TabIndex = 3;
            this.label7.Text = "Família:";
            // 
            // bt_searchtypemovement
            // 
            this.bt_searchtypemovement.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.bt_searchtypemovement.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_searchtypemovement.BackgroundImage")));
            this.bt_searchtypemovement.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_searchtypemovement.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_searchtypemovement.FlatAppearance.BorderSize = 0;
            this.bt_searchtypemovement.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_searchtypemovement.ForeColor = System.Drawing.Color.White;
            this.bt_searchtypemovement.Location = new System.Drawing.Point(199, 36);
            this.bt_searchtypemovement.Name = "bt_searchtypemovement";
            this.bt_searchtypemovement.Padding = new System.Windows.Forms.Padding(2);
            this.bt_searchtypemovement.Size = new System.Drawing.Size(43, 43);
            this.bt_searchtypemovement.TabIndex = 27;
            this.bt_searchtypemovement.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bt_searchtypemovement.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bt_searchtypemovement.UseVisualStyleBackColor = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.label11.Location = new System.Drawing.Point(5, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(106, 18);
            this.label11.TabIndex = 3;
            this.label11.Text = "T. movimento:";
            // 
            // panel_date
            // 
            this.panel_date.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel_date.Controls.Add(this.lb_endDate);
            this.panel_date.Controls.Add(this.dt_endDate);
            this.panel_date.Controls.Add(this.bt_searchDate);
            this.panel_date.Controls.Add(this.lb_startDate);
            this.panel_date.Controls.Add(this.dt_startDate);
            this.panel_date.Location = new System.Drawing.Point(10, 370);
            this.panel_date.Name = "panel_date";
            this.panel_date.Size = new System.Drawing.Size(250, 137);
            this.panel_date.TabIndex = 18;
            // 
            // lb_endDate
            // 
            this.lb_endDate.AutoSize = true;
            this.lb_endDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lb_endDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_endDate.Location = new System.Drawing.Point(7, 45);
            this.lb_endDate.Name = "lb_endDate";
            this.lb_endDate.Size = new System.Drawing.Size(77, 16);
            this.lb_endDate.TabIndex = 22;
            this.lb_endDate.Text = "Data final:";
            // 
            // dt_endDate
            // 
            this.dt_endDate.CalendarFont = new System.Drawing.Font("Arial", 9F);
            this.dt_endDate.CustomFormat = "dd/MM/yyyy";
            this.dt_endDate.Font = new System.Drawing.Font("Arial", 9F);
            this.dt_endDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_endDate.Location = new System.Drawing.Point(10, 62);
            this.dt_endDate.Name = "dt_endDate";
            this.dt_endDate.Size = new System.Drawing.Size(234, 21);
            this.dt_endDate.TabIndex = 21;
            // 
            // bt_searchDate
            // 
            this.bt_searchDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.bt_searchDate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_searchDate.BackgroundImage")));
            this.bt_searchDate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_searchDate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_searchDate.FlatAppearance.BorderSize = 0;
            this.bt_searchDate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_searchDate.ForeColor = System.Drawing.Color.White;
            this.bt_searchDate.Location = new System.Drawing.Point(201, 87);
            this.bt_searchDate.Name = "bt_searchDate";
            this.bt_searchDate.Padding = new System.Windows.Forms.Padding(2);
            this.bt_searchDate.Size = new System.Drawing.Size(43, 43);
            this.bt_searchDate.TabIndex = 20;
            this.bt_searchDate.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bt_searchDate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bt_searchDate.UseVisualStyleBackColor = false;
            this.bt_searchDate.Click += new System.EventHandler(this.bt_searchDate_Click);
            // 
            // lb_startDate
            // 
            this.lb_startDate.AutoSize = true;
            this.lb_startDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lb_startDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_startDate.Location = new System.Drawing.Point(7, 4);
            this.lb_startDate.Name = "lb_startDate";
            this.lb_startDate.Size = new System.Drawing.Size(88, 16);
            this.lb_startDate.TabIndex = 3;
            this.lb_startDate.Text = "Data inicial:";
            // 
            // dt_startDate
            // 
            this.dt_startDate.CalendarFont = new System.Drawing.Font("Arial", 9F);
            this.dt_startDate.CustomFormat = "dd/MM/yyyy";
            this.dt_startDate.Font = new System.Drawing.Font("Arial", 9F);
            this.dt_startDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_startDate.Location = new System.Drawing.Point(10, 22);
            this.dt_startDate.Name = "dt_startDate";
            this.dt_startDate.Size = new System.Drawing.Size(234, 21);
            this.dt_startDate.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(5);
            this.panel3.Size = new System.Drawing.Size(1036, 44);
            this.panel3.TabIndex = 19;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.panel4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel4.BackgroundImage")));
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel4.Location = new System.Drawing.Point(7, 9);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(5);
            this.panel4.Size = new System.Drawing.Size(165, 30);
            this.panel4.TabIndex = 20;
            // 
            // tbPageNo
            // 
            this.tbPageNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbPageNo.Font = new System.Drawing.Font("Arial", 23F);
            this.tbPageNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.tbPageNo.Location = new System.Drawing.Point(566, 50);
            this.tbPageNo.Name = "tbPageNo";
            this.tbPageNo.Size = new System.Drawing.Size(99, 43);
            this.tbPageNo.TabIndex = 40;
            this.tbPageNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbPageNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPageNo_KeyDown);
            // 
            // exportMenu
            // 
            this.exportMenu.Name = "exportMenu";
            this.exportMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // btnLast
            // 
            this.btnLast.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.btnLast.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnLast.BackgroundImage")));
            this.btnLast.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLast.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnLast.FlatAppearance.BorderSize = 0;
            this.btnLast.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLast.ForeColor = System.Drawing.Color.White;
            this.btnLast.Location = new System.Drawing.Point(720, 50);
            this.btnLast.Name = "btnLast";
            this.btnLast.Padding = new System.Windows.Forms.Padding(2);
            this.btnLast.Size = new System.Drawing.Size(43, 43);
            this.btnLast.TabIndex = 42;
            this.btnLast.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnLast.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnLast.UseVisualStyleBackColor = false;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.btnNext.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnNext.BackgroundImage")));
            this.btnNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnNext.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.ForeColor = System.Drawing.Color.White;
            this.btnNext.Location = new System.Drawing.Point(671, 50);
            this.btnNext.Name = "btnNext";
            this.btnNext.Padding = new System.Windows.Forms.Padding(2);
            this.btnNext.Size = new System.Drawing.Size(43, 43);
            this.btnNext.TabIndex = 41;
            this.btnNext.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNext.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrior
            // 
            this.btnPrior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.btnPrior.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPrior.BackgroundImage")));
            this.btnPrior.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPrior.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnPrior.FlatAppearance.BorderSize = 0;
            this.btnPrior.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrior.ForeColor = System.Drawing.Color.White;
            this.btnPrior.Location = new System.Drawing.Point(517, 50);
            this.btnPrior.Name = "btnPrior";
            this.btnPrior.Padding = new System.Windows.Forms.Padding(2);
            this.btnPrior.Size = new System.Drawing.Size(43, 43);
            this.btnPrior.TabIndex = 40;
            this.btnPrior.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnPrior.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPrior.UseVisualStyleBackColor = false;
            this.btnPrior.Click += new System.EventHandler(this.btnPrior_Click);
            // 
            // btnFirst
            // 
            this.btnFirst.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.btnFirst.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFirst.BackgroundImage")));
            this.btnFirst.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFirst.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnFirst.FlatAppearance.BorderSize = 0;
            this.btnFirst.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFirst.ForeColor = System.Drawing.Color.White;
            this.btnFirst.Location = new System.Drawing.Point(468, 50);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Padding = new System.Windows.Forms.Padding(2);
            this.btnFirst.Size = new System.Drawing.Size(43, 43);
            this.btnFirst.TabIndex = 39;
            this.btnFirst.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnFirst.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnFirst.UseVisualStyleBackColor = false;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.btnZoomIn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnZoomIn.BackgroundImage")));
            this.btnZoomIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnZoomIn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnZoomIn.FlatAppearance.BorderSize = 0;
            this.btnZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZoomIn.ForeColor = System.Drawing.Color.White;
            this.btnZoomIn.Location = new System.Drawing.Point(916, 50);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Padding = new System.Windows.Forms.Padding(2);
            this.btnZoomIn.Size = new System.Drawing.Size(43, 43);
            this.btnZoomIn.TabIndex = 38;
            this.btnZoomIn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnZoomIn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnZoomIn.UseVisualStyleBackColor = false;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.btnZoomOut.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnZoomOut.BackgroundImage")));
            this.btnZoomOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnZoomOut.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnZoomOut.FlatAppearance.BorderSize = 0;
            this.btnZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZoomOut.ForeColor = System.Drawing.Color.White;
            this.btnZoomOut.Location = new System.Drawing.Point(867, 50);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Padding = new System.Windows.Forms.Padding(2);
            this.btnZoomOut.Size = new System.Drawing.Size(43, 43);
            this.btnZoomOut.TabIndex = 37;
            this.btnZoomOut.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnZoomOut.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnZoomOut.UseVisualStyleBackColor = false;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.btnExport.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnExport.BackgroundImage")));
            this.btnExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExport.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(315, 50);
            this.btnExport.Name = "btnExport";
            this.btnExport.Padding = new System.Windows.Forms.Padding(2);
            this.btnExport.Size = new System.Drawing.Size(43, 43);
            this.btnExport.TabIndex = 36;
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.btnPrint.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPrint.BackgroundImage")));
            this.btnPrint.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPrint.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnPrint.FlatAppearance.BorderSize = 0;
            this.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrint.ForeColor = System.Drawing.Color.White;
            this.btnPrint.Location = new System.Drawing.Point(266, 50);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Padding = new System.Windows.Forms.Padding(2);
            this.btnPrint.Size = new System.Drawing.Size(43, 43);
            this.btnPrint.TabIndex = 35;
            this.btnPrint.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // bt_design
            // 
            this.bt_design.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bt_design.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.bt_design.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_design.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_design.FlatAppearance.BorderSize = 0;
            this.bt_design.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_design.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.bt_design.ForeColor = System.Drawing.Color.White;
            this.bt_design.Image = ((System.Drawing.Image)(resources.GetObject("bt_design.Image")));
            this.bt_design.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_design.Location = new System.Drawing.Point(10, 678);
            this.bt_design.Name = "bt_design";
            this.bt_design.Padding = new System.Windows.Forms.Padding(2);
            this.bt_design.Size = new System.Drawing.Size(190, 54);
            this.bt_design.TabIndex = 24;
            this.bt_design.Text = "Desenhar";
            this.bt_design.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_design.UseVisualStyleBackColor = false;
            this.bt_design.Click += new System.EventHandler(this.bt_design_Click);
            // 
            // bt_close
            // 
            this.bt_close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bt_close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(100)))), ((int)(((byte)(86)))));
            this.bt_close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_close.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_close.FlatAppearance.BorderSize = 0;
            this.bt_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_close.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_close.ForeColor = System.Drawing.Color.White;
            this.bt_close.Image = ((System.Drawing.Image)(resources.GetObject("bt_close.Image")));
            this.bt_close.Location = new System.Drawing.Point(206, 678);
            this.bt_close.Name = "bt_close";
            this.bt_close.Padding = new System.Windows.Forms.Padding(2);
            this.bt_close.Size = new System.Drawing.Size(54, 54);
            this.bt_close.TabIndex = 55;
            this.bt_close.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_close.UseVisualStyleBackColor = false;
            this.bt_close.Click += new System.EventHandler(this.bt_close_Click);
            // 
            // previewControl1
            // 
            this.previewControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.previewControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(153)))), ((int)(((byte)(174)))));
            this.previewControl1.Buttons = ((FastReport.PreviewButtons)(((((((((((FastReport.PreviewButtons.Print | FastReport.PreviewButtons.Open) 
            | FastReport.PreviewButtons.Save) 
            | FastReport.PreviewButtons.Email) 
            | FastReport.PreviewButtons.Find) 
            | FastReport.PreviewButtons.Zoom) 
            | FastReport.PreviewButtons.Outline) 
            | FastReport.PreviewButtons.PageSetup) 
            | FastReport.PreviewButtons.Edit) 
            | FastReport.PreviewButtons.Watermark) 
            | FastReport.PreviewButtons.Navigator)));
            this.previewControl1.Font = new System.Drawing.Font("Arial", 8F);
            this.previewControl1.Location = new System.Drawing.Point(266, 99);
            this.previewControl1.Name = "previewControl1";
            this.previewControl1.PageOffset = new System.Drawing.Point(0, 0);
            this.previewControl1.Size = new System.Drawing.Size(758, 669);
            this.previewControl1.StatusbarVisible = false;
            this.previewControl1.TabIndex = 8;
            this.previewControl1.ToolbarVisible = false;
            this.previewControl1.UIStyle = FastReport.Utils.UIStyle.Office2003;
            this.previewControl1.PageChanged += new System.EventHandler(this.previewControl1_PageChanged);
            // 
            // panel_defaultReport
            // 
            this.panel_defaultReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_defaultReport.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel_defaultReport.BackgroundImage")));
            this.panel_defaultReport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panel_defaultReport.Location = new System.Drawing.Point(266, 45);
            this.panel_defaultReport.Name = "panel_defaultReport";
            this.panel_defaultReport.Size = new System.Drawing.Size(770, 723);
            this.panel_defaultReport.TabIndex = 58;
            // 
            // manualScrollPanel1
            // 
            this.manualScrollPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.manualScrollPanel1.Controls.Add(this.manualScrollPanel2);
            this.manualScrollPanel1.Location = new System.Drawing.Point(10, 84);
            this.manualScrollPanel1.Name = "manualScrollPanel1";
            this.manualScrollPanel1.Size = new System.Drawing.Size(247, 242);
            this.manualScrollPanel1.TabIndex = 63;
            // 
            // manualScrollPanel2
            // 
            this.manualScrollPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.manualScrollPanel2.Controls.Add(this.bt_closingBox);
            this.manualScrollPanel2.Controls.Add(this.bt_closingDay);
            this.manualScrollPanel2.Controls.Add(this.bt_CurrentAccount);
            this.manualScrollPanel2.Controls.Add(this.bt_DetailsCurrentAccount);
            this.manualScrollPanel2.Controls.Add(this.bt_totalCash);
            this.manualScrollPanel2.Controls.Add(this.bt_totalday);
            this.manualScrollPanel2.Controls.Add(this.bt_totalterminal);
            this.manualScrollPanel2.Controls.Add(this.bt_totalfamily);
            this.manualScrollPanel2.Controls.Add(this.bt_totalCustomer);
            this.manualScrollPanel2.Controls.Add(this.bt_movCustomers);
            this.manualScrollPanel2.Controls.Add(this.bt_recordsCustomer);
            this.manualScrollPanel2.Controls.Add(this.bt_totalzone);
            this.manualScrollPanel2.Controls.Add(this.bt_totalzonetable);
            this.manualScrollPanel2.Controls.Add(this.bt_occupationPlace);
            this.manualScrollPanel2.Controls.Add(this.bt_listArticles);
            this.manualScrollPanel2.Controls.Add(this.bt_stockArticles);
            this.manualScrollPanel2.Location = new System.Drawing.Point(3, 3);
            this.manualScrollPanel2.Name = "manualScrollPanel2";
            this.manualScrollPanel2.Size = new System.Drawing.Size(250, 618);
            this.manualScrollPanel2.TabIndex = 62;
            // 
            // bt_closingBox
            // 
            this.bt_closingBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_closingBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_closingBox.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_closingBox.FlatAppearance.BorderSize = 0;
            this.bt_closingBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_closingBox.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_closingBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_closingBox.Image = ((System.Drawing.Image)(resources.GetObject("bt_closingBox.Image")));
            this.bt_closingBox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_closingBox.Location = new System.Drawing.Point(3, 3);
            this.bt_closingBox.Name = "bt_closingBox";
            this.bt_closingBox.Padding = new System.Windows.Forms.Padding(2);
            this.bt_closingBox.Size = new System.Drawing.Size(236, 33);
            this.bt_closingBox.TabIndex = 78;
            this.bt_closingBox.Text = "Fecho da caixa";
            this.bt_closingBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_closingBox.UseVisualStyleBackColor = false;
            this.bt_closingBox.Click += new System.EventHandler(this.bt_closingBox_Click);
            // 
            // bt_closingDay
            // 
            this.bt_closingDay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_closingDay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_closingDay.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_closingDay.FlatAppearance.BorderSize = 0;
            this.bt_closingDay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_closingDay.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_closingDay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_closingDay.Image = ((System.Drawing.Image)(resources.GetObject("bt_closingDay.Image")));
            this.bt_closingDay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_closingDay.Location = new System.Drawing.Point(3, 42);
            this.bt_closingDay.Name = "bt_closingDay";
            this.bt_closingDay.Padding = new System.Windows.Forms.Padding(2);
            this.bt_closingDay.Size = new System.Drawing.Size(236, 33);
            this.bt_closingDay.TabIndex = 79;
            this.bt_closingDay.Text = "Fecho do dia";
            this.bt_closingDay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_closingDay.UseVisualStyleBackColor = false;
            this.bt_closingDay.Click += new System.EventHandler(this.bt_closingDay_Click);
            // 
            // bt_CurrentAccount
            // 
            this.bt_CurrentAccount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_CurrentAccount.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_CurrentAccount.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_CurrentAccount.FlatAppearance.BorderSize = 0;
            this.bt_CurrentAccount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_CurrentAccount.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_CurrentAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_CurrentAccount.Image = ((System.Drawing.Image)(resources.GetObject("bt_CurrentAccount.Image")));
            this.bt_CurrentAccount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_CurrentAccount.Location = new System.Drawing.Point(3, 81);
            this.bt_CurrentAccount.Name = "bt_CurrentAccount";
            this.bt_CurrentAccount.Padding = new System.Windows.Forms.Padding(2);
            this.bt_CurrentAccount.Size = new System.Drawing.Size(236, 33);
            this.bt_CurrentAccount.TabIndex = 80;
            this.bt_CurrentAccount.Text = "Conta Corrente Cliente";
            this.bt_CurrentAccount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_CurrentAccount.UseVisualStyleBackColor = false;
            this.bt_CurrentAccount.Click += new System.EventHandler(this.bt_CurrentAccount_Click);
            // 
            // bt_DetailsCurrentAccount
            // 
            this.bt_DetailsCurrentAccount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_DetailsCurrentAccount.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_DetailsCurrentAccount.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_DetailsCurrentAccount.FlatAppearance.BorderSize = 0;
            this.bt_DetailsCurrentAccount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_DetailsCurrentAccount.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_DetailsCurrentAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_DetailsCurrentAccount.Image = ((System.Drawing.Image)(resources.GetObject("bt_DetailsCurrentAccount.Image")));
            this.bt_DetailsCurrentAccount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_DetailsCurrentAccount.Location = new System.Drawing.Point(3, 120);
            this.bt_DetailsCurrentAccount.Name = "bt_DetailsCurrentAccount";
            this.bt_DetailsCurrentAccount.Padding = new System.Windows.Forms.Padding(2);
            this.bt_DetailsCurrentAccount.Size = new System.Drawing.Size(236, 33);
            this.bt_DetailsCurrentAccount.TabIndex = 81;
            this.bt_DetailsCurrentAccount.Text = "C.C. Cliente (detalhado)";
            this.bt_DetailsCurrentAccount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_DetailsCurrentAccount.UseVisualStyleBackColor = false;
            this.bt_DetailsCurrentAccount.Click += new System.EventHandler(this.bt_DetailsCurrentAccount_Click);
            // 
            // bt_totalCash
            // 
            this.bt_totalCash.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_totalCash.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_totalCash.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_totalCash.FlatAppearance.BorderSize = 0;
            this.bt_totalCash.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_totalCash.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_totalCash.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_totalCash.Image = ((System.Drawing.Image)(resources.GetObject("bt_totalCash.Image")));
            this.bt_totalCash.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_totalCash.Location = new System.Drawing.Point(3, 159);
            this.bt_totalCash.Name = "bt_totalCash";
            this.bt_totalCash.Padding = new System.Windows.Forms.Padding(2);
            this.bt_totalCash.Size = new System.Drawing.Size(236, 31);
            this.bt_totalCash.TabIndex = 74;
            this.bt_totalCash.Text = "Total caixa";
            this.bt_totalCash.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_totalCash.UseVisualStyleBackColor = false;
            this.bt_totalCash.Click += new System.EventHandler(this.bt_totalCash_Click);
            // 
            // bt_totalday
            // 
            this.bt_totalday.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_totalday.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_totalday.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_totalday.FlatAppearance.BorderSize = 0;
            this.bt_totalday.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_totalday.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_totalday.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_totalday.Image = ((System.Drawing.Image)(resources.GetObject("bt_totalday.Image")));
            this.bt_totalday.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_totalday.Location = new System.Drawing.Point(3, 196);
            this.bt_totalday.Name = "bt_totalday";
            this.bt_totalday.Padding = new System.Windows.Forms.Padding(2);
            this.bt_totalday.Size = new System.Drawing.Size(236, 33);
            this.bt_totalday.TabIndex = 67;
            this.bt_totalday.Text = "Total por dia";
            this.bt_totalday.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_totalday.UseVisualStyleBackColor = false;
            this.bt_totalday.Click += new System.EventHandler(this.bt_totalday_Click);
            // 
            // bt_totalterminal
            // 
            this.bt_totalterminal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_totalterminal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_totalterminal.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_totalterminal.FlatAppearance.BorderSize = 0;
            this.bt_totalterminal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_totalterminal.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_totalterminal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_totalterminal.Image = ((System.Drawing.Image)(resources.GetObject("bt_totalterminal.Image")));
            this.bt_totalterminal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_totalterminal.Location = new System.Drawing.Point(3, 235);
            this.bt_totalterminal.Name = "bt_totalterminal";
            this.bt_totalterminal.Padding = new System.Windows.Forms.Padding(2);
            this.bt_totalterminal.Size = new System.Drawing.Size(236, 33);
            this.bt_totalterminal.TabIndex = 71;
            this.bt_totalterminal.Text = "Total por terminal";
            this.bt_totalterminal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_totalterminal.UseVisualStyleBackColor = false;
            this.bt_totalterminal.Click += new System.EventHandler(this.bt_totalterminal_Click);
            // 
            // bt_totalfamily
            // 
            this.bt_totalfamily.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_totalfamily.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_totalfamily.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_totalfamily.FlatAppearance.BorderSize = 0;
            this.bt_totalfamily.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_totalfamily.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_totalfamily.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_totalfamily.Image = ((System.Drawing.Image)(resources.GetObject("bt_totalfamily.Image")));
            this.bt_totalfamily.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_totalfamily.Location = new System.Drawing.Point(3, 274);
            this.bt_totalfamily.Name = "bt_totalfamily";
            this.bt_totalfamily.Padding = new System.Windows.Forms.Padding(2);
            this.bt_totalfamily.Size = new System.Drawing.Size(236, 33);
            this.bt_totalfamily.TabIndex = 69;
            this.bt_totalfamily.Text = "Total por família";
            this.bt_totalfamily.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_totalfamily.UseVisualStyleBackColor = false;
            this.bt_totalfamily.Click += new System.EventHandler(this.bt_totalfamily_Click);
            // 
            // bt_totalCustomer
            // 
            this.bt_totalCustomer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_totalCustomer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_totalCustomer.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_totalCustomer.FlatAppearance.BorderSize = 0;
            this.bt_totalCustomer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_totalCustomer.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_totalCustomer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_totalCustomer.Image = ((System.Drawing.Image)(resources.GetObject("bt_totalCustomer.Image")));
            this.bt_totalCustomer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_totalCustomer.Location = new System.Drawing.Point(3, 313);
            this.bt_totalCustomer.Name = "bt_totalCustomer";
            this.bt_totalCustomer.Padding = new System.Windows.Forms.Padding(2);
            this.bt_totalCustomer.Size = new System.Drawing.Size(236, 31);
            this.bt_totalCustomer.TabIndex = 77;
            this.bt_totalCustomer.Text = "Total por empregado";
            this.bt_totalCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_totalCustomer.UseVisualStyleBackColor = false;
            this.bt_totalCustomer.Click += new System.EventHandler(this.bt_totalCustomer_Click);
            // 
            // bt_movCustomers
            // 
            this.bt_movCustomers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_movCustomers.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_movCustomers.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_movCustomers.FlatAppearance.BorderSize = 0;
            this.bt_movCustomers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_movCustomers.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_movCustomers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_movCustomers.Image = ((System.Drawing.Image)(resources.GetObject("bt_movCustomers.Image")));
            this.bt_movCustomers.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_movCustomers.Location = new System.Drawing.Point(3, 350);
            this.bt_movCustomers.Name = "bt_movCustomers";
            this.bt_movCustomers.Padding = new System.Windows.Forms.Padding(2);
            this.bt_movCustomers.Size = new System.Drawing.Size(236, 31);
            this.bt_movCustomers.TabIndex = 72;
            this.bt_movCustomers.Text = "Movimentos por empregado";
            this.bt_movCustomers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_movCustomers.UseVisualStyleBackColor = false;
            this.bt_movCustomers.Click += new System.EventHandler(this.bt_movCustomers_Click);
            // 
            // bt_recordsCustomer
            // 
            this.bt_recordsCustomer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_recordsCustomer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_recordsCustomer.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_recordsCustomer.FlatAppearance.BorderSize = 0;
            this.bt_recordsCustomer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_recordsCustomer.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_recordsCustomer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_recordsCustomer.Image = ((System.Drawing.Image)(resources.GetObject("bt_recordsCustomer.Image")));
            this.bt_recordsCustomer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_recordsCustomer.Location = new System.Drawing.Point(3, 387);
            this.bt_recordsCustomer.Name = "bt_recordsCustomer";
            this.bt_recordsCustomer.Padding = new System.Windows.Forms.Padding(2);
            this.bt_recordsCustomer.Size = new System.Drawing.Size(236, 31);
            this.bt_recordsCustomer.TabIndex = 73;
            this.bt_recordsCustomer.Text = "Top registos por empregado";
            this.bt_recordsCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_recordsCustomer.UseVisualStyleBackColor = false;
            this.bt_recordsCustomer.Click += new System.EventHandler(this.bt_recordsCustomer_Click);
            // 
            // bt_totalzone
            // 
            this.bt_totalzone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_totalzone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_totalzone.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_totalzone.FlatAppearance.BorderSize = 0;
            this.bt_totalzone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_totalzone.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_totalzone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_totalzone.Image = ((System.Drawing.Image)(resources.GetObject("bt_totalzone.Image")));
            this.bt_totalzone.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_totalzone.Location = new System.Drawing.Point(3, 424);
            this.bt_totalzone.Name = "bt_totalzone";
            this.bt_totalzone.Padding = new System.Windows.Forms.Padding(2);
            this.bt_totalzone.Size = new System.Drawing.Size(236, 33);
            this.bt_totalzone.TabIndex = 70;
            this.bt_totalzone.Text = "Total por zona";
            this.bt_totalzone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_totalzone.UseVisualStyleBackColor = false;
            this.bt_totalzone.Click += new System.EventHandler(this.bt_totalzone_Click);
            // 
            // bt_totalzonetable
            // 
            this.bt_totalzonetable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_totalzonetable.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_totalzonetable.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_totalzonetable.FlatAppearance.BorderSize = 0;
            this.bt_totalzonetable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_totalzonetable.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_totalzonetable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_totalzonetable.Image = ((System.Drawing.Image)(resources.GetObject("bt_totalzonetable.Image")));
            this.bt_totalzonetable.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_totalzonetable.Location = new System.Drawing.Point(3, 463);
            this.bt_totalzonetable.Name = "bt_totalzonetable";
            this.bt_totalzonetable.Padding = new System.Windows.Forms.Padding(2);
            this.bt_totalzonetable.Size = new System.Drawing.Size(236, 33);
            this.bt_totalzonetable.TabIndex = 68;
            this.bt_totalzonetable.Text = "Total por zona/mesa";
            this.bt_totalzonetable.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_totalzonetable.UseVisualStyleBackColor = false;
            this.bt_totalzonetable.Click += new System.EventHandler(this.bt_totalzonetable_Click);
            // 
            // bt_occupationPlace
            // 
            this.bt_occupationPlace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_occupationPlace.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_occupationPlace.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_occupationPlace.FlatAppearance.BorderSize = 0;
            this.bt_occupationPlace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_occupationPlace.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_occupationPlace.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_occupationPlace.Image = ((System.Drawing.Image)(resources.GetObject("bt_occupationPlace.Image")));
            this.bt_occupationPlace.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_occupationPlace.Location = new System.Drawing.Point(3, 502);
            this.bt_occupationPlace.Name = "bt_occupationPlace";
            this.bt_occupationPlace.Padding = new System.Windows.Forms.Padding(2);
            this.bt_occupationPlace.Size = new System.Drawing.Size(236, 31);
            this.bt_occupationPlace.TabIndex = 76;
            this.bt_occupationPlace.Text = "Ocupação Zona/Mesa";
            this.bt_occupationPlace.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_occupationPlace.UseVisualStyleBackColor = false;
            this.bt_occupationPlace.Click += new System.EventHandler(this.bt_occupationPlace_Click);
            // 
            // bt_listArticles
            // 
            this.bt_listArticles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_listArticles.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_listArticles.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_listArticles.FlatAppearance.BorderSize = 0;
            this.bt_listArticles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_listArticles.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_listArticles.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_listArticles.Image = ((System.Drawing.Image)(resources.GetObject("bt_listArticles.Image")));
            this.bt_listArticles.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_listArticles.Location = new System.Drawing.Point(3, 539);
            this.bt_listArticles.Name = "bt_listArticles";
            this.bt_listArticles.Padding = new System.Windows.Forms.Padding(2);
            this.bt_listArticles.Size = new System.Drawing.Size(236, 31);
            this.bt_listArticles.TabIndex = 75;
            this.bt_listArticles.Text = "Lista de artigos";
            this.bt_listArticles.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_listArticles.UseVisualStyleBackColor = false;
            this.bt_listArticles.Click += new System.EventHandler(this.bt_listArticles_Click);
            // 
            // bt_scrollup
            // 
            this.bt_scrollup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(88)))), ((int)(((byte)(88)))));
            this.bt_scrollup.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_scrollup.BackgroundImage")));
            this.bt_scrollup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_scrollup.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_scrollup.FlatAppearance.BorderSize = 0;
            this.bt_scrollup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_scrollup.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_scrollup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_scrollup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_scrollup.Location = new System.Drawing.Point(59, 45);
            this.bt_scrollup.Name = "bt_scrollup";
            this.bt_scrollup.Padding = new System.Windows.Forms.Padding(2);
            this.bt_scrollup.Size = new System.Drawing.Size(155, 33);
            this.bt_scrollup.TabIndex = 59;
            this.bt_scrollup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_scrollup.UseVisualStyleBackColor = false;
            this.bt_scrollup.Click += new System.EventHandler(this.bt_scrollup_Click);
            // 
            // bt_scrolldown
            // 
            this.bt_scrolldown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(88)))), ((int)(((byte)(88)))));
            this.bt_scrolldown.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_scrolldown.BackgroundImage")));
            this.bt_scrolldown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_scrolldown.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_scrolldown.FlatAppearance.BorderSize = 0;
            this.bt_scrolldown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_scrolldown.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_scrolldown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_scrolldown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_scrolldown.Location = new System.Drawing.Point(59, 332);
            this.bt_scrolldown.Name = "bt_scrolldown";
            this.bt_scrolldown.Padding = new System.Windows.Forms.Padding(2);
            this.bt_scrolldown.Size = new System.Drawing.Size(155, 33);
            this.bt_scrolldown.TabIndex = 60;
            this.bt_scrolldown.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_scrolldown.UseVisualStyleBackColor = false;
            this.bt_scrolldown.Click += new System.EventHandler(this.bt_scrolldown_Click);
            // 
            // panel_SearchItemsClosingDay
            // 
            this.panel_SearchItemsClosingDay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel_SearchItemsClosingDay.Controls.Add(this.lb_ItemsClosingDay);
            this.panel_SearchItemsClosingDay.Location = new System.Drawing.Point(10, 513);
            this.panel_SearchItemsClosingDay.Name = "panel_SearchItemsClosingDay";
            this.panel_SearchItemsClosingDay.Size = new System.Drawing.Size(250, 125);
            this.panel_SearchItemsClosingDay.TabIndex = 20;
            this.panel_SearchItemsClosingDay.Visible = false;
            // 
            // lb_ItemsClosingDay
            // 
            this.lb_ItemsClosingDay.Appearance.Font = new System.Drawing.Font("Arial", 10F);
            this.lb_ItemsClosingDay.Appearance.Options.UseFont = true;
            this.lb_ItemsClosingDay.Location = new System.Drawing.Point(7, 10);
            this.lb_ItemsClosingDay.Name = "lb_ItemsClosingDay";
            this.lb_ItemsClosingDay.Size = new System.Drawing.Size(237, 104);
            this.lb_ItemsClosingDay.TabIndex = 62;
            this.lb_ItemsClosingDay.SelectedIndexChanged += new System.EventHandler(this.lb_ItemsClosingDay_SelectedIndexChanged);
            // 
            // lb_employee
            // 
            this.lb_employee.AutoSize = true;
            this.lb_employee.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lb_employee.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_employee.Location = new System.Drawing.Point(7, 1);
            this.lb_employee.Name = "lb_employee";
            this.lb_employee.Size = new System.Drawing.Size(92, 16);
            this.lb_employee.TabIndex = 3;
            this.lb_employee.Text = "Empregado:";
            // 
            // cb_customer
            // 
            this.cb_customer.Font = new System.Drawing.Font("Arial", 9F);
            this.cb_customer.FormattingEnabled = true;
            this.cb_customer.Location = new System.Drawing.Point(10, 18);
            this.cb_customer.Name = "cb_customer";
            this.cb_customer.Size = new System.Drawing.Size(232, 23);
            this.cb_customer.TabIndex = 4;
            // 
            // bt_searchEmployee
            // 
            this.bt_searchEmployee.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.bt_searchEmployee.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_searchEmployee.BackgroundImage")));
            this.bt_searchEmployee.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_searchEmployee.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_searchEmployee.FlatAppearance.BorderSize = 0;
            this.bt_searchEmployee.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_searchEmployee.ForeColor = System.Drawing.Color.White;
            this.bt_searchEmployee.Location = new System.Drawing.Point(199, 48);
            this.bt_searchEmployee.Name = "bt_searchEmployee";
            this.bt_searchEmployee.Padding = new System.Windows.Forms.Padding(2);
            this.bt_searchEmployee.Size = new System.Drawing.Size(43, 43);
            this.bt_searchEmployee.TabIndex = 28;
            this.bt_searchEmployee.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bt_searchEmployee.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bt_searchEmployee.UseVisualStyleBackColor = false;
            this.bt_searchEmployee.Click += new System.EventHandler(this.bt_searchEmployee_Click);
            // 
            // panel_Customers
            // 
            this.panel_Customers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel_Customers.Controls.Add(this.bt_searchEmployee);
            this.panel_Customers.Controls.Add(this.cb_customer);
            this.panel_Customers.Controls.Add(this.lb_employee);
            this.panel_Customers.Location = new System.Drawing.Point(10, 455);
            this.panel_Customers.Name = "panel_Customers";
            this.panel_Customers.Size = new System.Drawing.Size(250, 97);
            this.panel_Customers.TabIndex = 19;
            this.panel_Customers.Visible = false;
            // 
            // lb_zones
            // 
            this.lb_zones.AutoSize = true;
            this.lb_zones.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lb_zones.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_zones.Location = new System.Drawing.Point(7, 1);
            this.lb_zones.Name = "lb_zones";
            this.lb_zones.Size = new System.Drawing.Size(46, 16);
            this.lb_zones.TabIndex = 3;
            this.lb_zones.Text = "Zona:";
            // 
            // cb_zones
            // 
            this.cb_zones.BackColor = System.Drawing.SystemColors.Window;
            this.cb_zones.Font = new System.Drawing.Font("Arial", 9F);
            this.cb_zones.FormattingEnabled = true;
            this.cb_zones.Location = new System.Drawing.Point(10, 18);
            this.cb_zones.Name = "cb_zones";
            this.cb_zones.Size = new System.Drawing.Size(232, 23);
            this.cb_zones.TabIndex = 4;
            // 
            // bt_searchZones
            // 
            this.bt_searchZones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.bt_searchZones.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_searchZones.BackgroundImage")));
            this.bt_searchZones.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_searchZones.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_searchZones.FlatAppearance.BorderSize = 0;
            this.bt_searchZones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_searchZones.ForeColor = System.Drawing.Color.White;
            this.bt_searchZones.Location = new System.Drawing.Point(199, 47);
            this.bt_searchZones.Name = "bt_searchZones";
            this.bt_searchZones.Padding = new System.Windows.Forms.Padding(2);
            this.bt_searchZones.Size = new System.Drawing.Size(43, 43);
            this.bt_searchZones.TabIndex = 24;
            this.bt_searchZones.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bt_searchZones.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bt_searchZones.UseVisualStyleBackColor = false;
            this.bt_searchZones.Click += new System.EventHandler(this.bt_searchZones_Click);
            // 
            // panel_Zones
            // 
            this.panel_Zones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel_Zones.Controls.Add(this.bt_searchZones);
            this.panel_Zones.Controls.Add(this.cb_zones);
            this.panel_Zones.Controls.Add(this.lb_zones);
            this.panel_Zones.Location = new System.Drawing.Point(10, 455);
            this.panel_Zones.Name = "panel_Zones";
            this.panel_Zones.Size = new System.Drawing.Size(250, 97);
            this.panel_Zones.TabIndex = 14;
            this.panel_Zones.Visible = false;
            // 
            // lb_family
            // 
            this.lb_family.AutoSize = true;
            this.lb_family.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lb_family.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_family.Location = new System.Drawing.Point(7, 1);
            this.lb_family.Name = "lb_family";
            this.lb_family.Size = new System.Drawing.Size(61, 16);
            this.lb_family.TabIndex = 3;
            this.lb_family.Text = "Família:";
            // 
            // cb_family
            // 
            this.cb_family.Font = new System.Drawing.Font("Arial", 9F);
            this.cb_family.FormattingEnabled = true;
            this.cb_family.Location = new System.Drawing.Point(10, 18);
            this.cb_family.Name = "cb_family";
            this.cb_family.Size = new System.Drawing.Size(232, 23);
            this.cb_family.TabIndex = 4;
            // 
            // bt_searchFamily
            // 
            this.bt_searchFamily.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.bt_searchFamily.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_searchFamily.BackgroundImage")));
            this.bt_searchFamily.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_searchFamily.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_searchFamily.FlatAppearance.BorderSize = 0;
            this.bt_searchFamily.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_searchFamily.ForeColor = System.Drawing.Color.White;
            this.bt_searchFamily.Location = new System.Drawing.Point(199, 47);
            this.bt_searchFamily.Name = "bt_searchFamily";
            this.bt_searchFamily.Padding = new System.Windows.Forms.Padding(2);
            this.bt_searchFamily.Size = new System.Drawing.Size(43, 43);
            this.bt_searchFamily.TabIndex = 28;
            this.bt_searchFamily.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bt_searchFamily.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bt_searchFamily.UseVisualStyleBackColor = false;
            this.bt_searchFamily.Click += new System.EventHandler(this.bt_searchFamily_Click);
            // 
            // panel_Family
            // 
            this.panel_Family.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel_Family.Controls.Add(this.bt_searchFamily);
            this.panel_Family.Controls.Add(this.cb_family);
            this.panel_Family.Controls.Add(this.lb_family);
            this.panel_Family.Location = new System.Drawing.Point(10, 455);
            this.panel_Family.Name = "panel_Family";
            this.panel_Family.Size = new System.Drawing.Size(250, 97);
            this.panel_Family.TabIndex = 18;
            this.panel_Family.Visible = false;
            // 
            // lb_typeMovement
            // 
            this.lb_typeMovement.AutoSize = true;
            this.lb_typeMovement.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lb_typeMovement.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_typeMovement.Location = new System.Drawing.Point(7, 1);
            this.lb_typeMovement.Name = "lb_typeMovement";
            this.lb_typeMovement.Size = new System.Drawing.Size(146, 16);
            this.lb_typeMovement.TabIndex = 3;
            this.lb_typeMovement.Text = "Tipo de movimento:";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel5.Controls.Add(this.panel7);
            this.panel5.Controls.Add(this.button5);
            this.panel5.Controls.Add(this.comboBox6);
            this.panel5.Controls.Add(this.label14);
            this.panel5.Location = new System.Drawing.Point(10, 515);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(250, 84);
            this.panel5.TabIndex = 13;
            this.panel5.Visible = false;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel7.Controls.Add(this.button4);
            this.panel7.Controls.Add(this.comboBox5);
            this.panel7.Controls.Add(this.label13);
            this.panel7.Location = new System.Drawing.Point(10, 515);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(250, 84);
            this.panel7.TabIndex = 16;
            this.panel7.Visible = false;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.button4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button4.BackgroundImage")));
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button4.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(199, 36);
            this.button4.Name = "button4";
            this.button4.Padding = new System.Windows.Forms.Padding(2);
            this.button4.Size = new System.Drawing.Size(43, 43);
            this.button4.TabIndex = 25;
            this.button4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.button4.UseVisualStyleBackColor = false;
            // 
            // comboBox5
            // 
            this.comboBox5.Font = new System.Drawing.Font("Arial", 10.5F);
            this.comboBox5.FormattingEnabled = true;
            this.comboBox5.Location = new System.Drawing.Point(72, 6);
            this.comboBox5.Name = "comboBox5";
            this.comboBox5.Size = new System.Drawing.Size(170, 24);
            this.comboBox5.TabIndex = 4;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.label13.Location = new System.Drawing.Point(5, 9);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(61, 18);
            this.label13.TabIndex = 3;
            this.label13.Text = "Família:";
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.button5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button5.BackgroundImage")));
            this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button5.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button5.FlatAppearance.BorderSize = 0;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Location = new System.Drawing.Point(199, 36);
            this.button5.Name = "button5";
            this.button5.Padding = new System.Windows.Forms.Padding(2);
            this.button5.Size = new System.Drawing.Size(43, 43);
            this.button5.TabIndex = 27;
            this.button5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.button5.UseVisualStyleBackColor = false;
            // 
            // comboBox6
            // 
            this.comboBox6.Font = new System.Drawing.Font("Arial", 10.5F);
            this.comboBox6.FormattingEnabled = true;
            this.comboBox6.Location = new System.Drawing.Point(104, 6);
            this.comboBox6.Name = "comboBox6";
            this.comboBox6.Size = new System.Drawing.Size(138, 24);
            this.comboBox6.TabIndex = 4;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.label14.Location = new System.Drawing.Point(5, 9);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(93, 18);
            this.label14.TabIndex = 3;
            this.label14.Text = "Empregado:";
            // 
            // bt_typemovement
            // 
            this.bt_typemovement.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.bt_typemovement.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_typemovement.BackgroundImage")));
            this.bt_typemovement.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_typemovement.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_typemovement.FlatAppearance.BorderSize = 0;
            this.bt_typemovement.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_typemovement.ForeColor = System.Drawing.Color.White;
            this.bt_typemovement.Location = new System.Drawing.Point(199, 47);
            this.bt_typemovement.Name = "bt_typemovement";
            this.bt_typemovement.Padding = new System.Windows.Forms.Padding(2);
            this.bt_typemovement.Size = new System.Drawing.Size(43, 43);
            this.bt_typemovement.TabIndex = 28;
            this.bt_typemovement.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bt_typemovement.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bt_typemovement.UseVisualStyleBackColor = false;
            this.bt_typemovement.Click += new System.EventHandler(this.bt_typemovement_Click);
            // 
            // cb_typemovement
            // 
            this.cb_typemovement.Font = new System.Drawing.Font("Arial", 9F);
            this.cb_typemovement.FormattingEnabled = true;
            this.cb_typemovement.Location = new System.Drawing.Point(10, 18);
            this.cb_typemovement.Name = "cb_typemovement";
            this.cb_typemovement.Size = new System.Drawing.Size(234, 23);
            this.cb_typemovement.TabIndex = 29;
            // 
            // panel_Movements
            // 
            this.panel_Movements.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel_Movements.Controls.Add(this.cb_typemovement);
            this.panel_Movements.Controls.Add(this.bt_typemovement);
            this.panel_Movements.Controls.Add(this.panel5);
            this.panel_Movements.Controls.Add(this.lb_typeMovement);
            this.panel_Movements.Location = new System.Drawing.Point(10, 455);
            this.panel_Movements.Name = "panel_Movements";
            this.panel_Movements.Size = new System.Drawing.Size(250, 97);
            this.panel_Movements.TabIndex = 18;
            this.panel_Movements.Visible = false;
            // 
            // lb_zone
            // 
            this.lb_zone.AutoSize = true;
            this.lb_zone.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lb_zone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_zone.Location = new System.Drawing.Point(7, 1);
            this.lb_zone.Name = "lb_zone";
            this.lb_zone.Size = new System.Drawing.Size(46, 16);
            this.lb_zone.TabIndex = 3;
            this.lb_zone.Text = "Zona:";
            // 
            // cb_zone
            // 
            this.cb_zone.Font = new System.Drawing.Font("Arial", 9F);
            this.cb_zone.FormattingEnabled = true;
            this.cb_zone.Location = new System.Drawing.Point(10, 18);
            this.cb_zone.Name = "cb_zone";
            this.cb_zone.Size = new System.Drawing.Size(234, 23);
            this.cb_zone.TabIndex = 4;
            this.cb_zone.SelectedIndexChanged += new System.EventHandler(this.cb_zone_SelectedIndexChanged);
            // 
            // lb_table
            // 
            this.lb_table.AutoSize = true;
            this.lb_table.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lb_table.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_table.Location = new System.Drawing.Point(7, 41);
            this.lb_table.Name = "lb_table";
            this.lb_table.Size = new System.Drawing.Size(50, 16);
            this.lb_table.TabIndex = 5;
            this.lb_table.Text = "Mesa:";
            // 
            // cb_table
            // 
            this.cb_table.Font = new System.Drawing.Font("Arial", 9F);
            this.cb_table.FormattingEnabled = true;
            this.cb_table.Location = new System.Drawing.Point(10, 58);
            this.cb_table.Name = "cb_table";
            this.cb_table.Size = new System.Drawing.Size(234, 23);
            this.cb_table.TabIndex = 6;
            // 
            // bt_searchTables
            // 
            this.bt_searchTables.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.bt_searchTables.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_searchTables.BackgroundImage")));
            this.bt_searchTables.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_searchTables.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_searchTables.FlatAppearance.BorderSize = 0;
            this.bt_searchTables.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_searchTables.ForeColor = System.Drawing.Color.White;
            this.bt_searchTables.Location = new System.Drawing.Point(201, 85);
            this.bt_searchTables.Name = "bt_searchTables";
            this.bt_searchTables.Padding = new System.Windows.Forms.Padding(2);
            this.bt_searchTables.Size = new System.Drawing.Size(43, 43);
            this.bt_searchTables.TabIndex = 22;
            this.bt_searchTables.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bt_searchTables.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bt_searchTables.UseVisualStyleBackColor = false;
            this.bt_searchTables.Click += new System.EventHandler(this.bt_searchTables_Click);
            // 
            // panel_Zone_Table
            // 
            this.panel_Zone_Table.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel_Zone_Table.Controls.Add(this.bt_searchTables);
            this.panel_Zone_Table.Controls.Add(this.cb_table);
            this.panel_Zone_Table.Controls.Add(this.lb_table);
            this.panel_Zone_Table.Controls.Add(this.cb_zone);
            this.panel_Zone_Table.Controls.Add(this.lb_zone);
            this.panel_Zone_Table.Location = new System.Drawing.Point(10, 455);
            this.panel_Zone_Table.Name = "panel_Zone_Table";
            this.panel_Zone_Table.Size = new System.Drawing.Size(250, 134);
            this.panel_Zone_Table.TabIndex = 12;
            this.panel_Zone_Table.Visible = false;
            // 
            // lb_terminal
            // 
            this.lb_terminal.AutoSize = true;
            this.lb_terminal.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lb_terminal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_terminal.Location = new System.Drawing.Point(7, 1);
            this.lb_terminal.Name = "lb_terminal";
            this.lb_terminal.Size = new System.Drawing.Size(71, 16);
            this.lb_terminal.TabIndex = 3;
            this.lb_terminal.Text = "Terminal:";
            // 
            // cb_terminal
            // 
            this.cb_terminal.Font = new System.Drawing.Font("Arial", 9F);
            this.cb_terminal.FormattingEnabled = true;
            this.cb_terminal.Location = new System.Drawing.Point(10, 18);
            this.cb_terminal.Name = "cb_terminal";
            this.cb_terminal.Size = new System.Drawing.Size(232, 23);
            this.cb_terminal.TabIndex = 4;
            // 
            // bt_terminal
            // 
            this.bt_terminal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.bt_terminal.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_terminal.BackgroundImage")));
            this.bt_terminal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_terminal.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_terminal.FlatAppearance.BorderSize = 0;
            this.bt_terminal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_terminal.ForeColor = System.Drawing.Color.White;
            this.bt_terminal.Location = new System.Drawing.Point(199, 47);
            this.bt_terminal.Name = "bt_terminal";
            this.bt_terminal.Padding = new System.Windows.Forms.Padding(2);
            this.bt_terminal.Size = new System.Drawing.Size(43, 43);
            this.bt_terminal.TabIndex = 28;
            this.bt_terminal.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bt_terminal.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bt_terminal.UseVisualStyleBackColor = false;
            this.bt_terminal.Click += new System.EventHandler(this.bt_terminal_Click);
            // 
            // panel_Terminal
            // 
            this.panel_Terminal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel_Terminal.Controls.Add(this.bt_terminal);
            this.panel_Terminal.Controls.Add(this.cb_terminal);
            this.panel_Terminal.Controls.Add(this.lb_terminal);
            this.panel_Terminal.Location = new System.Drawing.Point(10, 455);
            this.panel_Terminal.Name = "panel_Terminal";
            this.panel_Terminal.Size = new System.Drawing.Size(250, 97);
            this.panel_Terminal.TabIndex = 17;
            this.panel_Terminal.Visible = false;
            // 
            // lb_typePayment
            // 
            this.lb_typePayment.AutoSize = true;
            this.lb_typePayment.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lb_typePayment.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_typePayment.Location = new System.Drawing.Point(7, 1);
            this.lb_typePayment.Name = "lb_typePayment";
            this.lb_typePayment.Size = new System.Drawing.Size(146, 16);
            this.lb_typePayment.TabIndex = 3;
            this.lb_typePayment.Text = "Tipo de pagamento:";
            // 
            // cb_typepayment
            // 
            this.cb_typepayment.Font = new System.Drawing.Font("Arial", 9F);
            this.cb_typepayment.FormattingEnabled = true;
            this.cb_typepayment.Location = new System.Drawing.Point(10, 18);
            this.cb_typepayment.Name = "cb_typepayment";
            this.cb_typepayment.Size = new System.Drawing.Size(232, 23);
            this.cb_typepayment.TabIndex = 4;
            // 
            // bt_searchtypepayment
            // 
            this.bt_searchtypepayment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.bt_searchtypepayment.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_searchtypepayment.BackgroundImage")));
            this.bt_searchtypepayment.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_searchtypepayment.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_searchtypepayment.FlatAppearance.BorderSize = 0;
            this.bt_searchtypepayment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_searchtypepayment.ForeColor = System.Drawing.Color.White;
            this.bt_searchtypepayment.Location = new System.Drawing.Point(199, 47);
            this.bt_searchtypepayment.Name = "bt_searchtypepayment";
            this.bt_searchtypepayment.Padding = new System.Windows.Forms.Padding(2);
            this.bt_searchtypepayment.Size = new System.Drawing.Size(43, 43);
            this.bt_searchtypepayment.TabIndex = 27;
            this.bt_searchtypepayment.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bt_searchtypepayment.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bt_searchtypepayment.UseVisualStyleBackColor = false;
            this.bt_searchtypepayment.Click += new System.EventHandler(this.bt_searchtypepayment_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.comboBox2);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(10, 515);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(250, 84);
            this.panel2.TabIndex = 16;
            this.panel2.Visible = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.button1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button1.BackgroundImage")));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(199, 36);
            this.button1.Name = "button1";
            this.button1.Padding = new System.Windows.Forms.Padding(2);
            this.button1.Size = new System.Drawing.Size(43, 43);
            this.button1.TabIndex = 25;
            this.button1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.button1.UseVisualStyleBackColor = false;
            // 
            // comboBox2
            // 
            this.comboBox2.Font = new System.Drawing.Font("Arial", 10.5F);
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(72, 6);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(170, 24);
            this.comboBox2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.label3.Location = new System.Drawing.Point(5, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = "Família:";
            // 
            // panel_Payment
            // 
            this.panel_Payment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel_Payment.Controls.Add(this.panel2);
            this.panel_Payment.Controls.Add(this.bt_searchtypepayment);
            this.panel_Payment.Controls.Add(this.cb_typepayment);
            this.panel_Payment.Controls.Add(this.lb_typePayment);
            this.panel_Payment.Location = new System.Drawing.Point(10, 455);
            this.panel_Payment.Name = "panel_Payment";
            this.panel_Payment.Size = new System.Drawing.Size(250, 97);
            this.panel_Payment.TabIndex = 14;
            this.panel_Payment.Visible = false;
            // 
            // lb_customer
            // 
            this.lb_customer.AutoSize = true;
            this.lb_customer.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lb_customer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_customer.Location = new System.Drawing.Point(7, 1);
            this.lb_customer.Name = "lb_customer";
            this.lb_customer.Size = new System.Drawing.Size(62, 16);
            this.lb_customer.TabIndex = 3;
            this.lb_customer.Text = "Cliente:";
            // 
            // cb_clients
            // 
            this.cb_clients.Font = new System.Drawing.Font("Arial", 9F);
            this.cb_clients.FormattingEnabled = true;
            this.cb_clients.Location = new System.Drawing.Point(10, 18);
            this.cb_clients.Name = "cb_clients";
            this.cb_clients.Size = new System.Drawing.Size(234, 23);
            this.cb_clients.TabIndex = 4;
            // 
            // lb_StatePay
            // 
            this.lb_StatePay.AutoSize = true;
            this.lb_StatePay.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lb_StatePay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.lb_StatePay.Location = new System.Drawing.Point(7, 41);
            this.lb_StatePay.Name = "lb_StatePay";
            this.lb_StatePay.Size = new System.Drawing.Size(164, 16);
            this.lb_StatePay.TabIndex = 27;
            this.lb_StatePay.Text = "Estado do pagamento:";
            // 
            // bt_client
            // 
            this.bt_client.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.bt_client.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_client.BackgroundImage")));
            this.bt_client.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_client.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_client.FlatAppearance.BorderSize = 0;
            this.bt_client.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_client.ForeColor = System.Drawing.Color.White;
            this.bt_client.Location = new System.Drawing.Point(201, 85);
            this.bt_client.Name = "bt_client";
            this.bt_client.Padding = new System.Windows.Forms.Padding(2);
            this.bt_client.Size = new System.Drawing.Size(43, 43);
            this.bt_client.TabIndex = 26;
            this.bt_client.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bt_client.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bt_client.UseVisualStyleBackColor = false;
            this.bt_client.Click += new System.EventHandler(this.bt_client_Click);
            // 
            // cb_StatePay
            // 
            this.cb_StatePay.Font = new System.Drawing.Font("Arial", 9F);
            this.cb_StatePay.FormattingEnabled = true;
            this.cb_StatePay.Location = new System.Drawing.Point(10, 58);
            this.cb_StatePay.Name = "cb_StatePay";
            this.cb_StatePay.Size = new System.Drawing.Size(234, 23);
            this.cb_StatePay.TabIndex = 28;
            // 
            // panel_Clients
            // 
            this.panel_Clients.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel_Clients.Controls.Add(this.cb_StatePay);
            this.panel_Clients.Controls.Add(this.bt_client);
            this.panel_Clients.Controls.Add(this.lb_StatePay);
            this.panel_Clients.Controls.Add(this.cb_clients);
            this.panel_Clients.Controls.Add(this.lb_customer);
            this.panel_Clients.Location = new System.Drawing.Point(10, 455);
            this.panel_Clients.Name = "panel_Clients";
            this.panel_Clients.Size = new System.Drawing.Size(250, 134);
            this.panel_Clients.TabIndex = 15;
            this.panel_Clients.Visible = false;
            // 
            // br_searchTerminalDatesClosingBox
            // 
            this.br_searchTerminalDatesClosingBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.br_searchTerminalDatesClosingBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(119)))), ((int)(((byte)(164)))));
            this.br_searchTerminalDatesClosingBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.br_searchTerminalDatesClosingBox.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.br_searchTerminalDatesClosingBox.FlatAppearance.BorderSize = 0;
            this.br_searchTerminalDatesClosingBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.br_searchTerminalDatesClosingBox.Font = new System.Drawing.Font("Arial", 9.5F, System.Drawing.FontStyle.Bold);
            this.br_searchTerminalDatesClosingBox.ForeColor = System.Drawing.Color.White;
            this.br_searchTerminalDatesClosingBox.Image = ((System.Drawing.Image)(resources.GetObject("br_searchTerminalDatesClosingBox.Image")));
            this.br_searchTerminalDatesClosingBox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.br_searchTerminalDatesClosingBox.Location = new System.Drawing.Point(7, 105);
            this.br_searchTerminalDatesClosingBox.Name = "br_searchTerminalDatesClosingBox";
            this.br_searchTerminalDatesClosingBox.Padding = new System.Windows.Forms.Padding(2);
            this.br_searchTerminalDatesClosingBox.Size = new System.Drawing.Size(237, 57);
            this.br_searchTerminalDatesClosingBox.TabIndex = 63;
            this.br_searchTerminalDatesClosingBox.Text = "Pesquisar novamente \r\npor Terminal ou Datas";
            this.br_searchTerminalDatesClosingBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.br_searchTerminalDatesClosingBox.UseVisualStyleBackColor = false;
            this.br_searchTerminalDatesClosingBox.Click += new System.EventHandler(this.br_searchTerminalDatesClosingBox_Click);
            // 
            // lb_ItemsClosingBox
            // 
            this.lb_ItemsClosingBox.Appearance.Font = new System.Drawing.Font("Arial", 10F);
            this.lb_ItemsClosingBox.Appearance.Options.UseFont = true;
            this.lb_ItemsClosingBox.Location = new System.Drawing.Point(7, 6);
            this.lb_ItemsClosingBox.Name = "lb_ItemsClosingBox";
            this.lb_ItemsClosingBox.Size = new System.Drawing.Size(237, 95);
            this.lb_ItemsClosingBox.TabIndex = 62;
            this.lb_ItemsClosingBox.SelectedIndexChanged += new System.EventHandler(this.lb_searchItemsClosingBox_SelectedIndexChanged);
            // 
            // panel_SearchItemsClosingBox
            // 
            this.panel_SearchItemsClosingBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(248)))), ((int)(((byte)(218)))));
            this.panel_SearchItemsClosingBox.Controls.Add(this.lb_ItemsClosingBox);
            this.panel_SearchItemsClosingBox.Controls.Add(this.br_searchTerminalDatesClosingBox);
            this.panel_SearchItemsClosingBox.Location = new System.Drawing.Point(10, 370);
            this.panel_SearchItemsClosingBox.Name = "panel_SearchItemsClosingBox";
            this.panel_SearchItemsClosingBox.Size = new System.Drawing.Size(250, 174);
            this.panel_SearchItemsClosingBox.TabIndex = 19;
            this.panel_SearchItemsClosingBox.Visible = false;
            // 
            // bt_stockArticles
            // 
            this.bt_stockArticles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            this.bt_stockArticles.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bt_stockArticles.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bt_stockArticles.FlatAppearance.BorderSize = 0;
            this.bt_stockArticles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_stockArticles.Font = new System.Drawing.Font("Arial", 10F);
            this.bt_stockArticles.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.bt_stockArticles.Image = ((System.Drawing.Image)(resources.GetObject("bt_stockArticles.Image")));
            this.bt_stockArticles.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_stockArticles.Location = new System.Drawing.Point(3, 576);
            this.bt_stockArticles.Name = "bt_stockArticles";
            this.bt_stockArticles.Padding = new System.Windows.Forms.Padding(2);
            this.bt_stockArticles.Size = new System.Drawing.Size(236, 31);
            this.bt_stockArticles.TabIndex = 82;
            this.bt_stockArticles.Text = "Stock de artigos";
            this.bt_stockArticles.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_stockArticles.UseVisualStyleBackColor = false;
            this.bt_stockArticles.Click += new System.EventHandler(this.bt_stockArticles_Click);
            // 
            // FormReporting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(88)))), ((int)(((byte)(88)))));
            this.ClientSize = new System.Drawing.Size(1036, 749);
            this.Controls.Add(this.manualScrollPanel1);
            this.Controls.Add(this.panel_SearchItemsClosingBox);
            this.Controls.Add(this.panel_Clients);
            this.Controls.Add(this.panel_Payment);
            this.Controls.Add(this.panel_Terminal);
            this.Controls.Add(this.panel_Zone_Table);
            this.Controls.Add(this.panel_SearchItemsClosingDay);
            this.Controls.Add(this.bt_design);
            this.Controls.Add(this.bt_scrolldown);
            this.Controls.Add(this.panel_Movements);
            this.Controls.Add(this.panel_Family);
            this.Controls.Add(this.bt_scrollup);
            this.Controls.Add(this.panel_Zones);
            this.Controls.Add(this.panel_Customers);
            this.Controls.Add(this.panel_defaultReport);
            this.Controls.Add(this.bt_close);
            this.Controls.Add(this.btnLast);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrior);
            this.Controls.Add(this.btnFirst);
            this.Controls.Add(this.btnZoomIn);
            this.Controls.Add(this.btnZoomOut);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.tbPageNo);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel_date);
            this.Controls.Add(this.previewControl1);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "FormReporting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Relatórios";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormReporting_FormClosed);
            this.Load += new System.EventHandler(this.FormReporting_Load);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel_date.ResumeLayout(false);
            this.panel_date.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.manualScrollPanel1.ResumeLayout(false);
            this.manualScrollPanel2.ResumeLayout(false);
            this.panel_SearchItemsClosingDay.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lb_ItemsClosingDay)).EndInit();
            this.panel_Customers.ResumeLayout(false);
            this.panel_Customers.PerformLayout();
            this.panel_Zones.ResumeLayout(false);
            this.panel_Zones.PerformLayout();
            this.panel_Family.ResumeLayout(false);
            this.panel_Family.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel_Movements.ResumeLayout(false);
            this.panel_Movements.PerformLayout();
            this.panel_Zone_Table.ResumeLayout(false);
            this.panel_Zone_Table.PerformLayout();
            this.panel_Terminal.ResumeLayout(false);
            this.panel_Terminal.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel_Payment.ResumeLayout(false);
            this.panel_Payment.PerformLayout();
            this.panel_Clients.ResumeLayout(false);
            this.panel_Clients.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lb_ItemsClosingBox)).EndInit();
            this.panel_SearchItemsClosingBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.ComboBox cb_customers;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel_date;
        private System.Windows.Forms.Label lb_startDate;
        private System.Windows.Forms.DateTimePicker dt_startDate;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button bt_design;
        private System.Windows.Forms.Button bt_searchCustomers;
        private System.Windows.Forms.TextBox tbPageNo;
        private System.Windows.Forms.ContextMenuStrip exportMenu;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.Button btnPrior;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnLast;
        private FastReport.Preview.PreviewControl previewControl1;
        private System.Windows.Forms.Button bt_close;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button bt_searchtypemovement;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lb_endDate;
        private System.Windows.Forms.DateTimePicker dt_endDate;
        private System.Windows.Forms.Panel panel_defaultReport;
        private Button bt_scrollup;
        private Button bt_scrolldown;
        private Button bt_totalday;
        private Button bt_totalzonetable;
        private Button bt_totalfamily;
        private Button bt_totalzone;
        private Button bt_totalterminal;
        private Button bt_totalCash;
        private Button bt_totalCustomer;
        private Button bt_movCustomers;
        private Button bt_recordsCustomer;
        private Button bt_listArticles;
        private Button bt_occupationPlace;
        private Button bt_searchDate;
        private Button bt_closingBox;
        private Button bt_closingDay;
        private Panel panel_SearchItemsClosingDay;
        private DevExpress.XtraEditors.ListBoxControl lb_ItemsClosingDay;
        private Button bt_CurrentAccount;
        private Button bt_DetailsCurrentAccount;
        private FlowLayoutPanel manualScrollPanel2;
        private Panel manualScrollPanel1;
        private Label lb_employee;
        private ComboBox cb_customer;
        private Button bt_searchEmployee;
        private Panel panel_Customers;
        private Label lb_zones;
        private ComboBox cb_zones;
        private Button bt_searchZones;
        private Panel panel_Zones;
        private Label lb_family;
        private ComboBox cb_family;
        private Button bt_searchFamily;
        private Panel panel_Family;
        private Label lb_typeMovement;
        private Panel panel5;
        private Panel panel7;
        private Button button4;
        private ComboBox comboBox5;
        private Label label13;
        private Button button5;
        private ComboBox comboBox6;
        private Label label14;
        private Button bt_typemovement;
        private ComboBox cb_typemovement;
        private Panel panel_Movements;
        private Label lb_zone;
        private ComboBox cb_zone;
        private Label lb_table;
        private ComboBox cb_table;
        private Button bt_searchTables;
        private Panel panel_Zone_Table;
        private Label lb_terminal;
        private ComboBox cb_terminal;
        private Button bt_terminal;
        private Panel panel_Terminal;
        private Label lb_typePayment;
        private ComboBox cb_typepayment;
        private Button bt_searchtypepayment;
        private Panel panel2;
        private Button button1;
        private ComboBox comboBox2;
        private Label label3;
        private Panel panel_Payment;
        private Label lb_customer;
        private ComboBox cb_clients;
        private Label lb_StatePay;
        private Button bt_client;
        private ComboBox cb_StatePay;
        private Panel panel_Clients;
        private Button br_searchTerminalDatesClosingBox;
        private DevExpress.XtraEditors.ListBoxControl lb_ItemsClosingBox;
        private Panel panel_SearchItemsClosingBox;
        private Button bt_stockArticles;

    }
}