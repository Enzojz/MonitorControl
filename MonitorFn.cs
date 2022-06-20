using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace MonitorControl
{
    internal class MonitorFn : INotifyPropertyChanged
    {
        public MonitorFn()
        {
            var hMonitors = new List<IntPtr>();

            WinAPI.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                (IntPtr hMonitor, IntPtr hdcMonitor, ref WinAPI.Rect lprcMonitor, IntPtr dwData) =>
                {
                    hMonitors.Add(hMonitor);
                    return true;
                }, IntPtr.Zero);

            Monitors = hMonitors
                .Select(hMonitor =>
                {
                    var minfo = new WinAPI.MonitorInfoEx();
                    minfo.Init();
                    WinAPI.GetMonitorInfo(hMonitor, ref minfo);

                    WinAPI.GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, out uint NumberOfPhysicalMonitors);
                    var PhysicalMonitors = new WinAPI.PHYSICAL_MONITOR[NumberOfPhysicalMonitors];
                    WinAPI.GetPhysicalMonitorsFromHMONITOR(hMonitor, NumberOfPhysicalMonitors, PhysicalMonitors);

                    return PhysicalMonitors.Select(m => (monitor: m, deviceName: minfo.DeviceName));
                })
                .SelectMany(s => s)
                .Select((m, i) => new Monitor(m.monitor, m.deviceName, i))
                .ToArray();


            ReadProfile();
            LoadProfile("Default");
        }

        public Monitor[] Monitors
        {
            get;
            private set;
        }

        public List<ProfileState> Profiles => profiles.Select(v => new ProfileState(v.Value.Guid, v.Key)).ToList();

        private void WriteProfile()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(
                typeof(Dictionary<String, Profile>),
                new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true }
            );
            var stream = File.CreateText(filepath);
            ser.WriteObject(stream.BaseStream, profiles);
            stream.Close();
        }




        private void CreateNewProfile()
        {
            if (profiles.Count == 0 || !profiles.ContainsKey("Default"))
            {
                profiles["Default"] = new Profile()
                {
                    Guid = System.Guid.NewGuid(),
                    Monitors = Monitors.ToDictionary(m => m.DeviceName, m => m.Profile)
                };
            }
            WriteProfile();
        }

        private void ReadProfile()
        {
            if (File.Exists(filepath))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(
                    typeof(Dictionary<String, Profile>),
                    new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true }
                );
                bool hasException = false;
                var stream = File.OpenRead(filepath);
                try
                {
                    profiles = (Dictionary<String, Profile>)ser.ReadObject(stream);
                }
                catch (SerializationException e)
                {
                    hasException = true;
                }
                stream.Close();
                if (hasException)
                    CreateNewProfile();
            }
            else
                CreateNewProfile();
        }

        public void LoadProfile(string profile)
        {
            //if (Profiles.ContainsKey(profile))
            //{
            //    var p = Profiles[profile];
            //    foreach (var m in Monitors)
            //    {
            //        if (p.ContainsKey(m.DeviceName))
            //            m.Profile = p[m.DeviceName];
            //    }
            //    currentProfile = profile;
            //}
        }

        public void SaveProfile(string profile)
        {
            if (profiles.ContainsKey(profile))
            {
                profiles[profile].Monitors = Monitors.ToDictionary(m => m.DeviceName, m => m.Profile);
            }
            else
            {
                profiles[profile] = new Profile()
                {
                    Guid = Guid.NewGuid(),
                    Monitors = Monitors.ToDictionary(m => m.DeviceName, m => m.Profile)
                };
            }
            WriteProfile();
            currentProfile = profile;
            OnPropertyChanged("Profiles");
        }

        public void RenameProfile(string oldName, string newName)
        {
            if (currentProfile == oldName)
                currentProfile = newName;
            profiles[newName] = profiles[oldName];
            profiles.Remove(oldName);
            WriteProfile();
            OnPropertyChanged("Profiles");
        }

        public void RemoveProfile(string profile)
        {
            if (profile != "Default")
            {
                if (currentProfile == profile)
                {
                    currentProfile = "Default";
                }
                profiles.Remove(profile);
                WriteProfile();
                OnPropertyChanged("Profiles");
            }
        }



        #region Private members

        string currentProfile;
        const string filepath = "profile.json";
        Dictionary<String, Profile> profiles = new Dictionary<String, Profile>();

        #endregion

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
