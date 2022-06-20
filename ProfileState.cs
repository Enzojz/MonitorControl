using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorControl
{
    internal class ProfileState : INotifyPropertyChanged
    {
        internal ProfileState(Guid guid, String name)
        {
            Guid = guid;
            Name = name;
        }

        internal Visibility TextBlockVisibility => IsEditing ? Visibility.Collapsed : Visibility.Visible;
        internal Visibility TextBoxVisibility => !IsEditing ? Visibility.Collapsed : Visibility.Visible;
        internal Visibility SaveVisibility => (IsActive && !IsEditing) ? Visibility.Visible : Visibility.Collapsed;
        internal Visibility LoadVisibility => (IsActive && !IsEditing) ? Visibility.Visible : Visibility.Collapsed;
        internal Visibility RemoveVisibility => (IsActive && !IsEditing) && Name != "Default" ? Visibility.Visible : Visibility.Collapsed;
        internal Visibility EditVisibility => (IsActive && !IsEditing) && Name != "Default" ? Visibility.Visible : Visibility.Collapsed;

        internal String Name;

        private Guid Guid;

        private bool IsEditing;

        private bool IsCurrent;

        private bool IsActive;


        public void PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            IsActive = true;
            Notify();

        }
        public void PointerExited(object sender, PointerRoutedEventArgs e)
        {
            IsActive = false;
            Notify();
        }

        public void Tapped(object sender, TappedRoutedEventArgs e)
        {
        }

        public void ConfirmEditByEnter(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                IsEditing = false;
                if (Name == null || Name.Length == 0)
                {
                    Instance.SaveProfile((sender as TextBox).Text);
                    Name = null;
                    OnPropertyChanged("Name");
                }
                else
                    Instance.RenameProfile(Name, (sender as TextBox).Text);
                e.Handled = true;
            }
        }

        public void ConfirmEdit(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
            Instance.RenameProfile(Name, (sender as Button).Tag.ToString());
            Notify();
        }

        public void CancelEdit(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
            Notify();
        }

        public void Load(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
            Instance.LoadProfile(Name);
        }

        public void Remove(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
            Instance.RemoveProfile(Name);
        }


        public void Rename(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
            Instance.RenameProfile(Name, (sender as Button).Tag.ToString());
        }

        public void Edit(object sender, RoutedEventArgs e)
        {
            IsEditing = true;
            Notify();
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
            if (Name == null || Name.Length == 0)
            {
                Instance.SaveProfile((sender as Button).Tag.ToString());
                Name = null;
                OnPropertyChanged("Name");
            }
            else 
                Instance.SaveProfile(Name);
            Notify();
        }

        private MonitorFn Instance => App.Instance;
        private void Notify()
        {
            OnPropertyChanged("TextBlockVisibility");
            OnPropertyChanged("TextBoxVisibility");
            OnPropertyChanged("SaveVisibility");
            OnPropertyChanged("LoadVisibility");
            OnPropertyChanged("RemoveVisibility");
            OnPropertyChanged("EditVisibility");
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
