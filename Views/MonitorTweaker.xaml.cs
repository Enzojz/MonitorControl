﻿using Microsoft.UI;
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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MonitorControl
{
    public sealed partial class MonitorTweaker : Page
    {
        public MonitorTweaker()
        {
            this.InitializeComponent();
            DataContextChanged += (s, e) => Bindings.Update();
            DataContextChanged += (s, e) =>
            {
                var win = new MonitorIdentifier(Monitor.Description, Monitor.TopLeft);
                win.Activate();
            };

        }


        public Monitor Monitor => DataContext as Monitor;
    }
}