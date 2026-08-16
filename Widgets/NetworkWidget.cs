using System;
using System.Net.NetworkInformation;
using Avalonia.Media;
using GlassWidgets;

namespace GlassWidgets.Widgets;

/// <summary>网络吞吐：上行网卡收发字节差换算 Mb/s，环弧按链路速率归一化。</summary>
public class NetworkWidget : RingMetricWidget
{
    private NetworkInterface? _nic;
    private long _prevBytes;
    private DateTime _prevTime;

    public NetworkWidget() : base("GeoNetwork", iconOffsetY: 4)
    {
        Gauge.Warn = 101; // 吞吐高不视为告警，保持固定色
        Gauge.Hot = 101;
        Gauge.ArcColor = Color.FromRgb(232, 120, 170); // 粉
        Timer.Interval = TimeSpan.FromSeconds(1);
    }

    protected override void OnAttach()
    {
        _nic = NetworkInterface.GetAllNetworkInterfaces()
            .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up &&
                                 n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                                 n.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                                 !n.Description.Contains("Bluetooth", StringComparison.OrdinalIgnoreCase) &&
                                 !n.Name.Contains("Bluetooth", StringComparison.OrdinalIgnoreCase));
        if (_nic != null)
        {
            var s = _nic.GetIPv4Statistics();
            _prevBytes = s.BytesReceived + s.BytesSent;
            _prevTime = DateTime.Now;
        }
    }

    protected override void Sample()
    {
        if (_nic == null) { Show(0); return; }
        try
        {
            var s = _nic.GetIPv4Statistics();
            long cur = s.BytesReceived + s.BytesSent;
            var now = DateTime.Now;
            var dt = (now - _prevTime).TotalSeconds;
            double pct = 0;
            if (dt > 0.001)
            {
                var bytesPerSec = (cur - _prevBytes) / dt;
                var speed = _nic.Speed; // bits/sec
                pct = speed > 0 ? Math.Min(100, bytesPerSec * 8 / speed * 100) : Math.Min(100, bytesPerSec * 8 / 1e6);
            }
            _prevBytes = cur;
            _prevTime = now;
            Show(pct);
        }
        catch { Show(0); }
    }

    protected override void OnDetach() => _nic = null;
}
