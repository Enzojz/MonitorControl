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

            Profiles = new Dictionary<String, Dictionary<string, DeviceProfile>>();

            Action createNewProfile = () =>
            {
                if (Profiles.Count == 0 || !Profiles.ContainsKey("Default"))
                    Profiles["Default"] = Monitors.ToDictionary(m => m.DeviceName, m => m.Profile);
                WriteProfile();
            };

            if (File.Exists(filepath))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(
                    typeof(Dictionary<String, Dictionary<string, DeviceProfile>>),
                    new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true }
                );
                bool hasException = false;
                var stream = File.OpenRead(filepath);
                try
                {
                    Profiles = (Dictionary<String, Dictionary<string, DeviceProfile>>)ser.ReadObject(stream);
                }
                catch (SerializationException e)
                {
                    hasException = true;
                }
                stream.Close();
                if (hasException)
                    createNewProfile();
            }
            else
                createNewProfile();

            LoadProfile("Default");

            //IsHidden = true;
        }

        public bool IsHidden { set; get; }

        public Monitor[] Monitors
        {
            get;
            private set;
        }

        private void WriteProfile()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(
                typeof(Dictionary<String, Dictionary<string, DeviceProfile>>),
                new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true }
            );
            var stream = File.CreateText(filepath);
            ser.WriteObject(stream.BaseStream, Profiles);
            stream.Close();
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
            Profiles[profile] = Monitors.ToDictionary(m => m.DeviceName, m => m.Profile);
            WriteProfile();
            currentProfile = profile;
        }

        public Dictionary<String, Dictionary<string, DeviceProfile>> Profiles { set; get; }


        #region Private members

        string currentProfile;
        const string filepath = "profile.json";

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
