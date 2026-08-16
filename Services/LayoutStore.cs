using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GlassWidgets.Services;

public record WidgetState(string Kind, int X, int Y);

/// <summary>布局持久化：组件种类与桌面坐标序列化到 %LOCALAPPDATA%/GlassWidgets/layout.json。</summary>
public static class LayoutStore
{
    private static readonly string Path = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GlassWidgets", "layout.json");

    public static List<WidgetState> Load()
    {
        try
        {
            if (!File.Exists(Path)) return new();
            var json = File.ReadAllText(Path);
            return JsonSerializer.Deserialize<List<WidgetState>>(json) ?? new();
        }
        catch (Exception ex) { Logger.Log("LayoutStore.Load 失败: " + ex); return new(); }
    }

    public static void Save(IEnumerable<WidgetState> states)
    {
        try
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path)!);
            File.WriteAllText(Path, JsonSerializer.Serialize(states, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex) { Logger.Log("LayoutStore.Save 失败: " + ex); }
    }
}
