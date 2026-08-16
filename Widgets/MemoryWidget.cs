using System.Diagnostics;
using Avalonia.Media;
using GlassWidgets;

namespace GlassWidgets.Widgets;

/// <summary>内存使用率：% Committed Bytes In Use。</summary>
public class MemoryWidget : RingMetricWidget
{
    private PerformanceCounter? _c;

    public MemoryWidget() : base("GeoMemory")
    {
        Gauge.Warn = 70;
        Gauge.Hot = 88;
        Gauge.ArcColor = Color.FromRgb(150, 120, 240); // 紫
        Timer.Interval = TimeSpan.FromSeconds(1);
    }

    protected override void OnAttach()
    {
        _c = new PerformanceCounter("Memory", "% Committed Bytes In Use", true);
        _c.NextValue();
    }

    protected override void Sample()
    {
        if (_c == null) return;
        Show(_c.NextValue());
    }

    protected override void OnDetach() { _c?.Dispose(); _c = null; }
}
