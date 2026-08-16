using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using GlassWidgets.Controls;
using GlassWidgets.Services;

namespace GlassWidgets.Widgets;

/// <summary>
/// 环状指标组件基类（规范 §10）：单块内容 = 环形仪表 + 环中央 SVG 图标，**无文字**。
/// 定时采样并带异常兜底（单组件失败仅停用自身，不拖垮进程）。
/// </summary>
public abstract class RingMetricWidget : UserControl
{
    protected readonly RingGauge Gauge = new();
    protected readonly DispatcherTimer Timer = new();

    private readonly string _iconKey;

    protected RingMetricWidget(string iconKey, double ringSize = 112, double iconOffsetY = 4)
    {
        _iconKey = iconKey;

        Gauge.Width = ringSize;
        Gauge.Height = ringSize;
        Gauge.HorizontalAlignment = HorizontalAlignment.Center;
        Gauge.VerticalAlignment = VerticalAlignment.Center;

        var icon = new Avalonia.Controls.Shapes.Path
        {
            Classes = { "icon" },
            Data = (StreamGeometry)(Application.Current?.FindResource(iconKey) ?? new StreamGeometry()),
            Width = 30,
            Height = 30,
            Opacity = 0.95,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Stretch = Stretch.Uniform,
            RenderTransform = new TranslateTransform(0, iconOffsetY),
            Stroke = new SolidColorBrush(Colors.White),
            StrokeThickness = 1.8,
            StrokeLineCap = PenLineCap.Round,
        };

        var grid = new Grid();
        grid.Children.Add(Gauge);
        grid.Children.Add(icon);
        Content = grid;

        Timer.Tick += (_, _) =>
        {
            try { Sample(); }
            catch (Exception ex) { Logger.Log($"{GetType().Name} 采样失败，已停用该组件: " + ex); Timer.Stop(); }
        };
        AttachedToVisualTree += (_, _) =>
        {
            try { OnAttach(); } catch (Exception ex) { Logger.Log($"{GetType().Name} OnAttach 失败: " + ex); }
            Timer.Start();
        };
        DetachedFromVisualTree += (_, _) =>
        {
            Timer.Stop();
            try { OnDetach(); } catch { /* ignore */ }
        };
    }

    /// <summary>挂载到可视树后调用一次：初始化数据源（如 PerformanceCounter 的 prime）。</summary>
    protected virtual void OnAttach() { }

    /// <summary>每次定时采样：读取数据并调用 Show 刷新环弧。</summary>
    protected abstract void Sample();

    /// <summary>从可视树卸载后调用：释放数据源。</summary>
    protected virtual void OnDetach() { }

    protected void Show(double pct) => Gauge.Value = pct;
}
