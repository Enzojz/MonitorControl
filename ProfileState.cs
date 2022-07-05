using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;

namespace MonitorControl
{
    internal class ProfileState : INotifyPropertyChanged
    {
        internal ProfileState(string name, Profile profile)
        {
            Name = name;
            Profile = profile;
        }

        internal Profile Profile;

        internal Visibility TextBlockVisibility => IsEditing ? Visibility.Collapsed : Visibility.Visible;
        internal Visibility TextBoxVisibility => !IsEditing ? Visibility.Collapsed : Visibility.Visible;
        internal Visibility SaveVisibility => (IsActive && !IsEditing) ? Visibility.Visible : Visibility.Collapsed;
        internal Visibility LoadVisibility => (IsActive && !IsEditing) ? Visibility.Visible : Visibility.Collapsed;
        internal Visibility RemoveVisibility => (IsActive && !IsEditing) && Name != "Default" ? Visibility.Visible : Visibility.Collapsed;
        internal Visibility EditVisibility => (IsActive && !IsEditing) && Name != "Default" ? Visibility.Visible : Visibility.Collapsed;

        public string Name { get; internal set; }

        private bool IsEditing;

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

        public void ConfirmEditByEnter(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var newName = (sender as TextBox).Text;
                if (newName != null && newName.Length > 0)
                {
                    IsEditing = false;
                    if (Name == null || Name.Length == 0)
                    {
                        Instance.SaveProfile(newName);
                        Name = null;
                        OnPropertyChanged("Name");
                    }
                    else
                    {
                        Instance.RenameProfile(Name, (sender as TextBox).Text);
                        e.Handled = true;
                    }
                }
            }
        }

        public void ConfirmEdit(object sender, RoutedEventArgs e)
        {
            var newName = (sender as Button).Tag.ToString();
            if (newName != null && newName.Length > 0)
            {
                IsEditing = false;
                Instance.RenameProfile(Name, newName);
            }
            Notify();
        }

        public void CancelEdit(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
            Notify();
        }

        public void Remove(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
            Instance.RemoveProfile(Name);
        }

        public void Edit(object sender, RoutedEventArgs e)
        {
            IsEditing = true;
            Notify();
        }

        public void Save(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
            if (Name != null && Name.Length > 0)
            {
                Instance.SaveProfile(Name);
            }
            Notify();
        }

        private InstanceCore Instance => App.Instance;
        private void Notify()
        {
            OnPropertyChanged("TextBlockVisibility");
            OnPropertyChanged("TextBoxVisibility");
            OnPropertyChanged("SaveVisibility");
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
