using System.Diagnostics;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;

namespace MonitorControl
{
    public partial class App : Application
    {
        App()
        {
            var current = Process.GetCurrentProcess();
            Process.GetProcessesByName(current.ProcessName)
                .Except(new List<Process>() { current })
                .ToList()
                .ForEach(p =>
                {
                    try { p.Close(); } catch { p.Kill(); }
                });
        }


        void startup(object sender, StartupEventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString();
            Directory.SetCurrentDirectory(path);
            Fn = new MonitorFn(e.Args.Length == 1 ? e.Args[0] : null);

            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = Fn;
        }
        private void exit(object sender, ExitEventArgs e)
        {
            Fn.OnExit();
        }

        private MonitorFn Fn { set; get; }
    }
}
