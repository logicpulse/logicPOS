using logicpos.financial;
using logicpos.reports.Resources.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace logicpos.reports.Forms
{
    public partial class FormMessageNotFound : Form
    {
        public FormMessageNotFound()
        {
            InitializeComponent();
         
            label1.Text = Resx.Msg_NoRecordsWereFound1;
            label2.Text = Resx.Msg_NoRecordsWereFound2;
            label10.Text = Resx.Information;
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

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void customPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonOk_Click(object sender, EventArgs e)
        {

        }

     
    }
}
