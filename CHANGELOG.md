# 更新日志 (Changelog)

本文件遵循 [Keep a Changelog](https://keepachangelog.com/) 规范，版本号遵循 [语义化版本](https://semver.org/lang/zh-CN/)。

## [Unreleased]

## [1.0.0] - 2026-08-16

### Added
- 基于 Avalonia 11.2.1 的 Windows 桌面玻璃小组件框架
- 6 类环状组件：时间、CPU、内存、磁盘、网络、电量
- 小组件中心（主界面）：卡片式添加组件
- macOS 风格交通灯（左上角，带 1px 描边，hover 显形半透黑符号）
- 可拖拽摆放、非置顶、不进入任务栏
- 系统托盘恢复入口（程序化生成图标，无托盘环境自动降级）
- DWM 物理圆角（窗口级 AcrylicBlur 透明）
- 布局持久化至 `%LOCALAPPDATA%/GlassWidgets/layout.json`

### Fixed
- 组件关闭按钮真正关闭窗口（`WidgetManager.Remove` 补 `w.Close()`）
- 拖动命中回溯：从命中源沿视觉树回溯判断，避免误吞 `Click`
- 图标垂直居中对齐（电池/网络等独立偏移校正）

### Security
- **[Security Fix]** `Tmds.DBus.Protocol` 0.20.0 → 0.21.3
  - Advisory: GHSA-xrw6-gwf8-vvr9 / CVE-2026-39959（High）
  - 说明：经 `Avalonia.Desktop` 间接引入，仅 Linux 运行期生效；本程序 Windows-only 实际风险≈0，为清除 SCA 告警锁定至同线兼容修复版。
