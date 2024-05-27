using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using FastReport;
using FastReport.Data;
using FastReport.Dialog;
using FastReport.Barcode;
using FastReport.Table;
using FastReport.Utils;

//WIP: Currently not Used

namespace FastReport
{
  public class ReportScript
  {
    //To Work Require assign Function
    //<inherited Name="Text1" Border.Lines="All" Fill.Color="DarkGray" BeforePrintEvent="Text1_BeforePrint" Font="Arial, 9.75pt, style=Bold" TextFill.Color="Maroon" Style="NEW STYLE"/>
    private void _StartReport(object sender, EventArgs e)
    {
      //text.BeforePrint = Text1_BeforePrint;
      //Text1.BeforePrint += delegate { Text1.Text = logicpos.GlobalApp.Settings["appName"]; };
    }

    private void Text1_BeforePrint(object sender, EventArgs e)
    {
      //TextObject text = (TextObject) sender;
      //text.Text = logicpos.GlobalApp.Settings["appName"];
      //text.Text = string.Format("{0} {1}", logicpos.GlobalApp.Settings["appName"], logicpos.Utils.ProductVersion);
    }
  }
}
