using Microsoft.UI;
using Microsoft.UI.Windowing;
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
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MonitorControl
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MonitorIdentifier : Window
    {
        public MonitorIdentifier(string label, Vector2 topLeft)
        {
            this.Label = label;
            this.InitializeComponent();

            var hWindow = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var idWindow = Win32Interop.GetWindowIdFromWindow(hWindow);
            var appWindow = AppWindow.GetFromWindowId(idWindow);

            var pos = appWindow.Position;
            pos.X = (int)(topLeft.X);
            pos.Y = (int)(topLeft.Y);
            appWindow.Move(pos);

            float dpi = WinAPI.GetDpiForWindow(hWindow);
            var dpiScaling = dpi / 96;

            pos.X = (int)(topLeft.X + 50 * dpiScaling);
            pos.Y = (int)(topLeft.Y + 50 * dpiScaling);
            appWindow.Move(pos);

            appWindow.Resize(new SizeInt32 { Width = (int)(500 * dpiScaling), Height = (int)(75 * dpiScaling) });

            var presenter = appWindow.Presenter as OverlappedPresenter;
            presenter.IsResizable = false;

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                ExtendsContentIntoTitleBar = true;  // enable custom titlebar
            }

            m_backdropHelper = new BackdropManager(this);
            m_backdropHelper.SetBackdrop(BackdropManager.BackdropType.Acrylic);

            //var timer = new DispatcherTimer();
            //timer.Tick += (object sender, object e) => Close();
            //timer.Interval = new TimeSpan(0, 0, 1);
            //timer.Start();
        }

        public string Label { private set; get; }

        private BackdropManager m_backdropHelper;
    }
}
