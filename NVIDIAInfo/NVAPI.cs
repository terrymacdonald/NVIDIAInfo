using System;
using System.Runtime.InteropServices;
using FARPROC = System.IntPtr;
using HMODULE = System.IntPtr;

namespace DisplayMagicianShared.NVIDIA
{
    public delegate IntPtr ADL_Main_Memory_Alloc_Delegate(int size);

    public enum NVAPI_STATUS
    {
        // Result Codes
        NVAPI_OK = 0,      //!< Success. Request is completed.
        NVAPI_ERROR = -1,      //!< Generic error
        NVAPI_LIBRARY_NOT_FOUND = -2,      //!< NVAPI support library cannot be loaded.
        NVAPI_NO_IMPLEMENTATION = -3,      //!< not implemented in current driver installation
        NVAPI_API_NOT_INITIALIZED = -4,      //!< NvAPI_Initialize has not been called (successfully)
        NVAPI_INVALID_ARGUMENT = -5,      //!< The argument/parameter value is not valid or NULL.
        NVAPI_NVIDIA_DEVICE_NOT_FOUND = -6,      //!< No NVIDIA display driver, or NVIDIA GPU driving a display, was found.
        NVAPI_END_ENUMERATION = -7,      //!< No more items to enumerate
        NVAPI_INVALID_HANDLE = -8,      //!< Invalid handle
        NVAPI_INCOMPATIBLE_STRUCT_VERSION = -9,      //!< An argument's structure version is not supported
        NVAPI_HANDLE_INVALIDATED = -10,     //!< The handle is no longer valid (likely due to GPU or display re-configuration)
        NVAPI_OPENGL_CONTEXT_NOT_CURRENT = -11,     //!< No NVIDIA OpenGL context is current (but needs to be)
        NVAPI_INVALID_POINTER = -14,     //!< An invalid pointer, usually NULL, was passed as a parameter
        NVAPI_NO_GL_EXPERT = -12,     //!< OpenGL Expert is not supported by the current drivers
        NVAPI_INSTRUMENTATION_DISABLED = -13,     //!< OpenGL Expert is supported, but driver instrumentation is currently disabled
        NVAPI_NO_GL_NSIGHT = -15,     //!< OpenGL does not support Nsight

        NVAPI_EXPECTED_LOGICAL_GPU_HANDLE = -100,    //!< Expected a logical GPU handle for one or more parameters
        NVAPI_EXPECTED_PHYSICAL_GPU_HANDLE = -101,    //!< Expected a physical GPU handle for one or more parameters
        NVAPI_EXPECTED_DISPLAY_HANDLE = -102,    //!< Expected an NV display handle for one or more parameters
        NVAPI_INVALID_COMBINATION = -103,    //!< The combination of parameters is not valid. 
        NVAPI_NOT_SUPPORTED = -104,    //!< Requested feature is not supported in the selected GPU
        NVAPI_PORTID_NOT_FOUND = -105,    //!< No port ID was found for the I2C transaction
        NVAPI_EXPECTED_UNATTACHED_DISPLAY_HANDLE = -106,    //!< Expected an unattached display handle as one of the input parameters.
        NVAPI_INVALID_PERF_LEVEL = -107,    //!< Invalid perf level 
        NVAPI_DEVICE_BUSY = -108,    //!< Device is busy; request not fulfilled
        NVAPI_NV_PERSIST_FILE_NOT_FOUND = -109,    //!< NV persist file is not found
        NVAPI_PERSIST_DATA_NOT_FOUND = -110,    //!< NV persist data is not found
        NVAPI_EXPECTED_TV_DISPLAY = -111,    //!< Expected a TV output display
        NVAPI_EXPECTED_TV_DISPLAY_ON_DCONNECTOR = -112,    //!< Expected a TV output on the D Connector - HDTV_EIAJ4120.
        NVAPI_NO_ACTIVE_SLI_TOPOLOGY = -113,    //!< SLI is not active on this device.
        NVAPI_SLI_RENDERING_MODE_NOTALLOWED = -114,    //!< Setup of SLI rendering mode is not possible right now.
        NVAPI_EXPECTED_DIGITAL_FLAT_PANEL = -115,    //!< Expected a digital flat panel.
        NVAPI_ARGUMENT_EXCEED_MAX_SIZE = -116,    //!< Argument exceeds the expected size.
        NVAPI_DEVICE_SWITCHING_NOT_ALLOWED = -117,    //!< Inhibit is ON due to one of the flags in NV_GPU_DISPLAY_CHANGE_INHIBIT or SLI active.
        NVAPI_TESTING_CLOCKS_NOT_SUPPORTED = -118,    //!< Testing of clocks is not supported.
        NVAPI_UNKNOWN_UNDERSCAN_CONFIG = -119,    //!< The specified underscan config is from an unknown source (e.g. INF)
        NVAPI_TIMEOUT_RECONFIGURING_GPU_TOPO = -120,    //!< Timeout while reconfiguring GPUs
        NVAPI_DATA_NOT_FOUND = -121,    //!< Requested data was not found
        NVAPI_EXPECTED_ANALOG_DISPLAY = -122,    //!< Expected an analog display
        NVAPI_NO_VIDLINK = -123,    //!< No SLI video bridge is present
        NVAPI_REQUIRES_REBOOT = -124,    //!< NVAPI requires a reboot for the settings to take effect
        NVAPI_INVALID_HYBRID_MODE = -125,    //!< The function is not supported with the current Hybrid mode.
        NVAPI_MIXED_TARGET_TYPES = -126,    //!< The target types are not all the same
        NVAPI_SYSWOW64_NOT_SUPPORTED = -127,    //!< The function is not supported from 32-bit on a 64-bit system.
        NVAPI_IMPLICIT_SET_GPU_TOPOLOGY_CHANGE_NOT_ALLOWED = -128,    //!< There is no implicit GPU topology active. Use NVAPI_SetHybridMode to change topology.
        NVAPI_REQUEST_USER_TO_CLOSE_NON_MIGRATABLE_APPS = -129,      //!< Prompt the user to close all non-migratable applications.    
        NVAPI_OUT_OF_MEMORY = -130,    //!< Could not allocate sufficient memory to complete the call.
        NVAPI_WAS_STILL_DRAWING = -131,    //!< The previous operation that is transferring information to or from this surface is incomplete.
        NVAPI_FILE_NOT_FOUND = -132,    //!< The file was not found.
        NVAPI_TOO_MANY_UNIQUE_STATE_OBJECTS = -133,    //!< There are too many unique instances of a particular type of state object.
        NVAPI_INVALID_CALL = -134,    //!< The method call is invalid. For example, a method's parameter may not be a valid pointer.
        NVAPI_D3D10_1_LIBRARY_NOT_FOUND = -135,    //!< d3d10_1.dll cannot be loaded.
        NVAPI_FUNCTION_NOT_FOUND = -136,    //!< Couldn't find the function in the loaded DLL.
        NVAPI_INVALID_USER_PRIVILEGE = -137,    //!< The application will require Administrator privileges to access this API.
                                                //!< The application can be elevated to a higher permission level by selecting "Run as Administrator".
        NVAPI_EXPECTED_NON_PRIMARY_DISPLAY_HANDLE = -138,    //!< The handle corresponds to GDIPrimary.
        NVAPI_EXPECTED_COMPUTE_GPU_HANDLE = -139,    //!< Setting Physx GPU requires that the GPU is compute-capable.
        NVAPI_STEREO_NOT_INITIALIZED = -140,    //!< The Stereo part of NVAPI failed to initialize completely. Check if the stereo driver is installed.
        NVAPI_STEREO_REGISTRY_ACCESS_FAILED = -141,    //!< Access to stereo-related registry keys or values has failed.
        NVAPI_STEREO_REGISTRY_PROFILE_TYPE_NOT_SUPPORTED = -142, //!< The given registry profile type is not supported.
        NVAPI_STEREO_REGISTRY_VALUE_NOT_SUPPORTED = -143,    //!< The given registry value is not supported.
        NVAPI_STEREO_NOT_ENABLED = -144,    //!< Stereo is not enabled and the function needed it to execute completely.
        NVAPI_STEREO_NOT_TURNED_ON = -145,    //!< Stereo is not turned on and the function needed it to execute completely.
        NVAPI_STEREO_INVALID_DEVICE_INTERFACE = -146,    //!< Invalid device interface.
        NVAPI_STEREO_PARAMETER_OUT_OF_RANGE = -147,    //!< Separation percentage or JPEG image capture quality is out of [0-100] range.
        NVAPI_STEREO_FRUSTUM_ADJUST_MODE_NOT_SUPPORTED = -148, //!< The given frustum adjust mode is not supported.
        NVAPI_TOPO_NOT_POSSIBLE = -149,    //!< The mosaic topology is not possible given the current state of the hardware.
        NVAPI_MODE_CHANGE_FAILED = -150,    //!< An attempt to do a display resolution mode change has failed.        
        NVAPI_D3D11_LIBRARY_NOT_FOUND = -151,    //!< d3d11.dll/d3d11_beta.dll cannot be loaded.
        NVAPI_INVALID_ADDRESS = -152,    //!< Address is outside of valid range.
        NVAPI_STRING_TOO_SMALL = -153,    //!< The pre-allocated string is too small to hold the result.
        NVAPI_MATCHING_DEVICE_NOT_FOUND = -154,    //!< The input does not match any of the available devices.
        NVAPI_DRIVER_RUNNING = -155,    //!< Driver is running.
        NVAPI_DRIVER_NOTRUNNING = -156,    //!< Driver is not running.
        NVAPI_ERROR_DRIVER_RELOAD_REQUIRED = -157,    //!< A driver reload is required to apply these settings.
        NVAPI_SET_NOT_ALLOWED = -158,    //!< Intended setting is not allowed.
        NVAPI_ADVANCED_DISPLAY_TOPOLOGY_REQUIRED = -159,    //!< Information can't be returned due to "advanced display topology".
        NVAPI_SETTING_NOT_FOUND = -160,    //!< Setting is not found.
        NVAPI_SETTING_SIZE_TOO_LARGE = -161,    //!< Setting size is too large.
        NVAPI_TOO_MANY_SETTINGS_IN_PROFILE = -162,    //!< There are too many settings for a profile. 
        NVAPI_PROFILE_NOT_FOUND = -163,    //!< Profile is not found.
        NVAPI_PROFILE_NAME_IN_USE = -164,    //!< Profile name is duplicated.
        NVAPI_PROFILE_NAME_EMPTY = -165,    //!< Profile name is empty.
        NVAPI_EXECUTABLE_NOT_FOUND = -166,    //!< Application not found in the Profile.
        NVAPI_EXECUTABLE_ALREADY_IN_USE = -167,    //!< Application already exists in the other profile.
        NVAPI_DATATYPE_MISMATCH = -168,    //!< Data Type mismatch 
        NVAPI_PROFILE_REMOVED = -169,    //!< The profile passed as parameter has been removed and is no longer valid.
        NVAPI_UNREGISTERED_RESOURCE = -170,    //!< An unregistered resource was passed as a parameter. 
        NVAPI_ID_OUT_OF_RANGE = -171,    //!< The DisplayId corresponds to a display which is not within the normal outputId range.
        NVAPI_DISPLAYCONFIG_VALIDATION_FAILED = -172,    //!< Display topology is not valid so the driver cannot do a mode set on this configuration.
        NVAPI_DPMST_CHANGED = -173,    //!< Display Port Multi-Stream topology has been changed.
        NVAPI_INSUFFICIENT_BUFFER = -174,    //!< Input buffer is insufficient to hold the contents.    
        NVAPI_ACCESS_DENIED = -175,    //!< No access to the caller.
        NVAPI_MOSAIC_NOT_ACTIVE = -176,    //!< The requested action cannot be performed without Mosaic being enabled.
        NVAPI_SHARE_RESOURCE_RELOCATED = -177,    //!< The surface is relocated away from video memory.
        NVAPI_REQUEST_USER_TO_DISABLE_DWM = -178,    //!< The user should disable DWM before calling NvAPI.
        NVAPI_D3D_DEVICE_LOST = -179,    //!< D3D device status is D3DERR_DEVICELOST or D3DERR_DEVICENOTRESET - the user has to reset the device.
        NVAPI_INVALID_CONFIGURATION = -180,    //!< The requested action cannot be performed in the current state.
        NVAPI_STEREO_HANDSHAKE_NOT_DONE = -181,    //!< Call failed as stereo handshake not completed.
        NVAPI_EXECUTABLE_PATH_IS_AMBIGUOUS = -182,    //!< The path provided was too short to determine the correct NVDRS_APPLICATION
        NVAPI_DEFAULT_STEREO_PROFILE_IS_NOT_DEFINED = -183,    //!< Default stereo profile is not currently defined
        NVAPI_DEFAULT_STEREO_PROFILE_DOES_NOT_EXIST = -184,    //!< Default stereo profile does not exist
        NVAPI_CLUSTER_ALREADY_EXISTS = -185,    //!< A cluster is already defined with the given configuration.
        NVAPI_DPMST_DISPLAY_ID_EXPECTED = -186,    //!< The input display id is not that of a multi stream enabled connector or a display device in a multi stream topology 
        NVAPI_INVALID_DISPLAY_ID = -187,    //!< The input display id is not valid or the monitor associated to it does not support the current operation
        NVAPI_STREAM_IS_OUT_OF_SYNC = -188,    //!< While playing secure audio stream, stream goes out of sync
        NVAPI_INCOMPATIBLE_AUDIO_DRIVER = -189,    //!< Older audio driver version than required
        NVAPI_VALUE_ALREADY_SET = -190,    //!< Value already set, setting again not allowed.
        NVAPI_TIMEOUT = -191,    //!< Requested operation timed out 
        NVAPI_GPU_WORKSTATION_FEATURE_INCOMPLETE = -192,    //!< The requested workstation feature set has incomplete driver internal allocation resources
        NVAPI_STEREO_INIT_ACTIVATION_NOT_DONE = -193,    //!< Call failed because InitActivation was not called.
        NVAPI_SYNC_NOT_ACTIVE = -194,    //!< The requested action cannot be performed without Sync being enabled.    
        NVAPI_SYNC_MASTER_NOT_FOUND = -195,    //!< The requested action cannot be performed without Sync Master being enabled.
        NVAPI_INVALID_SYNC_TOPOLOGY = -196,    //!< Invalid displays passed in the NV_GSYNC_DISPLAY pointer.
        NVAPI_ECID_SIGN_ALGO_UNSUPPORTED = -197,    //!< The specified signing algorithm is not supported. Either an incorrect value was entered or the current installed driver/hardware does not support the input value.
        NVAPI_ECID_KEY_VERIFICATION_FAILED = -198,    //!< The encrypted public key verification has failed.
        NVAPI_FIRMWARE_OUT_OF_DATE = -199,    //!< The device's firmware is out of date.
        NVAPI_FIRMWARE_REVISION_NOT_SUPPORTED = -200,    //!< The device's firmware is not supported.
        NVAPI_LICENSE_CALLER_AUTHENTICATION_FAILED = -201,    //!< The caller is not authorized to modify the License.
        NVAPI_D3D_DEVICE_NOT_REGISTERED = -202,    //!< The user tried to use a deferred context without registering the device first  
        NVAPI_RESOURCE_NOT_ACQUIRED = -203,    //!< Head or SourceId was not reserved for the VR Display before doing the Modeset.
        NVAPI_TIMING_NOT_SUPPORTED = -204,    //!< Provided timing is not supported.
        NVAPI_HDCP_ENCRYPTION_FAILED = -205,    //!< HDCP Encryption Failed for the device. Would be applicable when the device is HDCP Capable.
        NVAPI_PCLK_LIMITATION_FAILED = -206,    //!< Provided mode is over sink device pclk limitation.
        NVAPI_NO_CONNECTOR_FOUND = -207,    //!< No connector on GPU found. 
        NVAPI_HDCP_DISABLED = -208,    //!< When a non-HDCP capable HMD is connected, we would inform user by this code.
        NVAPI_API_IN_USE = -209,    //!< Atleast an API is still being called
        NVAPI_NVIDIA_DISPLAY_NOT_FOUND = -210,    //!< No display found on Nvidia GPU(s).
        NVAPI_PRIV_SEC_VIOLATION = -211,    //!< Priv security violation, improper access to a secured register.
        NVAPI_INCORRECT_VENDOR = -212,    //!< NVAPI cannot be called by this vendor
        NVAPI_DISPLAY_IN_USE = -213,    //!< DirectMode Display is already in use
        NVAPI_UNSUPPORTED_CONFIG_NON_HDCP_HMD = -214,    //!< The Config is having Non-NVidia GPU with Non-HDCP HMD connected
        NVAPI_MAX_DISPLAY_LIMIT_REACHED = -215,    //!< GPU's Max Display Limit has Reached
        NVAPI_INVALID_DIRECT_MODE_DISPLAY = -216,    //!< DirectMode not Enabled on the Display
        NVAPI_GPU_IN_DEBUG_MODE = -217,    //!< GPU is in debug mode, OC is NOT allowed.
        NVAPI_D3D_CONTEXT_NOT_FOUND = -218,    //!< No NvAPI context was found for this D3D object
        NVAPI_STEREO_VERSION_MISMATCH = -219,    //!< there is version mismatch between stereo driver and dx driver
        NVAPI_GPU_NOT_POWERED = -220,    //!< GPU is not powered and so the request cannot be completed.
        NVAPI_ERROR_DRIVER_RELOAD_IN_PROGRESS = -221,    //!< The display driver update in progress.
        NVAPI_WAIT_FOR_HW_RESOURCE = -222,    //!< Wait for HW resources allocation
        NVAPI_REQUIRE_FURTHER_HDCP_ACTION = -223,    //!< operation requires further HDCP action
        NVAPI_DISPLAY_MUX_TRANSITION_FAILED = -224,    //!< Dynamic Mux transition failure
        NVAPI_INVALID_DSC_VERSION = -225,    //!< Invalid DSC version
        NVAPI_INVALID_DSC_SLICECOUNT = -226,    //!< Invalid DSC slice count
        NVAPI_INVALID_DSC_OUTPUT_BPP = -227,    //!< Invalid DSC output BPP
        NVAPI_FAILED_TO_LOAD_FROM_DRIVER_STORE = -228,    //!< There was an error while loading nvapi.dll from the driver store.
        NVAPI_NO_VULKAN = -229,    //!< OpenGL does not export Vulkan fake extensions
        NVAPI_REQUEST_PENDING = -230,    //!< A request for NvTOPPs telemetry CData has already been made and is pending a response.
        NVAPI_RESOURCE_IN_USE = -231,    //!< Operation cannot be performed because the resource is in use.

    }    


    class NVImport
    {

        public const uint NV_MAX_HEADS = 4;
        public const uint NV_MAX_VID_PROFILES = 4;
        public const uint NV_MAX_VID_STREAMS = 4;
        public const uint NV_ADVANCED_DISPLAY_HEADS = 4;
        public const uint NV_GENERIC_STRING_MAX = 4096;
        public const uint NV_LONG_STRING_MAX = 256;
        public const uint NV_MAX_ACPI_IDS = 16;
        public const uint NV_MAX_AUDIO_DEVICES = 16;
        public const uint NV_MAX_AVAILABLE_CPU_TOPOLOGIES = 256;
        public const uint NV_MAX_AVAILABLE_SLI_GROUPS = 256;
        public const uint NV_MAX_AVAILABLE_DISPLAY_HEADS = 2;
        public const uint NV_MAX_DISPLAYS = NV_PHYSICAL_GPUS * NV_ADVANCED_DISPLAY_HEADS;
        public const uint NV_MAX_GPU_PER_TOPOLOGY = 8;
        public const uint NV_MAX_GPU_TOPOLOGIES = NV_MAX_PHYSICAL_GPUS;
        public const uint NV_MAX_HEADS_PER_GPU = 32;
        public const uint NV_MAX_LOGICAL_GPUS = 64;
        public const uint NV_MAX_PHYSICAL_BRIDGES = 100;
        public const uint NV_MAX_PHYSICAL_GPUS = 64;
        public const uint NV_MAX_VIEW_MODES = 8;
        public const uint NV_PHYSICAL_GPUS = 32;
        public const uint NV_SHORT_STRING_MAX = 64;
        public const uint NV_SYSTEM_HWBC_INVALID_ID = 0xffffffff;
        public const uint NV_SYSTEM_MAX_DISPLAYS = NV_MAX_PHYSICAL_GPUS * NV_MAX_HEADS;
        public const uint NV_SYSTEM_MAX_HWBCS = 128;


        #region Internal Constant
        /// <summary> Nvapi64_FileName </summary>
        public const string NVAPI_DLL = "nvapi64.dll";
        /// <summary> Kernel32_FileName </summary>
        public const string Kernel32_FileName = "kernel32.dll";
        #endregion Internal Constant

        #region DLLImport
        [DllImport(Kernel32_FileName)]
        public static extern HMODULE GetModuleHandle(string moduleName);

        // This function initializes the NvAPI library (if not already initialized) but always increments the ref-counter.
        // This must be called before calling other NvAPI_ functions. Note: It is now mandatory to call NvAPI_Initialize before calling any other NvAPI. NvAPI_Unload should be called to unload the NVAPI Library.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS NvAPI_Initialize();

        //DESCRIPTION: Decrements the ref-counter and when it reaches ZERO, unloads NVAPI library.This must be called in pairs with NvAPI_Initialize.
        // If the client wants unload functionality, it is recommended to always call NvAPI_Initialize and NvAPI_Unload in pairs.
        // Unloading NvAPI library is not supported when the library is in a resource locked state.
        // Some functions in the NvAPI library initiates an operation or allocates certain resources and there are corresponding functions available, to complete the operation or free the allocated resources.
        // All such function pairs are designed to prevent unloading NvAPI library. For example, if NvAPI_Unload is called after NvAPI_XXX which locks a resource, it fails with NVAPI_ERROR.
        // Developers need to call the corresponding NvAPI_YYY to unlock the resources, before calling NvAPI_Unload again.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS NvAPI_Unload();

        // This is used to get a string containing the NVAPI version 
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS NvAPI_GetInterfaceVersionStringEx(out string description);

        // NVAPI SESSION HANDLING FUNCTIONS
        // This is used to get a session handle to use to maintain state across multiple NVAPI calls
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS NvAPI_DRS_CreateSession(out IntPtr session);

        // This is used to destroy a session handle to used to maintain state across multiple NVAPI calls
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS NvAPI_DRS_DestorySession(IntPtr session);

        /*// adapter functions
        //typedef int (* ADL2_DISPLAY_POSSIBLEMODE_GET) (ADL_CONTEXT_HANDLE, int, int*, ADLMode**);
        // This function retrieves the OS possible modes list for a specified input adapter.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_PossibleMode_Get(IntPtr ADLContextHandle, int adapterIndex, out int numModes, out IntPtr modes);

        //typedef int (* ADL2_ADAPTER_PRIMARY_SET) (ADL_CONTEXT_HANDLE, int);
        // This function sets the adapter index for a specified primary display adapter.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Adapter_Primary_Set(IntPtr ADLContextHandle, int primaryAdapterIndex);

        //typedef int (* ADL2_ADAPTER_PRIMARY_GET) (ADL_CONTEXT_HANDLE, int*);
        // This function retrieves the adapter index for the primary display adapter.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Adapter_Primary_Get(IntPtr ADLContextHandle, out int primaryAdapterIndex);

        //typedef int (* ADL2_ADAPTER_ACTIVE_SET) (ADL_CONTEXT_HANDLE, int, int, int*);
        // This function enables or disables extended desktop mode for a specified display.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Adapter_Active_Set(IntPtr ADLContextHandle, int primaryAdapterIndex, int desiredStatus, out int newlyActivated);


        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Adapter_NumberOfAdapters_Get(IntPtr contextHandle, out int numAdapters);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Adapter_Active_Get(IntPtr ADLContextHandle, int adapterIndex, out int status);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_AdapterX2_Caps(IntPtr ADLContextHandle, int adapterIndex, out ADL_ADAPTER_CAPSX2 adapterCapabilities);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Adapter_AdapterInfo_Get(IntPtr ADLContextHandle, int inputSize, out IntPtr AdapterInfoArray);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Adapter_AdapterInfoX2_Get(IntPtr ADLContextHandle, out IntPtr AdapterInfoArray);

        //typedef int (* ADL2_ADAPTER_ADAPTERINFOX3_GET) (ADL_CONTEXT_HANDLE context, int iAdapterIndex, int* numAdapters, AdapterInfo** lppAdapterInfo);
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Adapter_AdapterInfoX3_Get(IntPtr ADLContextHandle, int adapterIndex, out int numAdapters, out IntPtr AdapterInfoArray);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Adapter_AdapterInfoX4_Get(IntPtr ADLContextHandle, int adapterIndex, out int numAdapters, out IntPtr AdapterInfoX2Array);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_DDCInfo2_Get(IntPtr contextHandle, int adapterIndex, int displayIndex, out ADL_DDC_INFO2 DDCInfo);

        //typedef int (* ADL2_DISPLAY_DISPLAYINFO_GET) (ADL_CONTEXT_HANDLE context, int iAdapterIndex, int* lpNumDisplays, ADLDisplayInfo** lppInfo, int iForceDetect);
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_DisplayInfo_Get(IntPtr ADLContextHandle, int adapterIndex, out int numDisplays, out IntPtr displayInfoArray, int forceDetect);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_DeviceConfig_Get(IntPtr ADLContextHandle, int adapterIndex, int displayIndex, out ADL_DISPLAY_CONFIG displayConfig);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_HDRState_Get(IntPtr ADLContextHandle, int adapterIndex, ADL_DISPLAY_ID displayID, out int support, out int enable);
                               
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_DisplayMapConfig_PossibleAddAndRemove(IntPtr ADLContextHandle, int adapterIndex, int numDisplayMap, in ADL_DISPLAY_MAP displayMap, int numDisplayTarget, in ADL_DISPLAY_TARGET displayTarget, out int numPossibleAddTarget, out IntPtr possibleAddTarget, out int numPossibleRemoveTarget, out IntPtr possibleRemoveTarget);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Adapter_Desktop_Caps(IntPtr ADLContextHandle, int adapterIndex, out int DesktopCapsValue, out int DesktopCapsMask);
       
        // Function to retrieve active desktop supported SLS grid size.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Adapter_Desktop_SupportedSLSGridTypes_Get(IntPtr ADLContextHandle, int adapterIndex, int numDisplayTargetToUse, ref ADL_DISPLAY_TARGET displayTargetToUse, out int numSLSGrid, out ADL_DISPLAY_TARGET SLSGrid, out int option);

        // Function to get the current supported SLS grid patterns (MxN) for a GPU.
        // This function gets a list of supported SLS grids for a specified input adapter based on display devices currently connected to the GPU.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_SLSGrid_Caps(IntPtr ADLContextHandle, int adapterIndex, ref int NumSLSGrid, out IntPtr SLSGrid, int option);

        // Function to get the active SLS map index list for a given GPU.
        // This function retrieves a list of active SLS map indexes for a specified input GPU.            
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_SLSMapIndexList_Get(IntPtr ADLContextHandle, int adapterIndex, ref int numSLSMapIndexList, out IntPtr SLSMapIndexList, int option);

        // Definitions of the used function pointers. Add more if you use other ADL APIs
        // SLS functions
        //typedef int (* ADL2_DISPLAY_SLSMAPCONFIG_VALID) (ADL_CONTEXT_HANDLE context, int iAdapterIndex, ADLSLSMap slsMap, int iNumDisplayTarget, ADLSLSTarget* lpSLSTarget, int* lpSupportedSLSLayoutImageMode, int* lpReasonForNotSupportSLS, int iOption);
        // Function to Set SLS configuration for each display index the controller and the adapter is being mapped to.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_SLSMapConfig_Valid(IntPtr ADLContextHandle, int adapterIndex, ADL_SLS_MAP SLSMap, int numDisplayTarget, ADL_DISPLAY_TARGET[] displayTarget, out int supportedSLSLayoutImageMode, out int reasonForNotSupportingSLS, int option);

        //typedef int (* ADL2_DISPLAY_SLSMAPINDEX_GET) (ADL_CONTEXT_HANDLE, int, int, ADLDisplayTarget*, int*);
        // Function to get a SLS map index based on a group of displays that are connected in the given adapter. The driver only searches the active SLS grid database.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_SLSMapIndex_Get(IntPtr ADLContextHandle, int adapterIndex, int numDisplayTarget, ADL_DISPLAY_TARGET[] displayTarget, out int SLSMapIndex);

        //typedef int (* ADL2_DISPLAY_SLSMAPCONFIG_GET) (ADL_CONTEXT_HANDLE, int, int, ADLSLSMap*, int*, ADLSLSTarget**, int*, ADLSLSMode**, int*, ADLBezelTransientMode**, int*, ADLBezelTransientMode**, int*, ADLSLSOffset**, int);
        // This function retrieves an SLS configuration, which includes the, SLS map, SLS targets, SLS standard modes, bezel modes or a transient mode, and offsets.           
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_SLSMapConfig_Get(IntPtr ADLContextHandle, int adapterIndex, int SLSMapIndex, out ADL_SLS_MAP SLSMap, out int NumSLSTarget, out IntPtr SLSTargetArray, out int lpNumNativeMode, out IntPtr NativeMode, out int NumBezelMode, out IntPtr BezelMode, out int NumTransientMode, out IntPtr TransientMode, out int NumSLSOffset, out IntPtr SLSOffset, int iOption);

        // typedef int ADL2_Display_SLSMapConfigX2_Get(ADL_CONTEXT_HANDLE context, int iAdapterIndex, int iSLSMapIndex, ADLSLSMap* lpSLSMap, int* lpNumSLSTarget, ADLSLSTarget** lppSLSTarget, int* lpNumNativeMode, ADLSLSMode** lppNativeMode, int* lpNumNativeModeOffsets, ADLSLSOffset** lppNativeModeOffsets, int* lpNumBezelMode, ADLBezelTransientMode** lppBezelMode, int* lpNumTransientMode, ADLBezelTransientMode** lppTransientMode, int* lpNumSLSOffset, ADLSLSOffset** lppSLSOffset, int iOption)
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_SLSMapConfigX2_Get(IntPtr ADLContextHandle, int adapterIndex, int SLSMapIndex, out ADL_SLS_MAP SLSMap, out int NumSLSTarget, out IntPtr SLSTargetArray, out int lpNumNativeMode, out IntPtr NativeMode, out int NumNativeModeOffsets, out IntPtr NativeModeOffsets, out int NumBezelMode, out IntPtr BezelMode, out int NumTransientMode, out IntPtr TransientMode, out int NumSLSOffset, out IntPtr SLSOffset, int option);

        //typedef int (* ADL2_DISPLAY_SLSMAPCONFIG_DELETE) (ADL_CONTEXT_HANDLE context, int iAdapterIndex, int iSLSMapIndex);
        // This function deletes an SLS map from the driver database based on the input SLS map index.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Display_SLSMapConfig_Delete(IntPtr ADLContextHandle, int adapterIndex, int SLSMapIndex);


        //typedef int (* ADL2_DISPLAY_SLSMAPCONFIG_CREATE) (ADL_CONTEXT_HANDLE context, int iAdapterIndex, ADLSLSMap SLSMap, int iNumTarget, ADLSLSTarget* lpSLSTarget, int iBezelModePercent, int* lpSLSMapIndex, int iOption);
        // This function creates an SLS configuration with a given grid, targets, and bezel mode percent. It will output an SLS map index if the SLS map is successfully created.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_SLSMapConfig_Create(IntPtr ADLContextHandle, int adapterIndex, ADL_SLS_MAP[] SLSMap, int numDisplayTarget, ref ADL_DISPLAY_TARGET[] displayTarget, int bezelModePercent, out int SLSMapIndex, int option);

        //typedef int (* ADL2_DISPLAY_SLSMAPCONFIG_REARRANGE) (ADL_CONTEXT_HANDLE, int, int, int, ADLSLSTarget*, ADLSLSMap, int);
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_SLSMapConfig_Rearrange(IntPtr ADLContextHandle, int adapterIndex, int SLSMapIndex, int numDisplayTarget, ref ADL_DISPLAY_TARGET[] displayTarget, ADL_SLS_MAP[] SLSMap, int option);

        //typedef int (* ADL2_DISPLAY_SLSMAPCONFIG_SETSTATE) (ADL_CONTEXT_HANDLE, int, int, int);
        // This is used to set the SLS Grid we want from the SLSMap by selecting the one we want and supplying that as an index.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_SLSMapConfig_SetState(IntPtr ADLContextHandle, int adapterIndex, int SLSMapIndex, int State);


        //typedef int 	ADL2_Display_SLSRecords_Get (ADL_CONTEXT_HANDLE context, int iAdapterIndex, ADLDisplayID displayID, int *lpNumOfRecords, int **lppGridIDList)
        // This is used to set the SLS Grid we want from the SLSMap by selecting the one we want and supplying that as an index.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_SLSRecords_Get(IntPtr ADLContextHandle, int adapterIndex, ADL_DISPLAY_ID displayID, out int numOfRecords, out IntPtr gridIDList);

        //typedef int (* ADL2_DISPLAY_SLSMAPINDEXLIST_GET) (ADL_CONTEXT_HANDLE, int, int*, int**, int);
        // This function retrieves a list of active SLS map indexes for a specified input GPU.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_SLSMapIndexList_Get(IntPtr ADLContextHandle, int AdapterIndex, out int numSLSMapIndexList, IntPtr SLSMapIndexList, int option);

        //typedef int (* ADL2_DISPLAY_MODES_GET) (ADL_CONTEXT_HANDLE, int, int, int*, ADLMode**);
        // This function retrieves the current display mode information.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_Modes_Get(IntPtr ADLContextHandle, int adapterIndex, int displayIndex, out int numModes, out IntPtr modes);


        //typedef int (* ADL2_DISPLAY_MODES_SET) (ADL_CONTEXT_HANDLE, int, int, int, ADLMode*);
        // This function sets the current display mode information.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_Modes_Set(IntPtr ADLContextHandle, int adapterIndex, int displayIndex, int numModes, ref ADL_MODE modes);

        //typedef int (* ADL2_DISPLAY_BEZELOFFSET_SET) (ADL_CONTEXT_HANDLE, int, int, int, LPADLSLSOffset, ADLSLSMap, int);
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_BezelOffset_Set(IntPtr ADLContextHandle, int adapterIndex, int SLSMapIndex, int numBezelOffset, ref ADL_SLS_OFFSET displayTargetToUse, ADL_SLS_MAP SLSMap, int option);

        //display map functions
        //typedef int (* ADL2_DISPLAY_DISPLAYMAPCONFIG_GET) (ADL_CONTEXT_HANDLE context, int iAdapterIndex, int* lpNumDisplayMap, ADLDisplayMap** lppDisplayMap, int* lpNumDisplayTarget, ADLDisplayTarget** lppDisplayTarget, int iOptions);
        // This function retrieves the current display map configurations, including the controllers and adapters mapped to each display.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_DisplayMapConfig_Get(IntPtr ADLContextHandle, int adapterIndex, out int numDisplayMap, out IntPtr displayMap, out int numDisplayTarget, out IntPtr displayTarget, int options);

        //typedef int (* ADL2_DISPLAY_DISPLAYMAPCONFIG_SET) (ADL_CONTEXT_HANDLE, int, int, ADLDisplayMap*, int, ADLDisplayTarget*);
        // This function sets the current display configurations for each display, including the controller and adapter mapped to each display.
        // Possible display configurations are single, clone, extended desktop, and stretch mode.
        // If clone mode is desired and the current display configuration is extended desktop mode, the function disables extended desktop mode in order to enable clone mode.
        // If extended display mode is desired and the current display configuration is single mode, this function enables extended desktop.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_DisplayMapConfig_Set(IntPtr ADLContextHandle, int adapterIndex, int numDisplayMap, ADL_DISPLAY_MAP[] displayMap, int numDisplayTarget, ADL_DISPLAY_TARGET[] displayTarget);

        // This function validate the list of the display configurations for a specified input adapter. This function is applicable to all OS platforms.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL2_Display_DisplayMapConfig_Validate(IntPtr ADLContextHandle, int adapterIndex, int numPossibleMap, ref ADL_POSSIBLE_MAP possibleMaps, out int numPossibleMapResult, out IntPtr possibleMapResult);


        // ======================================


        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Main_Control_Create(ADL_Main_Memory_Alloc_Delegate callback, int enumConnectedAdapters);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Main_Control_Destroy();


        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Main_Control_IsFunctionValid(HMODULE module, string procName);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern FARPROC ADL_Main_Control_GetProcAddress(HMODULE module, string procName);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Adapter_NumberOfAdapters_Get(ref int numAdapters);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Adapter_AdapterInfo_Get(out IntPtr info, int inputSize);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Adapter_Active_Get(int adapterIndex, ref int status);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Adapter_ID_Get(int adapterIndex, ref int adapterId);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_AdapterX2_Caps(int adapterIndex, out ADL_ADAPTER_CAPSX2 adapterCapabilities);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Display_DisplayInfo_Get(int adapterIndex, ref int numDisplays, out IntPtr displayInfoArray, int forceDetect);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Display_DeviceConfig_Get(int adapterIndex, int displayIndex, out ADL_DISPLAY_CONFIG displayConfig);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Display_EdidData_Get(int adapterIndex, int displayIndex, ref ADL_DISPLAY_EDID_DATA EDIDData);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Display_DisplayMapConfig_Get(int adapterIndex, out int numDisplayMap, out IntPtr displayMap, out int numDisplayTarget, out IntPtr displayTarget, int options);

        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Display_DisplayMapConfig_PossibleAddAndRemove(int adapterIndex, int numDisplayMap, ADL_DISPLAY_MAP displayMap, int numDisplayTarget, ADL_DISPLAY_TARGET displayTarget, out int numPossibleAddTarget, out IntPtr possibleAddTarget, out int numPossibleRemoveTarget, out IntPtr possibleRemoveTarget);

        //typedef int (* ADL2_DISPLAY_SLSMAPCONFIG_GET) (ADL_CONTEXT_HANDLE, int, int, ADLSLSMap*, int*, ADLSLSTarget**, int*, ADLSLSMode**, int*, ADLBezelTransientMode**, int*, ADLBezelTransientMode**, int*, ADLSLSOffset**, int);
        // This function retrieves an SLS configuration, which includes the, SLS map, SLS targets, SLS standard modes, bezel modes or a transient mode, and offsets.           
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Display_SLSMapConfig_Get(int adapterIndex, int SLSMapIndex, out ADL_SLS_MAP SLSMap, out int NumSLSTarget, out IntPtr SLSTargetArray, out int lpNumNativeMode, out IntPtr NativeMode, out int NumBezelMode, out IntPtr BezelMode, out int NumTransientMode, out IntPtr TransientMode, out int NumSLSOffset, out IntPtr SLSOffset, int iOption);

        // typedef int ADL2_Display_SLSMapConfigX2_Get(ADL_CONTEXT_HANDLE context, int iAdapterIndex, int iSLSMapIndex, ADLSLSMap* lpSLSMap, int* lpNumSLSTarget, ADLSLSTarget** lppSLSTarget, int* lpNumNativeMode, ADLSLSMode** lppNativeMode, int* lpNumNativeModeOffsets, ADLSLSOffset** lppNativeModeOffsets, int* lpNumBezelMode, ADLBezelTransientMode** lppBezelMode, int* lpNumTransientMode, ADLBezelTransientMode** lppTransientMode, int* lpNumSLSOffset, ADLSLSOffset** lppSLSOffset, int iOption)
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Display_SLSMapConfigX2_Get(int adapterIndex, int SLSMapIndex, out ADL_SLS_MAP SLSMap, out int NumSLSTarget, out IntPtr SLSTargetArray, out int lpNumNativeMode, out IntPtr NativeMode, out int NumNativeModeOffsets, out IntPtr NativeModeOffsets, out int NumBezelMode, out IntPtr BezelMode, out int NumTransientMode, out IntPtr TransientMode, out int NumSLSOffset, out IntPtr SLSOffset, int option);

        // This is used to set the SLS Grid we want from the SLSMap by selecting the one we want and supplying that as an index.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Display_SLSMapConfig_SetState(int AdapterIndex, int SLSMapIndex, int State);

        // Function to get the current supported SLS grid patterns (MxN) for a GPU.
        // This function gets a list of supported SLS grids for a specified input adapter based on display devices currently connected to the GPU.
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Display_SLSGrid_Caps(int adapterIndex, ref int NumSLSGrid, out IntPtr SLSGrid, int option);

        // Function to get the active SLS map index list for a given GPU.
        // This function retrieves a list of active SLS map indexes for a specified input GPU.            
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Display_SLSMapIndexList_Get(int adapterIndex, ref int numSLSMapIndexList, out IntPtr SLSMapIndexList, int options);

        // Function to get the active SLS map index list for a given GPU.
        // This function retrieves a list of active SLS map indexes for a specified input GPU.            
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS ADL_Display_SLSMapIndex_Get(int adapterIndex, int ADLNumDisplayTarget, ref ADL_DISPLAY_TARGET displayTarget, ref int SLSMapIndex);*/

        #endregion DLLImport

        public static ADL_Main_Memory_Alloc_Delegate ADL_Main_Memory_Alloc = ADL_Main_Memory_Alloc_Function;
        /// <summary> Build in memory allocation function</summary>
        /// <param name="size">input size</param>
        /// <returns>return the memory buffer</returns>
        public static IntPtr ADL_Main_Memory_Alloc_Function(int size)
        {
            //Console.WriteLine($"\nCallback called with param: {size}");
            IntPtr result = Marshal.AllocCoTaskMem(size);           
            return result;
        }

    }        
}
