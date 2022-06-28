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

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                this.ExtendsContentIntoTitleBar = true;
                this.SetTitleBar(this.AppTitleBar);
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
                TitleRow.Height = new GridLength(0);
            }

            this.Title = "Monitor Control";

            m_backdropHelper = new BackdropManager(this);
            App.SettingManager.ThemeChanged += ThemeChanged;
            ThemeChanged(null, App.SettingManager.ThemeEnum);
        }

        private void ThemeChanged(object sender, BackdropManager.BackdropType backdrop)
        {
            m_backdropHelper.SetBackdrop(backdrop);
            MainGrid.Background = App.SettingManager.ThemeEnum == BackdropManager.BackdropType.Classic ? Application.Current.Resources["ApplicationPageBackgroundThemeBrush"] as SolidColorBrush : null;
        }

        internal MonitorFn Instance => App.Instance;

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
