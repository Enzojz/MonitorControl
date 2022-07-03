using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MonitorControl
{
    internal class TrayIcon : IDisposable
    {
        public TrayIcon()
        {
            var wndClassEx = new WinAPI.WNDCLASSEX
            {
                cbSize = Marshal.SizeOf(typeof(WinAPI.WNDCLASSEX)),
                hInstance = Marshal.GetHINSTANCE(GetType().Module),
                hCursor = IntPtr.Zero,
                lpszMenuName = string.Empty,
                lpszClassName = "MonitorControlTrayIcon",
                lpfnWndProc = WindowProc
            };

            var atom = WinAPI.RegisterClassEx(ref wndClassEx);
            if (0 == atom)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "RegisterClassEx failed");
            }

            m_classAtom = new IntPtr(atom);

            m_hWindow = WinAPI.CreateWindowEx(0, m_classAtom, IntPtr.Zero, 0, 0, 0, 0, 0, new IntPtr(HWND_MESSAGE), IntPtr.Zero, m_hInstance, IntPtr.Zero);
            if (IntPtr.Zero == m_hWindow)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateWindowEx failed");
            }
        }

        private IntPtr m_hInstance = IntPtr.Zero;
        private IntPtr m_classAtom = IntPtr.Zero;
        private IntPtr m_hWindow = IntPtr.Zero;

        internal Action OpenWindow { get; set; }

        internal Action<int, int> PopupMenu { get; set; }

        public const int HWND_MESSAGE = -3;

        protected virtual IntPtr WindowProc(IntPtr hWnd, WinAPI.WM msg, UIntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WinAPI.WM.WM_CREATE:
                    SetTrayIcon(hWnd);
                    break;
                case WinAPI.WM.WM_DESTROY:
                    RemoveTrayIcon(hWnd);
                    break;
                case WinAPI.WM.WM_USER:
                    switch ((WinAPI.WM)lParam)
                    {
                        case WinAPI.WM.WM_RBUTTONDOWN:
                            if (PopupMenu != null)
                            {
                                WinAPI.Point pt;
                                WinAPI.GetCursorPos(out pt);
                                PopupMenu(pt.x, pt.y);
                            }
                            break;
                        case WinAPI.WM.WM_LBUTTONDOWN:
                            if (OpenWindow != null)
                                OpenWindow();
                            break;
                    }
                    return IntPtr.Zero;
            }
            return WinAPI.DefWindowProcW(hWnd, (uint)msg, wParam, lParam);
        }

        private void SetTrayIcon(IntPtr hWnd)
        {

            var data = new WinAPI.NOTIFYICONDATA
            {
                cbSize = Marshal.SizeOf<WinAPI.NOTIFYICONDATA>(),
                hWnd = hWnd,
                uFlags = WinAPI.NotifyFlags.NIF_ICON | WinAPI.NotifyFlags.NIF_MESSAGE,
                uID = 1,
                hIcon = WinAPI.LoadImage(IntPtr.Zero, "Assets/MonitorControl.ico", 1, 32, 32, 0x00000010),
                uCallbackMessage = WinAPI.WM.WM_USER,
            };

            WinAPI.Shell_NotifyIcon(WinAPI.NotifyIconMessage.NIM_ADD, ref data);
        }

        private void RemoveTrayIcon(IntPtr hWnd)
        {
            var data = new WinAPI.NOTIFYICONDATA
            {
                cbSize = Marshal.SizeOf<WinAPI.NOTIFYICONDATA>(),
                hWnd = hWnd,
                uID = 1
            };

            WinAPI.Shell_NotifyIcon(WinAPI.NotifyIconMessage.NIM_DELETE, ref data);
        }

        public void Dispose()
        {
            if (m_hWindow != IntPtr.Zero)
            {
                try
                {
                    WinAPI.DestroyWindow(this.m_hWindow);
                }
                catch (System.ExecutionEngineException e)
                {

                }
                finally
                {
                    m_hWindow = IntPtr.Zero;
                }
            }

            if (m_classAtom != IntPtr.Zero)
            {
                WinAPI.UnregisterClass(m_classAtom, m_hInstance);
            }
            GC.SuppressFinalize(this);
        }

    }
}
