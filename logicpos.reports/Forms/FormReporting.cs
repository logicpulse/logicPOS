using FastReport;
using FastReport.Export;
using FastReport.Utils;
using logicpos.reports.App;
using logicpos.reports.Forms;
using logicpos.reports.Resources.Localization;
using logicpos.reports.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace logicpos.reports
{
    public partial class FormReporting : Form
    {
        private DataSet FDataSet = new DataSet();
        private DataTable terminals = new DataTable();
        private Report FReport = new Report();
        private string FReportsFolder;
        private bool FReportRunning;
        private string thisFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));
        private string report;
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _startDate;
        private string _endDate;
        private string _startDateFormSelectTerminal;
        private string _endDateFormSelectTerminal;
        private bool _status;
        private List<string> dates = new List<string>();
        private DataTable tableTerminals = new DataTable();
        private string fileReport;
        private bool bMessageNotFound = false;

        private void CreateDataSources()
        {
            try
            {
                DataReportsXML.GetAllData();
                FReportsFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));
                FDataSet.ReadXml(FReportsFolder + "sales.xml");
                _log.Debug("Read xml " + FDataSet);
            }
            catch (Exception exx)
            {
                _log.Error("CreateDataSources:" + exx.Message, exx);
            }
        }

        private void DesignReport()
        {
            try
            {
                if (FReportRunning)
                    return;

                if (report == null)
                {
                    FormMessageInfDraw frmMessageInfDraw = new FormMessageInfDraw();
                    frmMessageInfDraw.ShowDialog();

                }
                else
                {
                    CreateDataSources();
                    FReport.Load(report);
                    FReport.RegisterData(FDataSet, "NewDataSet");
                    FReport.Design();
                    PreviewReport();
                }
            }
            catch (Exception exx)
            {
                _log.Error("DesignReport:" + exx.Message, exx);
            }
        }


        private void PreviewReport()
        {
            try
            {
                if (FReportRunning)
                    return;

                FReportRunning = true;
                try
                {
                    _log.Debug("FReportRunning is true ");

                    FReport.RegisterData(FDataSet, "NewDataSet");

                    FReport.Show();
                }
                finally
                {
                    FReportRunning = false;

                    _log.Debug("FReportRunning is false ");
                }
            }
            catch (Exception exx)
            {
                _log.Error("CreateDataSources:" + exx.Message, exx);
            }
        }

        private void btnDesign_Click(object sender, EventArgs e)
        {
            try
            {
                DesignReport();
            }
            catch (Exception exx)
            {
                _log.Error("CreateDataSources:" + exx.Message, exx);
            }
            _log.Debug("DesignReport");
        }


        private void FormReporting_Load(object sender, EventArgs e)
        {
            try
            {
                previewControl1.ZoomIn();

                Config.ReportSettings.StartProgress += new EventHandler(ReportSettings_StartProgress);
                Config.ReportSettings.Progress += new ProgressEventHandler(ReportSettings_Progress);
                Config.ReportSettings.FinishProgress += new EventHandler(ReportSettings_FinishProgress);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("FormReporting_Load(e.Message): {0}", ex.Message), ex);

            }
        }

        private void ReportSettings_StartProgress(object sender, EventArgs e)
        {
        }

        private void ReportSettings_Progress(object sender, ProgressEventArgs e)
        {
            try
            {
                previewControl1.ShowStatus(e.Message);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("previewControl1.ShowStatus(e.Message): {0}", ex.Message), ex);

            }
        }

        private void ReportSettings_FinishProgress(object sender, EventArgs e)
        {
        }

        public FormReporting()
        {
            _log.Debug("Show BackOfficeReportWindow");
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

                InitializeComponent();

                _log.Debug("InitializeComponent BackOfficeReportWindow");

                panel_date.Visible = false;
                panel_Zone_Table.Visible = false;
                panel_Customers.Visible = false;
                panel_Clients.Visible = false;
                panel_Family.Visible = false;
                panel_Terminal.Visible = false;
                panel_Zones.Visible = false;
                panel_Payment.Visible = false;
                panel_Movements.Visible = false;
                panel_SearchItemsClosingBox.Visible = false;
                panel_SearchItemsClosingDay.Visible = false;

               DataReportsXML.GetParametersReport();
               
              //  Data.GetParameters();

                StatesPay();

                GetZones();

                GetAllZones();

                _log.Debug("GetAllZones");

                GetTables_Zones(cb_zone.Text);

                _log.Debug("GetTables_Zones");

                GetAllFamilyArticle();

                _log.Debug("GetAllFamilyArticle");

                GetAllTerminal();

                _log.Debug("GetAllTerminal");

                GetAllTypePayment();

                _log.Debug("GetAllTypePayment");

                GetAllTypeMovement();

                _log.Debug("GetAllTypeMovement");

                GetAllClients();

                GetAllCustomer_Records();

                GetAllCustomer_Movements();

                GetAllCustomer_Totals();

           //     LocalizationReports.Localization_Reports();

                lb_customer.Text = Resx.label_client;
                lb_employee.Text = Resx.label_customer;
                lb_endDate.Text = Resx.label_EndDate;
                lb_family.Text = Resx.label_family;
                lb_startDate.Text = Resx.label_StartDate;
                lb_table.Text = Resx.label_table;
                lb_terminal.Text = Resx.label_terminal;
                lb_typeMovement.Text = Resx.label_typeMovement;
                lb_typePayment.Text = Resx.label_TypePayment;
                lb_zone.Text = Resx.label_zone;
                lb_zones.Text = Resx.label_zones;
                bt_totalday.Text = Resx.button_totalday;
                bt_totalzonetable.Text = Resx.button_totalzonetable;
                bt_totalfamily.Text = Resx.button_totalfamily;
                bt_totalzone.Text = Resx.button_totalzone;
                bt_totalterminal.Text = Resx.button_totalterminal;
                bt_totalCash.Text = Resx.button_totalCash;
                bt_totalCustomer.Text = Resx.button_totalEmployee;
                bt_movCustomers.Text = Resx.button_movCustomers;
                bt_recordsCustomer.Text = Resx.button_recordsCustomer;
                bt_occupationPlace.Text = Resx.button_occupationPlace;
                bt_listArticles.Text = Resx.button_listArticles;
                bt_closingBox.Text = Resx.button_closingBox;
                bt_closingDay.Text = Resx.button_closingDay;
                br_searchTerminalDatesClosingBox.Text = Resx.searchTerminalDates;
                bt_CurrentAccount.Text = Resx.bt_CurrentAccount;
                bt_DetailsCurrentAccount.Text = Resx.bt_DetailsCurrentAccount;
                lb_customer.Text = Resx.label_customer;
                lb_StatePay.Text = Resx.lb_StatePay;
                bt_design.Text = Resx.button_design;

                panel_defaultReport.Visible = true;

                DataReportsXML.ConfigureXMLReports();

                DataReportsXML.GetCurrency();

                _log.Debug("ConfigureXMLReports");

                FReport.RegisterData(FDataSet, "NewDataSet");

                _log.Debug("RegisterData " + FDataSet);

                FReport.Preview = previewControl1;

                _log.Debug("Preview report" + FReport.Preview);

                Config.ReportSettings.ShowPerformance = true;

                _log.Debug("ShowPerformance is true");

                FrameworkUtils.HideWaitingCursor();
                _log.Debug("After HideWaitingCursor");
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("GetTerminal(e.Message): {0}", ex.Message), ex);
            }
            _log.Debug("FormReporting end");
        }

        private void StatesPay()
        {
            try
            {
                cb_StatePay.Items.Add(Resx.All);
                cb_StatePay.Items.Add(Resx.Paid);
                cb_StatePay.Items.Add(Resx.NotPaid);


                cb_StatePay.SelectedIndex = 0;
            }
            catch (Exception exx)
            {
                _log.Error("StatesPay:" + exx.Message, exx);
            }
        }


        private void ItemsSearchClosingBox()
        {
            try
            {
                if (lb_ItemsClosingBox.Items.Count == 0)
                {
                    lb_ItemsClosingBox.Items.Add(Resx.Family);
                    lb_ItemsClosingBox.Items.Add(Resx.Subfamily);
                    lb_ItemsClosingBox.Items.Add(Resx.Article);
                    lb_ItemsClosingBox.Items.Add(Resx.VATRate);
                    lb_ItemsClosingBox.Items.Add(Resx.PaymentMethod);

                }
            }
            catch (Exception exx)
            {
                _log.Error("ItemsSearchClosingBox:" + exx.Message, exx);
            }
        }

        private void ItemsSearchClosingDay()
        {
            try
            {
                if (lb_ItemsClosingDay.Items.Count == 0)
                {
                    lb_ItemsClosingDay.Items.Add(Resx.Family);
                    lb_ItemsClosingDay.Items.Add(Resx.Subfamily);
                    lb_ItemsClosingDay.Items.Add(Resx.Article);
                    lb_ItemsClosingDay.Items.Add(Resx.VATRate);
                    lb_ItemsClosingDay.Items.Add(Resx.PaymentMethod);

                }
            }
            catch (Exception exx)
            {
                _log.Error("ItemsSearchClosingDay:" + exx.Message, exx);
            }
        }


        private void GetZones()
        {
            try
            {
                DataTable dt = new DataTable("dataTable");

                dt = Data.GetZones();

                cb_zones.DataSource = dt;
                cb_zones.DisplayMember = "designation";
                cb_zones.ValueMember = "designation";
            }
            catch (Exception exx)
            {
                _log.Error("GetZones:" + exx.Message, exx);
            }
        }



        private void cb_zone_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_zone.Text.Equals(Resx.All))
                {
                    cb_table.DataSource = null;
                    cb_table.Items.Add(Resx.All);
                    cb_table.SelectedIndex = 0;

                    
                }
                else
                {
                    GetTables_Zones(cb_zone.Text);
                }
            }
            catch (Exception exx)
            {
                _log.Error("cb_zone_SelectedIndexChanged:" + exx.Message, exx);
            }
        }

        private void GetAllTypePayment()
        {
            try
            {
                DataTable dt = new DataTable("dataTable");

                dt = Data.GetPayment();

                cb_typepayment.DataSource = dt;
                cb_typepayment.DisplayMember = "name";
                cb_typepayment.ValueMember = "name";
            }
            catch (Exception exx)
            {
                _log.Error("GetAllTypePayment:" + exx.Message, exx);
            }
        }


        private void GetAllTypeMovement()
        {
            try
            {
                DataTable dt = new DataTable("dataTable");

                dt = Data.GetMovements();

                cb_typemovement.DataSource = dt;
                cb_typemovement.DisplayMember = "name";
                cb_typemovement.ValueMember = "name";
            }
            catch (Exception exx)
            {
                _log.Error("GetAllTypeMovement:" + exx.Message, exx);
            }
        }

        private void GetAllClients()
        {
            try
            {
                DataTable dt = new DataTable("dataTable");

                dt = Data.GetClients();

                cb_clients.DataSource = dt;
                cb_clients.DisplayMember = "name";
                cb_clients.ValueMember = "name";
            }
            catch (Exception exx)
            {
                _log.Error("GetAllClients:" + exx.Message, exx);
            }
        }

        private void GetAllCustomer_Movements()
        {
            try
            {
                DataTable dt = new DataTable("dataTable");

                dt = Data.GetCustomer();

                cb_customer.DataSource = dt;
                cb_customer.DisplayMember = "name";
                cb_customer.ValueMember = "name";
            }
            catch (Exception exx)
            {
                _log.Error("GetAllCustomer_Movements:" + exx.Message, exx);
            }
        }


        private void GetAllCustomer_Totals()
        {
            try
            {
                DataTable dt = new DataTable("dataTable");

                dt = Data.GetTotalsCustomer();

                cb_customer.DataSource = dt;
                cb_customer.DisplayMember = "name";
                cb_customer.ValueMember = "name";
            }
            catch (Exception exx)
            {
                _log.Error("GetAllCustomer_Totals:" + exx.Message, exx);
            }
        }

        private void GetAllCustomer_Records()
        {
            try
            {
                DataTable dt = new DataTable("dataTable");

                dt = Data.GetCustomerRecords();

                cb_customer.DataSource = dt;
                cb_customer.DisplayMember = "name";
                cb_customer.ValueMember = "name";
            }
            catch (Exception exx)
            {
                _log.Error("GetAllCustomer_Records:" + exx.Message, exx);
            }
        }

        private void GetAllTerminal()
        {
            try
            {
                DataTable dt = new DataTable("dataTable");

                dt = Data.GetTerminal();

                cb_terminal.DataSource = dt;
                cb_terminal.DisplayMember = "designation";
                cb_terminal.ValueMember = "designation";
            }
            catch (Exception exx)
            {
                _log.Error("GetAllTerminal:" + exx.Message, exx);
            }
        }

        private void GetAllZones()
        {
            try
            {
                DataTable dt = new DataTable("dataTable");

                dt = Data.GetAllTablesZones();

                cb_zone.DataSource = dt;
                cb_zone.DisplayMember = "designation";
                cb_zone.ValueMember = "designation";
            }
            catch (Exception exx)
            {
                _log.Error("GetAllZones:" + exx.Message, exx);
            }
        }

        private void GetAllFamilyArticle()
        {
            try
            {
                DataTable dt = new DataTable("dataTable");

                dt = Data.GetAllFamilyArticles();

                cb_family.DataSource = dt;
                cb_family.DisplayMember = "designation";
                cb_family.ValueMember = "designation";
            }
            catch (Exception exx)
            {
                _log.Error("GetAllFamilyArticle:" + exx.Message, exx);
            }
        }

        private void GetFamilyArticle()
        {
            try
            {
                DataTable dt = new DataTable("dataTable");

                dt = Data.GetFamilyArticles();

                cb_family.DataSource = dt;
                cb_family.DisplayMember = "designation";
                cb_family.ValueMember = "designation";
            }
            catch (Exception exx)
            {
                _log.Error("GetFamilyArticle:" + exx.Message, exx);
            }
        }

        private void GetTables_Zones(string _zone)
        {
            try
            {
                if (_zone.Equals(Resx.All))
                {
                    cb_table.SelectedIndex = 0;
                }
                else
                {
                    DataTable dt = new DataTable("dataTable");
                    dt = Data.GetAllTables(_zone);

                    if (dt.Rows.Count != 0)
                    {
                        cb_table.DataSource = dt;
                        cb_table.DisplayMember = "designation";
                        cb_table.ValueMember = "designation";
                        cb_table.SelectedIndex = 0;
                    }
                    else
                    {
                        cb_table.Items.Add(Resx.All);
                        cb_table.SelectedIndex = 0;
                    }
                }


            }
            catch (Exception exx)
            {
                _log.Error("GetFamilyArticle:" + exx.Message, exx);
            }
        }

        //private List<string> DateFormat(string startDate, string endDate)
        //{
        //    List<string> dates = new List<string>();

        //    startDate = _startDateFormSelectTerminal;
        //    endDate = _endDateFormSelectTerminal;

        //    string[] splitStartDate = startDate.Split(' ');
        //    string[] splitEndDate = endDate.Split(' ');

        //    dates.Add(splitStartDate[0]);
        //    dates.Add(splitEndDate[0]);

        //    return dates;

        //}

        private void SearchItemsClosingBox(string _itemSelected, string startDate, string endDate)
        {
            try
            {
                Data.DatesReport(startDate, endDate, FReport.FileName);

                try
                {
                    fileReport = "Fecho_caixa.frx";
                    thisFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));
                    report = thisFolder + fileReport;

                    FDataSet.Clear();
                    FDataSet = DataReportsXML.GetClosingBox_Day(startDate, endDate, tableTerminals, _itemSelected);

                    DataReportsXML.ChangeItemsClosingBox(FDataSet, fileReport, _itemSelected);

                    FReport.Load(report);
                    FReport.RegisterData(FDataSet, "NewDataSet");

                    if (FDataSet.Tables["view_worksessionmovementresume"] == null || FDataSet.Tables["view_worksessionmovementresume"].Rows.Count.Equals(0))
                    {
                        panel_date.Visible = false;
                        panel_Zone_Table.Visible = false;
                        panel_Customers.Visible = false;
                        panel_Clients.Visible = false;
                        panel_Family.Visible = false;
                        panel_Terminal.Visible = false;
                        panel_Zones.Visible = false;
                        panel_Payment.Visible = false;
                        panel_Movements.Visible = false;
                        panel_SearchItemsClosingBox.Visible = true;
                        panel_SearchItemsClosingDay.Visible = false;

                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();

                    }
                    else
                    {
                        FReport.Prepare();
                        FReport.ShowPrepared();

                        panel_date.Visible = false;
                        panel_Zone_Table.Visible = false;
                        panel_Customers.Visible = false;
                        panel_Clients.Visible = false;
                        panel_Family.Visible = false;
                        panel_Terminal.Visible = false;
                        panel_Zones.Visible = false;
                        panel_Payment.Visible = false;
                        panel_Movements.Visible = false;
                        panel_defaultReport.Visible = false;
                        panel_SearchItemsClosingBox.Visible = true;
                        panel_SearchItemsClosingDay.Visible = false;
                    }

                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("SearchItemsClosingBox 2", ex.Message), ex);

                }
            }
            catch (Exception exx)
            {
                _log.Error("SearchItemsClosingBox 1:" + exx.Message, exx);
            }

        }

        private void SearchItemsClosingDay(string _itemSelected, string startDate, string endDate)
        {
            try
            {
                thisFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));

                Data.DatesReport(startDate, endDate, FReport.FileName);

                panel_defaultReport.Visible = false;

                if (FReport.FileName != "")
                {
                    FReport.Load(FReport.FileName);
                }

                try
                {
                    fileReport = "Fecho_dia.frx";

                    report = thisFolder + fileReport;

                    terminals = Data.GetTerminal_MovementResume();
                    FDataSet.Clear();
                    FDataSet = DataReportsXML.GetClosingBox_Day(startDate, endDate, terminals, _itemSelected);

                    DataReportsXML.ChangeItemsClosingBox(FDataSet, fileReport, _itemSelected);

                    panel_date.Visible = true;
                    panel_Zone_Table.Visible = false;
                    panel_Customers.Visible = false;
                    panel_Clients.Visible = false;
                    panel_Family.Visible = false;
                    panel_Terminal.Visible = false;
                    panel_Zones.Visible = false;
                    panel_Payment.Visible = false;
                    panel_Movements.Visible = false;
                    panel_SearchItemsClosingBox.Visible = false;
                    panel_SearchItemsClosingDay.Visible = true;

                    FReport.Load(report);
                    FReport.RegisterData(FDataSet, "NewDataSet");

                    if (FDataSet.Tables == null || FDataSet.Tables["view_worksessionmovementresume"] == null || FDataSet.Tables["view_worksessionmovementresume"].Rows.Count == 0)
                    {
                        if (bMessageNotFound == false)
                        {
                            panel_defaultReport.Visible = true;

                            FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                            frmMessageNotFound.ShowDialog();
                            bMessageNotFound = true;
                        }

                    }
                    else
                    {
                        panel_defaultReport.Visible = false;

                        FReport.Prepare();
                        FReport.ShowPrepared();
                    }

                }

                catch (Exception ex)
                {
                    _log.Error(string.Format("SearchItemsClosingDay 2: {0}", ex.Message), ex);

                }
            }
            catch (Exception exx)
            {
                _log.Error("SearchItemsClosingDay 1:" + exx.Message, exx);
            }
        }


        private void SelectTablesZones(string _report)
        {
            try
            {
                _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
                _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);


                Data.DatesReport(_startDate, _endDate, FReport.FileName);

                if (FReport.FileName != "")
                {
                    FReport.Load(FReport.FileName);
                }

                if (_report.Equals("Total_mesas"))
                {
                    report = thisFolder + "Total_mesas.frx";
                }
                else
                {
                    report = thisFolder + "Top_média_ocupação.frx";
                }

                panel_date.Visible = true;
                panel_Zone_Table.Visible = true;
                panel_Customers.Visible = false;
                panel_Clients.Visible = false;
                panel_Family.Visible = false;
                panel_Terminal.Visible = false;
                panel_Zones.Visible = false;
                panel_Payment.Visible = false;
                panel_Movements.Visible = false;
                panel_defaultReport.Visible = false;
                panel_SearchItemsClosingBox.Visible = false;
                panel_SearchItemsClosingDay.Visible = false;

                if (cb_zone.Text.Equals(Resx.All))
                {
                    FDataSet.Clear();

                    FDataSet = DataReportsXML.GetData_TotalZones(_startDate, _endDate);


                    if (FDataSet.Tables["documentfinancemaster"] == null || FDataSet.Tables["documentfinancemaster"].Rows.Count.Equals(0))
                    {

                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();
                    }
                    else
                    {
                        panel_defaultReport.Visible = false;

                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        FReport.Prepare();
                        FReport.ShowPrepared();

                    }
                }
                else
                {
                    FDataSet.Clear();
                    FDataSet = Data.GetTablesByDesignation(cb_table.Text, cb_zone.Text, _startDate, _endDate);

                    if (FDataSet.Tables["documentfinancemaster"] == null || FDataSet.Tables["documentfinancemaster"].Rows.Count.Equals(0))
                    {
                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();
                    }
                    else
                    {
                        panel_defaultReport.Visible = false;

                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        FReport.Prepare();
                        FReport.ShowPrepared();
                    }

                }
            }
            catch (Exception exx)
            {
                _log.Error("SelectTablesZones: " + exx.Message, exx);
            }
        }

        private void SelectFamily(string _report)
        {
            try
            {
                _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
                _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);


                Data.DatesReport(_startDate, _endDate, FReport.FileName);

                if (FReport.FileName != "")
                {
                    FReport.Load(FReport.FileName);
                }

                if (_report.Equals("ListArticles") || _report.Equals("Reports/Lista_artigos.frx"))
                {
                    panel_date.Visible = false;
                    panel_Zone_Table.Visible = false;
                    panel_Customers.Visible = false;
                    panel_Clients.Visible = false;
                    panel_Family.Visible = true;
                    panel_Family.Location = new System.Drawing.Point(10, 371);
                    panel_Terminal.Visible = false;
                    panel_Zones.Visible = false;
                    panel_Payment.Visible = false;
                    panel_Movements.Visible = false;
                    panel_SearchItemsClosingBox.Visible = false;
                    panel_SearchItemsClosingDay.Visible = false;

                    report = thisFolder + "Lista_artigos.frx";
                }
                else
                {
                    panel_date.Visible = true;
                    panel_Zone_Table.Visible = false;
                    panel_Customers.Visible = false;
                    panel_Clients.Visible = false;
                    panel_Family.Visible = true;
                    panel_Family.Location = new System.Drawing.Point(10, 455);
                    panel_Terminal.Visible = false;
                    panel_Zones.Visible = false;
                    panel_Payment.Visible = false;
                    panel_Movements.Visible = false;
                    panel_SearchItemsClosingBox.Visible = false;
                    panel_SearchItemsClosingDay.Visible = false;

                    report = thisFolder + "Total_família.frx";
                }


                if (cb_family.Text.Equals(Resx.All))
                {
                    FDataSet.Clear();
                    if (report.Equals("Reports/Lista_artigos.frx"))
                    {
                        FDataSet = DataReportsXML.GetData_ListArticles();
                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        FReport.Prepare();
                        FReport.ShowPrepared();
                    }
                    else
                    {

                        FDataSet = DataReportsXML.GetData_TotalFamily(_startDate, _endDate);

                        if (FDataSet.Tables["documentfinancedetail"] == null || FDataSet.Tables["documentfinancedetail"].Rows.Count.Equals(0))
                        {
                            panel_defaultReport.Visible = true;

                            FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                            frmMessageNotFound.ShowDialog();

                        }
                        else
                        {
                            panel_defaultReport.Visible = false;

                            FReport.Load(report);
                            FReport.RegisterData(FDataSet, "NewDataSet");
                            FReport.Prepare();
                            FReport.ShowPrepared();
                        }
                    }


                }
                else
                {
                    if (report.Equals("Reports/Lista_artigos.frx"))
                    {
                        FDataSet.Clear();
                        FDataSet = Data.GetArticles(cb_family.Text);
                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        FReport.Prepare();
                        FReport.ShowPrepared();

                    }
                    else
                    {
                        FDataSet.Clear();
                        FDataSet = Data.GetArticleFamilyByDesignation(cb_family.Text, _startDate, _endDate);
                        if (FDataSet.Tables["documentfinancedetail"] == null || FDataSet.Tables["documentfinancedetail"].Rows.Count.Equals(0))
                        {
                            panel_defaultReport.Visible = true;

                            FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                            frmMessageNotFound.ShowDialog();
                        }
                        else
                        {
                            panel_defaultReport.Visible = false;

                            FReport.Load(report);
                            FReport.RegisterData(FDataSet, "NewDataSet");
                            FReport.Prepare();
                            FReport.ShowPrepared();
                        }

                    }


                }
            }
            catch (Exception exx)
            {
                _log.Error("SelectFamily: " + exx.Message, exx);
            }
        }

        private void SelectTerminal()
        {
            try
            {
                _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
                _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);

                panel_date.Visible = true;
                panel_Zone_Table.Visible = false;
                panel_Customers.Visible = false;
                panel_Clients.Visible = false;
                panel_Family.Visible = false;
                panel_Terminal.Visible = true;
                panel_Zones.Visible = false;
                panel_Payment.Visible = false;
                panel_Movements.Visible = false;
                panel_SearchItemsClosingBox.Visible = false;
                panel_SearchItemsClosingDay.Visible = false;

                report = thisFolder + "Total_terminal.frx";

                if (cb_terminal.Text.Equals(Resx.All))
                {
                    FDataSet.Clear();
                    FDataSet = DataReportsXML.GetData_TotalTerminal(_startDate, _endDate);
                    if (FDataSet.Tables["documentfinancemaster"] == null || FDataSet.Tables["documentfinancemaster"].Rows.Count.Equals(0))
                    {
                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();
                    }
                    else
                    {
                        panel_defaultReport.Visible = false;

                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        FReport.Prepare();
                        FReport.ShowPrepared();
                    }
                }
                else
                {
                    FDataSet.Clear();

                    FDataSet = Data.GetTerminalByDesignation(cb_terminal.Text, _startDate, _endDate);
                    if (FDataSet.Tables["documentfinancemaster"] == null || FDataSet.Tables["documentfinancemaster"].Rows.Count.Equals(0))
                    {
                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();
                    }
                    else
                    {
                        panel_defaultReport.Visible = false;

                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        FReport.Prepare();
                        FReport.ShowPrepared();
                    }
                }
            }
            catch (Exception exx)
            {
                _log.Error("SelectTerminal: " + exx.Message, exx);
            }
        }


        private void SelectClients(string _report)
        {
            try
            {
                _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
                _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);


                string fileReport_CurrentAccount = "Reports/Conta_corrente_clientes.frx";

                if (FReport.FileName != "")
                {
                    FReport.Load(FReport.FileName);
                }

                if (_report.Equals("Conta_corrente_clientes") || _report.Equals(fileReport_CurrentAccount))
                {
                    report = thisFolder + "Conta_corrente_clientes.frx";
                }
                else
                {
                    report = thisFolder + "Conta_corrente_clientes_Detalhado.frx";
                }

                Data.DatesReport(_startDate, _endDate, report);

                panel_date.Visible = true;
                panel_Zone_Table.Visible = false;
                panel_Customers.Visible = false;
                panel_Clients.Visible = true;
                panel_Family.Visible = false;
                panel_Terminal.Visible = false;
                panel_Zones.Visible = false;
                panel_Payment.Visible = false;
                panel_Movements.Visible = false;
                panel_SearchItemsClosingBox.Visible = false;
                panel_SearchItemsClosingDay.Visible = false;


                if (cb_clients.Text.Equals(Resx.All))
                {
                    FDataSet.Clear();

                    FDataSet = DataReportsXML.GetData_TotalClients((string)cb_StatePay.SelectedItem, _startDate, _endDate, report);

                    if (report.Equals(fileReport_CurrentAccount))
                    {


                        if (FDataSet.Tables["documentfinancemaster"] == null || FDataSet.Tables["documentfinancemaster"].Rows.Count.Equals(0))
                        {
                            panel_defaultReport.Visible = true;

                            FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                            frmMessageNotFound.ShowDialog();
                        }
                        else
                        {
                            panel_defaultReport.Visible = false;

                            FReport.Load(report);
                            FReport.RegisterData(FDataSet, "NewDataSet");
                            FReport.Prepare();
                            FReport.ShowPrepared();
                        }
                    }
                    else
                    {
                        if (FDataSet.Tables["documentfinancedetail"] == null || FDataSet.Tables["documentfinancedetail"].Rows.Count.Equals(0))
                        {
                            panel_defaultReport.Visible = true;

                            FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                            frmMessageNotFound.ShowDialog();
                        }
                        else
                        {
                            panel_defaultReport.Visible = false;

                            FReport.Load(report);
                            FReport.RegisterData(FDataSet, "NewDataSet");
                            FReport.Prepare();
                            FReport.ShowPrepared();
                        }

                    }
                }
                else
                {
                    FDataSet.Clear();
                    FDataSet = Data.GetClientsByName(cb_clients.Text, (string)cb_StatePay.SelectedItem, _startDate, _endDate, report);

                    if (report.Equals(fileReport_CurrentAccount))
                    {

                        if (FDataSet.Tables["documentfinancemaster"] == null || FDataSet.Tables["documentfinancemaster"].Rows.Count.Equals(0))
                        {
                            panel_defaultReport.Visible = true;

                            FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                            frmMessageNotFound.ShowDialog();
                        }
                        else
                        {
                            panel_defaultReport.Visible = false;

                            FReport.Load(report);
                            FReport.RegisterData(FDataSet, "NewDataSet");
                            FReport.Prepare();
                            FReport.ShowPrepared();
                        }
                    }
                    else
                    {
                        if (FDataSet.Tables["documentfinancedetail"] == null || FDataSet.Tables["documentfinancedetail"].Rows.Count.Equals(0))
                        {
                            panel_defaultReport.Visible = true;

                            FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                            frmMessageNotFound.ShowDialog();
                        }
                        else
                        {
                            panel_defaultReport.Visible = false;

                            FReport.Load(report);
                            FReport.RegisterData(FDataSet, "NewDataSet");
                            FReport.Prepare();
                            FReport.ShowPrepared();
                        }

                    }
                }
            }
            catch (Exception exx)
            {
                _log.Error("SelectClients: " + exx.Message, exx);
            }
        }

        private void SelectZone()
        {
            try
            {
                _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
                _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);


                Data.DatesReport(_startDate, _endDate, FReport.FileName);

                if (FReport.FileName != "")
                {
                    FReport.Load(FReport.FileName);
                }

                panel_date.Visible = true;
                panel_Zone_Table.Visible = false;
                panel_Customers.Visible = false;
                panel_Clients.Visible = false;
                panel_Family.Visible = false;
                panel_Terminal.Visible = false;
                panel_Zones.Visible = true;
                panel_Payment.Visible = false;
                panel_Movements.Visible = false;
                panel_SearchItemsClosingBox.Visible = false;
                panel_SearchItemsClosingDay.Visible = false;

                report = thisFolder + "Total_zonas.frx";

                if (cb_zones.Text.Equals(Resx.All))
                {
                    FDataSet.Clear();
                    FDataSet = DataReportsXML.GetData_TotalZones(_startDate, _endDate);
                    if (FDataSet.Tables["documentfinancemaster"] == null || FDataSet.Tables["documentfinancemaster"].Rows.Count.Equals(0))
                    {
                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();
                    }
                    else
                    {
                        panel_defaultReport.Visible = false;

                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        FReport.Prepare();
                        FReport.ShowPrepared();

                    }
                }
                else
                {
                    FDataSet.Clear();
                    FDataSet = Data.GetTablesByDesignation(null, cb_zones.Text, _startDate, _endDate);

                    if (FDataSet.Tables["documentfinancemaster"] == null || FDataSet.Tables["documentfinancemaster"].Rows.Count.Equals(0))
                    {
                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();
                    }
                    else
                    {
                        panel_defaultReport.Visible = false;

                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        FReport.Prepare();
                        FReport.ShowPrepared();
                    }
                }

            }
            catch (Exception exx)
            {
                _log.Error("SelectZone: " + exx.Message, exx);
            }
        }

        private void bt_design_Click(object sender, EventArgs e)
        {
            DesignReport();
        }

        private void bt_searchTables_Click(object sender, EventArgs e)
        {
            FrameworkUtils.ShowWaitingCursor();

            SelectTablesZones(FReport.FileName);

            FrameworkUtils.HideWaitingCursor();
        }

        private void bt_searchZones_Click(object sender, EventArgs e)
        {
            FrameworkUtils.ShowWaitingCursor();

            SelectZone();

            FrameworkUtils.HideWaitingCursor();
        }


        private void bt_client_Click(object sender, EventArgs e)
        {
            FrameworkUtils.ShowWaitingCursor();

            SelectClients(FReport.FileName);

            FrameworkUtils.HideWaitingCursor();
        }


        private void SelectCustomersFinanceMaster()
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();
                string _customer = cb_customers.Text;
                _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
                _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);

                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("SelectCustomersFinanceMaster: " + exx.Message, exx);
            }
        }

        private void SelectCustomers(string _report)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
                _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);

                Data.DatesReport(_startDate, _endDate, FReport.FileName);

                if (FReport.FileName != "")
                {
                    FReport.Load(FReport.FileName);
                }

                panel_date.Visible = true;
                panel_Zone_Table.Visible = false;
                panel_Customers.Visible = true;
                panel_Clients.Visible = false;
                panel_Family.Visible = false;
                panel_Terminal.Visible = false;
                panel_Zones.Visible = false;
                panel_Payment.Visible = false;
                panel_Movements.Visible = false;
                panel_SearchItemsClosingBox.Visible = false;
                panel_SearchItemsClosingDay.Visible = false;

                if (cb_customer.Text.Equals(Resx.All))
                {
                    if (_report.Equals("Movimentos_empregado") || _report.Equals(thisFolder + "Movimentos_empregado.frx"))
                    {
                        report = thisFolder + "Movimentos_empregado.frx";

                        FDataSet.Clear();
                        FDataSet = DataReportsXML.GetData_MovCustomers(_startDate, _endDate);

                        if (FDataSet.Tables["systemaudit"] == null || FDataSet.Tables["systemaudit"].Rows.Count.Equals(0))
                        {
                            FReport.Clear();
                            panel_defaultReport.Visible = true;

                            FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                            frmMessageNotFound.ShowDialog();
                        }
                        else
                        {
                            FReport.Clear();
                            panel_defaultReport.Visible = false;

                            FReport.Load(report);
                            FReport.RegisterData(FDataSet, "NewDataSet");
                            FReport.Prepare();
                            FReport.ShowPrepared();
                        }
                    }
                    else if (_report.Equals(thisFolder + "Top_Registos_Empregados.frx") || _report.Equals("Top_Registos_Empregados"))
                    {

                        report = thisFolder + "Top_Registos_Empregados.frx";

                        FDataSet.Clear();
                        FDataSet = DataReportsXML.GetData_RecordsCustomer(_startDate, _endDate);

                        if (FDataSet.Tables["documentfinancemaster"] == null || FDataSet.Tables["documentfinancemaster"].Rows.Count.Equals(0))
                        {
                            FReport.Clear();
                            panel_defaultReport.Visible = true;

                            FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                            frmMessageNotFound.ShowDialog();
                        }
                        else
                        {
                            FReport.Clear();
                            panel_defaultReport.Visible = false;

                            FReport.Load(report);
                            FReport.RegisterData(FDataSet, "NewDataSet");
                            FReport.Prepare();
                            FReport.ShowPrepared();
                        }
                    }
                    else
                    {
                        report = thisFolder + "Total_empregado.frx";

                        FDataSet.Clear();
                        FDataSet = DataReportsXML.GetData_TotalCustomer(_startDate, _endDate);

                        if (FDataSet.Tables["documentfinancemaster"] == null || FDataSet.Tables["documentfinancemaster"].Rows.Count.Equals(0))
                        {
                            FReport.Clear();
                            panel_defaultReport.Visible = true;

                            FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                            frmMessageNotFound.ShowDialog();
                        }
                        else
                        {
                            FReport.Clear();
                            panel_defaultReport.Visible = false;

                            FReport.Load(report);
                            FReport.RegisterData(FDataSet, "NewDataSet");
                            FReport.Prepare();
                            FReport.ShowPrepared();
                        }

                    }
                }
                else if (_report.Equals("Movimentos_empregado") || _report.Equals(thisFolder + "Movimentos_empregado.frx"))
                {
                    report = thisFolder + "Movimentos_empregado.frx";
                    FDataSet.Clear();
                    FDataSet = Data.GetMovCustomers(cb_customer.Text, _startDate, _endDate);

                    if (FDataSet.Tables["systemaudit"] == null || FDataSet.Tables["systemaudit"].Rows.Count.Equals(0))
                    {
                        FReport.Clear();
                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();
                    }
                    else
                    {
                        FReport.Clear();
                        panel_defaultReport.Visible = false;

                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        FReport.Prepare();
                        FReport.ShowPrepared();
                    }
                }
                else if (_report.Equals(thisFolder + "Top_Registos_Empregados.frx") || _report.Equals("Top_Registos_Empregados"))
                {
                    report = thisFolder + "Top_Registos_Empregados.frx";
                    FDataSet.Clear();
                    FDataSet = Data.GetRecordsCustomerByDesignation(cb_customer.Text, _startDate, _endDate);
                    if (FDataSet.Tables["documentfinancemaster"] == null || FDataSet.Tables["documentfinancemaster"].Rows.Count.Equals(0))
                    {
                        FReport.Clear();
                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();
                    }
                    else
                    {
                        FReport.Clear();
                        panel_defaultReport.Visible = false;

                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        FReport.Prepare();
                        FReport.ShowPrepared();
                    }
                }
                else
                {
                    report = thisFolder + "Total_empregado.frx";
                    FDataSet.Clear();
                    FDataSet = Data.GetCustomerByDesignation(cb_customer.Text, _startDate, _endDate);
                    if (FDataSet.Tables["documentfinancemaster"] == null || FDataSet.Tables["documentfinancemaster"].Rows.Count.Equals(0))
                    {
                        FReport.Clear();
                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();
                    }
                    else
                    {
                        FReport.Clear();
                        panel_defaultReport.Visible = false;

                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        FReport.Prepare();
                        FReport.ShowPrepared();
                    }

                }
                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("SelectCustomersF: " + exx.Message, exx);
            }
        }



        private void bt_terminal_Click(object sender, EventArgs e)
        {
            SelectTerminal();
        }



        private void item_Click(object sender, EventArgs e)
        {
            try
            {
                ObjectInfo info = (sender as ToolStripMenuItem).Tag as ObjectInfo;
                if (info == null)
                {
                    _log.Debug(string.Format("info == null"));
                    // we clicked "Save to .fpx" item
                    previewControl1.Save();

                }
                else
                {
                    _log.Debug(string.Format("info != null"));
                    ExportBase export = Activator.CreateInstance(info.Object) as ExportBase;
                    export.CurPage = previewControl1.PageNo;
                    export.Export(previewControl1.Report);


                }
            }
            catch (Exception exx)
            {
                _log.Error("item_Click: " + exx.Message, exx);
            }
        }

        private void tbPageNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    _log.Debug(string.Format("e.KeyData == Keys.Enter"));
                    previewControl1.PageNo = int.Parse(tbPageNo.Text);
                }
            }
            catch (Exception exx)
            {
                _log.Error("tbPageNo_KeyDown: " + exx.Message, exx);
            }
        }

        private void previewControl1_PageChanged(object sender, EventArgs e)
        {
            try
            {
                tbPageNo.Text = previewControl1.PageNo.ToString();
            }
            catch (Exception exx)
            {
                _log.Error("previewControl1_PageChanged: " + exx.Message, exx);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                previewControl1.Print();
            }
            catch (Exception exx)
            {
                _log.Error("btnPrint_Click: " + exx.Message, exx);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                //  build export menu
                exportMenu.Items.Clear();
                List<ObjectInfo> list = new List<ObjectInfo>();
                RegisteredObjects.Objects.EnumItems(list);

                ToolStripMenuItem saveNative = new ToolStripMenuItem("Save to .fpx file...");
                saveNative.Click += new EventHandler(item_Click);
                exportMenu.Items.Add(saveNative);
                try
                {

                    foreach (ObjectInfo info in list)
                    {
                        if (info.Object != null && info.Object.IsSubclassOf(typeof(ExportBase)))
                        {
                            ToolStripMenuItem item = new ToolStripMenuItem(Res.TryGet(info.Text) + "...");
                            item.Tag = info;
                            item.Click += new EventHandler(item_Click);
                            if (info.ImageIndex != -1)
                                item.Image = Res.GetImage(info.ImageIndex);
                            exportMenu.Items.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("btnExport_Click(e.Message) 2: {0}", ex.Message), ex);

                }

                exportMenu.Show(btnExport, new Point(0, exportMenu.Height));
            }
            catch (Exception exx)
            {
                _log.Error("btnExport_Click 1: " + exx.Message, exx);
            }
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            previewControl1.ZoomOut();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            previewControl1.ZoomIn();

        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            previewControl1.First();
        }

        private void btnPrior_Click(object sender, EventArgs e)
        {
            previewControl1.Prior();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            previewControl1.Next();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            previewControl1.Last();
        }



        private void bt_totalzonetable_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SelectTablesZones("Total_mesas");

                FrameworkUtils.HideWaitingCursor();

            }
            catch (Exception exx)
            {
                _log.Error("bt_totalzonetable_Click: " + exx.Message, exx);
            }
        }


        private void bt_totalfamily_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SelectFamily("TotalFamily");

                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_totalfamily_Click: " + exx.Message, exx);
            }

        }

        private void bt_totalzone_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SelectZone();

                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_totalzone_Click: " + exx.Message, exx);
            }
        }

        private void bt_totalterminal_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SelectTerminal();

                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_totalterminal_Click: " + exx.Message, exx);
            }
        }


        private void bt_close_Click(object sender, EventArgs e)
        {
            try
            {
                FReport.Dispose();

                FormSaveProjectQuit frmSaveProjectQuit = new FormSaveProjectQuit();
                frmSaveProjectQuit.ShowDialog();

                if (frmSaveProjectQuit.DialogResult == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
            catch (Exception exx)
            {
                _log.Error("bt_close_Click: " + exx.Message, exx);
            }
        }

        private void FormReporting_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void bt_movCustomers_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SelectCustomers("Movimentos_empregado");

                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_movCustomers_Click: " + exx.Message, exx);
            }
        }


        private void bt_recordsCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SelectCustomers("Top_Registos_Empregados");

                FrameworkUtils.ShowWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_recordsCustomer_Click: " + exx.Message, exx);
            }
        }

        private void bt_totalCash_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SelectTypePayment();

                FrameworkUtils.HideWaitingCursor();

            }
            catch (Exception exx)
            {
                _log.Error("bt_totalCash_Click: " + exx.Message, exx);
            }
        }



        private void bt_searchtypepayment_Click(object sender, EventArgs e)
        {
            FrameworkUtils.ShowWaitingCursor();

            SelectTypePayment();

            FrameworkUtils.HideWaitingCursor();
        }

        private void SelectTypePayment()
        {
            try
            {
               string _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
               string _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);

                Data.DatesReport(_startDate, _endDate, FReport.FileName);

                if (FReport.FileName != "")
                {
                    FReport.Load(FReport.FileName);
                }
                report = thisFolder + "Total_caixa.frx";

                panel_date.Visible = true;
                panel_Zone_Table.Visible = false;
                panel_Customers.Visible = false;
                panel_Clients.Visible = false;
                panel_Family.Visible = false;
                panel_Terminal.Visible = false;
                panel_Zones.Visible = false;
                panel_Payment.Visible = true;
                panel_Movements.Visible = true;
                panel_SearchItemsClosingBox.Visible = false;
                panel_SearchItemsClosingDay.Visible = false;

                if (cb_typepayment.Text.Equals(Resx.All))
                {
                    FDataSet.Clear();
                    FDataSet = DataReportsXML.GetData_TotalCash(_startDate, _endDate);
                    if (FDataSet.Tables["worksessionmovement"] == null || FDataSet.Tables["worksessionmovement"].Rows.Count.Equals(0))
                    {
                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();
                    }
                    else
                    {
                        panel_defaultReport.Visible = false;

                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        FReport.Prepare();
                        FReport.ShowPrepared();
                    }
                }
                else
                {
                    FDataSet.Clear();

                    FDataSet = Data.GetTypePaymentByDesignation(cb_typepayment.Text, _startDate, _endDate);
                    if (FDataSet.Tables["worksessionmovement"] == null || FDataSet.Tables["worksessionmovement"].Rows.Count.Equals(0))
                    {
                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();
                    }
                    else
                    {
                        panel_defaultReport.Visible = false;

                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        FReport.Prepare();
                        FReport.ShowPrepared();
                    }

                }
            }
            catch (Exception exx)
            {
                _log.Error("SelectTypePayment: " + exx.Message, exx);
            }
        }


        private void bt_typemovement_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SelectTypeMovement();

                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_typemovement_Click: " + exx.Message, exx);
            }
        }

        private void SelectTypeMovement()
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
                _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);

                FDataSet.Clear();

                if (cb_typemovement.Text.Equals(Resx.All))
                {
                    FDataSet.Clear();
                    FDataSet = DataReportsXML.GetData_MovCustomers(_startDate, _endDate);

                    if (FDataSet.Tables["systemaudit"] == null || FDataSet.Tables["systemaudit"].Rows.Count.Equals(0))
                    {
                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();
                    }
                    else
                    {
                        panel_defaultReport.Visible = false;
                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");
                        //  FReport.Show();

                        FReport.Prepare();
                        FReport.ShowPrepared();
                    }
                }
                else
                {
                    FDataSet.Clear();

                    FDataSet = Data.GetTypeMovementByDesignation(cb_typemovement.Text, _startDate, _endDate);

                    if (FDataSet.Tables["systemaudit"] == null || FDataSet.Tables["systemaudit"].Rows.Count.Equals(0))
                    {
                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();
                    }
                    else
                    {
                        panel_defaultReport.Visible = false;
                        FReport.Load(report);
                        FReport.RegisterData(FDataSet, "NewDataSet");

                        FReport.Prepare();
                        FReport.ShowPrepared();
                    }

                }
                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("SelectTypeMovement: " + exx.Message, exx);
            }
        }



        private void bt_searchFamily_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SelectFamily(FReport.FileName);

                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_searchFamily_Click: " + exx.Message, exx);
            }
        }



        private void bt_totalCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SelectCustomers("Total_empregado");

                panel_defaultReport.Visible = false;

                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_totalCustomer_Click: " + exx.Message, exx);
            }
        }

        private void bt_listArticles_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                panel_defaultReport.Visible = false;

                GetFamilyArticle();

                SelectFamily("ListArticles");

                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_listArticles_Click: " + exx.Message, exx);
            }
        }

        private void bt_occupationPlace_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SelectTablesZones("Top_media_ocupação");

                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_occupationPlace_Click: " + exx.Message, exx);
            }
        }

        private void bt_searchDate_Click(object sender, EventArgs e)
        {
            try
            {
                bMessageNotFound = false;

                FrameworkUtils.ShowWaitingCursor();

               string _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
               string  _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);

                DataSet resultSearch = new DataSet();

                Data.DatesReport(_startDate, _endDate, FReport.FileName);

                thisFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));

                if (FReport.FileName != "")
                {
                    FReport.Load(FReport.FileName);
                }

                if (FReport.FileName != "Reports/Stock_artigos.frx")
                {
                    ItemsSearchClosingDay();
                    panel_SearchItemsClosingDay.Visible = true;
                }

                if (panel_SearchItemsClosingDay.Visible == true)
                {
                    FrameworkUtils.ShowWaitingCursor();

                    lb_ItemsClosingDay.SelectedIndex = 0;

                    SearchItemsClosingDay(lb_ItemsClosingDay.SelectedItem.ToString(), _startDate, _endDate);

                    FrameworkUtils.HideWaitingCursor();

                }
                else
                {

                    if (FReport.FileName.Equals(thisFolder + "Total_datas.frx"))
                    {
                        FDataSet = Data.GetFinanceMasterByDate(false, _startDate, _endDate);

                        _log.Debug(string.Format("GetFinanceMasterByDate"));

                        FReport.RegisterData(FDataSet, "NewDataSet");

                        if (FDataSet.Tables["documentfinancemaster"] == null || FDataSet.Tables["documentfinancemaster"].Rows.Count.Equals(0))
                        {

                            FReport.Clear();
                            panel_defaultReport.Visible = true;

                            FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                            frmMessageNotFound.ShowDialog();
                        }
                        else
                        {
                            panel_defaultReport.Visible = false;

                            FReport.Prepare();
                            FReport.ShowPrepared();

                        }


                    }
                    else if (FReport.FileName.Equals(thisFolder + "Stock_artigos.frx"))
                    {
                        FDataSet = Data.GetStockArticlesByDates(_startDate, _endDate);

                        _log.Debug(string.Format("GetStockArticlesByDate"));

                        FReport.RegisterData(FDataSet, "NewDataSet");

                        if (FDataSet.Tables["view_articlestock"] == null || FDataSet.Tables["view_articlestock"].Rows.Count.Equals(0))
                        {

                            FReport.Clear();
                            panel_defaultReport.Visible = true;

                            FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                            frmMessageNotFound.ShowDialog();
                        }
                        else
                        {
                            panel_defaultReport.Visible = false;

                            FReport.Prepare();
                            FReport.ShowPrepared();

                        }


                    }
                }
                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_searchDate_Click: " + exx.Message, exx);
            }
        }

        private void bt_searchEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();


                SelectCustomers(FReport.FileName);

                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_searchEmployee_Click: " + exx.Message, exx);
            }
        }

        //FlowLayoutPanel flp = new FlowLayoutPanel() { Dock = DockStyle.Fill, AutoScroll = true };
        int tmpBorder = -1;
        private void bt_scrollup_Click(object sender, EventArgs e)
        {
            try
            {
                if (tmpBorder == -1)
                {
                    tmpBorder = manualScrollPanel1.Height;
                }
                if (
                  (manualScrollPanel2.Top < manualScrollPanel1.Height - tmpBorder)
                  )
                {
                    manualScrollPanel2.Top += 30;
                }
                manualScrollPanel2.Invalidate();
                manualScrollPanel1.Invalidate();
            }
            catch (Exception exx)
            {
                _log.Error("bt_scrollup_Click: " + exx.Message, exx);
            }
        }

        private void bt_scrolldown_Click(object sender, EventArgs e)
        {
            try
            {
                if (tmpBorder == -1)
                {
                    tmpBorder = manualScrollPanel1.Height;
                }

                if (
                  ((manualScrollPanel2.Top + manualScrollPanel2.Height) > (tmpBorder))
                  )
                {
                    manualScrollPanel2.Top -= 30;
                }
                manualScrollPanel2.Invalidate();
                manualScrollPanel1.Invalidate();
            }
            catch (Exception exx)
            {
                _log.Error("bt_scrolldown_Click: " + exx.Message, exx);
            }
        }

        private void manualScrollPanel_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
                {
                    e.NewValue.ToString();
                    return;
                }

            }
            catch (Exception exx)
            {
                _log.Error("manualScrollPanel_Scroll: " + exx.Message, exx);
            }
        }

        private void manualScrollPanel_Click(object sender, EventArgs e)
        {
            try
            {
                //this.manualScrollPanel.Focus();
            }
            catch (Exception exx)
            {
                _log.Error("manualScrollPanel_Click: " + exx.Message, exx);
            }
        }

        private void bt_totalday_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                thisFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));

              string  _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
              string  _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);

                Data.DatesReport(_startDate, _endDate, FReport.FileName);

                panel_defaultReport.Visible = false;

                if (FReport.FileName != "")
                {
                    FReport.Load(FReport.FileName);
                }

                try
                {
                    report = thisFolder + "Total_datas.frx";

                    FDataSet = Data.GetFinanceMasterByDate(false, _startDate, _endDate);

                    FReport.Load(report);
                    FReport.RegisterData(FDataSet, "NewDataSet");

                    if (FDataSet.Tables == null || FDataSet.Tables["documentfinancemaster"] == null)
                    {
                        panel_date.Visible = true;
                        panel_Zone_Table.Visible = false;
                        panel_Customers.Visible = false;
                        panel_Clients.Visible = false;
                        panel_Family.Visible = false;
                        panel_Terminal.Visible = false;
                        panel_Zones.Visible = false;
                        panel_Payment.Visible = false;
                        panel_Movements.Visible = false;
                        panel_SearchItemsClosingBox.Visible = false;
                        panel_SearchItemsClosingDay.Visible = false;

                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();

                    }
                    else
                    {
                        FReport.Prepare();
                        FReport.ShowPrepared();

                        panel_defaultReport.Visible = false;

                        panel_date.Visible = true;
                        panel_Zone_Table.Visible = false;
                        panel_Customers.Visible = false;
                        panel_Clients.Visible = false;
                        panel_Family.Visible = false;
                        panel_Terminal.Visible = false;
                        panel_Zones.Visible = false;
                        panel_Payment.Visible = false;
                        panel_Movements.Visible = false;
                        panel_SearchItemsClosingBox.Visible = false;
                        panel_SearchItemsClosingDay.Visible = false;
                    }

                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("bt_totalday_Click(e.Message): {0}", ex.Message), ex);

                }

                FrameworkUtils.ShowWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_totalday_Click 2: " + exx.Message, exx);
            }
        }

        private void bt_closingDay_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                ItemsSearchClosingDay();

                lb_ItemsClosingDay.SelectedIndex = 0;

              string  _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
              string  _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);

                if (lb_ItemsClosingDay.SelectedItem != null)
                {
                    SearchItemsClosingDay(lb_ItemsClosingDay.SelectedItem.ToString(), _startDate, _endDate);
                }

                FrameworkUtils.ShowWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_closingDay_Click: " + exx.Message, exx);
            }
        }



        private void Terminals_ListBoxClicked(object sender, FormSelectTerminal.TerminalSelectedEventArgs e)
        {
            try
            {
                tableTerminals = e.Terminals;

                _startDateFormSelectTerminal = Convert.ToDateTime(e.StartDate).Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
                _endDateFormSelectTerminal = Convert.ToDateTime(e.EndDate).Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);

                //_startDateFormSelectTerminal = e.StartDate;
                //_endDateFormSelectTerminal = e.EndDate;
                _status = e.Status;
            }
            catch (Exception exx)
            {
                _log.Error("Terminals_ListBoxClicked: " + exx.Message, exx);
            }
        }


        private void bt_closingBox_Click(object sender, EventArgs e)
        {
            try
            {
                FormSelectTerminal frmSaveProjectQuit = new FormSelectTerminal();
                frmSaveProjectQuit.TerminalSelected += new FormSelectTerminal.TerminalSelectedHandler(Terminals_ListBoxClicked);
                frmSaveProjectQuit.ShowDialog();

                if (_status == true)
                {

                    if (_startDateFormSelectTerminal != null || _endDateFormSelectTerminal != null || tableTerminals.Rows.Count != 0)
                    {
                        FrameworkUtils.ShowWaitingCursor();

                        ItemsSearchClosingBox();

                        lb_ItemsClosingBox.SelectedIndex = 0;

                        FrameworkUtils.ShowWaitingCursor();

                    }
                    else
                    {
                        frmSaveProjectQuit.ShowDialog();
                    }
                }
            }
            catch (Exception exx)
            {
                _log.Error("bt_closingBox_Click: " + exx.Message, exx);
            }
        }


        private void br_searchTerminalDatesClosingBox_Click(object sender, EventArgs e)
        {
            try
            {
                FormSelectTerminal frmSaveProjectQuit = new FormSelectTerminal();
                frmSaveProjectQuit.TerminalSelected += new FormSelectTerminal.TerminalSelectedHandler(Terminals_ListBoxClicked);
                frmSaveProjectQuit.ShowDialog();

                if (_status == true)
                {

                    if (_startDateFormSelectTerminal != null || _endDateFormSelectTerminal != null || tableTerminals.Rows.Count != 0)
                    {
                        FrameworkUtils.ShowWaitingCursor();

                        ItemsSearchClosingBox();

                        lb_ItemsClosingBox.SelectedIndex = 0;


                        FrameworkUtils.ShowWaitingCursor();
                    }
                }
                //else
                //{
                //  frmSaveProjectQuit.ShowDialog();
                //}
            }
            catch (Exception exx)
            {
                _log.Error("br_searchTerminalDatesClosingBox_Click: " + exx.Message, exx);
            }
        }

        private void lb_searchItemsClosingBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                ItemsSearchClosingBox();

                SearchItemsClosingBox(lb_ItemsClosingBox.SelectedItem.ToString(), _startDateFormSelectTerminal, _endDateFormSelectTerminal);

                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("lb_searchItemsClosingBox_SelectedIndexChanged: " + exx.Message, exx);
            }
        }

        private void lb_ItemsClosingDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ItemsSearchClosingDay();

               string _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
               string _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);

                FrameworkUtils.ShowWaitingCursor();

                SearchItemsClosingDay(lb_ItemsClosingDay.SelectedItem.ToString(), _startDate, _endDate);

                FrameworkUtils.HideWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("lb_ItemsClosingDay_SelectedIndexChanged: " + exx.Message, exx);
            }
        }

        private void bt_CurrentAccount_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SelectClients("Conta_corrente_clientes");

                FrameworkUtils.ShowWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_CurrentAccount_Click: " + exx.Message, exx);
            }
        }

        private void bt_DetailsCurrentAccount_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                SelectClients("Conta_corrente_clientes_Detalhado");

                FrameworkUtils.ShowWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_DetailsCurrentAccount_Click: " + exx.Message, exx);
            }
        }

        private void bt_stockArticles_Click(object sender, EventArgs e)
        {
            try
            {
                FrameworkUtils.ShowWaitingCursor();

                thisFolder = (string.Format("{0}", GlobalFramework.Path["reports"]));

                string _startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
                string _endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);

                Data.DatesReport(_startDate, _endDate, FReport.FileName);

                panel_defaultReport.Visible = false;

                if (FReport.FileName != "")
                {
                    FReport.Load(FReport.FileName);
                }

                try
                {
                    report = thisFolder + "Stock_artigos.frx";

                    FDataSet = Data.GetStockArticlesByDates(_startDate, _endDate);

                    FReport.Load(report);
                    FReport.RegisterData(FDataSet, "NewDataSet");

                    if (FDataSet.Tables == null || FDataSet.Tables["view_articlestock"] == null)
                    {
                        panel_date.Visible = true;
                        panel_Zone_Table.Visible = false;
                        panel_Customers.Visible = false;
                        panel_Clients.Visible = false;
                        panel_Family.Visible = false;
                        panel_Terminal.Visible = false;
                        panel_Zones.Visible = false;
                        panel_Payment.Visible = false;
                        panel_Movements.Visible = false;
                        panel_SearchItemsClosingBox.Visible = false;
                        panel_SearchItemsClosingDay.Visible = false;

                        panel_defaultReport.Visible = true;

                        FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                        frmMessageNotFound.ShowDialog();

                    }
                    else
                    {
                        FReport.Prepare();
                        FReport.ShowPrepared();

                        panel_defaultReport.Visible = false;

                        panel_date.Visible = true;
                        panel_Zone_Table.Visible = false;
                        panel_Customers.Visible = false;
                        panel_Clients.Visible = false;
                        panel_Family.Visible = false;
                        panel_Terminal.Visible = false;
                        panel_Zones.Visible = false;
                        panel_Payment.Visible = false;
                        panel_Movements.Visible = false;
                        panel_SearchItemsClosingBox.Visible = false;
                        panel_SearchItemsClosingDay.Visible = false;
                    }

                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("bt_stockArticles_Click(e.Message): {0}", ex.Message), ex);

                }

                FrameworkUtils.ShowWaitingCursor();
            }
            catch (Exception exx)
            {
                _log.Error("bt_stockArticles_Click 2: " + exx.Message, exx);
            }
        }

    }
}