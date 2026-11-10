using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

namespace LogicPOS.App.Views;

/// <summary>
/// Os quatro quadrados do logótipo logicPOS; o verde salta de quadrado em quadrado enquanto carrega.
/// </summary>
/// <remarks>
/// As animações correm no compositor (thread de render) para continuarem enquanto o UI thread está ocupado a carregar.
/// </remarks>
public partial class LogoLoader : UserControl
{
    private static readonly int[] HopOrder = [0, 1, 3, 2];
    private static readonly TimeSpan Cycle = TimeSpan.FromMilliseconds(420 * 4);
    private const float Fade = 0.04f;

    private readonly Panel[] _tiles;
    private bool _started;

    public LogoLoader()
    {
        InitializeComponent();
        _tiles = [Tile0, Tile1, Tile2, Tile3];
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        StartAnimations();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        _started = false;
        base.OnUnloaded(e);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        var side = Math.Min(e.NewSize.Width, e.NewSize.Height);
        Tiles.RowSpacing = Tiles.ColumnSpacing = Math.Max(2, side * 0.08);
        var radius = new CornerRadius(Math.Max(3, side * 0.11));
        foreach (var tile in _tiles)
        {
            foreach (var child in tile.Children)
            {
                if (child is Border border)
                {
                    border.CornerRadius = radius;
                }
            }
        }

        var tileSide = (side - Tiles.RowSpacing) / 2;
        foreach (var tile in _tiles)
        {
            if (ElementComposition.GetElementVisual(tile) is { } visual)
            {
                visual.CenterPoint = new Vector3D(tileSide / 2, tileSide / 2, 0);
            }
        }
    }

    private void StartAnimations()
    {
        if (_started)
        {
            return;
        }

        for (var slot = 0; slot < HopOrder.Length; slot++)
        {
            var tile = _tiles[HopOrder[slot]];
            var tileVisual = ElementComposition.GetElementVisual(tile);
            var greenVisual = ElementComposition.GetElementVisual(tile.Children[1]);
            if (tileVisual is null || greenVisual is null)
            {
                return;
            }

            tileVisual.CenterPoint = new Vector3D(tile.Bounds.Width / 2, tile.Bounds.Height / 2, 0);
            var start = slot / (float)HopOrder.Length;
            var end = (slot + 1) / (float)HopOrder.Length;
            greenVisual.StartAnimation("Opacity", CreateGreenAnimation(greenVisual.Compositor, start, end));
            tileVisual.StartAnimation("Scale", CreateHopAnimation(tileVisual.Compositor, start, end));
        }

        _started = true;
    }

    private static ScalarKeyFrameAnimation CreateGreenAnimation(Compositor compositor, float start, float end)
    {
        var animation = compositor.CreateScalarKeyFrameAnimation();
        animation.Duration = Cycle;
        animation.IterationBehavior = AnimationIterationBehavior.Forever;
        if (end >= 1f)
        {
            // Último salto: o verde desvanece no início do ciclo seguinte.
            animation.InsertKeyFrame(0f, 1f);
            animation.InsertKeyFrame(Fade, 0f);
            animation.InsertKeyFrame(start, 0f);
            animation.InsertKeyFrame(start + Fade, 1f);
            animation.InsertKeyFrame(1f, 1f);
            return animation;
        }

        animation.InsertKeyFrame(0f, 0f);
        if (start > 0f)
        {
            animation.InsertKeyFrame(start, 0f);
        }

        animation.InsertKeyFrame(start + Fade, 1f);
        animation.InsertKeyFrame(end, 1f);
        animation.InsertKeyFrame(end + Fade, 0f);
        animation.InsertKeyFrame(1f, 0f);
        return animation;
    }

    private static Vector3DKeyFrameAnimation CreateHopAnimation(Compositor compositor, float start, float end)
    {
        var animation = compositor.CreateVector3DKeyFrameAnimation();
        animation.Duration = Cycle;
        animation.IterationBehavior = AnimationIterationBehavior.Forever;
        var span = end - start;
        if (start > 0f)
        {
            animation.InsertKeyFrame(0f, new Vector3D(1, 1, 1));
            animation.InsertKeyFrame(start - 0.001f, new Vector3D(1, 1, 1));
        }

        animation.InsertKeyFrame(start, new Vector3D(0.82, 0.82, 1));
        animation.InsertKeyFrame(start + span * 0.35f, new Vector3D(0.92, 1.16, 1));
        animation.InsertKeyFrame(start + span * 0.6f, new Vector3D(1.12, 0.9, 1));
        animation.InsertKeyFrame(start + span * 0.8f, new Vector3D(0.97, 1.04, 1));
        animation.InsertKeyFrame(end, new Vector3D(1, 1, 1));
        if (end < 1f)
        {
            animation.InsertKeyFrame(1f, new Vector3D(1, 1, 1));
        }

        return animation;
    }
}
