using Avalonia;

namespace GlassWidgets;

/// <summary>组件静态描述：显示名、SVG 图标资源键（Geometries.axaml）、默认尺寸。</summary>
public static class WidgetSpecs
{
    public static (string Display, string IconKey, Size DefaultSize) For(WidgetKind kind) => kind switch
    {
        WidgetKind.Clock => ("时间", "GeoClock", new Size(150, 150)),
        WidgetKind.Cpu => ("CPU", "GeoCpu", new Size(150, 150)),
        WidgetKind.Memory => ("内存", "GeoMemory", new Size(150, 150)),
        WidgetKind.Disk => ("磁盘", "GeoDisk", new Size(150, 150)),
        WidgetKind.Network => ("网络", "GeoNetwork", new Size(150, 150)),
        WidgetKind.Battery => ("电量", "GeoBattery", new Size(150, 150)),
        _ => ("?", "GeoCpu", new Size(150, 150)),
    };
}
