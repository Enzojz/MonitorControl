﻿using System;
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
            CreateWindow();
        }

        private IntPtr m_hInstance = IntPtr.Zero;
        private IntPtr m_classAtom = IntPtr.Zero;
        private IntPtr m_hWindow = IntPtr.Zero;
        private WinAPI.WNDPROC m_windowProc;


        private IntPtr m_hMenu;

        internal Action Exit { get; set; }
        internal Action ShowWindow { get; set; }
        
        public const int HWND_MESSAGE = -3;

        protected virtual IntPtr OnWindowProc(IntPtr hWnd, WinAPI.WM msg, UIntPtr wParam, IntPtr lParam)
        {
            Console.WriteLine(msg);
            switch (msg)
            {
                case WinAPI.WM.WM_CREATE:
                    {
                        SetTrayIcon(hWnd);
                        return IntPtr.Zero;
                    }
                case WinAPI.WM.WM_DESTROY:
                    {
                        RemoveTrayIcon(hWnd);
                        return IntPtr.Zero;
                    }
                case WinAPI.WM.WM_COMMAND:
                    {
                        var command = (int)wParam;
                        if (command == 0)
                        {
                            if (Exit != null)
                            {
                                Exit();

                            }
                        } else if (command == 1)
                        {
                            if (ShowWindow != null)
                            {
                                ShowWindow();

                            }
                        } else if (command > 99)
                        {
                            var order = command - 100;
                            var profile = App.Instance.Profiles[order];
                            App.Instance.LoadProfile(profile.Name);
                        }
                        return IntPtr.Zero;
                    }
                case WinAPI.WM.WM_USER:
                    {
                        switch ((WinAPI.WM)lParam)
                        {
                            case WinAPI.WM.WM_RBUTTONDOWN:
                                {
                                    WinAPI.Point pt;
                                    WinAPI.GetCursorPos(out pt);
                                    PopupMenu(pt.x, pt.y);
                                    return IntPtr.Zero;
                                }
                            case WinAPI.WM.WM_LBUTTONDOWN:
                                {
                                    if (ShowWindow != null)
                                    {
                                        ShowWindow();

                                    }
                                    return IntPtr.Zero;
                                }
                            default:
                                return WinAPI.DefWindowProcW(hWnd, (uint)msg, wParam, lParam);
                        }
                    }
                default: return WinAPI.DefWindowProcW(hWnd, (uint)msg, wParam, lParam);
            }
        }

        public void CreateWindow()
        {
            // intentionally here, prevents garbage collection
            this.m_windowProc = this.OnWindowProc;

            var wndClassEx = new WinAPI.WNDCLASSEX
            {
                cbSize = Marshal.SizeOf(typeof(WinAPI.WNDCLASSEX)),
                hInstance = Marshal.GetHINSTANCE(GetType().Module),
                hCursor = IntPtr.Zero,
                lpszMenuName = string.Empty,
                lpszClassName = "MonitorControlTrayIcon",
                lpfnWndProc = OnWindowProc
            };

            var atom = WinAPI.RegisterClassEx(ref wndClassEx);
            if (0 == atom)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "RegisterClassEx failed");
            }

            this.m_classAtom = new IntPtr(atom);

            this.m_hWindow = WinAPI.CreateWindowEx(0, this.m_classAtom, IntPtr.Zero, 0, 0, 0, 0, 0, new IntPtr(HWND_MESSAGE), IntPtr.Zero, this.m_hInstance, IntPtr.Zero);
            if (IntPtr.Zero == this.m_hWindow)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateWindowEx failed");
            }
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

        private void PopupMenu(int x, int y)
        {
            m_hMenu = WinAPI.CreatePopupMenu();

            for (int i = 0; i < App.Instance.Profiles.Count; i++)
            {
                WinAPI.AppendMenuA(m_hMenu, WinAPI.MenuFlags.MF_STRING, (UIntPtr)(i + 100), WinAPI.StringToByteArray(App.Instance.Profiles[i].Name, Encoding.Default));
            }
            WinAPI.AppendMenuA(m_hMenu, WinAPI.MenuFlags.MF_SEPARATOR, (UIntPtr)2, null);
            WinAPI.AppendMenuA(m_hMenu, WinAPI.MenuFlags.MF_STRING, (UIntPtr)1, WinAPI.StringToByteArray("Show Monitor Control", Encoding.Default));
            WinAPI.AppendMenuA(m_hMenu, WinAPI.MenuFlags.MF_STRING, (UIntPtr)0, WinAPI.StringToByteArray("Exit", Encoding.Default));
            
            uint menuItemID = WinAPI.TrackPopupMenuEx(m_hMenu,
                WinAPI.TpmFlags.TPM_BOTTOMALIGN,
                x, y,
                this.m_hWindow,
                IntPtr.Zero);

        }
        
        public void Dispose()
        {
            if (m_hWindow != IntPtr.Zero)
            {
                WinAPI.DestroyWindow(this.m_hWindow);

                m_hWindow = IntPtr.Zero;
            }

            if (m_classAtom != IntPtr.Zero)
            {
                WinAPI.UnregisterClass(this.m_classAtom, this.m_hInstance);
            }
            GC.SuppressFinalize(this);
        }

    }
}