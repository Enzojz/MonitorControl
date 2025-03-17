using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MonitorControl
{
    public class ProfileState : INotifyPropertyChanged
    {
        internal ProfileState(string name, Profile profile)
        {
            Name = name;
            Profile = profile;
        }

        internal Profile Profile;

        public Visibility TextBlockVisibility => IsEditing ? Visibility.Collapsed : Visibility.Visible;
        public Visibility TextBoxVisibility => !IsEditing ? Visibility.Collapsed : Visibility.Visible;
        public Visibility SaveVisibility => (IsActive && !IsEditing) ? Visibility.Visible : Visibility.Collapsed;
        public Visibility LoadVisibility => (IsActive && !IsEditing) ? Visibility.Visible : Visibility.Collapsed;
        public Visibility RemoveVisibility => (IsActive && !IsEditing) && Name != "Default" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility EditVisibility => (IsActive && !IsEditing) && Name != "Default" ? Visibility.Visible : Visibility.Collapsed;

        public string Name { get; internal set; }

        public bool IsEditing { get; set; } = false;

        public bool IsActive { get; set; } = false;

        public void PointerEntered(object sender, EventArgs e)
        {
            IsActive = true;
            Notify();

        }
        public void PointerExited(object sender, EventArgs e)
        {
            IsActive = false;
            Notify();
        }

        public void ConfirmEditByEnter(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var newName = (sender as System.Windows.Controls.TextBox).Text;
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
                        Instance.RenameProfile(Name, (sender as System.Windows.Controls.TextBox).Text);
                        e.Handled = true;
                    }
                }
            }
        }

        public void ConfirmEdit(object sender, EventArgs e)
        {
            var newName = (sender as System.Windows.Controls.Button).Tag.ToString();
            if (newName != null && newName.Length > 0)
            {
                IsEditing = false;
                Instance.RenameProfile(Name, newName);
            }
            Notify();
        }

        public void CancelEdit(object sender, EventArgs e)
        {
            IsEditing = false;
            Notify();
        }

        public void Remove(object sender, EventArgs e)
        {
            IsEditing = false;
            Instance.RemoveProfile(Name);
        }

        public void Edit(object sender, EventArgs e)
        {
            IsEditing = true;
            Notify();
        }

        public void Save(object sender, EventArgs e)
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
