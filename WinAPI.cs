using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MonitorControl
{
    internal class WinAPI
    {
        #region types
        private const int CCHDEVICENAME = 32;
        internal const uint EDD_GET_DEVICE_INTERFACE_NAME = 0x00000001;
        internal const uint QDC_ONLY_ACTIVE_PATHS = 2;
        internal const int ERROR_SUCCESS = 0;

        #region monitors
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
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            PrimaryDevice = 0x4,
            MirroringDriver = 0x8,
            VGACompatible = 0x10,
            Removable = 0x20,
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
            internal int x;
            internal int y;
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
            internal DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
            internal DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
            internal uint flags;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct DISPLAYCONFIG_MODE_INFO
        {
            [FieldOffset(0)]
            internal DisplayConfigModeInfoType infoType;

            [FieldOffset(4)]
            internal uint id;

            [FieldOffset(8)]
            internal LUID adapterId;

            [FieldOffset(16)]
            internal DISPLAYCONFIG_TARGET_MODE targetMode;

            [FieldOffset(16)]
            internal DISPLAYCONFIG_SOURCE_MODE sourceMode;

            [FieldOffset(16)]
            internal DISPLAYCONFIG_DESKTOP_IMAGE_INFO desktopImageInfo;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct DISPLAYCONFIG_TARGET_DEVICE_NAME
        {
            internal DISPLAYCONFIG_DEVICE_INFO_HEADER header;
            internal DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS flags;
            internal DisplayConfigOutputTechnology outputTechnology;
            internal ushort edidManufactureId;
            internal ushort edidProductCodeId;
            internal uint connectorInstance;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            internal string monitorFriendlyDeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            internal string monitorDevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_PATH_SOURCE_INFO
        {
            internal LUID adapterId;
            internal uint id;
            internal uint modeInfoIdx;
            internal uint statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_PATH_TARGET_INFO
        {
            internal LUID adapterId;
            internal uint id;
            internal uint modeInfoIdx;
            internal DisplayConfigOutputTechnology outputTechnology;
            internal DisplayConfigRotation rotation;
            internal uint scaling;
            internal DISPLAYCONFIG_RATIONAL refreshRate;
            internal uint scanLineOrdering;

            [MarshalAs(UnmanagedType.Bool)]
            internal bool targetAvailable;

            internal uint statusFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LUID
        {
            internal uint LowPart;
            internal int HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_TARGET_MODE
        {
            internal DISPLAYCONFIG_VIDEO_SIGNAL_INFO targetVideoSignalInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO
        {
            internal ulong pixelRate;
            internal DISPLAYCONFIG_RATIONAL hSyncFreq;
            internal DISPLAYCONFIG_RATIONAL vSyncFreq;
            internal DISPLAYCONFIG_2DREGION activeSize;
            internal DISPLAYCONFIG_2DREGION totalSize;
            internal uint videoStandard;
            internal uint scanLineOrdering;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_RATIONAL
        {
            internal uint Numerator;
            internal uint Denominator;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_2DREGION
        {
            internal uint cx;
            internal uint cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_SOURCE_MODE
        {
            internal uint width;
            internal uint height;
            internal uint pixelFormat;
            internal Point position;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_DESKTOP_IMAGE_INFO
        {
            internal Point PathSourceSize;
            internal Rect DesktopImageRegion;
            internal Rect DesktopImageClip;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_DEVICE_INFO_HEADER
        {
            internal DISPLAYCONFIG_DEVICE_INFO_TYPE type;
            internal uint size;
            internal LUID adapterId;
            internal uint id;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
        {
            internal uint value;
        }

        #endregion

        #region windows

        internal enum NotifyIconMessage : uint
        {
            NIM_ADD = 0x00000000,
            NIM_MODIFY = 0x00000001,
            NIM_DELETE = 0x00000002,
            NIM_SETFOCUS = 0x00000003,
            NIM_SETVERSION = 0x00000004,
        }

        internal enum NotifyFlags
        {
            NIF_MESSAGE = 0x00000001,
            NIF_ICON = 0x00000002,
            NIF_TIP = 0x00000004,
            NIF_STATE = 0x00000008,
            NIF_INFO = 0x00000010,
            NIF_GUID = 0x00000020,
            NIF_REALTIME = 0x00000040,
            NIF_SHOWTIP = 0x00000080
        }

        internal const int GWLP_WNDPROC = -4;

        internal enum WM : uint
        {
            WM_NULL = 0x0000,
            WM_CREATE = 0x0001,
            WM_DESTROY = 0x0002,
            WM_MOVE = 0x0003,
            WM_SIZE = 0x0005,
            WM_ACTIVATE = 0x0006,
            WM_SETFOCUS = 0x0007,
            WM_KILLFOCUS = 0x0008,
            WM_ENABLE = 0x000A,
            WM_SETREDRAW = 0x000B,
            WM_SETTEXT = 0x000C,
            WM_GETTEXT = 0x000D,
            WM_GETTEXTLENGTH = 0x000E,
            WM_PAINT = 0x000F,
            WM_CLOSE = 0x0010,
            WM_QUERYENDSESSION = 0x0011,
            WM_QUERYOPEN = 0x0013,
            WM_ENDSESSION = 0x0016,
            WM_QUIT = 0x0012,
            WM_ERASEBKGND = 0x0014,
            WM_SYSCOLORCHANGE = 0x0015,
            WM_SHOWWINDOW = 0x0018,
            WM_WININICHANGE = 0x001A,
            WM_SETTINGCHANGE = WM_WININICHANGE,
            WM_DEVMODECHANGE = 0x001B,
            WM_ACTIVATEAPP = 0x001C,
            WM_FONTCHANGE = 0x001D,
            WM_TIMECHANGE = 0x001E,
            WM_CANCELMODE = 0x001F,
            WM_SETCURSOR = 0x0020,
            WM_MOUSEACTIVATE = 0x0021,
            WM_CHILDACTIVATE = 0x0022,
            WM_QUEUESYNC = 0x0023,
            WM_GETMINMAXINFO = 0x0024,
            WM_PAINTICON = 0x0026,
            WM_ICONERASEBKGND = 0x0027,
            WM_NEXTDLGCTL = 0x0028,
            WM_SPOOLERSTATUS = 0x002A,
            WM_DRAWITEM = 0x002B,
            WM_MEASUREITEM = 0x002C,
            WM_DELETEITEM = 0x002D,
            WM_VKEYTOITEM = 0x002E,
            WM_CHARTOITEM = 0x002F,
            WM_SETFONT = 0x0030,
            WM_GETFONT = 0x0031,
            WM_SETHOTKEY = 0x0032,
            WM_GETHOTKEY = 0x0033,
            WM_QUERYDRAGICON = 0x0037,
            WM_COMPAREITEM = 0x0039,
            WM_GETOBJECT = 0x003D,
            WM_COMPACTING = 0x0041,

            [Obsolete("Obsolete for Win32 Based Applications")]
            WM_COMMNOTIFY = 0x0044,
            WM_WINDOWPOSCHANGING = 0x0046,
            WM_WINDOWPOSCHANGED = 0x0047,

            [Obsolete("Provided only for compatibility with 16-bit Windows-based applications")]
            WM_POWER = 0x0048,
            WM_COPYDATA = 0x004A,
            WM_CANCELJOURNAL = 0x004B,
            WM_NOTIFY = 0x004E,
            WM_INPUTLANGCHANGEREQUEST = 0x0050,
            WM_INPUTLANGCHANGE = 0x0051,
            WM_TCARD = 0x0052,
            WM_HELP = 0x0053,
            WM_USERCHANGED = 0x0054,
            WM_NOTIFYFORMAT = 0x0055,
            WM_CONTEXTMENU = 0x007B,
            WM_STYLECHANGING = 0x007C,
            WM_STYLECHANGED = 0x007D,
            WM_DISPLAYCHANGE = 0x007E,
            WM_GETICON = 0x007F,
            WM_SETICON = 0x0080,
            WM_NCCREATE = 0x0081,
            WM_NCDESTROY = 0x0082,
            WM_NCCALCSIZE = 0x0083,
            WM_NCHITTEST = 0x0084,
            WM_NCPAINT = 0x0085,
            WM_NCACTIVATE = 0x0086,
            WM_GETDLGCODE = 0x0087,
            WM_SYNCPAINT = 0x0088,
            WM_NCMOUSEMOVE = 0x00A0,
            WM_NCLBUTTONDOWN = 0x00A1,
            WM_NCLBUTTONUP = 0x00A2,
            WM_NCLBUTTONDBLCLK = 0x00A3,
            WM_NCRBUTTONDOWN = 0x00A4,
            WM_NCRBUTTONUP = 0x00A5,
            WM_NCRBUTTONDBLCLK = 0x00A6,
            WM_NCMBUTTONDOWN = 0x00A7,
            WM_NCMBUTTONUP = 0x00A8,
            WM_NCMBUTTONDBLCLK = 0x00A9,
            WM_NCXBUTTONDOWN = 0x00AB,
            WM_NCXBUTTONUP = 0x00AC,
            WM_NCXBUTTONDBLCLK = 0x00AD,
            WM_INPUT_DEVICE_CHANGE = 0x00FE,
            WM_INPUT = 0x00FF,
            WM_KEYFIRST = 0x0100,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_CHAR = 0x0102,
            WM_DEADCHAR = 0x0103,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_SYSCHAR = 0x0106,
            WM_SYSDEADCHAR = 0x0107,
            WM_UNICHAR = 0x0109,
            WM_KEYLAST = 0x0108,
            WM_IME_STARTCOMPOSITION = 0x010D,
            WM_IME_ENDCOMPOSITION = 0x010E,
            WM_IME_COMPOSITION = 0x010F,
            WM_IME_KEYLAST = 0x010F,
            WM_INITDIALOG = 0x0110,
            WM_COMMAND = 0x0111,
            WM_SYSCOMMAND = 0x0112,
            WM_TIMER = 0x0113,
            WM_HSCROLL = 0x0114,
            WM_VSCROLL = 0x0115,
            WM_INITMENU = 0x0116,
            WM_INITMENUPOPUP = 0x0117,
            WM_MENUSELECT = 0x011F,
            WM_MENUCHAR = 0x0120,
            WM_ENTERIDLE = 0x0121,
            WM_MENURBUTTONUP = 0x0122,
            WM_MENUDRAG = 0x0123,
            WM_MENUGETOBJECT = 0x0124,
            WM_UNINITMENUPOPUP = 0x0125,
            WM_MENUCOMMAND = 0x0126,
            WM_CHANGEUISTATE = 0x0127,
            WM_UPDATEUISTATE = 0x0128,
            WM_QUERYUISTATE = 0x0129,
            WM_CTLCOLORMSGBOX = 0x0132,
            WM_CTLCOLOREDIT = 0x0133,
            WM_CTLCOLORLISTBOX = 0x0134,
            WM_CTLCOLORBTN = 0x0135,
            WM_CTLCOLORDLG = 0x0136,
            WM_CTLCOLORSCROLLBAR = 0x0137,
            WM_CTLCOLORSTATIC = 0x0138,
            WM_MOUSEFIRST = 0x0200,
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_MBUTTONDBLCLK = 0x0209,
            WM_MOUSEWHEEL = 0x020A,
            WM_XBUTTONDOWN = 0x020B,
            WM_XBUTTONUP = 0x020C,
            WM_XBUTTONDBLCLK = 0x020D,
            WM_MOUSEHWHEEL = 0x020E,
            WM_MOUSELAST = 0x020E,
            WM_PARENTNOTIFY = 0x0210,
            WM_ENTERMENULOOP = 0x0211,
            WM_EXITMENULOOP = 0x0212,
            WM_NEXTMENU = 0x0213,
            WM_SIZING = 0x0214,
            WM_CAPTURECHANGED = 0x0215,
            WM_MOVING = 0x0216,
            WM_POWERBROADCAST = 0x0218,
            WM_DEVICECHANGE = 0x0219,
            WM_MDICREATE = 0x0220,
            WM_MDIDESTROY = 0x0221,
            WM_MDIACTIVATE = 0x0222,
            WM_MDIRESTORE = 0x0223,
            WM_MDINEXT = 0x0224,
            WM_MDIMAXIMIZE = 0x0225,
            WM_MDITILE = 0x0226,
            WM_MDICASCADE = 0x0227,
            WM_MDIICONARRANGE = 0x0228,
            WM_MDIGETACTIVE = 0x0229,
            WM_MDISETMENU = 0x0230,
            WM_ENTERSIZEMOVE = 0x0231,
            WM_EXITSIZEMOVE = 0x0232,
            WM_DROPFILES = 0x0233,
            WM_MDIREFRESHMENU = 0x0234,
            WM_IME_SETCONTEXT = 0x0281,
            WM_IME_NOTIFY = 0x0282,
            WM_IME_CONTROL = 0x0283,
            WM_IME_COMPOSITIONFULL = 0x0284,
            WM_IME_SELECT = 0x0285,
            WM_IME_CHAR = 0x0286,
            WM_IME_REQUEST = 0x0288,
            WM_IME_KEYDOWN = 0x0290,
            WM_IME_KEYUP = 0x0291,
            WM_MOUSEHOVER = 0x02A1,
            WM_MOUSELEAVE = 0x02A3,
            WM_NCMOUSEHOVER = 0x02A0,
            WM_NCMOUSELEAVE = 0x02A2,
            WM_WTSSESSION_CHANGE = 0x02B1,
            WM_TABLET_FIRST = 0x02c0,
            WM_TABLET_LAST = 0x02df,
            WM_CUT = 0x0300,
            WM_COPY = 0x0301,
            WM_PASTE = 0x0302,
            WM_CLEAR = 0x0303,
            WM_UNDO = 0x0304,
            WM_RENDERFORMAT = 0x0305,
            WM_RENDERALLFORMATS = 0x0306,
            WM_DESTROYCLIPBOARD = 0x0307,
            WM_DRAWCLIPBOARD = 0x0308,
            WM_PAINTCLIPBOARD = 0x0309,
            WM_VSCROLLCLIPBOARD = 0x030A,
            WM_SIZECLIPBOARD = 0x030B,
            WM_ASKCBFORMATNAME = 0x030C,
            WM_CHANGECBCHAIN = 0x030D,
            WM_HSCROLLCLIPBOARD = 0x030E,
            WM_QUERYNEWPALETTE = 0x030F,
            WM_PALETTEISCHANGING = 0x0310,
            WM_PALETTECHANGED = 0x0311,
            WM_HOTKEY = 0x0312,
            WM_PRINT = 0x0317,
            WM_PRINTCLIENT = 0x0318,
            WM_APPCOMMAND = 0x0319,
            WM_THEMECHANGED = 0x031A,
            WM_CLIPBOARDUPDATE = 0x031D,
            WM_DWMCOMPOSITIONCHANGED = 0x031E,
            WM_DWMNCRENDERINGCHANGED = 0x031F,
            WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320,
            WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321,
            WM_GETTITLEBARINFOEX = 0x033F,
            WM_HANDHELDFIRST = 0x0358,
            WM_HANDHELDLAST = 0x035F,
            WM_AFXFIRST = 0x0360,
            WM_AFXLAST = 0x037F,
            WM_PENWINFIRST = 0x0380,
            WM_PENWINLAST = 0x038F,
            WM_APP = 0x8000,
            WM_USER = 0x0400,
            WM_CPL_LAUNCH = WM_USER + 0x1000,
            WM_CPL_LAUNCHED = WM_USER + 0x1001,
            WM_SYSTIMER = 0x118,
            WM_HSHELL_ACCESSIBILITYSTATE = 11,
            WM_HSHELL_ACTIVATESHELLWINDOW = 3,
            WM_HSHELL_APPCOMMAND = 12,
            WM_HSHELL_GETMINRECT = 5,
            WM_HSHELL_LANGUAGE = 8,
            WM_HSHELL_REDRAW = 6,
            WM_HSHELL_TASKMAN = 7,
            WM_HSHELL_WINDOWCREATED = 1,
            WM_HSHELL_WINDOWDESTROYED = 2,
            WM_HSHELL_WINDOWACTIVATED = 4,
            WM_HSHELL_WINDOWREPLACED = 13
        }

        [StructLayout(LayoutKind.Sequential)]
        struct GUID
        {
            internal ulong a;
            internal ushort b;
            internal ushort c;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            internal byte[] d;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NOTIFYICONDATA
        {
            internal int cbSize; // DWORD
            internal IntPtr hWnd; // HWND
            internal int uID; // UINT
            internal NotifyFlags uFlags; // UINT
            internal WM uCallbackMessage; // UINT
            internal IntPtr hIcon; // HICON
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            internal string szTip; // char[128]
            internal int dwState; // DWORD
            internal int dwStateMask; // DWORD
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            internal string szInfo; // char[256]
            internal uint uTimeoutOrVersion; // UINT
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            internal string szInfoTitle; // char[64]
            internal int dwInfoFlags; // DWORD
            GUID guidItem;
            internal IntPtr hBalloonIcon;
        }

        [Flags]
        internal enum TpmFlags : uint
        {
            TPM_LEFTALIGN = 0x0000,
            TPM_CENTERALIGN = 0x0004,
            TPM_RIGHTALIGN = 0x0008,
            TPM_TOPALIGN = 0x0000,
            TPM_VCENTERALIGN = 0x0010,
            TPM_BOTTOMALIGN = 0x0020,
            TPM_HORIZONTAL = 0x0000,
            TPM_VERTICAL = 0x0040,
            TPM_RETURNCMD = 0x0100
        }

        [Flags]
        public enum MenuFlags : uint
        {
            MF_BITMAP = 0x00000004,
            MF_CHECKED = 0x00000008,
            MF_DISABLED = 0x00000002,
            MF_INSERT = 0x00000000,
            MF_CHANGE = 0x00000080,
            MF_APPEND = 0x00000100,
            MF_DELETE = 0x00000200,
            MF_REMOVE = 0x00001000,
            MF_BYCOMMAND = 0x00000000,
            MF_BYPOSITION = 0x00000400,
            MF_ENABLED = 0x00000000,
            MF_GRAYED = 0x00000001,
            MF_USECHECKBITMAPS = 0x00000200,
            MF_MENUBARBREAK = 0x00000020,
            MF_MENUBREAK = 0x00000040,
            MF_OWNERDRAW = 0x00000100,
            MF_POPUP = 0x00000010,
            MF_SEPARATOR = 0x00000800,
            MF_STRING = 0x00000000,
            MF_UNCHECKED = 0x00000000,
            MF_UNHILITE = 0x00000000,
            MF_HILITE = 0x00000080,
            MF_DEFAULT = 0x00001000,
            MF_SYSMENU = 0x00002000,
            MF_HELP = 0x00004000,
            MF_RIGHTJUSTIFY = 0x00004000,
            MF_MOUSESELECT = 0x00008000,
            MF_END = 0x00000080  /* Obsolete -- only used by old RES files */
        }

        internal delegate IntPtr WNDPROC(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct WNDCLASSEX
        {
            [MarshalAs(UnmanagedType.U4)]
            internal int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            internal int style;
            internal WNDPROC lpfnWndProc; // not WndProc
            internal int cbClsExtra;
            internal int cbWndExtra;
            internal IntPtr hInstance;
            internal IntPtr hIcon;
            internal IntPtr hCursor;
            internal IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPStr)]
            internal string lpszMenuName;
            [MarshalAs(UnmanagedType.LPStr)]
            internal string lpszClassName;
            internal IntPtr hIconSm;
        }

        [Flags]
        internal enum MIIM
        {
            BITMAP = 0x00000080,
            CHECKMARKS = 0x00000008,
            DATA = 0x00000020,
            FTYPE = 0x00000100,
            ID = 0x00000002,
            STATE = 0x00000001,
            STRING = 0x00000040,
            SUBMENU = 0x00000004,
            TYPE = 0x00000010
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class MENUITEMINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MENUITEMINFO));
            public MIIM fMask;
            public uint fType;
            public uint fState;
            public uint wID;
            public IntPtr hSubMenu;
            public IntPtr hbmpChecked;
            public IntPtr hbmpUnchecked;
            public IntPtr dwItemData;
            public string dwTypeData = null;
            public uint cch; // length of dwTypeData
            public IntPtr hbmpItem;
        }

        [Flags]
        internal enum WS : long
        {
            WS_BORDER = 0x00800000L,
            WS_CAPTION = 0x00C00000L,
            WS_CHILD = 0x40000000L,
            WS_CHILDWINDOW = 0x40000000L,
            WS_CLIPCHILDREN = 0x02000000L,
            WS_CLIPSIBLINGS = 0x04000000L,
            WS_DISABLED = 0x08000000L,
            WS_DLGFRAME = 0x00400000L,
            WS_GROUP = 0x00020000L,
            WS_HSCROLL = 0x00100000L,
            WS_ICONIC = 0x20000000L,
            WS_MAXIMIZE = 0x01000000L,
            WS_MAXIMIZEBOX = 0x00010000L,
            WS_MINIMIZE = 0x20000000L,
            WS_MINIMIZEBOX = 0x00020000L,
            WS_OVERLAPPED = 0x00000000L,
            WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
            WS_POPUP = 0x80000000L,
            WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU),
            WS_SIZEBOX = 0x00040000L,
            WS_SYSMENU = 0x00080000L,
            WS_TABSTOP = 0x00010000L,
            WS_THICKFRAME = 0x00040000L,
            WS_TILED = 0x00000000L,
            WS_TILEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
            WS_VISIBLE = 0x10000000L,
            WS_VSCROLL = 0x00200000L
        }

        [Flags]
        internal enum WS_EX : long
        {
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_APPWINDOW = 0x00040000L,
            WS_EX_CLIENTEDGE = 0x00000200L,
            WS_EX_COMPOSITED = 0x02000000L,
            WS_EX_CONTEXTHELP = 0x00000400L,
            WS_EX_CONTROLPARENT = 0x00010000L,
            WS_EX_DLGMODALFRAME = 0x00000001L,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_LAYOUTRTL = 0x00400000L,
            WS_EX_LEFT = 0x00000000L,
            WS_EX_LEFTSCROLLBAR = 0x00004000L,
            WS_EX_LTRREADING = 0x00000000L,
            WS_EX_MDICHILD = 0x00000040L,
            WS_EX_NOACTIVATE = 0x08000000L,
            WS_EX_NOINHERITLAYOUT = 0x00100000L,
            WS_EX_NOPARENTNOTIFY = 0x00000004L,
            WS_EX_NOREDIRECTIONBITMAP = 0x00200000L,
            WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
            WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
            WS_EX_RIGHT = 0x00001000L,
            WS_EX_RIGHTSCROLLBAR = 0x00000000L,
            WS_EX_RTLREADING = 0x00002000L,
            WS_EX_STATICEDGE = 0x00020000L,
            WS_EX_TOOLWINDOW = 0x00000080L,
            WS_EX_TOPMOST = 0x00000008L,
            WS_EX_TRANSPARENT = 0x00000020L,
            WS_EX_WINDOWEDGE = 0x00000100L
        }

        [Flags]
        internal enum CmdShow : int
        {
            SW_HIDE = 0,
            SW_NORMAL = 1,
            SW_SHOWNORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_MAXIMIZE = 3,
            SW_SHOWMAXIMIZED = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11
        }


        #endregion

        #endregion

        #region functions

        #region monitors

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

        #endregion

        #region windows

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType, int cxDesired, int cyDesired, uint fuLoad);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, nuint wParam, nint lParam);

        [DllImport("shell32.dll")]
        internal static extern bool Shell_NotifyIcon(NotifyIconMessage dwMessage, [In] ref NOTIFYICONDATA pnid);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U2)]
        internal static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool UnregisterClass(IntPtr lpClassName, IntPtr hInstance);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr DefWindowProcW(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr CreateWindowEx(uint dwExStyle, IntPtr lpClassName, IntPtr lpWindowName, uint dwStyle,
           int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll")]
        internal static extern IntPtr CreatePopupMenu();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint TrackPopupMenuEx(IntPtr hMenu, TpmFlags uFlags, int x, int y, IntPtr hWnd, IntPtr tpmParams);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool AppendMenuA(IntPtr hMenu, MenuFlags uFlags, UIntPtr uIDNewItem, byte[] lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool InsertMenuItemA(IntPtr hMenu, uint item, bool fByPosition, MENUITEMINFO lpmi);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool DestroyMenu(IntPtr hMenu);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out Point point);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, WNDPROC dwNewLong);
        [DllImport("user32.dll")]
        internal static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowLongPtrA(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hwnd, WM msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        internal static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, uint wFlags);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        internal static extern IntPtr ShowWindow(IntPtr hWnd, CmdShow nCmdShow);


        #endregion

        #endregion

        #region wrappers
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

        #endregion

        public static byte[] StringToByteArray(string str, Encoding encoding)
        {
            int len = encoding.GetByteCount(str);

            // Here we leave a "space" for the ending \0
            // Note the trick to discover the length of the \0 in the encoding:
            // It could be 1 (for Ansi, Utf8, ...), 2 (for Unicode, UnicodeBE), 4 (for UTF32)
            // We simply ask the encoder how long it would be to encode it :-)
            byte[] bytes = new byte[len + encoding.GetByteCount("\0")];
            encoding.GetBytes(str, 0, str.Length, bytes, 0);
            return bytes;
        }

    }
}
