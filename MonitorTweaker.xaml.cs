using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MonitorControl
{
    /// <summary>
    /// MonitorTweaker.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorTweaker : Page
    {
        public MonitorTweaker()
        {
            InitializeComponent();

            //DataContextChanged += (s, e) => Bindings.Update();
            //DataContextChanged += (s, e) =>
            //{
            //    var win = new MonitorIdentifier(Monitor.Description, Monitor.TopLeft);
            //    win.Activate();
            //};
        }
        public Monitor Monitor => this.DataContext as Monitor;
    }
}
