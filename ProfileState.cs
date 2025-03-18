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

        public string Name { get; internal set; }

        public bool IsEditing { get; private set; } = false;

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
                Instance.RenameProfile(Name, newName);
                SetEditing(false);
            }
        }

        public void CancelEdit(object sender, EventArgs e)
        {
            SetEditing(false);
        }

        public void Remove(object sender, EventArgs e)
        {
            SetEditing(false);
            Instance.RemoveProfile(Name);
        }

        public void Edit(object sender, EventArgs e)
        {
            SetEditing(true);
        }

        public void Save(object sender, EventArgs e)
        {
            if (Name != null && Name.Length > 0)
            {
                Instance.SaveProfile(Name);
            }
            SetEditing(false);
        }

        private InstanceCore Instance => App.Instance;
        private void SetEditing(bool isEditing)
        {
            IsEditing = isEditing;
            OnPropertyChanged("IsEditing");
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
