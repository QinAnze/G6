using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace GlassWidgets.Controls;

/// <summary>
/// 单块玻璃板（规范 §5）：retained-mode 控制，只负责画毛玻璃质感（低不透明度中性 tint + 发丝描边 + 顶部柔光）。
/// 真正的模糊来自窗口级 AcrylicBlur（不在本控件叠 BlurEffect）。画刷按 (tint,opacity,gloss) 缓存，属性变才重绘。
/// </summary>
public class GlassSurface : Control
{
    public static readonly StyledProperty<Color> TintColorProperty =
        AvaloniaProperty.Register<GlassSurface, Color>(nameof(TintColor), Color.FromRgb(34, 34, 38));
    public static readonly StyledProperty<double> TintOpacityProperty =
        AvaloniaProperty.Register<GlassSurface, double>(nameof(TintOpacity), 0.55);
    public static readonly StyledProperty<double> GlossStrengthProperty =
        AvaloniaProperty.Register<GlassSurface, double>(nameof(GlossStrength), 1.0);
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<GlassSurface, CornerRadius>(nameof(CornerRadius), new CornerRadius(16));

    public Color TintColor { get => GetValue(TintColorProperty); set => SetValue(TintColorProperty, value); }
    public double TintOpacity { get => GetValue(TintOpacityProperty); set => SetValue(TintOpacityProperty, value); }
    public double GlossStrength { get => GetValue(GlossStrengthProperty); set => SetValue(GlossStrengthProperty, value); }
    public CornerRadius CornerRadius { get => GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }

    public GlassSurface() => IsHitTestVisible = false;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TintColorProperty || change.Property == TintOpacityProperty ||
            change.Property == GlossStrengthProperty || change.Property == CornerRadiusProperty)
            InvalidateVisual();
    }

    private Color _cachedTint;
    private byte _cachedA;
    private double _cachedGloss = -1;
    private LinearGradientBrush? _bodyBrush;
    private LinearGradientBrush? _sheenBrush;

    private void EnsureBrushes(Color tint, byte baseA, double gloss)
    {
        if (_bodyBrush != null && _cachedTint == tint && _cachedA == baseA &&
            Math.Abs(_cachedGloss - gloss) < 0.01) return;
        _cachedTint = tint; _cachedA = baseA; _cachedGloss = gloss;

        var top = Lighten(tint, 0.08);
        _bodyBrush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops = new GradientStops
            {
                new GradientStop(Color.FromArgb(baseA, top.R, top.G, top.B), 0),
                new GradientStop(Color.FromArgb(baseA, tint.R, tint.G, tint.B), 1),
            }
        };
        _sheenBrush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 0.22, RelativeUnit.Relative),
            GradientStops = new GradientStops
            {
                new GradientStop(Color.FromArgb((byte)(40 * gloss), 255, 255, 255), 0),
                new GradientStop(Color.FromArgb(0, 255, 255, 255), 1),
            }
        };
    }

    public override void Render(DrawingContext context)
    {
        var rect = new Rect(0, 0, Bounds.Width, Bounds.Height);
        if (rect.Width <= 1 || rect.Height <= 1) return;
        var gloss = GlossStrength;
        var baseA = (byte)Math.Clamp(TintOpacity * 255, 0, 255);
        EnsureBrushes(TintColor, baseA, gloss);

        // 画满窗口矩形；圆角由 OS DWM 统一提供，这里不画独立圆角/描边，避免两层弧线。
        context.DrawRectangle(_bodyBrush, null, rect);  // 磨砂体（透出模糊）
        context.DrawRectangle(_sheenBrush, null, rect); // 顶部柔光（静态）
    }

    private static Color Lighten(Color c, double amt)
    {
        byte Mix(double v) => (byte)Math.Clamp(v + (255 - v) * amt, 0, 255);
        return Color.FromArgb(c.A, Mix(c.R), Mix(c.G), Mix(c.B));
    }
}
