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
    public sealed partial class ProcessHolder : Window
    {
        public ProcessHolder()
        {
            this.InitializeComponent();
        }

        internal void OpenWindow()
        {
            if (m_window == null)
            {
                m_window = new MainWindow();
                MainWindow.BeforeActivated(m_window);
                m_window.Closed += WindowClosed;
                m_window.Activate();
            }
        }

        internal void CloseWindow()
        {
            if (m_window != null)
            {
                m_window.Close();
            }
        }

        private void WindowClosed(object sender, WindowEventArgs args)
        {
            m_window = null;
            if (!App.SettingManager.RunInBackground)
            {
                this.Close();
            }
        }

        MainWindow window
        {
            get => m_window;
        }

        private MainWindow m_window;
    }
}
