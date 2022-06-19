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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Profile : Page
    {
        public Profile()
        {
            this.InitializeComponent();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            App.Instance.SaveProfile(((Button)sender).DataContext as string);
        }
        private void OpenClick(object sender, RoutedEventArgs e)
        {
            App.Instance.LoadProfile(((Button)sender).DataContext as string);
        }

        internal MonitorFn Instance => App.Instance;

        private void NewProfilePreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                App.Instance.SaveProfile(((TextBox)sender).Text);
                e.Handled = true;
            }
        }
    }
}
