using DisplayMagicianShared.Windows;
using System;
using System.Runtime.InteropServices;
using static DisplayMagicianShared.Intel.IGCLImport;
//using FARPROC = System.IntPtr;
using HMODULE = System.IntPtr;
using ctl_display_output_handle_t = System.IntPtr;
using ctl_device_adapter_handle_t = System.IntPtr;
using ctl_api_handle_t = System.IntPtr;


namespace DisplayMagicianShared.Intel
{
    public delegate IntPtr ADL_Main_Memory_Alloc_Delegate(int size);

    public enum ctl_result_t : Int32
    {
        // Result Codes
        CTL_RESULT_SUCCESS = 0x00000000,                //< success
        CTL_RESULT_SUCCESS_STILL_OPEN_BY_ANOTHER_CALLER = 0x00000001,   //< success but still open by another caller
        CTL_RESULT_ERROR_SUCCESS_END = 0x0000FFFF,      //< "Success group error code end value, not to be used
                                                        //< "
        CTL_RESULT_ERROR_GENERIC_START = 0x40000000,    //< Generic error code starting value, not to be used
        CTL_RESULT_ERROR_NOT_INITIALIZED = 0x40000001,  //< Result not initialized
        CTL_RESULT_ERROR_ALREADY_INITIALIZED = 0x40000002,  //< Already initialized
        CTL_RESULT_ERROR_DEVICE_LOST = 0x40000003,      //< Device hung, reset, was removed, or driver update occurred
        CTL_RESULT_ERROR_OUT_OF_HOST_MEMORY = 0x40000004,   //< Insufficient host memory to satisfy call
        CTL_RESULT_ERROR_OUT_OF_DEVICE_MEMORY = 0x40000005, //< Insufficient device memory to satisfy call
        CTL_RESULT_ERROR_INSUFFICIENT_PERMISSIONS = 0x40000006, //< Access denied due to permission level
        CTL_RESULT_ERROR_NOT_AVAILABLE = 0x40000007,    //< Resource was removed
        CTL_RESULT_ERROR_UNINITIALIZED = 0x40000008,    //< Library not initialized
        CTL_RESULT_ERROR_UNSUPPORTED_VERSION = 0x40000009,  //< Generic error code for unsupported versions
        CTL_RESULT_ERROR_UNSUPPORTED_FEATURE = 0x4000000a,  //< Generic error code for unsupported features
        CTL_RESULT_ERROR_INVALID_ARGUMENT = 0x4000000b, //< Generic error code for invalid arguments
        CTL_RESULT_ERROR_INVALID_API_HANDLE = 0x4000000c,   //< API handle in invalid
        CTL_RESULT_ERROR_INVALID_NULL_HANDLE = 0x4000000d,  //< Handle argument is not valid
        CTL_RESULT_ERROR_INVALID_NULL_POINTER = 0x4000000e, //< Pointer argument may not be nullptr
        CTL_RESULT_ERROR_INVALID_SIZE = 0x4000000f,     //< Size argument is invalid (e.g., must not be zero)
        CTL_RESULT_ERROR_UNSUPPORTED_SIZE = 0x40000010, //< Size argument is not supported by the device (e.g., too large)
        CTL_RESULT_ERROR_UNSUPPORTED_IMAGE_FORMAT = 0x40000011, //< Image format is not supported by the device
        CTL_RESULT_ERROR_DATA_READ = 0x40000012,        //< Data read error
        CTL_RESULT_ERROR_DATA_WRITE = 0x40000013,       //< Data write error
        CTL_RESULT_ERROR_DATA_NOT_FOUND = 0x40000014,   //< Data not found error
        CTL_RESULT_ERROR_NOT_IMPLEMENTED = 0x40000015,  //< Function not implemented
        CTL_RESULT_ERROR_OS_CALL = 0x40000016,          //< Operating system call failure
        CTL_RESULT_ERROR_KMD_CALL = 0x40000017,         //< Kernel mode driver call failure
        CTL_RESULT_ERROR_UNLOAD = 0x40000018,           //< Library unload failure
        CTL_RESULT_ERROR_ZE_LOADER = 0x40000019,        //< Level0 loader not found
        CTL_RESULT_ERROR_INVALID_OPERATION_TYPE = 0x4000001a,   //< Invalid operation type
        CTL_RESULT_ERROR_NULL_OS_INTERFACE = 0x4000001b,//< Null OS interface
        CTL_RESULT_ERROR_NULL_OS_ADAPATER_HANDLE = 0x4000001c,  //< Null OS adapter handle
        CTL_RESULT_ERROR_NULL_OS_DISPLAY_OUTPUT_HANDLE = 0x4000001d,//< Null display output handle
        CTL_RESULT_ERROR_WAIT_TIMEOUT = 0x4000001e,     //< Timeout in Wait function
        CTL_RESULT_ERROR_PERSISTANCE_NOT_SUPPORTED = 0x4000001f,//< Persistance not supported
        CTL_RESULT_ERROR_PLATFORM_NOT_SUPPORTED = 0x40000020,   //< Platform not supported
        CTL_RESULT_ERROR_UNKNOWN_APPLICATION_UID = 0x40000021,  //< Unknown Appplicaion UID in Initialization call 
        CTL_RESULT_ERROR_INVALID_ENUMERATION = 0x40000022,  //< The enum is not valid
        CTL_RESULT_ERROR_FILE_DELETE = 0x40000023,      //< Error in file delete operation
        CTL_RESULT_ERROR_RESET_DEVICE_REQUIRED = 0x40000024,//< The device requires a reset.
        CTL_RESULT_ERROR_FULL_REBOOT_REQUIRED = 0x40000025, //< The device requires a full reboot.
        CTL_RESULT_ERROR_LOAD = 0x40000026,             //< Library load failure
        CTL_RESULT_ERROR_UNKNOWN = 0x4000FFFF,          //< Unknown or internal error
        CTL_RESULT_ERROR_RETRY_OPERATION = 0x40010000,  //< Operation failed, retry previous operation again
        CTL_RESULT_ERROR_GENERIC_END = 0x4000FFFF,      //< "Generic error code end value, not to be used
                                                        //< "
        CTL_RESULT_ERROR_CORE_START = 0x44000000,       //< Core error code starting value, not to be used
        CTL_RESULT_ERROR_CORE_OVERCLOCK_NOT_SUPPORTED = 0x44000001, //< The Overclock is not supported.
        CTL_RESULT_ERROR_CORE_OVERCLOCK_VOLTAGE_OUTSIDE_RANGE = 0x44000002, //< The Voltage exceeds the acceptable min/max.
        CTL_RESULT_ERROR_CORE_OVERCLOCK_FREQUENCY_OUTSIDE_RANGE = 0x44000003,   //< The Frequency exceeds the acceptable min/max.
        CTL_RESULT_ERROR_CORE_OVERCLOCK_POWER_OUTSIDE_RANGE = 0x44000004,   //< The Power exceeds the acceptable min/max.
        CTL_RESULT_ERROR_CORE_OVERCLOCK_TEMPERATURE_OUTSIDE_RANGE = 0x44000005, //< The Power exceeds the acceptable min/max.
        CTL_RESULT_ERROR_CORE_OVERCLOCK_IN_VOLTAGE_LOCKED_MODE = 0x44000006,//< The Overclock is in voltage locked mode.
        CTL_RESULT_ERROR_CORE_OVERCLOCK_RESET_REQUIRED = 0x44000007,//< It indicates that the requested change will not be applied until the
                                                                    //< device is reset.
        CTL_RESULT_ERROR_CORE_OVERCLOCK_WAIVER_NOT_SET = 0x44000008,//< The $OverclockWaiverSet function has not been called.
        CTL_RESULT_ERROR_CORE_END = 0x0440FFFF,         //< "Core error code end value, not to be used
                                                        //< "
        CTL_RESULT_ERROR_3D_START = 0x60000000,         //< 3D error code starting value, not to be used
        CTL_RESULT_ERROR_3D_END = 0x6000FFFF,           //< "3D error code end value, not to be used
                                                        //< "
        CTL_RESULT_ERROR_MEDIA_START = 0x50000000,      //< Media error code starting value, not to be used
        CTL_RESULT_ERROR_MEDIA_END = 0x5000FFFF,        //< "Media error code end value, not to be used
                                                        //< "
        CTL_RESULT_ERROR_DISPLAY_START = 0x48000000,    //< Display error code starting value, not to be used
        CTL_RESULT_ERROR_INVALID_AUX_ACCESS_FLAG = 0x48000001,  //< Invalid flag for Aux access
        CTL_RESULT_ERROR_INVALID_SHARPNESS_FILTER_FLAG = 0x48000002,//< Invalid flag for Sharpness
        CTL_RESULT_ERROR_DISPLAY_NOT_ATTACHED = 0x48000003, //< Error for Display not attached
        CTL_RESULT_ERROR_DISPLAY_NOT_ACTIVE = 0x48000004,   //< Error for display attached but not active
        CTL_RESULT_ERROR_INVALID_POWERFEATURE_OPTIMIZATION_FLAG = 0x48000005,   //< Error for invalid power optimization flag
        CTL_RESULT_ERROR_INVALID_POWERSOURCE_TYPE_FOR_DPST = 0x48000006,//< DPST is supported only in DC Mode
        CTL_RESULT_ERROR_INVALID_PIXTX_GET_CONFIG_QUERY_TYPE = 0x48000007,  //< Invalid query type for pixel transformation get configuration
        CTL_RESULT_ERROR_INVALID_PIXTX_SET_CONFIG_OPERATION_TYPE = 0x48000008,  //< Invalid operation type for pixel transformation set configuration
        CTL_RESULT_ERROR_INVALID_SET_CONFIG_NUMBER_OF_SAMPLES = 0x48000009, //< Invalid number of samples for pixel transformation set configuration
        CTL_RESULT_ERROR_INVALID_PIXTX_BLOCK_ID = 0x4800000a,   //< Invalid block id for pixel transformation
        CTL_RESULT_ERROR_INVALID_PIXTX_BLOCK_TYPE = 0x4800000b, //< Invalid block type for pixel transformation
        CTL_RESULT_ERROR_INVALID_PIXTX_BLOCK_NUMBER = 0x4800000c,   //< Invalid block number for pixel transformation
        CTL_RESULT_ERROR_INSUFFICIENT_PIXTX_BLOCK_CONFIG_MEMORY = 0x4800000d,   //< Insufficient memery allocated for BlockConfigs
        CTL_RESULT_ERROR_3DLUT_INVALID_PIPE = 0x4800000e,   //< Invalid pipe for 3dlut
        CTL_RESULT_ERROR_3DLUT_INVALID_DATA = 0x4800000f,   //< Invalid 3dlut data
        CTL_RESULT_ERROR_3DLUT_NOT_SUPPORTED_IN_HDR = 0x48000010,   //< 3dlut not supported in HDR
        CTL_RESULT_ERROR_3DLUT_INVALID_OPERATION = 0x48000011,  //< Invalid 3dlut operation
        CTL_RESULT_ERROR_3DLUT_UNSUCCESSFUL = 0x48000012,   //< 3dlut call unsuccessful
        CTL_RESULT_ERROR_AUX_DEFER = 0x48000013,        //< AUX defer failure
        CTL_RESULT_ERROR_AUX_TIMEOUT = 0x48000014,      //< AUX timeout failure
        CTL_RESULT_ERROR_AUX_INCOMPLETE_WRITE = 0x48000015, //< AUX incomplete write failure
        CTL_RESULT_ERROR_I2C_AUX_STATUS_UNKNOWN = 0x48000016,   //< I2C/AUX unkonown failure
        CTL_RESULT_ERROR_I2C_AUX_UNSUCCESSFUL = 0x48000017, //< I2C/AUX unsuccessful
        CTL_RESULT_ERROR_LACE_INVALID_DATA_ARGUMENT_PASSED = 0x48000018,//< Lace Incorrrect AggressivePercent data or LuxVsAggressive Map data
                                                                        //< passed by user
        CTL_RESULT_ERROR_EXTERNAL_DISPLAY_ATTACHED = 0x48000019,//< External Display is Attached hence fail the Display Switch
        CTL_RESULT_ERROR_CUSTOM_MODE_STANDARD_CUSTOM_MODE_EXISTS = 0x4800001a,  //< Standard custom mode exists
        CTL_RESULT_ERROR_CUSTOM_MODE_NON_CUSTOM_MATCHING_MODE_EXISTS = 0x4800001b,  //< Non custom matching mode exists
        CTL_RESULT_ERROR_CUSTOM_MODE_INSUFFICIENT_MEMORY = 0x4800001c,  //< Custom mode insufficent memory
        CTL_RESULT_ERROR_ADAPTER_ALREADY_LINKED = 0x4800001d,   //< Adapter is already linked
        CTL_RESULT_ERROR_ADAPTER_NOT_IDENTICAL = 0x4800001e,//< Adapter is not identical for linking
        CTL_RESULT_ERROR_ADAPTER_NOT_SUPPORTED_ON_LDA_SECONDARY = 0x4800001f,   //< Adapter is LDA Secondary, so not supporting requested operation
        CTL_RESULT_ERROR_SET_FBC_FEATURE_NOT_SUPPORTED = 0x48000020,//< Set FBC Feature not supported
        CTL_RESULT_ERROR_DISPLAY_END = 0x4800FFFF,      //< "Display error code end value, not to be used
                                                        //< "
        CTL_RESULT_MAX

    }


    ///////////////////////////////////////////////////////////////////////////////
    /// @brief Combined Display operation type
    public enum ctl_combined_display_optype_t : Int32
    {
        CTL_COMBINED_DISPLAY_OPTYPE_IS_SUPPORTED_CONFIG = 1,///< To check whether given outputs can form a combined display, no changes
                                                            ///< are applied
        CTL_COMBINED_DISPLAY_OPTYPE_ENABLE = 2,         ///< To setup and enable a combined display
        CTL_COMBINED_DISPLAY_OPTYPE_DISABLE = 3,        ///< To disable combined display
        CTL_COMBINED_DISPLAY_OPTYPE_QUERY_CONFIG = 4,   ///< To query combined display configuration
        CTL_COMBINED_DISPLAY_OPTYPE_MAX

    };

    ///////////////////////////////////////////////////////////////////////////////
    /// @brief Display orientation (rotation)
    public enum ctl_display_orientation_t : UInt32
    {
        CTL_DISPLAY_ORIENTATION_0 = 0,                  ///< 0 Degree
        CTL_DISPLAY_ORIENTATION_90 = 1,                 ///< 90 Degree
        CTL_DISPLAY_ORIENTATION_180 = 2,                ///< 180 Degree
        CTL_DISPLAY_ORIENTATION_270 = 3,                ///< 270 Degree
        CTL_DISPLAY_ORIENTATION_MAX

    }



    [Flags]
    public enum ADL_DISPLAY_MODE_FLAG : int
    {
        ColourFormat565 = 1,
        ColourFormat8888 = 2,
        Degrees0 = 4,
        Degrees90 = 8,
        Degrees180 = 10,
        Degrees270 = 20,
        ExactRefreshRate = 80,
        RoundedRefreshRate = 40
    }
    public enum ADL_DISPLAY_MODE_INTERLACING : int
    {
        Progressive = 0,
        Interlaced = 2
    }

    public enum ADL_COLORDEPTH : int
    {
        ColorDepth_Unknown = 0,
        ColorDepth_666 = 1,
        ColorDepth_888 = 2,
        ColorDepth_101010 = 3,
        ColorDepth_121212 = 4,
        ColorDepth_141414 = 5,
        ColorDepth_161616 = 6,
    }




    ///////////////////////////////////////////////////////////////////////////////
    /// @brief Application Unique ID
    [StructLayout(LayoutKind.Sequential)]
    public struct ctl_application_id_t
    {
        UInt32 Data1;                                 ///< [in] Data1
        UInt16 Data2;                                 ///< [in] Data2
        UInt16 Data3;                                 ///< [in] Data3
        byte Data4;                               ///< [in] Data4
    }


    ///////////////////////////////////////////////////////////////////////////////
    /// @brief Init arguments
    [StructLayout(LayoutKind.Sequential)]
    public struct ctl_init_args_t
    {
        UInt32 Size;                                  //< [in] size of this structure
        byte Version;                                //< [in] version of this structure
        UInt32 AppVersion;                  //< [in][release] App's IGCL version
        UInt32 flags;                         //< [in][release] Caller version
        UInt32 SupportedVersion;            //< [out][release] IGCL implementation version
        ctl_application_id_t ApplicationUID;            //< [in] Application Provided Unique ID.Application can pass all 0's as
                                                        //< the default ID
    }


    ///////////////////////////////////////////////////////////////////////////////
    /// @brief Combined Display's child display target mode
    [StructLayout(LayoutKind.Sequential)]
    public struct ctl_child_display_target_mode_t : IEquatable<ctl_child_display_target_mode_t>
    {
        UInt32 Width;                                 ///< [in,out] Width
        UInt32 Height;                                ///< [in,out] Height
        float RefreshRate;                              ///< [in,out] Refresh Rate
        UInt32 ReservedFields;                     ///< [out] Reserved field of 16 bytes

        public override bool Equals(object obj) => obj is ctl_child_display_target_mode_t other && this.Equals(other);
        public bool Equals(ctl_child_display_target_mode_t other)
            => Width == other.Width &&
                Height == other.Height &&
                RefreshRate == other.RefreshRate &&
                ReservedFields == other.ReservedFields;

        public override int GetHashCode()
        {
            return (Width, Height, RefreshRate, ReservedFields).GetHashCode();
        }

        public static bool operator ==(ctl_child_display_target_mode_t lhs, ctl_child_display_target_mode_t rhs) => lhs.Equals(rhs);

        public static bool operator !=(ctl_child_display_target_mode_t lhs, ctl_child_display_target_mode_t rhs) => !(lhs.Equals(rhs));

    }

    
    ///////////////////////////////////////////////////////////////////////////////
    /// @brief Rectangle
    [StructLayout(LayoutKind.Sequential)]
    public struct ctl_rect_t : IEquatable<ctl_rect_t>
    {
        Int32 Left;                                   ///< [in,out] Left
        Int32 Top;                                    ///< [in,out] Top
        Int32 Right;                                  ///< [in,out] Right
        Int32 Bottom;                                 ///< [in,out] Bottom

        public override bool Equals(object obj) => obj is ctl_rect_t other && this.Equals(other);
        public bool Equals(ctl_rect_t other)
            => Left == other.Left &&
                Top == other.Top &&
                Right == other.Right &&
                Bottom == other.Bottom;

        public override int GetHashCode()
        {
            return (Left, Top, Right, Bottom).GetHashCode();
        }

        public static bool operator ==(ctl_rect_t lhs, ctl_rect_t rhs) => lhs.Equals(rhs);

        public static bool operator !=(ctl_rect_t lhs, ctl_rect_t rhs) => !(lhs.Equals(rhs));

    }


    ///////////////////////////////////////////////////////////////////////////////
    /// @brief Combined Display's child display information
    [StructLayout(LayoutKind.Sequential)]
    public struct ctl_combined_display_child_info_t : IEquatable<ctl_combined_display_child_info_t>
    {
        ctl_display_output_handle_t hDisplayOutput;     ///< [in,out] Display output handle under combined display configuration
        ctl_rect_t FbSrc;                               ///< [in,out] FrameBuffer source's RECT within Combined Display respective
        ctl_rect_t FbPos;                               ///< [in,out] FrameBuffer target's RECT within output size
        ctl_display_orientation_t DisplayOrientation;   ///< [in,out] 0/180 Degree Display orientation (rotation)
        ctl_child_display_target_mode_t TargetMode;     ///< [in,out] Desired target mode (width, height, refresh)

        public override bool Equals(object obj) => obj is ctl_combined_display_child_info_t other && this.Equals(other);
        public bool Equals(ctl_combined_display_child_info_t other)
            => hDisplayOutput == other.hDisplayOutput &&
                FbSrc == other.FbSrc &&
                FbPos == other.FbPos &&
                DisplayOrientation == other.DisplayOrientation &&
                TargetMode == other.TargetMode;

        public override int GetHashCode()
        {
            return (hDisplayOutput, FbSrc, FbPos, DisplayOrientation, TargetMode).GetHashCode();
        }

        public static bool operator ==(ctl_combined_display_child_info_t lhs, ctl_combined_display_child_info_t rhs) => lhs.Equals(rhs);

        public static bool operator !=(ctl_combined_display_child_info_t lhs, ctl_combined_display_child_info_t rhs) => !(lhs.Equals(rhs));

    }

    ///////////////////////////////////////////////////////////////////////////////
    /// @brief Combined Display arguments
    [StructLayout(LayoutKind.Sequential)]
    public struct ctl_combined_display_args_t : IEquatable<ctl_combined_display_args_t>
    {
        UInt32 Size;                                  ///< [in] size of this structure
        byte Version;                                ///< [in] version of this structure
        ctl_combined_display_optype_t OpType;           ///< [in] Combined display operation type
        bool IsSupported;                               ///< [out] Returns yes/no in response to IS_SUPPORTED_CONFIG command
        byte NumOutputs;                             ///< [in,out] Number of outputs part of desired combined display
                                                        ///< configuration
        UInt32 CombinedDesktopWidth;                  ///< [in,out] Width of desired combined display configuration
        UInt32 CombinedDesktopHeight;                 ///< [in,out] Height of desired combined display configuration
        IntPtr pChildInfo;  ///< [in,out] List of child display information respective to each output.
                                                        ///< Up to 16 displays are supported with up to 4 displays per GPU.
        IntPtr hCombinedDisplayOutput; ///< [in,out] Handle to combined display output

        public override bool Equals(object obj) => obj is ctl_combined_display_args_t other && this.Equals(other);
        public bool Equals(ctl_combined_display_args_t other)
            => Size == other.Size &&
                Version == other.Version &&
                OpType == other.OpType &&
                IsSupported == other.IsSupported &&
                NumOutputs == other.NumOutputs &&
                CombinedDesktopWidth == other.CombinedDesktopWidth &&
                CombinedDesktopHeight == other.CombinedDesktopHeight &&
                pChildInfo == other.pChildInfo &&
                hCombinedDisplayOutput == other.hCombinedDisplayOutput;

        public override int GetHashCode()
        {
            return (Size, Version, OpType, IsSupported, NumOutputs, CombinedDesktopWidth, CombinedDesktopHeight, pChildInfo, hCombinedDisplayOutput).GetHashCode();
        }

        public static bool operator ==(ctl_combined_display_args_t lhs, ctl_combined_display_args_t rhs) => lhs.Equals(rhs);

        public static bool operator !=(ctl_combined_display_args_t lhs, ctl_combined_display_args_t rhs) => !(lhs.Equals(rhs));


    }



    class IGCLImport
    {

        /// <summary> Selects all adapters instead of aparticular single adapter</summary>
        public const int ADL_ADAPTER_INDEX_ALL = -1;
        ///    Defines APIs with iOption none
        public const int ADL_MAIN_API_OPTION_NONE = 0;
        /// <summary> Define the maximum char</summary>
        public const int ADL_MAX_CHAR = 4096;
        /// <summary> Define the maximum path</summary>
        public const int ADL_MAX_PATH = 256;
        /// <summary> Define the maximum adapters</summary>
        public const int ADL_MAX_ADAPTERS = 250;
        /// <summary> Define the maximum displays</summary>
        public const int ADL_MAX_DISPLAYS = 150;
        /// <summary> Define the maximum device name length</summary>
        public const int ADL_MAX_DEVICENAME = 32;
        /// <summary> Define the maximum EDID Data length</summary>
        public const int ADL_MAX_EDIDDATA_SIZE = 256; // number of UCHAR
        /// <summary> Define the maximum display names</summary>
        public const int ADL_MAX_DISPLAY_NAME = 256;

        // Display Mode Constants
        /// <summary> Indicates the display is in interlaced mode</summary>
        public const int ADL_DISPLAY_MODE_INTERLACED_FLAG = 2;
        /// <summary> Indicates the display is in progressive mode </summary>
        public const int ADL_DISPLAY_MODE_PROGRESSIVE_FLAG = 0;
        /// <summary> Indicates the display colour format is 565</summary>
        public const int ADL_DISPLAY_MODE_COLOURFORMAT_565 = 0x00000001;
        /// <summary> Indicates the display colour format is 8888 </summary>
        public const int ADL_DISPLAY_MODE_COLOURFORMAT_8888 = 0x00000002; // 
        /// <summary> Indicates the display orientation is normal position</summary>
        public const int ADL_DISPLAY_MODE_ORIENTATION_SUPPORTED_000 = 0x00000004;
        /// <summary> Indicates the display is in the 90 degree position</summary>
        public const int ADL_DISPLAY_MODE_ORIENTATION_SUPPORTED_090 = 0x00000008;
        /// <summary> Indicates the display in the 180 degree position</summary>
        public const int ADL_DISPLAY_MODE_ORIENTATION_SUPPORTED_180 = 0x00000010;
        /// <summary> Indicates the display is in the 270 degree position</summary>
        public const int ADL_DISPLAY_MODE_ORIENTATION_SUPPORTED_270 = 0x00000020;
        /// <summary> Indicates the display refresh rate is exact </summary>
        public const int ADL_DISPLAY_MODE_REFRESHRATE_ONLY = 0x00000080;
        /// <summary> Indicates the display refresh rate is rounded</summary>
        public const int ADL_DISPLAY_MODE_REFRESHRATE_ROUNDED = 0x00000040;

        // DDCInfoX2 DDCInfo Flag values
        /// <summary> Indicates the display is a projector </summary>
        public const int ADL_DISPLAYDDCINFOEX_FLAG_PROJECTORDEVICE = (1 << 0);
        /// <summary> Indicates the display is a projector </summary>
        public const int ADL_DISPLAYDDCINFOEX_FLAG_EDIDEXTENSION = (1 << 1);
        /// <summary> Indicates the display is a projector </summary>
        public const int ADL_DISPLAYDDCINFOEX_FLAG_DIGITALDEVICE = (1 << 2);
        /// <summary> Indicates the display is a projector </summary>
        public const int ADL_DISPLAYDDCINFOEX_FLAG_HDMIAUDIODEVICE = (1 << 3);
        /// <summary> Indicates the display is a projector </summary>
        public const int ADL_DISPLAYDDCINFOEX_FLAG_SUPPORTS_AI = (1 << 4);
        /// <summary> Indicates the display is a projector </summary>
        public const int ADL_DISPLAYDDCINFOEX_FLAG_SUPPORT_xvYCC601 = (1 << 5);
        /// <summary> Indicates the display is a projector </summary>
        public const int ADL_DISPLAYDDCINFOEX_FLAG_SUPPORT_xvYCC709 = (1 << 6);




        #region Internal Constant
        /// <summary> Atiadlxx_FileName </summary>
        public const string INTEL_IGCL_DLL = "ControlLib.dll";
        /// <summary> Kernel32_FileName </summary>
        public const string Kernel32_FileName = "kernel32.dll";
        #endregion Internal Constant

        #region DLLImport
        [DllImport(Kernel32_FileName)]
        public static extern HMODULE GetModuleHandle(string moduleName);

        ///////////////////////////////////////////////////////////////////////////////
        /// @brief Control Api Init
        /// 
        /// @details
        ///     - Control Api Init
        /// 
        /// @returns
        ///     - CTL_RESULT_SUCCESS
        ///     - CTL_RESULT_ERROR_UNINITIALIZED
        ///     - CTL_RESULT_ERROR_DEVICE_LOST
        ///     - CTL_RESULT_ERROR_INVALID_NULL_POINTER
        ///         + `nullptr == pInitDesc`
        ///         + `nullptr == phAPIHandle`
        ///     - ::CTL_RESULT_ERROR_UNSUPPORTED_VERSION - "Unsupported version"
        //ctl_result_t ctlInit(
        //      ctl_init_args_t* pInitDesc,                     ///< [in][out] App's control API version
        //      ctl_api_handle_t* phAPIHandle                   ///< [in][out][release] Control API handle
        //);
        [DllImport(INTEL_IGCL_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern ctl_result_t ctlInit([In][Out] ctl_init_args_t pInitDesc, [In][Out] IntPtr phAPIHandle);

        ///////////////////////////////////////////////////////////////////////////////
        /// @brief Control Api Destroy
        /// 
        /// @details
        ///     - Control Api Close
        /// 
        /// @returns
        ///     - CTL_RESULT_SUCCESS
        ///     - CTL_RESULT_ERROR_UNINITIALIZED
        ///     - CTL_RESULT_ERROR_DEVICE_LOST
        ///     - CTL_RESULT_ERROR_INVALID_NULL_HANDLE
        ///         + `nullptr == hAPIHandle`
        ///     - ::CTL_RESULT_ERROR_UNSUPPORTED_VERSION - "Unsupported version"
        //CTL_APIEXPORT ctl_result_t CTL_APICALL
        //ctlClose(
        //    ctl_api_handle_t hAPIHandle                     ///< [in][release] Control API implementation handle obtained during init
        //                                                    ///< call
        //    );
        [DllImport(INTEL_IGCL_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern ctl_result_t ctlClose([In] IntPtr contextHandle);


        ///////////////////////////////////////////////////////////////////////////////
        /// @brief Get/Set Combined Display
        /// 
        /// @details
        ///     - To get or set combined display with given Child Targets on a Single
        ///       GPU or across identical GPUs. Multi-GPU(MGPU) combined display is
        ///       reserved i.e. it is not public and requires special application GUID.
        ///       MGPU Combined Display will get activated or deactivated in next boot.
        ///       MGPU scenario will internally link the associated adapters via Linked
        ///       Display Adapter Call, with supplied hDeviceAdapter being the LDA
        ///       Primary. If Genlock and enabled in Driver registry and supported by
        ///       given Display Config, MGPU Combined Display will enable MGPU Genlock
        ///       with supplied hDeviceAdapter being the Genlock Primary Adapter and the
        ///       First Child Display being the Primary Display.
        /// 
        /// @returns
        ///     - CTL_RESULT_SUCCESS
        ///     - CTL_RESULT_ERROR_UNINITIALIZED
        ///     - CTL_RESULT_ERROR_DEVICE_LOST
        ///     - CTL_RESULT_ERROR_INVALID_NULL_HANDLE
        ///         + `nullptr == hDeviceAdapter`
        ///     - CTL_RESULT_ERROR_INVALID_NULL_POINTER
        ///         + `nullptr == pCombinedDisplayArgs`
        ///     - ::CTL_RESULT_ERROR_UNSUPPORTED_VERSION - "Unsupported version"
        ///     - ::CTL_RESULT_ERROR_INVALID_OPERATION_TYPE - "Invalid operation type"
        ///     - ::CTL_RESULT_ERROR_INSUFFICIENT_PERMISSIONS - "Insufficient permissions"
        ///     - ::CTL_RESULT_ERROR_INVALID_NULL_POINTER - "Invalid null pointer"
        ///     - ::CTL_RESULT_ERROR_NULL_OS_DISPLAY_OUTPUT_HANDLE - "Null OS display output handle"
        ///     - ::CTL_RESULT_ERROR_NULL_OS_INTERFACE - "Null OS interface"
        ///     - ::CTL_RESULT_ERROR_NULL_OS_ADAPATER_HANDLE - "Null OS adapter handle"
        ///     - ::CTL_RESULT_ERROR_KMD_CALL - "Kernel mode driver call failure"
        ///     - ::CTL_RESULT_ERROR_FEATURE_NOT_SUPPORTED - "Combined Display feature is not supported in this platform"
        ///     - ::CTL_RESULT_ERROR_ADAPTER_NOT_SUPPORTED_ON_LDA_SECONDARY - "Unsupported (secondary) adapter handle passed"
        [DllImport(INTEL_IGCL_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern ctl_result_t ctlGetSetCombinedDisplay(
            [In] IntPtr hDeviceAdapter,     ///< [in][release] Handle to control device adapter - pointer to ctl_device_adapter_handle_t
            [In][Out] IntPtr pCombinedDisplayArgs   ///< [in,out] Setup and get combined display arguments - pointer to ctl_combined_display_args_t
            );


        ///////////////////////////////////////////////////////////////////////////////
        /// @brief Enumerate devices
        /// 
        /// @details
        ///     - The application enumerates all device adapters in the system
        /// 
        /// @returns
        ///     - CTL_RESULT_SUCCESS
        ///     - CTL_RESULT_ERROR_UNINITIALIZED
        ///     - CTL_RESULT_ERROR_DEVICE_LOST
        ///     - CTL_RESULT_ERROR_INVALID_NULL_HANDLE
        ///         + `nullptr == hAPIHandle`
        ///     - CTL_RESULT_ERROR_INVALID_NULL_POINTER
        ///         + `nullptr == pCount`
        ///     - ::CTL_RESULT_ERROR_UNSUPPORTED_VERSION - "Unsupported version"
        [DllImport(INTEL_IGCL_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern ctl_result_t ctlEnumerateDevices(
            [In] ctl_api_handle_t hAPIHandle,                    //< [in][release] Applications should pass the Control API handle returned
                                                            //< by the CtlInit function 
            [In][Out] UInt32 pCount,                        //< [in,out][release] pointer to the number of device instances. If count
                                                            //< is zero, then the api will update the value with the total
                                                            //< number of drivers available. If count is non-zero, then the api will
                                                            //< only retrieve the number of drivers.
                                                            //< If count is larger than the number of drivers available, then the api
                                                            //< will update the value with the correct number of drivers available.
            [In][Out] ctl_device_adapter_handle_t phDevices          //< [in,out][optional][release][range(0, *pCount)] array of driver
                                                            //< instance handles
            );

        ///////////////////////////////////////////////////////////////////////////////
        /// @brief Enumerate display outputs
        /// 
        /// @details
        ///     - Enumerates display output capabilities
        /// 
        /// @returns
        ///     - CTL_RESULT_SUCCESS
        ///     - CTL_RESULT_ERROR_UNINITIALIZED
        ///     - CTL_RESULT_ERROR_DEVICE_LOST
        ///     - CTL_RESULT_ERROR_INVALID_NULL_HANDLE
        ///         + `nullptr == hDeviceAdapter`
        ///     - CTL_RESULT_ERROR_INVALID_NULL_POINTER
        ///         + `nullptr == pCount`
        ///     - ::CTL_RESULT_ERROR_UNSUPPORTED_VERSION - "Unsupported version"
        [DllImport(INTEL_IGCL_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern ctl_result_t ctlEnumerateDisplayOutputs(
            [In] ctl_device_adapter_handle_t hDeviceAdapter,     //< [in][release] handle to control device adapter
            [In][Out] UInt32 pCount,                        //< [in,out][release] pointer to the number of display output instances.
                                                            //< If count is zero, then the api will update the value with the total
                                                            //< number of outputs available. If count is non-zero, then the api will
                                                            //< only retrieve the number of outputs.
                                                            //< If count is larger than the number of drivers available, then the api
                                                            //< will update the value with the correct number of drivers available.
            [In] [Out] ctl_display_output_handle_t phDisplayOutputs   //< [in,out][optional][release][range(0, *pCount)] array of display output
                                                                      //< instance handles
            );


        #endregion DLLImport

    }
}