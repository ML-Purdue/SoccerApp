using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FootballSimulationApp
{
    internal static class NativeMethods
    {
        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage(
            out Message message,
            IntPtr hWnd,
            uint filterMin,
            uint filterMax,
            uint flags);

        [StructLayout(LayoutKind.Sequential)]
        public struct Message
        {
            private readonly IntPtr hWnd;
            private readonly uint Msg;
            private readonly IntPtr wParam;
            private readonly IntPtr lParam;
            private readonly uint Time;
            private readonly Point Point;
        }
    }
}