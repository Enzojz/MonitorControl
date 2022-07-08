﻿using Microsoft.UI.Xaml;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MonitorControl
{
    [DataContract]
    internal class SettingData
    {
        [DataMember]
        internal BackdropManager.BackdropType Theme = BackdropManager.BackdropType.Mica;
        [DataMember]
        internal string DefaultProfile = "Default";
        [DataMember]
        internal bool Autostart = false;
        [DataMember]
        internal bool ReloadProfile = false;
        [DataMember]
        internal bool RunInBackground = true;
        [DataMember]
        internal string ProfilePath = "profile.mcp";
    }

    internal class Setting : INotifyPropertyChanged
    {
        public Setting()
        {
            Load();

            if (Autostart)
            {
                var rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (rkApp.GetValue("MonitorControl", null) == null)
                {
                    m_data.Autostart = false;
                }
            }

        }

        private SettingData m_data;

        public delegate void BackdropEventHander(object sneder, BackdropManager.BackdropType type);
        public event BackdropEventHander ThemeChanged;

        public BackdropManager.BackdropType ThemeEnum { get => m_data.Theme; }

        internal string Theme
        {
            get => m_data.Theme.ToString();
            set
            {
                try
                {
                    var newValue = Enum.Parse<BackdropManager.BackdropType>(value);
                    m_data.Theme = newValue;
                    ThemeChanged(this, m_data.Theme);
                    Save();
                    App.Instance.Message = String.Format("Theme changed to {0}.", value);
                }
                catch
                {
                }
            }
        }

        internal string DefaultProfile
        {
            set
            {
                m_data.DefaultProfile = value;
                Save();
                App.Instance.Message = String.Format("Default profile changed to {0}.", value);
            }
            get => m_data.DefaultProfile ?? "Default";
        }

        internal string ProfilePath
        {
            set
            {
                m_data.ProfilePath = value;
                Save();
                App.Instance.Message = String.Format("Profile path changed to {0}.", value);
                OnPropertyChanged("ProfilePath");
            }
            get => m_data.ProfilePath;
        }

        internal bool Autostart
        {
            set
            {
                var rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (value)
                {
                    var q = System.Reflection.Assembly.GetExecutingAssembly();
                    var exePath = Path.ChangeExtension(System.Reflection.Assembly.GetExecutingAssembly().Location, "exe");
                    var autorunPath = String.Format("{0} -silent", exePath);
                    rkApp.SetValue("MonitorControl", autorunPath);
                }
                else
                {
                    rkApp.DeleteValue("MonitorControl", false);
                }

                m_data.Autostart = value;
                Save();
                App.Instance.Message = "Setting saved!";
            }
            get => m_data.Autostart;
        }

        internal bool ReloadProfile
        {
            set
            {
                m_data.ReloadProfile = value;
                Save();
                App.Instance.Message = "Setting saved!";
            }
            get => m_data.ReloadProfile;
        }

        internal bool RunInBackground
        {
            set
            {
                m_data.RunInBackground = value;
                Save();
                App.Instance.Message = "Setting saved!";
            }
            get => m_data.RunInBackground;
        }

        private void Save()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(
                typeof(SettingData),
                new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true }
            );
            using (var stream = File.CreateText("settings.json"))
            {
                ser.WriteObject(stream.BaseStream, m_data);
            }
        }

        private void Load()
        {
            if (File.Exists("settings.json"))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(
                    typeof(SettingData),
                    new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true }
                );
                using (var stream = File.OpenRead("settings.json"))
                {
                    try
                    {
                        m_data = (SettingData)ser.ReadObject(stream);
                    }
                    catch (SerializationException e)
                    {
                        m_data = new SettingData();
                        Save();
                    }
                }
            }
            else
            {
                m_data = new SettingData();
                Save();
            }
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
