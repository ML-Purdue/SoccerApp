using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FootballSimulationApp
{
    internal static class NativeMethods
    {
        public static bool IsMessageAvailable
        {
            get
            {
                Message message;
                return PeekMessage(out message, IntPtr.Zero, 0, 0, 0);
            }
        }

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PeekMessage(
            out Message message,
            IntPtr hWnd,
            uint filterMin,
            uint filterMax,
            uint flags);

        [StructLayout(LayoutKind.Sequential)]
        private struct Message
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