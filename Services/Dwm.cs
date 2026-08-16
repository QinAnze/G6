using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;

namespace GlassWidgets.Services;

/// <summary>
/// 窗口圆角贴合：让 OS 的 DWM 负责唯一的圆角（AcrylicBlur 窗口会随之裁剪内容），
/// 玻璃板不再画自己的圆角矩形，避免"玻璃圆角 + 窗口圆角"两层弧线。
/// DWM 半径按物理像素解释，乘以 RenderScaling 后与 DIP 尺寸对齐。
/// </summary>
public static class Dwm
{
    private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    private const int DWMWA_WINDOW_CORNER_RADIUS = 32;
    private const int DWMWCP_ROUND = 2;

    [DllImport("dwmapi", SetLastError = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int pv, int cb);

    public static void Round(Window w, double radius)
    {
        var ph = w.TryGetPlatformHandle();
        if (ph?.Handle is not { } hwnd || hwnd == IntPtr.Zero) return;

        int pref = DWMWCP_ROUND;
        _ = DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, sizeof(int));

        int phys = (int)Math.Round(radius * w.RenderScaling);
        _ = DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_RADIUS, ref phys, sizeof(int));
    }
}
