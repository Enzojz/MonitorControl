using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace MonitorControl
{
    public class MonitorControlDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Monitor { get; set; }
        public DataTemplate Profile { get; set; }
        public DataTemplate Setting { get; set; }
        public DataTemplate Home { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject dp)
        {
            if (item is Monitor)
                return Monitor;
            if (item is List<ProfileState>)
                return Profile;
            else if (item is int && ((int)item == 0))
                return Setting;
            else
                return Home;
        }
    }
}
