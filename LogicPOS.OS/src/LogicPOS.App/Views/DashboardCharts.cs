using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace LogicPOS.App.Views;

/// <summary>Gives every dashboard chart a real height inside the page stack.</summary>
public abstract class DashboardChartControl : Control
{
    protected DashboardChartControl(double height)
    {
        Height = height;
        MinHeight = height;
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsInfinity(availableSize.Width) || availableSize.Width <= 0 ? 480 : availableSize.Width;
        return new Size(width, Height);
    }
}

/// <summary>Green column chart used by the billing dashboard.</summary>
public sealed class ColumnChart : DashboardChartControl
{
    public ColumnChart()
        : base(220)
    {
    }

    private IReadOnlyList<DashboardChartPoint> _points = [];

    public void SetPoints(IReadOnlyList<DashboardChartPoint> points)
    {
        _points = points;
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        var bounds = new Rect(Bounds.Size);
        if (bounds.Width < 40 || bounds.Height < 40 || _points.Count == 0)
        {
            return;
        }

        var plot = new Rect(36, 8, Math.Max(10, bounds.Width - 44), Math.Max(10, bounds.Height - 32));
        var max = Math.Max(1d, _points.Max(point => point.Value) * 1.15);
        DrawGrid(context, plot, max);
        var slot = plot.Width / _points.Count;
        var barWidth = Math.Max(8, slot * 0.55);
        for (var index = 0; index < _points.Count; index++)
        {
            var point = _points[index];
            var height = plot.Height * (point.Value / max);
            var x = plot.X + (slot * index) + ((slot - barWidth) / 2);
            var y = plot.Bottom - height;
            context.DrawRectangle(ChartBrushes.Column, null, new Rect(x, y, barWidth, Math.Max(height, 0)), 5, 5);
            DrawLabel(context, point.Label, new Point(plot.X + (slot * index) + (slot / 2), plot.Bottom + 4), 10, ChartBrushes.Axis, center: true);
        }
    }

    private void DrawGrid(DrawingContext context, Rect plot, double max)
    {
        var pen = new Pen(ChartBrushes.Grid, 1);
        for (var step = 0; step <= 4; step++)
        {
            var y = plot.Bottom - (plot.Height * step / 4);
            context.DrawLine(pen, new Point(plot.X, y), new Point(plot.Right, y));
            var value = max * step / 4;
            DrawLabel(context, value.ToString("0.00", CultureInfo.GetCultureInfo("pt-PT")), new Point(0, y - 7), 10, ChartBrushes.Axis, center: false);
        }
    }

    private void DrawLabel(DrawingContext context, string text, Point origin, double size, IBrush brush, bool center)
    {
        var formatted = new FormattedText(text, CultureInfo.GetCultureInfo("pt-PT"), FlowDirection.LeftToRight, Typeface.Default, size, brush);
        var x = center ? origin.X - (formatted.Width / 2) : origin.X;
        context.DrawText(formatted, new Point(x, origin.Y));
    }
}

/// <summary>Area line of the twelve monthly totals.</summary>
public sealed class AreaChart : DashboardChartControl
{
    public AreaChart()
        : base(220)
    {
    }

    private IReadOnlyList<DashboardChartPoint> _points = [];

    public void SetPoints(IReadOnlyList<DashboardChartPoint> points)
    {
        _points = points;
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        var bounds = new Rect(Bounds.Size);
        if (bounds.Width < 40 || bounds.Height < 40 || _points.Count == 0)
        {
            return;
        }

        var plot = new Rect(36, 8, Math.Max(10, bounds.Width - 44), Math.Max(10, bounds.Height - 32));
        var max = Math.Max(1d, _points.Max(point => point.Value) * 1.15);
        DrawGrid(context, plot, max);
        var step = _points.Count == 1 ? 0 : plot.Width / (_points.Count - 1);
        var markers = new Point[_points.Count];
        for (var index = 0; index < _points.Count; index++)
        {
            var y = plot.Bottom - (plot.Height * (_points[index].Value / max));
            markers[index] = new Point(plot.X + (step * index), y);
        }

        var fill = new StreamGeometry();
        using (var geometry = fill.Open())
        {
            geometry.BeginFigure(new Point(markers[0].X, plot.Bottom), true);
            foreach (var marker in markers)
            {
                geometry.LineTo(marker);
            }

            geometry.LineTo(new Point(markers[^1].X, plot.Bottom));
            geometry.EndFigure(true);
        }

        context.DrawGeometry(ChartBrushes.Area, null, fill);
        var line = new Pen(ChartBrushes.Line, 2.5);
        for (var index = 1; index < markers.Length; index++)
        {
            context.DrawLine(line, markers[index - 1], markers[index]);
        }

        foreach (var marker in markers)
        {
            context.DrawEllipse(ChartBrushes.Line, null, marker, 3.5, 3.5);
        }

        for (var index = 0; index < _points.Count; index++)
        {
            DrawLabel(context, _points[index].Label, new Point(markers[index].X, plot.Bottom + 4), 10, ChartBrushes.Axis, center: true);
        }
    }

    private void DrawGrid(DrawingContext context, Rect plot, double max)
    {
        var pen = new Pen(ChartBrushes.Grid, 1);
        for (var step = 0; step <= 4; step++)
        {
            var y = plot.Bottom - (plot.Height * step / 4);
            context.DrawLine(pen, new Point(plot.X, y), new Point(plot.Right, y));
            var value = max * step / 4;
            DrawLabel(context, value.ToString("0.00", CultureInfo.GetCultureInfo("pt-PT")), new Point(0, y - 7), 10, ChartBrushes.Axis, center: false);
        }
    }

    private void DrawLabel(DrawingContext context, string text, Point origin, double size, IBrush brush, bool center)
    {
        var formatted = new FormattedText(text, CultureInfo.GetCultureInfo("pt-PT"), FlowDirection.LeftToRight, Typeface.Default, size, brush);
        var x = center ? origin.X - (formatted.Width / 2) : origin.X;
        context.DrawText(formatted, new Point(x, origin.Y));
    }
}

/// <summary>Donut of the four quarters, with a flat ring when every quarter is zero.</summary>
public sealed class DonutChart : DashboardChartControl
{
    public DonutChart()
        : base(200)
    {
    }

    private static readonly IBrush[] Palette =
    [
        ChartBrushes.Column,
        new SolidColorBrush(Color.Parse("#6B9E2E")),
        ChartBrushes.Line,
        new SolidColorBrush(Color.Parse("#A8D96A"))
    ];

    private IReadOnlyList<DashboardChartPoint> _points = [];

    public void SetPoints(IReadOnlyList<DashboardChartPoint> points)
    {
        _points = points;
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        var bounds = new Rect(Bounds.Size);
        if (bounds.Width < 40 || bounds.Height < 40)
        {
            return;
        }

        var side = Math.Min(bounds.Width, bounds.Height) - 8;
        var plot = new Rect((bounds.Width - side) / 2, 4, side, side);
        var center = plot.Center;
        var radius = (side / 2) - 14;
        var total = _points.Sum(point => Math.Max(0, point.Value));
        if (total <= 0)
        {
            context.DrawEllipse(null, new Pen(ChartBrushes.EmptyRing, 18), center, radius, radius);
            return;
        }

        var start = -90d;
        for (var index = 0; index < _points.Count; index++)
        {
            var sweep = _points[index].Value <= 0 ? 0 : _points[index].Value / total * 360d;
            if (sweep >= 359.9)
            {
                context.DrawEllipse(null, new Pen(Palette[index % Palette.Length], 18), center, radius, radius);
                return;
            }

            DrawArc(context, center, radius, start, sweep, Palette[index % Palette.Length]);
            start += sweep;
        }
    }

    private static void DrawArc(DrawingContext context, Point center, double radius, double start, double sweep, IBrush brush)
    {
        if (sweep <= 0.1)
        {
            return;
        }

        var geometry = new StreamGeometry();
        using (var figure = geometry.Open())
        {
            figure.BeginFigure(PointOn(center, radius, start), false);
            figure.ArcTo(PointOn(center, radius, start + sweep), new Size(radius, radius), 0, sweep > 180, SweepDirection.Clockwise);
        }

        context.DrawGeometry(null, new Pen(brush, 18), geometry);
    }

    private static Point PointOn(Point center, double radius, double angle)
    {
        var radians = angle * Math.PI / 180d;
        return new Point(center.X + (radius * Math.Cos(radians)), center.Y + (radius * Math.Sin(radians)));
    }
}

public sealed class DashboardChartPoint
{
    public string Label { get; init; } = string.Empty;

    public double Value { get; init; }
}

internal static class ChartBrushes
{
    public static IBrush Column { get; } = new SolidColorBrush(Color.Parse("#8CC63F"));

    public static IBrush Area { get; } = new SolidColorBrush(Color.Parse("#598CC63F"));

    public static IBrush Line { get; } = new SolidColorBrush(Color.Parse("#4D6610"));

    public static IBrush Grid { get; } = new SolidColorBrush(Color.Parse("#ECECEC"));

    public static IBrush Axis { get; } = new SolidColorBrush(Color.Parse("#8A8A8A"));

    public static IBrush EmptyRing { get; } = new SolidColorBrush(Color.Parse("#E6E6E6"));
}
