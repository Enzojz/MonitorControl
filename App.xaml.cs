using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MonitorControl
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(Environment.ProcessPath);
            this.InitializeComponent();


            m_popupMenu = new PopupMenu();

            m_popupMenu.OpenWindow += OpenWindow;
            m_popupMenu.ExitApplication += ExitApplication;


            if (!Environment.GetCommandLineArgs().Contains("-silent"))
            {
                OpenWindow();
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (m_setting == null)
            {
                m_setting = new Setting();
            }

            if (m_instance == null)
            {
                m_instance = new InstanceCore();
            }
        }

        internal void OpenWindow()
        {
            if (m_window == null)
            {
                m_window = new MainWindow();
                m_window.Closed += MainWindowClosed;
                m_window.Activate();
            }
            else
            {
                m_window.Activate();
            }
        }

        private void MainWindowClosed(object sender, WindowEventArgs args)
        {
            m_window.Closed -= MainWindowClosed;
            m_window = null;
            if (!SettingManager.RunInBackground)
            {
                ExitApplication();
            }
        }

        internal void ExitApplication()
        {
            if (m_window != null)
                m_window.Close();

            if (m_instance != null)
                m_instance.Dispose();

            m_popupMenu.Close();
        }

        internal static InstanceCore Instance { get => m_instance; }

        internal static Setting SettingManager { get => m_setting; }

        private MainWindow m_window;
        private PopupMenu m_popupMenu;

        private static InstanceCore m_instance;
        private static Setting m_setting;
    }
}
