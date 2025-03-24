using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Media3D;
using System.Windows.Shell;
using System.Windows.Threading;

namespace MonitorControl
{
    public partial class MonitorIdentifier : Window
    {
        public MonitorIdentifier(string label, Rectangle rect)
        {
            InitializeComponent();
            this.label.Text = label;

            WindowChrome.SetWindowChrome(
                this,
                new WindowChrome
                {
                    CaptionHeight = 0,
                    CornerRadius = default,
                    GlassFrameThickness = new Thickness(-1),
                    ResizeBorderThickness = default,
                    UseAeroCaptionButtons = true
                }
            );

            var helper = new WindowInteropHelper(this);
            helper.EnsureHandle();
            var m_hWindow = helper.Handle;

            WinAPI.SetWindowLongPtr(m_hWindow, -16, (IntPtr)(WinAPI.WS.WS_CAPTION));

            Loaded += (s, e) =>
            {
                WinAPI.SetWindowPos(m_hWindow, -1, (int)(rect.X + 50), (int)(rect.Y + 50), 0, 0, 0x0001);
            };

            var timer = new DispatcherTimer();
            timer.Tick += (s, e) => Close();
            timer.Interval = new TimeSpan(0, 0, 1);

            Show();
            Activate();
            timer.Start();
        }
    }
}
