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
    public partial class FormSaveProjectQuit : Form
    {
        public FormSaveProjectQuit()
        {
            InitializeComponent();

            buttonOk.Text = Resx.buttonOk;
            buttonCancel.Text = Resx.buttonCancel;
            label1.Text = Resx.Msg_Quit;
            label10.Text = Resx.Exit;
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

    }
}
