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
        public string EditName { get; set; } = null;

        public bool IsEditing { get => EditName != null; }

        public void ConfirmEditByEnter(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (EditName != null && EditName.Length > 0)
                {
                    Instance.RenameProfile(Name, EditName);
                    SetEditing(false);
                    e.Handled = true;
                }
            }
        }

        public void ConfirmEdit(object sender, EventArgs e)
        {
            if (EditName != null && EditName.Length > 0)
            {
                Instance.RenameProfile(Name, EditName);
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
            if (isEditing)
            {
                EditName = Name;
            }
            else
            {
                EditName = null;
            }
            OnPropertyChanged("IsEditing");
            OnPropertyChanged("EditName");
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
