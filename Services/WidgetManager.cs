using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using GlassWidgets.Center;
using GlassWidgets.Widgets;

namespace GlassWidgets.Services;

/// <summary>组件生命周期与布局持久化中枢。</summary>
public static class WidgetManager
{
    public static WidgetCenterWindow? Center;
    public static IClassicDesktopStyleApplicationLifetime? Lifetime;
    public static readonly List<WidgetWindow> Widgets = new();

    public static void Init(IClassicDesktopStyleApplicationLifetime desktop)
    {
        Lifetime = desktop;

        // 小组件中心 = 主界面：设为主窗口并随启动直接打开（无托盘驻留、无右下角启动器）
        Center = new WidgetCenterWindow();
        desktop.MainWindow = Center;
        Center.Show();
    }

    public static void Restore()
    {
        // 主界面已在 Init 打开；这里仅恢复历史组件
        foreach (var s in LayoutStore.Load())
            if (Enum.TryParse<WidgetKind>(s.Kind, out var k))
                Spawn(k, new PixelPoint(s.X, s.Y), persist: false);
    }

    public static void Spawn(WidgetKind kind, PixelPoint? pos = null, bool persist = true)
    {
        var spec = WidgetSpecs.For(kind);
        var p = pos ?? NextCascade();
        try
        {
            var w = new WidgetWindow(kind, p, spec.DefaultSize);
            Widgets.Add(w);
            w.Show();
            if (persist) Save();
        }
        catch (Exception ex) { Logger.Log($"Spawn {kind} 失败（已跳过该组件）: {ex}"); }
    }

    public static void Remove(WidgetWindow w)
    {
        Widgets.Remove(w);
        w.Close(); // 必须真正关闭窗口，否则只从列表移除、窗口仍留在屏幕上
        Save();
    }

    /// <summary>最小化语义：只收起小组件中心窗口，组件保持显示在桌面。</summary>
    public static void HideCenter() => Center?.Hide();

    /// <summary>恢复小组件中心窗口。</summary>
    public static void ShowCenter()
    {
        Center?.Show();
        Center?.Activate();
    }

    public static void RemoveAll()
    {
        foreach (var w in Widgets.ToList()) w.Close();
        Widgets.Clear();
        Save();
    }

    public static void Save()
    {
        var states = Widgets.Select(w => new WidgetState(w.Kind.ToString(), w.Position.X, w.Position.Y));
        LayoutStore.Save(states);
    }

    private static PixelPoint NextCascade()
    {
        var n = Widgets.Count;
        return new PixelPoint(220 + (n % 6) * 36, 180 + (n % 6) * 36);
    }
}
