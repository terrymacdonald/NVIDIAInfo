using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using DisplayMagicianShared;
using System.ComponentModel;
using DisplayMagicianShared.Windows;
using EDIDParser;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DisplayMagicianShared.NVIDIA
{

    [StructLayout(LayoutKind.Sequential)]
    public struct NVIDIA_MOSAIC_CONFIG : IEquatable<NVIDIA_MOSAIC_CONFIG>
    {
        public bool IsMosaicEnabled;
        public TopologyBrief  MosaicTopologyBrief;
        public IDisplaySettings MosaicDisplaySettings;
        public Int32 OverlapX;
        public Int32 OverlapY;
        public IGridTopology[] MosaicGridTopos;
        public UInt32 MosaicGridCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (Int32)NvConstants.NV_MOSAIC_MAX_DISPLAYS)]
        public List<ViewPortF[]> MosaicViewports;
        public UInt32 PrimaryDisplayId;

        public override bool Equals(object obj) => obj is NVIDIA_MOSAIC_CONFIG other && this.Equals(other);

        public bool Equals(NVIDIA_MOSAIC_CONFIG other)
        => IsMosaicEnabled == other.IsMosaicEnabled &&
           MosaicTopologyBrief.Equals(other.MosaicTopologyBrief) &&
           MosaicDisplaySettings.Equals(other.MosaicDisplaySettings) &&
           OverlapX == other.OverlapX &&
           OverlapY == other.OverlapY &&
           MosaicGridTopos.SequenceEqual(other.MosaicGridTopos) &&
           MosaicGridCount == other.MosaicGridCount &&
           NVIDIALibrary.ListOfArraysEqual(MosaicViewports, other.MosaicViewports) &&
           PrimaryDisplayId == other.PrimaryDisplayId;

        public override int GetHashCode()
        {
            return (IsMosaicEnabled, MosaicTopologyBrief, MosaicDisplaySettings, OverlapX, OverlapY, MosaicGridTopos, MosaicGridCount, MosaicViewports, PrimaryDisplayId).GetHashCode();
        }
        public static bool operator ==(NVIDIA_MOSAIC_CONFIG lhs, NVIDIA_MOSAIC_CONFIG rhs) => lhs.Equals(rhs);

        public static bool operator !=(NVIDIA_MOSAIC_CONFIG lhs, NVIDIA_MOSAIC_CONFIG rhs) => !(lhs == rhs);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NVIDIA_PER_DISPLAY_CONFIG : IEquatable<NVIDIA_PER_DISPLAY_CONFIG>
    {
        public bool HasNvHdrEnabled;
        public IHDRCapabilities HdrCapabilities;
        public IHDRColorData HdrColorData;
        public bool HasAdaptiveSync;
        public SetAdaptiveSyncData AdaptiveSyncConfig;
        public bool HasColorData;
        public IColorData ColorData;
        public bool HasCustomDisplay;
        public List<CustomDisplay> CustomDisplays;


        public override bool Equals(object obj) => obj is NVIDIA_PER_DISPLAY_CONFIG other && this.Equals(other);
        public bool Equals(NVIDIA_PER_DISPLAY_CONFIG other)
        => HasNvHdrEnabled == other.HasNvHdrEnabled &&
            HdrCapabilities.Equals(other.HdrCapabilities) &&
            HdrColorData.Equals(other.HdrColorData) &&
            // Disabled the Adaptive Sync equality matching as we are having trouble applying it, which is causing issues in profile matching in DisplayMagician
            // To fix this bit, we need to test the SetActiveConfigOverride Adaptive Sync part of the codebase to apply this properly.
            // But for now, we'll exclude it from the equality matching and also stop trying to use the adaptive sync config.
            //HasAdaptiveSync == other.HasAdaptiveSync &&
            //AdaptiveSyncConfig.Equals(other.AdaptiveSyncConfig) &&
            HasColorData == other.HasColorData &&
            ColorData.Equals(other.ColorData) &&
            HasCustomDisplay == other.HasCustomDisplay &&
            CustomDisplays.SequenceEqual(other.CustomDisplays);

        public override int GetHashCode()
        {
            // Disabled the Adaptive Sync equality matching as we are having trouble applying it, which is causing issues in profile matching in DisplayMagician
            // To fix this bit, we need to test the SetActiveConfigOverride Adaptive Sync part of the codebase to apply this properly.
            // But for now, we'll exclude it from the equality matching and also stop trying to use the adaptive sync config.
            //return (HasNvHdrEnabled, HdrCapabilities, HdrColorData, HasAdaptiveSync, AdaptiveSyncConfig, HasColorData, ColorData, HasCustomDisplay, CustomDisplays).GetHashCode();
            return (HasNvHdrEnabled, HdrCapabilities, HdrColorData, HasColorData, ColorData, HasCustomDisplay, CustomDisplays).GetHashCode();
        }
        public static bool operator ==(NVIDIA_PER_DISPLAY_CONFIG lhs, NVIDIA_PER_DISPLAY_CONFIG rhs) => lhs.Equals(rhs);

        public static bool operator !=(NVIDIA_PER_DISPLAY_CONFIG lhs, NVIDIA_PER_DISPLAY_CONFIG rhs) => !(lhs == rhs);
    }


    /*[StructLayout(LayoutKind.Sequential)]
    public struct NVIDIA_CUSTOM_DISPLAY_CONFIG : IEquatable<NVIDIA_CUSTOM_DISPLAY_CONFIG>
    {
        public List<NV_CUSTOM_DISPLAY_V1> CustomDisplay;

        public override bool Equals(object obj) => obj is NVIDIA_CUSTOM_DISPLAY_CONFIG other && this.Equals(other);
        public bool Equals(NVIDIA_CUSTOM_DISPLAY_CONFIG other)
        => CustomDisplay.SequenceEqual(other.CustomDisplay);

        public override int GetHashCode()
        {
            return (CustomDisplay).GetHashCode();
        }
        public static bool operator ==(NVIDIA_CUSTOM_DISPLAY_CONFIG lhs, NVIDIA_CUSTOM_DISPLAY_CONFIG rhs) => lhs.Equals(rhs);

        public static bool operator !=(NVIDIA_CUSTOM_DISPLAY_CONFIG lhs, NVIDIA_CUSTOM_DISPLAY_CONFIG rhs) => !(lhs == rhs);
    }*/

    [StructLayout(LayoutKind.Sequential)]
    public struct NVIDIA_DRS_CONFIG : IEquatable<NVIDIA_DRS_CONFIG>
    {
        //public bool HasDRSSettings;
        public bool IsBaseProfile;
        public DRSProfileV1 ProfileInfo;
        public List<DRSSettingV1> DriverSettings;

        public override bool Equals(object obj) => obj is NVIDIA_DRS_CONFIG other && this.Equals(other);
        public bool Equals(NVIDIA_DRS_CONFIG other)
        => IsBaseProfile == other.IsBaseProfile &&
            ProfileInfo == other.ProfileInfo &&
            DriverSettings.SequenceEqual(other.DriverSettings);

        public override int GetHashCode()
        {
            return (IsBaseProfile, ProfileInfo, DriverSettings).GetHashCode();
            //return (HasDRSSettings, ProfileInfo, DriverSettings).GetHashCode();
        }
        public static bool operator ==(NVIDIA_DRS_CONFIG lhs, NVIDIA_DRS_CONFIG rhs) => lhs.Equals(rhs);

        public static bool operator !=(NVIDIA_DRS_CONFIG lhs, NVIDIA_DRS_CONFIG rhs) => !(lhs == rhs);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NVIDIA_PER_ADAPTER_CONFIG : IEquatable<NVIDIA_PER_ADAPTER_CONFIG>
    {
        public bool IsQuadro;
        public bool HasLogicalGPU;
        public LogicalGPUData LogicalGPU;
        public UInt32 DisplayCount;
        public Dictionary<UInt32, NVIDIA_PER_DISPLAY_CONFIG> Displays;

        public override bool Equals(object obj) => obj is NVIDIA_PER_ADAPTER_CONFIG other && this.Equals(other);
        public bool Equals(NVIDIA_PER_ADAPTER_CONFIG other)
        => IsQuadro == other.IsQuadro &&
            HasLogicalGPU == other.HasLogicalGPU &&
            LogicalGPU.Equals(other.LogicalGPU) &&
            DisplayCount == other.DisplayCount &&
            NVIDIALibrary.EqualButDifferentOrder<uint, NVIDIA_PER_DISPLAY_CONFIG>(Displays, other.Displays);

        public override int GetHashCode()
        {
            return (IsQuadro, HasLogicalGPU, LogicalGPU, DisplayCount, Displays).GetHashCode();
        }
        public static bool operator ==(NVIDIA_PER_ADAPTER_CONFIG lhs, NVIDIA_PER_ADAPTER_CONFIG rhs) => lhs.Equals(rhs);

        public static bool operator !=(NVIDIA_PER_ADAPTER_CONFIG lhs, NVIDIA_PER_ADAPTER_CONFIG rhs) => !(lhs == rhs);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NVIDIA_DISPLAY_CONFIG : IEquatable<NVIDIA_DISPLAY_CONFIG>
    {
        public bool IsCloned;
        public bool IsOptimus;
        public NVIDIA_MOSAIC_CONFIG MosaicConfig;
        public Dictionary<UInt32, NVIDIA_PER_ADAPTER_CONFIG> PhysicalAdapters;
        public List<IPathInfo> DisplayConfigs;
        public List<NVIDIA_DRS_CONFIG> DRSSettings;
        // Note: We purposely have left out the DisplayNames from the Equals as it's order keeps changing after each reboot and after each profile swap
        // and it is informational only and doesn't contribute to the configuration (it's used for generating the Screens structure, and therefore for
        // generating the profile icon.
        public Dictionary<string, string> DisplayNames;
        public List<string> DisplayIdentifiers;

        public override bool Equals(object obj) => obj is NVIDIA_DISPLAY_CONFIG other && this.Equals(other);

        public bool Equals(NVIDIA_DISPLAY_CONFIG other)
        {
            if (!(IsCloned == other.IsCloned &&
            IsOptimus == other.IsOptimus &&
            PhysicalAdapters.SequenceEqual(other.PhysicalAdapters) &&
            MosaicConfig.Equals(other.MosaicConfig) &&
            DRSSettings.SequenceEqual(other.DRSSettings) &&
            DisplayIdentifiers.SequenceEqual(other.DisplayIdentifiers)))
            {
                return false;
            }

            // Now we need to go through the display configs comparing values, as the order changes if there is a cloned display
            if (!NVIDIALibrary.EqualButDifferentOrder<IPathInfo>(DisplayConfigs, other.DisplayConfigs))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return (IsCloned, IsOptimus, MosaicConfig, PhysicalAdapters, DisplayConfigs, DisplayIdentifiers, DRSSettings).GetHashCode();
        }
        public static bool operator ==(NVIDIA_DISPLAY_CONFIG lhs, NVIDIA_DISPLAY_CONFIG rhs) => lhs.Equals(rhs);

        public static bool operator !=(NVIDIA_DISPLAY_CONFIG lhs, NVIDIA_DISPLAY_CONFIG rhs) => !(lhs == rhs);
    }

    public class NVIDIALibrary : IDisposable
    {

        // Static members are 'eagerly initialized', that is, 
        // immediately when class is loaded for the first time.
        // .NET guarantees thread safety for static initialization
        private static NVIDIALibrary _instance = new NVIDIALibrary();

        private bool _initialised = false;
        private NVIDIA_DISPLAY_CONFIG _activeDisplayConfig;
        public List<MonitorConnectionType> SkippedColorConnectionTypes;
        public List<string> _allConnectedDisplayIdentifiers;

        // To detect redundant calls
        private bool _disposed = false;

        // Instantiate a SafeHandle instance.
        private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);

        static NVIDIALibrary() { }
        public NVIDIALibrary()
        {
            // Populate the list of ConnectionTypes we want to skip as they don't support querying
            SkippedColorConnectionTypes = new List<MonitorConnectionType> {
                MonitorConnectionType.VGA,
                MonitorConnectionType.Component,
                MonitorConnectionType.Composite,
                MonitorConnectionType.SVideo,
                MonitorConnectionType.DVI,
            };

            _activeDisplayConfig = CreateDefaultConfig();
            try
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/NVIDIALibrary: Attempting to load the NVIDIA NVAPI DLL");
                // Attempt to prelink all of the NVAPI functions
                //Marshal.PrelinkAll(typeof(NVImport));

                // If we get here then we definitely have the NVIDIA driver available.
                Status status = Status.Error;
                SharedLogger.logger.Trace("NVIDIALibrary/NVIDIALibrary: Intialising NVIDIA NVAPI library interface");
                // Step 1: Initialise the NVAPI
                _initialised = false;
                try
                {
                    if (NVAPI.IsAvailable())
                    {
                        _initialised = true;
                        SharedLogger.logger.Trace($"NVIDIALibrary/NVIDIALibrary: NVIDIA NVAPI library was initialised successfully");
                        SharedLogger.logger.Trace($"NVIDIALibrary/NVIDIALibrary: Running UpdateActiveConfig to ensure there is a config to use later");
                        _activeDisplayConfig = GetActiveConfig();
                        _allConnectedDisplayIdentifiers = GetAllConnectedDisplayIdentifiers();
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/NVIDIALibrary: Error intialising NVIDIA NVAPI library. NvAPI_Initialize() returned error code {status}");
                    }

                }
                catch (DllNotFoundException ex)
                {
                    // If this fires, then the DLL isn't available, so we need don't try to do anything else
                    SharedLogger.logger.Info(ex, $"NVIDIALibrary/NVIDIALibrary: Exception trying to load the NVIDIA NVAPI DLLs nvapi64.dll or nvapi.dll. This generally means you don't have the NVIDIA driver installed.");
                }
                catch (Exception ex)
                {
                    SharedLogger.logger.Error(ex, $"NVIDIALibrary/NVIDIALibrary: Exception intialising NVIDIA NVAPI library. NvAPI_Initialize() caused an exception.");
                }

            }
            catch (ArgumentNullException ex)
            {
                // If we get here then the PrelinkAll didn't work, meaning the AMD ADL DLL links don't work. We can't continue to use it, so we log the error and exit
                SharedLogger.logger.Info(ex, $"NVIDIALibrary/NVIDIALibrary: Exception2 trying to load the NVIDIA NVAPI DLLs nvapi64.dll or nvapi.dll. This generally means you don't have the NVIDIA driver installed.");
            }
            catch (Exception ex)
            {
                // If we get here then something else didn't work. We can't continue to use the AMD library, so we log the error and exit
                SharedLogger.logger.Info(ex, $"NVIDIALibrary/NVIDIALibrary: Exception3 trying to load the NVIDIA NVAPI DLLs nvapi64.dll or nvapi.dll. This generally means you don't have the NVIDIA driver installed.");
            }

        }

        ~NVIDIALibrary()
        {
            SharedLogger.logger.Trace("NVIDIALibrary/~NVIDIALibrary: Destroying NVIDIA NVAPI library interface");
            // The NVAPI library automatically runs NVAPI_Unload on Exit, so no need for anything here.
        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose() => Dispose(true);

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {

                // Dispose managed state (managed objects).
                _safeHandle?.Dispose();
            }

            _disposed = true;
        }


        public bool IsInstalled
        {
            get
            {
                return _initialised;
            }
        }

        public NVIDIA_DISPLAY_CONFIG ActiveDisplayConfig
        {
            get
            {
                if (_activeDisplayConfig == null)
                    _activeDisplayConfig = CreateDefaultConfig();
                return _activeDisplayConfig;
            }
        }

        public List<string> CurrentDisplayIdentifiers
        {
            get
            {
                if (_activeDisplayConfig == null)
                    _activeDisplayConfig = CreateDefaultConfig();
                return _activeDisplayConfig.DisplayIdentifiers;
            }
        }

        public List<string> PCIVendorIDs
        {
            get
            {
                return new List<string>() { "10DE" };
            }
        }

        public static NVIDIALibrary GetLibrary()
        {
            return _instance;
        }

        public NVIDIA_DISPLAY_CONFIG CreateDefaultConfig()
        {
            NVIDIA_DISPLAY_CONFIG myDefaultConfig = new NVIDIA_DISPLAY_CONFIG();

            // Fill in the minimal amount we need to avoid null references
            // so that we won't break json.net when we save a default config

            myDefaultConfig.MosaicConfig.IsMosaicEnabled = false;
            myDefaultConfig.MosaicConfig.MosaicGridTopos = new GridTopologyV2[] { };
            myDefaultConfig.MosaicConfig.MosaicViewports = new List<Rectangle[]>();
            myDefaultConfig.PhysicalAdapters = new Dictionary<UInt32, NVIDIA_PER_ADAPTER_CONFIG>();
            myDefaultConfig.DisplayConfigs = new List<PathInfoV2>();
            myDefaultConfig.DRSSettings = new List<NVIDIA_DRS_CONFIG>();
            myDefaultConfig.DisplayNames = new Dictionary<string, string>();
            myDefaultConfig.DisplayIdentifiers = new List<string>();
            myDefaultConfig.IsCloned = false;
            myDefaultConfig.IsOptimus = false;

            return myDefaultConfig;
        }

        public bool UpdateActiveConfig()
        {
            SharedLogger.logger.Trace($"NVIDIALibrary/UpdateActiveConfig: Updating the currently active config");
            try
            {
                _activeDisplayConfig = GetActiveConfig();
                _allConnectedDisplayIdentifiers = GetAllConnectedDisplayIdentifiers();
            }
            catch (Exception ex)
            {
                SharedLogger.logger.Trace(ex, $"NVIDIALibrary/UpdateActiveConfig: Exception updating the currently active config");
                return false;
            }

            return true;
        }

        public NVIDIA_DISPLAY_CONFIG GetActiveConfig()
        {
            SharedLogger.logger.Trace($"NVIDIALibrary/GetActiveConfig: Getting the currently active config");
            bool allDisplays = false;
            return GetNVIDIADisplayConfig(allDisplays);
        }

        private NVIDIA_DISPLAY_CONFIG GetNVIDIADisplayConfig(bool allDisplays = false)
        {
            NVIDIA_DISPLAY_CONFIG myDisplayConfig = CreateDefaultConfig();

            if (_initialised)
            {

                // Store all the found display IDs so we can use them later
                List<UInt32> foundDisplayIds = new List<uint>();
                int physicalGpuCount = 0;
                PhysicalGPUHandle[] physicalGpus = new PhysicalGPUHandle[PhysicalGPUHandle.MaxPhysicalGPUs];

                try
                {
                    // Enumerate all the Physical GPUs
                    physicalGpus = NVAPI.EnumPhysicalGPUs();
                    physicalGpuCount = physicalGpus.Length;
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_EnumPhysicalGPUs returned {physicalGpuCount} Physical GPUs");

                    // This check is to make sure that we only continue in this function if there are physical GPUs to actually do anything with
                    // If the driver is installed, but not physical GPUs are present then we just want to return a default blank config.
                    if (physicalGpuCount == 0)
                    {
                        // Return the default config
                        return CreateDefaultConfig();
                    }

                }
                catch (Exception ex)
                {
                    SharedLogger.logger.Error(ex,$"NVIDIALibrary/GetNVIDIADisplayConfig: Error getting physical GPU count.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Returning the blank NVIDIA config to try and allow other video libraries to work.");
                    // Return the default config to see if we can keep going.
                    return myDisplayConfig;
                }

                // This try/catch is to handle the case where there is an NVIDIA GPU in the machine but it's not being used!
                try
                {

                    // Go through the Physical GPUs one by one to get the logical adapter information
                    for (uint physicalGpuIndex = 0; physicalGpuIndex < physicalGpuCount; physicalGpuIndex++)
                    {
                        // Prepare the physicalGPU per adapter structure to use later
                        NVIDIA_PER_ADAPTER_CONFIG myAdapter = new NVIDIA_PER_ADAPTER_CONFIG();
                        myAdapter.LogicalGPU.PhysicalGPUHandles = new PhysicalGPUHandle[0];
                        myAdapter.IsQuadro = false;
                        myAdapter.HasLogicalGPU = false;
                        myAdapter.Displays = new Dictionary<uint, NVIDIA_PER_DISPLAY_CONFIG>();

                        try
                        {
                            if (NVAPI.QueryWorkstationFeatureSupport(physicalGpus[physicalGpuIndex], WorkstationFeatureType.Proviz))
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NVIDIA Video Card is one from the Quadro range");
                                myAdapter.IsQuadro = true;
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NVIDIA Video Card is not a Quadro range video card.");
                            }

                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Error(ex,$"NVIDIALibrary/GetNVIDIADisplayConfig: Exception caused whilst trying to find out if the card is from the Quadro range.");
                        }


                        try
                        {
                            // Firstly let's get the logical GPU from the Physical handle
                            LogicalGPUHandle logicalGPUHandle = NVAPI.GetLogicalGPUFromPhysicalGPU(physicalGpus[physicalGpuIndex]);
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Successfully got Logical GPU Handle from physical GPU.");
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Now attempting to get the Logical GPU Information");
                            LogicalGPUData logicalGPUData = new LogicalGPUData();
                            NVAPI.GetLogicalGPUInfo(logicalGPUHandle, out logicalGPUData);
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Successfully got the Logical GPU information from the NVIDIA driver!");
                            myAdapter.HasLogicalGPU = true;
                            myAdapter.LogicalGPU = logicalGPUData;                            
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Error(ex, $"NVIDIALibrary/GetNVIDIADisplayConfig: Exception caused whilstgetting Logical GPU handle from Physical GPU using NvAPI_GetLogicalGPUFromPhysicalGPU().");
                            myAdapter.HasLogicalGPU = false;
                        }

                        myDisplayConfig.PhysicalAdapters[physicalGpuIndex] = myAdapter;
                    }


                    TopologyBrief mosaicTopoBrief = new TopologyBrief();
                    IDisplaySettings mosaicDisplaySetting = new DisplaySettingsV2();
                    int mosaicOverlapX = 0;
                    int mosaicOverlapY = 0;

                    try
                    {
                        // Get current Mosaic Topology settings in brief (check whether Mosaic is on)
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Attempting to get the current mosaic topology brief and mosaic display settings.");
                        NVAPI.GetCurrentTopology(out mosaicTopoBrief, out mosaicDisplaySetting, out mosaicOverlapX, out mosaicOverlapY);
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Successfully got the current mosaic toplogy brief and mosaic display settings.");
                    }
                    catch (Exception ex)
                    {
                        SharedLogger.logger.Error(ex, $"NVIDIALibrary/GetNVIDIADisplayConfig: Exception caused whilst getting current mosiac topology brief and mosaic display settings.");
                    }

                    // Get more Mosaic Topology detailed settings
                    TopologyGroup mosaicTopoGroup = new TopologyGroup();

                    try
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Attempting to get the current mosaic topology gropup.");
                        mosaicTopoGroup = NVAPI.GetTopologyGroup(mosaicTopoBrief);
                    
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_GetTopoGroup returned OK.");
                        if (mosaicTopoBrief.IsPossible)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: The current Mosaic Topology of {mosaicTopoBrief.Topology} is possible to use");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: The current Mosaic Topology of {mosaicTopoBrief.Topology} is NOT possible to use");
                        }
                        if (mosaicTopoGroup.TopologiesCount > 0)
                        {
                            int m = 1;
                            foreach (var mosaicTopoDetail in mosaicTopoGroup.TopologyDetails)
                            {

                                if (mosaicTopoDetail.ValidityFlags == TopologyValidity.Valid)
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: The returned Mosaic Topology Group #{m} is VALID.");
                                }
                                else
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The returned Mosaic Topology Group #{m} is NOT VALID and cannot be used.");
                                    if (mosaicTopoDetail.ValidityFlags == TopologyValidity.MissingGPU)
                                    {
                                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The returned Mosaic Topology Group #{m} is MISSING THE GPU it was created with.");
                                    }
                                    if (mosaicTopoDetail.ValidityFlags == TopologyValidity.MissingDisplay)
                                    {
                                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The returned Mosaic Topology Group #{m} is MISSING ONE OR MORE DISPLAYS it was created with.");
                                    }
                                    if (mosaicTopoDetail.ValidityFlags == TopologyValidity.MixedDisplayTypes)
                                    {
                                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The returned Mosaic Topology Group #{m} is USING MIXED DISPLAY TYPES and NVIDIA don't support that at present.");
                                    }

                                }
                            }
                        }
                        else
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The returned Mosaic Topology Group doesn't have any returned Topo Groups. We expect there should be at least one if the Mosaic display layout is configured correctly.");
                        }
                    }
                    catch (Exception ex)
                    {
                        SharedLogger.logger.Error(ex, $"NVIDIALibrary/GetNVIDIADisplayConfig: Exception caused whilst getting current mosiac topology group.");
                    }

                    // Check if there is a topology and that Mosaic is enabled
                    if (mosaicTopoBrief.Topology != Topology.None && mosaicTopoBrief.IsEnable)
                    {
                        // Mosaic is enabled!
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NVIDIA Mosaic is enabled.");
                        myDisplayConfig.MosaicConfig.MosaicTopologyBrief = mosaicTopoBrief;
                        myDisplayConfig.MosaicConfig.MosaicDisplaySettings = mosaicDisplaySetting;
                        myDisplayConfig.MosaicConfig.OverlapX = mosaicOverlapX;
                        myDisplayConfig.MosaicConfig.OverlapY = mosaicOverlapY;
                        myDisplayConfig.MosaicConfig.IsMosaicEnabled = true;

                        IGridTopology[] mosaicGridTopos;
                        try
                        {
                            // Figure out how many Mosaic Grid topoligies there are                    
                            mosaicGridTopos = NVAPI.EnumDisplayGrids();
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Error(ex,$"NVIDIALibrary/GetNVIDIADisplayConfig: Exception occurred while getting Mosaic Topology! NvAPI_Mosaic_EnumDisplayGrids() returned error.");
                            mosaicGridTopos = new IGridTopology[0];
                        }

                        myDisplayConfig.MosaicConfig.MosaicGridTopos = mosaicGridTopos;
                        myDisplayConfig.MosaicConfig.MosaicGridCount = (uint)mosaicGridTopos.Length;

                        List<ViewPortF[]> allViewports = new List<ViewPortF[]>();
                        foreach (IGridTopology gridTopo in mosaicGridTopos)
                        {
                            // Get Current Mosaic Grid settings using the Grid topologies numbers we got before
                            ViewPortF[] viewports = new ViewPortF[NvConstants.NV_MOSAIC_MAX_DISPLAYS];
                            byte bezelCorrected = 0;
                            try
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Attempting to get mosaic display viewport details by resolution.");
                                NVAPI.GetDisplayViewportsByResolution(gridTopo.Displays.FirstOrDefault().DisplayId, 0, 0, out viewports, ref bezelCorrected);
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Successfully got mosaic display viewport details by resolution.");
                            }
                            catch (Exception ex)
                            {
                                SharedLogger.logger.Error(ex,$"NVIDIALibrary/GetNVIDIADisplayConfig: Exception occurred whilst getting display viewport details by resolution.");
                            }

                            // Save the viewports to the List
                            allViewports.Add(viewports);

                            // Get Current Mosaic Display Topology mode settings using the Grid topology we matched before before
                            IDisplaySettings[] mosaicDisplaySettings;
                            try
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Getting mosaic display modes from the current display topology.");
                                mosaicDisplaySettings = NVAPI.EnumDisplayModes(gridTopo);
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Successfully got mosaic display modes from teh current display topology.");

                            }
                            catch (Exception ex)
                            {
                                SharedLogger.logger.Error(ex, $"NVIDIALibrary/GetNVIDIADisplayConfig: Exception occurred whilst getting display modes from current display topology");
                                mosaicDisplaySettings = new IDisplaySettings[0];
                            }

                        }

                        myDisplayConfig.MosaicConfig.MosaicViewports = allViewports;
                    }
                    else
                    {
                        // Mosaic isn't enabled
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NVIDIA Mosaic is NOT enabled.");
                        myDisplayConfig.MosaicConfig.MosaicTopologyBrief = mosaicTopoBrief;
                        myDisplayConfig.MosaicConfig.IsMosaicEnabled = false;
                        myDisplayConfig.MosaicConfig.MosaicGridTopos = new IGridTopology[] { };
                        myDisplayConfig.MosaicConfig.MosaicViewports = new List<ViewPortF[]>();
                    }

                    // Check if Mosaic is possible and log that so we know if troubleshooting bugs
                    if (mosaicTopoBrief.IsPossible)
                    {
                        // Mosaic is possible!
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NVIDIA Mosaic is possible. Mosaic topology would be {mosaicTopoBrief.Topology.ToString("G")}.");
                    }
                    else
                    {
                        // Mosaic isn't possible
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NVIDIA Mosaic is NOT possible.");
                    }

                    // Now we try to get the NVIDIA Windows Display Config. This is needed for handling some of the advanced scaling settings that some advanced users make use of
                    IPathInfo[] pathInfos;
                    try
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Attempting to get NVIDIA display configuration.");
                        pathInfos = NVAPI.GetDisplayConfig();
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Successfully got NVIDIA display configuration..");
                    }
                    catch (Exception ex)
                    {
                        SharedLogger.logger.Error(ex, $"NVIDIALibrary/GetNVIDIADisplayConfig: Exception occurred whilst getting NVIDIA display configuration.");
                        pathInfos = new IPathInfo[0];
                    }

                    // Now try and see if we have a cloned display in the current layout
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Checking if there is a cloned display detected within NVIDIA Display Configuration.");
                    int pathInfoCount = pathInfos.Length;
                    for (int x = 0; x < pathInfoCount; x++)
                    {
                        if (pathInfos[x].TargetsInfo.Count() > 1)
                        {
                            // This is a cloned display, we need to mark this NVIDIA display profile as cloned so we correct the profile later
                            myDisplayConfig.IsCloned = true;

                        }
                    }
                    if (myDisplayConfig.IsCloned)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Cloned display detected within NVIDIA Display Configuration.");
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Cloned display NOT detected within NVIDIA Display Configuration.");
                    }

                    myDisplayConfig.DisplayConfigs = pathInfos.ToList();
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_DISP_GetDisplayConfig returned OK on third pass.");


                    // We want to get the primary monitor
                    UInt32 primaryDisplayId;
                    try
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Attempting to get the primary windows display id.");
                        primaryDisplayId = NVAPI.GetGDIPrimaryDisplayId();
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Successfully got the primary windows display id.");
                    }
                    catch (Exception ex)
                    {
                        SharedLogger.logger.Error(ex, $"NVIDIALibrary/GetNVIDIADisplayConfig: Exception occurred whilst getting the primary windows display id.");
                        primaryDisplayId = 0;
                    }
                    myDisplayConfig.MosaicConfig.PrimaryDisplayId = primaryDisplayId;

                    // We want to get the number of displays we have
                    // Go through the Physical GPUs one by one
                    for (uint physicalGpuIndex = 0; physicalGpuIndex < physicalGpuCount; physicalGpuIndex++)
                    {

                        // Get a new variable to the PhysicalAdapters to make easier to use
                        // NOTE: This struct was filled in earlier by code further up
                        NVIDIA_PER_ADAPTER_CONFIG myAdapter = myDisplayConfig.PhysicalAdapters[physicalGpuIndex];
                        myAdapter.Displays = new Dictionary<uint, NVIDIA_PER_DISPLAY_CONFIG>();

                        //This function retrieves the number of display IDs we know about
                        UInt32 displayCount = 0;
                        IDisplayIds[] displayIds;
                        try
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Attempting to get the list of connected display ids that VIDIA knows about.");
                            displayIds = NVAPI.GetConnectedDisplayIds(physicalGpus[physicalGpuIndex], ConnectedIdsFlag.None);
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Successfully got the list of connected display ids that VIDIA knows about.");
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Error(ex, $"NVIDIALibrary/GetNVIDIADisplayConfig: Exception occurred whilst getting the list of connected display ids that VIDIA knows about.");
                            displayIds = new IDisplayIds[0];
                        }

                        // Time to get the color settings, HDR capabilities and settings for each display
                        //bool isNvHdrEnabled = false;
                        for (int displayIndex = 0; displayIndex < displayCount; displayIndex++)
                        {
                            if (allDisplays)
                            {
                                // We want all physicallyconnected or connected displays
                                if (!(displayIds[displayIndex].IsConnected || displayIds[displayIndex].IsPhysicallyConnected))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                // We want only active displays, so skip any non-active ones
                                if (!displayIds[displayIndex].IsActive)
                                {
                                    continue;
                                }
                            }

                            // Record this as an active display ID
                            foundDisplayIds.Add(displayIds[displayIndex].DisplayId);

                            // Prepare the config structure for us to fill it in
                            NVIDIA_PER_DISPLAY_CONFIG myDisplay = new NVIDIA_PER_DISPLAY_CONFIG();
                            myDisplay.ColorData = new ColorDataV5();
                            myDisplay.HdrColorData = new HDRColorDataV2();
                            // TODO: Change to HDR V3 once we have added the V3 capabilities object.
                            myDisplay.HdrCapabilities = new HDRCapabilitiesV3();
                            myDisplay.AdaptiveSyncConfig = new GetAdaptiveSyncData(); // NOT SUPPORTED BY NvAPIWrapper code!
                            myDisplay.CustomDisplays = new List<CustomDisplay>();
                            myDisplay.HasNvHdrEnabled = false;
                            myDisplay.HasAdaptiveSync = false;
                            myDisplay.HasCustomDisplay = false;

                            // We need to skip recording anything that doesn't support color communication
                            if (!SkippedColorConnectionTypes.Contains(displayIds[displayIndex].ConnectionType))
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: This display supports color information, so attempting to get the various color configuration settings from it.");

                                // skip this monitor connection type as it won't provide the data in the section, and just creates errors                                
                                // We get the Color Capabilities of the display, by setting the command to GET
                                ColorDataV5 colorData5 = new ColorDataV5(ColorDataCommand.Get);
                                try
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Attempting to get the standard  color data from the display.");
                                    NVAPI.ColorControl(displayIds[displayIndex].DisplayId, ref colorData5);
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Successfully got the standard  color data from the display.");
                                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Your monitor {displayIds[displayIndex].DisplayId} has the following color settings set. BPC = {colorData5.Bpc.ToString("G")}. Color Format = {colorData5.ColorFormat.ToString()}. Colorimetry = {colorData5.Colorimetry.ToString("G")}. Color Selection Policy = {colorData5.SelectionPolicy.ToString("G")}. Color Depth = {colorData5.ColorDepth.ToString("G")}. Dynamic Range = {colorData5.DynamicRange.ToString("G")}. ");
                                    myDisplay.ColorData = colorData5;
                                    myDisplay.HasColorData = true;

                                }
                                catch (Exception ex)
                                {
                                    SharedLogger.logger.Error(ex, $"NVIDIALibrary/GetNVIDIADisplayConfig: Exception occurred whilst getting the standard  color data from the display.");
                                    ColorDataV4 colorData4 = new ColorDataV4(ColorDataCommand.Get);
                                    try
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Attempting to get the standard  color data from the display.");
                                        NVAPI.ColorControl(displayIds[displayIndex].DisplayId, ref colorData4);
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Successfully got the standard  color data from the display.");
                                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Your monitor {displayIds[displayIndex].DisplayId} has the following color settings set. BPC = {colorData4.Bpc.ToString("G")}. Color Format = {colorData4.ColorFormat.ToString("G")}. Colorimetry = {colorData4.Colorimetry.ToString("G")}. Color Selection Policy = {colorData4.SelectionPolicy.ToString("G")}. Color Depth = {colorData4.ColorDepth.ToString("G")}. Dynamic Range = {colorData4.DynamicRange.ToString("G")}.");
                                        myDisplay.ColorData = colorData4;
                                        myDisplay.HasColorData = true;

                                    }
                                    catch (Exception nex)
                                    {
                                        SharedLogger.logger.Error(nex, $"NVIDIALibrary/GetNVIDIADisplayConfig: Exception occurred whilst getting the standard  color data from the display.");
                                    }
                                }

                                // Now we get the HDR capabilities of the display
                                // TODO: CHange to HDRCapabilitiesV3 once the v3 struct is completed and tested
                                IHDRCapabilities hdrCapabilities;
                                try
                                {
                                    hdrCapabilities = NVAPI.GetHDRCapabilities(displayIds[displayIndex].DisplayId, false);
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Disp_GetHdrCapabilities returned OK.");
                                    if (hdrCapabilities.IsST2084EOTFSupported)
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} supports HDR mode ST2084 EOTF");
                                    }
                                    else
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} DOES NOT support HDR mode ST2084 EOTF");
                                    }
                                    if (hdrCapabilities.IsDolbyVisionSupported)
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} supports DolbyVision HDR");
                                    }
                                    else
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} DOES NOT support DolbyVision HDR");
                                    }
                                    if (hdrCapabilities.IsEDRSupported)
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} supports EDR");
                                    }
                                    else
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} DOES NOT support EDR");
                                    }
                                    if (hdrCapabilities.IsTraditionalHDRGammaSupported)
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} supports Traditional HDR Gama");
                                    }
                                    else
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} DOES NOT support Traditional HDR Gama");
                                    }

                                    if (hdrCapabilities.IsTraditionalSDRGammaSupported)
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} supports Traditional SDR Gama");
                                    }
                                    else
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} DOES NOT supports Traditional SDR Gama");
                                    }
                                    if (hdrCapabilities.IsDriverDefaultHDRParametersExpanded)
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} supports Driver Expanded Default HDR Parameters");
                                    }
                                    else
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} DOES NOT support Driver Expanded Default HDR Parameters ");
                                    }

                                }
                                catch (Exception nex)
                                {
                                    SharedLogger.logger.Error(nex, $"NVIDIALibrary/GetNVIDIADisplayConfig: Exception occurred whilst getting the standard  color data from the display.");
                                    hdrCapabilities = new HDRCapabilitiesV3();
                                }

                                myDisplay.HdrCapabilities = hdrCapabilities;
                            
 
                                // Now we get the HDR colour settings of the display
                                IHDRColorData hdrColorData;
                                try
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Trying to get the HDR Color Mode for Display ID# {displayIds[displayIndex].DisplayId}.");
                                    hdrColorData = new HDRColorDataV2(ColorDataHDRCommand.Get);
                                    NVAPI.HDRColorControl(displayIds[displayIndex].DisplayId, ref hdrColorData);
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Successfully got the HDR Color Mode for Display ID# {displayIds[displayIndex].DisplayId} is set to {hdrColorData.HDRMode.ToString("G")}.");
                                    if (hdrColorData.HDRMode != ColorDataHDRMode.Off)
                                    {
                                        myDisplay.HasNvHdrEnabled = true;
                                    }
                                    else
                                    {
                                        myDisplay.HasNvHdrEnabled = false;
                                    }

                                }
                                catch (Exception nex)
                                {
                                    SharedLogger.logger.Error(nex, $"NVIDIALibrary/GetNVIDIADisplayConfig: Exception occurred whilst getting the HDR Color Mode for Display ID# {displayIds[displayIndex].DisplayId}.");
                                    hdrColorData = new HDRColorDataV2();
                                }
                                myDisplay.HdrColorData = hdrColorData;
                            
                                // Now we get the Adaptive Sync Settings from the display
                                GetAdaptiveSyncData getAdaptiveSyncData = new GetAdaptiveSyncData();
                                try
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Trying to get the Adaptive Sync Settings for Display ID# {displayIds[displayIndex].DisplayId}.");
                                    NVAPI.GetAdaptiveSyncData(displayIds[displayIndex].DisplayId, ref getAdaptiveSyncData);
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Successfully got the Adaptive Sync Settings for Display ID# {displayIds[displayIndex].DisplayId} is set to {hdrColorData.HDRMode.ToString("G")}.");
                                    // Copy the AdaptiveSync Data we got into a NV_SET_ADAPTIVE_SYNC_DATA_V1 object so that it can be used without conversion
                                    SetAdaptiveSyncData setAdaptiveSyncData = new SetAdaptiveSyncData();
                                    setAdaptiveSyncData.Flags = getAdaptiveSyncData.Flags;
                                    setAdaptiveSyncData.MaxFrameInterval = getAdaptiveSyncData.MaxFrameInterval;

                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_DISP_GetAdaptiveSyncData returned OK.");
                                    if (getAdaptiveSyncData.DisableAdaptiveSync)
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: AdaptiveSync is DISABLED for Display {displayIds[displayIndex].DisplayId} .");
                                    }
                                    else
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: AdaptiveSync is ENABLED for Display {displayIds[displayIndex].DisplayId} .");
                                    }
                                    if (getAdaptiveSyncData.DisableFrameSplitting)
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: FrameSplitting is DISABLED for Display {displayIds[displayIndex].DisplayId} .");
                                    }
                                    else
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: FrameSplitting is ENABLED for Display {displayIds[displayIndex].DisplayId} .");
                                    }
                                    myDisplay.AdaptiveSyncConfig = setAdaptiveSyncData;
                                    myDisplay.HasAdaptiveSync = true;
                                }
                                catch (Exception nex)
                                {
                                    SharedLogger.logger.Error(nex, $"NVIDIALibrary/GetNVIDIADisplayConfig: Exception occurred whilst getting the Adaptive Sync Settings for Display ID# {displayIds[displayIndex].DisplayId}.");
                                    hdrColorData = new HDRColorDataV2();
                                }


                                // TEMPORARILY DISABLING THE CUSTOM DISPLAY CODE FOR NOW, AS NOT SURE WHAT NVIDIA SETTINGS IT TRACKS
                                // KEEPING IT IN CASE I NEED IT FOR LATER. I ORIGINALLY THOUGHT THAT IS WHERE INTEGER SCALING SETTINGS LIVED< BUT WAS WRONG
                                /*// Now we get the Custom Display settings of the display (if there are any)
                                //NVIDIA_CUSTOM_DISPLAY_CONFIG customDisplayConfig = new NVIDIA_CUSTOM_DISPLAY_CONFIG();
                                List<NV_CUSTOM_DISPLAY_V1> customDisplayConfig = new List<NV_CUSTOM_DISPLAY_V1>();
                                for (UInt32 d = 0; d < UInt32.MaxValue; d++)
                                {
                                    NV_CUSTOM_DISPLAY_V1 customDisplay = new NV_CUSTOM_DISPLAY_V1();
                                    status = NVAPI.EnumCustomDisplay(displayIds[displayIndex].DisplayId, d, ref customDisplay);
                                    if (status == Status.Ok)
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_DISP_EnumCustomDisplay returned OK. Custom Display settings retrieved.");
                                        myDisplay.CustomDisplay = customDisplay;
                                        myDisplay.HasCustomDisplay = true;
                                    }
                                    else if (status == Status.NVAPI_END_ENUMERATION)
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: We've reached the end of the list of Custom Displays. Breaking the polling loop.");
                                        break;
                                    }
                                    else if (status == Status.InvalidDisplayId)
                                    {
                                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_DISP_EnumCustomDisplay() returned error code {status}");
                                        break;
                                    }
                                    else if (status == Status.ApiNotInitialized)
                                    {
                                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_DISP_EnumCustomDisplay() returned error code {status}");
                                        break;
                                    }
                                    else if (status == Status.NoImplementation)
                                    {
                                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_DISP_EnumCustomDisplay() returned error code {status}");
                                        break;
                                    }
                                    else if (status == Status.IncompatibleStructureVersion)
                                    {
                                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The supplied struct is incompatible. NvAPI_DISP_EnumCustomDisplay() returned error code {status}");
                                        break;
                                    }
                                    else if (status == Status.Error)
                                    {
                                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_DISP_EnumCustomDisplay() returned error code {status}.");
                                        break;
                                    }
                                    else
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while enumerating the custom displays! NvAPI_DISP_EnumCustomDisplay() returned error code {status}.");
                                        break;
                                    }

                                }*/

                                myAdapter.Displays.Add(displayIds[displayIndex].DisplayId, myDisplay);
                                
                            }
                        }

                        myAdapter.DisplayCount = (UInt32)myAdapter.Displays.Count();
                        myDisplayConfig.PhysicalAdapters[physicalGpuIndex] = myAdapter;

                    }


                    // Now we need to loop through each of the windows paths so we can record the Windows DisplayName to DisplayID mapping
                    // This is needed for us to piece together the Screen layout for when we draw the NVIDIA screens!
                    myDisplayConfig.DisplayNames = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, List<uint>> displaySource in WinLibrary.GetDisplaySourceNames())
                    {
                        // Now we try to get the information about the displayIDs and map them to windows \\DISPLAY names e.g. \\DISPLAY1
                        string displayName = displaySource.Key;
                        UInt32 displayId = 0;
                        status = NVAPI.GetDisplayIdByDisplayName(displayName, out displayId);
                        if (status == Status.Ok)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_DISP_GetDisplayIdByDisplayName returned OK. The display {displayName} has NVIDIA DisplayID {displayId}");
                            myDisplayConfig.DisplayNames.Add(displayId.ToString(), displayName);
                        }
                        else if (status == Status.NvidiaDeviceNotFound)
                        {
                            SharedLogger.logger.Debug($"NVIDIALibrary/GetNVIDIADisplayConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DISP_GetDisplayIdByDisplayName() returned error code {status}");
                        }
                        else if (status == Status.InvalidArgument)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more args passed in are invalid. NvAPI_DISP_GetDisplayIdByDisplayName() returned error code {status}");
                        }
                        else if (status == Status.ApiNotInitialized)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_DISP_GetDisplayIdByDisplayName() returned error code {status}");
                        }
                        else if (status == Status.NoImplementation)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_DISP_GetDisplayIdByDisplayName() returned error code {status}");
                        }
                        else if (status == Status.Error)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_DISP_GetDisplayIdByDisplayName() returned error code {status}");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_DISP_GetDisplayIdByDisplayName() returned error code {status}");
                        }
                    }

                    // Get the display identifiers                
                    myDisplayConfig.DisplayIdentifiers = GetCurrentDisplayIdentifiers();

                    // Get the DRS Settings
                    NvDRSSessionHandle drsSessionHandle = new NvDRSSessionHandle();
                    status = NVAPI.CreateSession(out drsSessionHandle);
                    if (status == Status.Ok)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_DRS_CreateSession returned OK. We got a DRS Session Handle");

                        try
                        {
                            // Load the DRS Settings into memory
                            status = NVAPI.LoadSettings(drsSessionHandle);
                            if (status == Status.Ok)
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_DRS_LoadSettings returned OK. We successfully loaded the DRS Settings into memory.");
                            }
                            else if (status == Status.NvidiaDeviceNotFound)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_LoadSettings() returned error code {status}");
                            }
                            else if (status == Status.InvalidArgument)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more args passed in are invalid. NvAPI_DRS_LoadSettings() returned error code {status}");
                            }
                            else if (status == Status.ApiNotInitialized)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_DRS_LoadSettings() returned error code {status}");
                            }
                            else if (status == Status.NoImplementation)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_DRS_LoadSettings() returned error code {status}");
                            }
                            else if (status == Status.Error)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred whilst loading settings into memory. NvAPI_DRS_LoadSettings() returned error code {status}");
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while loading settings into memory! NvAPI_DRS_GetProfileInfo() returned error code {status}");
                            }

                            // Now we try to start getting the DRS Settings we need
                            // Firstly, we get the profile handle to the global DRS Profile currently in use
                            NvDRSProfileHandle drsProfileHandle = new NvDRSProfileHandle();
                            //status = NVAPI.GetCurrentGlobalProfile(drsSessionHandle, out drsProfileHandle);
                            status = NVAPI.GetBaseProfile(drsSessionHandle, out drsProfileHandle);
                            if (status == Status.Ok)
                            {
                                if (drsProfileHandle.Ptr == IntPtr.Zero)
                                {
                                    // There isn't a custom global profile set yet, so we ignore it
                                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_DRS_GetCurrentGlobalProfile returned OK, but there was no process handle set. THe DRS Settings may not have been loaded.");
                                }
                                else
                                {
                                    // There is a custom global profile set, so we continue
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_DRS_GetCurrentGlobalProfile returned OK. We got the DRS Profile Handle for the current global profile");

                                    // Next, we make a single DRS setting to track the global profile
                                    NVIDIA_DRS_CONFIG drsConfig = new NVIDIA_DRS_CONFIG();
                                    drsConfig.IsBaseProfile = true;

                                    // Next we grab the Profile Info and store it
                                    DRSProfileV1 drsProfileInfo = new DRSProfileV1();
                                    status = NVAPI.GetProfileInfo(drsSessionHandle, drsProfileHandle, ref drsProfileInfo);
                                    if (status == Status.Ok)
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_DRS_GetProfileInfo returned OK. We got the DRS Profile info for the current global profile. Profile Name is {drsProfileInfo.ProfileName}.");
                                        drsConfig.ProfileInfo = drsProfileInfo;
                                    }
                                    else if (status == Status.NvidiaDeviceNotFound)
                                    {
                                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_GetProfileInfo() returned error code {status}");
                                    }
                                    else if (status == Status.InvalidArgument)
                                    {
                                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more args passed in are invalid. NvAPI_DRS_GetProfileInfo() returned error code {status}");
                                    }
                                    else if (status == Status.ApiNotInitialized)
                                    {
                                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_DRS_GetProfileInfo() returned error code {status}");
                                    }
                                    else if (status == Status.NoImplementation)
                                    {
                                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_DRS_GetProfileInfo() returned error code {status}");
                                    }
                                    else if (status == Status.Error)
                                    {
                                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred whilst getting the profile info. NvAPI_DRS_GetProfileInfo() returned error code {status}");
                                    }
                                    else
                                    {
                                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting the profile info! NvAPI_DRS_GetProfileInfo() returned error code {status}");
                                    }

                                    if (drsProfileInfo.NumofSettings > 0)
                                    {
                                        // Next we grab the Profile Settings and store them
                                        NVDRS_SETTING_V1[] drsDriverSettings = new NVDRS_SETTING_V1[drsProfileInfo.NumofSettings];
                                        UInt32 drsNumSettings = drsProfileInfo.NumofSettings;
                                        //NVDRS_SETTING_V1 drsDriverSetting = new NVDRS_SETTING_V1();
                                        status = NVAPI.EnumSettings(drsSessionHandle, drsProfileHandle, 0, ref drsNumSettings, ref drsDriverSettings);
                                        if (status == Status.Ok)
                                        {
                                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_DRS_EnumSettings returned OK. We found {drsNumSettings} settings in the DRS Profile {drsProfileInfo.ProfileName}.");
                                            drsConfig.DriverSettings = drsDriverSettings.ToList();
                                        }
                                        else if (status == Status.NvidiaDeviceNotFound)
                                        {
                                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_EnumSettings() returned error code {status}");
                                        }
                                        else if (status == Status.InvalidArgument)
                                        {
                                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more args passed in are invalid. NvAPI_DRS_EnumSettings() returned error code {status}");
                                        }
                                        else if (status == Status.ApiNotInitialized)
                                        {
                                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_DRS_EnumSettings() returned error code {status}");
                                        }
                                        else if (status == Status.NoImplementation)
                                        {
                                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_DRS_EnumSettings() returned error code {status}");
                                        }
                                        else if (status == Status.Error)
                                        {
                                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred whilst enumerating settings. NvAPI_DRS_EnumSettings() returned error code {status}");
                                        }
                                        else
                                        {
                                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while enumerating settings! NvAPI_DRS_EnumSettings() returned error code {status}");
                                        }

                                        // And then we save the DRS Config to the main config so it gets saved
                                        myDisplayConfig.DRSSettings.Add(drsConfig);

                                    }

                                }
                            }
                            else if (status == Status.NvidiaDeviceNotFound)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                            }
                            else if (status == Status.InvalidArgument)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more args passed in are invalid. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                            }
                            else if (status == Status.ApiNotInitialized)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                            }
                            else if (status == Status.NoImplementation)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                            }
                            else if (status == Status.Error)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred whilst getting the base profile. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting the base profile! NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                            }

                        }
                        finally
                        {
                            // Destroy the DRS Session Handle to clean up
                            status = NVAPI.DestroySession(drsSessionHandle);
                            if (status == Status.Ok)
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_DRS_DestroySession returned OK. We cleaned up and destroyed our DRS Session Handle");
                            }
                            else if (status == Status.NvidiaDeviceNotFound)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_DestroySession() returned error code {status}");
                            }
                            else if (status == Status.InvalidArgument)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more args passed in are invalid. NvAPI_DRS_DestroySession() returned error code {status}");
                            }
                            else if (status == Status.ApiNotInitialized)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_DRS_DestroySession() returned error code {status}");
                            }
                            else if (status == Status.NoImplementation)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_DRS_DestroySession() returned error code {status}");
                            }
                            else if (status == Status.Error)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred whist destroying our DRS Session Handle. NvAPI_DRS_DestroySession() returned error code {status}");
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while destroying our DRS Session Handle! NvAPI_DRS_DestroySession() returned error code {status}");
                            }
                        }
                    }
                    else if (status == Status.NvidiaDeviceNotFound)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_CreateSession() returned error code {status}");
                    }
                    else if (status == Status.InvalidArgument)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more args passed in are invalid. NvAPI_DRS_CreateSession() returned error code {status}");
                    }
                    else if (status == Status.ApiNotInitialized)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_DRS_CreateSession() returned error code {status}");
                    }
                    else if (status == Status.NoImplementation)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_DRS_CreateSession() returned error code {status}");
                    }
                    else if (status == Status.Error)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred whist getting a DRS Session Handle. NvAPI_DRS_CreateSession() returned error code {status}");
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting a DRS Session Handle! NvAPI_DRS_CreateSession() returned error code {status}");
                    }

                }
                catch (Exception ex)
                {
                    SharedLogger.logger.Error(ex, $"NVIDIALibrary/GetNVIDIADisplayConfig: Exception trying to get the NVIDIA Configuration when we know there is an NVIDIA Physical GPU present.");
                    // Return the default config to see if we can keep going.
                    return CreateDefaultConfig();
                }
            }        
            else
            {
                SharedLogger.logger.Info($"NVIDIALibrary/GetNVIDIADisplayConfig: Tried to run GetNVIDIADisplayConfig but the NVIDIA NVAPI library isn't initialised! This generally means you don't have a NVIDIA video card in your machine.");
                //throw new NVIDIALibraryException($"Tried to run GetNVIDIADisplayConfig but the NVIDIA NVAPI library isn't initialised!");
            }

            // Return the configuration
            return myDisplayConfig;
        }


        public string PrintActiveConfig()
        {
            string stringToReturn = "";

            // Get the current config
            NVIDIA_DISPLAY_CONFIG displayConfig = ActiveDisplayConfig;

            stringToReturn += $"****** NVIDIA VIDEO CARDS *******\n";

            // Enumerate all the Physical GPUs
            PhysicalGpuHandle[] physicalGpus = new PhysicalGpuHandle[NVImport.NV_MAX_PHYSICAL_GPUS];
            uint physicalGpuCount = 0;
            Status status = NVImport.NvAPI_EnumPhysicalGPUs(ref physicalGpus, out physicalGpuCount);
            if (status == Status.Ok)
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: NvAPI_EnumPhysicalGPUs returned {physicalGpuCount} Physical GPUs");
                stringToReturn += $"Number of NVIDIA Video cards found: {physicalGpuCount}\n";
            }
            else
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Error getting physical GPU count. NvAPI_EnumPhysicalGPUs() returned error code {status}");
            }

            // This check is to make sure that if there aren't any physical GPUS then we exit!
            if (physicalGpuCount == 0)
            {
                // Print out that there aren't any video cards detected
                stringToReturn += "No NVIDIA Video Cards detected.";
                SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: No NVIDIA Videocards detected");
                return stringToReturn;
            }

            // Go through the Physical GPUs one by one
            for (uint physicalGpuIndex = 0; physicalGpuIndex < physicalGpuCount; physicalGpuIndex++)
            {
                //We want to get the name of the physical device
                string gpuName = "";
                status = NVAPI.GetFullName(physicalGpus[physicalGpuIndex], ref gpuName);
                if (status == Status.Ok)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: NvAPI_GPU_GetFullName returned OK. The GPU Full Name is {gpuName}");
                    stringToReturn += $"NVIDIA Video card #{physicalGpuIndex} is a {gpuName}\n";
                }
                else if (status == Status.NotSupported)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/PrintActiveConfig: Mosaic is not supported with the existing hardware. NvAPI_GPU_GetFullName() returned error code {status}");
                }
                else if (status == Status.InvalidArgument)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/PrintActiveConfig: One or more argumentss passed in are invalid. NvAPI_GPU_GetFullName() returned error code {status}");
                }
                else if (status == Status.ApiNotInitialized)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/PrintActiveConfig: The NvAPI API needs to be initialized first. NvAPI_GPU_GetFullName() returned error code {status}");
                }
                else if (status == Status.NoImplementation)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/PrintActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_GPU_GetFullName() returned error code {status}");
                }
                else if (status == Status.Error)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/PrintActiveConfig: A miscellaneous error occurred. NvAPI_GPU_GetFullName() returned error code {status}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Some non standard error occurred while getting the GPU full name! NvAPI_GPU_GetFullName() returned error code {status}");
                }

                //This function retrieves the Quadro status for the GPU (1 if Quadro, 0 if GeForce)
                uint quadroStatus = 0;
                status = NVAPI.GetQuadroStatus(physicalGpus[physicalGpuIndex], out quadroStatus);
                if (status == Status.Ok)
                {
                    if (quadroStatus == 0)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: NVIDIA Video Card is one from the GeForce range");
                        stringToReturn += $"NVIDIA Video card #{physicalGpuIndex} is in the GeForce range\n";
                    }
                    else if (quadroStatus == 1)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: NVIDIA Video Card is one from the Quadro range");
                        stringToReturn += $"NVIDIA Video card #{physicalGpuIndex} is in the Quadro range\n";
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: NVIDIA Video Card is neither a GeForce or Quadro range vodeo card (QuadroStatus = {quadroStatus})");
                    }
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Error GETTING qUADRO STATUS. NvAPI_GPU_GetQuadroStatus() returned error code {status}");
                }
            }

            stringToReturn += $"\n****** NVIDIA SURROUND/MOSAIC *******\n";
            if (displayConfig.MosaicConfig.IsMosaicEnabled)
            {
                stringToReturn += $"NVIDIA Surround/Mosaic is Enabled\n";
                if (displayConfig.MosaicConfig.MosaicGridTopos.Length > 1)
                {
                    stringToReturn += $"There are {displayConfig.MosaicConfig.MosaicGridTopos.Length} NVIDIA Surround/Mosaic Grid Topologies in use.\n";
                }
                if (displayConfig.MosaicConfig.MosaicGridTopos.Length == 1)
                {
                    stringToReturn += $"There is 1 NVIDIA Surround/Mosaic Grid Topology in use.\n";
                }
                else
                {
                    stringToReturn += $"There are no NVIDIA Surround/Mosaic Grid Topologies in use.\n";
                }

                int count = 0;
                foreach (NV_MOSAIC_GRID_TOPO_V2 gridTopology in displayConfig.MosaicConfig.MosaicGridTopos)
                {
                    stringToReturn += $"NOTE: This Surround/Mosaic screen will be treated as a single display by Windows.\n";
                    stringToReturn += $"The NVIDIA Surround/Mosaic Grid Topology #{count} is {gridTopology.Rows} Rows x {gridTopology.Columns} Columns\n";
                    stringToReturn += $"The NVIDIA Surround/Mosaic Grid Topology #{count} involves {gridTopology.DisplayCount} Displays\n";
                    count++;
                }
            }
            else
            {
                stringToReturn += $"NVIDIA Surround/Mosaic is Disabled\n";
            }

            // Start printing out things for the physical GPU
            foreach (KeyValuePair<UInt32, NVIDIA_PER_ADAPTER_CONFIG> physicalGPU in displayConfig.PhysicalAdapters)
            {
                stringToReturn += $"\n****** NVIDIA PHYSICAL ADAPTER {physicalGPU.Key} *******\n";

                NVIDIA_PER_ADAPTER_CONFIG myAdapter = physicalGPU.Value;

                foreach (KeyValuePair<UInt32, NVIDIA_PER_DISPLAY_CONFIG> myDisplayItem in myAdapter.Displays)
                {
                    string displayId = myDisplayItem.Key.ToString();
                    NVIDIA_PER_DISPLAY_CONFIG myDisplay = myDisplayItem.Value;

                    stringToReturn += $"\n****** NVIDIA PER DISPLAY CONFIG {displayId} *******\n";

                    stringToReturn += $"\n****** NVIDIA COLOR CONFIG *******\n";
                    stringToReturn += $"Display {displayId} BPC is {myDisplay.ColorData.Bpc.ToString("G")}.\n";
                    stringToReturn += $"Display {displayId} ColorFormat is {myDisplay.ColorData.ColorFormat.ToString("G")}.\n";
                    stringToReturn += $"Display {displayId} Colorimetry is {myDisplay.ColorData.Colorimetry.ToString("G")}.\n";
                    stringToReturn += $"Display {displayId} ColorSelectionPolicy is {myDisplay.ColorData.ColorSelectionPolicy.ToString("G")}.\n";
                    stringToReturn += $"Display {displayId} Depth is {myDisplay.ColorData.Depth.ToString("G")}.\n";
                    stringToReturn += $"Display {displayId} DynamicRange is {myDisplay.ColorData.DynamicRange.ToString("G")}.\n";

                    // Start printing out HDR things
                    stringToReturn += $"\n****** NVIDIA HDR CONFIG *******\n";
                    if (myDisplay.HasNvHdrEnabled)
                    {
                        stringToReturn += $"NVIDIA HDR is Enabled\n";
                        if (displayConfig.MosaicConfig.MosaicGridTopos.Length == 1)
                        {
                            stringToReturn += $"There is 1 NVIDIA HDR devices in use.\n";
                        }
                        else
                        {
                            stringToReturn += $"There are no NVIDIA HDR devices in use.\n";
                        }

                        if (myDisplay.HdrCapabilities.IsDolbyVisionSupported)
                        {
                            stringToReturn += $"Display {displayId} supports DolbyVision HDR.\n";
                        }
                        else
                        {
                            stringToReturn += $"Display {displayId} DOES NOT support DolbyVision HDR.\n";
                        }
                        if (myDisplay.HdrCapabilities.IsST2084EotfSupported)
                        {
                            stringToReturn += $"Display {displayId} supports ST2084EOTF HDR Mode.\n";
                        }
                        else
                        {
                            stringToReturn += $"Display {displayId} DOES NOT support ST2084EOTF HDR Mode.\n";
                        }
                        if (myDisplay.HdrCapabilities.IsTraditionalHdrGammaSupported)
                        {
                            stringToReturn += $"Display {displayId} supports Traditional HDR Gamma.\n";
                        }
                        else
                        {
                            stringToReturn += $"Display {displayId} DOES NOT support Traditional HDR Gamma.\n";
                        }
                        if (myDisplay.HdrCapabilities.IsEdrSupported)
                        {
                            stringToReturn += $"Display {displayId} supports EDR.\n";
                        }
                        else
                        {
                            stringToReturn += $"Display {displayId} DOES NOT support EDR.\n";
                        }
                        if (myDisplay.HdrCapabilities.IsTraditionalSdrGammaSupported)
                        {
                            stringToReturn += $"Display {displayId} supports SDR Gamma.\n";
                        }
                        else
                        {
                            stringToReturn += $"Display {displayId} DOES NOT support SDR Gamma.\n";
                        }
                    }
                    else
                    {
                        stringToReturn += $"NVIDIA HDR is Disabled (HDR may still be enabled within Windows itself)\n";
                    }
                }
            }

            // I have to disable this as NvAPI_DRS_EnumAvailableSettingIds function can't be found within the NVAPI.DLL
            // It's looking like it is a problem with the NVAPI.DLL rather than with my code, but I need to do more testing to be sure.
            // Disabling this for now.
            //stringToReturn += DumpAllDRSSettings();

            stringToReturn += $"\n\n";
            // Now we also get the Windows CCD Library info, and add it to the above
            stringToReturn += WinLibrary.GetLibrary().PrintActiveConfig();

            return stringToReturn;
        }

        public bool SetActiveConfig(NVIDIA_DISPLAY_CONFIG displayConfig)
        {

            if (_initialised)
            {

                Status status = Status.Error;
                bool logicalGPURefreshNeeded = false;

                // Remove any custom NVIDIA Colour settings
                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want to turn off colour if it's default set colour.");
                foreach (var physicalGPU in displayConfig.PhysicalAdapters)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Processing settings for Physical GPU #{physicalGPU.Key}");
                    NVIDIA_PER_ADAPTER_CONFIG myAdapter = physicalGPU.Value;
                    UInt32 myAdapterIndex = physicalGPU.Key;
                    foreach (var displayDict in myAdapter.Displays)
                    {
                        NVIDIA_PER_DISPLAY_CONFIG myDisplay = displayDict.Value;
                        UInt32 displayId = displayDict.Key;

                        if (!ActiveDisplayConfig.PhysicalAdapters[myAdapterIndex].Displays.ContainsKey(displayId))
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Display {displayId} doesn't exist in this setup, so skipping changing any NVIDIA display Settings.");
                            continue;
                        }

                        // Remove any custom NVIDIA Colour settings
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want to turn off colour if it's user set colour.");

                        NV_COLOR_DATA_V5 colorData = myDisplay.ColorData;
                        try
                        {
                            // If the setting for this display is not the same as we want, then we set it to NV_COLOR_SELECTION_POLICY_BEST_QUALITY
                            if (ActiveDisplayConfig.PhysicalAdapters[myAdapterIndex].Displays[displayId].ColorData.ColorSelectionPolicy != NV_COLOR_SELECTION_POLICY.NV_COLOR_SELECTION_POLICY_BEST_QUALITY)
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want to turn off NVIDIA customer colour settings for display {displayId}.");

                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want the standard colour settings to be {myDisplay.ColorData.ColorSelectionPolicy.ToString("G")} for Mosaic display {displayId}.");
                                // Force the colorData to be NV_COLOR_SELECTION_POLICY_BEST_QUALITY so that we return the color control to Windows
                                // We will change the colorData to whatever is required later on
                                //colorData = myDisplay.ColorData;
                                colorData.ColorSelectionPolicy = NV_COLOR_SELECTION_POLICY.NV_COLOR_SELECTION_POLICY_BEST_QUALITY;

                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want the standard colour settings to be {myDisplay.ColorData.ColorSelectionPolicy.ToString("G")} and they are currently {ActiveDisplayConfig.PhysicalAdapters[myAdapterIndex].Displays[displayId].ColorData.ColorSelectionPolicy.ToString("G")} for Mosaic display {displayId}.");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want to turn off standard colour mode for Mosaic display {displayId}.");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want standard colour settings Color selection policy {colorData.ColorSelectionPolicy.ToString("G")} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want standard colour settings BPC {colorData.Bpc} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want standard colour settings colour format {colorData.ColorFormat} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want standard colour settings colourimetry {colorData.Colorimetry} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want standard colour settings colour depth {colorData.Depth} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want standard colour settings dynamic range {colorData.DynamicRange} for Mosaic display {displayId}");

                                // Set the command as a 'SET'
                                colorData.Cmd = NV_COLOR_CMD.NV_COLOR_CMD_SET;
                                status = NVAPI.ColorControl(displayId, ref colorData);
                                if (status == Status.Ok)
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_Disp_ColorControl returned OK. BPC is set to {colorData.Bpc.ToString("G")}. Color Format is set to {colorData.ColorFormat.ToString("G")}. Colorimetry is set to {colorData.Colorimetry.ToString("G")}. Color Selection Policy is set to {colorData.ColorSelectionPolicy.ToString("G")}. Color Depth is set to {colorData.Depth.ToString("G")}. Dynamic Range is set to {colorData.DynamicRange.ToString("G")}");
                                    switch (colorData.ColorSelectionPolicy)
                                    {
                                        case NV_COLOR_SELECTION_POLICY.NV_COLOR_SELECTION_POLICY_USER:
                                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Color Selection Policy is set to NV_COLOR_SELECTION_POLICY_USER so the color settings have been set by the user in the NVIDIA Control Panel.");
                                            break;
                                        case NV_COLOR_SELECTION_POLICY.NV_COLOR_SELECTION_POLICY_BEST_QUALITY: // Also matches NV_COLOR_SELECTION_POLICY_DEFAULT as it is 1
                                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Color Selection Policy is set to NV_COLOR_SELECTION_POLICY_BEST_QUALITY so the color settings are being handled by the Windows OS.");
                                            break;
                                        case NV_COLOR_SELECTION_POLICY.NV_COLOR_SELECTION_POLICY_UNKNOWN:
                                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: Color Selection Policy is set to NV_COLOR_SELECTION_POLICY_UNKNOWN so the color settings aren't being handled by either the Windows OS or the NVIDIA Setup!");
                                            break;
                                    }
                                }
                                else if (status == Status.NotSupported)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: Your monitor {displayId} doesn't support the requested color settings. BPC = {colorData.Bpc.ToString("G")}. Color Format = {colorData.ColorFormat.ToString("G")}. Colorimetry = {colorData.Colorimetry.ToString("G")}. Color Selection Policy = {colorData.ColorSelectionPolicy.ToString("G")}. Color Depth = {colorData.Depth.ToString("G")}. Dynamic Range = {colorData.DynamicRange.ToString("G")}. NvAPI_Disp_ColorControl() returned error code {status}");
                                }
                                else if (status == Status.InsufficientBuffer)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The input buffer is not large enough to hold it's contents. NvAPI_Disp_ColorControl() returned error code {status}");
                                }
                                else if (status == Status.InvalidDisplayId)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_Disp_ColorControl() returned error code {status}");
                                }
                                else if (status == Status.ApiNotInitialized)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_Disp_ColorControl() returned error code {status}");
                                }
                                else if (status == Status.NoImplementation)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_Disp_ColorControl() returned error code {status}");
                                }
                                else if (status == Status.Error)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_Disp_ColorControl() returned error code {status}");
                                }
                                else
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while seting the color settings! NvAPI_Disp_ColorControl() returned error code {status}. It's most likely that your monitor {displayId} doesn't support this color mode.");
                                }
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want only want to turn off custom NVIDIA colour settings if needed for display {displayId}, and that currently isn't required. Skipping changing NVIDIA colour mode.");
                            }
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Error(ex, $"NVIDIALibrary/SetActiveConfig: Exception caused while turning off prior NVIDIA specific colour settings for display {displayId}.");
                        }

                        // Remove any custom NVIDIA HDR Colour settings
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want to turn off HDR colour if it's user set HDR colour.");

                        NV_HDR_COLOR_DATA_V2 hdrColorData = myDisplay.HdrColorData;
                        try
                        {

                            // if it's not the same HDR we want, then we turn off HDR (and will apply it if needed later on in SetActiveOverride)
                            if (ActiveDisplayConfig.PhysicalAdapters[myAdapterIndex].Displays[displayId].HdrColorData.HdrMode != NV_HDR_MODE.OFF)
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want to turn on custom HDR mode for display {displayId}.");

                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: HDR mode is currently {ActiveDisplayConfig.PhysicalAdapters[myAdapterIndex].Displays[displayId].HdrColorData.HdrMode.ToString("G")} for Mosaic display {displayId}.");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want HDR settings BPC  {hdrColorData.HdrBpc} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want HDR settings HDR Colour Format {hdrColorData.HdrColorFormat} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want HDR settings HDR dynamic range {hdrColorData.HdrDynamicRange} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want HDR settings HDR Mode {hdrColorData.HdrMode} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want HDR settings Mastering Display Data {hdrColorData.MasteringDisplayData} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want HDR settings Static Meradata Description ID {hdrColorData.StaticMetadataDescriptorId} for Mosaic display {displayId}");
                                // Apply the HDR removal
                                hdrColorData.Cmd = NV_HDR_CMD.CMD_SET;
                                hdrColorData.HdrMode = NV_HDR_MODE.OFF;
                                status = NVAPI.HdrColorControl(displayId, ref hdrColorData);
                                if (status == Status.Ok)
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_Disp_HdrColorControl returned OK. We just successfully turned off the HDR mode for Mosaic display {displayId}.");
                                }
                                else if (status == Status.InsufficientBuffer)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The input buffer is not large enough to hold it's contents. NvAPI_Disp_HdrColorControl() returned error code {status}");
                                }
                                else if (status == Status.InvalidDisplayId)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_Disp_HdrColorControl() returned error code {status}");
                                }
                                else if (status == Status.ApiNotInitialized)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_Disp_HdrColorControl() returned error code {status}");
                                }
                                else if (status == Status.NoImplementation)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_Disp_HdrColorControl() returned error code {status}");
                                }
                                else if (status == Status.Error)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_Disp_HdrColorControl() returned error code {status}");
                                }
                                else
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Disp_HdrColorControl() returned error code {status}. It's most likely that your monitor {displayId} doesn't support HDR.");
                                }
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want only want to turn off custom NVIDIA HDR settings if needed for display {displayId}, and that currently isn't required. Skipping changing NVIDIA HDR mode.");
                            }

                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Error(ex, $"NVIDIALibrary/SetActiveConfig: Exception caused while turning off prior NVIDIA HDR colour settings for display {displayId}.");
                        }


                    }

                }

                // Set the DRS Settings
                NvDRSSessionHandle drsSessionHandle = new NvDRSSessionHandle();
                status = NVAPI.CreateSession(out drsSessionHandle);
                if (status == Status.Ok)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_DRS_CreateSession returned OK. We got a DRS Session Handle");

                    try
                    {
                        // Load the current DRS Settings into memory
                        status = NVAPI.LoadSettings(drsSessionHandle);
                        if (status == Status.Ok)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_DRS_LoadSettings returned OK. We successfully loaded the DRS Settings into memory.");
                        }
                        else if (status == Status.NvidiaDeviceNotFound)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_LoadSettings() returned error code {status}");
                        }
                        else if (status == Status.InvalidArgument)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: One or more args passed in are invalid. NvAPI_DRS_LoadSettings() returned error code {status}");
                        }
                        else if (status == Status.ApiNotInitialized)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_DRS_LoadSettings() returned error code {status}");
                        }
                        else if (status == Status.NoImplementation)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_DRS_LoadSettings() returned error code {status}");
                        }
                        else if (status == Status.Error)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred whist destroying our DRS Session Handle. NvAPI_DRS_LoadSettings() returned error code {status}");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while destroying our DRS Session Handle! NvAPI_DRS_GetProfileInfo() returned error code {status}");
                        }

                        // Now we try to start getting the DRS Settings we need
                        // Firstly, we get the profile handle to the global DRS Profile currently in use
                        NvDRSProfileHandle drsProfileHandle = new NvDRSProfileHandle();
                        status = NVAPI.GetBaseProfile(drsSessionHandle, out drsProfileHandle);
                        if (status == Status.Ok)
                        {
                            if (drsProfileHandle.Ptr == IntPtr.Zero)
                            {
                                // There isn't a custom global profile set yet, so we ignore it
                                SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: NvAPI_DRS_GetCurrentGlobalProfile returned OK, but there was no process handle set. The DRS Settings may not have been loaded.");
                            }
                            else
                            {
                                // There is a custom global profile, so we continue
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_DRS_GetCurrentGlobalProfile returned OK. We got the DRS Profile Handle for the current global profile");

                                // Next, we go through all the settings we have in the saved profile, and we change the current profile settings to be the same
                                if (displayConfig.DRSSettings.Count > 0)
                                {
                                    bool needToSave = false;
                                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: There are {displayConfig.DRSSettings.Count} stored DRS profiles so we need to process them");

                                    try
                                    {
                                        // Get the Base Profiles from the stored config and the active config
                                        NVIDIA_DRS_CONFIG storedBaseProfile = displayConfig.DRSSettings.Find(p => p.IsBaseProfile == true);
                                        NVIDIA_DRS_CONFIG activeBaseProfile = ActiveDisplayConfig.DRSSettings.Find(p => p.IsBaseProfile == true);
                                        foreach (var drsSetting in storedBaseProfile.DriverSettings)
                                        {
                                            for (int i = 0; i < activeBaseProfile.DriverSettings.Count; i++)
                                            {
                                                NVDRS_SETTING_V1 currentSetting = activeBaseProfile.DriverSettings[i];

                                                // If the setting is also in the active base profile (it should be!), then we set it.
                                                if (drsSetting.SettingId == currentSetting.SettingId)
                                                {
                                                    if (drsSetting.CurrentValue.Equals(currentSetting.CurrentValue))
                                                    {
                                                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: '{currentSetting.Name}' ({currentSetting.SettingId}) is set to the same value as the one we want, so skipping changing it.");
                                                    }
                                                    else
                                                    {
                                                        status = NVAPI.SetSetting(drsSessionHandle, drsProfileHandle, drsSetting);
                                                        if (status == Status.Ok)
                                                        {
                                                            needToSave = true;
                                                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We changed setting '{currentSetting.Name}' ({currentSetting.SettingId}) from {currentSetting.CurrentValue} to {drsSetting.CurrentValue} using NvAPI_DRS_SetSetting()");
                                                        }
                                                        else if (status == Status.NvidiaDeviceNotFound)
                                                        {
                                                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_SetSetting() returned error code {status}");
                                                        }
                                                        else if (status == Status.InvalidArgument)
                                                        {
                                                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: One or more args passed in are invalid. NvAPI_DRS_SetSetting() returned error code {status}");
                                                        }
                                                        else if (status == Status.ApiNotInitialized)
                                                        {
                                                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_DRS_SetSetting() returned error code {status}");
                                                        }
                                                        else if (status == Status.NoImplementation)
                                                        {
                                                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_DRS_SetSetting() returned error code {status}");
                                                        }
                                                        else if (status == Status.Error)
                                                        {
                                                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred whist destroying our DRS Session Handle. NvAPI_DRS_SetSetting() returned error code {status}");
                                                        }
                                                        else
                                                        {
                                                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while destroying our DRS Session Handle! NvAPI_DRS_SetSetting() returned error code {status}");
                                                        }

                                                    }
                                                    break;
                                                }
                                            }
                                        }

                                        // Now go through and revert any unset settings to defaults. This guards against new settings being added by other profiles
                                        // after we've created a display profile. If we didn't do this those newer settings would stay set.                                        
                                        foreach (var currentSetting in activeBaseProfile.DriverSettings)
                                        {
                                            // Skip any settings that we've already set
                                            if (storedBaseProfile.DriverSettings.Exists(ds => ds.SettingId == currentSetting.SettingId))
                                            {
                                                continue;
                                            }

                                            status = NVAPI.RestoreProfileDefaultSetting(drsSessionHandle, drsProfileHandle, currentSetting.SettingId);
                                            if (status == Status.Ok)
                                            {
                                                needToSave = true;
                                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We changed active setting '{currentSetting.Name}' ({currentSetting.SettingId}) from {currentSetting.CurrentValue} to it's default  value using NvAPI_DRS_RestoreProfileDefaultSetting()");
                                            }
                                            else if (status == Status.NvidiaDeviceNotFound)
                                            {
                                                SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_RestoreProfileDefaultSetting() returned error code {status}");
                                            }
                                            else if (status == Status.InvalidArgument)
                                            {
                                                SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: One or more args passed in are invalid. NvAPI_DRS_RestoreProfileDefaultSetting() returned error code {status}");
                                            }
                                            else if (status == Status.ApiNotInitialized)
                                            {
                                                SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_DRS_RestoreProfileDefaultSetting() returned error code {status}");
                                            }
                                            else if (status == Status.NoImplementation)
                                            {
                                                SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_DRS_RestoreProfileDefaultSetting() returned error code {status}");
                                            }
                                            else if (status == Status.Error)
                                            {
                                                SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred whist destroying our DRS Session Handle. NvAPI_DRS_RestoreProfileDefaultSetting() returned error code {status}");
                                            }
                                            else
                                            {
                                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while destroying our DRS Session Handle! NvAPI_DRS_RestoreProfileDefaultSetting() returned error code {status}");
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        SharedLogger.logger.Error(ex, $"NVIDIALibrary/SetActiveConfig: Exception while trying to find base profiles in either the stored or active display configs.");
                                    }

                                    // Next we save the Settings if needed
                                    if (needToSave)
                                    {
                                        // Save the current DRS Settings as we changed them
                                        status = NVAPI.SaveSettings(drsSessionHandle);
                                        if (status == Status.Ok)
                                        {
                                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_DRS_SaveSettings returned OK. We successfully saved the DRS Settings.");
                                        }
                                        else if (status == Status.NvidiaDeviceNotFound)
                                        {
                                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_SaveSettings() returned error code {status}");
                                        }
                                        else if (status == Status.InvalidArgument)
                                        {
                                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: One or more args passed in are invalid. NvAPI_DRS_SaveSettings() returned error code {status}");
                                        }
                                        else if (status == Status.ApiNotInitialized)
                                        {
                                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_DRS_SaveSettings() returned error code {status}");
                                        }
                                        else if (status == Status.NoImplementation)
                                        {
                                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_DRS_SaveSettings() returned error code {status}");
                                        }
                                        else if (status == Status.Error)
                                        {
                                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred whilst saving driver settings. NvAPI_DRS_SaveSettings() returned error code {status}");
                                        }
                                        else
                                        {
                                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred whilst saving driver settings! NvAPI_DRS_SaveSettings() returned error code {status}");
                                        }
                                    }
                                }
                            }
                        }
                        else if (status == Status.NvidiaDeviceNotFound)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                        }
                        else if (status == Status.InvalidArgument)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: One or more args passed in are invalid. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                        }
                        else if (status == Status.ApiNotInitialized)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                        }
                        else if (status == Status.NoImplementation)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                        }
                        else if (status == Status.Error)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred whilst getting the Base Profile. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting the Base Profile Handle! NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                        }

                    }
                    finally
                    {
                        // Destroy the DRS Session Handle to clean up
                        status = NVAPI.DestroySession(drsSessionHandle);
                        if (status == Status.Ok)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_DRS_DestroySession returned OK. We cleaned up and destroyed our DRS Session Handle");
                        }
                        else if (status == Status.NvidiaDeviceNotFound)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_DestroySession() returned error code {status}");
                        }
                        else if (status == Status.InvalidArgument)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: One or more args passed in are invalid. NvAPI_DRS_DestroySession() returned error code {status}");
                        }
                        else if (status == Status.ApiNotInitialized)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_DRS_DestroySession() returned error code {status}");
                        }
                        else if (status == Status.NoImplementation)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_DRS_DestroySession() returned error code {status}");
                        }
                        else if (status == Status.Error)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred whist destroying our DRS Session Handle. NvAPI_DRS_DestroySession() returned error code {status}");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while destroying our DRS Session Handle! NvAPI_DRS_DestroySession() returned error code {status}");
                        }
                    }
                }
                else if (status == Status.NvidiaDeviceNotFound)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_CreateSession() returned error code {status}");
                }
                else if (status == Status.InvalidArgument)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: One or more args passed in are invalid. NvAPI_DRS_CreateSession() returned error code {status}");
                }
                else if (status == Status.ApiNotInitialized)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_DRS_CreateSession() returned error code {status}");
                }
                else if (status == Status.NoImplementation)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_DRS_CreateSession() returned error code {status}");
                }
                else if (status == Status.Error)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred whist getting a DRS Session Handle. NvAPI_DRS_CreateSession() returned error code {status}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting a DRS Session Handle! NvAPI_DRS_CreateSession() returned error code {status}");
                }


                // Now we've set the color the way we want it, lets do the thing
                // We want to check the NVIDIA Surround (Mosaic) config is valid
                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Testing whether the display configuration is valid");
                // 
                if (displayConfig.MosaicConfig.IsMosaicEnabled)
                {
                    if (displayConfig.MosaicConfig.Equals(ActiveDisplayConfig.MosaicConfig))
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Mosaic current config is exactly the same as the one we want, so skipping applying the Mosaic config");
                    }
                    else
                    {
                        /*// We need to change to a Mosaic profile, so we need to apply the new Mosaic Topology
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Mosaic current config is different as the one we want, so applying the Mosaic config now");
                        // If we get here then the display is valid, so now we actually apply the new Mosaic Topology
                        status = NVAPI.SetCurrentTopo(displayConfig.MosaicConfig.MosaicTopologyBrief, displayConfig.MosaicConfig.MosaicDisplaySettings, displayConfig.MosaicConfig.OverlapX, displayConfig.MosaicConfig.OverlapY, 0);
                        if (status == Status.Ok)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_Mosaic_SetCurrentTopo returned OK.");
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Waiting 0.5 second to let the Mosaic display change take place before continuing");
                            System.Threading.Thread.Sleep(500);
                        }
                        else if (status == Status.NVAPI_NO_ACTIVE_SLI_TOPOLOGY)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: No matching GPU topologies could be found. NvAPI_Mosaic_SetCurrentTopo() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.NVAPI_TOPO_NOT_POSSIBLE)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The topology passed in is not currently possible. NvAPI_Mosaic_SetCurrentTopo() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.InvalidArgument)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_SetCurrentTopo() returned error code {status}");
                        }
                        else if (status == Status.ApiNotInitialized)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_SetCurrentTopo() returned error code {status}");
                        }
                        else if (status == Status.NoImplementation)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_SetCurrentTopo() returned error code {status}");
                        }
                        else if (status == Status.IncompatibleStructureVersion)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The version of the structure passed in is not compatible with this entrypoint. NvAPI_Mosaic_SetCurrentTopo() returned error code {status}");
                        }
                        else if (status == Status.ModeChangeFailed)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: There was an error changing the display mode. NvAPI_Mosaic_SetCurrentTopo() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.Error)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_Mosaic_SetCurrentTopo() returned error code {status}");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Mosaic Display Grids! NvAPI_Mosaic_SetCurrentTopo() returned error code {status}");
                        }

                        // Turn on the selected Mosaic
                        uint enable = 1;
                        status = NVAPI.EnableCurrentTopo(enable);
                        if (status == Status.Ok)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_Mosaic_EnableCurrentTopo returned OK. Previously set Mosiac config re-enabled.");
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Waiting 0.5 second to let the Mosaic display change take place before continuing");
                            System.Threading.Thread.Sleep(500);
                        }
                        else if (status == Status.NotSupported)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                        }
                        else if (status == Status.NVAPI_TOPO_NOT_POSSIBLE)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The topology passed in is not currently possible. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.InvalidArgument)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                        }
                        else if (status == Status.ApiNotInitialized)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                        }
                        else if (status == Status.NoImplementation)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                        }
                        else if (status == Status.IncompatibleStructureVersion)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The version of the structure passed in is not compatible with this entrypoint. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                        }
                        else if (status == Status.ModeChangeFailed)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: There was an error disabling the display mode. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.Error)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                        }*/

                        //NV_MOSAIC_SETDISPLAYTOPO_FLAGS setTopoFlags = NV_MOSAIC_SETDISPLAYTOPO_FLAGS.MAXIMIZE_PERFORMANCE;
                        NV_MOSAIC_SETDISPLAYTOPO_FLAGS setTopoFlags = NV_MOSAIC_SETDISPLAYTOPO_FLAGS.NONE;

                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Mosaic current config is different as the one we want, so applying the Mosaic config now");
                        // If we get here then the display is valid, so now we actually apply the new Mosaic Topology
                        status = NVAPI.SetDisplayGrids(displayConfig.MosaicConfig.MosaicGridTopos, displayConfig.MosaicConfig.MosaicGridCount, setTopoFlags);
                        if (status == Status.Ok)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_Mosaic_SetDisplayGrids returned OK.");
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Waiting 0.5 second to let the Mosaic display change take place before continuing");
                            System.Threading.Thread.Sleep(500);
                            logicalGPURefreshNeeded = true;
                        }
                        else if (status == Status.NVAPI_NO_ACTIVE_SLI_TOPOLOGY)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: No matching GPU topologies could be found. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.NVAPI_TOPO_NOT_POSSIBLE)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The topology passed in is not currently possible. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.InvalidArgument)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: One or more arguments passed in are invalid. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}. This is often caused by new NVIDIA settings from an NVIDIA driver update. You may ned to recreate your Surround layout.");
                        }
                        else if (status == Status.ApiNotInitialized)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        }
                        else if (status == Status.NoImplementation)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        }
                        else if (status == Status.IncompatibleStructureVersion)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The version of the structure passed in is not compatible with this entrypoint. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        }
                        else if (status == Status.ModeChangeFailed)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: There was an error changing the display mode. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.NVAPI_OUT_OF_MEMORY)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The NvAPI driver is out of memory and is unable to allocate more. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.Error)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Mosaic Display Grids! NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        }
                    }

                }
                else if (!displayConfig.MosaicConfig.IsMosaicEnabled && ActiveDisplayConfig.MosaicConfig.IsMosaicEnabled)
                {
                    // We are on a Mosaic profile now, and we need to change to a non-Mosaic profile
                    // We need to disable the Mosaic Topology

                    //NV_MOSAIC_SETDISPLAYTOPO_FLAGS setTopoFlags = NV_MOSAIC_SETDISPLAYTOPO_FLAGS.ALLOW_INVALID;
                    NV_MOSAIC_SETDISPLAYTOPO_FLAGS setTopoFlags = NV_MOSAIC_SETDISPLAYTOPO_FLAGS.NONE;

                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Mosaic config that is currently set is no longer needed. Removing Mosaic config.");
                    NV_MOSAIC_GRID_TOPO_V2[] individualScreensTopology = CreateSingleScreenMosaicTopology();

                    // WARNING - Validation is disabled at present. This is mostly because there are errors in my NvAPI_Mosaic_ValidateDisplayGrids,
                    // but also because the config is coming from the NVIDIA Control Panel which will already do it's own validation checks.
                    /*// Firstly try to see if the oneScreenTopology is a valid config
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Checking if the 1x1 DisplayGrid we chose is valid for the NvAPI_Mosaic_SetDisplayGrids mosaic layout.");
                    NV_MOSAIC_DISPLAY_TOPO_STATUS_V1[] individualScreensStatuses = new NV_MOSAIC_DISPLAY_TOPO_STATUS_V1[(UInt32)individualScreensTopology.Length];
                    status = NVAPI.ValidateDisplayGrids(setTopoFlags, individualScreensTopology, ref individualScreensStatuses, (UInt32)individualScreensTopology.Length);
                    if (status == Status.Ok)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_Mosaic_ValidateDisplayGrids returned OK.");
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Waiting 0.5 second to let the Mosaic display change take place before continuing");
                        System.Threading.Thread.Sleep(500);
                    }
                    else if (status == Status.NVAPI_NO_ACTIVE_SLI_TOPOLOGY)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: No matching GPU topologies could be found. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.NVAPI_TOPO_NOT_POSSIBLE)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The topology passed in is not currently possible. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.InvalidDisplayId)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The Display ID of the first display is not currently possible to use. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}. Trying again with the next display.");
                        return false;
                    }
                    else if (status == Status.InvalidArgument)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: One or more arguments passed in are invalid. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.ApiNotInitialized)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.NoImplementation)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.IncompatibleStructureVersion)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The version of the structure passed in is not compatible with this entrypoint. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.ModeChangeFailed)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: There was an error changing the display mode. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.Error)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Mosaic Display Grids! NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                        return false;
                    }*/


                    // If we get here then the display is valid, so now we actually apply the new Mosaic Topology
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Trying to set a 1x1 DisplayGrid for the NvAPI_Mosaic_SetDisplayGrids mosaic layout.");
                    status = NVAPI.SetDisplayGrids(individualScreensTopology, (UInt32)individualScreensTopology.Length, setTopoFlags);
                    if (status == Status.Ok)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_Mosaic_SetDisplayGrids returned OK.");
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Waiting 0.5 second to let the Mosaic display change take place before continuing");
                        System.Threading.Thread.Sleep(500);
                        logicalGPURefreshNeeded = true;
                    }
                    else if (status == Status.NVAPI_NO_ACTIVE_SLI_TOPOLOGY)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: No matching GPU topologies could be found. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.NVAPI_TOPO_NOT_POSSIBLE)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The topology passed in is not currently possible. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.InvalidDisplayId)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The Display ID of the first display is not currently possible to use. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}. Trying again with the next display.");
                        return false;
                    }
                    else if (status == Status.InvalidArgument)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: One or more arguments passed in are invalid. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.ApiNotInitialized)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.NoImplementation)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.IncompatibleStructureVersion)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The version of the structure passed in is not compatible with this entrypoint. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.ModeChangeFailed)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: There was an error changing the display mode. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.Error)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        return false;
                    }
                    else
                    {
                        // If we get here, we may have an error, or it may have worked successfully! So we need to check again :( 
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Mosaic Display Grids! NvAPI_Mosaic_SetDisplayGrids() returned error code {status}");
                        return false;
                    }

                    // If we get here, it may or it may not have worked successfully! So we need to check again :( 
                    // We don't want to do a full ceck, so we do a quick check instead.
                    if (MosaicIsOn())
                    {
                        // If the Mosaic is still on, then the last mosaic disable failed, so we need to then try turning it off this using NvAPI_Mosaic_EnableCurrentTopo(0)
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Previous attempt to turn off Mosaic. Now trying to use NvAPI_Mosaic_EnableCurrentTopo to disable Mosaic instead.");
                        uint enable = 0;
                        status = NVAPI.EnableCurrentTopo(enable);
                        if (status == Status.Ok)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_Mosaic_EnableCurrentTopo returned OK. Previously set Mosiac config now disabled");
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Waiting 0.5 second to let the Mosaic display change take place before continuing");
                            System.Threading.Thread.Sleep(500);
                        }
                        else if (status == Status.NotSupported)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.NVAPI_TOPO_NOT_POSSIBLE)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The topology passed in is not currently possible. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.InvalidArgument)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.ApiNotInitialized)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.NoImplementation)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.IncompatibleStructureVersion)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The version of the structure passed in is not compatible with this entrypoint. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.ModeChangeFailed)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: There was an error disabling the display mode. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                            return false;
                        }
                        else if (status == Status.Error)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                            return false;
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_EnableCurrentTopo() returned error code {status}");
                            return false;
                        }
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Mosaic successfully disabled using NvAPI_Mosaic_SetDisplayGrids method.");
                    }
                }
                else if (!displayConfig.MosaicConfig.IsMosaicEnabled && !ActiveDisplayConfig.MosaicConfig.IsMosaicEnabled)
                {
                    // We are on a non-Mosaic profile now, and we are changing to a non-Mosaic profile
                    // so there is nothing to do as far as NVIDIA is concerned!
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We are on a non-Mosaic profile now, and we are changing to a non-Mosaic profile so there is no need to modify Mosaic settings!");
                }

                // TODO - NvAPI_DISP_SetDisplayConfig isn't working at the moment and will always error with a InvalidArgument. It is related to the 
                // structure and the fact that sometimes it changes in size. C# structs aren't the best way to handle this, so moving to a class based system
                // seems like the best way forward. It's what Soroush Falahait did in the past. 

                /*// Now we set the NVIDIA Display Config (if we have one!)
                // If the display profile is a cloned config then NVIDIA GetDisplayConfig doesn't work
                // so we need to check for that. We just skip the SetDisplayConfig as it won't exist
                if (displayConfig.DisplayConfigs.Count > 0)
                {
                    status = NVAPI.SetDisplayConfig((UInt32)displayConfig.DisplayConfigs.Count, displayConfig.DisplayConfigs.ToArray(), NV_DISPLAYCONFIG_FLAGS.SAVE_TO_PERSISTENCE);
                    if (status == Status.Ok)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_DISP_SetDisplayConfig returned OK.");
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Waiting 0.5 second to let the Display Config layout change take place before continuing");
                        System.Threading.Thread.Sleep(500);
                    }
                    else if (status == Status.NVAPI_NO_ACTIVE_SLI_TOPOLOGY)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: No matching GPU topologies could be found. NvAPI_DISP_SetDisplayConfig() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.InvalidDisplayId)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The Display ID of the first display is not currently possible to use. NvAPI_DISP_SetDisplayConfig() returned error code {status}. Trying again with the next display.");
                        return false;
                    }
                    else if (status == Status.InvalidArgument)
                    {
                        // We sometimes get an invalid argument here if NVIDIA has just disolved a mosaic surround screen into indivudal screens
                        // THis is because if there are any additional screens from other adapters, nvidia tells windows to disable them
                        // We need to wait until the Windows library applies the screen before the DisplayConfig will be applied.
                        if (!displayConfig.MosaicConfig.IsMosaicEnabled && ActiveDisplayConfig.MosaicConfig.IsMosaicEnabled)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: NvAPI_DISP_SetDisplayConfig() returned error code {status}, but this is expected as we are changing from a Surround screen layout to a non-surround layout. Ignoring this error.");
                        }
                        else
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: One or more arguments passed in are invalid. NvAPI_DISP_SetDisplayConfig() returned error code {status}");
                            return false;
                        }
                    }
                    else if (status == Status.ApiNotInitialized)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_DISP_SetDisplayConfig() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.NoImplementation)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_DISP_SetDisplayConfig() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.IncompatibleStructureVersion)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The version of the structure passed in is not compatible with this entrypoint. NvAPI_DISP_SetDisplayConfig() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.ModeChangeFailed)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: There was an error changing the display mode. NvAPI_DISP_SetDisplayConfig() returned error code {status}");
                        return false;
                    }
                    else if (status == Status.Error)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_DISP_SetDisplayConfig() returned error code {status}");
                        return false;
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Setting the NVIDIA Display Config! NvAPI_DISP_SetDisplayConfig() returned error code {status}");
                        return false;
                    }
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Skipping setting the NVIDIA Display Config as there isn't one provided in the configuration.");
                }*/

                // If the NVIDIA topology has changed, then we need to refresh our active config so it stays valid. 
                if (logicalGPURefreshNeeded)
                {
                    UpdateActiveConfig();
                }


            }
            else
            {
                SharedLogger.logger.Info($"NVIDIALibrary/SetActiveConfig: Tried to run SetActiveConfig but the NVIDIA NvAPI library isn't initialised! This generally means you don't have a NVIDIA video card in your machine.");
                //throw new NVIDIALibraryException($"Tried to run SetActiveConfig but the NVIDIA NvAPI library isn't initialised!");
            }

            return true;
        }

        public bool SetActiveConfigOverride(NVIDIA_DISPLAY_CONFIG displayConfig)
        {

            if (_initialised)
            {
                // Force another scan of what the display config is so that the following logic works
                UpdateActiveConfig();

                Status status = Status.Error;


                // Go through the physical adapters
                foreach (var physicalGPU in displayConfig.PhysicalAdapters)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: Processing settings for Physical GPU #{physicalGPU.Key}");
                    NVIDIA_PER_ADAPTER_CONFIG myAdapter = physicalGPU.Value;
                    UInt32 myAdapterIndex = physicalGPU.Key;

                    foreach (var displayDict in myAdapter.Displays)
                    {
                        NVIDIA_PER_DISPLAY_CONFIG myDisplay = displayDict.Value;
                        UInt32 displayId = displayDict.Key;

                        // Now we try to set each display settings
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want to process settings for display {displayId}.");

                        if (!ActiveDisplayConfig.PhysicalAdapters[myAdapterIndex].Displays.ContainsKey(displayId))
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: Display {displayId} doesn't exist in this setup, so skipping overriding any NVIDIA display Settings.");
                            continue;
                        }

                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want to turn on colour if it's user set colour.");
                        // Now we try to set each display color

                        NV_COLOR_DATA_V5 colorData = myDisplay.ColorData;
                        try
                        {
                            // If this is a setting that says it uses user colour settings, then we turn it off
                            if (ActiveDisplayConfig.PhysicalAdapters[myAdapterIndex].Displays[displayId].ColorData.ColorSelectionPolicy != colorData.ColorSelectionPolicy)
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want to use custom NVIDIA HDR Colour for display {displayId}.");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want the standard colour settings to be {myDisplay.ColorData.ColorSelectionPolicy.ToString("G")} and they are {ActiveDisplayConfig.PhysicalAdapters[myAdapterIndex].Displays[displayId].ColorData.ColorSelectionPolicy.ToString("G")} for Mosaic display {displayId}.");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want to turn off standard colour mode for Mosaic display {displayId}.");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want standard colour settings Color selection policy {colorData.ColorSelectionPolicy.ToString("G")} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want standard colour settings BPC {colorData.Bpc} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want standard colour settings colour format {colorData.ColorFormat} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want standard colour settings colourimetry {colorData.Colorimetry} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want standard colour settings colour depth {colorData.Depth} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want standard colour settings dynamic range {colorData.DynamicRange} for Mosaic display {displayId}");

                                // Set the command as a 'SET'
                                colorData.Cmd = NV_COLOR_CMD.NV_COLOR_CMD_SET;
                                status = NVAPI.ColorControl(displayId, ref colorData);
                                if (status == Status.Ok)
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: NvAPI_Disp_ColorControl returned OK. BPC is set to {colorData.Bpc.ToString("G")}. Color Format is set to {colorData.ColorFormat.ToString("G")}. Colorimetry is set to {colorData.Colorimetry.ToString("G")}. Color Selection Policy is set to {colorData.ColorSelectionPolicy.ToString("G")}. Color Depth is set to {colorData.Depth.ToString("G")}. Dynamic Range is set to {colorData.DynamicRange.ToString("G")}");
                                    switch (colorData.ColorSelectionPolicy)
                                    {
                                        case NV_COLOR_SELECTION_POLICY.NV_COLOR_SELECTION_POLICY_USER:
                                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: Color Selection Policy is set to NV_COLOR_SELECTION_POLICY_USER so the color settings have been set by the user in the NVIDIA Control Panel.");
                                            break;
                                        case NV_COLOR_SELECTION_POLICY.NV_COLOR_SELECTION_POLICY_BEST_QUALITY: // Also matches NV_COLOR_SELECTION_POLICY_DEFAULT as it is 1
                                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: Color Selection Policy is set to NV_COLOR_SELECTION_POLICY_BEST_QUALITY so the color settings are being handled by the Windows OS.");
                                            break;
                                        case NV_COLOR_SELECTION_POLICY.NV_COLOR_SELECTION_POLICY_UNKNOWN:
                                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfigOverride: Color Selection Policy is set to NV_COLOR_SELECTION_POLICY_UNKNOWN so the color settings aren't being handled by either the Windows OS or the NVIDIA Setup!");
                                            break;
                                    }
                                }
                                else if (status == Status.NotSupported)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfigOverride: Your monitor {displayId} doesn't support the requested color settings. BPC = {colorData.Bpc.ToString("G")}. Color Format = {colorData.ColorFormat.ToString("G")}. Colorimetry = {colorData.Colorimetry.ToString("G")}. Color Selection Policy = {colorData.ColorSelectionPolicy.ToString("G")}. Color Depth = {colorData.Depth.ToString("G")}. Dynamic Range = {colorData.DynamicRange.ToString("G")}. NvAPI_Disp_ColorControl() returned error code {status}");
                                }
                                else if (status == Status.InsufficientBuffer)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfigOverride: The input buffer is not large enough to hold it's contents. NvAPI_Disp_ColorControl() returned error code {status}");
                                }
                                else if (status == Status.InvalidDisplayId)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfigOverride: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_Disp_ColorControl() returned error code {status}");
                                }
                                else if (status == Status.ApiNotInitialized)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfigOverride: The NvAPI API needs to be initialized first. NvAPI_Disp_ColorControl() returned error code {status}");
                                }
                                else if (status == Status.NoImplementation)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfigOverride: This entry point not available in this NVIDIA Driver. NvAPI_Disp_ColorControl() returned error code {status}");
                                }
                                else if (status == Status.Error)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfigOverride: A miscellaneous error occurred. NvAPI_Disp_ColorControl() returned error code {status}");
                                }
                                else
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: Some non standard error occurred while seting the color settings! NvAPI_Disp_ColorControl() returned error code {status}. It's most likely that your monitor {displayId} doesn't support this color mode.");
                                }
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want only want to turn on custom NVIDIA colour settings if needed for display {displayId}, and that currently isn't required. Skipping changing NVIDIA colour mode.");
                            }
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Error(ex, $"NVIDIALibrary/SetActiveConfigOverride: Exception caused while turning on NVIDIA custom colour settings for display {displayId}.");
                        }



                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want to turn on NVIDIA HDR colour if it's user wants to use NVIDIA HDR colour.");
                        // Now we try to set each display color

                        NV_HDR_COLOR_DATA_V2 hdrColorData = myDisplay.HdrColorData;
                        try
                        {

                            // if it's HDR and it's a different mode than what we are in now, then set HDR
                            if (ActiveDisplayConfig.PhysicalAdapters[myAdapterIndex].Displays[displayId].HdrColorData.HdrMode != hdrColorData.HdrMode)
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want to turn on user-set HDR mode for display {displayId} as it's supposed to be on.");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: HDR mode is currently {ActiveDisplayConfig.PhysicalAdapters[myAdapterIndex].Displays[displayId].HdrColorData.HdrMode.ToString("G")} for Mosaic display {displayId}.");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want HDR settings BPC  {hdrColorData.HdrBpc} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want HDR settings HDR Colour Format {hdrColorData.HdrColorFormat} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want HDR settings HDR dynamic range {hdrColorData.HdrDynamicRange} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want HDR settings HDR Mode {hdrColorData.HdrMode} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want HDR settings Mastering Display Data {hdrColorData.MasteringDisplayData} for Mosaic display {displayId}");
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want HDR settings Static Meradata Description ID {hdrColorData.StaticMetadataDescriptorId} for Mosaic display {displayId}");
                                // Apply the HDR removal
                                hdrColorData.Cmd = NV_HDR_CMD.CMD_SET;
                                status = NVAPI.HdrColorControl(displayId, ref hdrColorData);
                                if (status == Status.Ok)
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: NvAPI_Disp_HdrColorControl returned OK. We just successfully turned off the HDR mode for Mosaic display {displayId}.");
                                }
                                else if (status == Status.InsufficientBuffer)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfigOverride: The input buffer is not large enough to hold it's contents. NvAPI_Disp_HdrColorControl() returned error code {status}");
                                }
                                else if (status == Status.InvalidDisplayId)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfigOverride: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_Disp_HdrColorControl() returned error code {status}");
                                }
                                else if (status == Status.ApiNotInitialized)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfigOverride: The NvAPI API needs to be initialized first. NvAPI_Disp_HdrColorControl() returned error code {status}");
                                }
                                else if (status == Status.NoImplementation)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfigOverride: This entry point not available in this NVIDIA Driver. NvAPI_Disp_HdrColorControl() returned error code {status}");
                                }
                                else if (status == Status.Error)
                                {
                                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfigOverride: A miscellaneous error occurred. NvAPI_Disp_HdrColorControl() returned error code {status}");
                                }
                                else
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: Some non standard error occurred while getting Mosaic Topology! NvAPI_Disp_HdrColorControl() returned error code {status}. It's most likely that your monitor {displayId} doesn't support HDR.");
                                }
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfigOverride: We want only want to turn on custom NVIDIA HDR if needed for display {displayId} and that currently isn't required. Skipping changing NVIDIA HDR mode.");
                            }
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Error(ex, $"NVIDIALibrary/SetActiveConfigOverride: Exception caused while turning on custom NVIDIA HDR colour settings for display {displayId}.");
                        }

                        // Disabled the Adaptive Sync equality matching as we are having trouble applying it, which is causing issues in profile matching in DisplayMagician
                        // To fix this bit, we need to test the SetActiveConfigOverride Adaptive Sync part of the codebase to apply this properly.
                        // But for now, we'll exclude it from the equality matching and also stop trying to use the adaptive sync config.

                        /*// Set any AdaptiveSync settings
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want to set any adaptive Sync settings if in use.");

                        NV_SET_ADAPTIVE_SYNC_DATA_V1 adaptiveSyncData = myDisplay.AdaptiveSyncConfig;
                        try
                        {
                            if (myDisplay.AdaptiveSyncConfig.DisableAdaptiveSync)
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want to DISABLE Adaptive Sync for display {displayId}.");
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want to ENABLE Adaptive Sync for display {displayId}.");
                            }

                            if (myDisplay.AdaptiveSyncConfig.DisableFrameSplitting)
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want to DISABLE Frame Splitting for display {displayId}.");
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want to ENABLE Frame Splitting for display {displayId}.");
                            }
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We want to set the Adaptice Sync Max Frame Interval to {myDisplay.AdaptiveSyncConfig.MaxFrameInterval}ms for display {displayId}.");

                            // Apply the AdaptiveSync settings
                            status = NVAPI.SetAdaptiveSyncData(displayId, ref adaptiveSyncData);
                            if (status == Status.Ok)
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_DISP_SetAdaptiveSyncData returned OK. We just successfully set the Adaptive Sync settings for display {displayId}.");
                            }
                            else if (status == Status.InsufficientBuffer)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The input buffer is not large enough to hold it's contents. NvAPI_DISP_SetAdaptiveSyncData() returned error code {status}");
                            }
                            else if (status == Status.InvalidDisplayId)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_DISP_SetAdaptiveSyncData() returned error code {status}");
                            }
                            else if (status == Status.ApiNotInitialized)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_DISP_SetAdaptiveSyncData() returned error code {status}");
                            }
                            else if (status == Status.NoImplementation)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_DISP_SetAdaptiveSyncData() returned error code {status}");
                            }
                            else if (status == Status.Error)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_DISP_SetAdaptiveSyncData() returned error code {status}");
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_DISP_SetAdaptiveSyncData() returned error code {status}. It's most likely that your monitor {displayId} doesn't support HDR.");
                            }

                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Error(ex, $"NVIDIALibrary/SetActiveConfig: Exception caused while trying to set NVIDIA Adaptive Sync settings for display {displayId}.");
                        }*/
                    }


                }

            }
            else
            {
                SharedLogger.logger.Info($"NVIDIALibrary/SetActiveConfigOverride: Tried to run SetActiveConfig but the NVIDIA NVAPI library isn't initialised! This generally means you don't have a NVIDIA video card in your machine.");
                //throw new NVIDIALibraryException($"Tried to run SetActiveConfigOverride but the NVIDIA NVAPI library isn't initialised!");
            }

            return true;
        }

        public bool IsActiveConfig(NVIDIA_DISPLAY_CONFIG displayConfig)
        {
            // Check whether the display config is in use now
            SharedLogger.logger.Trace($"NVIDIALibrary/IsActiveConfig: Checking whether the display configuration is already being used.");
            if (displayConfig.Equals(_activeDisplayConfig))
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/IsActiveConfig: The display configuration is already being used (supplied displayConfig Equals currentDisplayConfig");
                return true;
            }
            else
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/IsActiveConfig: The display configuration is NOT currently in use (supplied displayConfig does NOT equal currentDisplayConfig");
                return false;
            }

        }

        public bool IsValidConfig(NVIDIA_DISPLAY_CONFIG displayConfig)
        {
            // We want to check the NVIDIA Surround (Mosaic) config is valid
            SharedLogger.logger.Trace($"NVIDIALibrary/IsValidConfig: Testing whether the display configuration is valid");
            // 
            if (displayConfig.MosaicConfig.IsMosaicEnabled)
            {

                // ===================================================================================================================================
                // Important! ValidateDisplayGrids does not work at the moment. It errors when supplied with a Grid Topology that works in SetDisplaGrids
                // We therefore cannot use ValidateDisplayGrids to actually validate the config before it's use. We instead need to rely on SetDisplaGrids reporting an
                // error if it is unable to apply the requested configuration. While this works fine, it's not optimal.
                // TODO: Test ValidateDisplayGrids in a future NVIDIA driver release to see if they fixed it.
                // ===================================================================================================================================
                return true;

                /*// Figure out how many Mosaic Grid topoligies there are                    
                uint mosaicGridCount = 0;
                Status status = NVAPI.EnumDisplayGrids(ref mosaicGridCount);
                if (status == Status.Ok)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");
                }

                // Get Current Mosaic Grid settings using the Grid topologies fnumbers we got before
                //NV_MOSAIC_GRID_TOPO_V2[] mosaicGridTopos = new NV_MOSAIC_GRID_TOPO_V2[mosaicGridCount];
                NV_MOSAIC_GRID_TOPO_V1[] mosaicGridTopos = new NV_MOSAIC_GRID_TOPO_V1[mosaicGridCount];
                status = NVAPI.EnumDisplayGrids(ref mosaicGridTopos, ref mosaicGridCount);
                if (status == Status.Ok)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");
                }
                else if (status == Status.NotSupported)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_GetCurrentTopo() returned error code {status}");
                }
                else if (status == Status.InvalidArgument)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_GetCurrentTopo() returned error code {status}");
                }
                else if (status == Status.ApiNotInitialized)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_GetCurrentTopo() returned error code {status}");
                }
                else if (status == Status.NoImplementation)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_GetCurrentTopo() returned error code {status}");
                }
                else if (status == Status.Error)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_Mosaic_GetCurrentTopo() returned error code {status}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_GetCurrentTopo() returned error code {status}");
                }
                */

                /*NV_MOSAIC_SETDISPLAYTOPO_FLAGS setTopoFlags = NV_MOSAIC_SETDISPLAYTOPO_FLAGS.NONE;
                bool topoValid = false;
                NV_MOSAIC_DISPLAY_TOPO_STATUS_V1[] topoStatuses = new NV_MOSAIC_DISPLAY_TOPO_STATUS_V1[displayConfig.MosaicConfig.MosaicGridCount];
                Status status = NVAPI.ValidateDisplayGrids(setTopoFlags, ref displayConfig.MosaicConfig.MosaicGridTopos, ref topoStatuses, displayConfig.MosaicConfig.MosaicGridCount);
                //NV_MOSAIC_DISPLAY_TOPO_STATUS_V1[] topoStatuses = new NV_MOSAIC_DISPLAY_TOPO_STATUS_V1[mosaicGridCount];
                //status = NVAPI.ValidateDisplayGrids(setTopoFlags, ref mosaicGridTopos, ref topoStatuses, mosaicGridCount);
                if (status == Status.Ok)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");

                    for (int i = 0; i < topoStatuses.Length; i++)
                    {
                        // If there is an error then we need to log it!
                        // And make it not be used
                        if (topoStatuses[i].ErrorFlags == NV_MOSAIC_DISPLAYCAPS_PROBLEM_FLAGS.OK)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Congratulations! No error flags for GridTopology #{i}");
                            topoValid = true;
                        }
                        else if (topoStatuses[i].ErrorFlags == NV_MOSAIC_DISPLAYCAPS_PROBLEM_FLAGS.DISPLAY_ON_INVALID_GPU)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: Error with the GridTopology #{i}: Display is on an invalid GPU");
                        }
                        else if (topoStatuses[i].ErrorFlags == NV_MOSAIC_DISPLAYCAPS_PROBLEM_FLAGS.DISPLAY_ON_WRONG_CONNECTOR)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: Error with the GridTopology #{i}: Display is on the wrong connection. It was on a different connection when the display profile was saved.");
                        }
                        else if (topoStatuses[i].ErrorFlags == NV_MOSAIC_DISPLAYCAPS_PROBLEM_FLAGS.ECC_ENABLED)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: Error with the GridTopology #{i}: ECC has been enabled, and Mosaic/Surround doesn't work with ECC");
                        }
                        else if (topoStatuses[i].ErrorFlags == NV_MOSAIC_DISPLAYCAPS_PROBLEM_FLAGS.GPU_TOPOLOGY_NOT_SUPPORTED)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: Error with the GridTopology #{i}: This GPU topology is not supported.");
                        }
                        else if (topoStatuses[i].ErrorFlags == NV_MOSAIC_DISPLAYCAPS_PROBLEM_FLAGS.MISMATCHED_OUTPUT_TYPE)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: Error with the GridTopology #{i}: The output type has changed for the display. The display was connected through another output type when the display profile was saved.");
                        }
                        else if (topoStatuses[i].ErrorFlags == NV_MOSAIC_DISPLAYCAPS_PROBLEM_FLAGS.NOT_SUPPORTED)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: Error with the GridTopology #{i}: This Grid Topology is not supported on this video card.");
                        }
                        else if (topoStatuses[i].ErrorFlags == NV_MOSAIC_DISPLAYCAPS_PROBLEM_FLAGS.NO_COMMON_TIMINGS)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: Error with the GridTopology #{i}: Couldn't find common timings that suit all the displays in this Grid Topology.");
                        }
                        else if (topoStatuses[i].ErrorFlags == NV_MOSAIC_DISPLAYCAPS_PROBLEM_FLAGS.NO_DISPLAY_CONNECTED)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: Error with the GridTopology #{i}: No display connected.");
                        }
                        else if (topoStatuses[i].ErrorFlags == NV_MOSAIC_DISPLAYCAPS_PROBLEM_FLAGS.NO_EDID_AVAILABLE)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: Error with the GridTopology #{i}: Your display didn't provide any information when we attempted to query it. Your display either doesn't support support EDID querying or has it a fault. ");
                        }
                        else if (topoStatuses[i].ErrorFlags == NV_MOSAIC_DISPLAYCAPS_PROBLEM_FLAGS.NO_GPU_TOPOLOGY)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: Error with the GridTopology #{i}: There is no GPU topology provided.");
                        }
                        else if (topoStatuses[i].ErrorFlags == NV_MOSAIC_DISPLAYCAPS_PROBLEM_FLAGS.NO_SLI_BRIDGE)
                        {
                            SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: Error with the GridTopology #{i}: There is no SLI bridge, and there was one when the display profile was created.");
                        }

                        // And now we also check to see if there are any warnings we also need to log
                        if (topoStatuses[i].WarningFlags == NV_MOSAIC_DISPLAYTOPO_WARNING_FLAGS.NONE)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Congratulations! No warning flags for GridTopology #{i}");
                        }
                        else if (topoStatuses[i].WarningFlags == NV_MOSAIC_DISPLAYTOPO_WARNING_FLAGS.DISPLAY_POSITION)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: Warning for the GridTopology #{i}: The display position has changed, and this may affect your display view.");
                        }
                        else if (topoStatuses[i].WarningFlags == NV_MOSAIC_DISPLAYTOPO_WARNING_FLAGS.DRIVER_RELOAD_REQUIRED)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: Warning for the GridTopology #{i}: Your computer needs to be restarted before your NVIDIA device driver can use this Grid Topology.");
                        }
                    }

                }
                else if (status == Status.NotSupported)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                }
                else if (status == Status.NVAPI_NO_ACTIVE_SLI_TOPOLOGY)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: No matching GPU topologies could be found. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                }
                else if (status == Status.NVAPI_TOPO_NOT_POSSIBLE)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The topology passed in is not currently possible. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                }
                else if (status == Status.InvalidArgument)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                }
                else if (status == Status.ApiNotInitialized)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                }
                else if (status == Status.NoImplementation)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                }
                else if (status == Status.IncompatibleStructureVersion)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The version of the structure passed in is not compatible with this entrypoint. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                }
                else if (status == Status.ModeChangeFailed)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: There was an error changing the display mode. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                }
                else if (status == Status.Error)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_ValidateDisplayGrids() returned error code {status}");
                }


                // Cancel the screen change if there was an error with anything above this.
                if (topoValid)
                {
                    // If there was an issue then we need to return false
                    // to indicate that the display profile can't be applied
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: The display settings are valid.");
                    return true;
                }
                else
                {
                    // If there was an issue then we need to return false
                    // to indicate that the display profile can't be applied
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: There was an error when validating the requested grid topology that prevents us from using the display settings provided. THe display setttings are NOT valid.");
                    return false;
                }*/
            }
            else
            {
                // Its not a Mosaic topology, so we just let it pass, as it's windows settings that matter.
                return true;
            }
        }

        public bool IsPossibleConfig(NVIDIA_DISPLAY_CONFIG displayConfig)
        {
            // We want to check the NVIDIA profile can be used now
            SharedLogger.logger.Trace($"NVIDIALibrary/IsPossibleConfig: Testing whether the NVIDIA display configuration is possible to be used now");

            // CHeck that we have all the displayConfig DisplayIdentifiers we need available now
            if (displayConfig.DisplayIdentifiers.All(value => _allConnectedDisplayIdentifiers.Contains(value)))
            //if (currentAllIds.Intersect(displayConfig.DisplayIdentifiers).Count() == displayConfig.DisplayIdentifiers.Count)
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/IsPossibleConfig: Success! The NVIDIA display configuration is possible to be used now");
                return true;
            }
            else
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/IsPossibleConfig: Uh oh! The NVIDIA display configuration is possible cannot be used now");
                return false;
            }

        }

        public static bool MosaicIsOn()
        {
            PhysicalGpuHandle[] physicalGpus = new PhysicalGpuHandle[NVImport.NVAPI_MAX_PHYSICAL_GPUS];
            uint physicalGpuCount = 0;
            Status status = NVImport.NvAPI_EnumPhysicalGPUs(ref physicalGpus, out physicalGpuCount);
            if (status == Status.Ok)
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_EnumPhysicalGPUs returned {physicalGpuCount} Physical GPUs");
            }
            else
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Error getting physical GPU count. NvAPI_EnumPhysicalGPUs() returned error code {status}");
            }

            // Go through the Physical GPUs one by one
            for (uint physicalGpuIndex = 0; physicalGpuIndex < physicalGpuCount; physicalGpuIndex++)
            {
                // Get current Mosaic Topology settings in brief (check whether Mosaic is on)
                NV_MOSAIC_TOPO_BRIEF mosaicTopoBrief = new NV_MOSAIC_TOPO_BRIEF();
                NV_MOSAIC_DISPLAY_SETTING_V2 mosaicDisplaySetting = new NV_MOSAIC_DISPLAY_SETTING_V2();
                int mosaicOverlapX = 0;
                int mosaicOverlapY = 0;
                status = NVAPI.GetCurrentTopo(ref mosaicTopoBrief, ref mosaicDisplaySetting, out mosaicOverlapX, out mosaicOverlapY);
                if (status == Status.Ok)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");
                }
                else if (status == Status.NotSupported)
                {
                    SharedLogger.logger.Debug($"NVIDIALibrary/GetNVIDIADisplayConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_GetCurrentTopo() returned error code {status}");
                }
                else if (status == Status.InvalidArgument)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_GetCurrentTopo() returned error code {status}");
                }
                else if (status == Status.ApiNotInitialized)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_GetCurrentTopo() returned error code {status}");
                }
                else if (status == Status.NoImplementation)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_GetCurrentTopo() returned error code {status}");
                }
                else if (status == Status.Error)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_Mosaic_GetCurrentTopo() returned error code {status}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_GetCurrentTopo() returned error code {status}");
                }

                // Check if there is a topology and that Mosaic is enabled
                if (mosaicTopoBrief.Topo != NV_MOSAIC_TOPO.TOPO_NONE && mosaicTopoBrief.Enabled == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public List<string> GetCurrentDisplayIdentifiers()
        {
            SharedLogger.logger.Trace($"NVIDIALibrary/GetCurrentDisplayIdentifiers: Getting the current display identifiers for the displays in use now");
            return GetSomeDisplayIdentifiers(false);
        }

        public List<string> GetAllConnectedDisplayIdentifiers()
        {
            SharedLogger.logger.Trace($"NVIDIALibrary/GetAllConnectedDisplayIdentifiers: Getting all the display identifiers that can possibly be used");
            _allConnectedDisplayIdentifiers = GetSomeDisplayIdentifiers(true);

            return _allConnectedDisplayIdentifiers;
        }

        private List<string> GetSomeDisplayIdentifiers(bool allDisplays = true)
        {
            SharedLogger.logger.Debug($"NVIDIALibrary/GetCurrentDisplayIdentifiers: Generating the unique Display Identifiers for the currently active configuration");

            List<string> displayIdentifiers = new List<string>();

            // Enumerate all the Physical GPUs
            PhysicalGpuHandle[] physicalGpus = new PhysicalGpuHandle[NVImport.NV_MAX_PHYSICAL_GPUS];
            uint physicalGpuCount = 0;
            Status status = NVImport.NvAPI_EnumPhysicalGPUs(ref physicalGpus, out physicalGpuCount);
            if (status == Status.Ok)
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: NvAPI_EnumPhysicalGPUs returned {physicalGpuCount} Physical GPUs");
            }
            else
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Error getting physical GPU count. NvAPI_EnumPhysicalGPUs() returned error code {status}");
            }

            // This check is to make sure that if there aren't any physical GPUS then we exit!
            if (physicalGpuCount == 0)
            {
                // If there aren't any video cards detected, then return that empty list.
                SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: No Videocards detected so returning empty list");
                return new List<string>();
            }

            // Go through the Physical GPUs one by one
            for (uint physicalGpuIndex = 0; physicalGpuIndex < physicalGpuCount; physicalGpuIndex++)
            {
                //We want to get the name of the physical device
                string gpuName = "";
                status = NVAPI.GetFullName(physicalGpus[physicalGpuIndex], ref gpuName);
                if (status == Status.Ok)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: NvAPI_GPU_GetFullName returned OK. The GPU Full Name is {gpuName}");
                }
                else if (status == Status.NotSupported)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: Mosaic is not supported with the existing hardware. NvAPI_GPU_GetFullName() returned error code {status}");
                }
                else if (status == Status.InvalidArgument)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: One or more argumentss passed in are invalid. NvAPI_GPU_GetFullName() returned error code {status}");
                }
                else if (status == Status.ApiNotInitialized)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The NvAPI API needs to be initialized first. NvAPI_GPU_GetFullName() returned error code {status}");
                }
                else if (status == Status.NoImplementation)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: This entry point not available in this NVIDIA Driver. NvAPI_GPU_GetFullName() returned error code {status}");
                }
                else if (status == Status.Error)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: A miscellaneous error occurred. NvAPI_GPU_GetFullName() returned error code {status}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting the GPU full name! NvAPI_GPU_GetFullName() returned error code {status}");
                }

                //We want to get the physical details of the physical device
                NV_GPU_BUS_TYPE busType = NV_GPU_BUS_TYPE.UNDEFINED;
                status = NVAPI.GetBusType(physicalGpus[physicalGpuIndex], ref busType);
                if (status == Status.Ok)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: NvAPI_GPU_GetBoardInfo returned OK. THe GPU BusType is {busType.ToString("G")}");
                }
                else if (status == Status.NotSupported)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: Mosaic is not supported with the existing hardware. NvAPI_GPU_GetBoardInfo() returned error code {status}");
                }
                else if (status == Status.InvalidArgument)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: One or more argumentss passed in are invalid. NvAPI_GPU_GetBoardInfo() returned error code {status}");
                }
                else if (status == Status.ApiNotInitialized)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The NvAPI API needs to be initialized first. NvAPI_GPU_GetBoardInfo() returned error code {status}");
                }
                else if (status == Status.NoImplementation)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: This entry point not available in this NVIDIA Driver. NvAPI_GPU_GetBoardInfo() returned error code {status}");
                }
                else if (status == Status.Error)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: A miscellaneous error occurred. NvAPI_GPU_GetBoardInfo() returned error code {status}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Some non standard error occurred while getting Mosaic Topology! NvAPI_GPU_GetBoardInfo() returned error code {status}");
                }

                //We want to get the physical details of the physical device
                UInt32 busId = 0;
                status = NVAPI.GetBusId(physicalGpus[physicalGpuIndex], ref busId);
                if (status == Status.Ok)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: NvAPI_GPU_GetBusId returned OK. The GPU Bus ID was {busId}");
                }
                else if (status == Status.NotSupported)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: Mosaic is not supported with the existing hardware. NvAPI_GPU_GetBusId() returned error code {status}");
                }
                else if (status == Status.InvalidArgument)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: One or more argumentss passed in are invalid. NvAPI_GPU_GetBusId() returned error code {status}");
                }
                else if (status == Status.ApiNotInitialized)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The NvAPI API needs to be initialized first. NvAPI_GPU_GetBusId() returned error code {status}");
                }
                else if (status == Status.NoImplementation)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: This entry point not available in this NVIDIA Driver. NvAPI_GPU_GetBusId() returned error code {status}");
                }
                else if (status == Status.Error)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: A miscellaneous error occurred. NvAPI_GPU_GetBusId() returned error code {status}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Some non standard error occurred while getting Mosaic Topology! NvAPI_GPU_GetBusId() returned error code {status}");
                }

                // Next, we need to get all the connected Display IDs. 
                //This function retrieves the number of display IDs we know about
                UInt32 displayCount = 0;
                status = NVAPI.GetConnectedDisplayIds(physicalGpus[physicalGpuIndex], ref displayCount, 0);
                if (status == Status.Ok)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: NvAPI_DISP_GetGDIPrimaryDisplayId returned OK. We have {displayCount} connected displays detected.");
                }
                else if (status == Status.InsufficientBuffer)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The input buffer is not large enough to hold it's contents. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                }
                else if (status == Status.InvalidDisplayId)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                }
                else if (status == Status.ApiNotInitialized)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The NvAPI API needs to be initialized first. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                }
                else if (status == Status.NoImplementation)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: This entry point not available in this NVIDIA Driver. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                }
                else if (status == Status.Error)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: A miscellaneous error occurred. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Some non standard error occurred while getting Mosaic Topology! NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                }

                if (displayCount > 0)
                {
                    // Now we try to get the information about the displayIDs
                    NV_GPU_DISPLAYIDS_V2[] displayIds = new NV_GPU_DISPLAYIDS_V2[displayCount];
                    status = NVAPI.GetConnectedDisplayIds(physicalGpus[physicalGpuIndex], ref displayIds, ref displayCount, 0);
                    if (status == Status.Ok)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: NvAPI_GPU_GetConnectedDisplayIds returned OK. We have {displayCount} physical GPUs");
                    }
                    else if (status == Status.InsufficientBuffer)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The input buffer is not large enough to hold it's contents. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                    }
                    else if (status == Status.InvalidDisplayId)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                    }
                    else if (status == Status.ApiNotInitialized)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The NvAPI API needs to be initialized first. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                    }
                    else if (status == Status.NoImplementation)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: This entry point not available in this NVIDIA Driver. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                    }
                    else if (status == Status.Error)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: A miscellaneous error occurred. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Some non standard error occurred while getting Mosaic Topology! NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {status}");
                    }

                    // Now, we want to go through the displays 
                    foreach (NV_GPU_DISPLAYIDS_V2 oneDisplay in displayIds)
                    {
                        // If alldisplays is false, then we only want the active displays. We need to skip this one if it is not active
                        if (allDisplays == false && oneDisplay.IsActive == false)
                        {
                            // We want to skip this display as it is non-active, and we only want active displays
                            continue;
                        }


                        // Now we try to get the GPU and Output ID from the DisplayID
                        PhysicalGpuHandle physicalGpu = new PhysicalGpuHandle();
                        UInt32 gpuOutputId = 0;
                        status = NVImport.NvAPI_SYS_GetGpuAndOutputIdFromDisplayId(oneDisplay.DisplayId, out physicalGpu, out gpuOutputId);
                        if (status == Status.Ok)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: NvAPI_SYS_GetGpuAndOutputIdFromDisplayId returned OK. We received Physical GPU ID {physicalGpu} and GPU Output ID {gpuOutputId}");
                        }
                        else if (status == Status.InsufficientBuffer)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The input buffer is not large enough to hold it's contents. NvAPI_SYS_GetGpuAndOutputIdFromDisplayId() returned error code {status}");
                        }
                        else if (status == Status.InvalidDisplayId)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                        }
                        else if (status == Status.ApiNotInitialized)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The NvAPI API needs to be initialized first. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                        }
                        else if (status == Status.NoImplementation)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: This entry point not available in this NVIDIA Driver. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                        }
                        else if (status == Status.Error)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: A miscellaneous error occurred. NvAPI_GPU_GetConnectedDisplayIds() returned error code {status}");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Some non standard error occurred while getting Mosaic Topology! NvAPI_SYS_GetGpuAndOutputIdFromDisplayId() returned error code {status}");
                        }

                        // Lets set some EDID default in case the EDID doesn't work
                        string manufacturerName = "Unknown";
                        UInt32 productCode = 0;
                        UInt32 serialNumber = 0;
                        // We try to get an EDID block and extract the info                        
                        NV_EDID_V3 edidInfo = new NV_EDID_V3();
                        status = NVAPI.GetEDID(physicalGpu, gpuOutputId, ref edidInfo);
                        if (status == Status.Ok)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: NvAPI_GPU_GetEDID returned OK. We have got an EDID Block.");
                            EDID edidParsedInfo = new EDID(edidInfo.EDID_Data);
                            manufacturerName = edidParsedInfo.ManufacturerCode;
                            productCode = edidParsedInfo.ProductCode;
                            serialNumber = edidParsedInfo.SerialNumber;
                        }
                        else
                        {
                            if (status == Status.InvalidArgument)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: Either edidInfo was null when it was supplied, or gpuOutputId . NvAPI_GPU_GetEDID() returned status  code {status}");
                            }
                            else if (status == Status.NvidiaDeviceNotFound)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: No active GPU was found. NvAPI_GPU_GetEDID() returned status  code {status}");
                            }
                            else if (status == Status.NVAPI_EXPECTED_PHYSICAL_GPU_HANDLE)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The GPU Handle supplied was not a valid GPU Handle. NvAPI_GPU_GetEDID() returned status  code {status}");
                            }
                            else if (status == Status.NVAPI_DATA_NOT_FOUND)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The display does not support EDID. NvAPI_GPU_GetEDID() returned status code {status}");
                            }
                            else if (status == Status.ApiNotInitialized)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The NvAPI API needs to be initialized first. NvAPI_GPU_GetEDID() returned status  code {status}");
                            }
                            else if (status == Status.NoImplementation)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: This entry point not available in this NVIDIA Driver. NvAPI_GPU_GetEDID() returned status  code {status}");
                            }
                            else if (status == Status.Error)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: A miscellaneous error occurred. NvAPI_GPU_GetEDID() returned error code {status}");
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Some non standard error occurred while getting Mosaic Topology! NvAPI_GPU_GetEDID() returned error code {status}");
                            }
                        }


                        // Create an array of all the important display info we need to record
                        List<string> displayInfo = new List<string>();
                        displayInfo.Add("NVIDIA");
                        try
                        {
                            displayInfo.Add(gpuName.ToString());
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Warn(ex, $"NVIDIALibrary/GetSomeDisplayIdentifiers: Exception getting GPU Name from video card. Substituting with a # instead");
                            displayInfo.Add("#");
                        }
                        try
                        {
                            displayInfo.Add(busType.ToString());
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Warn(ex, $"NVIDIALibrary/GetSomeDisplayIdentifiers: Exception getting GPU Bus Type from video card. Substituting with a # instead");
                            displayInfo.Add("#");
                        }
                        try
                        {
                            displayInfo.Add(busId.ToString());
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Warn(ex, $"NVIDIALibrary/GetSomeDisplayIdentifiers: Exception getting GPU Bus ID from video card. Substituting with a # instead");
                            displayInfo.Add("#");
                        }
                        try
                        {
                            displayInfo.Add(oneDisplay.ConnectorType.ToString("G"));
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Warn(ex, $"NVIDIALibrary/GetSomeDisplayIdentifiers: Exception getting GPU Output ID from video card. Substituting with a # instead");
                            displayInfo.Add("#");
                        }
                        try
                        {
                            displayInfo.Add(manufacturerName.ToString());
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Warn(ex, $"NVIDIALibrary/GetSomeDisplayIdentifiers: Exception getting NVIDIA EDID Manufacturer Name for the display from video card. Substituting with a # instead");
                            displayInfo.Add("#");
                        }
                        try
                        {
                            displayInfo.Add(productCode.ToString());
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Warn(ex, $"NVIDIALibrary/GetSomeDisplayIdentifiers: Exception getting NVIDIA EDID Product Code for the display from video card. Substituting with a # instead");
                            displayInfo.Add("#");
                        }
                        try
                        {
                            displayInfo.Add(serialNumber.ToString());
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Warn(ex, $"NVIDIALibrary/GetSomeDisplayIdentifiers: Exception getting NVIDIA EDID Serial Number for the display from video card. Substituting with a # instead");
                            displayInfo.Add("#");
                        }
                        try
                        {
                            displayInfo.Add(oneDisplay.DisplayId.ToString());
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Warn(ex, $"NVIDIALibrary/GetSomeDisplayIdentifiers: Exception getting Display ID from video card. Substituting with a # instead");
                            displayInfo.Add("#");
                        }
                        // Create a display identifier out of it
                        string displayIdentifier = String.Join("|", displayInfo);
                        // Add it to the list of display identifiers so we can return it
                        // but only add it if it doesn't already exist. Otherwise we get duplicates :/
                        if (!displayIdentifiers.Contains(displayIdentifier))
                        {
                            displayIdentifiers.Add(displayIdentifier);
                            SharedLogger.logger.Debug($"NVIDIALibrary/GetSomeDisplayIdentifiers: DisplayIdentifier detected: {displayIdentifier}");
                        }
                    }
                }
            }

            // Sort the display identifiers
            displayIdentifiers.Sort();

            return displayIdentifiers;
        }

        public static string DumpAllDRSSettings()
        {
            // This bit of code dumps all the profiles in the DRS, and all the settings within that
            // This is really only used for debugging, but is still very useful to have!
            // Get the DRS Settings
            string stringToReturn = "";
            stringToReturn += $"\n****** CURRENTLY SET NVIDIA DRIVER SETTINGS (DRS) *******\n";

            NvDRSSessionHandle drsSessionHandle = new NvDRSSessionHandle();
            Status status;
            status = NVAPI.CreateSession(out drsSessionHandle);
            if (status == Status.Ok)
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/DumpAllDRSSettings: NvAPI_DRS_CreateSession returned OK. We got a DRS Session Handle");
                try
                {
                    // Load the DRS Settings into memory
                    status = NVAPI.LoadSettings(drsSessionHandle);
                    if (status == Status.Ok)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/DumpAllDRSSettings: NvAPI_DRS_LoadSettings returned OK. We successfully loaded the DRS Settings into memory.");
                    }
                    else if (status == Status.NvidiaDeviceNotFound)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_LoadSettings() returned error code {status}");
                    }
                    else if (status == Status.InvalidArgument)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: One or more args passed in are invalid. NvAPI_DRS_LoadSettings() returned error code {status}");
                    }
                    else if (status == Status.ApiNotInitialized)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: The NvAPI API needs to be initialized first. NvAPI_DRS_LoadSettings() returned error code {status}");
                    }
                    else if (status == Status.NoImplementation)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: This entry point not available in this NVIDIA Driver. NvAPI_DRS_LoadSettings() returned error code {status}");
                    }
                    else if (status == Status.Error)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: A miscellaneous error occurred whist destroying our DRS Session Handle. NvAPI_DRS_LoadSettings() returned error code {status}");
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/DumpAllDRSSettings: Some non standard error occurred while destroying our DRS Session Handle! NvAPI_DRS_GetProfileInfo() returned error code {status}");
                    }

                    // Get ALL available settings
                    UInt32 drsNumAvailableSettingIds = NVImport.NVAPI_SETTING_MAX_VALUES;
                    UInt32[] drsSettingIds = new UInt32[drsNumAvailableSettingIds];
                    status = NVAPI.EnumAvailableSettingIds(ref drsSettingIds, ref drsNumAvailableSettingIds);
                    if (status == Status.Ok)
                    {
                        int settingCount = 1;
                        foreach (var drsSettingId in drsSettingIds)
                        {
                            if (settingCount > drsNumAvailableSettingIds)
                            {
                                break;
                            }
                            string drsSettingName;
                            status = NVAPI.GetSettingNameFromId(drsSettingId, out drsSettingName);
                            stringToReturn += $"DRS Setting: {drsSettingName}:\n";
                            stringToReturn += $"OPTIONS:\n";
                            UInt32 numDrsSettingValues = NVImport.NVAPI_SETTING_MAX_VALUES;
                            NVDRS_SETTING_VALUES_V1[] drsSettingValues = new NVDRS_SETTING_VALUES_V1[(int)NVImport.NVAPI_SETTING_MAX_VALUES];
                            status = NVAPI.EnumAvailableSettingValues(drsSettingId, ref numDrsSettingValues, ref drsSettingValues);
                            if (status == Status.Ok)
                            {
                                int valuesCount = 1;
                                foreach (var drsSettingValue in drsSettingValues)
                                {
                                    if (valuesCount > numDrsSettingValues)
                                    {
                                        break;
                                    }
                                    stringToReturn += $"    Default Value: {drsSettingValue.DefaultValue}\n";
                                    stringToReturn += $"    All Values: {String.Join(", ", drsSettingValue.Values)}\n";
                                    valuesCount++;
                                }
                            }
                            else if (status == Status.NvidiaDeviceNotFound)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_EnumAvailableSettingValues() returned error code {status}");
                            }
                            else if (status == Status.InvalidArgument)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: One or more args passed in are invalid. NvAPI_DRS_EnumAvailableSettingValues() returned error code {status}");
                            }
                            else if (status == Status.ApiNotInitialized)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: The NvAPI API needs to be initialized first. NvAPI_DRS_EnumAvailableSettingValues() returned error code {status}");
                            }
                            else if (status == Status.NoImplementation)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: This entry point not available in this NVIDIA Driver. NvAPI_DRS_EnumAvailableSettingValues() returned error code {status}");
                            }
                            else if (status == Status.Error)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: A miscellaneous error occurred whist destroying our DRS Session Handle. NvAPI_DRS_EnumAvailableSettingValues() returned error code {status}");
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/DumpAllDRSSettings: Some non standard error occurred while destroying our DRS Session Handle! NvAPI_DRS_EnumNvAPI_DRS_EnumAvailableSettingValuesSettings() returned error code {status}");
                            }
                            settingCount++;
                        }

                    }
                    else if (status == Status.NvidiaDeviceNotFound)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                    }
                    else if (status == Status.InvalidArgument)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: One or more args passed in are invalid. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                    }
                    else if (status == Status.ApiNotInitialized)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: The NvAPI API needs to be initialized first. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                    }
                    else if (status == Status.NoImplementation)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: This entry point not available in this NVIDIA Driver. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                    }
                    else if (status == Status.Error)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: A miscellaneous error occurred whist destroying our DRS Session Handle. NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/DumpAllDRSSettings: Some non standard error occurred while destroying our DRS Session Handle! NvAPI_DRS_GetCurrentGlobalProfile() returned error code {status}");
                    }

                }
                finally
                {
                    // Destroy the DRS Session Handle to clean up
                    status = NVAPI.DestroySession(drsSessionHandle);
                    if (status == Status.Ok)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/DumpAllDRSSettings: NvAPI_DRS_DestroySession returned OK. We cleaned up and destroyed our DRS Session Handle");
                    }
                    else if (status == Status.NvidiaDeviceNotFound)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_DestroySession() returned error code {status}");
                    }
                    else if (status == Status.InvalidArgument)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: One or more args passed in are invalid. NvAPI_DRS_DestroySession() returned error code {status}");
                    }
                    else if (status == Status.ApiNotInitialized)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: The NvAPI API needs to be initialized first. NvAPI_DRS_DestroySession() returned error code {status}");
                    }
                    else if (status == Status.NoImplementation)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: This entry point not available in this NVIDIA Driver. NvAPI_DRS_DestroySession() returned error code {status}");
                    }
                    else if (status == Status.Error)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: A miscellaneous error occurred whist destroying our DRS Session Handle. NvAPI_DRS_DestroySession() returned error code {status}");
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/DumpAllDRSSettings: Some non standard error occurred while destroying our DRS Session Handle! NvAPI_DRS_DestroySession() returned error code {status}");
                    }
                }
            }
            else if (status == Status.NvidiaDeviceNotFound)
            {
                SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: GDI Primary not on an NVIDIA GPU. NvAPI_DRS_CreateSession() returned error code {status}");
            }
            else if (status == Status.InvalidArgument)
            {
                SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: One or more args passed in are invalid. NvAPI_DRS_CreateSession() returned error code {status}");
            }
            else if (status == Status.ApiNotInitialized)
            {
                SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: The NvAPI API needs to be initialized first. NvAPI_DRS_CreateSession() returned error code {status}");
            }
            else if (status == Status.NoImplementation)
            {
                SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: This entry point not available in this NVIDIA Driver. NvAPI_DRS_CreateSession() returned error code {status}");
            }
            else if (status == Status.Error)
            {
                SharedLogger.logger.Warn($"NVIDIALibrary/DumpAllDRSSettings: A miscellaneous error occurred whist getting a DRS Session Handle. NvAPI_DRS_CreateSession() returned error code {status}");
            }
            else
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/DumpAllDRSSettings: Some non standard error occurred while getting a DRS Session Handle! NvAPI_DRS_CreateSession() returned error code {status}");
            }
            return stringToReturn;
        }

        public static IGridTopology[] CreateSingleScreenMosaicTopology()
        {

            // Figure out how many Mosaic Grid topoligies there are                    
            uint mosaicGridCount = 0;
            Status status = NVAPI.EnumDisplayGrids(ref mosaicGridCount);
            if (status == Status.Ok)
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");
            }

            // Get Current Mosaic Grid settings using the Grid topologies fnumbers we got before
            GridTopologyV2[] mosaicGridTopos = new GridTopologyV2[mosaicGridCount];
            status = NVAPI.EnumDisplayGrids(ref mosaicGridTopos, ref mosaicGridCount);
            if (status == Status.Ok)
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_EnumDisplayGrids returned OK.");
            }
            else if (status == Status.NotSupported)
            {
                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_EnumDisplayGrids() returned error code {status}");
            }
            else if (status == Status.InvalidArgument)
            {
                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_EnumDisplayGrids() returned error code {status}");
            }
            else if (status == Status.ApiNotInitialized)
            {
                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_EnumDisplayGrids() returned error code {status}");
            }
            else if (status == Status.NoImplementation)
            {
                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_EnumDisplayGrids() returned error code {status}");
            }
            else if (status == Status.Error)
            {
                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_Mosaic_EnumDisplayGrids() returned error code {status}");
            }
            else
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_EnumDisplayGrids() returned error code {status}");
            }

            // Sum up all the screens we have
            //int totalScreenCount = mosaicGridTopos.Select(tp => tp.Displays).Sum(d => d.Count());
            List<GridTopologyV2> screensToReturn = new List<GridTopologyV2>();

            foreach (GridTopologyV2 gridTopo in mosaicGridTopos)
            {
                // Figure out how many Mosaic Display topologies there are                    
                UInt32 mosaicDisplayModesCount = 0;
                status = NVAPI.EnumDisplayModes(gridTopo, ref mosaicDisplayModesCount);
                if (status == Status.Ok)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_EnumDisplayModes returned OK.");
                }

                // Get Current Mosaic Display Topology settings using the Grid topologies numbers we got before
                //NV_MOSAIC_TOPO myGridTopo = gridTopo;
                DisplaySettingsV2[] mosaicDisplaySettings = new DisplaySettingsV2[mosaicDisplayModesCount];
                status = NVAPI.EnumDisplayModes(gridTopo, ref mosaicDisplaySettings, ref mosaicDisplayModesCount);
                if (status == Status.Ok)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_EnumDisplayModes returned OK.");
                }
                else if (status == Status.NotSupported)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_EnumDisplayModes() returned error code {status}");
                }
                else if (status == Status.InvalidArgument)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_EnumDisplayModes() returned error code {status}");
                }
                else if (status == Status.ApiNotInitialized)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_EnumDisplayModes() returned error code {status}");
                }
                else if (status == Status.NoImplementation)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_EnumDisplayModes() returned error code {status}");
                }
                else if (status == Status.Error)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_Mosaic_EnumDisplayModes() returned error code {status}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting Mosaic Topology Display Settings! NvAPI_Mosaic_EnumDisplayModes() returned error code {status}");
                }

                for (int displayIndexToUse = 0; displayIndexToUse < gridTopo.DisplayCount; displayIndexToUse++)
                {
                    DisplaySettingsV2 thisScreen = new DisplaySettingsV2();
                    thisScreen.Version = NVImport.NV_MOSAIC_GRID_TOPO_V2_VER;
                    thisScreen.Rows = 1;
                    thisScreen.Columns = 1;
                    thisScreen.DisplayCount = 1;
                    thisScreen.Flags = 0;
                    thisScreen.Displays = new NV_MOSAIC_GRID_TOPO_DISPLAY_V2[NVImport.NV_MOSAIC_MAX_DISPLAYS];
                    thisScreen.Displays[0].Version = NVImport.NV_MOSAIC_GRID_TOPO_DISPLAY_V2_VER;
                    thisScreen.Displays[0].DisplayId = gridTopo.Displays[displayIndexToUse].DisplayId;
                    thisScreen.Displays[0].CloneGroup = gridTopo.Displays[displayIndexToUse].CloneGroup;
                    thisScreen.Displays[0].OverlapX = gridTopo.Displays[displayIndexToUse].OverlapX;
                    thisScreen.Displays[0].OverlapY = gridTopo.Displays[displayIndexToUse].OverlapY;
                    thisScreen.Displays[0].PixelShiftType = gridTopo.Displays[displayIndexToUse].PixelShiftType;
                    thisScreen.Displays[0].Rotation = gridTopo.Displays[displayIndexToUse].Rotation;
                    thisScreen.DisplaySettings = new NV_MOSAIC_DISPLAY_SETTING_V1();
                    thisScreen.DisplaySettings.Version = gridTopo.DisplaySettings.Version;
                    thisScreen.DisplaySettings.Bpp = gridTopo.DisplaySettings.Bpp;
                    thisScreen.DisplaySettings.Freq = gridTopo.DisplaySettings.Freq;
                    thisScreen.DisplaySettings.Height = gridTopo.DisplaySettings.Height;
                    thisScreen.DisplaySettings.Width = gridTopo.DisplaySettings.Width;
                    screensToReturn.Add(thisScreen);
                }

            }

            return screensToReturn.ToArray();
        }

        public static bool ListOfArraysEqual(List<Rectangle[]> a1, List<Rectangle[]> a2)
        {
            if (a1.Count == a2.Count)
            {
                for (int i = 0; i < a1.Count; i++)
                {
                    if (a1[i].Length == a2[i].Length)
                    {
                        for (int j = 0; j < a1[i].Length; j++)
                        {
                            if (!a1[i][j].Equals(a2[i][j]))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ListOfArraysEqual(List<ViewPortF[]> a1, List<ViewPortF[]> a2)
        {
            if (a1.Count == a2.Count)
            {
                for (int i = 0; i < a1.Count; i++)
                {
                    if (a1[i].Length == a2[i].Length)
                    {
                        for (int j = 0; j < a1[i].Length; j++)
                        {
                            if (!a1[i][j].Equals(a2[i][j]))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool Arrays2DEqual(int[][] a1, int[][] a2)
        {
            if (a1.Length == a2.Length)
            {
                for (int i = 0; i < a1.Length; i++)
                {
                    if (a1[i].Length == a2[i].Length)
                    {
                        for (int j = 0; j < a1[i].Length; j++)
                        {
                            if (a1[i][j] != a2[i][j])
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool EqualButDifferentOrder<T>(IList<T> list1, IList<T> list2)
        {

            if (list1.Count != list2.Count)
            {
                return false;
            }

            // Now we need to go through the list1, checking that all it's items are in list2
            foreach (T item1 in list1)
            {
                bool foundIt = false;
                foreach (T item2 in list2)
                {
                    if (item1.Equals(item2))
                    {
                        foundIt = true;
                        break;
                    }
                }
                if (!foundIt)
                {
                    return false;
                }
            }

            // Now we need to go through the list2, checking that all it's items are in list1
            foreach (T item2 in list2)
            {
                bool foundIt = false;
                foreach (T item1 in list1)
                {
                    if (item1.Equals(item2))
                    {
                        foundIt = true;
                        break;
                    }
                }
                if (!foundIt)
                {
                    return false;
                }
            }

            return true;
        }


        public static bool EqualButDifferentOrder<TKey, TValue>(IDictionary<TKey, TValue> dict1, IDictionary<TKey, TValue> dict2)
        {

            if (dict1.Count != dict2.Count)
            {
                return false;
            }

            // Now we need to go through the dict1, checking that all it's items are in dict2
            foreach (KeyValuePair<TKey, TValue> item1 in dict1)
            {
                bool foundIt = false;
                foreach (KeyValuePair<TKey, TValue> item2 in dict2)
                {
                    if (item1.Key.Equals(item2.Key) && item1.Value.Equals(item2.Value))
                    {
                        foundIt = true;
                        break;
                    }
                }
                if (!foundIt)
                {
                    return false;
                }
            }

            // Now we need to go through the dict2, checking that all it's items are in dict1
            foreach (KeyValuePair<TKey, TValue> item2 in dict2)
            {
                bool foundIt = false;
                foreach (KeyValuePair<TKey, TValue> item1 in dict1)
                {
                    if (item1.Key.Equals(item2.Key) && item1.Value.Equals(item2.Value))
                    {
                        foundIt = true;
                        break;
                    }
                }
                if (!foundIt)
                {
                    return false;
                }
            }

            return true;
        }
    }





    [global::System.Serializable]
    public class NVIDIALibraryException : Exception
    {
        public NVIDIALibraryException() { }
        public NVIDIALibraryException(string message) : base(message) { }
        public NVIDIALibraryException(string message, Exception inner) : base(message, inner) { }
        protected NVIDIALibraryException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}