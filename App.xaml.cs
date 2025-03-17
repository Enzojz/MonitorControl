using System.Configuration;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows;

namespace MonitorControl;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    public App()
    {
        Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(Environment.ProcessPath);
        //this.InitializeComponent();


        //m_popupMenu = new PopupMenu();

        //m_popupMenu.OpenWindow += OpenWindow;
        //m_popupMenu.ExitApplication += ExitApplication;

    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnStartup(StartupEventArgs args)
    {
        string path = AppDomain.CurrentDomain.BaseDirectory.ToString();
        Directory.SetCurrentDirectory(path);

        if (m_setting == null)
        {
            m_setting = new Setting();
        }

        if (m_instance == null)
        {
            m_instance = new InstanceCore();
        }

        if (!Environment.GetCommandLineArgs().Contains("-silent"))
        {
            OpenWindow();
        }
    }

    internal void OpenWindow()
    {
        if (m_window == null)
        {
            m_window = new MainWindow();
            m_window.DataContext = m_instance;
            m_window.Closed += MainWindowClosed;
            m_window.Show();
            m_window.Activate();
        }
        else
        {
            m_window.Show();
            m_window.Activate();
        }
    }

    private void MainWindowClosed(object sender, EventArgs args)
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

        //m_popupMenu.Close();
    }

    internal static InstanceCore Instance { get => m_instance; }

    internal static Setting SettingManager { get => m_setting; }

    private MainWindow m_window;
    //private PopupMenu m_popupMenu;

    private static InstanceCore m_instance;
    private static Setting m_setting;
}

