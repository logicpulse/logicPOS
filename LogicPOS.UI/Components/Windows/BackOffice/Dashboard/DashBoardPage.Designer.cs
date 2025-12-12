using Gtk;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Components.Windows.BackOffice.Dashboard;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using Medsphere.Widgets;
using Serilog;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage
    {
        private string DashboardBackgroundImagePath => $"{AppSettings.Paths.Themes}Default/Backgrounds/Windows/LogicPOS_WorkFlow_{AppSettings.Culture.CurrentCultureName}.png";
        private System.Drawing.Image GetDashboardBackgroundImage() => System.Drawing.Image.FromFile(DashboardBackgroundImagePath);
        private Gdk.Pixbuf GetDashboardBackgroundPixbuf() => logicpos.Utils.ImageToPixbuf(GetDashboardBackgroundImage());

        private Gtk.Style GetDashboardBackgroundStyle()
        {
            var pixBuf = GetDashboardBackgroundPixbuf();
            Gdk.Pixmap pixmap = logicpos.Utils.PixbufToPixmap(pixBuf);

            if (pixmap != null)
            {
                Gtk.Style style = new Style();
                style.SetBgPixmap(StateType.Normal, pixmap);
                return style;
            }
            else
            {
                return null;
            }
        }

        private void ShowTotals(decimal? yearTotal=null)
        {
            var dayTotalSales = DashboardDataService.GetTotalSalesForDay(DateTime.Now);
            if (yearTotal == null)
            {
                LabelTotals.Text = $"{dayTotalSales.DayTotal:F2}\n\n{dayTotalSales.MonthTotal:F2}\n\n{dayTotalSales.YearTotal:F2} ";
            }
            else
            {
                LabelTotals.Text = $"{dayTotalSales.DayTotal:F2}\n\n{dayTotalSales.MonthTotal:F2}\n\n{yearTotal:F2} ";
            }
        }


        private void Design(Window parentWindow, dynamic themeWindow)
        {
            Frame.ShadowType = (ShadowType)0;
            ComboSalesYear.ModifyFg(StateType.Selected, Color.Black.ToGdkColor());

            InitializeEventBox();
            InitializeAlignment();

            try
            {
                EventBox.Style = GetDashboardBackgroundStyle();
                PlaceButtons(FixedContainer);

                InitializeLabelTotals();
                ShowTotals();

                Frame.Add(LabelTotals);
                FrameContainer.PackStart(Frame, false, false, 0);
                VBox1.PackStart(FrameContainer, false, false, 0);
                FixedContainer.Put(VBox1, 628, 515);

                DrawGraph(int.Parse(ComboSalesYear.ActiveText));
                FixedContainer.Put(GraphComponent, 55, 485);

                FixedContainer.Put(ComboSalesYear, 220, 665);
                EventBox.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));
                EventBox.Add(FixedContainer);
                FixedContainer.ModifyBg(StateType.Normal, new Gdk.Color(0, 0, 0));
            }
            catch (Exception ex)
            {
                CustomAlerts.ShowThemeRenderingErrorAlert($"Dashboard:\n\n{ex.Message}", parentWindow);
                Log.Logger.Error(ex, "Error rendering Dashboard page");
            }
        }

        private void InitializeLabelTotals()
        {
            LabelTotals.ModifyFont(Pango.FontDescription.FromString("Trebuchet MS 16"));
            LabelTotals.ModifyFg(StateType.Normal, Color.White.ToGdkColor());
            LabelTotals.Justify = Justification.Right;
        }

        private void InitializeAlignment()
        {
            Gtk.Alignment _alignmentWindow = new Alignment(0.0f, 0.0f, 0.0f, 0.0f)
            {
                EventBox
            };

            Add(_alignmentWindow);
        }

        private void PlaceButtons(Fixed container)
        {
            container.Put(BtnTerminals, 55, 62);
            container.Put(BtnPreferenceParameters, 55, 155);
            container.Put(BtnFiscalYears, 55, 250);
            container.Put(BtnPrinters, 55, 345);

            container.Put(BtnArticles, 245, 62);
            container.Put(BtnCustomers, 245, 155);
            container.Put(BtnUsers, 245, 250);
            container.Put(BtnTables, 245, 345);

            container.Put(BtnDocuments, 440, 62);
            container.Put(BtnNewDocument, 440, 155);
            container.Put(BtnPayments, 440, 250);
            container.Put(BtnArticleStock, 440, 345);

            container.Put(BtnReportsMenu, 635, 62);
            container.Put(BtnPrintReportRouter, 635, 155);
            container.Put(BtnCustomerBalanceDetails, 635, 250);
            container.Put(BtnSalesPerDate, 635, 345);
        }

        private void InitializeEventBox()
        {
            EventBox = new EventBox();
            EventBox.WidthRequest = BackOfficeWindow.ScreenSize.Width;
            EventBox.HeightRequest = BackOfficeWindow.ScreenSize.Height;
        }
    }
}
