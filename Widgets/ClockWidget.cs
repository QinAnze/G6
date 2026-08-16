using System;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace GlassWidgets.Widgets;

/// <summary>时间组件：两排加粗数字，上面时 / 下面分，每秒刷新。</summary>
public class ClockWidget : UserControl
{
    private readonly TextBlock _hour = new();
    private readonly TextBlock _min = new();
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };

    public ClockWidget()
    {
        _hour.Classes.Add("big");
        _hour.FontWeight = FontWeight.Bold;
        _hour.HorizontalAlignment = HorizontalAlignment.Center;
        _hour.Foreground = new SolidColorBrush(Colors.White);

        _min.Classes.Add("big");
        _min.FontWeight = FontWeight.Bold;
        _min.HorizontalAlignment = HorizontalAlignment.Center;
        _min.Foreground = new SolidColorBrush(Colors.White);

        var sp = new StackPanel
        {
            Spacing = -10,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        sp.Children.Add(_hour);
        sp.Children.Add(_min);

        Content = sp;

        _timer.Tick += (_, _) => Update();
        Update();
        _timer.Start();
    }

    private void Update()
    {
        var n = DateTime.Now;
        _hour.Text = n.Hour.ToString("D2");
        _min.Text = n.Minute.ToString("D2");
    }
}
