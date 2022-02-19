using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DisplayMagicianShared
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
    public struct RECT
    {
        public Int32 left;
        public Int32 top;
        public Int32 right;
        public Int32 bottom;
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 8)]
    public struct APPBARDATA
    {
        public int cbSize;
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public ABEDGE uEdge;
        public RECT rc;
        public int lParam;
    }



    /// <summary>
    /// The MONITORINFOEX structure contains information about a display monitor.
    /// The GetMonitorInfo function stores information into a MONITORINFOEX structure or a MONITORINFO structure.
    /// The MONITORINFOEX structure is a superset of the MONITORINFO structure. The MONITORINFOEX structure adds a string member to contain a name
    /// for the display monitor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
    public struct MONITORINFOEX
    {
        /// <summary>
        /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function.
        /// Doing so lets the function determine the type of structure you are passing to it.
        /// </summary>
        public UInt32 cbSize;

        /// <summary>
        /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RECT rcMonitor;

        /// <summary>
        /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications,
        /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor.
        /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RECT rcWork;

        /// <summary>
        /// The attributes of the display monitor.
        ///
        /// This member can be the following value:
        ///   1 : MONITORINFOF_PRIMARY
        /// </summary>
        public UInt32 dwFlags;

        /// <summary>
        /// A string that specifies the device name of the monitor being used. Most applications have no use for a display monitor name,
        /// and so can save some bytes by using a MONITORINFO structure.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Utils.CCHDEVICENAME)]
        public string szDevice;

        /*public void Init()
        {
            this.cbSize = 40 + 2 * Utils.CCHDEVICENAME;
            this.DeviceName = string.Empty;
        }*/
    }


    /// <summary>
    /// The MONITORINFO structure contains information about a display monitor.
    /// The GetMonitorInfo function stores information into a MONITORINFOEX structure or a MONITORINFO structure.
    /// The MONITORINFOEX structure is a superset of the MONITORINFO structure. The MONITORINFOEX structure adds a string member to contain a name
    /// for the display monitor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
    public struct MONITORINFO
    {
        /// <summary>
        /// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function.
        /// Doing so lets the function determine the type of structure you are passing to it.
        /// </summary>
        public UInt32 cbSize;

        /// <summary>
        /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RECT rcMonitor;

        /// <summary>
        /// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications,
        /// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor.
        /// The rest of the area in rcMonitor contains system windows such as the task bar and side bars.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RECT rcWork;

        /// <summary>
        /// The attributes of the display monitor.
        ///
        /// This member can be the following value:
        ///   1 : MONITORINFOF_PRIMARY
        /// </summary>
        public UInt32 dwFlags;

        /*public void Init()
        {
            this.cbSize = 40 + 2 * Utils.CCHDEVICENAME;
            this.DeviceName = string.Empty;
        }*/
    }


    [Flags]
    public enum SendMessageTimeoutFlag : uint
    {
        SMTO_NORMAL = 0x0,
        SMTO_BLOCK = 0x1,
        SMTO_ABORTIFHUNG = 0x2,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
        SMTO_ERRORONEXIT = 0x20
    }

    public enum ABEDGE : UInt32
    {
        ABE_LEFT = 0x0,
        ABE_TOP = 0x1,
        ABE_RIGHT = 0x2,
        ABE_BOTTOM = 0x3,
    }


    class Utils
    {
        

        #region enum HChangeNotifyEventID
        /// <summary>
        /// Describes the event that has occurred.
        /// Typically, only one event is specified at a time.
        /// If more than one event is specified, the values contained
        /// in the <i>dwItem1</i> and <i>dwItem2</i>
        /// parameters must be the same, respectively, for all specified events.
        /// This parameter can be one or more of the following values.
        /// </summary>
        /// <remarks>
        /// <para><b>Windows NT/2000/XP:</b> <i>dwItem2</i> contains the index
        /// in the system image list that has changed.
        /// <i>dwItem1</i> is not used and should be <see langword="null"/>.</para>
        /// <para><b>Windows 95/98:</b> <i>dwItem1</i> contains the index
        /// in the system image list that has changed.
        /// <i>dwItem2</i> is not used and should be <see langword="null"/>.</para>
        /// </remarks>
        [Flags]
        public enum HChangeNotifyEventID
        {
            /// <summary>
            /// All events have occurred.
            /// </summary>
            SHCNE_ALLEVENTS = 0x7FFFFFFF,

            /// <summary>
            /// A file type association has changed. <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/>
            /// must be specified in the <i>uFlags</i> parameter.
            /// <i>dwItem1</i> and <i>dwItem2</i> are not used and must be <see langword="null"/>.
            /// </summary>
            SHCNE_ASSOCCHANGED = 0x08000000,

            /// <summary>
            /// The attributes of an item or folder have changed.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the item or folder that has changed.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_ATTRIBUTES = 0x00000800,

            /// <summary>
            /// A nonfolder item has been created.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the item that was created.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_CREATE = 0x00000002,

            /// <summary>
            /// A nonfolder item has been deleted.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the item that was deleted.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_DELETE = 0x00000004,

            /// <summary>
            /// A drive has been added.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the root of the drive that was added.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_DRIVEADD = 0x00000100,

            /// <summary>
            /// A drive has been added and the Shell should create a new window for the drive.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the root of the drive that was added.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_DRIVEADDGUI = 0x00010000,

            /// <summary>
            /// A drive has been removed. <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the root of the drive that was removed.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_DRIVEREMOVED = 0x00000080,

            /// <summary>
            /// Not currently used.
            /// </summary>
            SHCNE_EXTENDED_EVENT = 0x04000000,

            /// <summary>
            /// The amount of free space on a drive has changed.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the root of the drive on which the free space changed.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_FREESPACE = 0x00040000,

            /// <summary>
            /// Storage media has been inserted into a drive.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the root of the drive that contains the new media.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_MEDIAINSERTED = 0x00000020,

            /// <summary>
            /// Storage media has been removed from a drive.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the root of the drive from which the media was removed.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_MEDIAREMOVED = 0x00000040,

            /// <summary>
            /// A folder has been created. <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/>
            /// or <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the folder that was created.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_MKDIR = 0x00000008,

            /// <summary>
            /// A folder on the local computer is being shared via the network.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the folder that is being shared.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_NETSHARE = 0x00000200,

            /// <summary>
            /// A folder on the local computer is no longer being shared via the network.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the folder that is no longer being shared.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_NETUNSHARE = 0x00000400,

            /// <summary>
            /// The name of a folder has changed.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the previous pointer to an item identifier list (PIDL) or name of the folder.
            /// <i>dwItem2</i> contains the new PIDL or name of the folder.
            /// </summary>
            SHCNE_RENAMEFOLDER = 0x00020000,

            /// <summary>
            /// The name of a nonfolder item has changed.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the previous PIDL or name of the item.
            /// <i>dwItem2</i> contains the new PIDL or name of the item.
            /// </summary>
            SHCNE_RENAMEITEM = 0x00000001,

            /// <summary>
            /// A folder has been removed.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the folder that was removed.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_RMDIR = 0x00000010,

            /// <summary>
            /// The computer has disconnected from a server.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the server from which the computer was disconnected.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// </summary>
            SHCNE_SERVERDISCONNECT = 0x00004000,

            /// <summary>
            /// The contents of an existing folder have changed,
            /// but the folder still exists and has not been renamed.
            /// <see cref="HChangeNotifyFlags.SHCNF_IDLIST"/> or
            /// <see cref="HChangeNotifyFlags.SHCNF_PATH"/> must be specified in <i>uFlags</i>.
            /// <i>dwItem1</i> contains the folder that has changed.
            /// <i>dwItem2</i> is not used and should be <see langword="null"/>.
            /// If a folder has been created, deleted, or renamed, use SHCNE_MKDIR, SHCNE_RMDIR, or
            /// SHCNE_RENAMEFOLDER, respectively, instead.
            /// </summary>
            SHCNE_UPDATEDIR = 0x00001000,

            /// <summary>
            /// An image in the system image list has changed.
            /// <see cref="HChangeNotifyFlags.SHCNF_DWORD"/> must be specified in <i>uFlags</i>.
            /// </summary>
            SHCNE_UPDATEIMAGE = 0x00008000,

        }
        #endregion // enum HChangeNotifyEventID

        #region public enum HChangeNotifyFlags
        /// <summary>
        /// Flags that indicate the meaning of the <i>dwItem1</i> and <i>dwItem2</i> parameters.
        /// The uFlags parameter must be one of the following values.
        /// </summary>
        [Flags]
        public enum HChangeNotifyFlags
        {
            /// <summary>
            /// The <i>dwItem1</i> and <i>dwItem2</i> parameters are DWORD values.
            /// </summary>
            SHCNF_DWORD = 0x0003,
            /// <summary>
            /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of ITEMIDLIST structures that
            /// represent the item(s) affected by the change.
            /// Each ITEMIDLIST must be relative to the desktop folder.
            /// </summary>
            SHCNF_IDLIST = 0x0000,
            /// <summary>
            /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of null-terminated strings of
            /// maximum length MAX_PATH that contain the full path names
            /// of the items affected by the change.
            /// </summary>
            SHCNF_PATHA = 0x0001,
            /// <summary>
            /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of null-terminated strings of
            /// maximum length MAX_PATH that contain the full path names
            /// of the items affected by the change.
            /// </summary>
            SHCNF_PATHW = 0x0005,
            /// <summary>
            /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of null-terminated strings that
            /// represent the friendly names of the printer(s) affected by the change.
            /// </summary>
            SHCNF_PRINTERA = 0x0002,
            /// <summary>
            /// <i>dwItem1</i> and <i>dwItem2</i> are the addresses of null-terminated strings that
            /// represent the friendly names of the printer(s) affected by the change.
            /// </summary>
            SHCNF_PRINTERW = 0x0006,
            /// <summary>
            /// The function should not return until the notification
            /// has been delivered to all affected components.
            /// As this flag modifies other data-type flags, it cannot by used by itself.
            /// </summary>
            SHCNF_FLUSH = 0x1000,
            /// <summary>
            /// The function should begin delivering notifications to all affected components
            /// but should return as soon as the notification process has begun.
            /// As this flag modifies other data-type flags, it cannot by used by itself.
            /// </summary>
            SHCNF_FLUSHNOWAIT = 0x2000
        }
        #endregion // enum HChangeNotifyFlags

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, String lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(IntPtr windowHandle, uint message, IntPtr wordParameter, IntPtr longParameter, SendMessageTimeoutFlag flag, uint timeout, out IntPtr resultHandle);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, string lParam);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, uint wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern void SHChangeNotify(HChangeNotifyEventID wEventId,
                                   HChangeNotifyFlags uFlags,
                                   IntPtr dwItem1,
                                   IntPtr dwItem2);

        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public static bool IsWindows11()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            var currentBuildStr = (string)reg.GetValue("CurrentBuild");
            var currentBuild = int.Parse(currentBuildStr);

            return currentBuild >= 22000;
        }

        public static int MakeLParam(int p, int p_2)
        {
            return ((p_2 << 16) | (p & 0xFFFF));
        }

        internal delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        private void EnumMonitors()
        {
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumCallBack, IntPtr.Zero);
        }

        private bool MonitorEnumCallBack(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
        {
            MONITORINFOEX mon_info = new MONITORINFOEX();
            mon_info.cbSize = (UInt32)Marshal.SizeOf(mon_info);
            //mon_info.szDevice = new char[Utils.CCHDEVICENAME];
            GetMonitorInfo(hMonitor, ref mon_info);
            ///Monitor info is stored in 'mon_info'
            return true;
        }

        /*public static bool RefreshNotificationTray()
        {
            Utils.SHChangeNotify(Utils.HChangeNotifyEventID.SHCNE_ASSOCCHANGED, Utils.HChangeNotifyFlags.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);

            IntPtr result;
            IntPtr lparam = Marshal.StringToHGlobalAuto("Environment");
            Utils.SendMessage((IntPtr)Utils.HWND_BROADCAST, Utils.WM_SETTINGCHANGE, (IntPtr)0, lparam);
            Utils.SendMessage((IntPtr)Utils.HWND_BROADCAST, Utils.WM_SETTINGCHANGE, (IntPtr)0, (IntPtr)0);
            return true;
        }*/

        public const int NULL = 0;
        public const int HWND_BROADCAST = 0xffff;
        public const int WM_NOTIFY = 0xA005; 
        public const int WM_SETTINGCHANGE = 0x001a;
        public const int WM_THEMECHANGED = 0x031a;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int SPI_SETWORKAREA = 0x002F;
        public const int SHELLHOOK = 0xC028;
        public const int WM_USER_REFRESHTASKBAR = 0x05CA;
        public const int WM_USER_451 = 0x05C3; 
        public const int WM_USER_440 = 0x05B8;
        public const int WM_USER_336 = 0x0550;
        public const int WM_USER_92 = 0x045C;
        public const int WM_USER_7 = 0x0407;
        public const int WM_USER_1 = 0x0401;
        public const int WM_USER_100 = 0x0464;
        public const int WM_USER_13 = 0x040D;
        public const int wParam_SHELLTRAY = 0x00000006;

        public const int ABM_NEW = 0x00000000;
        public const int ABM_REMOVE = 0x00000001;
        public const int ABM_QUERYPOS = 0x00000002;
        public const int ABM_SETPOS = 0x00000003;
        public const int ABM_GETSTATE = 0x00000004;
        public const int ABM_GETTASKBARPOS = 0x00000005;
        public const int ABM_ACTIVATE = 0x00000006; // lParam == TRUE/FALSE means activate/deactivate
        public const int ABM_GETAUTOHIDEBAR = 0x00000007;
        public const int ABM_SETAUTOHIDEBAR = 0x00000008; // this can fail at any time.  MUST check the result
                                                          // lParam = TRUE/FALSE  Set/Unset
                                                          // uEdge = what edge
        public const int ABM_WINDOWPOSCHANGED = 0x0000009;
        public const int ABM_SETSTATE = 0x0000000a;

        // these are put in the wparam of callback messages
        public const int ABN_STATECHANGE = 0x0000000;
        public const int ABN_POSCHANGED = 0x0000001;
        public const int ABN_FULLSCREENAPP = 0x0000002;
        public const int ABN_WINDOWARRANGE = 0x0000003; // lParam == TRUE means hide

        // flags for get state
        public const int ABS_AUTOHIDE = 0x0000001;
        public const int ABS_ALWAYSONTOP = 0x0000002;

        public const int ABE_LEFT = 0;
        public const int ABE_TOP = 1;
        public const int ABE_RIGHT = 2;
        public const int ABE_BOTTOM = 3;

        public const int MONITOR_DEFAULTTONULL = 0;
        public const int MONITOR_DEFAULTTOPRIMARY = 1;
        public const int MONITOR_DEFAULTTONEAREST = 2;

        // size of a device name string
        public const int CCHDEVICENAME = 32;
        public const uint MONITORINFOF_PRIMARY = 1;
    }


}
