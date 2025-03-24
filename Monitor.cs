using System;
using System.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Drawing;

namespace MonitorControl
{
    delegate bool GetState(IntPtr a, out uint min, out uint current, out uint max);
    public class Monitor : INotifyPropertyChanged, IDisposable
    {
        internal Monitor(WinAPI.PHYSICAL_MONITOR m, string description, string deviceId, Rectangle rect, int index)
        {
            hMonitor = m.hPhysicalMonitor;
            DeviceId = deviceId;
            Rect = rect;

            Description = $"#{index + 1}: {description}";
        }

        internal void Init()
        {
            for (int i = 0; i < 5 && mc == 0; i++)
            {
                WinAPI.GetMonitorCapabilities(hMonitor, out mc, out _);
                if (mc != 0)
                    break;
            }

            if (mc == 0)
                return;

            retriveDatas();

            OnPropertyChanged("IsReady");
            OnPropertyChanged("BrightnessSupported");
            OnPropertyChanged("ContrastSupported");
            OnPropertyChanged("RGBSupported");
        }

        private void retriveDatas()
        {
            var retriveDatas = new Action[] {
                () => retriveItem(mc, WinAPI.MC_CAP.MC_CAPS_BRIGHTNESS, ref brightness, WinAPI.GetMonitorBrightness),
                () => retriveItem(mc, WinAPI.MC_CAP.MC_CAPS_CONTRAST, ref contrast, WinAPI.GetMonitorContrast),
                () => retriveItem(mc, WinAPI.MC_CAP.MC_CAPS_RED_GREEN_BLUE_GAIN, ref red, WinAPI.GetMonitorRed),
                () => retriveItem(mc, WinAPI.MC_CAP.MC_CAPS_RED_GREEN_BLUE_GAIN, ref green, WinAPI.GetMonitorGreen),
                () => retriveItem(mc, WinAPI.MC_CAP.MC_CAPS_RED_GREEN_BLUE_GAIN, ref blue, WinAPI.GetMonitorBlue)
            };

            Task.WaitAll(retriveDatas.Select(Task.Run).ToArray());

            OnPropertyChanged("Brightness");
            OnPropertyChanged("Contrast");
            OnPropertyChanged("Red");
            OnPropertyChanged("Green");
            OnPropertyChanged("Blue");
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

        public bool BrightnessSupported { get => brightness.enabled; }
        public bool ContrastSupported { get => contrast.enabled; }
        public bool RGBSupported { get => red.enabled && blue.enabled && green.enabled; }
        public bool IsReady { get => BrightnessSupported && ContrastSupported && RGBSupported; }

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

        private WinAPI.MC_CAP mc = 0;

        public Rectangle Rect { private set; get; }
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
        internal void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Dispose()
        {
            WinAPI.DestroyPhysicalMonitor(hMonitor);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion
    }

}
