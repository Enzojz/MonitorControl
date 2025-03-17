using System.Windows.Controls;
using System.Windows;

namespace MonitorControl
{
    public class MonitorControlDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Monitor { get; set; }
        public DataTemplate Profile { get; set; }
        public DataTemplate Setting { get; set; }
        public DataTemplate Home { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject dp)
        {
            if (item is Monitor)
                return Monitor;
            if (item is int && (int)item == 1)
                return Profile;
            else if (item is int && (int)item == 0)
                return Setting;
            else
                return Home;
        }
    }
}
