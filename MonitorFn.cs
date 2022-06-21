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

        public ProfileState CurrentProfile
        {
            get { return currentProfile != null && profiles.ContainsKey(currentProfile) ? profiles[currentProfile] : null; }
            set {
                if (value != null && profiles.ContainsKey(value.Name) && value.Name != currentProfile)
                {
                    currentProfile = value.Name;
                    LoadProfile(currentProfile);
                }
            }
        }

        public List<ProfileState> Profiles => profiles.Values.ToList();

        private void WriteProfile()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(
                typeof(Dictionary<String, Profile>),
                new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true }
            );
            var stream = File.CreateText(filepath);
            ser.WriteObject(stream.BaseStream, profiles.ToDictionary(v => v.Key, v => v.Value.Profile));
            stream.Close();
        }




        private void CreateNewProfile()
        {
            if (profiles.Count == 0 || !profiles.ContainsKey("Default"))
            {
                profiles["Default"] = new ProfileState("Default",
                    new Profile()
                    {
                        Guid = System.Guid.NewGuid(),
                        Monitors = Monitors.ToDictionary(m => m.DeviceName, m => m.Profile)
                    }
                );
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
                    var mapProfiles = (Dictionary<String, Profile>)ser.ReadObject(stream);
                    profiles = mapProfiles.ToDictionary(v => v.Key, v => new ProfileState(v.Key, v.Value));
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
            if (profiles.ContainsKey(profile))
            {
                currentProfile = profile;
                var p = profiles[profile].Profile.Monitors;
                foreach (var m in Monitors)
                {
                    if (p.ContainsKey(m.DeviceName))
                        m.Profile = p[m.DeviceName];
                }
                OnPropertyChanged("CurrentProfile");
            }
        }

        public void SaveProfile(string profile)
        {
            if (profile.Length > 0)
            {
                if (profiles.ContainsKey(profile))
                {
                    profiles[profile].Profile.Monitors = Monitors.ToDictionary(m => m.DeviceName, m => m.Profile);
                }
                else
                {
                    profiles[profile] = new ProfileState(
                        profile,
                        new Profile()
                        {
                            Guid = Guid.NewGuid(),
                            Monitors = Monitors.ToDictionary(m => m.DeviceName, m => m.Profile)
                        });
                    OnPropertyChanged("Profiles");
                }
                WriteProfile();
            }
        }

        public void RenameProfile(string oldName, string newName)
        {
            if (oldName != newName)
            {
                profiles[newName] = profiles[oldName];
                profiles[newName].Name = newName;
                profiles.Remove(oldName);
                WriteProfile();
                OnPropertyChanged("Profiles");
                if (currentProfile == oldName)
                {
                    currentProfile = newName;
                    OnPropertyChanged("CurrentProfile");
                }
            }
        }

        public void RemoveProfile(string profile)
        {
            if (profile != "Default")
            {
                profiles.Remove(profile);
                WriteProfile();
                OnPropertyChanged("Profiles");
                if (currentProfile == profile)
                {
                    currentProfile = null;
                    OnPropertyChanged("CurrentProfile");
                }
            }
        }



        #region Private members

        string currentProfile;
        const string filepath = "profile.json";
        Dictionary<String, ProfileState> profiles = new Dictionary<String, ProfileState>();

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
