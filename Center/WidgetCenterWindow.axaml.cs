using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using GlassWidgets;
using GlassWidgets.Services;

namespace GlassWidgets.Center;

public partial class WidgetCenterWindow : Window
{
    public WidgetCenterWindow()
    {
        InitializeComponent();
        Dwm.Round(this, 16);

        MinBtn.Click += (_, _) => WidgetManager.HideCenter(); // 最小化：只收起中心窗口，组件保持显示
        CloseBtn.Click += (_, _) => WidgetManager.Lifetime?.Shutdown();
        ClearBtn.Click += (_, _) => WidgetManager.RemoveAll();

        Root.PointerPressed += OnRootPressed;

        foreach (WidgetKind k in Enum.GetValues<WidgetKind>())
        {
            var spec = WidgetSpecs.For(k);
            var card = new Button
            {
                Classes = { "card" },
                Width = 146,
                Height = 92,
                Margin = new Thickness(7),
                Content = MakeCard(spec.Display, spec.IconKey),
            };
            card.Click += (_, _) => WidgetManager.Spawn(k);
            CardHost.Children.Add(card);
        }
    }

    /// <summary>整窗拖动：命中任何按钮（交通灯/卡片/清空）不拖，其余区域（玻璃空白）拖动窗口。</summary>
    private void OnRootPressed(object? _, PointerPressedEventArgs e)
    {
        for (var cur = e.Source as Control; cur != null; cur = cur.Parent as Control)
            if (cur is Button) return;
        BeginMoveDrag(e);
    }

    private static Border MakeCard(string name, string iconKey)
    {
        var sp = new StackPanel
        {
            Spacing = 10,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        sp.Children.Add(new Avalonia.Controls.Shapes.Path
        {
            Classes = { "icon" },
            Data = (StreamGeometry)(Application.Current?.FindResource(iconKey) ?? new StreamGeometry()),
            Width = 30,
            Height = 30,
            Stretch = Stretch.Uniform,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Stroke = new SolidColorBrush(Colors.White),
            StrokeThickness = 1.8,
            StrokeLineCap = PenLineCap.Round,
        });
        sp.Children.Add(new TextBlock
        {
            Classes = { "title" },
            Text = name,
            HorizontalAlignment = HorizontalAlignment.Center,
        });
        return new Border { Child = sp };
    }
}
