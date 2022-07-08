using System;
using System.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Numerics;

namespace MonitorControl
{
    delegate bool GetState(IntPtr a, out uint min, out uint current, out uint max);
    public class Monitor : INotifyPropertyChanged, IDisposable
    {
        internal Monitor(WinAPI.PHYSICAL_MONITOR m, String description, String deviceId, Vector2 topLeft, int index)
        {
            hMonitor = m.hPhysicalMonitor;
            DeviceId = deviceId;
            TopLeft = topLeft;

            Description = string.Format("#{0}: {1}", index + 1, description);

            WinAPI.GetMonitorCapabilities(hMonitor, out WinAPI.MC_CAP mc, out WinAPI.MC_SUPPORTED_COLOR_TEMPERATURE _);
            for (int i = 0; i < 3 && mc == 0; i++)
            {
                WinAPI.GetMonitorCapabilities(hMonitor, out mc, out _);
                if (mc != 0)
                    break;
            }

            var f1 = () => { retriveItem(mc, WinAPI.MC_CAP.MC_CAPS_BRIGHTNESS, ref brightness, WinAPI.GetMonitorBrightness); };
            var f2 = () => { retriveItem(mc, WinAPI.MC_CAP.MC_CAPS_CONTRAST, ref contrast, WinAPI.GetMonitorContrast); };
            var f3 = () =>
            {
                retriveItem(mc, WinAPI.MC_CAP.MC_CAPS_RED_GREEN_BLUE_GAIN, ref red,
                (IntPtr h, out uint mi, out uint c, out uint ma) => WinAPI.GetMonitorRedGreenOrBlueGain(h, WinAPI.MC_GAIN_TYPE.MC_RED_GAIN, out mi, out c, out ma)
            );
            };
            var f4 = () =>
            {
                retriveItem(mc, WinAPI.MC_CAP.MC_CAPS_RED_GREEN_BLUE_GAIN, ref green,
                (IntPtr h, out uint mi, out uint c, out uint ma) => WinAPI.GetMonitorRedGreenOrBlueGain(h, WinAPI.MC_GAIN_TYPE.MC_GREEN_GAIN, out mi, out c, out ma)
            );
            };
            var f5 = () =>
            {
                retriveItem(mc, WinAPI.MC_CAP.MC_CAPS_RED_GREEN_BLUE_GAIN, ref blue,
                (IntPtr h, out uint mi, out uint c, out uint ma) => WinAPI.GetMonitorRedGreenOrBlueGain(h, WinAPI.MC_GAIN_TYPE.MC_BLUE_GAIN, out mi, out c, out ma)
            );
            };

            var f = new List<Action> { f1, f2, f3, f4, f5 };
            f.AsParallel().ForAll(f => f());

        }

        void retriveItem(WinAPI.MC_CAP mc, WinAPI.MC_CAP flag, ref Item item, GetState fn)
        {
            if (mc.HasFlag(flag))
                if (fn(hMonitor, out uint min, out uint current, out uint max))
                    item = new Item()
                    {
                        enabled = true,
                        current = current,
                        min = min,
                        range = max - min
                    };
                else
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            else
                item = new Item();
        }

        private uint getValue(Item item) => item.enabled ? (item.current - item.min) * 100 / item.range : 0;
        private void setValue(ref Item item, uint value, Func<IntPtr, uint, bool> fn)
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
        public string DeviceId { private set; get; }

        public Vector2 TopLeft { private set; get; }
        #endregion

        private struct Item
        {
            public Item() { }
            internal bool enabled = false;
            internal uint current = 0;
            internal uint min = 0;
            internal uint range = 0;
        }

        #region Private memebrs
        private Item brightness;
        private Item contrast;
        private Item red;
        private Item green;
        private Item blue;
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

        public void Dispose()
        {
            WinAPI.DestroyPhysicalMonitor(hMonitor);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

}
