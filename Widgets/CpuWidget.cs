using System.Diagnostics;
using Avalonia.Media;
using GlassWidgets;

namespace GlassWidgets.Widgets;

/// <summary>CPU 使用率：% Processor Time (_Total)。挂载时 prime 一次，每秒采样（修复“0/100 跳变”）。</summary>
public class CpuWidget : RingMetricWidget
{
    private PerformanceCounter? _c;

    public CpuWidget() : base("GeoCpu")
    {
        Gauge.Warn = 60;
        Gauge.Hot = 85;
        Gauge.ArcColor = Color.FromRgb(64, 200, 224); // 青
        Timer.Interval = TimeSpan.FromSeconds(1);
    }

    protected override void OnAttach()
    {
        _c = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
        _c.NextValue(); // prime，丢弃首值
    }

    protected override void Sample()
    {
        if (_c == null) return;
        Show(_c.NextValue());
    }

    protected override void OnDetach() { _c?.Dispose(); _c = null; }
}
