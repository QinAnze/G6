using System;
using System.IO;

namespace GlassWidgets.Services;

/// <summary>轻量日志：写入 %LOCALAPPDATA%/GlassWidgets/app.log，失败静默忽略（不阻断主流程）。</summary>
public static class Logger
{
    private static readonly string Path = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GlassWidgets", "app.log");

    public static void Log(string msg)
    {
        try
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path)!);
            File.AppendAllText(Path, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {msg}\n");
        }
        catch { /* 日志写入失败不应影响应用 */ }
    }
}
