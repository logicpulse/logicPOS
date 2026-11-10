using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace LogicPOS.App.Views;

/// <summary>
/// Os quatro quadrados do logótipo logicPOS; o verde salta de quadrado em quadrado enquanto carrega.
/// </summary>
public partial class LogoLoader : UserControl
{
    private static readonly int[] HopOrder = [0, 1, 3, 2];

    private readonly Border[] _tiles;
    private readonly DispatcherTimer _timer;
    private int _step;

    public LogoLoader()
    {
        InitializeComponent();
        _tiles = [Tile0, Tile1, Tile2, Tile3];
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(420) };
        _timer.Tick += (_, _) => Hop();
        SetActive(HopOrder[0]);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _timer.Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _timer.Stop();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        var side = Math.Min(e.NewSize.Width, e.NewSize.Height);
        Tiles.RowSpacing = Tiles.ColumnSpacing = Math.Max(2, side * 0.08);
        var radius = new CornerRadius(Math.Max(3, side * 0.11));
        foreach (var tile in _tiles)
        {
            tile.CornerRadius = radius;
        }
    }

    private void Hop()
    {
        if (IsEffectivelyVisible == false)
        {
            return;
        }

        _step = (_step + 1) % HopOrder.Length;
        SetActive(HopOrder[_step]);
    }

    private void SetActive(int index)
    {
        for (var i = 0; i < _tiles.Length; i++)
        {
            _tiles[i].Classes.Set("on", i == index);
        }
    }
}