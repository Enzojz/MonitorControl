using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace MonitorControl;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        Environment.CurrentDirectory = Path.GetDirectoryName(Environment.ProcessPath);
        InitializeComponent();

        //Process.GetProcessesByName(current.ProcessName)
        //    .Where(p => p.Id != current.Id)
        //    .ToList()
        //    .ForEach(p =>
        //    {
        //        try
        //        {
        //            WinAPI.PostMessage(p.MainWindowHandle, WinAPI.WM.WM_QUIT, 0, 0);
        //        }
        //        catch
        //        {
        //            p.Kill();
        //        }
        //    });
    }

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


        m_trayMenu = new TrayMenu()
        {
            OpenWindow = OpenWindow,
            ExitApplication = ExitApplication,
            Visibility = Visibility.Hidden
        };

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

        m_trayMenu.Close();
    }


    internal static InstanceCore Instance { get => m_instance; }

    internal static Setting SettingManager { get => m_setting; }

    private MainWindow m_window;
    private TrayMenu m_trayMenu;

    private static InstanceCore m_instance;
    private static Setting m_setting;
}

