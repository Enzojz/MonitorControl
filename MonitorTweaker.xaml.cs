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

            DataContextChanged += (s, e) =>
            {
                if (Monitor != null)
                {
                    new MonitorIdentifier(Monitor.Description, Monitor.TopLeft);
                }
            };
        }
        public Monitor Monitor => DataContext as Monitor;
    }
}
