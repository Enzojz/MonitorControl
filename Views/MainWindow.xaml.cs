using Windows.Foundation.Metadata;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System.Runtime.InteropServices; // For DllImport

using System;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Microsoft.UI;
using WinRT;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Shapes;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MonitorControl
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();


            this.Title = "Monitor Control";

            m_hWindow = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var idWindow = Win32Interop.GetWindowIdFromWindow(m_hWindow);
            float dpi = WinAPI.GetDpiForWindow(m_hWindow);
            var dpiScaling = dpi / 96;
            var appWindow = AppWindow.GetFromWindowId(idWindow);

            int baseHeight = App.Instance.Monitors.Count * 40 + 190;

            m_size = new Windows.Graphics.SizeInt32()
            {
                Width = (int)(900 * dpiScaling),
                Height = (int)((baseHeight < 380 ? 380 : baseHeight) * dpiScaling)
            };

            appWindow.Resize(m_size);

            var presenter = appWindow.Presenter as OverlappedPresenter;
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                ExtendsContentIntoTitleBar = true;
                SetTitleBar(this.AppTitleBar);
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
                TitleRow.Height = new GridLength(0);
            }

            var hIcon = WinAPI.LoadImage(IntPtr.Zero, "Assets/MonitorControl.ico", 1, 32, 32, 0x00000010);
            WinAPI.SendMessage(m_hWindow, 0x0080, 0, hIcon);

            m_backdropHelper = new BackdropManager(this);
            App.SettingManager.ThemeChanged += ThemeChanged;
            ThemeChanged(null, App.SettingManager.ThemeEnum);

            m_wndProc = WindowProc;
            WinAPI.SetWindowSubclass(m_hWindow, m_wndProc, UIntPtr.Zero, UIntPtr.Zero);
        }


        private IntPtr WindowProc(IntPtr hWnd, WinAPI.WM msg, IntPtr wParam, IntPtr lParam, UIntPtr uIdSubclass, UIntPtr dwRefData)
        {
            switch (msg)
            {
                case WinAPI.WM.WM_GETMINMAXINFO:
                    var minMaxInfo = Marshal.PtrToStructure<WinAPI.MINMAXINFO>(lParam);
                    minMaxInfo.ptMaxSize.x = m_size.Width;
                    minMaxInfo.ptMaxSize.y = m_size.Height;
                    Marshal.StructureToPtr(minMaxInfo, lParam, true);
                    break;
            }
            return WinAPI.DefSubclassProc(hWnd, msg, wParam, lParam);
        }

        private Windows.Graphics.SizeInt32 m_size;
        private IntPtr m_hWindow;
        private WinAPI.SUBCLASSPROC m_wndProc;

        private void ThemeChanged(object sender, BackdropManager.BackdropType backdrop)
        {
            m_backdropHelper.SetBackdrop(backdrop);
            MainGrid.Background = App.SettingManager.ThemeEnum == BackdropManager.BackdropType.Classic ? Application.Current.Resources["ApplicationPageBackgroundThemeBrush"] as SolidColorBrush : null;
        }

        internal InstanceCore Instance => App.Instance;

        private BackdropManager m_backdropHelper;


        private void SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            this.ContentFrame.Content = args.SelectedItem;
        }

        private void ProfileSelected(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.ContentFrame.Content = this.Instance.Profiles;
        }

        private void SettingSelected(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.ContentFrame.Content = 0;
        }
    }
}
