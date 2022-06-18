using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using Microsoft.UI;


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
        }

        internal MonitorFn Instance => App.Instance;
    }


    public class MonitorControlDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Monitor { get; set; }
        public DataTemplate Profile { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject dp)
        {
            if (item is Monitor)
            {
                return Monitor;
            }
            else if (item is String)
            {
                return Profile;
            } else
            {
                return null;
            }
        }
    }


}
