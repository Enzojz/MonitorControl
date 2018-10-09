﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;

namespace MonitorControl
{
    #region JSON datas
    [DataContract]
    public class DeviceProfile
    {
        [DataMember(Order = 1)]
        public uint Brightness { set; get; }

        [DataMember(Order = 2)]
        public uint Contrast { set; get; }

        [DataMember(Order = 3)]
        public uint Red { set; get; }

        [DataMember(Order = 4)]
        public uint Green { set; get; }

        [DataMember(Order = 5)]
        public uint Blue { set; get; }

        public (uint, uint, uint, uint, uint) Values
        {
            get => (Brightness, Contrast, Red, Green, Blue);
            set => (Brightness, Contrast, Red, Green, Blue) = value;
        }
    }
    #endregion

    class MonitorFn
    {
        #region WINAPI Import
        [Flags]
        public enum MC_CAP
        {
            MC_CAPS_BRIGHTNESS = 0x2,
            MC_CAPS_CONTRAST = 0x4,
            MC_CAPS_COLOR_TEMPERATURE = 0x8,
            MC_CAPS_RED_GREEN_BLUE_GAIN = 0x10,
            MC_CAPS_RED_GREEN_BLUE_DRIVE = 0x20
        }

        public enum MC_GAIN_TYPE
        {
            MC_RED_GAIN = 0x00,
            MC_GREEN_GAIN = 0x01,
            MC_BLUE_GAIN = 0x02,
        }

        private const int MC_SUPPORTED_COLOR_TEMPERATURE_NONE = 0x00;
        private const int MC_SUPPORTED_COLOR_TEMPERATURE_4000K = 0x01;
        private const int MC_SUPPORTED_COLOR_TEMPERATURE_5000K = 0x02;
        private const int MC_SUPPORTED_COLOR_TEMPERATURE_6500K = 0x04;
        private const int MC_SUPPORTED_COLOR_TEMPERATURE_7500K = 0x08;
        private const int MC_SUPPORTED_COLOR_TEMPERATURE_8200K = 0x10;
        private const int MC_SUPPORTED_COLOR_TEMPERATURE_9300K = 0x20;
        private const int MC_SUPPORTED_COLOR_TEMPERATURE_10000K = 0x40;
        private const int MC_SUPPORTED_COLOR_TEMPERATURE_11500K = 0x80;

        [StructLayout(LayoutKind.Sequential)]
        public struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 128)]
            public char[] szPhysicalMonitorDescription;
        }

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x10,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [DllImport("user32.dll", SetLastError = false)]
        private extern static IntPtr MonitorFromPoint(System.Drawing.Point pt, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, out uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool DestroyPhysicalMonitor(IntPtr hMonitor);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetMonitorCapabilities(IntPtr hMonitor, out uint pdwMonitorCapabilities, out uint pdwSupportedColorTemperatures);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetMonitorBrightness(IntPtr hMonitor, out uint pdwMinimumBrightness, out uint pdwCurrentBrightness, out uint pdwMaximumBrightness);

        [DllImport("dxva2.dll", SetLastError = true)]
        public extern static bool SetMonitorBrightness(IntPtr hMonitor, uint dwNewBrightness);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetMonitorContrast(IntPtr hMonitor, out uint pdwMinimumContrast, out uint pdwCurrentContrast, out uint pdwMaximumContrast);

        [DllImport("dxva2.dll", SetLastError = true)]
        public extern static bool SetMonitorContrast(IntPtr hMonitor, uint dwNewContrast);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetMonitorRedGreenOrBlueGain(IntPtr hMonitor, MC_GAIN_TYPE gtGainType, out uint pdwMinimumGain, out uint pdwCurrentGain, out uint pdwMaximumGain);

        [DllImport("dxva2.dll", SetLastError = true)]
        public extern static bool SetMonitorRedGreenOrBlueGain(IntPtr hMonitor, MC_GAIN_TYPE gtGainType, uint dwNewGain);

        #endregion

        delegate bool GetState(IntPtr a, out uint min, out uint current, out uint max);

        public class Monitor
        {
            public Monitor(PHYSICAL_MONITOR m, string DeviceName, int index)
            {
                hMonitor = m.hPhysicalMonitor;
                this.DeviceName = DeviceName;
                Description = string.Format("#{0}: {1}", index + 1, new string(m.szPhysicalMonitorDescription.TakeWhile(c => c != 0).ToArray()));

                GetMonitorCapabilities(hMonitor, out uint mc, out uint _);

                void retriveItem(MC_CAP flag, ref (bool enabled, uint current, uint min, uint range) item, GetState fn)
                {
                    item = (false, 0, 0, 0);
                    if (((MC_CAP)mc).HasFlag(flag))
                        if (fn(hMonitor, out uint min, out uint current, out uint max))
                            item = (true, current, min, max - min);
                        else
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                retriveItem(MC_CAP.MC_CAPS_BRIGHTNESS, ref brightness, GetMonitorBrightness);
                retriveItem(MC_CAP.MC_CAPS_CONTRAST, ref contrast, GetMonitorContrast);
                retriveItem(MC_CAP.MC_CAPS_RED_GREEN_BLUE_GAIN, ref red, (IntPtr h, out uint mi, out uint c, out uint ma) => GetMonitorRedGreenOrBlueGain(h, MC_GAIN_TYPE.MC_RED_GAIN, out mi, out c, out ma));
                retriveItem(MC_CAP.MC_CAPS_RED_GREEN_BLUE_GAIN, ref green, (IntPtr h, out uint mi, out uint c, out uint ma) => GetMonitorRedGreenOrBlueGain(h, MC_GAIN_TYPE.MC_GREEN_GAIN, out mi, out c, out ma));
                retriveItem(MC_CAP.MC_CAPS_RED_GREEN_BLUE_GAIN, ref blue, (IntPtr h, out uint mi, out uint c, out uint ma) => GetMonitorRedGreenOrBlueGain(h, MC_GAIN_TYPE.MC_BLUE_GAIN, out mi, out c, out ma));
            }

            ~Monitor() => DestroyPhysicalMonitor(hMonitor);

            private uint getValue((bool enabled, uint current, uint min, uint range) item) => item.enabled ? (item.current - item.min) * 100 / item.range : 0;
            private void setValue(ref (bool enabled, uint current, uint min, uint range) item, uint value, Func<IntPtr, uint, bool> fn)
            {
                if (item.enabled)
                {

                    var newVal = (uint)(value * 0.01 * item.range) + item.min;
                    if (newVal != item.current)
                    {
                        item.current = newVal;
                        if (!fn(hMonitor, item.current))
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
            }

            #region Properties
            public uint Brightness
            {
                get => getValue(brightness);
                set => setValue(ref brightness, value, SetMonitorBrightness);
            }

            public uint Contrast
            {
                get => getValue(contrast);
                set => setValue(ref contrast, value, SetMonitorContrast);
            }

            public uint Red
            {
                get => getValue(red);
                set => setValue(ref red, value, (h, v) => SetMonitorRedGreenOrBlueGain(h, MC_GAIN_TYPE.MC_RED_GAIN, v));
            }

            public uint Green
            {
                get => getValue(green);
                set => setValue(ref green, value, (h, v) => SetMonitorRedGreenOrBlueGain(h, MC_GAIN_TYPE.MC_GREEN_GAIN, v));
            }

            public uint Blue
            {
                get => getValue(blue);
                set => setValue(ref blue, value, (h, v) => SetMonitorRedGreenOrBlueGain(h, MC_GAIN_TYPE.MC_BLUE_GAIN, v));
            }

            public DeviceProfile Profile
            {
                get => new DeviceProfile() { Values = (Brightness, Contrast, Red, Green, Blue) };
                set => (Brightness, Contrast, Red, Green, Blue) = value.Values;
            }

            public string Description { private set; get; }
            public string DeviceName { private set; get; }
            private (bool, uint current, uint min, uint range) brightness;
            private (bool, uint current, uint min, uint range) contrast;
            private (bool, uint current, uint min, uint range) red;
            private (bool, uint current, uint min, uint range) green;
            private (bool, uint current, uint min, uint range) blue;
            private IntPtr hMonitor;
            #endregion

        }

        public MonitorFn()
        {
            Monitors = Screen.AllScreens
                .Select(screen =>
                {
                    var hMonitor = MonitorFromPoint(screen.Bounds.Location, 2);
                    GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, out uint NumberOfPhysicalMonitors);
                    var PhysicalMonitors = new PHYSICAL_MONITOR[NumberOfPhysicalMonitors];
                    GetPhysicalMonitorsFromHMONITOR(hMonitor, NumberOfPhysicalMonitors, PhysicalMonitors);
                    return PhysicalMonitors.Select(m => (monitor: m, deviceName: screen.DeviceName));
                })
                .SelectMany(s => s)
                .Select((m, i) => new Monitor(m.monitor, m.deviceName, i))
                .ToArray();

            Profiles = new Dictionary<String, Dictionary<string, DeviceProfile>>();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(
                typeof(Dictionary<String, Dictionary<string, DeviceProfile>>),
                new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true }
            );

            string filepath = "profile.json";
            if (File.Exists(filepath))
            {
                var stream = File.OpenRead(filepath);
                Profiles = (Dictionary<String, Dictionary<string, DeviceProfile>>)ser.ReadObject(stream);
                stream.Close();
            }
            else
            {
                if (Profiles.Count == 0)
                    Profiles["Default"] = Monitors.ToDictionary(m => m.DeviceName, m => m.Profile);
                var stream = File.CreateText(filepath);
                ser.WriteObject(stream.BaseStream, Profiles);
                stream.Close();
            }

            LoadProfile("Default");

            {
                notifyIcon = new NotifyIcon()
                {
                    Text = "Monitor Control",
                    Visible = true,
                    ContextMenu = new ContextMenu()
                };

                using (Stream stream = System.Windows.Application.GetResourceStream(new Uri("brightness.ico", UriKind.Relative)).Stream)
                {
                    notifyIcon.Icon = new System.Drawing.Icon(stream);
                }

                notifyIcon.ContextMenu.Popup += (s, e) =>
                {
                    notifyIcon.ContextMenu.MenuItems.Clear();
                    foreach (var k in Profiles.Keys)
                    {
                        notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem(k, (se, ev) => LoadProfile(k)));
                    }
                };
            }
        }

        ~MonitorFn()
        {
            notifyIcon.Icon.Dispose();
            notifyIcon.Dispose();
            notifyIcon = null;
        }

        public Monitor[] Monitors
        {
            get;
            private set;
        }

        public void LoadProfile(string profile)
        {
            if (Profiles.ContainsKey(profile))
            {
                var p = Profiles[profile];
                foreach (var m in Monitors)
                {
                    if (p.ContainsKey(m.DeviceName))
                        m.Profile = p[m.DeviceName];
                }
            }
        }

        public Dictionary<String, Dictionary<string, DeviceProfile>> Profiles { set; get; }

        NotifyIcon notifyIcon;
    }
}
