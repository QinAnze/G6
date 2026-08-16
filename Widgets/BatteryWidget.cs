using System;
using System.Runtime.InteropServices;
using Avalonia.Media;
using GlassWidgets;

namespace GlassWidgets.Widgets;

/// <summary>电量：GetSystemPowerStatus P/Invoke。环弧按百分比填充，固定绿色强调色。</summary>
public class BatteryWidget : RingMetricWidget
{
    [StructLayout(LayoutKind.Sequential)]
    private struct SystemPowerStatus
    {
        public byte ACLineStatus;
        public byte BatteryFlag;
        public byte BatteryLifePercent;
        public byte SystemStatusFlag;
        public int BatteryLifeTime;
        public int BatteryFullLifeTime;
    }

    [DllImport("kernel32", SetLastError = true)]
    private static extern bool GetSystemPowerStatus(out SystemPowerStatus sps);

    public BatteryWidget() : base("GeoBattery", iconOffsetY: 9)
    {
        Gauge.ArcColor = Color.FromRgb(52, 199, 89); // 绿
        Timer.Interval = TimeSpan.FromSeconds(5);
    }

    protected override void Sample()
    {
        if (!GetSystemPowerStatus(out var s)) { Show(0); return; }
        if ((s.BatteryFlag & 128) != 0 || s.BatteryLifePercent == 255) { Show(100); return; }
        Show(s.BatteryLifePercent);
    }
}
