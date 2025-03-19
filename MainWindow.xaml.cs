using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell; // For DllImport

namespace MonitorControl;

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
    }    

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
}