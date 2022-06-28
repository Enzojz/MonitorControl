using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

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
        internal bool Autostart = true;
        [DataMember]
        internal bool ReloadProfile = false;
        [DataMember]
        internal bool RunInBackground = true;
    }

    internal class SettingManager
    {
        public SettingManager()
        {
            Load();
        }

        public delegate void BackdropEventHander(object sneder, BackdropManager.BackdropType type);
        public event BackdropEventHander ThemeChanged;

        public BackdropManager.BackdropType ThemeEnum { get => m_data.Theme; }

        public string Theme
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
                }
                catch
                {
                }
            }
        }

        private SettingData m_data;

        public string DefaultProfile
        {
            set { m_data.DefaultProfile = value; }
            get => m_data.DefaultProfile;
        }

        public bool Autostart
        {
            set { m_data.Autostart = value; }
            get => m_data.Autostart;
        }

        public bool ReloadProfile
        {
            set { m_data.ReloadProfile = value; }
            get => m_data.ReloadProfile;
        }

        public bool RunInBackground
        {
            set { m_data.RunInBackground = value; }
            get => m_data.RunInBackground;
        }

        private void Save()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(
                typeof(SettingData),
                new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true }
            );
            var stream = File.CreateText("settings.json");
            ser.WriteObject(stream.BaseStream, m_data);
            stream.Close();
        }

        private void Load()
        {
            if (File.Exists("settings.json"))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(
                    typeof(SettingData),
                    new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true }
                );
                var stream = File.OpenRead("settings.json");
                try
                {
                    m_data = (SettingData)ser.ReadObject(stream);
                }
                catch (SerializationException e)
                {
                    m_data = new SettingData();
                }
                stream.Close();
            } else
            {
                m_data = new SettingData();
            }
        }
    }
}
