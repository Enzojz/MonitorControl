using System.Windows.Controls;
using System.Windows;

namespace MonitorControl
{
    public class PopupMenuDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Item { get; set; }
        public DataTemplate Seperator { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject dp)
        {
            if ((item as PopupMenuItem).Text == null)
                return Seperator;
            else
                return Item;
        }
    }
}
