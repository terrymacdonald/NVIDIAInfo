using System;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using FARPROC = System.IntPtr;
using HMODULE = System.IntPtr;

namespace DisplayMagicianShared.NVIDIA
{
    //public delegate IntPtr ADL_Main_Memory_Alloc_Delegate(int size);

    public enum NVAPI_STATUS : int
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

    [Flags]
    public enum NV_DISPLAYCONFIG_FLAGS : uint
    {
        NV_DISPLAYCONFIG_VALIDATE_ONLY = 0x00000001,
        NV_DISPLAYCONFIG_SAVE_TO_PERSISTENCE = 0x00000002,
        NV_DISPLAYCONFIG_DRIVER_RELOAD_ALLOWED = 0x00000004,               //!< Driver reload is permitted if necessary
        NV_DISPLAYCONFIG_FORCE_MODE_ENUMERATION = 0x00000008,               //!< Refresh OS mode list.
        NV_FORCE_COMMIT_VIDPN = 0x00000010,               //!< Tell OS to avoid optimizing CommitVidPn call during a modeset
    }

    public enum NV_ROTATE : uint
    {
        NV_ROTATE_0 = 0,
        NV_ROTATE_90 = 1,
        NV_ROTATE_180 = 2,
        NV_ROTATE_270 = 3,
        NV_ROTATE_IGNORED = 4,
    }

    public enum NV_FORMAT : uint
    {
        NV_FORMAT_UNKNOWN = 0,       //!< unknown. Driver will choose one as following value.
        NV_FORMAT_P8 = 41,       //!< for 8bpp mode
        NV_FORMAT_R5G6B5 = 23,       //!< for 16bpp mode
        NV_FORMAT_A8R8G8B8 = 21,       //!< for 32bpp mode
        NV_FORMAT_A16B16G16R16F = 113,      //!< for 64bpp(floating point) mode.

    }

    public enum NV_SCALING : uint
    {
        NV_SCALING_DEFAULT = 0,        //!< No change

        // New Scaling Declarations
        NV_SCALING_GPU_SCALING_TO_CLOSEST = 1,  //!< Balanced  - Full Screen
        NV_SCALING_GPU_SCALING_TO_NATIVE = 2,  //!< Force GPU - Full Screen
        NV_SCALING_GPU_SCANOUT_TO_NATIVE = 3,  //!< Force GPU - Centered\No Scaling
        NV_SCALING_GPU_SCALING_TO_ASPECT_SCANOUT_TO_NATIVE = 5,  //!< Force GPU - Aspect Ratio
        NV_SCALING_GPU_SCALING_TO_ASPECT_SCANOUT_TO_CLOSEST = 6,  //!< Balanced  - Aspect Ratio
        NV_SCALING_GPU_SCANOUT_TO_CLOSEST = 7,  //!< Balanced  - Centered\No Scaling
        NV_SCALING_GPU_INTEGER_ASPECT_SCALING = 8,  //!< Force GPU - Integer Scaling

        // Legacy Declarations
        NV_SCALING_MONITOR_SCALING = NV_SCALING_GPU_SCALING_TO_CLOSEST,
        NV_SCALING_ADAPTER_SCALING = NV_SCALING_GPU_SCALING_TO_NATIVE,
        NV_SCALING_CENTERED = NV_SCALING_GPU_SCANOUT_TO_NATIVE,
        NV_SCALING_ASPECT_SCALING = NV_SCALING_GPU_SCALING_TO_ASPECT_SCANOUT_TO_NATIVE,

        NV_SCALING_CUSTOMIZED = 255       //!< For future use
    }

    public enum NV_TARGET_VIEW_MODE : uint
    {
        NV_VIEW_MODE_STANDARD = 0,
        NV_VIEW_MODE_CLONE = 1,
        NV_VIEW_MODE_HSPAN = 2,
        NV_VIEW_MODE_VSPAN = 3,
        NV_VIEW_MODE_DUALVIEW = 4,
        NV_VIEW_MODE_MULTIVIEW = 5,
    }

    public enum NV_DISPLAY_TV_FORMAT : uint
    {
        NV_DISPLAY_TV_FORMAT_NONE = 0,
        NV_DISPLAY_TV_FORMAT_SD_NTSCM = 0x00000001,
        NV_DISPLAY_TV_FORMAT_SD_NTSCJ = 0x00000002,
        NV_DISPLAY_TV_FORMAT_SD_PALM = 0x00000004,
        NV_DISPLAY_TV_FORMAT_SD_PALBDGH = 0x00000008,
        NV_DISPLAY_TV_FORMAT_SD_PALN = 0x00000010,
        NV_DISPLAY_TV_FORMAT_SD_PALNC = 0x00000020,
        NV_DISPLAY_TV_FORMAT_SD_576i = 0x00000100,
        NV_DISPLAY_TV_FORMAT_SD_480i = 0x00000200,
        NV_DISPLAY_TV_FORMAT_ED_480p = 0x00000400,
        NV_DISPLAY_TV_FORMAT_ED_576p = 0x00000800,
        NV_DISPLAY_TV_FORMAT_HD_720p = 0x00001000,
        NV_DISPLAY_TV_FORMAT_HD_1080i = 0x00002000,
        NV_DISPLAY_TV_FORMAT_HD_1080p = 0x00004000,
        NV_DISPLAY_TV_FORMAT_HD_720p50 = 0x00008000,
        NV_DISPLAY_TV_FORMAT_HD_1080p24 = 0x00010000,
        NV_DISPLAY_TV_FORMAT_HD_1080i50 = 0x00020000,
        NV_DISPLAY_TV_FORMAT_HD_1080p50 = 0x00040000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp30 = 0x00080000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp30_3840 = NV_DISPLAY_TV_FORMAT_UHD_4Kp30,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp25 = 0x00100000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp25_3840 = NV_DISPLAY_TV_FORMAT_UHD_4Kp25,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp24 = 0x00200000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp24_3840 = NV_DISPLAY_TV_FORMAT_UHD_4Kp24,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp24_SMPTE = 0x00400000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp50_3840 = 0x00800000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp60_3840 = 0x00900000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp30_4096 = 0x00A00000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp25_4096 = 0x00B00000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp24_4096 = 0x00C00000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp50_4096 = 0x00D00000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp60_4096 = 0x00E00000,
        NV_DISPLAY_TV_FORMAT_UHD_8Kp24_7680 = 0x01000000,
        NV_DISPLAY_TV_FORMAT_UHD_8Kp25_7680 = 0x02000000,
        NV_DISPLAY_TV_FORMAT_UHD_8Kp30_7680 = 0x04000000,
        NV_DISPLAY_TV_FORMAT_UHD_8Kp48_7680 = 0x08000000,
        NV_DISPLAY_TV_FORMAT_UHD_8Kp50_7680 = 0x09000000,
        NV_DISPLAY_TV_FORMAT_UHD_8Kp60_7680 = 0x0A000000,
        NV_DISPLAY_TV_FORMAT_UHD_8Kp100_7680 = 0x0B000000,
        NV_DISPLAY_TV_FORMAT_UHD_8Kp120_7680 = 0x0C000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp48_3840 = 0x0D000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp48_4096 = 0x0E000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp100_4096 = 0x0F000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp100_3840 = 0x10000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp120_4096 = 0x11000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp120_3840 = 0x12000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp100_5120 = 0x13000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp120_5120 = 0x14000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp24_5120 = 0x15000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp25_5120 = 0x16000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp30_5120 = 0x17000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp48_5120 = 0x18000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp50_5120 = 0x19000000,
        NV_DISPLAY_TV_FORMAT_UHD_4Kp60_5120 = 0x20000000,
        NV_DISPLAY_TV_FORMAT_UHD_10Kp24_10240 = 0x21000000,
        NV_DISPLAY_TV_FORMAT_UHD_10Kp25_10240 = 0x22000000,
        NV_DISPLAY_TV_FORMAT_UHD_10Kp30_10240 = 0x23000000,
        NV_DISPLAY_TV_FORMAT_UHD_10Kp48_10240 = 0x24000000,
        NV_DISPLAY_TV_FORMAT_UHD_10Kp50_10240 = 0x25000000,
        NV_DISPLAY_TV_FORMAT_UHD_10Kp60_10240 = 0x26000000,
        NV_DISPLAY_TV_FORMAT_UHD_10Kp100_10240 = 0x27000000,
        NV_DISPLAY_TV_FORMAT_UHD_10Kp120_10240 = 0x28000000,


        NV_DISPLAY_TV_FORMAT_SD_OTHER = 0x30000000,
        NV_DISPLAY_TV_FORMAT_ED_OTHER = 0x40000000,
        NV_DISPLAY_TV_FORMAT_HD_OTHER = 0x50000000,

        NV_DISPLAY_TV_FORMAT_ANY = 0x80000000,

    }

    public enum NV_GPU_CONNECTOR_TYPE : uint
    {
        NVAPI_GPU_CONNECTOR_VGA_15_PIN = 0x00000000,
        NVAPI_GPU_CONNECTOR_TV_COMPOSITE = 0x00000010,
        NVAPI_GPU_CONNECTOR_TV_SVIDEO = 0x00000011,
        NVAPI_GPU_CONNECTOR_TV_HDTV_COMPONENT = 0x00000013,
        NVAPI_GPU_CONNECTOR_TV_SCART = 0x00000014,
        NVAPI_GPU_CONNECTOR_TV_COMPOSITE_SCART_ON_EIAJ4120 = 0x00000016,
        NVAPI_GPU_CONNECTOR_TV_HDTV_EIAJ4120 = 0x00000017,
        NVAPI_GPU_CONNECTOR_PC_POD_HDTV_YPRPB = 0x00000018,
        NVAPI_GPU_CONNECTOR_PC_POD_SVIDEO = 0x00000019,
        NVAPI_GPU_CONNECTOR_PC_POD_COMPOSITE = 0x0000001A,
        NVAPI_GPU_CONNECTOR_DVI_I_TV_SVIDEO = 0x00000020,
        NVAPI_GPU_CONNECTOR_DVI_I_TV_COMPOSITE = 0x00000021,
        NVAPI_GPU_CONNECTOR_DVI_I = 0x00000030,
        NVAPI_GPU_CONNECTOR_DVI_D = 0x00000031,
        NVAPI_GPU_CONNECTOR_ADC = 0x00000032,
        NVAPI_GPU_CONNECTOR_LFH_DVI_I_1 = 0x00000038,
        NVAPI_GPU_CONNECTOR_LFH_DVI_I_2 = 0x00000039,
        NVAPI_GPU_CONNECTOR_SPWG = 0x00000040,
        NVAPI_GPU_CONNECTOR_OEM = 0x00000041,
        NVAPI_GPU_CONNECTOR_DISPLAYPORT_EXTERNAL = 0x00000046,
        NVAPI_GPU_CONNECTOR_DISPLAYPORT_INTERNAL = 0x00000047,
        NVAPI_GPU_CONNECTOR_DISPLAYPORT_MINI_EXT = 0x00000048,
        NVAPI_GPU_CONNECTOR_HDMI_A = 0x00000061,
        NVAPI_GPU_CONNECTOR_HDMI_C_MINI = 0x00000063,
        NVAPI_GPU_CONNECTOR_LFH_DISPLAYPORT_1 = 0x00000064,
        NVAPI_GPU_CONNECTOR_LFH_DISPLAYPORT_2 = 0x00000065,
        NVAPI_GPU_CONNECTOR_VIRTUAL_WFD = 0x00000070,
        NVAPI_GPU_CONNECTOR_USB_C = 0x00000071,
        NVAPI_GPU_CONNECTOR_UNKNOWN = 0xFFFFFFFF,
    }

    public enum NV_TIMING_OVERRIDE : uint
    {
        NV_TIMING_OVERRIDE_CURRENT = 0,          //!< get the current timing
        NV_TIMING_OVERRIDE_AUTO,                 //!< the timing the driver will use based the current policy
        NV_TIMING_OVERRIDE_EDID,                 //!< EDID timing
        NV_TIMING_OVERRIDE_DMT,                  //!< VESA DMT timing
        NV_TIMING_OVERRIDE_DMT_RB,               //!< VESA DMT timing with reduced blanking
        NV_TIMING_OVERRIDE_CVT,                  //!< VESA CVT timing
        NV_TIMING_OVERRIDE_CVT_RB,               //!< VESA CVT timing with reduced blanking
        NV_TIMING_OVERRIDE_GTF,                  //!< VESA GTF timing
        NV_TIMING_OVERRIDE_EIA861,               //!< EIA 861x pre-defined timing
        NV_TIMING_OVERRIDE_ANALOG_TV,            //!< analog SD/HDTV timing
        NV_TIMING_OVERRIDE_CUST,                 //!< NV custom timings
        NV_TIMING_OVERRIDE_NV_PREDEFINED,        //!< NV pre-defined timing (basically the PsF timings)
        NV_TIMING_OVERRIDE_NV_PSF = NV_TIMING_OVERRIDE_NV_PREDEFINED,
        NV_TIMING_OVERRIDE_NV_ASPR,
        NV_TIMING_OVERRIDE_SDI,                  //!< Override for SDI timing

        NV_TIMING_OVRRIDE_MAX,
    }


    //
    //! This is a complete list of supported Mosaic topologies.
    //!
    //! Using a "Basic" topology combines multiple monitors to create a single desktop.
    //!
    //! Using a "Passive" topology combines multiples monitors to create a passive stereo desktop.
    //! In passive stereo, two identical topologies combine - one topology is used for the right eye and the other identical //! topology (targeting different displays) is used for the left eye.  \n  
    //! NOTE: common\inc\nvEscDef.h shadows a couple PASSIVE_STEREO enums.  If this
    //!       enum list changes and effects the value of NV_MOSAIC_TOPO_BEGIN_PASSIVE_STEREO
    //!       please update the corresponding value in nvEscDef.h
    public enum NV_MOSAIC_TOPO : uint
    {
        NV_MOSAIC_TOPO_NONE,

        // 'BASIC' topos start here
        //
        // The result of using one of these Mosaic topos is that multiple monitors
        // will combine to create a single desktop.
        //
        NV_MOSAIC_TOPO_BEGIN_BASIC,
        NV_MOSAIC_TOPO_1x2_BASIC = NV_MOSAIC_TOPO_BEGIN_BASIC,
        NV_MOSAIC_TOPO_2x1_BASIC,
        NV_MOSAIC_TOPO_1x3_BASIC,
        NV_MOSAIC_TOPO_3x1_BASIC,
        NV_MOSAIC_TOPO_1x4_BASIC,
        NV_MOSAIC_TOPO_4x1_BASIC,
        NV_MOSAIC_TOPO_2x2_BASIC,
        NV_MOSAIC_TOPO_2x3_BASIC,
        NV_MOSAIC_TOPO_2x4_BASIC,
        NV_MOSAIC_TOPO_3x2_BASIC,
        NV_MOSAIC_TOPO_4x2_BASIC,
        NV_MOSAIC_TOPO_1x5_BASIC,
        NV_MOSAIC_TOPO_1x6_BASIC,
        NV_MOSAIC_TOPO_7x1_BASIC,

        // Add padding for 10 more entries. 6 will be enough room to specify every
        // possible topology with 8 or fewer displays, so this gives us a little
        // extra should we need it.
        NV_MOSAIC_TOPO_END_BASIC = NV_MOSAIC_TOPO_7x1_BASIC + 9,

        // 'PASSIVE_STEREO' topos start here
        //
        // The result of using one of these Mosaic topos is that multiple monitors
        // will combine to create a single PASSIVE STEREO desktop.  What this means is
        // that there will be two topos that combine to create the overall desktop.
        // One topo will be used for the left eye, and the other topo (of the
        // same rows x cols), will be used for the right eye.  The difference between
        // the two topos is that different GPUs and displays will be used.
        //
        NV_MOSAIC_TOPO_BEGIN_PASSIVE_STEREO,    // value shadowed in nvEscDef.h
        NV_MOSAIC_TOPO_1x2_PASSIVE_STEREO = NV_MOSAIC_TOPO_BEGIN_PASSIVE_STEREO,
        NV_MOSAIC_TOPO_2x1_PASSIVE_STEREO,
        NV_MOSAIC_TOPO_1x3_PASSIVE_STEREO,
        NV_MOSAIC_TOPO_3x1_PASSIVE_STEREO,
        NV_MOSAIC_TOPO_1x4_PASSIVE_STEREO,
        NV_MOSAIC_TOPO_4x1_PASSIVE_STEREO,
        NV_MOSAIC_TOPO_2x2_PASSIVE_STEREO,
        NV_MOSAIC_TOPO_END_PASSIVE_STEREO = NV_MOSAIC_TOPO_2x2_PASSIVE_STEREO + 4,


        //
        // Total number of topos.  Always leave this at the end of the enumeration.
        //
        NV_MOSAIC_TOPO_MAX  //! Total number of topologies.

    }

    // From Soroush Falahati's NVAPIWrapper
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    internal struct StructureVersion
    {
        private readonly uint _version;

        public uint Version
        {
            get => _version;
        }

        public int VersionNumber
        {
            get => (int)(_version >> 16);
        }

        public int StructureSize
        {
            get => (int)(_version & ~(0xFFFF << 16));
        }

        public StructureVersion(int version, Type structureType) 
        {
            _version = (uint)(Marshal.SizeOf(structureType) | (version << 16));
        }

        public override string ToString()
        {
            return $"Structure Size: {StructureSize} Bytes, Version: {VersionNumber}";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayHandle
    {
        private readonly IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UnAttachedDisplayHandle
    {
        public readonly IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PhysicalGpuHandle
    {
        private readonly IntPtr ptr;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct LogicalGpuHandle
    {
        private readonly IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct NV_TIMINGEXT
    {
        public uint flag;          //!< Reserved for NVIDIA hardware-based enhancement, such as double-scan.
        public ushort rr;            //!< Logical refresh rate to present
        public uint rrx1k;         //!< Physical vertical refresh rate in 0.001Hz
        public uint aspect;        //!< Display aspect ratio Hi(aspect):horizontal-aspect, Low(aspect):vertical-aspect
        public ushort rep;           //!< Bit-wise pixel repetition factor: 0x1:no pixel repetition; 0x2:each pixel repeats twice horizontally,..
        public uint status;        //!< Timing standard
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string name;      //!< Timing name
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NV_TIMING
    {
        // VESA scan out timing parameters:
        public ushort HVisible;         //!< horizontal visible
        public ushort HBorder;          //!< horizontal border
        public ushort HFrontPorch;      //!< horizontal front porch
        public ushort HSyncWidth;       //!< horizontal sync width
        public ushort HTotal;           //!< horizontal total
        public byte HSyncPol;         //!< horizontal sync polarity: 1-negative, 0-positive

        public ushort VVisible;         //!< vertical visible
        public ushort VBorder;          //!< vertical border
        public ushort VFrontPorch;      //!< vertical front porch
        public ushort VSyncWidth;       //!< vertical sync width
        public ushort VTotal;           //!< vertical total
        public byte VSyncPol;         //!< vertical sync polarity: 1-negative, 0-positive

        public ushort interlaced;       //!< 1-interlaced, 0-progressive
        public uint pclk;             //!< pixel clock in 10 kHz

        //other timing related extras
        NV_TIMINGEXT etc;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct NV_POSITION
    {
        public int x;
        public int y;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct NV_RESOLUTION
    {
        public uint width;
        public uint height;
        public uint colorDepth;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NV_VIEWPORTF
    {
        public float x;    //!<  x-coordinate of the viewport top-left point
        public float y;    //!<  y-coordinate of the viewport top-left point
        public float w;    //!<  Width of the viewport
        public float h;    //!<  Height of the viewport
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NV_DISPLAYCONFIG_PATH_ADVANCED_TARGET_INFO
    {
        public uint version;

        // Rotation and Scaling
        public NV_ROTATE rotation;       //!< (IN) rotation setting.
        public NV_SCALING scaling;        //!< (IN) scaling setting.

        // Refresh Rate
        public uint refreshRate1K;  //!< (IN) Non-interlaced Refresh Rate of the mode, multiplied by 1000, 0 = ignored
                                    //!< This is the value which driver reports to the OS.
                                    // Flags
        //public uint interlaced:1;   //!< (IN) Interlaced mode flag, ignored if refreshRate == 0
        //public uint primary:1;      //!< (IN) Declares primary display in clone configuration. This is *NOT* GDI Primary.
                                    //!< Only one target can be primary per source. If no primary is specified, the first
                                    //!< target will automatically be primary.
        //public uint isPanAndScanTarget:1; //!< Whether on this target Pan and Scan is enabled or has to be enabled. Valid only
                                          //!< when the target is part of clone topology.
        //public uint disableVirtualModeSupport:1;
        //public uint isPreferredUnscaledTarget:1;
        //public uint reserved:27;
        // TV format information
        public NV_GPU_CONNECTOR_TYPE connector;      //!< Specify connector type. For TV only, ignored if tvFormat == NV_DISPLAY_TV_FORMAT_NONE
        public NV_DISPLAY_TV_FORMAT tvFormat;       //!< (IN) to choose the last TV format set this value to NV_DISPLAY_TV_FORMAT_NONE
                                                    //!< In case of NvAPI_DISP_GetDisplayConfig(), this field will indicate the currently applied TV format;
                                                    //!< if no TV format is applied, this field will have NV_DISPLAY_TV_FORMAT_NONE value.
                                                    //!< In case of NvAPI_DISP_SetDisplayConfig(), this field should only be set in case of TVs;
                                                    //!< for other displays this field will be ignored and resolution & refresh rate specified in input will be used to apply the TV format.

        // Backend (raster) timing standard
        public NV_TIMING_OVERRIDE timingOverride;     //!< Ignored if timingOverride == NV_TIMING_OVERRIDE_CURRENT
        public NV_TIMING timing;             //!< Scan out timing, valid only if timingOverride == NV_TIMING_OVERRIDE_CUST
                                      //!< The value NV_TIMING::NV_TIMINGEXT::rrx1k is obtained from the EDID. The driver may
                                      //!< tweak this value for HDTV, stereo, etc., before reporting it to the OS.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NV_DISPLAYCONFIG_PATH_TARGET_INFO_V2
    {
        public uint  displayId;  //!< Display ID
        NV_DISPLAYCONFIG_PATH_ADVANCED_TARGET_INFO[] details;    //!< May be NULL if no advanced settings are required
        public uint targetId;   //!< Windows CCD target ID. Must be present only for non-NVIDIA adapter, for NVIDIA adapter this parameter is ignored.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NV_DISPLAYCONFIG_PATH_INFO_V2
    {
        public uint Version;
        public uint SourceId;               //!< Identifies sourceId used by Windows CCD. This can be optionally set.

        public uint TargetInfoCount;            //!< Number of elements in targetInfo array
        public NV_DISPLAYCONFIG_PATH_TARGET_INFO_V2[] TargetInfo;
        public NV_DISPLAYCONFIG_SOURCE_MODE_INFO_V1[] sourceModeInfo;             //!< May be NULL if mode info is not important
        //public uint IsNonNVIDIAAdapter : 1;     //!< True for non-NVIDIA adapter.
        //public uint reserved : 31;              //!< Must be 0
        //public LUID pOSAdapterID;              //!< Used by Non-NVIDIA adapter for pointer to OS Adapter of LUID
                                     //!< type, type casted to void *.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NV_DISPLAYCONFIG_SOURCE_MODE_INFO_V1
    {
        public NV_RESOLUTION resolution;
        public NV_FORMAT colorFormat;                //!< Ignored at present, must be NV_FORMAT_UNKNOWN (0)
        public NV_POSITION position;                   //!< Is all positions are 0 or invalid, displays will be automatically
                                                       //!< positioned from left to right with GDI Primary at 0,0, and all
                                                       //!< other displays in the order of the path array.
        //public NV_DISPLAYCONFIG_SPANNING_ORIENTATION spanningOrientation;        //!< Spanning is only supported on XP
        //public uint bGDIPrimary : 1;
        //public uint bSLIFocus : 1;
        //public uint reserved : 30;              //!< Must be 0
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct NV_DISPLAYCONFIG_PATH_TARGET_INFO
    {
        public uint displayId;  //!< Display ID
        public NV_DISPLAYCONFIG_PATH_ADVANCED_TARGET_INFO[] details;    //!< May be NULL if no advanced settings are required
        public uint targetId;   //!< Windows CCD target ID. Must be present only for non-NVIDIA adapter, for NVIDIA adapter this parameter is ignored.
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct NV_MOSAIC_TOPO_BRIEF : IEquatable<NV_MOSAIC_TOPO_BRIEF> // Note: Version 1 of NV_MOSAIC_TOPO_BRIEF structure
    {
        public uint Version;            // Version of this structure - MUST BE SET TO 1
        public NV_MOSAIC_TOPO Topo;     //!< The topology
        public uint Enabled;            //!< 1 if topo is enabled, else 0
        public uint IsPossible;         //!< 1 if topo *can* be enabled, else 0

        public bool Equals(NV_MOSAIC_TOPO_BRIEF other)
        => Version == other.Version &&
           Topo.Equals(other.Topo) &&
           Enabled == other.Enabled &&
           IsPossible == other.IsPossible ;

        public override int GetHashCode()
        {
            return (Version, Topo, Enabled, IsPossible).GetHashCode();
        }

    }

    //
    //! This structure defines a group of topologies that work together to create one
    //! overall layout.  All of the supported topologies are represented with this
    //! structure.
    //!
    //! For example, a 'Passive Stereo' topology would be represented with this
    //! structure, and would have separate topology details for the left and right eyes.
    //! The count would be 2.  A 'Basic' topology is also represented by this structure,
    //! with a count of 1.
    //!
    //! The structure is primarily used internally, but is exposed to applications in a
    //! read-only fashion because there are some details in it that might be useful
    //! (like the number of rows/cols, or connected display information).  A user can
    //! get the filled-in structure by calling NvAPI_Mosaic_GetTopoGroup().
    //!
    //! You can then look at the detailed values within the structure.  There are no
    //! entrypoints which take this structure as input (effectively making it read-only).
    [StructLayout(LayoutKind.Sequential)]
    public struct NV_MOSAIC_TOPO_GROUP : IEquatable<NV_MOSAIC_TOPO_GROUP> // Note: Version 1 of NV_MOSAIC_TOPO_GROUP structure
    {
        public uint Version;                        // Version of this structure - MUST BE SET TO 1
        public NV_MOSAIC_TOPO_BRIEF Brief;          //!< The brief details of this topo
        public uint Count;                          //!< Number of topos in array below
        public NV_MOSAIC_TOPO_DETAILS[] Topos;      //!< Topo Array with 1 or 2 entries in it

        public bool Equals(NV_MOSAIC_TOPO_GROUP other)
        => Version == other.Version &&
           Brief.Equals(other.Brief) &&
           Count == other.Count; // &&
           //Topos.SequenceEqual(other.Topos);

        public override int GetHashCode()
        {
            return (Version, Brief, Count, Topos).GetHashCode();
        }

    }


    [StructLayout(LayoutKind.Explicit, Size = 1176)]
    public struct NV_MOSAIC_TOPO_DETAILS : IEquatable<NV_MOSAIC_TOPO_DETAILS> // Note: Version 1 of NV_MOSAIC_TOPO_DETAILS structure
    {
        [FieldOffset(0)]
        public uint Version;            // Version of this structure - MUST BE SET TO 1 size is 4
        [FieldOffset(4)]
        public LogicalGpuHandle LogicalGPUHandle;     //!< Logical GPU for this topology  size is 8
        [FieldOffset(12)]
        public uint ValidityMask;            //!< 0 means topology is valid with the current hardware. size is 4
                                             //! If not 0, inspect bits against NV_MOSAIC_TOPO_VALIDITY_*.
        [FieldOffset(16)]
        public uint RowCount;         //!< Number of displays in a row. size is 4
        [FieldOffset(20)]
        public uint ColCount;         //!< Number of displays in a column. size is 4
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] // 
        [MarshalAs(UnmanagedType.LPArray, ArraySubType= UnmanagedType.ByValArray, SizeParamIndex= 1, SizeConst = 64)] // 
        [FieldOffset(24)]
        public NV_MOSAIC_TOPO_GPU_LAYOUT_CELL[,] GPULayout;

        /*public NV_MOSAIC_TOPO_GPU_LAYOUT_CELL[][] GPULayout
        {
            get
            {
                var columns = (int)ColCount;

                return GPULayoutRows.Take((int)RowCount).Select(row => row.GPULayoutColumns.Take(columns).ToArray()).ToArray();
            }
        }
*/
        public bool Equals(NV_MOSAIC_TOPO_DETAILS other)
        => Version == other.Version &&
           LogicalGPUHandle.Equals(other.LogicalGPUHandle) &&
           ValidityMask == other.ValidityMask &&
           RowCount == other.RowCount &&
           ColCount == other.ColCount &&
           ValidityMask == other.ValidityMask; // &&
           //GPULayout.SequenceEqual(other.GPULayout);

        public override int GetHashCode()
        {
            return (Version, LogicalGPUHandle, ValidityMask, RowCount, ColCount, ValidityMask).GetHashCode();
        }

    }

    /*[StructLayout(LayoutKind.Sequential)]
    public struct NV_MOSAIC_TOPO_GPU_LAYOUT_ROW : IEquatable<NV_MOSAIC_TOPO_GPU_LAYOUT_ROW>
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] // 
        public NV_MOSAIC_TOPO_GPU_LAYOUT_CELL[] GPULayoutColumns;     //!< The GPU Layout Columns
        public bool Equals(NV_MOSAIC_TOPO_GPU_LAYOUT_ROW other)
        => GPULayoutColumns.Length == other.GPULayoutColumns.Length;
            //GPULayoutColumns.SequenceEqual(other.GPULayoutColumns);

        public override int GetHashCode()
        {
            return (GPULayoutColumns).GetHashCode();
        }

        *//* public bool SequenceEqual(NV_MOSAIC_TOPO_GPU_LAYOUT[][] other)
         => PhysicalGPUHandle.Equals(other.PhysicalGPUHandle) &&
            DisplayOutputId == other.DisplayOutputId &&
             OverlapX == other.OverlapX &&
             OverlapY == other.OverlapY;*//*

    }*/

    [StructLayout(LayoutKind.Explicit, Size=18)]
    public struct NV_MOSAIC_TOPO_GPU_LAYOUT_CELL : IEquatable<NV_MOSAIC_TOPO_GPU_LAYOUT_CELL>
    {
        [FieldOffset(0)]
        public PhysicalGpuHandle PhysicalGPUHandle;     //!< Physical GPU to be used in the topology (0 if GPU missing) size is 8
        [FieldOffset(8)]
        public uint DisplayOutputId;            //!< Connected display target(0 if no display connected) size is 4
        [FieldOffset(10)]
        public int OverlapX;         //!< Pixels of overlap on left of target: (+overlap, -gap) size is 4
        [FieldOffset(14)]
        public int OverlapY;         //!< Pixels of overlap on top of target: (+overlap, -gap) size is 4

        public bool Equals(NV_MOSAIC_TOPO_GPU_LAYOUT_CELL other)
        => PhysicalGPUHandle.Equals(other.PhysicalGPUHandle) &&
           DisplayOutputId == other.DisplayOutputId &&
            OverlapX == other.OverlapX &&
            OverlapY == other.OverlapY;

        public override int GetHashCode()
        {
            return (PhysicalGPUHandle, DisplayOutputId, OverlapX, OverlapY).GetHashCode();
        }

       /* public bool SequenceEqual(NV_MOSAIC_TOPO_GPU_LAYOUT[][] other)
        => PhysicalGPUHandle.Equals(other.PhysicalGPUHandle) &&
           DisplayOutputId == other.DisplayOutputId &&
            OverlapX == other.OverlapX &&
            OverlapY == other.OverlapY;*/

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NV_MOSAIC_DISPLAY_SETTING : IEquatable<NV_MOSAIC_DISPLAY_SETTING> // Note: Version 2 of NV_MOSAIC_DISPLAY_SETTING structure
    {
        public uint Version;            // Version of this structure - MUST BE SET TO 2
        public uint Width;              //!< Per-display width
        public uint Height;             //!< Per-display height
        public uint Bpp;                //!< Bits per pixel
        public uint Freq;               //!< Display frequency
        public uint Rrx1k;              //!< Display frequency in x1k

        public bool Equals(NV_MOSAIC_DISPLAY_SETTING other)
        => Version == other.Version &&
           Width == other.Width &&
           Height == other.Height &&
           Bpp == other.Bpp &&
           Freq == other.Freq &&
           Rrx1k == other.Rrx1k;

        public override int GetHashCode()
        {
            return (Version, Width, Height, Bpp, Freq, Rrx1k).GetHashCode();
        }
    }

    static class NVImport
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
        public const int NV_MAX_PHYSICAL_GPUS = 64;
        public const uint NV_MAX_VIEW_MODES = 8;
        public const uint NV_PHYSICAL_GPUS = 32;
        public const uint NV_SHORT_STRING_MAX = 64;
        public const uint NV_SYSTEM_HWBC_INVALID_ID = 0xffffffff;
        public const uint NV_SYSTEM_MAX_DISPLAYS = NV_MAX_PHYSICAL_GPUS * NV_MAX_HEADS;
        public const uint NV_SYSTEM_MAX_HWBCS = 128;
        public const uint NV_MOSAIC_DISPLAY_SETTINGS_MAX = 40;
        public const uint NV_MOSAIC_TOPO_IDX_DEFAULT = 0;
        public const uint NV_MOSAIC_TOPO_IDX_LEFT_EYE = 0;
        public const uint NV_MOSAIC_TOPO_IDX_RIGHT_EYE = 1;
        public const uint NV_MOSAIC_TOPO_NUM_EYES = 2;
        public const uint NVAPI_MAX_MOSAIC_DISPLAY_ROWS = 8;
        public const uint NVAPI_MAX_MOSAIC_DISPLAY_COLUMNS = 8;
        //
        //! This defines the maximum number of topos that can be in a topo group.
        //! At this time, it is set to 2 because our largest topo group (passive
        //! stereo) only needs 2 topos (left eye and right eye).
        //!
        //! If a new topo group with more than 2 topos is added above, then this
        //! number will also have to be incremented.
        public const uint NV_MOSAIC_MAX_TOPO_PER_TOPO_GROUP = 2;
        //
        // These bits are used to describe the validity of a topo.
        //
        public const uint NV_MOSAIC_TOPO_VALIDITY_VALID = 0x00000000;  //!< The topology is valid
        public const uint NV_MOSAIC_TOPO_VALIDITY_MISSING_GPU = 0x00000001;  //!< Not enough SLI GPUs were found to fill the entire
                                                                             //! topology. hPhysicalGPU will be 0 for these.
        public const uint NV_MOSAIC_TOPO_VALIDITY_MISSING_DISPLAY = 0x00000002;  //!< Not enough displays were found to fill the entire
                                                                                 //! topology. displayOutputId will be 0 for these.
        public const uint NV_MOSAIC_TOPO_VALIDITY_MIXED_DISPLAY_TYPES = 0x00000004;  //!< The topoogy is only possible with displays of the same
        //! NV_GPU_OUTPUT_TYPE. Check displayOutputIds to make
        //! sure they are all CRTs, or all DFPs.

        // Version Constants
        public const int NV_MOSAIC_TOPO_BRIEF_VER = 1; // We're using structure version 1
        public const int NV_MOSAIC_DISPLAY_SETTING_VER = 2; // We're using structure version 2
        public const int NV_MOSAIC_TOPO_GROUP_VER = 1; // We're using structure version 1
        public const int NV_MOSAIC_TOPO_DETAILS_VER = 1; // We're using structure version 1

        #region Internal Constant
        /// <summary> Nvapi64_FileName </summary>
        public const string NVAPI_DLL = @"nvapi64.dll";
        /// <summary> Kernel32_FileName </summary>
        //public const string Kernel32_FileName = "kernel32.dll";
        #endregion Internal Constant

        #region DLLImport
        
        /*[DllImport(Kernel32_FileName)]
        public static extern HMODULE GetModuleHandle(string moduleName);*/


        /*// Delegate
        // This function initializes the NvAPI library (if not already initialized) but always increments the ref-counter.
        // This must be called before calling other NvAPI_ functions. Note: It is now mandatory to call NvAPI_Initialize before calling any other NvAPI. NvAPI_Unload should be called to unload the NVAPI Library.
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate NVAPI_STATUS NvAPI_Initialize();

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

        // This API lets caller retrieve the current global display configuration.
        // USAGE: The caller might have to call this three times to fetch all the required configuration details as follows:
        // First Pass: Caller should Call NvAPI_DISP_GetDisplayConfig() with pathInfo set to NULL to fetch pathInfoCount.
        // Second Pass: Allocate memory for pathInfo with respect to the number of pathInfoCount(from First Pass) to fetch targetInfoCount. If sourceModeInfo is needed allocate memory or it can be initialized to NULL.
        // Third Pass(Optional, only required if target information is required): Allocate memory for targetInfo with respect to number of targetInfoCount(from Second Pass).
        [DllImport(NVAPI_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern NVAPI_STATUS NvAPI_DISP_GetDisplayConfig(ref ulong pathInfoCount, out NV_DISPLAYCONFIG_PATH_TARGET_INFO_V2 pathInfo);
*/

        #endregion DLLImport


        #region Defines

        
        #endregion

        

        #region Initialization code
        private static bool available = false;

        public static bool IsAvailable() { return NVImport.available; }

        static NVImport()
        {
            DllImportAttribute attribute = new DllImportAttribute(GetDllName());
            attribute.CallingConvention = CallingConvention.Cdecl;
            attribute.PreserveSig = true;
            attribute.EntryPoint = "nvapi_QueryInterface";
            PInvokeDelegateFactory.CreateDelegate(attribute, out QueryInterface);

            try
            {
                GetDelegate(NvId_Initialize, out InitializeInternal);
            }
            catch (DllNotFoundException) { return; }
            catch (EntryPointNotFoundException) { return; }
            catch (ArgumentNullException) { return; }

            if (InitializeInternal() == NVAPI_STATUS.NVAPI_OK)
            {
                GetDelegate(NvId_Unload, out UnloadInternal);
                GetDelegate(NvId_GetInterfaceVersionString, out GetInterfaceVersionStringInternal);
                GetDelegate(NvId_GetErrorMessage, out GetErrorMessageInternal);

                // Display
                GetDelegate(NvId_EnumNvidiaDisplayHandle, out EnumNvidiaDisplayHandleInternal);
                GetDelegate(NvId_EnumNvidiaUnAttachedDisplayHandle, out EnumNvidiaUnAttachedDisplayHandleInternal);
                GetDelegate(NvId_GetAssociatedNvidiaDisplayHandle, out GetAssociatedNvidiaDisplayHandleInternal);
                GetDelegate(NvId_DISP_GetAssociatedUnAttachedNvidiaDisplayHandle, out GetAssociatedUnAttachedNvidiaDisplayHandleInternal);

                // GPUs
                GetDelegate(NvId_EnumPhysicalGPUs, out EnumPhysicalGPUsInternal);
                GetDelegate(NvId_GPU_GetQuadroStatus, out GetQuadroStatusInternal);

                // Mosaic
                GetDelegate(NvId_Mosaic_GetCurrentTopo, out Mosaic_GetCurrentTopoInternal);
                GetDelegate(NvId_Mosaic_GetTopoGroup, out Mosaic_GetTopoGroupInternal);

                // Set the availability
                available = true;
            }

            AppDomain.CurrentDomain.ProcessExit += NVImport.OnExit;
        }

        private static void GetDelegate<T>(uint id, out T newDelegate) where T : class
        {
            IntPtr ptr = QueryInterface(id);
            if (ptr != IntPtr.Zero)
            {
                newDelegate = Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)) as T;
            }
            else
            {
                newDelegate = null;
            }
        }

        private static string GetDllName()
        {
            if (IntPtr.Size == 4)
            {
                return "nvapi.dll";
            }
            else
            {
                return "nvapi64.dll";
            }
        }

        private static void OnExit(object sender, EventArgs e)
        {
            available = false;

            if (NVImport.UnloadInternal != null) { NVImport.UnloadInternal(); }
        }

        public static TResult BitWiseConvert<TResult, T>(T source)
            where TResult : struct, IConvertible
            where T : struct, IConvertible
        {
            if (typeof(T) == typeof(TResult))
            {
                return (TResult)(object)source;
            }

            var sourceSize = Marshal.SizeOf(typeof(T));
            var destinationSize = Marshal.SizeOf(typeof(TResult));
            var minSize = Math.Min(sourceSize, destinationSize);
            var sourcePointer = Marshal.AllocHGlobal(sourceSize);
            Marshal.StructureToPtr(source, sourcePointer, false);
            var bytes = new byte[destinationSize];

            if (BitConverter.IsLittleEndian)
            {
                Marshal.Copy(sourcePointer, bytes, 0, minSize);
            }
            else
            {
                Marshal.Copy(sourcePointer + (sourceSize - minSize), bytes, destinationSize - minSize, minSize);
            }

            Marshal.FreeHGlobal(sourcePointer);
            var destinationPointer = Marshal.AllocHGlobal(destinationSize);
            Marshal.Copy(bytes, 0, destinationPointer, destinationSize);
            var destination = (TResult)Marshal.PtrToStructure(destinationPointer, typeof(TResult));
            Marshal.FreeHGlobal(destinationPointer);

            return destination;
        }

        public static bool GetBit<T>(this T integer, int index) where T : struct, IConvertible
        {
            var bigInteger = BitWiseConvert<ulong, T>(integer);
            var mask = 1ul << index;

            return (bigInteger & mask) > 0;
        }

        public static ulong GetBits<T>(this T integer, int index, int count) where T : struct, IConvertible
        {
            var bigInteger = BitWiseConvert<ulong, T>(integer);

            if (index > 0)
            {
                bigInteger >>= index;
            }

            count = 64 - count;
            bigInteger <<= count;
            bigInteger >>= count;

            return bigInteger;
        }

        /*public static class Utils
        {
            public static int SizeOf<T>(T obj)
            {
                return SizeOfCache<T>.SizeOf;
            }

            private static class SizeOfCache<T>
            {
                public static readonly int SizeOf;

                static SizeOfCache()
                {
                    var dm = new DynamicMethod("func", typeof(int),
                                               Type.EmptyTypes, typeof(Utils));

                    ILGenerator il = dm.GetILGenerator();
                    il.Emit(OpCodes.Sizeof, typeof(T));
                    il.Emit(OpCodes.Ret);

                    var func = (Func<int>)dm.CreateDelegate(typeof(Func<int>));
                    SizeOf = func();
                }
            }
        }*/


        
        #endregion


        // NvAPI Functions extracted from R470 Developer nvapi64.lib using dumpbin
        // e.g. dumpbin.exe /DISASM R470-developer\amd64\nvapi64.lib
        // Can also use this script: https://raw.githubusercontent.com/terrymacdonald/NVIDIAInfo/main/NVIDIAInfo/NVAPI_Function_Location_Extractor.ps1
        // Note: This only extracts the public NvAPI Functions! The private NvAPI calls were found by Soroush Falahati.

        #region NvAPI Public Functions 

        private const uint NvId_GetErrorMessage = 0x6C2D048C;
        private const uint NvId_GetInterfaceVersionString = 0x1053FA5;
        private const uint NvId_GPU_GetEDID = 0x37D32E69;
        private const uint NvId_SetView = 0x957D7B6;
        private const uint NvId_SetViewEx = 0x6B89E68;
        private const uint NvId_GetDisplayDriverVersion = 0xF951A4D1;
        private const uint NvId_SYS_GetDriverAndBranchVersion = 0x2926AAAD;
        private const uint NvId_GPU_GetMemoryInfo = 0x7F9B368;
        private const uint NvId_OGL_ExpertModeSet = 0x3805EF7A;
        private const uint NvId_OGL_ExpertModeGet = 0x22ED9516;
        private const uint NvId_OGL_ExpertModeDefaultsSet = 0xB47A657E;
        private const uint NvId_OGL_ExpertModeDefaultsGet = 0xAE921F12;
        private const uint NvId_EnumPhysicalGPUs = 0xE5AC921F;
        private const uint NvId_EnumTCCPhysicalGPUs = 0xD9930B07;
        private const uint NvId_EnumLogicalGPUs = 0x48B3EA59;
        private const uint NvId_GetPhysicalGPUsFromDisplay = 0x34EF9506;
        private const uint NvId_GetPhysicalGPUFromUnAttachedDisplay = 0x5018ED61;
        private const uint NvId_GetLogicalGPUFromDisplay = 0xEE1370CF;
        private const uint NvId_GetLogicalGPUFromPhysicalGPU = 0xADD604D1;
        private const uint NvId_GetPhysicalGPUsFromLogicalGPU = 0xAEA3FA32;
        private const uint NvId_GPU_GetShaderSubPipeCount = 0xBE17923;
        private const uint NvId_GPU_GetGpuCoreCount = 0xC7026A87;
        private const uint NvId_GPU_GetAllOutputs = 0x7D554F8E;
        private const uint NvId_GPU_GetConnectedOutputs = 0x1730BFC9;
        private const uint NvId_GPU_GetConnectedSLIOutputs = 0x680DE09;
        private const uint NvId_GPU_GetConnectedDisplayIds = 0x78DBA2;
        private const uint NvId_GPU_GetAllDisplayIds = 0x785210A2;
        private const uint NvId_GPU_GetConnectedOutputsWithLidState = 0xCF8CAF39;
        private const uint NvId_GPU_GetConnectedSLIOutputsWithLidState = 0x96043CC7;
        private const uint NvId_GPU_GetSystemType = 0xBAAABFCC;
        private const uint NvId_GPU_GetActiveOutputs = 0xE3E89B6F;
        private const uint NvId_GPU_SetEDID = 0xE83D6456;
        private const uint NvId_GPU_GetOutputType = 0x40A505E4;
        private const uint NvId_GPU_ValidateOutputCombination = 0x34C9C2D4;
        private const uint NvId_GPU_GetFullName = 0xCEEE8E9F;
        private const uint NvId_GPU_GetPCIIdentifiers = 0x2DDFB66E;
        private const uint NvId_GPU_GetGPUType = 0xC33BAEB1;
        private const uint NvId_GPU_GetBusType = 0x1BB18724;
        private const uint NvId_GPU_GetBusId = 0x1BE0B8E5;
        private const uint NvId_GPU_GetBusSlotId = 0x2A0A350F;
        private const uint NvId_GPU_GetIRQ = 0xE4715417;
        private const uint NvId_GPU_GetVbiosRevision = 0xACC3DA0A;
        private const uint NvId_GPU_GetVbiosOEMRevision = 0x2D43FB31;
        private const uint NvId_GPU_GetVbiosVersionString = 0xA561FD7D;
        private const uint NvId_GPU_GetAGPAperture = 0x6E042794;
        private const uint NvId_GPU_GetCurrentAGPRate = 0xC74925A0;
        private const uint NvId_GPU_GetCurrentPCIEDownstreamWidth = 0xD048C3B1;
        private const uint NvId_GPU_GetPhysicalFrameBufferSize = 0x46FBEB03;
        private const uint NvId_GPU_GetVirtualFrameBufferSize = 0x5A04B644;
        private const uint NvId_GPU_GetQuadroStatus = 0xE332FA47;
        private const uint NvId_GPU_GetBoardInfo = 0x22D54523;
        private const uint NvId_GPU_GetArchInfo = 0xD8265D24;
        private const uint NvId_I2CRead = 0x2FDE12C5;
        private const uint NvId_I2CWrite = 0xE812EB07;
        private const uint NvId_GPU_WorkstationFeatureSetup = 0x6C1F3FE4;
        private const uint NvId_GPU_WorkstationFeatureQuery = 0x4537DF;
        private const uint NvId_GPU_GetHDCPSupportStatus = 0xF089EEF5;
        private const uint NvId_GPU_GetTachReading = 0x5F608315;
        private const uint NvId_GPU_GetECCStatusInfo = 0xCA1DDAF3;
        private const uint NvId_GPU_GetECCErrorInfo = 0xC71F85A6;
        private const uint NvId_GPU_ResetECCErrorInfo = 0xC02EEC20;
        private const uint NvId_GPU_GetECCConfigurationInfo = 0x77A796F3;
        private const uint NvId_GPU_SetECCConfiguration = 0x1CF639D9;
        private const uint NvId_GPU_QueryWorkstationFeatureSupport = 0x80B1ABB9;
        private const uint NvId_GPU_SetScanoutIntensity = 0xA57457A4;
        private const uint NvId_GPU_GetScanoutIntensityState = 0xE81CE836;
        private const uint NvId_GPU_SetScanoutWarping = 0xB34BAB4F;
        private const uint NvId_GPU_GetScanoutWarpingState = 0x6F5435AF;
        private const uint NvId_GPU_SetScanoutCompositionParameter = 0xF898247D;
        private const uint NvId_GPU_GetScanoutCompositionParameter = 0x58FE51E6;
        private const uint NvId_GPU_GetScanoutConfiguration = 0x6A9F5B63;
        private const uint NvId_GPU_GetScanoutConfigurationEx = 0xE2E1E6F0;
        private const uint NvId_GPU_GetAdapterIdFromPhysicalGpu = 0xFF07FDE;
        private const uint NvId_GPU_GetVirtualizationInfo = 0x44E022A9;
        private const uint NvId_GPU_GetLogicalGpuInfo = 0x842B066E;
        private const uint NvId_GPU_GetLicensableFeatures = 0x3FC596AA;
        private const uint NvId_GPU_GetVRReadyData = 0x81D629C5;
        private const uint NvId_GPU_GetPerfDecreaseInfo = 0x7F7F4600;
        private const uint NvId_GPU_GetPstatesInfoEx = 0x843C0256;
        private const uint NvId_GPU_GetPstates20 = 0x6FF81213;
        private const uint NvId_GPU_GetCurrentPstate = 0x927DA4F6;
        private const uint NvId_GPU_GetDynamicPstatesInfoEx = 0x60DED2ED;
        private const uint NvId_GPU_GetThermalSettings = 0xE3640A56;
        private const uint NvId_GPU_GetAllClockFrequencies = 0xDCB616C3;
        private const uint NvId_GPU_QueryIlluminationSupport = 0xA629DA31;
        private const uint NvId_GPU_GetIllumination = 0x9A1B9365;
        private const uint NvId_GPU_SetIllumination = 0x254A187;
        private const uint NvId_GPU_ClientIllumDevicesGetInfo = 0xD4100E58;
        private const uint NvId_GPU_ClientIllumDevicesGetControl = 0x73C01D58;
        private const uint NvId_GPU_ClientIllumDevicesSetControl = 0x57024C62;
        private const uint NvId_GPU_ClientIllumZonesGetInfo = 0x4B81241B;
        private const uint NvId_GPU_ClientIllumZonesGetControl = 0x3DBF5764;
        private const uint NvId_GPU_ClientIllumZonesSetControl = 0x197D065E;
        private const uint NvId_Event_RegisterCallback = 0xE6DBEA69;
        private const uint NvId_Event_UnregisterCallback = 0xDE1F9B45;
        private const uint NvId_EnumNvidiaDisplayHandle = 0x9ABDD40D;
        private const uint NvId_EnumNvidiaUnAttachedDisplayHandle = 0x20DE9260;
        private const uint NvId_CreateDisplayFromUnAttachedDisplay = 0x63F9799E;
        private const uint NvId_GetAssociatedNvidiaDisplayHandle = 0x35C29134;
        private const uint NvId_DISP_GetAssociatedUnAttachedNvidiaDisplayHandle = 0xA70503B2;
        private const uint NvId_GetAssociatedNvidiaDisplayName = 0x22A78B05;
        private const uint NvId_GetUnAttachedAssociatedDisplayName = 0x4888D790;
        private const uint NvId_EnableHWCursor = 0x2863148D;
        private const uint NvId_DisableHWCursor = 0xAB163097;
        private const uint NvId_GetVBlankCounter = 0x67B5DB55;
        private const uint NvId_SetRefreshRateOverride = 0x3092AC32;
        private const uint NvId_GetAssociatedDisplayOutputId = 0xD995937E;
        private const uint NvId_GetDisplayPortInfo = 0xC64FF367;
        private const uint NvId_SetDisplayPort = 0xFA13E65A;
        private const uint NvId_GetHDMISupportInfo = 0x6AE16EC3;
        private const uint NvId_Disp_InfoFrameControl = 0x6067AF3F;
        private const uint NvId_Disp_ColorControl = 0x92F9D80D;
        private const uint NvId_Disp_GetHdrCapabilities = 0x84F2A8DF;
        private const uint NvId_Disp_HdrColorControl = 0x351DA224;
        private const uint NvId_DISP_GetTiming = 0x175167E9;
        private const uint NvId_DISP_GetMonitorCapabilities = 0x3B05C7E1;
        private const uint NvId_DISP_GetMonitorColorCapabilities = 0x6AE4CFB5;
        private const uint NvId_DISP_EnumCustomDisplay = 0xA2072D59;
        private const uint NvId_DISP_TryCustomDisplay = 0x1F7DB630;
        private const uint NvId_DISP_DeleteCustomDisplay = 0x552E5B9B;
        private const uint NvId_DISP_SaveCustomDisplay = 0x49882876;
        private const uint NvId_DISP_RevertCustomDisplayTrial = 0xCBBD40F0;
        private const uint NvId_GetView = 0xD6B99D89;
        private const uint NvId_GetViewEx = 0xDBBC0AF4;
        private const uint NvId_GetSupportedViews = 0x66FB7FC0;
        private const uint NvId_DISP_GetDisplayIdByDisplayName = 0xAE457190;
        private const uint NvId_DISP_GetGDIPrimaryDisplayId = 0x1E9D8A31;
        private const uint NvId_DISP_GetDisplayConfig = 0x11ABCCF8;
        private const uint NvId_DISP_SetDisplayConfig = 0x5D8CF8DE;
        private const uint NvId_DISP_GetAdaptiveSyncData = 0xB73D1EE9;
        private const uint NvId_DISP_SetAdaptiveSyncData = 0x3EEBBA1D;
        private const uint NvId_DISP_SetPreferredStereoDisplay = 0xC9D0E25F;
        private const uint NvId_DISP_GetPreferredStereoDisplay = 0x1F6B4666;
        private const uint NvId_Mosaic_GetSupportedTopoInfo = 0xFDB63C81;
        private const uint NvId_Mosaic_GetTopoGroup = 0xCB89381D;
        private const uint NvId_Mosaic_GetOverlapLimits = 0x989685F0;
        private const uint NvId_Mosaic_SetCurrentTopo = 0x9B542831;
        private const uint NvId_Mosaic_GetCurrentTopo = 0xEC32944E;
        private const uint NvId_Mosaic_EnableCurrentTopo = 0x5F1AA66C;
        private const uint NvId_Mosaic_GetDisplayViewportsByResolution = 0xDC6DC8D3;
        private const uint NvId_Mosaic_SetDisplayGrids = 0x4D959A89;
        private const uint NvId_Mosaic_ValidateDisplayGrids = 0xCF43903D;
        private const uint NvId_Mosaic_EnumDisplayModes = 0x78DB97D7;
        private const uint NvId_Mosaic_EnumDisplayGrids = 0xDF2887AF;
        private const uint NvId_GetSupportedMosaicTopologies = 0x410B5C25;
        private const uint NvId_GetCurrentMosaicTopology = 0xF60852BD;
        private const uint NvId_SetCurrentMosaicTopology = 0xD54B8989;
        private const uint NvId_EnableCurrentMosaicTopology = 0x74073CC9;
        private const uint NvId_GSync_EnumSyncDevices = 0xD9639601;
        private const uint NvId_GSync_QueryCapabilities = 0x44A3F1D1;
        private const uint NvId_GSync_GetTopology = 0x4562BC38;
        private const uint NvId_GSync_SetSyncStateSettings = 0x60ACDFDD;
        private const uint NvId_GSync_GetControlParameters = 0x16DE1C6A;
        private const uint NvId_GSync_SetControlParameters = 0x8BBFF88B;
        private const uint NvId_GSync_AdjustSyncDelay = 0x2D11FF51;
        private const uint NvId_GSync_GetSyncStatus = 0xF1F5B434;
        private const uint NvId_GSync_GetStatusParameters = 0x70D404EC;
        private const uint NvId_D3D_GetCurrentSLIState = 0x4B708B54;
        private const uint NvId_D3D9_RegisterResource = 0xA064BDFC;
        private const uint NvId_D3D9_UnregisterResource = 0xBB2B17AA;
        private const uint NvId_D3D9_AliasSurfaceAsTexture = 0xE5CEAE41;
        private const uint NvId_D3D9_StretchRectEx = 0x22DE03AA;
        private const uint NvId_D3D9_ClearRT = 0x332D3942;
        private const uint NvId_D3D_GetObjectHandleForResource = 0xFCEAC864;
        private const uint NvId_D3D_SetResourceHint = 0x6C0ED98C;
        private const uint NvId_D3D_BeginResourceRendering = 0x91123D6A;
        private const uint NvId_D3D_EndResourceRendering = 0x37E7191C;
        private const uint NvId_D3D9_GetSurfaceHandle = 0xF2DD3F2;
        private const uint NvId_D3D9_VideoSetStereoInfo = 0xB852F4DB;
        private const uint NvId_D3D10_SetDepthBoundsTest = 0x4EADF5D2;
        private const uint NvId_D3D11_CreateDevice = 0x6A16D3A0;
        private const uint NvId_D3D11_CreateDeviceAndSwapChain = 0xBB939EE5;
        private const uint NvId_D3D11_SetDepthBoundsTest = 0x7AAF7A04;
        private const uint NvId_D3D11_IsNvShaderExtnOpCodeSupported = 0x5F68DA40;
        private const uint NvId_D3D11_SetNvShaderExtnSlot = 0x8E90BB9F;
        private const uint NvId_D3D12_SetNvShaderExtnSlotSpace = 0xAC2DFEB5;
        private const uint NvId_D3D12_SetNvShaderExtnSlotSpaceLocalThread = 0x43D867C0;
        private const uint NvId_D3D11_SetNvShaderExtnSlotLocalThread = 0xE6482A0;
        private const uint NvId_D3D11_BeginUAVOverlapEx = 0xBA08208A;
        private const uint NvId_D3D11_BeginUAVOverlap = 0x65B93CA8;
        private const uint NvId_D3D11_EndUAVOverlap = 0x2216A357;
        private const uint NvId_D3D11_GetResourceHandle = 0x9D52986;
        private const uint NvId_D3D_SetFPSIndicatorState = 0xA776E8DB;
        private const uint NvId_D3D9_Present = 0x5650BEB;
        private const uint NvId_D3D9_QueryFrameCount = 0x9083E53A;
        private const uint NvId_D3D9_ResetFrameCount = 0xFA6A0675;
        private const uint NvId_D3D9_QueryMaxSwapGroup = 0x5995410D;
        private const uint NvId_D3D9_QuerySwapGroup = 0xEBA4D232;
        private const uint NvId_D3D9_JoinSwapGroup = 0x7D44BB54;
        private const uint NvId_D3D9_BindSwapBarrier = 0x9C39C246;
        private const uint NvId_D3D1x_Present = 0x3B845A1;
        private const uint NvId_D3D1x_QueryFrameCount = 0x9152E055;
        private const uint NvId_D3D1x_ResetFrameCount = 0xFBBB031A;
        private const uint NvId_D3D1x_QueryMaxSwapGroup = 0x9BB9D68F;
        private const uint NvId_D3D1x_QuerySwapGroup = 0x407F67AA;
        private const uint NvId_D3D1x_JoinSwapGroup = 0x14610CD7;
        private const uint NvId_D3D1x_BindSwapBarrier = 0x9DE8C729;
        private const uint NvId_D3D12_QueryPresentBarrierSupport = 0xA15FAEF7;
        private const uint NvId_D3D12_CreatePresentBarrierClient = 0x4D815DE9;
        private const uint NvId_D3D12_RegisterPresentBarrierResources = 0xD53C9EF0;
        private const uint NvId_DestroyPresentBarrierClient = 0x3C5C351B;
        private const uint NvId_JoinPresentBarrier = 0x17F6BF82;
        private const uint NvId_LeavePresentBarrier = 0xC3EC5A7F;
        private const uint NvId_QueryPresentBarrierFrameStatistics = 0x61B844A1;
        private const uint NvId_D3D11_CreateRasterizerState = 0xDB8D28AF;
        private const uint NvId_D3D_ConfigureAnsel = 0x341C6C7F;
        private const uint NvId_D3D11_CreateTiledTexture2DArray = 0x7886981A;
        private const uint NvId_D3D11_CheckFeatureSupport = 0x106A487E;
        private const uint NvId_D3D11_CreateImplicitMSAATexture2D = 0xB8F79632;
        private const uint NvId_D3D12_CreateCommittedImplicitMSAATexture2D = 0x24C6A07B;
        private const uint NvId_D3D11_ResolveSubresourceRegion = 0xE6BFEDD6;
        private const uint NvId_D3D12_ResolveSubresourceRegion = 0xC24A15BF;
        private const uint NvId_D3D11_TiledTexture2DArrayGetDesc = 0xF1A2B9D5;
        private const uint NvId_D3D11_UpdateTileMappings = 0x9A06EA07;
        private const uint NvId_D3D11_CopyTileMappings = 0xC09EE6BC;
        private const uint NvId_D3D11_TiledResourceBarrier = 0xD6839099;
        private const uint NvId_D3D11_AliasMSAATexture2DAsNonMSAA = 0xF1C54FC9;
        private const uint NvId_D3D11_CreateGeometryShaderEx_2 = 0x99ED5C1C;
        private const uint NvId_D3D11_CreateVertexShaderEx = 0xBEAA0B2;
        private const uint NvId_D3D11_CreateHullShaderEx = 0xB53CAB00;
        private const uint NvId_D3D11_CreateDomainShaderEx = 0xA0D7180D;
        private const uint NvId_D3D11_CreatePixelShaderEx_2 = 0x4162822B;
        private const uint NvId_D3D11_CreateFastGeometryShaderExplicit = 0x71AB7C9C;
        private const uint NvId_D3D11_CreateFastGeometryShader = 0x525D43BE;
        private const uint NvId_D3D11_DecompressView = 0x3A94E822;
        private const uint NvId_D3D12_CreateGraphicsPipelineState = 0x2FC28856;
        private const uint NvId_D3D12_CreateComputePipelineState = 0x2762DEAC;
        private const uint NvId_D3D12_SetDepthBoundsTestValues = 0xB9333FE9;
        private const uint NvId_D3D12_CreateReservedResource = 0x2C85F101;
        private const uint NvId_D3D12_CreateHeap = 0x5CB397CF;
        private const uint NvId_D3D12_CreateHeap2 = 0x924BE9D6;
        private const uint NvId_D3D12_QueryCpuVisibleVidmem = 0x26322BC3;
        private const uint NvId_D3D12_ReservedResourceGetDesc = 0x9AA2AABB;
        private const uint NvId_D3D12_UpdateTileMappings = 0xC6017A7D;
        private const uint NvId_D3D12_CopyTileMappings = 0x47F78194;
        private const uint NvId_D3D12_ResourceAliasingBarrier = 0xB942BAB7;
        private const uint NvId_D3D12_CaptureUAVInfo = 0x6E5EA9DB;
        private const uint NvId_D3D11_GetResourceGPUVirtualAddressEx = 0xAF6D14DA;
        private const uint NvId_D3D11_EnumerateMetaCommands = 0xC7453BA8;
        private const uint NvId_D3D11_CreateMetaCommand = 0xF505FBA0;
        private const uint NvId_D3D11_InitializeMetaCommand = 0xAEC629E9;
        private const uint NvId_D3D11_ExecuteMetaCommand = 0x82236C47;
        private const uint NvId_D3D12_EnumerateMetaCommands = 0xCD9141D8;
        private const uint NvId_D3D12_CreateMetaCommand = 0xEB29634B;
        private const uint NvId_D3D12_InitializeMetaCommand = 0xA4125399;
        private const uint NvId_D3D12_ExecuteMetaCommand = 0xDE24FC3D;
        private const uint NvId_D3D12_CreateCommittedResource = 0x27E98AE;
        private const uint NvId_D3D12_GetCopyableFootprints = 0xF6305EB5;
        private const uint NvId_D3D12_CopyTextureRegion = 0x82B91B25;
        private const uint NvId_D3D12_IsNvShaderExtnOpCodeSupported = 0x3DFACEC8;
        private const uint NvId_D3D_IsGSyncCapable = 0x9C1EED78;
        private const uint NvId_D3D_IsGSyncActive = 0xE942B0FF;
        private const uint NvId_D3D1x_DisableShaderDiskCache = 0xD0CBCA7D;
        private const uint NvId_D3D11_MultiGPU_GetCaps = 0xD2D25687;
        private const uint NvId_D3D11_MultiGPU_Init = 0x17BE49E;
        private const uint NvId_D3D11_CreateMultiGPUDevice = 0xBDB20007;
        private const uint NvId_D3D_QuerySinglePassStereoSupport = 0x6F5F0A6D;
        private const uint NvId_D3D_SetSinglePassStereoMode = 0xA39E6E6E;
        private const uint NvId_D3D12_QuerySinglePassStereoSupport = 0x3B03791B;
        private const uint NvId_D3D12_SetSinglePassStereoMode = 0x83556D87;
        private const uint NvId_D3D_QueryMultiViewSupport = 0xB6E0A41C;
        private const uint NvId_D3D_SetMultiViewMode = 0x8285C8DA;
        private const uint NvId_D3D_QueryModifiedWSupport = 0xCBF9F4F5;
        private const uint NvId_D3D_SetModifiedWMode = 0x6EA4BF4;
        private const uint NvId_D3D12_QueryModifiedWSupport = 0x51235248;
        private const uint NvId_D3D12_SetModifiedWMode = 0xE1FDABA7;
        private const uint NvId_D3D_CreateLateLatchObject = 0x2DB27D09;
        private const uint NvId_D3D_QueryLateLatchSupport = 0x8CECA0EC;
        private const uint NvId_D3D_RegisterDevice = 0x8C02C4D0;
        private const uint NvId_D3D11_MultiDrawInstancedIndirect = 0xD4E26BBF;
        private const uint NvId_D3D11_MultiDrawIndexedInstancedIndirect = 0x59E890F9;
        private const uint NvId_D3D_ImplicitSLIControl = 0x2AEDE111;
        private const uint NvId_D3D12_UseDriverHeapPriorities = 0xF0D978A8;
        private const uint NvId_D3D12_Mosaic_GetCompanionAllocations = 0xA46022C7;
        private const uint NvId_D3D12_Mosaic_GetViewportAndGpuPartitions = 0xB092B818;
        private const uint NvId_D3D1x_GetGraphicsCapabilities = 0x52B1499A;
        private const uint NvId_D3D12_GetGraphicsCapabilities = 0x1E87354;
        private const uint NvId_D3D11_RSSetExclusiveScissorRects = 0xAE4D73EF;
        private const uint NvId_D3D11_RSSetViewportsPixelShadingRates = 0x34F7938F;
        private const uint NvId_D3D11_CreateShadingRateResourceView = 0x99CA2DFF;
        private const uint NvId_D3D11_RSSetShadingRateResourceView = 0x1B0C2F83;
        private const uint NvId_D3D11_RSGetPixelShadingRateSampleOrder = 0x92442A1;
        private const uint NvId_D3D11_RSSetPixelShadingRateSampleOrder = 0xA942373A;
        private const uint NvId_D3D_InitializeVRSHelper = 0x4780D70B;
        private const uint NvId_D3D_InitializeNvGazeHandler = 0x5B3B7479;
        private const uint NvId_D3D_InitializeSMPAssist = 0x42763D0C;
        private const uint NvId_D3D_QuerySMPAssistSupport = 0xC57921DE;
        private const uint NvId_D3D_GetSleepStatus = 0xAEF96CA1;
        private const uint NvId_D3D_SetSleepMode = 0xAC1CA9E0;
        private const uint NvId_D3D_Sleep = 0x852CD1D2;
        private const uint NvId_D3D_GetLatency = 0x1A587F9C;
        private const uint NvId_D3D_SetLatencyMarker = 0xD9984C05;
        private const uint NvId_D3D12_CreateCubinComputeShader = 0x2A2C79E8;
        private const uint NvId_D3D12_CreateCubinComputeShaderEx = 0x3151211B;
        private const uint NvId_D3D12_CreateCubinComputeShaderWithName = 0x1DC7261F;
        private const uint NvId_D3D12_LaunchCubinShader = 0x5C52BB86;
        private const uint NvId_D3D12_DestroyCubinComputeShader = 0x7FB785BA;
        private const uint NvId_D3D12_GetCudaTextureObject = 0x80403FC9;
        private const uint NvId_D3D12_GetCudaSurfaceObject = 0x48F5B2EE;
        private const uint NvId_D3D12_IsFatbinPTXSupported = 0x70C07832;
        private const uint NvId_D3D11_CreateCubinComputeShader = 0xED98181;
        private const uint NvId_D3D11_CreateCubinComputeShaderEx = 0x32C2A0F6;
        private const uint NvId_D3D11_CreateCubinComputeShaderWithName = 0xB672BE19;
        private const uint NvId_D3D11_LaunchCubinShader = 0x427E236D;
        private const uint NvId_D3D11_DestroyCubinComputeShader = 0x1682C86;
        private const uint NvId_D3D11_IsFatbinPTXSupported = 0x6086BD93;
        private const uint NvId_D3D11_CreateUnorderedAccessView = 0x74A497A1;
        private const uint NvId_D3D11_CreateShaderResourceView = 0x65CB431E;
        private const uint NvId_D3D11_CreateSamplerState = 0x89ECA416;
        private const uint NvId_D3D11_GetCudaTextureObject = 0x9006FA68;
        private const uint NvId_D3D11_GetResourceGPUVirtualAddress = 0x1819B423;
        private const uint NvId_VIO_GetCapabilities = 0x1DC91303;
        private const uint NvId_VIO_Open = 0x44EE4841;
        private const uint NvId_VIO_Close = 0xD01BD237;
        private const uint NvId_VIO_Status = 0xE6CE4F1;
        private const uint NvId_VIO_SyncFormatDetect = 0x118D48A3;
        private const uint NvId_VIO_GetConfig = 0xD34A789B;
        private const uint NvId_VIO_SetConfig = 0xE4EEC07;
        private const uint NvId_VIO_SetCSC = 0xA1EC8D74;
        private const uint NvId_VIO_GetCSC = 0x7B0D72A3;
        private const uint NvId_VIO_SetGamma = 0x964BF452;
        private const uint NvId_VIO_GetGamma = 0x51D53D06;
        private const uint NvId_VIO_SetSyncDelay = 0x2697A8D1;
        private const uint NvId_VIO_GetSyncDelay = 0x462214A9;
        private const uint NvId_VIO_GetPCIInfo = 0xB981D935;
        private const uint NvId_VIO_IsRunning = 0x96BD040E;
        private const uint NvId_VIO_Start = 0xCDE8E1A3;
        private const uint NvId_VIO_Stop = 0x6BA2A5D6;
        private const uint NvId_VIO_IsFrameLockModeCompatible = 0x7BF0A94D;
        private const uint NvId_VIO_EnumDevices = 0xFD7C5557;
        private const uint NvId_VIO_QueryTopology = 0x869534E2;
        private const uint NvId_VIO_EnumSignalFormats = 0xEAD72FE4;
        private const uint NvId_VIO_EnumDataFormats = 0x221FA8E8;
        private const uint NvId_Stereo_CreateConfigurationProfileRegistryKey = 0xBE7692EC;
        private const uint NvId_Stereo_DeleteConfigurationProfileRegistryKey = 0xF117B834;
        private const uint NvId_Stereo_SetConfigurationProfileValue = 0x24409F48;
        private const uint NvId_Stereo_DeleteConfigurationProfileValue = 0x49BCEECF;
        private const uint NvId_Stereo_Enable = 0x239C4545;
        private const uint NvId_Stereo_Disable = 0x2EC50C2B;
        private const uint NvId_Stereo_IsEnabled = 0x348FF8E1;
        private const uint NvId_Stereo_GetStereoSupport = 0x296C434D;
        private const uint NvId_Stereo_CreateHandleFromIUnknown = 0xAC7E37F4;
        private const uint NvId_Stereo_DestroyHandle = 0x3A153134;
        private const uint NvId_Stereo_Activate = 0xF6A1AD68;
        private const uint NvId_Stereo_Deactivate = 0x2D68DE96;
        private const uint NvId_Stereo_IsActivated = 0x1FB0BC30;
        private const uint NvId_Stereo_GetSeparation = 0x451F2134;
        private const uint NvId_Stereo_SetSeparation = 0x5C069FA3;
        private const uint NvId_Stereo_DecreaseSeparation = 0xDA044458;
        private const uint NvId_Stereo_IncreaseSeparation = 0xC9A8ECEC;
        private const uint NvId_Stereo_GetConvergence = 0x4AB00934;
        private const uint NvId_Stereo_SetConvergence = 0x3DD6B54B;
        private const uint NvId_Stereo_DecreaseConvergence = 0x4C87E317;
        private const uint NvId_Stereo_IncreaseConvergence = 0xA17DAABE;
        private const uint NvId_Stereo_GetFrustumAdjustMode = 0xE6839B43;
        private const uint NvId_Stereo_SetFrustumAdjustMode = 0x7BE27FA2;
        private const uint NvId_Stereo_CaptureJpegImage = 0x932CB140;
        private const uint NvId_Stereo_InitActivation = 0xC7177702;
        private const uint NvId_Stereo_Trigger_Activation = 0xD6C6CD2;
        private const uint NvId_Stereo_CapturePngImage = 0x8B7E99B5;
        private const uint NvId_Stereo_ReverseStereoBlitControl = 0x3CD58F89;
        private const uint NvId_Stereo_SetNotificationMessage = 0x6B9B409E;
        private const uint NvId_Stereo_SetActiveEye = 0x96EEA9F8;
        private const uint NvId_Stereo_SetDriverMode = 0x5E8F0BEC;
        private const uint NvId_Stereo_GetEyeSeparation = 0xCE653127;
        private const uint NvId_Stereo_IsWindowedModeSupported = 0x40C8ED5E;
        private const uint NvId_Stereo_SetSurfaceCreationMode = 0xF5DCFCBA;
        private const uint NvId_Stereo_GetSurfaceCreationMode = 0x36F1C736;
        private const uint NvId_Stereo_Debug_WasLastDrawStereoized = 0xED4416C5;
        private const uint NvId_Stereo_SetDefaultProfile = 0x44F0ECD1;
        private const uint NvId_Stereo_GetDefaultProfile = 0x624E21C2;
        private const uint NvId_D3D1x_CreateSwapChain = 0x1BC21B66;
        private const uint NvId_D3D9_CreateSwapChain = 0x1A131E09;
        private const uint NvId_DRS_CreateSession = 0x694D52E;
        private const uint NvId_DRS_DestroySession = 0xDAD9CFF8;
        private const uint NvId_DRS_LoadSettings = 0x375DBD6B;
        private const uint NvId_DRS_SaveSettings = 0xFCBC7E14;
        private const uint NvId_DRS_LoadSettingsFromFile = 0xD3EDE889;
        private const uint NvId_DRS_SaveSettingsToFile = 0x2BE25DF8;
        private const uint NvId_DRS_CreateProfile = 0xCC176068;
        private const uint NvId_DRS_DeleteProfile = 0x17093206;
        private const uint NvId_DRS_SetCurrentGlobalProfile = 0x1C89C5DF;
        private const uint NvId_DRS_GetCurrentGlobalProfile = 0x617BFF9F;
        private const uint NvId_DRS_GetProfileInfo = 0x61CD6FD6;
        private const uint NvId_DRS_SetProfileInfo = 0x16ABD3A9;
        private const uint NvId_DRS_FindProfileByName = 0x7E4A9A0B;
        private const uint NvId_DRS_EnumProfiles = 0xBC371EE0;
        private const uint NvId_DRS_GetNumProfiles = 0x1DAE4FBC;
        private const uint NvId_DRS_CreateApplication = 0x4347A9DE;
        private const uint NvId_DRS_DeleteApplicationEx = 0xC5EA85A1;
        private const uint NvId_DRS_DeleteApplication = 0x2C694BC6;
        private const uint NvId_DRS_GetApplicationInfo = 0xED1F8C69;
        private const uint NvId_DRS_EnumApplications = 0x7FA2173A;
        private const uint NvId_DRS_FindApplicationByName = 0xEEE566B2;
        private const uint NvId_DRS_SetSetting = 0x577DD202;
        private const uint NvId_DRS_GetSetting = 0x73BF8338;
        private const uint NvId_DRS_EnumSettings = 0xAE3039DA;
        private const uint NvId_DRS_EnumAvailableSettingIds = 0xF020614A;
        private const uint NvId_DRS_EnumAvailableSettingValues = 0x2EC39F90;
        private const uint NvId_DRS_GetSettingIdFromName = 0xCB7309CD;
        private const uint NvId_DRS_GetSettingNameFromId = 0xD61CBE6E;
        private const uint NvId_DRS_DeleteProfileSetting = 0xE4A26362;
        private const uint NvId_DRS_RestoreAllDefaults = 0x5927B094;
        private const uint NvId_DRS_RestoreProfileDefault = 0xFA5F6134;
        private const uint NvId_DRS_RestoreProfileDefaultSetting = 0x53F0381E;
        private const uint NvId_DRS_GetBaseProfile = 0xDA8466A0;
        private const uint NvId_SYS_GetChipSetInfo = 0x53DABBCA;
        private const uint NvId_SYS_GetLidAndDockInfo = 0xCDA14D8A;
        private const uint NvId_SYS_GetDisplayIdFromGpuAndOutputId = 0x8F2BAB4;
        private const uint NvId_SYS_GetGpuAndOutputIdFromDisplayId = 0x112BA1A5;
        private const uint NvId_SYS_GetPhysicalGpuFromDisplayId = 0x9EA74659;
        private const uint NvId_SYS_GetDisplayDriverInfo = 0x721FACEB;
        private const uint NvId_GPU_ClientRegisterForUtilizationSampleUpdates = 0xADEEAF67;
        private const uint NvId_Unload = 0xD7C61344;
        
        # endregion 

        #region Private Internal NvAPI Functions

        private const uint NvId_3D_GetProperty = 0x8061A4B1;
        private const uint NvId_3D_GetPropertyRange = 0x0B85DE27C;
        private const uint NvId_3D_SetProperty = 0x0C9175E8D;
        private const uint NvId_AccessDisplayDriverRegistry = 0xF5579360;
        private const uint NvId_Coproc_GetApplicationCoprocInfo = 0x79232685;
        private const uint NvId_Coproc_GetCoprocInfoFlagsEx = 0x69A9874D;
        private const uint NvId_Coproc_GetCoprocStatus = 0x1EFC3957;
        private const uint NvId_Coproc_NotifyCoprocPowerState = 0x0CADCB956;
        private const uint NvId_Coproc_SetCoprocInfoFlagsEx = 0x0F4C863AC;
        private const uint NvId_CreateUnAttachedDisplayFromDisplay = 0xA0C72EE4;
        private const uint NvId_D3D_CreateQuery = 0x5D19BCA4;
        private const uint NvId_D3D_DestroyQuery = 0x0C8FF7258;
        private const uint NvId_D3D_Query_Begin = 0x0E5A9AAE0;
        private const uint NvId_D3D_Query_End = 0x2AC084FA;
        private const uint NvId_D3D_Query_GetData = 0x0F8B53C69;
        private const uint NvId_D3D_Query_GetDataSize = 0x0F2A54796;
        private const uint NvId_D3D_Query_GetType = 0x4ACEEAF7;
        private const uint NvId_D3D_RegisterApp = 0x0D44D3C4E;
        private const uint NvId_D3D10_AliasPrimaryAsTexture = 0x8AAC133D;
        private const uint NvId_D3D10_BeginShareResource = 0x35233210;
        private const uint NvId_D3D10_BeginShareResourceEx = 0x0EF303A9D;
        private const uint NvId_D3D10_CreateDevice = 0x2DE11D61;
        private const uint NvId_D3D10_CreateDeviceAndSwapChain = 0x5B803DAF;
        private const uint NvId_D3D10_EndShareResource = 0x0E9C5853;
        private const uint NvId_D3D10_GetRenderedCursorAsBitmap = 0x0CAC3CE5D;
        private const uint NvId_D3D10_ProcessCallbacks = 0x0AE9C2019;
        private const uint NvId_D3D10_SetPrimaryFlipChainCallbacks = 0x73EB9329;
        private const uint NvId_D3D11_BeginShareResource = 0x121BDC6;
        private const uint NvId_D3D11_EndShareResource = 0x8FFB8E26;
        private const uint NvId_D3D1x_IFR_SetUpTargetBufferToSys = 0x473F7828;
        private const uint NvId_D3D1x_IFR_TransferRenderTarget = 0x9FBAE4EB;
        private const uint NvId_D3D9_AliasPrimaryAsTexture = 0x13C7112E;
        private const uint NvId_D3D9_AliasPrimaryFromDevice = 0x7C20C5BE;
        private const uint NvId_D3D9_CreatePathContextNV = 0x0A342F682;
        private const uint NvId_D3D9_CreatePathNV = 0x71329DF3;
        private const uint NvId_D3D9_CreateRenderTarget = 0x0B3827C8;
        private const uint NvId_D3D9_CreateTexture = 0x0D5E13573;
        private const uint NvId_D3D9_CreateVideo = 0x89FFD9A3;
        private const uint NvId_D3D9_CreateVideoBegin = 0x84C9D553;
        private const uint NvId_D3D9_CreateVideoEnd = 0x0B476BF61;
        private const uint NvId_D3D9_DeletePathNV = 0x73E0019A;
        private const uint NvId_D3D9_DestroyPathContextNV = 0x667C2929;
        private const uint NvId_D3D9_DMA = 0x962B8AF6;
        private const uint NvId_D3D9_DrawPathNV = 0x13199B3D;
        private const uint NvId_D3D9_EnableStereo = 0x492A6954;
        private const uint NvId_D3D9_EnumVideoFeatures = 0x1DB7C52C;
        private const uint NvId_D3D9_FreeVideo = 0x3111BED1;
        private const uint NvId_D3D9_GetCurrentRenderTargetHandle = 0x22CAD61;
        private const uint NvId_D3D9_GetCurrentZBufferHandle = 0x0B380F218;
        private const uint NvId_D3D9_GetIndexBufferHandle = 0x0FC5A155B;
        private const uint NvId_D3D9_GetOverlaySurfaceHandles = 0x6800F5FC;
        private const uint NvId_D3D9_GetSLIInfo = 0x694BFF4D;
        private const uint NvId_D3D9_GetTextureHandle = 0x0C7985ED5;
        private const uint NvId_D3D9_GetVertexBufferHandle = 0x72B19155;
        private const uint NvId_D3D9_GetVideoCapabilities = 0x3D596B93;
        private const uint NvId_D3D9_GetVideoState = 0x0A4527BF8;
        private const uint NvId_D3D9_GPUBasedCPUSleep = 0x0D504DDA7;
        private const uint NvId_D3D9_GpuSyncAcquire = 0x0D00B8317;
        private const uint NvId_D3D9_GpuSyncEnd = 0x754033F0;
        private const uint NvId_D3D9_GpuSyncGetHandleSize = 0x80C9FD3B;
        private const uint NvId_D3D9_GpuSyncInit = 0x6D6FDAD4;
        private const uint NvId_D3D9_GpuSyncMapIndexBuffer = 0x12EE68F2;
        private const uint NvId_D3D9_GpuSyncMapSurfaceBuffer = 0x2AB714AB;
        private const uint NvId_D3D9_GpuSyncMapTexBuffer = 0x0CDE4A28A;
        private const uint NvId_D3D9_GpuSyncMapVertexBuffer = 0x0DBC803EC;
        private const uint NvId_D3D9_GpuSyncRelease = 0x3D7A86BB;
        private const uint NvId_D3D9_IFR_SetUpTargetBufferToNV12BLVideoSurface = 0x0CFC92C15;
        private const uint NvId_D3D9_IFR_SetUpTargetBufferToSys = 0x55255D05;
        private const uint NvId_D3D9_IFR_TransferRenderTarget = 0x0AB7C2DC;
        private const uint NvId_D3D9_IFR_TransferRenderTargetToNV12BLVideoSurface = 0x5FE72F64;
        private const uint NvId_D3D9_Lock = 0x6317345C;
        private const uint NvId_D3D9_NVFBC_GetStatus = 0x0bd3eb475;
        private const uint NvId_D3D9_PathClearDepthNV = 0x157E45C4;
        private const uint NvId_D3D9_PathDepthNV = 0x0FCB16330;
        private const uint NvId_D3D9_PathEnableColorWriteNV = 0x3E2804A2;
        private const uint NvId_D3D9_PathEnableDepthTestNV = 0x0E99BA7F3;
        private const uint NvId_D3D9_PathMatrixNV = 0x0D2F6C499;
        private const uint NvId_D3D9_PathParameterfNV = 0x0F7FF00C1;
        private const uint NvId_D3D9_PathParameteriNV = 0x0FC31236C;
        private const uint NvId_D3D9_PathVerticesNV = 0x0C23DF926;
        private const uint NvId_D3D9_PresentSurfaceToDesktop = 0x0F7029C5;
        private const uint NvId_D3D9_PresentVideo = 0x5CF7F862;
        private const uint NvId_D3D9_QueryAAOverrideMode = 0x0DDF5643C;
        private const uint NvId_D3D9_QueryVideoInfo = 0x1E6634B3;
        private const uint NvId_D3D9_SetGamutData = 0x2BBDA32E;
        private const uint NvId_D3D9_SetPitchSurfaceCreation = 0x18CDF365;
        private const uint NvId_D3D9_SetResourceHint = 0x905F5C27;
        private const uint NvId_D3D9_SetSLIMode = 0x0BFDC062C;
        private const uint NvId_D3D9_SetSurfaceCreationLayout = 0x5609B86A;
        private const uint NvId_D3D9_SetVideoState = 0x0BD4BC56F;
        private const uint NvId_D3D9_StretchRect = 0x0AEAECD41;
        private const uint NvId_D3D9_Unlock = 0x0C182027E;
        private const uint NvId_D3D9_VideoSurfaceEncryptionControl = 0x9D2509EF;
        private const uint NvId_DeleteCustomDisplay = 0x0E7CB998D;
        private const uint NvId_DeleteUnderscanConfig = 0x0F98854C8;
        private const uint NvId_Disp_DpAuxChannelControl = 0x8EB56969;
        private const uint NvId_DISP_EnumHDMIStereoModes = 0x0D2CCF5D6;
        private const uint NvId_DISP_GetDisplayBlankingState = 0x63E5D8DB;
        private const uint NvId_DISP_GetHCloneTopology = 0x47BAD137;
        private const uint NvId_DISP_GetVirtualModeData = 0x3230D69A;
        private const uint NvId_DISP_OverrideDisplayModeList = 0x291BFF2;
        private const uint NvId_DISP_SetDisplayBlankingState = 0x1E17E29B;
        private const uint NvId_DISP_SetHCloneTopology = 0x61041C24;
        private const uint NvId_DISP_ValidateHCloneTopology = 0x5F4C2664;
        private const uint NvId_EnumCustomDisplay = 0x42892957;
        private const uint NvId_EnumUnderscanConfig = 0x4144111A;
        private const uint NvId_GetDisplayDriverBuildTitle = 0x7562E947;
        private const uint NvId_GetDisplayDriverCompileType = 0x988AEA78;
        private const uint NvId_GetDisplayDriverMemoryInfo = 0x774AA982;
        private const uint NvId_GetDisplayDriverRegistryPath = 0x0E24CEEE;
        private const uint NvId_GetDisplayDriverSecurityLevel = 0x9D772BBA;
        private const uint NvId_GetDisplayFeatureConfig = 0x8E985CCD;
        private const uint NvId_GetDisplayFeatureConfigDefaults = 0x0F5F4D01;
        private const uint NvId_GetDisplayPosition = 0x6BB1EE5D;
        private const uint NvId_GetDisplaySettings = 0x0DC27D5D4;
        private const uint NvId_GetDriverMemoryInfo = 0x2DC95125;
        private const uint NvId_GetDriverModel = 0x25EEB2C4;
        private const uint NvId_GetDVCInfo = 0x4085DE45;
        private const uint NvId_GetDVCInfoEx = 0x0E45002D;
        private const uint NvId_GetGPUIDfromPhysicalGPU = 0x6533EA3E;
        private const uint NvId_GetHDCPLinkParameters = 0x0B3BB0772;
        private const uint NvId_GetHUEInfo = 0x95B64341;
        private const uint NvId_GetHybridMode = 0x0E23B68C1;
        private const uint NvId_GetImageSharpeningInfo = 0x9FB063DF;
        private const uint NvId_GetInfoFrame = 0x9734F1D;
        private const uint NvId_GetInfoFrameState = 0x41511594;
        private const uint NvId_GetInfoFrameStatePvt = 0x7FC17574;
        private const uint NvId_GetInvalidGpuTopologies = 0x15658BE6;
        private const uint NvId_GetLoadedMicrocodePrograms = 0x919B3136;
        private const uint NvId_GetPhysicalGPUFromDisplay = 0x1890E8DA;
        private const uint NvId_GetPhysicalGPUFromGPUID = 0x5380AD1A;
        private const uint NvId_GetPVExtName = 0x2F5B08E0;
        private const uint NvId_GetPVExtProfile = 0x1B1B9A16;
        private const uint NvId_GetScalingCaps = 0x8E875CF9;
        private const uint NvId_GetTiming = 0x0AFC4833E;
        private const uint NvId_GetTopologyDisplayGPU = 0x813D89A8;
        private const uint NvId_GetTVEncoderControls = 0x5757474A;
        private const uint NvId_GetTVOutputBorderColor = 0x6DFD1C8C;
        private const uint NvId_GetTVOutputInfo = 0x30C805D5;
        private const uint NvId_GetUnAttachedDisplayDriverRegistryPath = 0x633252D8;
        private const uint NvId_GetValidGpuTopologies = 0x5DFAB48A;
        private const uint NvId_GetVideoState = 0x1C5659CD;
        private const uint NvId_GPS_GetPerfSensors = 0x271C1109;
        private const uint NvId_GPS_GetPowerSteeringStatus = 0x540EE82E;
        private const uint NvId_GPS_GetThermalLimit = 0x583113ED;
        private const uint NvId_GPS_GetVPStateCap = 0x71913023;
        private const uint NvId_GPS_SetPowerSteeringStatus = 0x9723D3A2;
        private const uint NvId_GPS_SetThermalLimit = 0x0C07E210F;
        private const uint NvId_GPS_SetVPStateCap = 0x68888EB4;
        private const uint NvId_GPU_ClearPCIELinkAERInfo = 0x521566BB;
        private const uint NvId_GPU_ClearPCIELinkErrorInfo = 0x8456FF3D;
        private const uint NvId_GPU_ClientPowerPoliciesGetInfo = 0x34206D86;
        private const uint NvId_GPU_ClientPowerPoliciesGetStatus = 0x70916171;
        private const uint NvId_GPU_ClientPowerPoliciesSetStatus = 0x0AD95F5ED;
        private const uint NvId_GPU_ClientPowerTopologyGetInfo = 0x0A4DFD3F2;
        private const uint NvId_GPU_ClientPowerTopologyGetStatus = 0x0EDCF624E;
        private const uint NvId_GPU_CudaEnumComputeCapableGpus = 0x5786CC6E;
        private const uint NvId_GPU_EnableDynamicPstates = 0x0FA579A0F;
        private const uint NvId_GPU_EnableOverclockedPstates = 0x0B23B70EE;
        private const uint NvId_GPU_Get_DisplayPort_DongleInfo = 0x76A70E8D;
        private const uint NvId_GPU_GetAllClocks = 0x1BD69F49;
        private const uint NvId_GPU_GetAllGpusOnSameBoard = 0x4DB019E6;
        private const uint NvId_GPU_GetBarInfo = 0xE4B701E3;
        private const uint NvId_GPU_GetClockBoostLock = 0xe440b867; // unknown name; NVAPI_ID_CURVE_GET
        private const uint NvId_GPU_GetClockBoostMask = 0x507b4b59;
        private const uint NvId_GPU_GetClockBoostRanges = 0x64b43a6a;
        private const uint NvId_GPU_GetClockBoostTable = 0x23f1b133;
        private const uint NvId_GPU_GetColorSpaceConversion = 0x8159E87A;
        private const uint NvId_GPU_GetConnectorInfo = 0x4ECA2C10;
        private const uint NvId_GPU_GetCoolerPolicyTable = 0x518A32C;
        private const uint NvId_GPU_GetCoolerSettings = 0x0DA141340;
        private const uint NvId_GPU_GetCoreVoltageBoostPercent = 0x9df23ca1;
        private const uint NvId_GPU_GetCurrentFanSpeedLevel = 0x0BD71F0C9;
        private const uint NvId_GPU_GetCurrentThermalLevel = 0x0D2488B79;
        private const uint NvId_GPU_GetCurrentVoltage = 0x465f9bcf;
        private const uint NvId_GPU_GetDeepIdleState = 0x1AAD16B4;
        private const uint NvId_GPU_GetDeviceDisplayMode = 0x0D2277E3A;
        private const uint NvId_GPU_GetDisplayUnderflowStatus = 0xED9E8057;
        private const uint NvId_GPU_GetDitherControl = 0x932AC8FB;
        private const uint NvId_GPU_GetExtendedMinorRevision = 0x25F17421;
        private const uint NvId_GPU_GetFBWidthAndLocation = 0x11104158;
        private const uint NvId_GPU_GetFlatPanelInfo = 0x36CFF969;
        private const uint NvId_GPU_GetFoundry = 0x5D857A00;
        private const uint NvId_GPU_GetFrameBufferCalibrationLockFailures = 0x524B9773;
        private const uint NvId_GPU_GetHardwareQualType = 0xF91E777B;
        private const uint NvId_GPU_GetHybridControllerInfo = 0xD26B8A58;
        private const uint NvId_GPU_GetLogicalFBWidthAndLocation = 0x8efc0978;
        private const uint NvId_GPU_GetManufacturingInfo = 0xA4218928;
        private const uint NvId_GPU_GetMemPartitionMask = 0x329D77CD;
        private const uint NvId_GPU_GetMXMBlock = 0xB7AB19B9;
        private const uint NvId_GPU_GetPartitionCount = 0x86F05D7A;
        private const uint NvId_GPU_GetPCIEInfo = 0xE3795199;
        private const uint NvId_GPU_GetPerfClocks = 0x1EA54A3B;
        private const uint NvId_GPU_GetPerfHybridMode = 0x5D7CCAEB;
        private const uint NvId_GPU_GetPerGpuTopologyStatus = 0x0A81F8992;
        private const uint NvId_GPU_GetPixelClockRange = 0x66AF10B7;
        private const uint NvId_GPU_GetPowerMizerInfo = 0x76BFA16B;
        private const uint NvId_GPU_GetPSFloorSweepStatus = 0xDEE047AB;
        private const uint NvId_GPU_GetPstateClientLimits = 0x88C82104;
        private const uint NvId_GPU_GetPstatesInfo = 0x0BA94C56E;
        private const uint NvId_GPU_GetRamBankCount = 0x17073A3C;
        private const uint NvId_GPU_GetRamBusWidth = 0x7975C581;
        private const uint NvId_GPU_GetRamConfigStrap = 0x51CCDB2A;
        private const uint NvId_GPU_GetRamMaker = 0x42aea16a;
        private const uint NvId_GPU_GetRamType = 0x57F7CAAC;
        private const uint NvId_GPU_GetRawFuseData = 0xE0B1DCE9;
        private const uint NvId_GPU_GetROPCount = 0xfdc129fa;
        private const uint NvId_GPU_GetSampleType = 0x32E1D697;
        private const uint NvId_GPU_GetSerialNumber = 0x14B83A5F;
        private const uint NvId_GPU_GetShaderPipeCount = 0x63E2F56F;
        private const uint NvId_GPU_GetShortName = 0xD988F0F3;
        private const uint NvId_GPU_GetSMMask = 0x0EB7AF173;
        private const uint NvId_GPU_GetTargetID = 0x35B5FD2F;
        private const uint NvId_GPU_GetThermalPoliciesInfo = 0x00D258BB5; // private const uint NvId_GPU_ClientThermalPoliciesGetInfo
        private const uint NvId_GPU_GetThermalPoliciesStatus = 0x0E9C425A1;
        private const uint NvId_GPU_GetThermalTable = 0xC729203C;
        private const uint NvId_GPU_GetTotalSMCount = 0x0AE5FBCFE;
        private const uint NvId_GPU_GetTotalSPCount = 0x0B6D62591;
        private const uint NvId_GPU_GetTotalTPCCount = 0x4E2F76A8;
        private const uint NvId_GPU_GetTPCMask = 0x4A35DF54;
        private const uint NvId_GPU_GetUsages = 0x189a1fdf;
        private const uint NvId_GPU_GetVbiosImage = 0xFC13EE11;
        private const uint NvId_GPU_GetVbiosMxmVersion = 0xE1D5DABA;
        private const uint NvId_GPU_GetVFPCurve = 0x21537ad4;
        private const uint NvId_GPU_GetVoltageDomainsStatus = 0x0C16C7E2C;
        private const uint NvId_GPU_GetVoltages = 0x7D656244;
        private const uint NvId_GPU_GetVoltageStep = 0x28766157; // unsure of the name
        private const uint NvId_GPU_GetVPECount = 0xD8CBF37B;
        private const uint NvId_GPU_GetVSFloorSweepStatus = 0xD4F3944C;
        private const uint NvId_GPU_GPIOQueryLegalPins = 0x0FAB69565;
        private const uint NvId_GPU_GPIOReadFromPin = 0x0F5E10439;
        private const uint NvId_GPU_GPIOWriteToPin = 0x0F3B11E68;
        private const uint NvId_GPU_PerfPoliciesGetInfo = 0x409d9841;
        private const uint NvId_GPU_PerfPoliciesGetStatus = 0x3d358a0c;
        private const uint NvId_GPU_PhysxQueryRecommendedState = 0x7A4174F4;
        private const uint NvId_GPU_PhysxSetState = 0x4071B85E;
        private const uint NvId_GPU_QueryActiveApps = 0x65B1C5F5;
        private const uint NvId_GPU_RestoreCoolerPolicyTable = 0x0D8C4FE63;
        private const uint NvId_GPU_RestoreCoolerSettings = 0x8F6ED0FB;
        private const uint NvId_GPU_SetClockBoostLock = 0x39442cfb; // unknown name; NVAPI_ID_CURVE_SET
        private const uint NvId_GPU_SetClockBoostTable = 0x0733e009;
        private const uint NvId_GPU_SetClocks = 0x6F151055;
        private const uint NvId_GPU_SetColorSpaceConversion = 0x0FCABD23A;
        private const uint NvId_GPU_SetCoolerLevels = 0x891FA0AE;
        private const uint NvId_GPU_SetCoolerPolicyTable = 0x987947CD;
        private const uint NvId_GPU_SetCoreVoltageBoostPercent = 0xb9306d9b;
        private const uint NvId_GPU_SetCurrentPCIESpeed = 0x3BD32008;
        private const uint NvId_GPU_SetCurrentPCIEWidth = 0x3F28E1B9;
        private const uint NvId_GPU_SetDeepIdleState = 0x568A2292;
        private const uint NvId_GPU_SetDisplayUnderflowMode = 0x387B2E41;
        private const uint NvId_GPU_SetDitherControl = 0x0DF0DFCDD;
        private const uint NvId_GPU_SetPerfClocks = 0x7BCF4AC;
        private const uint NvId_GPU_SetPerfHybridMode = 0x7BC207F8;
        private const uint NvId_GPU_SetPixelClockRange = 0x5AC7F8E5;
        private const uint NvId_GPU_SetPowerMizerInfo = 0x50016C78;
        private const uint NvId_GPU_SetPstateClientLimits = 0x0FDFC7D49;
        private const uint NvId_GPU_SetPstates20 = 0x0F4DAE6B;
        private const uint NvId_GPU_SetPstatesInfo = 0x0CDF27911;
        private const uint NvId_GPU_SetThermalPoliciesStatus = 0x034C0B13D;
        private const uint NvId_Hybrid_IsAppMigrationStateChangeable = 0x584CB0B6;
        private const uint NvId_Hybrid_QueryBlockedMigratableApps = 0x0F4C2F8CC;
        private const uint NvId_Hybrid_QueryUnblockedNonMigratableApps = 0x5F35BCB5;
        private const uint NvId_Hybrid_SetAppMigrationState = 0x0FA0B9A59;
        private const uint NvId_I2CReadEx = 0x4D7B0709;
        private const uint NvId_I2CWriteEx = 0x283AC65A;
        private const uint NvId_LoadMicrocode = 0x3119F36E;
        private const uint NvId_Mosaic_ChooseGpuTopologies = 0x0B033B140;
        private const uint NvId_Mosaic_EnumGridTopologies = 0x0A3C55220;
        private const uint NvId_Mosaic_GetDisplayCapabilities = 0x0D58026B9;
        private const uint NvId_Mosaic_GetMosaicCapabilities = 0x0DA97071E;
        private const uint NvId_Mosaic_GetMosaicViewports = 0x7EBA036;
        private const uint NvId_Mosaic_SetGridTopology = 0x3F113C77;
        private const uint NvId_Mosaic_ValidateDisplayGridsWithSLI = 0x1ECFD263;
        private const uint NvId_QueryNonMigratableApps = 0x0BB9EF1C3;
        private const uint NvId_QueryUnderscanCap = 0x61D7B624;
        private const uint NvId_RestartDisplayDriver = 0xB4B26B65;
        private const uint NvId_RevertCustomDisplayTrial = 0x854BA405;
        private const uint NvId_SaveCustomDisplay = 0x0A9062C78;
        private const uint NvId_SetDisplayFeatureConfig = 0x0F36A668D;
        private const uint NvId_SetDisplayPosition = 0x57D9060F;
        private const uint NvId_SetDisplaySettings = 0x0E04F3D86;
        private const uint NvId_SetDVCLevel = 0x172409B4;
        private const uint NvId_SetDVCLevelEx = 0x4A82C2B1;
        private const uint NvId_SetFrameRateNotify = 0x18919887;
        private const uint NvId_SetGpuTopologies = 0x25201F3D;
        private const uint NvId_SetHUEAngle = 0x0F5A0F22C;
        private const uint NvId_SetHybridMode = 0x0FB22D656;
        private const uint NvId_SetImageSharpeningLevel = 0x3FC9A59C;
        private const uint NvId_SetInfoFrame = 0x69C6F365;
        private const uint NvId_SetInfoFrameState = 0x67EFD887;
        private const uint NvId_SetPVExtName = 0x4FEEB498;
        private const uint NvId_SetPVExtProfile = 0x8354A8F4;
        private const uint NvId_SetTopologyDisplayGPU = 0xF409D5E5;
        private const uint NvId_SetTopologyFocusDisplayAndView = 0x0A8064F9;
        private const uint NvId_SetTVEncoderControls = 0x0CA36A3AB;
        private const uint NvId_SetTVOutputBorderColor = 0x0AED02700;
        private const uint NvId_SetUnderscanConfig = 0x3EFADA1D;
        private const uint NvId_SetVideoState = 0x54FE75A;
        private const uint NvId_Stereo_AppHandShake = 0x8C610BDA;
        private const uint NvId_Stereo_ForceToScreenDepth = 0x2D495758;
        private const uint NvId_Stereo_GetCursorSeparation = 0x72162B35;
        private const uint NvId_Stereo_GetPixelShaderConstantB = 0x0C79333AE;
        private const uint NvId_Stereo_GetPixelShaderConstantF = 0x0D4974572;
        private const uint NvId_Stereo_GetPixelShaderConstantI = 0x0ECD8F8CF;
        private const uint NvId_Stereo_GetStereoCaps = 0x0DFC063B7;
        private const uint NvId_Stereo_GetVertexShaderConstantB = 0x712BAA5B;
        private const uint NvId_Stereo_GetVertexShaderConstantF = 0x622FDC87;
        private const uint NvId_Stereo_GetVertexShaderConstantI = 0x5A60613A;
        private const uint NvId_Stereo_HandShake_Message_Control = 0x315E0EF0;
        private const uint NvId_Stereo_HandShake_Trigger_Activation = 0x0B30CD1A7;
        private const uint NvId_Stereo_Is3DCursorSupported = 0x0D7C9EC09;
        private const uint NvId_Stereo_SetCursorSeparation = 0x0FBC08FC1;
        private const uint NvId_Stereo_SetPixelShaderConstantB = 0x0BA6109EE;
        private const uint NvId_Stereo_SetPixelShaderConstantF = 0x0A9657F32;
        private const uint NvId_Stereo_SetPixelShaderConstantI = 0x912AC28F;
        private const uint NvId_Stereo_SetVertexShaderConstantB = 0x5268716F;
        private const uint NvId_Stereo_SetVertexShaderConstantF = 0x416C07B3;
        private const uint NvId_Stereo_SetVertexShaderConstantI = 0x7923BA0E;
        private const uint NvId_SYS_GetChipSetTopologyStatus = 0x8A50F126;
        private const uint NvId_SYS_GetSliApprovalCookie = 0xB539A26E;
        private const uint NvId_SYS_SetPostOutput = 0xD3A092B1;
        private const uint NvId_SYS_VenturaGetCoolingBudget = 0x0C9D86E33;
        private const uint NvId_SYS_VenturaGetPowerReading = 0x63685979;
        private const uint NvId_SYS_VenturaGetState = 0x0CB7C208D;
        private const uint NvId_SYS_VenturaSetCoolingBudget = 0x85FF5A15;
        private const uint NvId_SYS_VenturaSetState = 0x0CE2E9D9;
        private const uint NvId_TryCustomDisplay = 0x0BF6C1762;
        private const uint NvId_VideoGetStereoInfo = 0x8E1F8CFE;
        private const uint NvId_VideoSetStereoInfo = 0x97063269;
        private const uint NvId_GPU_ClientFanCoolersGetInfo = 0xfb85b01e;
        private const uint NvId_GPU_ClientFanCoolersGetStatus = 0x35aed5e8;
        private const uint NvId_GPU_ClientFanCoolersGetControl = 0x814b209f;
        private const uint NvId_GPU_ClientFanCoolersSetControl = 0xa58971a5;

        #endregion
        private const uint NvId_Initialize = 0x150E828;

        #region General NVAPI Functions
        // QueryInterface
        private delegate IntPtr QueryInterfaceDelegate(uint id);
        private static readonly QueryInterfaceDelegate QueryInterface;

        // Initialize
        private delegate NVAPI_STATUS InitializeDelegate();
        private static readonly InitializeDelegate InitializeInternal;

        // Unload
        private delegate NVAPI_STATUS UnloadDelegate();
        private static readonly UnloadDelegate UnloadInternal;

        // GetErrorMessage
        private delegate NVAPI_STATUS GetErrorMessageDelegate(NVAPI_STATUS nr, StringBuilder szDesc);
        private static readonly GetErrorMessageDelegate GetErrorMessageInternal;

        /// <summary>
        /// This function converts an NvAPI error code into a null terminated string.
        /// </summary>
        /// <param name="nr">The error code to convert</param>
        /// <param name="szDesc">The string corresponding to the error code</param>
        /// <returns></returns>
        public static NVAPI_STATUS NvAPI_GetErrorMessage(NVAPI_STATUS nr, out string szDesc)
        {
            StringBuilder builder = new StringBuilder((int)NV_SHORT_STRING_MAX);

            NVAPI_STATUS status;
            if (GetErrorMessageInternal != null) { status = GetErrorMessageInternal(nr, builder); }
            else { status = NVAPI_STATUS.NVAPI_FUNCTION_NOT_FOUND; }
            szDesc = builder.ToString();

            return status;
        }

        // GetInterfaceVersionString
        private delegate NVAPI_STATUS GetInterfaceVersionStringDelegate(StringBuilder szDesc);
        private static readonly GetInterfaceVersionStringDelegate GetInterfaceVersionStringInternal;

        /// <summary>
        /// This function returns a string describing the version of the NvAPI library. The contents of the string are human readable. Do not assume a fixed format.
        /// </summary>
        /// <param name="szDesc">User readable string giving NvAPI version information</param>
        /// <returns></returns>
        public static NVAPI_STATUS NvAPI_GetInterfaceVersionString(out string szDesc)
        {
            StringBuilder builder = new StringBuilder((int)NV_SHORT_STRING_MAX);

            NVAPI_STATUS status;
            if (GetErrorMessageInternal != null) { status = GetInterfaceVersionStringInternal(builder); }
            else { status = NVAPI_STATUS.NVAPI_FUNCTION_NOT_FOUND; }
            szDesc = builder.ToString();

            return status;
        }
        #endregion


        #region Display NVAPI Functions
        // EnumNvidiaDisplayHandle
        private delegate NVAPI_STATUS EnumNvidiaDisplayHandleDelegate(int thisEnum, ref DisplayHandle displayHandle);
        private static readonly EnumNvidiaDisplayHandleDelegate EnumNvidiaDisplayHandleInternal;

        /// <summary>
        /// This function returns the handle of the NVIDIA display specified by the enum index (thisEnum). The client should keep enumerating until it returns NVAPI_END_ENUMERATION.
        /// Note: Display handles can get invalidated on a modeset, so the calling applications need to renum the handles after every modeset.
        /// </summary>
        /// <param name="thisEnum">The index of the NVIDIA display.</param>
        /// <param name="displayHandle">Pointer to the NVIDIA display handle.</param>
        /// <returns></returns>
        public static NVAPI_STATUS NvAPI_EnumNvidiaDisplayHandle(int thisEnum, ref DisplayHandle displayHandle)
        {
            NVAPI_STATUS status;
            if (EnumNvidiaDisplayHandleInternal != null) { status = EnumNvidiaDisplayHandleInternal(thisEnum, ref displayHandle); }
            else { status = NVAPI_STATUS.NVAPI_FUNCTION_NOT_FOUND; }
            return status;
        }

        // EnumNvidiaUnAttachedDisplayHandle
        private delegate NVAPI_STATUS EnumNvidiaUnAttachedDisplayHandleDelegate(int thisEnum, ref UnAttachedDisplayHandle pNvDispHandle);
        private static readonly EnumNvidiaUnAttachedDisplayHandleDelegate EnumNvidiaUnAttachedDisplayHandleInternal;

        /// <summary>
        /// This function returns the handle of the NVIDIA unattached display specified by the enum index (thisEnum). The client should keep enumerating until it returns error. Note: Display handles can get invalidated on a modeset, so the calling applications need to renum the handles after every modeset.
        /// </summary>
        /// <param name="thisEnum">The index of the NVIDIA display.</param>
        /// <param name="pNvDispHandle">Pointer to the NVIDIA display handle of the unattached display.</param>
        /// <returns></returns>
        public static NVAPI_STATUS NvAPI_EnumNvidiaUnAttachedDisplayHandle(int thisEnum, ref UnAttachedDisplayHandle pNvDispHandle)
        {
            NVAPI_STATUS status;
            if (EnumNvidiaUnAttachedDisplayHandleInternal != null) { status = EnumNvidiaUnAttachedDisplayHandleInternal(thisEnum, ref pNvDispHandle); }
            else { status = NVAPI_STATUS.NVAPI_FUNCTION_NOT_FOUND; }
            return status;
        }

        // GetAssociatedUnAttachedNvidiaDisplayHandle
        private delegate NVAPI_STATUS GetAssociatedNvidiaDisplayHandleDelegate(StringBuilder szDisplayName, ref DisplayHandle pNvDispHandle);
        private static readonly GetAssociatedNvidiaDisplayHandleDelegate GetAssociatedNvidiaDisplayHandleInternal;

        /// <summary>
        /// This function returns the handle of the NVIDIA display that is associated with the given display "name" (such as "\\.\DISPLAY1").
        /// </summary>
        /// <param name="szDisplayName"></param>
        /// <param name="pNvDispHandle"></param>
        /// <returns></returns>
        public static NVAPI_STATUS NvAPI_GetAssociatedNvidiaDisplayHandle(string szDisplayName, ref DisplayHandle pNvDispHandle)
        {
            StringBuilder builder = new StringBuilder((int)NV_SHORT_STRING_MAX);
            builder.Append(szDisplayName);

            NVAPI_STATUS status;
            if (GetAssociatedNvidiaDisplayHandleInternal != null) { status = GetAssociatedNvidiaDisplayHandleInternal(builder, ref pNvDispHandle); }
            else { status = NVAPI_STATUS.NVAPI_FUNCTION_NOT_FOUND; }
            szDisplayName = builder.ToString();

            return status;
        }

        // GetAssociatedUnAttachedNvidiaDisplayHandle
        private delegate NVAPI_STATUS GetAssociatedUnAttachedNvidiaDisplayHandleDelegate(StringBuilder szDisplayName, ref UnAttachedDisplayHandle pNvUnAttachedDispHandle);
        private static readonly GetAssociatedUnAttachedNvidiaDisplayHandleDelegate GetAssociatedUnAttachedNvidiaDisplayHandleInternal;

        /// <summary>
        /// This function returns the handle of an unattached NVIDIA display that is associated with the given display name (such as "\\DISPLAY1").
        /// </summary>
        /// <param name="szDisplayName"></param>
        /// <param name="pNvUnAttachedDispHandle"></param>
        /// <returns></returns>
        public static NVAPI_STATUS NvAPI_GetAssociatedUnAttachedNvidiaDisplayHandle(string szDisplayName, ref UnAttachedDisplayHandle pNvUnAttachedDispHandle)
        {
            StringBuilder builder = new StringBuilder((int)NV_SHORT_STRING_MAX);
            builder.Append(szDisplayName);

            NVAPI_STATUS status;
            if (GetAssociatedUnAttachedNvidiaDisplayHandleInternal != null) { status = GetAssociatedUnAttachedNvidiaDisplayHandleInternal(builder, ref pNvUnAttachedDispHandle); }
            else { status = NVAPI_STATUS.NVAPI_FUNCTION_NOT_FOUND; }
            szDisplayName = builder.ToString();

            Console.WriteLine(pNvUnAttachedDispHandle.ptr);

            return status;
        }
        #endregion

        // EnumPhysicalGPUs
        private delegate NVAPI_STATUS EnumPhysicalGPUsDelegate(
            [In] [Out] [MarshalAs(UnmanagedType.LPArray, SizeConst = NV_MAX_PHYSICAL_GPUS)] PhysicalGpuHandle[] gpuHandles,
            [Out] out uint gpuCount);
        private static readonly EnumPhysicalGPUsDelegate EnumPhysicalGPUsInternal;

        /// <summary>
        /// This function returns an array of physical GPU handles. Each handle represents a physical GPU present in the system. That GPU may be part of an SLI configuration, or may not be visible to the OS directly.
        /// At least one GPU must be present in the system and running an NVIDIA display driver. The array nvGPUHandle will be filled with physical GPU handle values. The returned gpuCount determines how many entries in the array are valid..
        /// </summary>
        /// <param name="NvGPUHandle"></param>
        /// <param name="pGPUCount"></param>
        /// <returns></returns>
        public static NVAPI_STATUS NvAPI_EnumPhysicalGPUs(ref PhysicalGpuHandle[] NvGPUHandle, out uint pGPUCount)
        {
            NVAPI_STATUS status;
            uint retGPUCount = 0;
            if (EnumPhysicalGPUsInternal != null) { status = EnumPhysicalGPUsInternal(NvGPUHandle, out retGPUCount); }
            else { status = NVAPI_STATUS.NVAPI_FUNCTION_NOT_FOUND; }
            pGPUCount = retGPUCount;
            return status;
        }

        // GetQuadroStatus
        private delegate NVAPI_STATUS GetQuadroStatusDelegate(
            [In] PhysicalGpuHandle gpuHandle,
            [Out] out uint status);
        private static readonly GetQuadroStatusDelegate GetQuadroStatusInternal;

        /// <summary>
        /// This function retrieves the Quadro status for the GPU (1 if Quadro, 0 if GeForce)
        /// </summary>
        /// <param name="NvGPUHandle"></param>
        /// <param name="pStatus"></param>
        /// <returns></returns>
        public static NVAPI_STATUS NvAPI_GPU_GetQuadroStatus(PhysicalGpuHandle NvGPUHandle, out uint pStatus)
        {
            NVAPI_STATUS status;
            uint retStatus = 0;
            if (GetQuadroStatusInternal != null) { status = GetQuadroStatusInternal(NvGPUHandle, out retStatus); }
            else { status = NVAPI_STATUS.NVAPI_FUNCTION_NOT_FOUND; }
            pStatus = retStatus;
            return status;
        }


        // NVAPI_INTERFACE NvAPI_Mosaic_GetCurrentTopo(NV_MOSAIC_TOPO_BRIEF* pTopoBrief, NV_MOSAIC_DISPLAY_SETTING* pDisplaySetting, NvS32* pOverlapX, NvS32* pOverlapY);
        // NvAPI_Mosaic_GetCurrentTopo
        private delegate NVAPI_STATUS Mosaic_GetCurrentTopoDelegate(
            [In][Out] ref NV_MOSAIC_TOPO_BRIEF pTopoBrief,
            [In][Out] ref NV_MOSAIC_DISPLAY_SETTING pDisplaySetting,
            [Out] out int pOverlapX,
            [Out] out int pOverlapY);
        private static readonly Mosaic_GetCurrentTopoDelegate Mosaic_GetCurrentTopoInternal;

        /// <summary>
        ///  This API returns information for the current Mosaic topology. This includes topology, display settings, and overlap values.
        ///  You can call NvAPI_Mosaic_GetTopoGroup() with the topology if you require more information. If there isn't a current topology, then pTopoBrief->topo will be NV_MOSAIC_TOPO_NONE.
        /// </summary>
        /// <param name="pTopoBrief"></param>
        /// <param name="pDisplaySetting"></param>
        /// <param name="pOverlapX"></param>
        /// <param name="pOverlapY"></param>
        /// <returns></returns>
        public static NVAPI_STATUS NvAPI_Mosaic_GetCurrentTopo(ref NV_MOSAIC_TOPO_BRIEF pTopoBrief, ref NV_MOSAIC_DISPLAY_SETTING pDisplaySetting, out int pOverlapX, out int pOverlapY)
        {
            NVAPI_STATUS status;
            pOverlapX = 0;
            pOverlapY = 0;
            pTopoBrief = new NV_MOSAIC_TOPO_BRIEF();
            pTopoBrief.Version = new StructureVersion(NVImport.NV_MOSAIC_TOPO_BRIEF_VER, typeof(NV_MOSAIC_TOPO_BRIEF)).Version; // set the structure version
            pDisplaySetting = new NV_MOSAIC_DISPLAY_SETTING();
            pDisplaySetting.Version =  new StructureVersion(NVImport.NV_MOSAIC_DISPLAY_SETTING_VER, typeof(NV_MOSAIC_DISPLAY_SETTING)).Version;  // set the structure version
            
            if (Mosaic_GetCurrentTopoInternal != null) { status = Mosaic_GetCurrentTopoInternal(ref pTopoBrief, ref pDisplaySetting, out pOverlapX, out pOverlapY ); }
            else { status = NVAPI_STATUS.NVAPI_FUNCTION_NOT_FOUND; }

            return status;
        }


        // NVAPI_INTERFACE NvAPI_Mosaic_GetTopoGroup(NV_MOSAIC_TOPO_BRIEF* pTopoBrief, NV_MOSAIC_TOPO_GROUP* pTopoGroup)
        private delegate NVAPI_STATUS Mosaic_GetTopoGroupDelegate(
            [In] in NV_MOSAIC_TOPO_BRIEF pTopoBrief,
            [In][Out] ref NV_MOSAIC_TOPO_GROUP pTopoGroup);
        private static readonly Mosaic_GetTopoGroupDelegate Mosaic_GetTopoGroupInternal;

        /// <summary>
        ///  This API returns information for the current Mosaic topology. This includes topology, display settings, and overlap values.
        ///  You can call NvAPI_Mosaic_GetTopoGroup() with the topology if you require more information. If there isn't a current topology, then pTopoBrief->topo will be NV_MOSAIC_TOPO_NONE.
        /// </summary>
        /// <param name="pTopoBrief"></param>
        /// <param name="pTopoGroup"></param>
        /// <returns></returns>
        public static NVAPI_STATUS NvAPI_Mosaic_GetTopoGroup(in NV_MOSAIC_TOPO_BRIEF pTopoBrief, ref NV_MOSAIC_TOPO_GROUP pTopoGroup)
        {
            uint totalGpuLayoutCount = NVAPI_MAX_MOSAIC_DISPLAY_ROWS * NVAPI_MAX_MOSAIC_DISPLAY_COLUMNS;
            NVAPI_STATUS status;
            pTopoGroup = new NV_MOSAIC_TOPO_GROUP();
            pTopoGroup.Version = new StructureVersion(NVImport.NV_MOSAIC_TOPO_GROUP_VER, typeof(NV_MOSAIC_TOPO_GROUP)).Version;  // set the structure version
            pTopoGroup.Topos = new NV_MOSAIC_TOPO_DETAILS[NV_MOSAIC_MAX_TOPO_PER_TOPO_GROUP];
            for (int i = 0; i < NV_MOSAIC_MAX_TOPO_PER_TOPO_GROUP; i++)
            {
                pTopoGroup.Topos[i].Version = new StructureVersion(NVImport.NV_MOSAIC_TOPO_DETAILS_VER, typeof(NV_MOSAIC_TOPO_DETAILS)).Version;  // set the NV_MOSAIC_TOPO_DETAILS structure version
                pTopoGroup.Topos[i].GPULayout = new NV_MOSAIC_TOPO_GPU_LAYOUT_CELL[NVAPI_MAX_MOSAIC_DISPLAY_ROWS, NVAPI_MAX_MOSAIC_DISPLAY_COLUMNS];
                /*for (int y = 0; y < NVAPI_MAX_MOSAIC_DISPLAY_ROWS; y++)
                {
                    pTopoGroup.Topos[i].GPULayoutRows[y].GPULayoutColumns = new NV_MOSAIC_TOPO_GPU_LAYOUT_CELL[NVAPI_MAX_MOSAIC_DISPLAY_COLUMNS];
                }*/
            }

            if (Mosaic_GetTopoGroupInternal != null) { status = Mosaic_GetTopoGroupInternal(in pTopoBrief, ref pTopoGroup); }
            else { status = NVAPI_STATUS.NVAPI_FUNCTION_NOT_FOUND; }

            return status;
        }

    }
}
