using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using GlassWidgets.Center;
using GlassWidgets.Services;

namespace GlassWidgets;

public partial class App : Application
{
    private TrayIcon? _tray;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // 普通桌面应用：关闭最后一个窗口即退出
            desktop.ShutdownMode = ShutdownMode.OnLastWindowClose;
            WidgetManager.Init(desktop);
            WidgetManager.Restore();
            SetupTray(desktop);
        }

        // 仅记录域级致命异常到日志，便于排查；不吞掉 UI 异常，
        // 确保窗口创建期的真实错误能暴露出来，而不是静默“打不开”。
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            Logger.Log("域级未处理异常: " + (e.ExceptionObject?.ToString() ?? "null"));
    }

    /// <summary>
    /// 程序化创建托盘图标（最小化收起后的恢复入口）。
    /// 全程 try/catch：无托盘环境（远程桌面/沙箱）初始化失败仅记日志，绝不拖垮应用；
    /// 右下角常驻启动器仍可恢复窗口（兜底）。
    /// </summary>
    private void SetupTray(IClassicDesktopStyleApplicationLifetime desktop)
    {
        try
        {
            var show = new NativeMenuItem("显示小组件中心");
            show.Click += (_, _) => WidgetManager.ShowCenter();
            var quit = new NativeMenuItem("退出");
            quit.Click += (_, _) => desktop.Shutdown();

            var menu = new NativeMenu();
            menu.Items.Add(show);
            menu.Items.Add(new NativeMenuItemSeparator());
            menu.Items.Add(quit);

            var icon = LoadLogoIcon();
            if (desktop.MainWindow is Window w) w.Icon = icon;

            _tray = new TrayIcon
            {
                Icon = icon,
                ToolTipText = "GlassWidgets",
                IsVisible = true,
                Menu = menu,
            };
            TrayIcon.SetIcons(this, new TrayIcons { _tray });
        }
        catch (Exception ex)
        {
            Logger.Log("图标/托盘初始化失败（不影响使用）: " + ex);
        }
    }

    /// <summary>从内嵌资源加载高清 logo（Assets/logo.png，与 exe 文件图标、README 同源），窗口与托盘共用。</summary>
    private static WindowIcon LoadLogoIcon()
    {
        var uri = new Uri("avares://GlassWidgets/Assets/logo.png");
        using var stream = AssetLoader.Open(uri);
        return new WindowIcon(stream);
    }
}
