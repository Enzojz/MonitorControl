using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Shell;

namespace MonitorControl
{
    /// <summary>
    /// TrayMenu.xaml 的交互逻辑
    /// </summary>
    public partial class TrayMenu : Window
    {
        public TrayMenu()
        {
            InitializeComponent();

            WindowChrome.SetWindowChrome(
                this,
                new WindowChrome
                {
                    CaptionHeight = 0,
                    CornerRadius = default,
                    GlassFrameThickness = new Thickness(-1),
                    ResizeBorderThickness = ResizeMode == ResizeMode.NoResize ? default : new Thickness(4),
                    UseAeroCaptionButtons = true
                }
            );

            var helper = new WindowInteropHelper(this);
            helper.EnsureHandle();
            m_hWindow = helper.Handle;

            m_wndProc = this.WindowProc;

            SetTrayIcon();

            WinAPI.SetWindowSubclass(m_hWindow, m_wndProc, UIntPtr.Zero, UIntPtr.Zero);
            Closed += OnClosed;
        }

        private void OnClosed(object sender, EventArgs args)
        {
            RemoveTrayIcon();
        }

        private IntPtr m_hWindow;

        private WinAPI.SUBCLASSPROC m_wndProc;

        internal Action OpenWindow;
        internal Action ExitApplication;
        
        internal void SetPosition(int x, int y)
        {
            //ItemList.ItemsSource = MenuItems;
            float dpi = WinAPI.GetDpiForWindow(m_hWindow);
            var dpiScaling = dpi / 96;

            var width = 200 * dpiScaling;
            var height = (App.Instance.Profiles.Count * 40 + 84) * dpiScaling;

            WinAPI.SetWindowPos(m_hWindow, -1, (int)(x - width), (int)(y - height), (int)width, (int)height, 0x0040);
            WinAPI.SetForegroundWindow(m_hWindow);
        }

        private IntPtr WindowProc(IntPtr hWnd, WinAPI.WM msg, IntPtr wParam, IntPtr lParam, UIntPtr uIdSubclass, UIntPtr dwRefData)
        {
            switch (msg)
            {
                case WinAPI.WM.WM_NCACTIVATE:
                    if (wParam.ToInt32() == 0)
                    {
                        var hWindow = WinAPI.GetActiveWindow();
                        if (hWindow == hWnd)
                        {
                            Hide();
                            if (lParam.ToInt32() == 0)
                            {
                                return IntPtr.Zero;
                            }
                        }
                    }
                    break;
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
                            break; ;
                    }
                    break;
            }
            return WinAPI.DefSubclassProc(hWnd, msg, wParam, lParam);
        }


        internal void Hide()
        {
            WinAPI.ShowWindow(m_hWindow, WinAPI.CmdShow.SW_HIDE);
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
                            Callback = OpenWindow
                        },
                        new PopupMenuItem() {
                            Text = "Exit",
                            Callback = ExitApplication
                        }
                    }
                    ).ToArray();
            }
        }
        internal void SetTrayIcon()
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
    }
}
