﻿using System;
using System.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MonitorControl
{
    delegate bool GetState(IntPtr a, out uint min, out uint current, out uint max);
    public class Monitor : INotifyPropertyChanged
    {

        internal Monitor(WinAPI.PHYSICAL_MONITOR m, String deviceName, int index)
        {
            hMonitor = m.hPhysicalMonitor;
            this.DeviceName = deviceName;

            Description = string.Format("#{0}: {1}", index + 1, new string(m.szPhysicalMonitorDescription.TakeWhile(c => c != 0).ToArray()));

            WinAPI.GetMonitorCapabilities(hMonitor, out uint mc, out uint _);

            void retriveItem(WinAPI.MC_CAP flag, ref (bool enabled, uint current, uint min, uint range) item, GetState fn)
            {
                item = (false, 0, 0, 0);
                if (((WinAPI.MC_CAP)mc).HasFlag(flag))
                    if (fn(hMonitor, out uint min, out uint current, out uint max))
                        item = (true, current, min, max - min);
                    else
                        throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            retriveItem(WinAPI.MC_CAP.MC_CAPS_BRIGHTNESS, ref brightness, WinAPI.GetMonitorBrightness);
            retriveItem(WinAPI.MC_CAP.MC_CAPS_CONTRAST, ref contrast, WinAPI.GetMonitorContrast);
            retriveItem(WinAPI.MC_CAP.MC_CAPS_RED_GREEN_BLUE_GAIN, ref red, (IntPtr h, out uint mi, out uint c, out uint ma) => WinAPI.GetMonitorRedGreenOrBlueGain(h, WinAPI.MC_GAIN_TYPE.MC_RED_GAIN, out mi, out c, out ma));
            retriveItem(WinAPI.MC_CAP.MC_CAPS_RED_GREEN_BLUE_GAIN, ref green, (IntPtr h, out uint mi, out uint c, out uint ma) => WinAPI.GetMonitorRedGreenOrBlueGain(h, WinAPI.MC_GAIN_TYPE.MC_GREEN_GAIN, out mi, out c, out ma));
            retriveItem(WinAPI.MC_CAP.MC_CAPS_RED_GREEN_BLUE_GAIN, ref blue, (IntPtr h, out uint mi, out uint c, out uint ma) => WinAPI.GetMonitorRedGreenOrBlueGain(h, WinAPI.MC_GAIN_TYPE.MC_BLUE_GAIN, out mi, out c, out ma));
        }

        ~Monitor() => WinAPI.DestroyPhysicalMonitor(hMonitor);

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
            set
            {
                setValue(ref brightness, value, WinAPI.SetMonitorBrightness);
                OnPropertyChanged("Brightness");
            }
        }

        public uint Contrast
        {
            get => getValue(contrast);
            set
            {
                setValue(ref contrast, value, WinAPI.SetMonitorContrast);
                OnPropertyChanged("Contrast");
            }
        }

        public uint Red
        {
            get => getValue(red);
            set
            {
                setValue(ref red, value, (h, v) => WinAPI.SetMonitorRedGreenOrBlueGain(h, WinAPI.MC_GAIN_TYPE.MC_RED_GAIN, v));
                OnPropertyChanged("Red");
            }
        }

        public uint Green
        {
            get => getValue(green);
            set
            {
                setValue(ref green, value, (h, v) => WinAPI.SetMonitorRedGreenOrBlueGain(h, WinAPI.MC_GAIN_TYPE.MC_GREEN_GAIN, v));
                OnPropertyChanged("Green");
            }
        }

        public uint Blue
        {
            get => getValue(blue);
            set
            {
                setValue(ref blue, value, (h, v) => WinAPI.SetMonitorRedGreenOrBlueGain(h, WinAPI.MC_GAIN_TYPE.MC_BLUE_GAIN, v));
                OnPropertyChanged("Blue");
            }
        }

        internal DeviceProfile Profile
        {
            get => new DeviceProfile() { Values = (Brightness, Contrast, Red, Green, Blue) };
            set => (Brightness, Contrast, Red, Green, Blue) = value.Values;
        }

        public string Description { private set; get; }
        public string DeviceName { private set; get; }
        #endregion

        #region Private memebrs
        private (bool, uint current, uint min, uint range) brightness;
        private (bool, uint current, uint min, uint range) contrast;
        private (bool, uint current, uint min, uint range) red;
        private (bool, uint current, uint min, uint range) green;
        private (bool, uint current, uint min, uint range) blue;
        private IntPtr hMonitor;
        #endregion

        #region PropertyChanged
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

}