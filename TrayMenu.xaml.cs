using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Shell;
using Windows.Devices.Geolocation;

namespace MonitorControl
{
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
                    ResizeBorderThickness = default,
                    UseAeroCaptionButtons = true
                }
            );

            var helper = new WindowInteropHelper(this);
            helper.EnsureHandle();
            m_hWindow = helper.Handle;

            m_wndProc = WindowProc;
            WinAPI.SetWindowLongPtr(m_hWindow, -16, (IntPtr)(WinAPI.WS.WS_CAPTION));

            SetTrayIcon();

            WinAPI.SetWindowSubclass(m_hWindow, m_wndProc, UIntPtr.Zero, UIntPtr.Zero);
            
            DataContext = App.Instance;
            
            Loaded += (_, _) => SetPosition();
            Closed += (_, _) => RemoveTrayIcon();
        }

        private IntPtr m_hWindow;

        private WinAPI.SUBCLASSPROC m_wndProc;

        public Action OpenWindow;
        public Action ExitApplication;

        internal void SetPosition()
        {
            WinAPI.Point pt;
            WinAPI.GetCursorPos(out pt);

            float dpi = WinAPI.GetDpiForWindow(m_hWindow);
            var dpiScaling = dpi / 96;

            var width = this.ActualWidth * dpiScaling;
            var height = this.ActualHeight * dpiScaling;

            WinAPI.SetWindowPos(m_hWindow, -1, (int)(pt.x - width), (int)(pt.y - height), (int)width, (int)height, 0x0040);
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
                            SetPosition();
                            Show();
                            Activate();
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

        private void TrayMenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Hide();
        }

        private void OpenWindowClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Hide();
            OpenWindow();
        }

        private void ExitClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Hide();
            ExitApplication();

        }
    }
}
