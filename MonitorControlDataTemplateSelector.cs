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

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject dp)
        {
            if (item is Monitor)
                return Monitor;
            if (item is List<ProfileState>)
                return Profile;
            else
                return null;
        }
    }
}
