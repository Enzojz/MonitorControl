using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Shell; // For DllImport

namespace MonitorControl;

[ValueConversion(typeof(double), typeof(double))]
public class HalfConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return 0.5 * (double)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        WindowChrome.SetWindowChrome(
            this,
            new WindowChrome
            {
                CaptionHeight = 42,
                CornerRadius = default,
                GlassFrameThickness = new Thickness(-1),
                ResizeBorderThickness = ResizeMode == ResizeMode.NoResize ? default : new Thickness(4),
                UseAeroCaptionButtons = true
            }
        );

        var helper = new WindowInteropHelper(this);
        helper.EnsureHandle();
        m_hWindow = helper.Handle;

        m_wndProc = WindowProc;
        WinAPI.SetWindowSubclass(m_hWindow, m_wndProc, UIntPtr.Zero, UIntPtr.Zero);
    }
    private IntPtr WindowProc(IntPtr hWnd, WinAPI.WM msg, IntPtr wParam, IntPtr lParam, UIntPtr uIdSubclass, UIntPtr dwRefData)
    {
        switch (msg)
        {
            case WinAPI.WM.WM_DISPLAYCHANGE:
                App.Instance.EnumMonitors();
                break;
        }
        return WinAPI.DefSubclassProc(hWnd, msg, wParam, lParam);
    }

    private IntPtr m_hWindow;

    private WinAPI.SUBCLASSPROC m_wndProc;

    private void MonitorChanged(object sender, SelectionChangedEventArgs e)
    {

        var selected = (e.Source as ListView).SelectedItem;
        if (selected != null)
        {
            FnList.SelectedItem = null;
            ContentFrame.Content = selected;
        }
    }
    private void FnSelected(object sender, SelectionChangedEventArgs e)
    {
        var selected = (e.Source as ListView).SelectedItem;
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

    private void MonitorClick(object sender, RoutedEventArgs e)
    {
        MonitorList.SelectedItem = (sender as Button).DataContext;
    }
}