using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace GlassWidgets.Controls;

/// <summary>
/// 环形仪表（规范 §10）：retained-mode 画背景整环 + 按值扫过的弧 + 端点。
/// 数值用 CubicEaseOut 补间（进入场从 0 缓动），负载语义色或自定义 ArcColor 用于描边（绝不用于文字）。
/// </summary>
public class RingGauge : Control
{
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<RingGauge, double>(nameof(Value), 0, coerce: Coerce01);
    public static readonly StyledProperty<double> ThicknessProperty =
        AvaloniaProperty.Register<RingGauge, double>(nameof(Thickness), 9);
    public static readonly StyledProperty<double> WarnProperty =
        AvaloniaProperty.Register<RingGauge, double>(nameof(Warn), 60);
    public static readonly StyledProperty<double> HotProperty =
        AvaloniaProperty.Register<RingGauge, double>(nameof(Hot), 85);
    public static readonly StyledProperty<bool> InvertColorProperty =
        AvaloniaProperty.Register<RingGauge, bool>(nameof(InvertColor), false);
    public static readonly StyledProperty<Color?> ArcColorProperty =
        AvaloniaProperty.Register<RingGauge, Color?>(nameof(ArcColor), null);

    public double Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public double Thickness { get => GetValue(ThicknessProperty); set => SetValue(ThicknessProperty, value); }
    public double Warn { get => GetValue(WarnProperty); set => SetValue(WarnProperty, value); }
    public double Hot { get => GetValue(HotProperty); set => SetValue(HotProperty, value); }
    public bool InvertColor { get => GetValue(InvertColorProperty); set => SetValue(InvertColorProperty, value); }
    public Color? ArcColor { get => GetValue(ArcColorProperty); set => SetValue(ArcColorProperty, value); }

    private static double Coerce01(AvaloniaObject _, double v) => Math.Clamp(v, 0, 100);

    private double _shown;
    private double _from, _to;
    private DateTime _start;
    private readonly DispatcherTimer _tween = new() { Interval = TimeSpan.FromMilliseconds(16) };

    public RingGauge()
    {
        _shown = Value;
        _tween.Tick += Tick;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValueProperty) BeginTween((double)change.NewValue!);
        else if (change.Property == ThicknessProperty || change.Property == WarnProperty ||
                 change.Property == HotProperty || change.Property == InvertColorProperty ||
                 change.Property == ArcColorProperty) InvalidateVisual();
    }

    private void BeginTween(double to)
    {
        _from = _shown; _to = to; _start = DateTime.Now;
        if (!_tween.IsEnabled) _tween.Start();
    }

    private void Tick(object? _, EventArgs e)
    {
        var t = (DateTime.Now - _start).TotalMilliseconds / 280;
        if (t >= 1) { _shown = _to; _tween.Stop(); }
        else { var k = 1 - Math.Pow(1 - t, 3); _shown = _from + (_to - _from) * k; }  // CubicEaseOut
        InvalidateVisual();
    }

    public override void Render(DrawingContext c)
    {
        var w = Bounds.Width; var h = Bounds.Height;
        if (w <= 1 || h <= 1) return;
        var r = Math.Min(w, h) / 2 - Thickness / 2 - 1;
        var cx = w / 2; var cy = h / 2;

        // 背景整环（纯白低透明度，符合纯黑白体系）
        c.DrawGeometry(null,
            new Pen { Brush = new SolidColorBrush(Color.FromArgb(36, 255, 255, 255)), Thickness = Thickness,
                LineCap = PenLineCap.Round, LineJoin = PenLineJoin.Round },
            ArcGeometry(cx, cy, r, 0, 360));

        // 值弧（从顶部 270° 顺时针扫过 value%）
        if (_shown > 0.5)
        {
            var col = ArcColor ?? (InvertColor ? InvertStatusColor(_shown, Warn, Hot) : StatusColor(_shown, Warn, Hot));
            c.DrawGeometry(null,
                new Pen { Brush = new SolidColorBrush(col), Thickness = Thickness,
                    LineCap = PenLineCap.Round, LineJoin = PenLineJoin.Round },
                ArcGeometry(cx, cy, r, 270, _shown / 100.0 * 360));
        }
    }

    private static StreamGeometry ArcGeometry(double cx, double cy, double r, double startDeg, double sweepDeg)
    {
        var geo = new StreamGeometry();
        using var ctx = geo.Open();
        ctx.BeginFigure(Polar(cx, cy, r, startDeg), false);
        var remaining = sweepDeg;
        var cur = startDeg;
        while (remaining > 0.01)
        {
            var seg = Math.Min(remaining, 180);
            ctx.ArcTo(Polar(cx, cy, r, cur + seg), new Size(r, r), 0, false, SweepDirection.Clockwise, true);
            cur += seg;
            remaining -= seg;
        }
        return geo;
    }

    private static Point Polar(double cx, double cy, double r, double deg)
    {
        var rad = deg * Math.PI / 180;
        return new Point(cx + r * Math.Cos(rad), cy + r * Math.Sin(rad));
    }

    internal static Color StatusColor(double v, double warn, double hot)
    {
        if (v >= hot) return Color.FromRgb(255, 95, 86);
        if (v >= warn) return Color.FromRgb(255, 191, 46);
        return Color.FromRgb(52, 199, 89);
    }

    internal static Color InvertStatusColor(double v, double warn, double hot)
    {
        if (v <= hot) return Color.FromRgb(255, 95, 86);
        if (v <= warn) return Color.FromRgb(255, 191, 46);
        return Color.FromRgb(52, 199, 89);
    }
}
