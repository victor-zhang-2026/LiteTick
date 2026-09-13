using System.Runtime.InteropServices;

namespace LiteTick.Windows.Platform;

internal static class NativeMethods
{
    internal const uint MonitorDefaultToPrimary = 1;
    internal const uint MonitorDefaultToNearest = 2;
    internal const uint SwpNoActivate = 0x0010;
    internal const uint SwpNoOwnerZOrder = 0x0200;
    internal const uint SwpShowWindow = 0x0040;
    internal static readonly IntPtr HwndTopmost = new(-1);

    [StructLayout(LayoutKind.Sequential)]
    internal struct Point
    {
        internal int X;
        internal int Y;

        internal Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;

        internal int Width => Right - Left;
        internal int Height => Bottom - Top;
        internal Point Center => new(Left + Width / 2, Top + Height / 2);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MonitorInfo
    {
        internal uint Size;
        internal Rect Monitor;
        internal Rect Work;
        internal uint Flags;
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(out Point point);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetWindowRect(IntPtr window, out Rect rect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowPos(
        IntPtr window,
        IntPtr insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromPoint(Point point, uint flags);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromRect(ref Rect rect, uint flags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo info);

    internal static Rect WorkAreaForPoint(Point point, bool nearest = true)
    {
        var monitor = MonitorFromPoint(point, nearest ? MonitorDefaultToNearest : MonitorDefaultToPrimary);
        return GetWorkArea(monitor);
    }

    internal static Rect WorkAreaForRect(Rect rect)
    {
        var monitor = MonitorFromRect(ref rect, MonitorDefaultToNearest);
        return GetWorkArea(monitor);
    }

    internal static void MoveTopmost(IntPtr window, int x, int y, int width, int height, bool activate)
    {
        var flags = SwpNoOwnerZOrder | SwpShowWindow;
        if (!activate)
        {
            flags |= SwpNoActivate;
        }
        _ = SetWindowPos(window, HwndTopmost, x, y, width, height, flags);
    }

    private static Rect GetWorkArea(IntPtr monitor)
    {
#if NET35
        var info = new MonitorInfo { Size = (uint)Marshal.SizeOf(typeof(MonitorInfo)) };
#else
        var info = new MonitorInfo { Size = (uint)Marshal.SizeOf<MonitorInfo>() };
#endif
        if (monitor == IntPtr.Zero || !GetMonitorInfo(monitor, ref info))
        {
            return new Rect { Left = 0, Top = 0, Right = 1920, Bottom = 1080 };
        }
        return info.Work;
    }
}
