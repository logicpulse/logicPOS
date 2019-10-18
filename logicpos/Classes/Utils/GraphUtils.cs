using System;
using Gtk;

using Medsphere.Widgets;

namespace logicpos
{
    public class GraphUtils 
    {
        public GraphUtils() 
        {
            VBox vbox = new VBox(false, 0);

            //Add(vbox);

            graph = CreateRandomGraph();

            vbox.PackStart(graph, true, true, 0);

            vbox.PackStart(new HSeparator(), false, true, 0);

            HBox hbox = new HBox(false, 0);

            Button randomizer = new Button();
            randomizer.Label = "New random view";
            randomizer.Clicked += new EventHandler(OnRandomizerClicked);

            hbox.PackStart(randomizer, false, false, 0);

            Label rightSpace = new Label(String.Empty);

            hbox.Add(rightSpace);

            vbox.PackEnd(hbox, false, true, 0);
        }

        private Graph graph;

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

        public PointShape[] pointShapes = new PointShape[] {
            PointShape.Circle,
            PointShape.Square,
            PointShape.Diamond,
            PointShape.Triangle
        };

        public Graph CreateRandomGraph()
        {
            Graph newGraph = new Graph2D();
            newGraph.AppendAxis(new DateTimeAxis(0, AxisLocation.Bottom));
            newGraph.AppendAxis(new LinearAxis(1, AxisLocation.Left));

            AddRandomPlot(newGraph);

            return newGraph;
        }

        private void AddRandomPlot(Graph graph)
        {
            Random random = new Random();

            int typeOfGraph = random.Next(3);

            switch (typeOfGraph)
            {
                default:
                case 0:
                    // default
                    AddRandomLinePlot(graph);
                    break;
                case 1:
                    AddRandomHistogramPlot(graph);
                    break;
                case 2:
                    AddRandomLinkedLinePlot(graph);
                    break;
            }

            if (Convert.ToBoolean(random.Next(2)))
            {
                AddRandomReferenceRangePlot(graph);
            }

            graph.RecalculateAxisRange();
        }

        public void AddRandomLinePlot(Graph graph)
        {
            Random random = new Random();

            LinePlot plot = new LinePlot(
                CreateRandomModel(),
                GetRandomPlotColor(),
                GetRandomPointShape());

            plot.Name = "LinePlot";
            plot.ShowValues = Convert.ToBoolean(random.Next(2));

            plot.SetValueDataColumn(0, 0);
            plot.SetValueDataColumn(1, 1);

            graph.AddPlot(plot, graph.Axes);
        }

        public void AddRandomHistogramPlot(Graph graph)
        {
            Random random = new Random();

            HistogramPlot plot = new HistogramPlot(
                CreateRandomModel(),
                GetRandomPlotColor());

            plot.Name = "HistogramPlot";
            plot.ShowValues = Convert.ToBoolean(random.Next(2));

            plot.SetValueDataColumn(0, 0);
            plot.SetValueDataColumn(1, 1);

            graph.AddPlot(plot, graph.Axes);
        }

        public void AddRandomLinkedLinePlot(Graph graph)
        {
            Random random = new Random();

            LinePlot[] linePlots = new LinePlot[3];
            for (int i = 0; i < linePlots.Length; i++)
            {

                linePlots[i] = new LinePlot(
                    CreateRandomModel(),
                    GetRandomPlotColor(),
                    GetRandomPointShape());

                linePlots[i].Name = "LinePlot[" + i.ToString() + "]";
                linePlots[i].ShowValues = Convert.ToBoolean(random.Next(2));

                linePlots[i].SetValueDataColumn(0, 0);
                linePlots[i].SetValueDataColumn(1, 1);
            }

            LinkedLinePlot plot = new LinkedLinePlot(linePlots);

            plot.Name = "LinkedLinePlot";

            graph.AddPlot(plot, graph.Axes);
        }

        public void AddRandomReferenceRangePlot(Graph graph)
        {
            Random random = new Random();

            double lower = 10.0 + (double)random.Next(20);
            double upper = 60.0 + (double)random.Next(30);

            ReferenceRangePlot plot = new ReferenceRangePlot(lower, upper, 1,
                GetRandomPlotColor());

            graph.AddPlot(plot, graph.Axes);
        }

        public TreeStore CreateRandomModel()
        {
            TreeStore store = new TreeStore(typeof(DateTime), typeof(double));

            double[] numbers = CreateSomeNumbers();
            DateTime[] dateTimes = CreateDateTimeSequence(numbers.Length);

            for (int i = 0; i < numbers.Length; i++)
            {
                store.AppendValues(dateTimes[i], numbers[i]);
            }

            return store;
        }

        public double[] CreateSomeNumbers()
        {
            Random random = new Random();

            double[] numbers = new double[30*6];
            double k = 0.5 + 0.1 * random.Next(4);
            double x;

            for (int i = 0; i < numbers.Length; i++)
            {
                x = 0.5 + 0.5 * i;
                numbers[i] = Math.Round(Math.Sin(k * x) / (k * x), 2);
                numbers[i] = (numbers[i] + 0.3) * 70.0;
            }

            return numbers;
        }

        public DateTime[] CreateDateTimeSequence(int count)
        {
            DateTime[] dateTimes = new DateTime[count];

            // define date range by its last day equal to today and an
            // one day interval
            dateTimes[dateTimes.Length - 1] = DateTime.Now;
            for (int i = dateTimes.Length - 2; i >= 0; i--)
            {
                dateTimes[i] = dateTimes[i + 1].Subtract(TimeSpan.FromDays(1.0));
            }

            return dateTimes;
        }

        public PlotColor GetRandomPlotColor()
        {
            Random random = new Random();
            int i = random.Next(plotColors.Length);
            return plotColors[i];
        }

        public PointShape GetRandomPointShape()
        {
            Random random = new Random();
            int i = random.Next(pointShapes.Length);
            return pointShapes[i];
        }

        public void RemovePlot(Graph graph)
        {
            foreach (IPlot plot in graph.Plots)
            {
                graph.RemovePlot(plot);
            }
        }

        public void OnRandomizerClicked(object sender, EventArgs args)
        {
            RemovePlot(graph);
            AddRandomPlot(graph);
        }
    }
}