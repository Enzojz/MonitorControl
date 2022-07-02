using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace MonitorControl
{
    public class PopupMenuDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Item { get; set; }
        public DataTemplate Seperator { get; set; }
        
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject dp)
        {
            if ((item as PopupMenuItem).Text == null)
                return Seperator;
            else
                return Item;
        }
    }
}
