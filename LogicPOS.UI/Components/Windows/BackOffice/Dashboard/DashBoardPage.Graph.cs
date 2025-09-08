using Gtk;
using Medsphere.Widgets;
using Serilog;
using System;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage
    {
        private void HistogramPlot(Graph graph, DateTime date)
        {
            var treeStore = CreateHistogramTreeModel(date);
            HistogramPlot plot = new HistogramPlot(treeStore, PlotColor.Green)
            {
                Name = "Vendas por Mês",
                ShowValues = true
            };

            plot.SetValueDataColumn(0, 0);
            plot.SetValueDataColumn(1, 1);
            graph.AddPlot(plot, graph.Axes);
        }

        private Widget DrawGraph(DateTime date, bool cleanGraph)
        {
            if (cleanGraph)
            {
                Graph.Clear();
            }

            HBox hboxGraph = new HBox(false, 0);
            DateTimeAxis dtA = new DateTimeAxis(0, AxisLocation.Bottom);
            dtA.Padding = 5;
            dtA.ShowGridLines = false;
            dtA.ShowTicks = true;
            dtA.ShowTickLabels = true;

            Graph.AppendAxis(dtA);
            Graph.AppendAxis(new LinearAxis(1, AxisLocation.Left));
            HistogramPlot(Graph, date);
            Graph.CreatePangoContext();
            Graph.ModifyBg(StateType.Normal, new Gdk.Color(218, 218, 218));
            Graph.ModifyFg(StateType.Normal, new Gdk.Color(100, 100, 100));
            Graph.WidthRequest = 515;
            Graph.HeightRequest = 170;
            hboxGraph.PackStart(Graph, false, false, 0);
            return hboxGraph;
        }

        public TreeStore CreateHistogramTreeModel(DateTime year)
        {
            TreeStore treeStore = new TreeStore(typeof(DateTime), typeof(double));

            Random random = new Random();

            try
            {
                for (int i = 1; i <= 12; i++)
                {
                    DateTime saleDAte = new DateTime(year.Year, i, 1);
                    treeStore.AppendValues(saleDAte,Convert.ToDouble(random.Next(50000)));
                }
                return treeStore;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex,"Error while creating dashboard histogram tree model");
                return null;
            }
        }
    }
}
