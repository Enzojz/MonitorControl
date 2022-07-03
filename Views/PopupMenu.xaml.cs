using Microsoft.UI;
using Microsoft.UI.Windowing;
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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MonitorControl
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PopupMenu : Window
    {
        const int GWL_STYLE = -16;
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

            m_backdropHelper = new BackdropManager(this);
            App.SettingManager.ThemeChanged += ThemeChanged;
            ThemeChanged(null, App.SettingManager.ThemeEnum);
        }

        private IntPtr m_hWindow;
        private Rect m_rect;
        private BackdropManager m_backdropHelper;

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

        public PopupMenuItem[] MenuItems
        {
            get
            {
                return App.Instance.Profiles
                    .Select(profile => new PopupMenuItem()
                    {
                        Text = profile.Name,
                        IsChecked = App.Instance.CurrentProfile != null && App.Instance.CurrentProfile.Profile.Guid == profile.Profile.Guid,
                        Callback = () => {
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
