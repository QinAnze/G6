using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using GlassWidgets.Services;

namespace GlassWidgets.Widgets;

public partial class WidgetWindow : Window
{
    private readonly WidgetKind _kind;
    public WidgetKind Kind => _kind;

    // 无参构造，避免 AVLN3001 编译告警；并初始化 XAML 控件树
    public WidgetWindow() { InitializeComponent(); }

    public WidgetWindow(WidgetKind kind, PixelPoint pos, Size size) : this()
    {
        _kind = kind;
        Dwm.Round(this, 16);
        Width = size.Width;
        Height = size.Height;
        Position = pos;
        // 不置顶（Topmost=false）：组件作为普通窗口待在桌面，打开其他软件会盖住它
        Host.Content = CreateContent(kind);

        CloseBtn.Click += (_, _) => WidgetManager.Remove(this);
        Root.PointerPressed += OnRootPressed;
    }

    private void OnRootPressed(object? _, PointerPressedEventArgs e)
    {
        // 命中元素落在关闭按钮内（含其内 Path）时不触发拖动，交给 CloseBtn.Click 关闭
        for (var cur = e.Source as Control; cur != null; cur = cur.Parent as Control)
            if (cur == CloseBtn) return;
        BeginMoveDrag(e);
    }

    private Control CreateContent(WidgetKind kind) => kind switch
    {
        WidgetKind.Clock => new ClockWidget(),
        WidgetKind.Cpu => new CpuWidget(),
        WidgetKind.Memory => new MemoryWidget(),
        WidgetKind.Disk => new DiskWidget(),
        WidgetKind.Network => new NetworkWidget(),
        WidgetKind.Battery => new BatteryWidget(),
        _ => new ClockWidget(),
    };
}
