using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Diagnostics;
using System.Windows.Threading;

namespace MonitorControl
{
    public class InstanceCore : INotifyPropertyChanged, IDisposable
    {

        public InstanceCore()
        {
            m_timer = new DispatcherTimer();
            m_timer.Tick += (object? sender, EventArgs e) => { Message = null; };
            m_timer.Interval = new TimeSpan(0, 0, 3);

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

            var ws = new Stopwatch();
            ws.Start();

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
                    var description = (monitor.displayName != null && monitor.displayName.Length > 0) ?
                        monitor.displayName :
                        (monitor.description != null && monitor.description.Length > 0) ?
                            monitor.description :
                            new string(physicalMonitor.szPhysicalMonitorDescription.TakeWhile(c => c != 0).ToArray());
                    return new Monitor(physicalMonitor, description, monitor.deviceId, center, i);
                })
                .ToList();

            Task.WaitAll(Monitors.Select(m => Task.Run(m.Init)).ToArray());
            Monitors.ForEach(m => m.SetReady());

            ws.Stop();
            Debug.WriteLine(ws.Elapsed);

            ReadProfile(App.SettingManager.ProfilePath);
            if (App.SettingManager != null && App.SettingManager.ReloadProfile)
            {
                LoadProfile(App.SettingManager.DefaultProfile);
            }
        }

        DispatcherTimer m_timer;

        private string m_message;
        public string Message
        {
            get => m_message;
            set
            {
                m_message = value;
                OnPropertyChanged("Message");
                OnPropertyChanged("ShowMessage");

                if (value != null)
                {
                    m_timer.Start();
                } else
                {
                    m_timer.Stop();
                }

            }
        }

        public bool ShowMessage { get => m_message != null; set => m_message = null; }

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

        internal void WriteProfile(string path)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(
                typeof(Dictionary<String, Profile>),
                new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true }
            );
            var stream = File.CreateText(path);
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
            WriteProfile(App.SettingManager.ProfilePath);
        }

        internal void ReadProfile(string path)
        {
            if (File.Exists(path))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(
                    typeof(Dictionary<string, Profile>),
                    new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true }
                );
                using (var stream = File.OpenRead(path))
                {
                    try
                    {
                        var mapProfiles = (Dictionary<string, Profile>)ser.ReadObject(stream);
                        profiles = mapProfiles.ToDictionary(v => v.Key, v => new ProfileState(v.Key, v.Value));
                        OnPropertyChanged("Profiles");
                    }
                    catch (SerializationException e)
                    {
                        CreateNewProfile();
                    }
                }
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
                Message = $"Profile {profile} has been loaded!";
            }
        }

        public bool CanSaveProfile(string profile)
        {
            return profile != null && profile.Length > 0 && !profiles.ContainsKey(profile);
        }

        public void SaveProfile(string profile)
        {
            if (profile.Length > 0)
            {
                if (profiles.ContainsKey(profile))
                {
                    profiles[profile].Profile.Monitors = Monitors.ToDictionary(m => m.DeviceId, m => m.Profile);
                    Message = $"Profile {profile} has been saved!";
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
                    Message = $"Profile {profile} has been created!";
                }
                WriteProfile(App.SettingManager.ProfilePath);
            }
        }

        public void RenameProfile(string oldName, string newName)
        {
            if (oldName != newName)
            {
                profiles[newName] = profiles[oldName];
                profiles[newName].Name = newName;
                profiles.Remove(oldName);
                WriteProfile(App.SettingManager.ProfilePath);
                OnPropertyChanged("Profiles");
                Message = $"Profile {oldName} has been renamed to {newName}!";
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
                WriteProfile(App.SettingManager.ProfilePath);
                OnPropertyChanged("Profiles");
                Message = $"Profile {profile} has been saved!";
                if (currentProfile == profile)
                {
                    currentProfile = null;
                    OnPropertyChanged("CurrentProfile");
                }
            }
        }



        #region Private members

        string currentProfile { get; set; }

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
