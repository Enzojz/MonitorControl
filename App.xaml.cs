using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
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
            System.Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            this.InitializeComponent();
            m_holder = new ProcessHolder();
            m_holder.Closed += HolderClosed;
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
                m_setting = new SettingManager();
            }

            if (m_instance == null)
            {
                m_instance = new InstanceCore();
            }

            m_trayIcon = new TrayIcon();
            m_trayIcon.ShowWindow = () =>
            {
                if (m_holder != null)
                {
                    m_holder.OpenWindow();
                }
            };

            m_trayIcon.Exit = () =>
            {
                if (m_holder != null)
                {
                    m_holder.CloseWindow();
                    m_holder.Close();
                }
            };
        }

        private void HolderClosed(object sender, WindowEventArgs args)
        {
            if (m_instance != null)
                m_instance.Dispose();
            m_trayIcon.Dispose();
        }

        internal static InstanceCore Instance { get => m_instance; }

        internal static SettingManager SettingManager { get => m_setting; }

        private ProcessHolder m_holder;
        private TrayIcon m_trayIcon;

        private static InstanceCore m_instance;
        private static SettingManager m_setting;
    }
}
