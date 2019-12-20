using System.Diagnostics;
using System.Windows;
using System.Linq;
using System.Collections.Generic;

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
                .ForEach(p => {
                    try { p.Close(); } catch { p.Kill(); }

                });
        }
    }
}
