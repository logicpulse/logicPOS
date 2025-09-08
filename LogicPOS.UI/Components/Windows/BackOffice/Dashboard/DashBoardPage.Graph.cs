using Gtk;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Components.Windows.BackOffice.Dashboard;
using Medsphere.Widgets;
using Serilog;
using System;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage
    {
        private void HistogramPlot(Graph graph, int year)
        {
            var treeStore = CreateHistogramTreeModel(year);
            HistogramPlot plot = new HistogramPlot(treeStore, PlotColor.Green)
            {
                Name = "Vendas por Mês",
                ShowValues = true
            };

            plot.SetValueDataColumn(0, 0);
            plot.SetValueDataColumn(1, 1);
            graph.AddPlot(plot, graph.Axes);
        }

        private void DrawGraph(int year)
        {
            Graph.Clear();

            foreach (var item in GraphComponent.Children)
            {
                GraphComponent.Remove(item);
            }
           
            DateTimeAxis dateTimeAxis = new DateTimeAxis(0, AxisLocation.Bottom);

            dateTimeAxis.Padding = 5;
            dateTimeAxis.ShowGridLines = true;
            dateTimeAxis.ShowTicks = true;
            dateTimeAxis.ShowTickLabels = true;

            Graph.AppendAxis(dateTimeAxis);
            Graph.AppendAxis(new LinearAxis(1, AxisLocation.Left));
            HistogramPlot(Graph, year);
            Graph.CreatePangoContext();
            Graph.ModifyBg(StateType.Normal, new Gdk.Color(218, 218, 218));
            Graph.ModifyFg(StateType.Normal, new Gdk.Color(100, 100, 100));
            Graph.WidthRequest = 515;
            Graph.HeightRequest = 170;
            GraphComponent.PackStart(Graph, false, false, 0);
        }

        public TreeStore CreateHistogramTreeModel(int year)
        {
            TreeStore treeStore = new TreeStore(typeof(DateTime), typeof(double));

            try
            {
                var data = DashboardDataService.GetMonthlySalesReportData(year);

                foreach (var monthSale in data.Sales)
                {
                    treeStore.AppendValues(new DateTime(year, monthSale.Month, 1), Convert.ToDouble(Math.Round(monthSale.FinalTotal, 2)));
                }

                return treeStore;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error while creating dashboard histogram tree model");
                CustomAlerts.Error(BackOfficeWindow.Instance).WithMessage($"Erro ao carregar dados do gráfico de vendas:\n\n{ex.Message}").ShowAlert();
                return null;
            }
        }
    }
}
