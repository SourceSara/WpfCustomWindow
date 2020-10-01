using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CustomControls.Utils
{
    public static class NativeMethods
    {
        #region Dll Imports    

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref System.Drawing.Point lpPoint);

        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint(System.Drawing.Point pt, uint dwFlags);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hmonitor, [In, Out]MonitorInfo info);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("shell32.dll")]
        private static extern uint SHAppBarMessage(uint dwMessage, ref TaskbarData pData);

        #endregion

        #region Types

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public class MonitorInfo
        {
            public int cbSize = Marshal.SizeOf(typeof(MonitorInfo));
            public Rect rcMonitor = new Rect();
            public Rect rcWork = new Rect();
            public int dwFlags = 0;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szDevice = new char[32];
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public readonly int x;
            public readonly int y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private enum TaskbarMessages
        {
            New = 0x00,
            Remove = 0x01,
            QueryPos = 0x02,
            SetPos = 0x03,
            GetState = 0x04,
            GetTaskBarPos = 0x05,
            Activate = 0x06,
            GetAutoHideBar = 0x07,
            SetAutoHideBar = 0x08,
            WindowPosChanged = 0x09,
            SetState = 0x0a
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TaskbarData
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public Rectangle rc;
            public int lParam;
        }

        public enum TaskbarStates
        {
            AutoHide = 0x01,
            AlwaysOnTop = 0x02
        }

        #endregion

        #region Constants

        private const int MonitorDefaultToNearest = 2;
        private const string TaskbarWindowName = "System_TrayWnd";
        public const int SwpShowWindow = 0x0040;

        #endregion

        #region Methods

        public static IntPtr GetCurrentMonitor()
        {
            var p = new System.Drawing.Point(0, 0);
            GetCursorPos(ref p);
            return MonitorFromPoint(p, MonitorDefaultToNearest);
        }

        public static TaskbarStates GetTaskbarState()
        {
            TaskbarData msgData = new TaskbarData();
            msgData.cbSize = (uint)Marshal.SizeOf(msgData);
            msgData.hWnd = FindWindow(TaskbarWindowName, null);
            return (TaskbarStates)SHAppBarMessage((uint)TaskbarMessages.GetState, ref msgData);
        }

        #endregion
    }
}
