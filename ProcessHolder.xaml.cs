using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MonitorControl
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProcessHolder : Window
    {
        private IntPtr m_hWnd;
        private IntPtr m_wndProc;
        private MainWindow m_window;
        private PopupMenu m_popupMenu;

        public PopupMenuItem[] MenuItems
        {
            get
            {
                return App.Instance.Profiles
                    .Select(profile => new PopupMenuItem() {
                        Text = profile.Name, 
                        IsChecked = App.Instance.CurrentProfile != null && App.Instance.CurrentProfile.Profile.Guid == profile.Profile.Guid,
                        Callback = () => { 
                            ClosePopup(); 
                            App.Instance.LoadProfile(profile.Name); 
                        } 
                    })
                    .Union(
                    new PopupMenuItem[]
                    {
                        new PopupMenuItem(),
                        new PopupMenuItem() { 
                            Text = "Show Monitor Control", 
                            Callback = () => { 
                                ClosePopup(); 
                                OpenMainWindow(); 
                            } 
                        },
                        new PopupMenuItem() { 
                            Text = "Exit", 
                            Callback = () => { 
                                CloseMainWindow(); 
                                ClosePopup(); 
                                Close(); 
                            } 
                        }
                    }
                    ).ToArray();
            }
        }

        public ProcessHolder()
        {
            this.InitializeComponent();
            m_hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            this.Closed += OnClosed;

            SetTrayIcon();

            m_wndProc = SetWndProc(m_hWnd, this.OnWindowProc);

            if (!App.SettingManager.RunInBackground)
            {
                OpenMainWindow();
            }
        }

        private void OnClosed(object sender, WindowEventArgs args)
        {
            CloseMainWindow();
            RemoveTrayIcon();
        }

        private IntPtr OnWindowProc(IntPtr hWnd, WinAPI.WM msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WinAPI.WM.WM_USER:
                    {
                        switch ((WinAPI.WM)lParam)
                        {
                            case WinAPI.WM.WM_RBUTTONDOWN:
                                WinAPI.Point pt;
                                WinAPI.GetCursorPos(out pt);
                                OpenPopup(pt.x, pt.y);
                                return IntPtr.Zero;
                            case WinAPI.WM.WM_LBUTTONDOWN:
                                OpenMainWindow();
                                return IntPtr.Zero;
                            default:
                                return WinAPI.CallWindowProc(m_wndProc, hWnd, msg, wParam, lParam);
                        }
                    }
                default:
                    return WinAPI.CallWindowProc(m_wndProc, hWnd, msg, wParam, lParam);
            }
        }

        private void OpenPopup(int x, int y)
        {
            if (m_popupMenu == null)
            {
                m_popupMenu = new PopupMenu(MenuItems);
                m_popupMenu.BeforeActivated(x, y);
                m_popupMenu.Activate();
            }
        }

        private void ClosePopup()
        {
            if (m_popupMenu != null)
            {
                m_popupMenu.Close();
                m_popupMenu = null;
            }
        }

        internal void OpenMainWindow()
        {
            if (m_window == null)
            {
                m_window = new MainWindow();
                MainWindow.BeforeActivated(m_window);
                m_window.Closed += MainWindowClosed;
                m_window.Activate();
            }
        }

        internal void CloseMainWindow()
        {
            if (m_window != null)
            {
                m_window.Close();
            }
        }

        private void MainWindowClosed(object sender, WindowEventArgs args)
        {
            m_window.Closed -= MainWindowClosed;
            m_window = null;
            if (!App.SettingManager.RunInBackground)
            {
                this.Close();
            }
        }

        private void SetTrayIcon()
        {
            var data = new WinAPI.NOTIFYICONDATA
            {
                cbSize = Marshal.SizeOf<WinAPI.NOTIFYICONDATA>(),
                hWnd = this.m_hWnd,
                uFlags = WinAPI.NotifyFlags.NIF_ICON | WinAPI.NotifyFlags.NIF_MESSAGE,
                uID = 1,
                hIcon = WinAPI.LoadImage(IntPtr.Zero, "Assets/MonitorControl.ico", 1, 32, 32, 0x00000010),
                uCallbackMessage = WinAPI.WM.WM_USER,
            };

            WinAPI.Shell_NotifyIcon(WinAPI.NotifyIconMessage.NIM_ADD, ref data);
        }

        private void RemoveTrayIcon()
        {
            var data = new WinAPI.NOTIFYICONDATA
            {
                cbSize = Marshal.SizeOf<WinAPI.NOTIFYICONDATA>(),
                hWnd = this.m_hWnd,
                uID = 1
            };

            WinAPI.Shell_NotifyIcon(WinAPI.NotifyIconMessage.NIM_DELETE, ref data);
        }

        private static IntPtr SetWndProc(IntPtr hWnd, WinAPI.WndProcDelegate newProc)
        {
            // Thanks to https://www.travelneil.com/wndproc-in-uwp.html so that I avoid to create a raw window

            IntPtr wndProcPtr = Marshal.GetFunctionPointerForDelegate(newProc);
            return WinAPI.SetWindowLongPtr(hWnd, WinAPI.GWLP_WNDPROC, wndProcPtr);
        }
    }
}
