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

            // Check to see if customization is supported.
            // Currently only supported on Windows 11.
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                this.ExtendsContentIntoTitleBar = true;  // enable custom titlebar
                this.SetTitleBar(this.AppTitleBar);      // set user ui element as titlebar
            }
            else
            {
                // Title bar customization using these APIs is currently
                // supported only on Windows 11. In other cases, hide
                // the custom title bar element.
                AppTitleBar.Visibility = Visibility.Collapsed;
            }

            m_backdropHelper = new BackdropManager(this);
            m_backdropHelper.SetBackdrop(App.SettingManager.ThemeEnum);

            App.SettingManager.ThemeChanged += ThemeChanged;
        }

        private void ThemeChanged(object sender, BackdropManager.BackdropType backdrop)
        {
            m_backdropHelper.SetBackdrop(backdrop);
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
            this.ContentFrame.Content = null;
        }
    }
}
