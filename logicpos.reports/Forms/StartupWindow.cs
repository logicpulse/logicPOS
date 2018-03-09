using logicpos.financial;
using logicpos.reports.App;
using logicpos.reports.Forms;
using logicpos.reports.Resources.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace logicpos.reports
{
    public partial class StartupWindow : Form
    {
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Color _colorEntryValidationValidFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationValidFont"]);
        private Color _colorEntryValidationInvalidFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorEntryValidationInvalidFont"]);

        public StartupWindow()
        {
            InitializeComponent();

            tb_code.ForeColor = _colorEntryValidationInvalidFont;


            label13.Text = Resx.Quit;

            cp_Key9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            cp_Key9.BackColor2 = System.Drawing.Color.Gainsboro;
            cp_Key8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            cp_Key8.BackColor2 = System.Drawing.Color.Gainsboro;
            cp_Key7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            cp_Key7.BackColor2 = System.Drawing.Color.Gainsboro;
            cp_Key6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            cp_Key6.BackColor2 = System.Drawing.Color.Gainsboro;
            cp_Key5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            cp_Key5.BackColor2 = System.Drawing.Color.Gainsboro;
            cp_Key4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            cp_Key4.BackColor2 = System.Drawing.Color.Gainsboro;
            cp_Key3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            cp_Key3.BackColor2 = System.Drawing.Color.Gainsboro;
            cp_Key2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            cp_Key2.BackColor2 = System.Drawing.Color.Gainsboro;
            cp_Key1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(171)))), ((int)(((byte)(171)))));
            cp_Key1.BackColor2 = System.Drawing.Color.Gainsboro;

            cp_CE.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(199)))), ((int)(((byte)(100)))), ((int)(((byte)(86)))));
            cp_CE.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(157)))), ((int)(((byte)(146)))));

            cp_Quit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(84)))), ((int)(((byte)(96)))));
            cp_Quit.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(113)))), ((int)(((byte)(134)))));

            cp_OK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(210)))), ((int)(((byte)(87)))));
            cp_OK.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(238)))), ((int)(((byte)(143)))));

        }



        private void tb_code_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        public void ChangeEnableButton()
        {
            if (tb_code.Text.Length >= 4 && tb_code.Text.Length <= 12)
            {
                tb_code.ForeColor = _colorEntryValidationValidFont;
                cp_OK.Enabled = true;
            }
        }


        private void customPanel_MouseLeave(object sender, EventArgs e)
        {
            CustomPanel control = ((CustomPanel)sender);
            System.Drawing.Color tempColor = control.BackColor;
            control.BackColor = control.BackColor2;
            control.BackColor2 = tempColor;
            control.Invalidate();
        }


        private void cp_CE_MouseClick(object sender, MouseEventArgs e)
        {
            string removeCode = tb_code.Text.ToString();
            if (tb_code.Text.Length > 0)
            {
                tb_code.Text = removeCode.Remove(removeCode.Length - 1);
            }
        }

        private void cp_Quit_MouseClick(object sender, MouseEventArgs e)
        {
            FormSaveProjectQuit frmSaveProjectQuit = new FormSaveProjectQuit();
            frmSaveProjectQuit.ShowDialog();

            if (frmSaveProjectQuit.DialogResult == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void cp_OK_MouseClick(object sender, MouseEventArgs e)
        {
            string code = tb_code.Text;

            var result = Data.GetUser(code);

            if (result == "")
            {
                tb_code.PasswordChar = (char)0;
                tb_code.Text = Resx.codeErrorPin;
                tb_code.ForeColor = _colorEntryValidationInvalidFont;
            }
            else
            {
                FrameworkUtils.ShowWaitingCursor();
                _log.Debug("Before new FormReporting()");
                FormReporting reporting = new FormReporting();
                _log.Debug("After new FormReporting()");

                _log.Debug("Before reporting.Show()");
                reporting.Show();
                _log.Debug("After reporting.Show()");

                this.Hide();
                FrameworkUtils.HideWaitingCursor();

            }
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            Label label = ((Label)sender);
            System.Drawing.Color tempColor = System.Drawing.Color.Transparent;
            label.BackColor = tempColor;
            label.Invalidate();

        }

        private void cp_Key0_MouseClick(object sender, MouseEventArgs e)
        {
            tb_code.Text += "0";
            ChangeEnableButton();
        }

        private void cp_Key1_MouseClick(object sender, MouseEventArgs e)
        {
            tb_code.Text += "1";
            ChangeEnableButton();
        }

        private void cp_Key2_MouseClick(object sender, MouseEventArgs e)
        {
            tb_code.Text += "2";
            ChangeEnableButton();
        }

        private void cp_Key3_MouseClick(object sender, MouseEventArgs e)
        {
            tb_code.Text += "3";
            ChangeEnableButton();
        }

        private void cp_Key4_MouseClick(object sender, MouseEventArgs e)
        {
            tb_code.Text += "4";
            ChangeEnableButton();
        }

        private void cp_Key5_MouseClick(object sender, MouseEventArgs e)
        {
            tb_code.Text += "5";
            ChangeEnableButton();
        }

        private void cp_Key6_MouseClick(object sender, MouseEventArgs e)
        {
            tb_code.Text += "6";
            ChangeEnableButton();
        }

        private void cp_Key7_MouseClick(object sender, MouseEventArgs e)
        {
            tb_code.Text += "7";
            ChangeEnableButton();
        }

        private void cp_Key8_MouseClick(object sender, MouseEventArgs e)
        {
            tb_code.Text += "8";
            ChangeEnableButton();
        }

        private void cp_Key9_MouseClick(object sender, MouseEventArgs e)
        {
            tb_code.Text += "9";
            ChangeEnableButton();
        }




    }
}
