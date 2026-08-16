using System.Diagnostics;
using Avalonia.Media;
using GlassWidgets;

namespace GlassWidgets.Widgets;

/// <summary>磁盘活动时间：% Disk Time (_Total)，同样 prime 后再采样。</summary>
public class DiskWidget : RingMetricWidget
{
    private PerformanceCounter? _c;

    public DiskWidget() : base("GeoDisk")
    {
        Gauge.Warn = 80;
        Gauge.Hot = 95;
        Gauge.ArcColor = Color.FromRgb(240, 180, 70); // 琥珀
        Timer.Interval = TimeSpan.FromSeconds(1);
    }

    protected override void OnAttach()
    {
        _c = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total", true);
        _c.NextValue();
    }

    protected override void Sample()
    {
        if (_c == null) return;
        Show(_c.NextValue());
    }

    protected override void OnDetach() { _c?.Dispose(); _c = null; }
}
