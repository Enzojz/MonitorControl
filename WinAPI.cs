using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MonitorControl
{
    internal class WinAPI
    {
        private const int CCHDEVICENAME = 32;
        internal const uint EDD_GET_DEVICE_INTERFACE_NAME = 0x00000001;
        internal const uint QDC_ONLY_ACTIVE_PATHS = 2;
        internal const int ERROR_SUCCESS = 0;

        [Flags]
        internal enum MC_CAP
        {
            MC_CAPS_BRIGHTNESS = 0x2,
            MC_CAPS_CONTRAST = 0x4,
            MC_CAPS_COLOR_TEMPERATURE = 0x8,
            MC_CAPS_RED_GREEN_BLUE_GAIN = 0x10,
            MC_CAPS_RED_GREEN_BLUE_DRIVE = 0x20
        }

        internal enum MC_GAIN_TYPE
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



        internal enum DisplayConfigOutputTechnology : uint
        {
            Other = 0xFFFFFFFF,
            HD15 = 0,
            SVideo = 1,
            CompositeVideo = 2,
            ComponenetVideo = 3,
            DVI = 4,
            HDMI = 5,
            LVDS = 6,
            DJpn = 8,
            SDI = 9,
            DisplayPortExtern = 10,
            DisplayPortEmbedded = 11,
            UDIExtern = 12,
            UDIEmbeded = 13,
            SDTV = 14,
            Miracast = 15,
            IndirectWired = 16,
            IndirectVirtual = 17,
            Internal = 0x80000000,
            ForceUint32 = 0xFFFFFFFF
        }

        internal enum DisplayConfigModeInfoType : uint
        {
            Source = 1,
            Target = 2,
            DesktopImage = 3,
            ForceUint32 = 0xFFFFFFFF
        }

        internal enum DisplayConfigRotation : uint
        {
            Identity = 1,
            Rotate90 = 2,
            Rotate180 = 3,
            Rotata270 = 4,
            ForceUint32 = 0xFFFFFFFF
        }

        internal enum DISPLAYCONFIG_DEVICE_INFO_TYPE : uint
        {
            DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
            DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4,
            DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5,
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6,
            DISPLAYCONFIG_DEVICE_INFO_GET_SUPPORT_VIRTUAL_RESOLUTION = 7,
            DISPLAYCONFIG_DEVICE_INFO_SET_SUPPORT_VIRTUAL_RESOLUTION = 8,
            DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO = 9,
            DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE = 10,
            DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL = 11,
            DISPLAYCONFIG_DEVICE_INFO_FORCE_UINT32 = 0xFFFFFFFF
        }

        [Flags()]
        internal enum DisplayDeviceStateFlags : int
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct PHYSICAL_MONITOR
        {
            internal IntPtr hPhysicalMonitor;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 128)]
            internal char[] szPhysicalMonitorDescription;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            internal int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            internal string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            internal string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            internal DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            internal string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            internal string DeviceKey;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Point
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct Rect
        {
            internal int left;
            internal int top;
            internal int right;
            internal int bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct MonitorInfoEx
        {
            internal int Size;
            internal Rect Monitor;
            internal Rect WorkArea;
            internal uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            internal string DeviceName;
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_PATH_INFO
        {
            public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
            public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
            public uint flags;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct DISPLAYCONFIG_MODE_INFO
        {
            [FieldOffset(0)]
            public DisplayConfigModeInfoType infoType;

            [FieldOffset(4)]
            public uint id;

            [FieldOffset(8)]
            public LUID adapterId;

            [FieldOffset(16)]
            public DISPLAYCONFIG_TARGET_MODE targetMode;

            [FieldOffset(16)]
            public DISPLAYCONFIG_SOURCE_MODE sourceMode;

            [FieldOffset(16)]
            public DISPLAYCONFIG_DESKTOP_IMAGE_INFO desktopImageInfo;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct DISPLAYCONFIG_TARGET_DEVICE_NAME
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;
            public DisplayConfigOutputTechnology outputTechnology;
            public ushort edidManufactureId;
            public ushort edidProductCodeId;
            public uint connectorInstance;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string monitorFriendlyDeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string monitorDevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_PATH_SOURCE_INFO
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;
            public uint statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_PATH_TARGET_INFO
        {
            public LUID adapterId;
            public uint id;
            public uint modeInfoIdx;
            public DisplayConfigOutputTechnology outputTechnology;
            public DisplayConfigRotation rotation;
            public uint scaling;
            public DISPLAYCONFIG_RATIONAL refreshRate;
            public uint scanLineOrdering;

            [MarshalAs(UnmanagedType.Bool)]
            public bool targetAvailable;

            public uint statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_TARGET_MODE
        {
            public DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
        {
            public ulong pixelRate;
            public DISPLAYCONFIG_RATIONAL hSyncFreq;
            public DISPLAYCONFIG_RATIONAL vSyncFreq;
            public DISPLAYCONFIG_2DREGION activeSize;
            public DISPLAYCONFIG_2DREGION totalSize;
            public uint videoStandard;
            public uint scanLineOrdering;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_RATIONAL
        {
            public uint Numerator;
            public uint Denominator;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_2DREGION
        {
            public uint cx;
            public uint cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_SOURCE_MODE
        {
            public uint width;
            public uint height;
            public uint pixelFormat;
            public Point position;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_DESKTOP_IMAGE_INFO
        {
            public Point PathSourceSize;
            public Rect DesktopImageRegion;
            public Rect DesktopImageClip;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_DEVICE_INFO_HEADER
        {
            public DISPLAYCONFIG_DEVICE_INFO_TYPE type;
            public uint size;
            public LUID adapterId;
            public uint id;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
        {
            public uint value;
        }

        internal enum MonitorDpiType
        {
            MDT_EFFECTIVE_DPI = 0,
            MDT_ANGULAR_DPI = 1,
            MDT_RAW_DPI = 2,
        }

        [DllImport("user32.dll")]
        internal extern static bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private extern static bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

        internal delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll", SetLastError = false)]
        private extern static IntPtr MonitorFromPoint(System.Drawing.Point pt, uint dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private extern static bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        internal extern static uint GetDpiForWindow(IntPtr hWindow);

        [DllImport("user32.dll")]
        private extern static int GetDisplayConfigBufferSizes(uint flags, out uint numPathArrayElements, out uint numModeInfoArrayElements);

        [DllImport("user32.dll")]
        private extern static int QueryDisplayConfig(uint flags, ref uint numPathArrayElements, [Out] DISPLAYCONFIG_PATH_INFO[] pathInfoArray, ref uint numModeInfoArrayElements, [Out] DISPLAYCONFIG_MODE_INFO[] modeInfoArray, IntPtr currentTopologyId);

        [DllImport("user32.dll")]
        private extern static int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME requestPacket);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, out uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", SetLastError = true)]
        internal extern static bool DestroyPhysicalMonitor(IntPtr hMonitor);

        [DllImport("dxva2.dll", SetLastError = true)]
        internal extern static bool GetMonitorCapabilities(IntPtr hMonitor, out uint pdwMonitorCapabilities, out uint pdwSupportedColorTemperatures);

        [DllImport("dxva2.dll", SetLastError = true)]
        internal extern static bool GetMonitorBrightness(IntPtr hMonitor, out uint pdwMinimumBrightness, out uint pdwCurrentBrightness, out uint pdwMaximumBrightness);

        [DllImport("dxva2.dll", SetLastError = true)]
        internal extern static bool SetMonitorBrightness(IntPtr hMonitor, uint dwNewBrightness);

        [DllImport("dxva2.dll", SetLastError = true)]
        internal extern static bool GetMonitorContrast(IntPtr hMonitor, out uint pdwMinimumContrast, out uint pdwCurrentContrast, out uint pdwMaximumContrast);

        [DllImport("dxva2.dll", SetLastError = true)]
        internal extern static bool SetMonitorContrast(IntPtr hMonitor, uint dwNewContrast);

        [DllImport("dxva2.dll", SetLastError = true)]
        internal extern static bool GetMonitorRedGreenOrBlueGain(IntPtr hMonitor, MC_GAIN_TYPE gtGainType, out uint pdwMinimumGain, out uint pdwCurrentGain, out uint pdwMaximumGain);

        [DllImport("dxva2.dll", SetLastError = true)]
        internal extern static bool SetMonitorRedGreenOrBlueGain(IntPtr hMonitor, MC_GAIN_TYPE gtGainType, uint dwNewGain);

        [DllImport("shcore.dll")]
        internal static extern uint GetDpiForMonitor(IntPtr hMonitor, MonitorDpiType dpiType, out uint dpiX, out uint dpiY);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, nuint wParam, nint lParam);

        internal static (DISPLAYCONFIG_PATH_INFO[] displayPaths, DISPLAYCONFIG_MODE_INFO[] displayModes)? GetDisplayConfigs()
        {
            if (GetDisplayConfigBufferSizes(
                QDC_ONLY_ACTIVE_PATHS,
                out uint pathCount,
                out uint modeCount) != ERROR_SUCCESS)
                return null;

            var displayPaths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            var displayModes = new DISPLAYCONFIG_MODE_INFO[modeCount];

            if (QueryDisplayConfig(
                QDC_ONLY_ACTIVE_PATHS,
                ref pathCount,
                displayPaths,
                ref modeCount,
                displayModes,
                IntPtr.Zero) != ERROR_SUCCESS)
                return null;

            return (
                displayPaths,
                displayModes
            );
        }

        internal static DISPLAYCONFIG_TARGET_DEVICE_NAME? DisplayConfigGetDeviceInfo(DISPLAYCONFIG_MODE_INFO displayMode)
        {
            var deviceName = new DISPLAYCONFIG_TARGET_DEVICE_NAME
            {
                header = new DISPLAYCONFIG_DEVICE_INFO_HEADER
                {
                    size = (uint)Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>(),
                    adapterId = displayMode.adapterId,
                    id = displayMode.id,
                    type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
                }
            };

            if (DisplayConfigGetDeviceInfo(ref deviceName) != ERROR_SUCCESS)
                return null;

            return deviceName;
        }

        internal static IEnumerable<DISPLAY_DEVICE> GetDisplays()
        {
            var size = Marshal.SizeOf<DISPLAY_DEVICE>();
            var display = new DISPLAY_DEVICE { cb = size };

            for (uint i = 0; EnumDisplayDevices(null, i, ref display, WinAPI.EDD_GET_DEVICE_INTERFACE_NAME); i++)
            {
                if (!display.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
                    continue;
                yield return display;
            }
        }

        internal static IEnumerable<DISPLAY_DEVICE> GetMonitorsFromDisplay(DISPLAY_DEVICE display)
        {
            var size = Marshal.SizeOf<DISPLAY_DEVICE>();
            var monitor = new DISPLAY_DEVICE { cb = size };

            for (uint i = 0; WinAPI.EnumDisplayDevices(display.DeviceName, i, ref monitor, EDD_GET_DEVICE_INTERFACE_NAME); i++)
            {
                if (!monitor.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
                    continue;
                yield return monitor;
            }
        }

        internal static MonitorInfoEx GetMonitorInfo(IntPtr hMonitor)
        {
            MonitorInfoEx monitorInfo = new MonitorInfoEx
            {
                Size = 40 + 2 * CCHDEVICENAME,
                DeviceName = string.Empty
            };
            GetMonitorInfo(hMonitor, ref monitorInfo);
            return monitorInfo;
        }

        internal static PHYSICAL_MONITOR[] GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor)
        {
            GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, out uint NumberOfPhysicalMonitors);
            var PhysicalMonitors = new PHYSICAL_MONITOR[NumberOfPhysicalMonitors];
            GetPhysicalMonitorsFromHMONITOR(hMonitor, NumberOfPhysicalMonitors, PhysicalMonitors);
            return PhysicalMonitors;
        }
    }
}
