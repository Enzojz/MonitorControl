using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Diagnostics;

namespace MonitorControl
{
    internal class MonitorFn : INotifyPropertyChanged, IDisposable
    {

        public MonitorFn()
        {
            var displayConfigs = DisplayConfigs().ToDictionary(d => d.id);
            var displays = WinAPI.GetDisplays()
                .Select(display =>
                (
                    deviceName: display.DeviceName,
                    monitors: WinAPI.GetMonitorsFromDisplay(display)
                        .Select(monitor =>
                        {
                            var config = displayConfigs[monitor.DeviceID];
                            return (
                                description: monitor.DeviceString,
                                displayName: config.displayName,
                                output: config.output,
                                deviceId: monitor.DeviceID
                            );
                        })
                        .ToList()
                ))
                .ToDictionary(dm => dm.deviceName, dm => dm.monitors);

            var hMonitors = new List<IntPtr>();
            WinAPI.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                (IntPtr hMonitor, IntPtr hdcMonitor, ref WinAPI.Rect lprcMonitor, IntPtr dwData) =>
                {
                    hMonitors.Add(hMonitor);
                    return true;
                }, IntPtr.Zero);

            Monitors = hMonitors
                .SelectMany(hMonitor =>
                {
                    var monitorInfo = WinAPI.GetMonitorInfo(hMonitor);
                    var topLeft = new System.Numerics.Vector2(monitorInfo.Monitor.left, monitorInfo.Monitor.top);
                    var physicalMonitors = WinAPI.GetPhysicalMonitorsFromHMONITOR(hMonitor).ToList();
                    var monitors = displays.GetValueOrDefault(monitorInfo.DeviceName);
                    Debug.Assert(physicalMonitors.Count == monitors.Count);
                    return physicalMonitors.Zip(monitors).Select(m => (m.First, m.Second, topLeft));
                })
                .Select((m, i) =>
                {
                    var (physicalMonitor, monitor, center) = m;
                    var description = monitor.displayName ?? monitor.description ?? new string(physicalMonitor.szPhysicalMonitorDescription.TakeWhile(c => c != 0).ToArray());
                    return new Monitor(physicalMonitor, description, monitor.deviceId, center, i);
                })
                .ToList();

            ReadProfile();
            LoadProfile("Default");
        }

        public List<Monitor> Monitors
        {
            get;
            private set;
        }

        public ProfileState CurrentProfile
        {
            get { return currentProfile != null && profiles.ContainsKey(currentProfile) ? profiles[currentProfile] : null; }
            set
            {
                if (value != null && profiles.ContainsKey(value.Name) && value.Name != currentProfile)
                {
                    currentProfile = value.Name;
                    LoadProfile(currentProfile);
                }
            }
        }

        public List<ProfileState> Profiles => profiles.Values.ToList();

        private static IEnumerable<(string id, string displayName, WinAPI.DisplayConfigOutputTechnology output)> DisplayConfigs()
        {
            if (WinAPI.GetDisplayConfigs() is (WinAPI.DISPLAYCONFIG_PATH_INFO[] arrDisplayPaths, WinAPI.DISPLAYCONFIG_MODE_INFO[] arrDisplayModes))
            {
                var displayModes = arrDisplayModes
                    .Where(m => m.infoType == WinAPI.DisplayConfigModeInfoType.Target)
                    .ToDictionary(m => m.id);

                foreach (var displayPath in arrDisplayPaths)
                {
                    var displayMode = displayModes[displayPath.targetInfo.id];
                    if (displayMode.Equals(default(WinAPI.DISPLAYCONFIG_MODE_INFO)))
                        continue;

                    if (WinAPI.DisplayConfigGetDeviceInfo(displayMode) is WinAPI.DISPLAYCONFIG_TARGET_DEVICE_NAME deviceName)
                    {
                        yield return (
                                id: deviceName.monitorDevicePath,
                                displayName: deviceName.monitorFriendlyDeviceName,
                                output: deviceName.outputTechnology
                            );
                    }
                }
            }
            yield break;
        }

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
                        Monitors = Monitors.ToDictionary(m => m.DeviceId, m => m.Profile)
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
                    if (p.ContainsKey(m.DeviceId))
                        m.Profile = p[m.DeviceId];
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
                    profiles[profile].Profile.Monitors = Monitors.ToDictionary(m => m.DeviceId, m => m.Profile);
                }
                else
                {
                    profiles[profile] = new ProfileState(
                        profile,
                        new Profile()
                        {
                            Guid = Guid.NewGuid(),
                            Monitors = Monitors.ToDictionary(m => m.DeviceId, m => m.Profile)
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

        public void Dispose()
        {
            Monitors.ForEach(m => m.Dispose());
            Monitors.Clear();
        }
        #endregion
    }
}
