using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorControl
{
    public class PopupMenuItem
    {
        internal string Text { get; set; }

        internal Action Callback { get; set; }

        internal void Clicked (object sender, RoutedEventArgs e)
        {
            if (Callback != null)
            {
                Callback();
            };
        }

        internal bool IsChecked { get; set; } = false;

    }
}
