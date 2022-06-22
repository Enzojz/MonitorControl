using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

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
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            if (m_instance == null)
            {
                m_instance = new MonitorFn();
            }

            m_window = new MainWindow();
            m_window.Closed += WindowClosed;

            var hWindow = WinRT.Interop.WindowNative.GetWindowHandle(m_window);
            var idWindow = Win32Interop.GetWindowIdFromWindow(hWindow);
            float dpi = WinAPI.GetDpiForWindow(hWindow);
            var dpiScaling = dpi / 96;
            var appWindow = AppWindow.GetFromWindowId(idWindow);

            int baseHeight = m_instance.Monitors.Count * 40 + 190;
            
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = (int)(900 * dpiScaling), Height = (int)((baseHeight < 380 ? 380 : baseHeight) * dpiScaling)});

            var presenter = appWindow.Presenter as OverlappedPresenter;
            //presenter.IsResizable = false;

            m_window.Activate();

        }

        private void WindowClosed(object sender, WindowEventArgs args)
        {
            if (m_instance != null)
                m_instance.Dispose();
        }

        internal static MonitorFn Instance => m_instance;

        private Window m_window;
        private static MonitorFn m_instance;
    }
}
