using System.Windows.Controls;

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
