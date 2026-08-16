using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace GlassWidgets.Controls;

/// <summary>
/// 环状抽象时钟（规范 §10 可视化范式）：retained-mode 画表圈 + 12 刻度 + 时分秒针。
/// 仅改重绘、不动布局；1s 节流重绘。指针纯白、不同粗细/透明度表达层级。
/// </summary>
public class RingClock : Control
{
    private DateTime _now;
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };

    public RingClock()
    {
        _now = DateTime.Now;
        _timer.Tick += (_, _) => { _now = DateTime.Now; InvalidateVisual(); };
        _timer.Start();
    }

    public override void Render(DrawingContext c)
    {
        var w = Bounds.Width; var h = Bounds.Height;
        if (w <= 1 || h <= 1) return;
        var cx = w / 2; var cy = h / 2;
        var r = Math.Min(w, h) / 2 - 6;

        // 表圈
        c.DrawGeometry(null,
            new Pen { Brush = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)), Thickness = 2,
                LineCap = PenLineCap.Round, LineJoin = PenLineJoin.Round },
            ArcGeometry(cx, cy, r, 0, 360));

        // 12 刻度
        for (var i = 0; i < 12; i++)
        {
            var isHour = i % 3 == 0;
            var outer = Polar(cx, cy, r - 3, i * 30 - 90);
            var inner = Polar(cx, cy, r - (isHour ? 12 : 8), i * 30 - 90);
            c.DrawLine(new Pen { Brush = new SolidColorBrush(Color.FromArgb((byte)(isHour ? 200 : 110), 255, 255, 255)),
                Thickness = isHour ? 2 : 1.2, LineCap = PenLineCap.Round, LineJoin = PenLineJoin.Round }, inner, outer);
        }

        var sec = _now.Second + _now.Millisecond / 1000.0;
        var min = _now.Minute + sec / 60.0;
        var hour = (_now.Hour % 12) + min / 60.0;

        DrawHand(c, cx, cy, r * 0.5, hour / 12 * 360 - 90, 175, 3.2);   // 时针
        DrawHand(c, cx, cy, r * 0.72, min / 60 * 360 - 90, 225, 2.2);   // 分针
        DrawHand(c, cx, cy, r * 0.82, sec / 60 * 360 - 90, 255, 1.2);   // 秒针

        // 中心圆点
        c.DrawGeometry(new SolidColorBrush(Colors.White), null,
            new EllipseGeometry(new Rect(cx - 2.5, cy - 2.5, 5, 5)));
    }

    private static void DrawHand(DrawingContext c, double cx, double cy, double len, double deg, byte alpha, double thick)
    {
        c.DrawLine(new Pen { Brush = new SolidColorBrush(Color.FromArgb(alpha, 255, 255, 255)), Thickness = thick,
            LineCap = PenLineCap.Round, LineJoin = PenLineJoin.Round }, new Point(cx, cy), Polar(cx, cy, len, deg));
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
}
