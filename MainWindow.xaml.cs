using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.UI.WindowManagement;
using System.Runtime.InteropServices; // For DllImport

namespace MonitorControl;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();


        //this.Title = "Monitor Control";

        //m_hWindow = WinRT.Interop.WindowNative.GetWindowHandle(this);
        //var idWindow = Win32Interop.GetWindowIdFromWindow(m_hWindow);
        float dpi = WinAPI.GetDpiForWindow(m_hWindow);
        var dpiScaling = dpi / 96;
        //var appWindow = AppWindow.GetFromWindowId(idWindow);
        int baseHeight = App.Instance.Monitors.Count * 40 + 190;

        this.Height = baseHeight;

        //m_size = new Windows.Graphics.SizeInt32()
        //{
        //    Width = (int)(900 * dpiScaling),
        //    Height = (int)((baseHeight < 380 ? 380 : baseHeight) * dpiScaling)
        //};

        //appWindow.Resize(m_size);

        //var presenter = appWindow.Presenter as OverlappedPresenter;
        //presenter.IsResizable = false;
        //presenter.IsMaximizable = false;

        //if (AppWindowTitleBar.IsCustomizationSupported())
        //{
        //    ExtendsContentIntoTitleBar = true;
        //    SetTitleBar(this.AppTitleBar);
        //}
        //else
        //{
        //    AppTitleBar.Visibility = Visibility.Collapsed;
        //    TitleRow.Height = new GridLength(0);
        //}

        //var hIcon = WinAPI.LoadImage(IntPtr.Zero, "Assets/MonitorControl.ico", 1, 32, 32, 0x00000010);
        //WinAPI.SendMessage(m_hWindow, 0x0080, 0, hIcon);

        //m_backdropHelper = new BackdropManager(this);
        //App.SettingManager.ThemeChanged += ThemeChanged;
        //ThemeChanged(null, App.SettingManager.ThemeEnum);

        //m_wndProc = WindowProc;
        //WinAPI.SetWindowSubclass(m_hWindow, m_wndProc, UIntPtr.Zero, UIntPtr.Zero);
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

    private void MonitorChanged(object sender, SelectionChangedEventArgs e)
    {

        var selected = (e.Source as System.Windows.Controls.ListView).SelectedItem;
        if (selected != null)
        {
            FnList.SelectedItem = null;
            ContentFrame.Content = selected;
        }
    }
    private void FnSelected(object sender, SelectionChangedEventArgs e)
    {
        var selected = (e.Source as System.Windows.Controls.ListView).SelectedItem;
        if (selected == SettingsFn)
        {
            ContentFrame.Content = 0;
        }
        else if (selected == ProfileFn)
        {
            ContentFrame.Content = 1;
        }

        if (selected != null)
        {
            MonitorList.SelectedItem = null;
        }
    }
}