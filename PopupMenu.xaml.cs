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
        public PopupMenu(PopupMenuItem[] DataContext)
        {
            this.InitializeComponent();
            ItemList.ItemsSource = DataContext;
        }

        internal void BeforeActivated(int x, int y)
        {
            var hWindow = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var idWindow = Win32Interop.GetWindowIdFromWindow(hWindow);
            float dpi = WinAPI.GetDpiForWindow(hWindow);
            var dpiScaling = dpi / 96;
            var appWindow = AppWindow.GetFromWindowId(idWindow);

            int height = (int)((App.Instance.Profiles.Count * 40 + 84) * dpiScaling);
            int width = (int)(200 * dpiScaling);

            WinAPI.SetWindowPos(hWindow, -1, x - width, y - height, width, height, 0x0010);

            var presenter = appWindow.Presenter as OverlappedPresenter;
            presenter.IsResizable = false;
            presenter.SetBorderAndTitleBar(false, false);

            var hIcon = WinAPI.LoadImage(IntPtr.Zero, "Assets/MonitorControl.ico", 1, 32, 32, 0x00000010);
            WinAPI.SendMessage(hWindow, 0x0080, 0, hIcon);
        }
    }
}
