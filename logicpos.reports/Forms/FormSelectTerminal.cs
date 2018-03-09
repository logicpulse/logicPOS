using logicpos.reports.App;
using logicpos.reports.Resources.Localization;
using logicpos.reports.Utils;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace logicpos.reports.Forms
{
    public partial class FormSelectTerminal : Form
    {
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        DataTable _dataTerminal = new DataTable();
        string startDate;
        string endDate;
        bool status = true;
        DataSet data = new DataSet();

        public delegate void TerminalSelectedHandler(object sender, TerminalSelectedEventArgs e);

        public event TerminalSelectedHandler TerminalSelected;



        public FormSelectTerminal()
        {
            InitializeComponent();

            lb_endDate.Text = Resx.label_EndDate;
            lb_startDate.Text = Resx.label_StartDate;
            lb_reportData.Text = Resx.lb_reportData;
            cb_allTerminal.Text = Resx.All;
            buttonCancel.Text = Resx.bt_cancel;

            _dataTerminal = Data.GetTerminal_MovementResume();

            //dt_startDate.Value = DateTime.Today;
            //dt_endDate.Value = DateTime.Today;

            for (int i = 0; i < _dataTerminal.Rows.Count; i++)
            {
                lb_terminal.Items.Add(_dataTerminal.Rows[i].ItemArray[0].ToString());
            }
        }

        public class TerminalSelectedEventArgs : System.EventArgs
        {
            private DataTable mterminals;
            private string mstartDate;
            private string mendDate;
            private bool mstatus = true;

            public TerminalSelectedEventArgs(DataTable _terminals, string _startDate, string _endDate, bool _status)
            {
                this.mterminals = _terminals;
                this.mstartDate = _startDate;
                this.mendDate = _endDate;
                this.mstatus = _status;
            }

            public DataTable Terminals
            {
                get
                {
                    return mterminals;
                }
            }

            public string StartDate
            {
                get
                {
                    return mstartDate;
                }
            }
            public string EndDate
            {
                get
                {
                    return mendDate;
                }
            }

            public bool Status
            {
                get
                {
                    return mstatus;
                }
            }

        }

        private Point mouse_offset;
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_offset = new Point(-e.X, -e.Y);
        }
        //the Event of MouseMove, move the form if user click the left button of the mouse
        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouse_offset.X, mouse_offset.Y);
                this.Location = mousePos; //move the form to the desired location
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            try
            {
                _dataTerminal.Clear();
                foreach (var item in lb_terminal.SelectedItems)
                {
                    _dataTerminal.Rows.Add(item.ToString());
                }

                startDate = dt_startDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);
                endDate = dt_endDate.Value.Date.ToString(GlobalFramework.Settings["dateTimeFormatDocumentDate"]);

                string[] splitStartDate = startDate.Split(' ');
                string[] splitEndDate = endDate.Split(' ');

                string startDateSplit = splitStartDate[0];
                string endDateSplit = splitEndDate[0];

                data.Clear();
                data = DataReportsXML.GetClosingBox_Day(startDate, endDate, _dataTerminal, null);

                if (data.Tables == null || data.Tables["view_worksessionmovementresume"] == null || data.Tables["view_worksessionmovementresume"].Rows.Count == 0)
                {
                    FormMessageNotFound frmMessageNotFound = new FormMessageNotFound();
                    frmMessageNotFound.ShowDialog();
                    TerminalSelectedEventArgs args = new TerminalSelectedEventArgs(_dataTerminal, startDate, endDate, false);
                    TerminalSelected(this, args);
                }
                else
                {
                    TerminalSelectedEventArgs args = new TerminalSelectedEventArgs(_dataTerminal, startDate, endDate, status);
                    TerminalSelected(this, args);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

        }

        private void cb_allTerminal_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_allTerminal.Checked)
            {
                lb_terminal.SelectAll();
            }
            else
            {
                lb_terminal.UnSelectAll();
            }

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            TerminalSelectedEventArgs args = new TerminalSelectedEventArgs(_dataTerminal, startDate, endDate, false);
            TerminalSelected(this, args);
            this.Close();

        }







    }
}
