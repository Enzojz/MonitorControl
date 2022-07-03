using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MonitorControl
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PopupMenu : Window
    {
        public PopupMenu()
        {
            this.InitializeComponent();
            m_hWindow = WinRT.Interop.WindowNative.GetWindowHandle(this);

            var hIcon = WinAPI.LoadImage(IntPtr.Zero, "Assets/MonitorControl.ico", 1, 32, 32, 0x00000010);
            WinAPI.SendMessage(m_hWindow, 0x0080, 0, hIcon);

            var idWindow = Win32Interop.GetWindowIdFromWindow(m_hWindow);
            var appWindow = AppWindow.GetFromWindowId(idWindow);

            var presenter = appWindow.Presenter as OverlappedPresenter;
            presenter.IsResizable = false;
            presenter.SetBorderAndTitleBar(false, false);

            WinAPI.SetWindowLongPtr(m_hWindow, -20, (IntPtr)WinAPI.WS_EX.WS_EX_TOOLWINDOW);

            m_backdropHelper = new BackdropManager(this);
            App.SettingManager.ThemeChanged += ThemeChanged;
            ThemeChanged(null, App.SettingManager.ThemeEnum);

            m_wndProc = this.WindowProc;

            SetTrayIcon();

            // Thanks to https://www.travelneil.com/wndproc-in-uwp.html so that I avoid to create a raw window
            m_wndProcLegacy = WinAPI.SetWindowLongPtr(m_hWindow, WinAPI.GWLP_WNDPROC, m_wndProc);

            Closed += OnClosed;
        }

        private void OnClosed(object sender, WindowEventArgs args)
        {
            RemoveTrayIcon();
        }

        private IntPtr m_hWindow;
        private Rect m_rect;
        private BackdropManager m_backdropHelper;

        private IntPtr m_wndProcLegacy;
        private WinAPI.WNDPROC m_wndProc;

        internal Action OpenWindow;
        internal Action ExitApplication;

        internal void SetPosition(int x, int y)
        {
            ItemList.ItemsSource = MenuItems;
            float dpi = WinAPI.GetDpiForWindow(m_hWindow);
            var dpiScaling = dpi / 96;

            m_rect.Width = 200 * dpiScaling;
            m_rect.Height = (App.Instance.Profiles.Count * 40 + 84) * dpiScaling;
            m_rect.X = x - m_rect.Width;
            m_rect.Y = y - m_rect.Height;

            WinAPI.SetWindowPos(m_hWindow, -1, (int)m_rect.X, (int)m_rect.Y, (int)m_rect.Width, (int)m_rect.Height, 0x0040);
        }

        internal void Hide()
        {
            WinAPI.SetWindowPos(m_hWindow, 1, (int)m_rect.X, (int)m_rect.Y, (int)m_rect.Width, (int)m_rect.Height, 0x0080);
        }

        private void ThemeChanged(object sender, BackdropManager.BackdropType backdrop)
        {
            m_backdropHelper.SetBackdrop(backdrop);
        }

        private IntPtr WindowProc(IntPtr hWnd, WinAPI.WM msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WinAPI.WM.WM_USER:
                    switch ((WinAPI.WM)lParam)
                    {
                        case WinAPI.WM.WM_RBUTTONDOWN:
                            WinAPI.Point pt;
                            WinAPI.GetCursorPos(out pt);
                            SetPosition(pt.x, pt.y);
                            break;
                        case WinAPI.WM.WM_LBUTTONDOWN:
                            OpenWindow();
                            break;;
                    }
                    break;
            }
            return WinAPI.CallWindowProc(m_wndProcLegacy, hWnd, msg, wParam, lParam);
        }


        private void SetTrayIcon()
        {

            var data = new WinAPI.NOTIFYICONDATA
            {
                cbSize = Marshal.SizeOf<WinAPI.NOTIFYICONDATA>(),
                hWnd = m_hWindow,
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
                hWnd = m_hWindow,
                uID = 1
            };

            WinAPI.Shell_NotifyIcon(WinAPI.NotifyIconMessage.NIM_DELETE, ref data);
        }

        public PopupMenuItem[] MenuItems
        {
            get
            {
                return App.Instance.Profiles
                    .Select(profile => new PopupMenuItem()
                    {
                        Text = profile.Name,
                        IsChecked = App.Instance.CurrentProfile != null && App.Instance.CurrentProfile.Profile.Guid == profile.Profile.Guid,
                        Callback = () =>
                        {
                            Hide();
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
                                Hide();
                                OpenWindow();
                            }
                        },
                        new PopupMenuItem() {
                            Text = "Exit",
                            Callback = () => {
                                ExitApplication();
                            }
                        }
                    }
                    ).ToArray();
            }
        }
    }
}
