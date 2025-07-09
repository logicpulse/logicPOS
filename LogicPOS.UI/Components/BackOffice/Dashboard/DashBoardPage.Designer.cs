using Gtk;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using Medsphere.Widgets;
using System;
using System.Collections;
using System.Drawing;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage
    {

        //Cores usadas nos gráficos
        public PlotColor[] plotColors = new PlotColor[] {
            PlotColor.Red,
            PlotColor.Blue,
            PlotColor.Green,
            PlotColor.Orange,
            PlotColor.Purple,
            PlotColor.Brown,
            PlotColor.DarkRed,
            PlotColor.DarkBlue,
            PlotColor.DarkGreen,
            PlotColor.DarkOrange,
            PlotColor.DarkPurple,
            PlotColor.DarkYellow,
            PlotColor.DarkBrown
        };



        //ScreenArea
        protected EventBox _eventBox;
        protected Color _colorBaseDialogDefaultButtonFont = ("76, 72, 70").StringToColor();
        protected Color _colorBaseDialogDefaultButtonBackground = ("156, 191, 42").StringToColor();
        protected Color _colorBaseDialogActionAreaButtonFont = ("0, 0, 0").StringToColor();
        protected Color _colorBaseDialogActionAreaButtonBackground = AppSettings.Instance.ColorBaseDialogActionAreaButtonBackground;
        //protected String _fontBaseDialogButton = SharedUtils.OSSlash(LogicPOS.Settings.AppSettings.Instance.FontBaseDialogButton"]);
        protected string _fontBaseDialogActionAreaButton = AppSettings.Instance.FontBaseDialogActionAreaButton;
        protected string _fileActionDefault = AppSettings.Paths.Images + @"Icons\icon_pos_default.png";
        protected string _fileActionOK = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
        protected string _fileActionCancel = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";

        //Colors
        private readonly Color colorBackOfficeContentBackground = AppSettings.Instance.ColorBackOfficeContentBackground;
        private readonly Color colorBackOfficeStatusBarBackground = AppSettings.Instance.ColorBackOfficeStatusBarBackground;
        private readonly Color colorBackOfficeAccordionFixBackground = AppSettings.Instance.ColorBackOfficeAccordionFixBackground;
        private readonly Color colorBackOfficeStatusBarFont = AppSettings.Instance.ColorBackOfficeStatusBarFont;
        private readonly Color colorBackOfficeStatusBarBottomBackground = AppSettings.Instance.ColorBackOfficeStatusBarBottomBackground;
        public Color slateBlue = Color.FromName("White");
        //private Frame frame;


        private void Design(Window parentWindow, dynamic themeWindow)
        {
            Color screenBackgroundColor = (themeWindow.Globals.ScreenBackgroundColor as string).StringToColor();
            Color white = Color.White;
            Color black = Color.Black;

            string errorMessage = "Node: <Window ID=\"PosBaseWindow\">";

            Fixed fix = new Fixed();
            HBox hbox = new HBox();
            Frame frame = new Frame();

            VBox vbox = new VBox(false, 2);
            VBox vbox2 = new VBox(true, 0);
            VBox vbox3 = new VBox(false, 5);


            _eventBox = new EventBox();
            _eventBox.WidthRequest = BackOfficeWindow.ScreenSize.Width;
            _eventBox.HeightRequest = BackOfficeWindow.ScreenSize.Height;
            Gtk.Alignment _alignmentWindow = new Alignment(0.0f, 0.0f, 0.0f, 0.0f)
            {
                _eventBox
            };
            Add(_alignmentWindow);
            try
            {
                //Imagem carregada aqui para o dashboard
                string fileImageBack = $"{AppSettings.Paths.Themes}Default/Backgrounds/Windows/LogicPOS_WorkFlow_{AppSettings.Culture.CurrentCultureName}.png";
                System.Drawing.Image pImage = System.Drawing.Image.FromFile(fileImageBack);
                Gdk.Pixbuf pixbuf = logicpos.Utils.ImageToPixbuf(pImage);
                _eventBox.Style = logicpos.Utils.GetImageBackgroundDashboard(pixbuf);



                fix.Put(BtnTerminals, 55, 62);
                fix.Put(BtnPreferenceParameters, 55, 155);
                fix.Put(BtnFiscalYears, 55, 250);
                fix.Put(BtnPrinters, 55, 345);

                fix.Put(BtnArticles, 245, 62);
                fix.Put(BtnCustomers, 245, 155);
                fix.Put(BtnUsers, 245, 250);
                fix.Put(BtnTables, 245, 345);

                fix.Put(BtnDocuments, 440, 62);
                fix.Put(BtnNewDocument, 440, 155);
                fix.Put(BtnPayments, 440, 250);
                fix.Put(BtnArticleStock, 440, 345);

                fix.Put(BtnReportsMenu, 635, 62);
                fix.Put(BtnPrintReportRouter, 635, 155);
                fix.Put(BtnCustomerBalanceDetails, 635, 250);
                fix.Put(BtnSalesPerDate, 635, 345);

                string currency = PreferenceParametersService.SystemCurrency;

                decimal dailyTotal = 0;
                decimal MonthlyTotal = 0;
                decimal annualTotal = 0;
                ArrayList values = new ArrayList
                {
                    DateTime.Now.Year.ToString()
                };

                _label = new Label();
                frame.ShadowType = (ShadowType)0;

                _label.Text = string.Format("{0} {3}\n\n{1} {3}\n\n{2} {3}",
                    Convert.ToInt64(Math.Round(dailyTotal, 0)).ToString(),
                    Convert.ToInt64(Math.Round(MonthlyTotal, 0)).ToString(),
                    Convert.ToInt64(Math.Round(annualTotal, 0)).ToString(),
                    currency.ToString());

                _label.ModifyFont(Pango.FontDescription.FromString("Trebuchet MS 16"));
                _label.ModifyFg(StateType.Normal, white.ToGdkColor());
                _label.Justify = Justification.Right;
                frame.Add(_label);
                hbox.PackStart(frame, false, false, 0);
                vbox.PackStart(hbox, false, false, 0);
                fix.Put(vbox, 628, 515);

                //COMBO BOX selecionar os anos do gráfico
                int w = 1;
                string[] getYears = new string[values.Count];
                getYears[0] = (string)values[0];
                for (int i = values.Count - 1; i > 0; i--)
                {
                    getYears[i] = (string)values[w];
                    w++;
                }
                //w = 1;
                selAno = new ComboBox(getYears);
                selAno.ModifyFg(StateType.Selected, black.ToGdkColor());

                TreeIter iter;
                selAno.Model.GetIterFirst(out iter);
                do
                {
                    GLib.Value thisRow = new GLib.Value();
                    selAno.Model.GetValue(iter, 0, ref thisRow);
                    if ((thisRow.Val as string).Equals(getYears[0]))
                    {
                        selAno.SetActiveIter(iter);
                        break;
                    }

                } while (selAno.Model.IterNext(ref iter));
                selAno.Changed += delegate
                {
                    annualTotal = 0;
                    _label.Text = string.Format("{0} {3}\n\n{1} {3}\n\n{2} {3}",
                     Convert.ToInt64(Math.Round(dailyTotal, 0)).ToString(),
                     Convert.ToInt64(Math.Round(MonthlyTotal, 0)).ToString(),
                     Convert.ToInt64(Math.Round(annualTotal, 0)).ToString(),
                     currency.ToString());

                    _label.ModifyFont(Pango.FontDescription.FromString("Trebuchet MS 16"));
                    _label.ModifyFg(StateType.Normal, white.ToGdkColor());
                    _label.Justify = Justification.Right;
                    frame.Add(_label);

                    hbox.PackStart(frame, false, false, 0);
                    vbox.PackStart(hbox, false, false, 0);
                    string selectedDate = string.Format("01/01/{0}", (selAno.ActiveText.ToString()));
                    fix.Put(vbox, 640, 515);

                };

                fix.Put(selAno, 220, 665);

                _eventBox.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));
                _eventBox.Add(fix);
                fix.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));
            }
            catch (Exception ex)
            {
                CustomAlerts.ShowThemeRenderingErrorAlert($"{errorMessage}\n\n{ex.Message}", parentWindow);
            }
        }

    }
}
