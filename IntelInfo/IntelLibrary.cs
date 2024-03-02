using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using DisplayMagicianShared;
using System.ComponentModel;
using DisplayMagicianShared.Windows;
using System.Threading;

namespace DisplayMagicianShared.Intel
{
    [StructLayout(LayoutKind.Sequential)]
    public struct INTEL_ADAPTER_CONFIG : IEquatable<INTEL_ADAPTER_CONFIG>
    {
        public int AdapterDeviceNumber;
        public int AdapterBusNumber;
        public int AdapterIndex;
        public bool IsPrimaryAdapter;
        public string DisplayName;
        public int OSDisplayIndex;

        public override bool Equals(object obj) => obj is INTEL_ADAPTER_CONFIG other && this.Equals(other);

        public bool Equals(INTEL_ADAPTER_CONFIG other)
        => AdapterIndex == other.AdapterIndex &&
           AdapterBusNumber == other.AdapterBusNumber &&
           AdapterDeviceNumber == other.AdapterDeviceNumber &&
           IsPrimaryAdapter == other.IsPrimaryAdapter &&
           DisplayName == other.DisplayName &&
           OSDisplayIndex == other.OSDisplayIndex;

        public override int GetHashCode()
        {
            return (AdapterIndex, AdapterBusNumber, AdapterDeviceNumber, IsPrimaryAdapter, DisplayName, OSDisplayIndex).GetHashCode();
        }

        public static bool operator ==(INTEL_ADAPTER_CONFIG lhs, INTEL_ADAPTER_CONFIG rhs) => lhs.Equals(rhs);

        public static bool operator !=(INTEL_ADAPTER_CONFIG lhs, INTEL_ADAPTER_CONFIG rhs) => !(lhs == rhs);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTEL_SLSMAP_CONFIG : IEquatable<INTEL_SLSMAP_CONFIG>
    {
        public ADL_SLS_MAP SLSMap;
        public List<ADL_SLS_TARGET> SLSTargets;
        public List<ADL_SLS_MODE> NativeModes;
        public List<ADL_SLS_OFFSET> NativeModeOffsets;
        public List<ADL_BEZEL_TRANSIENT_MODE> BezelModes;
        public List<ADL_BEZEL_TRANSIENT_MODE> TransientModes;
        public List<ADL_SLS_OFFSET> SLSOffsets;
        public int BezelModePercent;

        public override bool Equals(object obj) => obj is INTEL_SLS_CONFIG other && this.Equals(other);

        public bool Equals(INTEL_SLSMAP_CONFIG other)
        => SLSMap == other.SLSMap &&
           SLSTargets.SequenceEqual(other.SLSTargets) &&
           NativeModes.SequenceEqual(other.NativeModes) &&
           NativeModeOffsets.SequenceEqual(other.NativeModeOffsets) &&
           BezelModes.SequenceEqual(other.BezelModes) &&
           TransientModes.SequenceEqual(other.TransientModes) &&
           SLSOffsets.SequenceEqual(other.SLSOffsets) &&
           BezelModePercent == other.BezelModePercent;

        public override int GetHashCode()
        {
            return (SLSMap, SLSTargets, NativeModes, NativeModeOffsets, BezelModes, TransientModes, SLSOffsets, BezelModePercent).GetHashCode();
        }
        public static bool operator ==(INTEL_SLSMAP_CONFIG lhs, INTEL_SLSMAP_CONFIG rhs) => lhs.Equals(rhs);

        public static bool operator !=(INTEL_SLSMAP_CONFIG lhs, INTEL_SLSMAP_CONFIG rhs) => !(lhs == rhs);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTEL_SLS_CONFIG : IEquatable<INTEL_SLS_CONFIG>
    {
        public bool IsSlsEnabled;
        public List<INTEL_SLSMAP_CONFIG> SLSMapConfigs;
        public List<ADL_MODE> SLSEnabledDisplayTargets;

        public override bool Equals(object obj) => obj is INTEL_SLS_CONFIG other && this.Equals(other);

        public bool Equals(INTEL_SLS_CONFIG other)
        => IsSlsEnabled == other.IsSlsEnabled &&
           SLSMapConfigs.SequenceEqual(other.SLSMapConfigs) &&
           SLSEnabledDisplayTargets.SequenceEqual(other.SLSEnabledDisplayTargets);

        public override int GetHashCode()
        {
            return (IsSlsEnabled, SLSMapConfigs, SLSEnabledDisplayTargets).GetHashCode();
        }
        public static bool operator ==(INTEL_SLS_CONFIG lhs, INTEL_SLS_CONFIG rhs) => lhs.Equals(rhs);

        public static bool operator !=(INTEL_SLS_CONFIG lhs, INTEL_SLS_CONFIG rhs) => !(lhs == rhs);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTEL_HDR_CONFIG : IEquatable<INTEL_HDR_CONFIG>
    {
        public int AdapterIndex;
        public bool HDRSupported;
        public bool HDREnabled;

        public override bool Equals(object obj) => obj is INTEL_HDR_CONFIG other && this.Equals(other);
        public bool Equals(INTEL_HDR_CONFIG other)
        => AdapterIndex == other.AdapterIndex &&
           HDRSupported == other.HDRSupported &&
           HDREnabled == other.HDREnabled;

        public override int GetHashCode()
        {
            return (AdapterIndex, HDRSupported, HDREnabled).GetHashCode();
        }
        public static bool operator ==(INTEL_HDR_CONFIG lhs, INTEL_HDR_CONFIG rhs) => lhs.Equals(rhs);

        public static bool operator !=(INTEL_HDR_CONFIG lhs, INTEL_HDR_CONFIG rhs) => !(lhs == rhs);
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct INTEL_DISPLAY_CONFIG : IEquatable<INTEL_DISPLAY_CONFIG>
    {
        public List<INTEL_ADAPTER_CONFIG> AdapterConfigs;
        //public INTEL_SLS_CONFIG SlsConfig;
        //public List<ADL_DISPLAY_MAP> DisplayMaps;
        //public List<ADL_DISPLAY_TARGET> DisplayTargets;
        //public Dictionary<int, INTEL_HDR_CONFIG> HdrConfigs;
        public List<string> DisplayIdentifiers;
        
        public override bool Equals(object obj) => obj is INTEL_DISPLAY_CONFIG other && this.Equals(other);

        public bool Equals(INTEL_DISPLAY_CONFIG other)
        => AdapterConfigs.SequenceEqual(other.AdapterConfigs) &&
           SlsConfig.Equals(other.SlsConfig) &&
           DisplayMaps.SequenceEqual(other.DisplayMaps) &&
           DisplayTargets.SequenceEqual(other.DisplayTargets) &&
           HdrConfigs.SequenceEqual(other.HdrConfigs) &&
           DisplayIdentifiers.SequenceEqual(other.DisplayIdentifiers);

        public override int GetHashCode()
        {
            return (AdapterConfigs, SlsConfig, DisplayMaps, DisplayTargets, DisplayIdentifiers).GetHashCode();
        }

        public static bool operator ==(INTEL_DISPLAY_CONFIG lhs, INTEL_DISPLAY_CONFIG rhs) => lhs.Equals(rhs);

        public static bool operator !=(INTEL_DISPLAY_CONFIG lhs, INTEL_DISPLAY_CONFIG rhs) => !(lhs == rhs);
    }

    class IntelLibrary : IDisposable
    {

        // Static members are 'eagerly initialized', that is, 
        // immediately when class is loaded for the first time.
        // .NET guarantees thread safety for static initialization
        private static IntelLibrary _instance = new IntelLibrary();

        private bool _initialised = false;

        // To detect redundant calls
        private bool _disposed = false;

        // Instantiate a SafeHandle instance.
        private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);
        private IntPtr _adlContextHandle = IntPtr.Zero;
        private INTEL_DISPLAY_CONFIG _activeDisplayConfig;
        public List<ADL_DISPLAY_CONNECTION_TYPE> SkippedColorConnectionTypes;
        public List<string> _allConnectedDisplayIdentifiers;

        static IntelLibrary() { }
        public IntelLibrary()
        {
            // Populate the list of ConnectionTypes we want to skip as they don't support querying
            SkippedColorConnectionTypes = new List<ADL_DISPLAY_CONNECTION_TYPE> {
                ADL_DISPLAY_CONNECTION_TYPE.Composite,
                ADL_DISPLAY_CONNECTION_TYPE.DVI_D,
                ADL_DISPLAY_CONNECTION_TYPE.DVI_I,
                ADL_DISPLAY_CONNECTION_TYPE.RCA_3Component,
                ADL_DISPLAY_CONNECTION_TYPE.SVideo,
                ADL_DISPLAY_CONNECTION_TYPE.VGA
            };

            _activeDisplayConfig = CreateDefaultConfig();
            try
            {
                SharedLogger.logger.Trace($"IntelLibrary/IntelLibrary: Attempting to load the Intel ADL DLL {ADLImport.ATI_ADL_DLL}");
                // Attempt to prelink all of the NVAPI functions
                Marshal.PrelinkAll(typeof(IGCLImport));

                SharedLogger.logger.Trace("IntelLibrary/IntelLibrary: Intialising Intel ADL2 library interface");
                // Second parameter is 1 so that we only the get connected adapters in use now

                // We set the environment variable as a workaround so that ADL2_Display_SLSMapConfigX2_Get works :(
                // This is a weird thing that Intel even set in their own code! WTF! Who programmed that as a feature?
                Environment.SetEnvironmentVariable("ADL_4KWORKAROUND_CANCEL", "TRUE");

                try
                {
                    ctl_result_t ADLRet;
                    ADLRet = ADLImport.ADL2_Main_Control_Create(ADLImport.ADL_Main_Memory_Alloc, ADLImport.ADL_TRUE, out _adlContextHandle);
                    if (ADLRet == ctl_result_t.ADL_OK)
                    {
                        _initialised = true;
                        SharedLogger.logger.Trace($"IntelLibrary/IntelLibrary: Intel ADL2 library was initialised successfully");
                        SharedLogger.logger.Trace($"IntelLibrary/IntelLibrary: Running UpdateActiveConfig to ensure there is a config to use later");
                        _activeDisplayConfig = GetActiveConfig();
                        _allConnectedDisplayIdentifiers = GetAllConnectedDisplayIdentifiers();
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/IntelLibrary: Error intialising Intel ADL2 library. ADL2_Main_Control_Create() returned error code {ADLRet}");
                    }
                }
                catch (Exception ex)
                {
                    SharedLogger.logger.Trace(ex, $"IntelLibrary/IntelLibrary: Exception intialising Intel ADL2 library. ADL2_Main_Control_Create() caused an exception.");
                }

            }
            catch (DllNotFoundException ex)
            {
                // If we get here then the Intel ADL DLL wasn't found. We can't continue to use it, so we log the error and exit
                SharedLogger.logger.Info(ex, $"IntelLibrary/IntelLibrary: Exception trying to load the Intel ADL DLL {ADLImport.ATI_ADL_DLL}. This generally means you don't have the Intel ADL driver installed.");
            }

        }

        ~IntelLibrary()
        {
            SharedLogger.logger.Trace("IntelLibrary/~IntelLibrary: Destroying Intel ADL2 library interface");
            // If the ADL2 library was initialised, then we need to free it up.
            if (_initialised)
            {
                try
                {
                    ADLImport.ADL2_Main_Control_Destroy(_adlContextHandle);
                    SharedLogger.logger.Trace($"IntelLibrary/IntelLibrary: Intel ADL2 library was destroyed successfully");
                }
                catch (Exception ex)
                {
                    SharedLogger.logger.Trace(ex, $"IntelLibrary/IntelLibrary: Exception destroying Intel ADL2 library. ADL2_Main_Control_Destroy() caused an exception.");
                }

            }
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

                //ADLImport.ADL_Main_Control_Destroy();

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

        public List<string> PCIVendorIDs
        {
            get
            {
                // A list of all the matching PCI Vendor IDs are per https://www.pcilookup.com/?ven=Intel&dev=&action=submit
                return new List<string>() { "1002" };
            }
        }

        public INTEL_DISPLAY_CONFIG ActiveDisplayConfig
        {
            get
            {
                return _activeDisplayConfig;
            }
            set
            {
                _activeDisplayConfig = value;
            }
        }

        public List<string> CurrentDisplayIdentifiers
        {
            get
            {
                return _activeDisplayConfig.DisplayIdentifiers;
            }
        }

        public static IntelLibrary GetLibrary()
        {
            return _instance;
        }

        public INTEL_DISPLAY_CONFIG CreateDefaultConfig()
        {
            INTEL_DISPLAY_CONFIG myDefaultConfig = new INTEL_DISPLAY_CONFIG();

            // Fill in the minimal amount we need to avoid null references
            // so that we won't break json.net when we save a default config

            myDefaultConfig.AdapterConfigs = new List<INTEL_ADAPTER_CONFIG>();
            myDefaultConfig.SlsConfig.IsSlsEnabled = false;
            myDefaultConfig.SlsConfig.SLSMapConfigs = new List<INTEL_SLSMAP_CONFIG>();
            myDefaultConfig.SlsConfig.SLSEnabledDisplayTargets = new List<ADL_MODE>();
            myDefaultConfig.DisplayMaps = new List<ADL_DISPLAY_MAP>();
            myDefaultConfig.DisplayTargets = new List<ADL_DISPLAY_TARGET>();
            myDefaultConfig.HdrConfigs = new Dictionary<int, INTEL_HDR_CONFIG>();
            myDefaultConfig.DisplayIdentifiers = new List<string>();

            return myDefaultConfig;
        }

        public bool UpdateActiveConfig()
        {
            SharedLogger.logger.Trace($"IntelLibrary/UpdateActiveConfig: Updating the currently active config");
            try
            {
                _activeDisplayConfig = GetActiveConfig();
                _allConnectedDisplayIdentifiers = GetAllConnectedDisplayIdentifiers();
            }
            catch (Exception ex)
            {
                SharedLogger.logger.Trace(ex, $"IntelLibrary/UpdateActiveConfig: Exception updating the currently active config");
                return false;
            }

            return true;
        }

        public INTEL_DISPLAY_CONFIG GetActiveConfig()
        {
            SharedLogger.logger.Trace($"IntelLibrary/GetActiveConfig: Getting the currently active config");
            bool allDisplays = true;
            return GetIntelDisplayConfig(allDisplays);
        }

        private INTEL_DISPLAY_CONFIG GetIntelDisplayConfig(bool allDisplays = false)
        {
            INTEL_DISPLAY_CONFIG myDisplayConfig = CreateDefaultConfig();

            if (_initialised)
            {

                // Get the Adapter info for ALL adapter and put it in the AdapterBuffer
                SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: Running ADL2_Adapter_AdapterInfoX4_Get to get the information about all Intel Adapters.");
                int numAdaptersInfo = 0;
                IntPtr adapterInfoBuffer = IntPtr.Zero;
                ctl_result_t ADLRet = ADLImport.ADL2_Adapter_AdapterInfoX4_Get(_adlContextHandle, -1, out numAdaptersInfo, out adapterInfoBuffer);
                if (ADLRet == ctl_result_t.ADL_OK)
                {
                    SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: ADL2_Adapter_AdapterInfoX4_Get returned information about all Intel Adapters.");
                }
                else
                {
                    SharedLogger.logger.Error($"IntelLibrary/GetIntelDisplayConfig: ERROR - ADL2_Adapter_AdapterInfoX4_Get returned ADL_STATUS {ADLRet} when trying to get the adapter info about all Intel Adapters. Trying to skip this adapter so something at least works.");
                    return myDisplayConfig;
                }

                ADL_ADAPTER_INFOX2[] adapterArray = new ADL_ADAPTER_INFOX2[numAdaptersInfo];
                if (numAdaptersInfo > 0)
                {
                    IntPtr currentDisplayTargetBuffer = adapterInfoBuffer;
                    for (int i = 0; i < numAdaptersInfo; i++)
                    {
                        // build a structure in the array slot
                        adapterArray[i] = new ADL_ADAPTER_INFOX2();
                        // fill the array slot structure with the data from the buffer
                        adapterArray[i] = (ADL_ADAPTER_INFOX2)Marshal.PtrToStructure(currentDisplayTargetBuffer, typeof(ADL_ADAPTER_INFOX2));
                        // destroy the bit of memory we no longer need
                        //Marshal.DestroyStructure(currentDisplayTargetBuffer, typeof(ADL_ADAPTER_INFOX2));
                        // advance the buffer forwards to the next object
                        currentDisplayTargetBuffer = (IntPtr)((long)currentDisplayTargetBuffer + Marshal.SizeOf(adapterArray[i]));
                    }
                    // Free the memory used by the buffer                        
                    Marshal.FreeCoTaskMem(adapterInfoBuffer);
                }

                // Now go through each adapter and get the information we need from it
                for (int adapterIndex = 0; adapterIndex < numAdaptersInfo; adapterIndex++)
                {
                    // Skip this adapter if it isn't active
                    ADL_ADAPTER_INFOX2 oneAdapter = adapterArray[adapterIndex]; // There is always just one as we asked for a specific one!
                    if (oneAdapter.Exist != ADLImport.ADL_TRUE)
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: Intel Adapter #{oneAdapter.AdapterIndex.ToString()} doesn't exist at present so skipping detection for this adapter.");
                        continue;
                    }

                    // Only skip non-present displays if we want all displays information
                    if (oneAdapter.Present != ADLImport.ADL_TRUE)
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: Intel Adapter #{oneAdapter.AdapterIndex.ToString()} isn't enabled at present so skipping detection for this adapter.");
                        continue;
                    }

                    // Check if the adapter is active
                    // Skip this adapter if it isn't active
                    int adapterActiveStatus = ADLImport.ADL_FALSE;
                    ADLRet = ADLImport.ADL2_Adapter_Active_Get(_adlContextHandle, adapterIndex, out adapterActiveStatus);
                    if (ADLRet == ctl_result_t.ADL_OK)
                    {
                        if (adapterActiveStatus == ADLImport.ADL_TRUE)
                        {
                            SharedLogger.logger.Trace($"IntelLibrary/GetSomeDisplayIdentifiers: ADL2_Adapter_Active_Get returned ADL_TRUE - Intel Adapter #{adapterIndex} is active! We can continue.");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"IntelLibrary/GetSomeDisplayIdentifiers: ADL2_Adapter_Active_Get returned ADL_FALSE - Intel Adapter #{adapterIndex} is NOT active, so skipping.");
                            continue;
                        }
                    }
                    else
                    {
                        SharedLogger.logger.Warn($"IntelLibrary/GetSomeDisplayIdentifiers: WARNING - ADL2_Adapter_Active_Get returned ADL_STATUS {ADLRet} when trying to see if Intel Adapter #{adapterIndex} is active. Trying to skip this adapter so something at least works.");
                        continue;
                    }

                    // Go grab the DisplayMaps and DisplayTargets as that is useful infor for creating screens
                    int numDisplayTargets = 0;
                    int numDisplayMaps = 0;
                    IntPtr displayTargetBuffer = IntPtr.Zero;
                    IntPtr displayMapBuffer = IntPtr.Zero;
                    ADLRet = ADLImport.ADL2_Display_DisplayMapConfig_Get(_adlContextHandle, adapterIndex, out numDisplayMaps, out displayMapBuffer, out numDisplayTargets, out displayTargetBuffer, ADLImport.ADL_DISPLAY_DISPLAYMAP_OPTION_GPUINFO);
                    if (ADLRet == ctl_result_t.ADL_OK)
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: ADL2_Display_DisplayMapConfig_Get returned information about all displaytargets connected to Intel adapter {adapterIndex}.");
                    }
                    else
                    {
                        SharedLogger.logger.Error($"IntelLibrary/GetIntelDisplayConfig: ERROR - ADL2_Display_DisplayMapConfig_Get returned ADL_STATUS {ADLRet} when trying to get the display target info from Intel adapter {adapterIndex} in the computer.");
                        throw new IntelLibraryException($"ADL2_Display_DisplayMapConfig_Get returned ADL_STATUS {ADLRet} when trying to get the display target info from Intel adapter {adapterIndex} in the computer");
                    }

                    ADL_DISPLAY_MAP[] displayMapArray = { };
                    if (numDisplayMaps > 0)
                    {

                        IntPtr currentDisplayMapBuffer = displayMapBuffer;
                        displayMapArray = new ADL_DISPLAY_MAP[numDisplayMaps];
                        for (int i = 0; i < numDisplayMaps; i++)
                        {
                            // build a structure in the array slot
                            displayMapArray[i] = new ADL_DISPLAY_MAP();
                            // fill the array slot structure with the data from the buffer
                            displayMapArray[i] = (ADL_DISPLAY_MAP)Marshal.PtrToStructure(currentDisplayMapBuffer, typeof(ADL_DISPLAY_MAP));
                            // destroy the bit of memory we no longer need
                            Marshal.DestroyStructure(currentDisplayMapBuffer, typeof(ADL_DISPLAY_MAP));
                            // advance the buffer forwards to the next object
                            currentDisplayMapBuffer = (IntPtr)((long)currentDisplayMapBuffer + Marshal.SizeOf(displayMapArray[i]));
                        }
                        // Free the memory used by the buffer                        
                        Marshal.FreeCoTaskMem(displayMapBuffer);
                        // Save the item
                        myDisplayConfig.DisplayMaps = displayMapArray.ToList<ADL_DISPLAY_MAP>();

                    }

                    ADL_DISPLAY_TARGET[] displayTargetArray = { };
                    if (numDisplayTargets > 0)
                    {
                        IntPtr currentDisplayTargetBuffer = displayTargetBuffer;
                        //displayTargetArray = new ADL_DISPLAY_TARGET[numDisplayTargets];
                        displayTargetArray = new ADL_DISPLAY_TARGET[numDisplayTargets];
                        for (int i = 0; i < numDisplayTargets; i++)
                        {
                            // build a structure in the array slot
                            displayTargetArray[i] = new ADL_DISPLAY_TARGET();
                            //displayTargetArray[i] = new ADL_DISPLAY_TARGET();
                            // fill the array slot structure with the data from the buffer
                            displayTargetArray[i] = (ADL_DISPLAY_TARGET)Marshal.PtrToStructure(currentDisplayTargetBuffer, typeof(ADL_DISPLAY_TARGET));
                            //displayTargetArray[i] = (ADL_DISPLAY_TARGET)Marshal.PtrToStructure(currentDisplayTargetBuffer, typeof(ADL_DISPLAY_TARGET));
                            // destroy the bit of memory we no longer need
                            Marshal.DestroyStructure(currentDisplayTargetBuffer, typeof(ADL_DISPLAY_TARGET));
                            // advance the buffer forwards to the next object
                            currentDisplayTargetBuffer = (IntPtr)((long)currentDisplayTargetBuffer + Marshal.SizeOf(displayTargetArray[i]));
                            //currentDisplayTargetBuffer = (IntPtr)((long)currentDisplayTargetBuffer + Marshal.SizeOf(displayTargetArray[i]));

                        }
                        // Free the memory used by the buffer                        
                        Marshal.FreeCoTaskMem(displayTargetBuffer);
                        // Save the item                            
                        //savedAdapterConfig.DisplayTargets = new ADL_DISPLAY_TARGET[numDisplayTargets];
                        myDisplayConfig.DisplayTargets = displayTargetArray.ToList<ADL_DISPLAY_TARGET>();
                    }

                    // Loop through all the displayTargets currently in use
                    foreach (var displayTarget in displayTargetArray)
                    {
                        if (displayTarget.DisplayID.DisplayLogicalAdapterIndex == oneAdapter.AdapterIndex)
                        {
                            // we only want to record the adapters that are currently in use as displayTargets
                            INTEL_ADAPTER_CONFIG savedAdapterConfig = new INTEL_ADAPTER_CONFIG();
                            savedAdapterConfig.AdapterBusNumber = oneAdapter.BusNumber;
                            savedAdapterConfig.AdapterDeviceNumber = oneAdapter.DeviceNumber;
                            savedAdapterConfig.AdapterIndex = oneAdapter.AdapterIndex;
                            savedAdapterConfig.DisplayName = oneAdapter.DisplayName;
                            savedAdapterConfig.OSDisplayIndex = oneAdapter.OSDisplayIndex;

                            // Save the Intel Adapter Config
                            if (!myDisplayConfig.AdapterConfigs.Contains(savedAdapterConfig))
                            {
                                // Save the new adapter config only if we haven't already
                                myDisplayConfig.AdapterConfigs.Add(savedAdapterConfig);
                            }

                        }
                    }

                    // Prep the SLSMapConfig list
                    myDisplayConfig.SlsConfig.SLSMapConfigs = new List<INTEL_SLSMAP_CONFIG>();

                    // If there are more than 1 display targets then eyefinity is possible
                    if (numDisplayTargets > 1)
                    {
                        // Check if SLS is enabled for this adapter!
                        int matchingSLSMapIndex = -1;
                        ADLRet = ADLImport.ADL2_Display_SLSMapIndex_Get(_adlContextHandle, oneAdapter.AdapterIndex, numDisplayTargets, displayTargetArray, out matchingSLSMapIndex);
                        if (ADLRet == ctl_result_t.ADL_OK && matchingSLSMapIndex != -1)
                        {
                            // We have a matching SLS index!
                            SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: Intel Adapter #{oneAdapter.AdapterIndex.ToString()} has one or more SLS Maps that could be used with this display configuration! Eyefinity (SLS) could be enabled.");

                            INTEL_SLSMAP_CONFIG mySLSMapConfig = new INTEL_SLSMAP_CONFIG();

                            // We want to get the SLSMapConfig for this matching SLS Map to see if it is actually in use
                            int numSLSTargets = 0;
                            IntPtr slsTargetBuffer = IntPtr.Zero;
                            int numNativeMode = 0;
                            IntPtr nativeModeBuffer = IntPtr.Zero;
                            int numNativeModeOffsets = 0;
                            IntPtr nativeModeOffsetsBuffer = IntPtr.Zero;
                            int numBezelMode = 0;
                            IntPtr bezelModeBuffer = IntPtr.Zero;
                            int numTransientMode = 0;
                            IntPtr transientModeBuffer = IntPtr.Zero;
                            int numSLSOffset = 0;
                            IntPtr slsOffsetBuffer = IntPtr.Zero;
                            ADL_SLS_MAP slsMap = new ADL_SLS_MAP();
                            ADLRet = ADLImport.ADL2_Display_SLSMapConfigX2_Get(
                                                                            _adlContextHandle,
                                                                                oneAdapter.AdapterIndex,
                                                                                matchingSLSMapIndex,
                                                                                ref slsMap,
                                                                                out numSLSTargets,
                                                                                out slsTargetBuffer,
                                                                                out numNativeMode,
                                                                                out nativeModeBuffer,
                                                                                out numNativeModeOffsets,
                                                                                out nativeModeOffsetsBuffer,
                                                                                out numBezelMode,
                                                                                out bezelModeBuffer,
                                                                                out numTransientMode,
                                                                                out transientModeBuffer,
                                                                                out numSLSOffset,
                                                                                out slsOffsetBuffer,
                                                                                ADLImport.ADL_DISPLAY_SLSGRID_CAP_OPTION_RELATIVETO_CURRENTANGLE);
                            if (ADLRet == ctl_result_t.ADL_OK)
                            {
                                SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: ADL2_Display_SLSMapConfigX2_Get returned information about the SLS Info connected to Intel adapter {adapterIndex}.");
                            }
                            else
                            {
                                SharedLogger.logger.Error($"IntelLibrary/GetIntelDisplayConfig: ERROR - ADL2_Display_SLSMapConfigX2_Get returned ADL_STATUS {ADLRet} when trying to get the SLS Info from Intel adapter {adapterIndex} in the computer.");
                                continue;
                            }

                            // First check that the number of grid entries is equal to the number
                            // of display targets associated with this adapter & SLS surface.
                            if (numDisplayTargets != (slsMap.Grid.SLSGridColumn * slsMap.Grid.SLSGridRow))
                            {
                                //Number of display targets returned is not equal to the SLS grid size, so SLS can't be enabled fo this display
                                //myDisplayConfig.SlsConfig.IsSlsEnabled = false; // This is already set to false at the start!
                                break;
                            }

                            // Add the slsMap to the config we want to store
                            mySLSMapConfig.SLSMap = slsMap;

                            // Process the slsTargetBuffer
                            ADL_SLS_TARGET[] slsTargetArray = new ADL_SLS_TARGET[numSLSTargets];
                            if (numSLSTargets > 0)
                            {
                                IntPtr currentSLSTargetBuffer = slsTargetBuffer;
                                for (int i = 0; i < numSLSTargets; i++)
                                {
                                    // build a structure in the array slot
                                    slsTargetArray[i] = new ADL_SLS_TARGET();
                                    // fill the array slot structure with the data from the buffer
                                    slsTargetArray[i] = (ADL_SLS_TARGET)Marshal.PtrToStructure(currentSLSTargetBuffer, typeof(ADL_SLS_TARGET));
                                    // destroy the bit of memory we no longer need
                                    //Marshal.DestroyStructure(currentDisplayTargetBuffer, typeof(ADL_ADAPTER_INFOX2));
                                    // advance the buffer forwards to the next object
                                    currentSLSTargetBuffer = (IntPtr)((long)currentSLSTargetBuffer + Marshal.SizeOf(slsTargetArray[i]));
                                }
                                // Free the memory used by the buffer                        
                                Marshal.FreeCoTaskMem(slsTargetBuffer);

                                // Add the slsTarget to the config we want to store
                                mySLSMapConfig.SLSTargets = slsTargetArray.ToList();

                            }
                            else
                            {
                                // Add the slsTarget to the config we want to store
                                mySLSMapConfig.SLSTargets = new List<ADL_SLS_TARGET>();
                            }

                            // Process the nativeModeBuffer
                            ADL_SLS_MODE[] nativeModeArray = new ADL_SLS_MODE[numNativeMode];
                            if (numNativeMode > 0)
                            {
                                IntPtr currentNativeModeBuffer = nativeModeBuffer;
                                for (int i = 0; i < numNativeMode; i++)
                                {
                                    // build a structure in the array slot
                                    nativeModeArray[i] = new ADL_SLS_MODE();
                                    // fill the array slot structure with the data from the buffer
                                    nativeModeArray[i] = (ADL_SLS_MODE)Marshal.PtrToStructure(currentNativeModeBuffer, typeof(ADL_SLS_MODE));
                                    // destroy the bit of memory we no longer need
                                    //Marshal.DestroyStructure(currentDisplayTargetBuffer, typeof(ADL_ADAPTER_INFOX2));
                                    // advance the buffer forwards to the next object
                                    currentNativeModeBuffer = (IntPtr)((long)currentNativeModeBuffer + Marshal.SizeOf(nativeModeArray[i]));
                                }
                                // Free the memory used by the buffer                        
                                Marshal.FreeCoTaskMem(nativeModeBuffer);

                                // Add the nativeMode to the config we want to store
                                mySLSMapConfig.NativeModes = nativeModeArray.ToList();

                            }
                            else
                            {
                                // Add the slsTarget to the config we want to store
                                mySLSMapConfig.NativeModes = new List<ADL_SLS_MODE>();
                            }

                            // Process the nativeModeOffsetsBuffer
                            ADL_SLS_OFFSET[] nativeModeOffsetArray = new ADL_SLS_OFFSET[numNativeModeOffsets];
                            if (numNativeModeOffsets > 0)
                            {
                                IntPtr currentNativeModeOffsetsBuffer = nativeModeOffsetsBuffer;
                                for (int i = 0; i < numNativeModeOffsets; i++)
                                {
                                    // build a structure in the array slot
                                    nativeModeOffsetArray[i] = new ADL_SLS_OFFSET();
                                    // fill the array slot structure with the data from the buffer
                                    nativeModeOffsetArray[i] = (ADL_SLS_OFFSET)Marshal.PtrToStructure(currentNativeModeOffsetsBuffer, typeof(ADL_SLS_OFFSET));
                                    // destroy the bit of memory we no longer need
                                    //Marshal.DestroyStructure(currentDisplayTargetBuffer, typeof(ADL_ADAPTER_INFOX2));
                                    // advance the buffer forwards to the next object
                                    currentNativeModeOffsetsBuffer = (IntPtr)((long)currentNativeModeOffsetsBuffer + Marshal.SizeOf(nativeModeOffsetArray[i]));
                                }
                                // Free the memory used by the buffer                        
                                Marshal.FreeCoTaskMem(nativeModeOffsetsBuffer);

                                // Add the nativeModeOffsets to the config we want to store
                                mySLSMapConfig.NativeModeOffsets = nativeModeOffsetArray.ToList();

                            }
                            else
                            {
                                // Add the empty list to the config we want to store
                                mySLSMapConfig.NativeModeOffsets = new List<ADL_SLS_OFFSET>();
                            }

                            // Process the bezelModeBuffer
                            ADL_BEZEL_TRANSIENT_MODE[] bezelModeArray = new ADL_BEZEL_TRANSIENT_MODE[numBezelMode];
                            if (numBezelMode > 0)
                            {
                                IntPtr currentBezelModeBuffer = bezelModeBuffer;
                                for (int i = 0; i < numBezelMode; i++)
                                {
                                    // build a structure in the array slot
                                    bezelModeArray[i] = new ADL_BEZEL_TRANSIENT_MODE();
                                    // fill the array slot structure with the data from the buffer
                                    bezelModeArray[i] = (ADL_BEZEL_TRANSIENT_MODE)Marshal.PtrToStructure(currentBezelModeBuffer, typeof(ADL_BEZEL_TRANSIENT_MODE));
                                    // destroy the bit of memory we no longer need
                                    //Marshal.DestroyStructure(currentDisplayTargetBuffer, typeof(ADL_ADAPTER_INFOX2));
                                    // advance the buffer forwards to the next object
                                    currentBezelModeBuffer = (IntPtr)((long)currentBezelModeBuffer + Marshal.SizeOf(bezelModeArray[i]));
                                }
                                // Free the memory used by the buffer                        
                                Marshal.FreeCoTaskMem(bezelModeBuffer);

                                // Add the bezelModes to the config we want to store
                                mySLSMapConfig.BezelModes = bezelModeArray.ToList();

                            }
                            else
                            {
                                // Add the slsTarget to the config we want to store
                                mySLSMapConfig.BezelModes = new List<ADL_BEZEL_TRANSIENT_MODE>();
                            }

                            // Process the transientModeBuffer
                            ADL_BEZEL_TRANSIENT_MODE[] transientModeArray = new ADL_BEZEL_TRANSIENT_MODE[numTransientMode];
                            if (numTransientMode > 0)
                            {
                                IntPtr currentTransientModeBuffer = transientModeBuffer;
                                for (int i = 0; i < numTransientMode; i++)
                                {
                                    // build a structure in the array slot
                                    transientModeArray[i] = new ADL_BEZEL_TRANSIENT_MODE();
                                    // fill the array slot structure with the data from the buffer
                                    transientModeArray[i] = (ADL_BEZEL_TRANSIENT_MODE)Marshal.PtrToStructure(currentTransientModeBuffer, typeof(ADL_BEZEL_TRANSIENT_MODE));
                                    // destroy the bit of memory we no longer need
                                    //Marshal.DestroyStructure(currentDisplayTargetBuffer, typeof(ADL_ADAPTER_INFOX2));
                                    // advance the buffer forwards to the next object
                                    currentTransientModeBuffer = (IntPtr)((long)currentTransientModeBuffer + Marshal.SizeOf(transientModeArray[i]));
                                }
                                // Free the memory used by the buffer                        
                                Marshal.FreeCoTaskMem(transientModeBuffer);

                                // Add the transientModes to the config we want to store
                                mySLSMapConfig.TransientModes = transientModeArray.ToList();
                            }
                            else
                            {
                                // Add the slsTarget to the config we want to store
                                mySLSMapConfig.TransientModes = new List<ADL_BEZEL_TRANSIENT_MODE>();
                            }

                            // Process the slsOffsetBuffer
                            ADL_SLS_OFFSET[] slsOffsetArray = new ADL_SLS_OFFSET[numSLSOffset];
                            if (numSLSOffset > 0)
                            {
                                IntPtr currentSLSOffsetBuffer = slsOffsetBuffer;
                                for (int i = 0; i < numSLSOffset; i++)
                                {
                                    // build a structure in the array slot
                                    slsOffsetArray[i] = new ADL_SLS_OFFSET();
                                    // fill the array slot structure with the data from the buffer
                                    slsOffsetArray[i] = (ADL_SLS_OFFSET)Marshal.PtrToStructure(currentSLSOffsetBuffer, typeof(ADL_SLS_OFFSET));
                                    // destroy the bit of memory we no longer need
                                    //Marshal.DestroyStructure(currentDisplayTargetBuffer, typeof(ADL_ADAPTER_INFOX2));
                                    // advance the buffer forwards to the next object
                                    currentSLSOffsetBuffer = (IntPtr)((long)currentSLSOffsetBuffer + Marshal.SizeOf(slsOffsetArray[i]));
                                }
                                // Free the memory used by the buffer                        
                                Marshal.FreeCoTaskMem(slsOffsetBuffer);

                                // Add the slsOffsets to the config we want to store
                                mySLSMapConfig.SLSOffsets = slsOffsetArray.ToList();

                            }
                            else
                            {
                                // Add the slsTarget to the config we want to store
                                mySLSMapConfig.SLSOffsets = new List<ADL_SLS_OFFSET>();
                            }

                            // Now we try to calculate whether SLS is enabled
                            // NFI why they don't just add a ADL2_Display_SLSMapConfig_GetState function to make this easy for ppl :(
                            // NVIDIA make it easy, why can't you Intel?

                            // Logic cribbed from https://github.com/elitak/Intel-adl-sdk/blob/master/Sample/Eyefinity/ati_eyefinity.c
                            // Go through each display Target
                            foreach (var displayTarget in displayTargetArray)
                            {
                                // Get the current Display Modes for this adapter/display combination
                                int numDisplayModes;
                                IntPtr displayModeBuffer;
                                ADLRet = ADLImport.ADL2_Display_Modes_Get(
                                                                            _adlContextHandle,
                                                                                oneAdapter.AdapterIndex,
                                                                                displayTarget.DisplayID.DisplayLogicalIndex,
                                                                                out numDisplayModes,
                                                                                out displayModeBuffer);
                                if (ADLRet == ctl_result_t.ADL_OK)
                                {
                                    SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: ADL2_Display_Modes_Get returned information about the display modes used by display #{displayTarget.DisplayID.DisplayLogicalAdapterIndex} connected to Intel adapter {adapterIndex}.");
                                }
                                else
                                {
                                    SharedLogger.logger.Error($"IntelLibrary/GetIntelDisplayConfig: ERROR - ADL2_Display_Modes_Get returned ADL_STATUS {ADLRet} when trying to get the display modes from Intel adapter {adapterIndex} in the computer.");
                                    continue;
                                }

                                ADL_MODE[] displayModeArray = new ADL_MODE[numDisplayModes];
                                if (numDisplayModes > 0)
                                {
                                    IntPtr currentDisplayModeBuffer = displayModeBuffer;
                                    for (int i = 0; i < numDisplayModes; i++)
                                    {
                                        // build a structure in the array slot
                                        displayModeArray[i] = new ADL_MODE();
                                        // fill the array slot structure with the data from the buffer
                                        displayModeArray[i] = (ADL_MODE)Marshal.PtrToStructure(currentDisplayModeBuffer, typeof(ADL_MODE));
                                        // destroy the bit of memory we no longer need
                                        //Marshal.DestroyStructure(currentDisplayTargetBuffer, typeof(ADL_ADAPTER_INFOX2));
                                        // advance the buffer forwards to the next object
                                        currentDisplayModeBuffer = (IntPtr)((long)currentDisplayModeBuffer + Marshal.SizeOf(displayModeArray[i]));
                                    }
                                    // Free the memory used by the buffer                        
                                    Marshal.FreeCoTaskMem(displayModeBuffer);

                                    // Add the slsOffsets to the config we want to store
                                    //mySLSMapConfig.SLSOffsets = displayModeArray.ToList();

                                }

                                // If Eyefinity is enabled for this adapter, then the display mode of an
                                // attached display target will match one of the SLS display modes reported by
                                // ADL_Display_SLSMapConfig_Get(). The match will either be with "native" SLS 
                                // modes (which are not bezel-compensated), or with "bezel" SLS modes which are.
                                // 
                                // So, simply compare current display mode against all the ones listed for the
                                // SLS native or bezel-compensated modes: if there is a match, then the mode
                                // currently used by this adapter is an Eyefinity/SLS mode, and Eyefinity is enabled.
                                // First check the native SLS mode list
                                // Process the slsOffsetBuffer
                                bool isSlsEnabled = false;
                                bool isBezelCompensatedDisplay = false;
                                foreach (var displayMode in displayModeArray)
                                {
                                    foreach (var nativeMode in nativeModeArray)
                                    {
                                        if (nativeMode.DisplayMode.XRes == displayMode.XRes && nativeMode.DisplayMode.YRes == displayMode.YRes)
                                        {
                                            isSlsEnabled = true;
                                            break;
                                        }

                                    }

                                    // If no match was found, check the bezel-compensated SLS mode list
                                    if (!isSlsEnabled)
                                    {
                                        foreach (var bezelMode in bezelModeArray)
                                        {
                                            if (bezelMode.DisplayMode.XRes == displayMode.XRes && bezelMode.DisplayMode.YRes == displayMode.YRes)
                                            {
                                                isSlsEnabled = true;
                                                isBezelCompensatedDisplay = true;
                                                break;
                                            }
                                        }
                                    }

                                    // Now we check which slot we need to put this display into
                                    if (isSlsEnabled)
                                    {
                                        // SLS is enabled for this display
                                        if (!myDisplayConfig.SlsConfig.SLSEnabledDisplayTargets.Contains(displayMode))
                                        {
                                            myDisplayConfig.SlsConfig.SLSEnabledDisplayTargets.Add(displayMode);
                                        }
                                        // we also update the main IsSLSEnabled so that it is indicated at the top level too

                                        myDisplayConfig.SlsConfig.IsSlsEnabled = true;
                                        SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: Intel Adapter #{oneAdapter.AdapterIndex.ToString()} has a matching SLS grid set! Eyefinity (SLS) is enabled. Setting IsSlsEnabled to true");

                                    }
                                }

                            }

                            // Only Add the mySLSMapConfig to the displayConfig if SLS is enabled
                            if (myDisplayConfig.SlsConfig.IsSlsEnabled)
                            {
                                myDisplayConfig.SlsConfig.SLSMapConfigs.Add(mySLSMapConfig);
                            }

                        }
                        else
                        {
                            // If we get here then there there was no active SLSGrid, meaning Eyefinity is disabled!
                            SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: Intel Adapter #{oneAdapter.AdapterIndex.ToString()} has no active SLS grids set! Eyefinity (SLS) hasn't even been setup yet. Keeping the default IsSlsEnabled value of false.");
                        }
                    }
                    else
                    {
                        // If we get here then there are less than two displays connected. Eyefinity cannot be enabled in this case!
                        SharedLogger.logger.Error($"IntelLibrary/GetIntelDisplayConfig: There are less than two displays connected to this adapter so Eyefinity cannot be enabled.");
                    }


                    int forceDetect = 0;
                    int numDisplays;
                    IntPtr displayInfoBuffer;
                    ADLRet = ADLImport.ADL2_Display_DisplayInfo_Get(_adlContextHandle, adapterIndex, out numDisplays, out displayInfoBuffer, forceDetect);
                    if (ADLRet == ctl_result_t.ADL_OK)
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: ADL2_Display_DisplayInfo_Get returned information about all displaytargets connected to Intel adapter #{adapterIndex}.");
                    }
                    else if (ADLRet == ctl_result_t.ADL_ERR_NULL_POINTER || ADLRet == ctl_result_t.ADL_ERR_NOT_SUPPORTED)
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: ADL2_Display_DisplayInfo_Get returned ADL_ERR_NULL_POINTER so skipping getting display info from Intel adapter #{adapterIndex}.");
                        continue;
                    }
                    else
                    {
                        SharedLogger.logger.Error($"IntelLibrary/PrintActiveConfig: ERROR - ADL2_Display_DisplayInfo_Get returned ADL_STATUS {ADLRet} when trying to get the display target info from Intel adapter #{adapterIndex}.");
                    }

                    ADL_DISPLAY_INFO[] displayInfoArray = { };
                    if (numDisplays > 0)
                    {
                        IntPtr currentDisplayInfoBuffer = displayInfoBuffer;
                        displayInfoArray = new ADL_DISPLAY_INFO[numDisplays];
                        for (int i = 0; i < numDisplays; i++)
                        {
                            // build a structure in the array slot
                            displayInfoArray[i] = new ADL_DISPLAY_INFO();
                            // fill the array slot structure with the data from the buffer
                            displayInfoArray[i] = (ADL_DISPLAY_INFO)Marshal.PtrToStructure(currentDisplayInfoBuffer, typeof(ADL_DISPLAY_INFO));
                            // destroy the bit of memory we no longer need
                            Marshal.DestroyStructure(currentDisplayInfoBuffer, typeof(ADL_DISPLAY_INFO));
                            // advance the buffer forwards to the next object
                            currentDisplayInfoBuffer = (IntPtr)((long)currentDisplayInfoBuffer + Marshal.SizeOf(displayInfoArray[i]));
                            //currentDisplayTargetBuffer = (IntPtr)((long)currentDisplayTargetBuffer + Marshal.SizeOf(displayTargetArray[i]));

                        }
                        // Free the memory used by the buffer                        
                        Marshal.FreeCoTaskMem(displayInfoBuffer);
                    }

                    myDisplayConfig.HdrConfigs = new Dictionary<int, INTEL_HDR_CONFIG>();

                    // Now we need to get all the displays connected to this adapter so that we can get their HDR state
                    foreach (var displayTarget in displayTargetArray)
                    {
                        // We need to skip recording anything that doesn't support color communication
                        // Firstly find the display connector if we can
                        ADL_DISPLAY_CONNECTION_TYPE displayConnector;
                        try
                        {
                            displayConnector = displayInfoArray.First(d => d.DisplayID == displayTarget.DisplayID).DisplayConnector;
                        }
                        catch (Exception ex)
                        {
                            displayConnector = ADL_DISPLAY_CONNECTION_TYPE.Unknown;
                        }
                        SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: Display {displayTarget.DisplayID} on Intel adapter #{adapterIndex} has a {displayConnector} connector.");
                        // Then only get the HDR config stuff if the connection actually suports getting the HDR info!
                        if (!SkippedColorConnectionTypes.Contains(displayConnector))
                        {
                            // Go through each display and see if HDR is supported
                            int supported = 0;
                            int enabled = 0;
                            ADLRet = ADLImport.ADL2_Display_HDRState_Get(_adlContextHandle, adapterIndex, displayTarget.DisplayID, out supported, out enabled);
                            if (ADLRet == ctl_result_t.ADL_OK)
                            {
                                if (supported > 0 && enabled > 0)
                                {
                                    SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: ADL2_Display_HDRState_Get says that display {displayTarget.DisplayID.DisplayLogicalIndex} on adapter {adapterIndex} supports HDR and HDR is enabled.");
                                }
                                else if (supported > 0 && enabled == 0)
                                {
                                    SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: ADL2_Display_HDRState_Get says that display {displayTarget.DisplayID.DisplayLogicalIndex} on adapter {adapterIndex} supports HDR and HDR is NOT enabled.");
                                }
                                else
                                {
                                    SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: ADL2_Display_HDRState_Get says that display {displayTarget.DisplayID.DisplayLogicalIndex} on adapter {adapterIndex} does NOT support HDR.");
                                }
                            }
                            else
                            {
                                SharedLogger.logger.Error($"IntelLibrary/GetIntelDisplayConfig: ERROR - ADL2_Display_HDRState_Get returned ADL_STATUS {ADLRet} when trying to get the display target info from Intel adapter {adapterIndex} in the computer.");
                                throw new IntelLibraryException($"ADL2_Display_HDRState_Get returned ADL_STATUS {ADLRet} when trying to get the display target info from Intel adapter {adapterIndex} in the computer");
                            }

                            INTEL_HDR_CONFIG hdrConfig = new INTEL_HDR_CONFIG();
                            hdrConfig.AdapterIndex = displayTarget.DisplayID.DisplayPhysicalAdapterIndex;
                            hdrConfig.HDREnabled = enabled > 0 ? true : false;
                            hdrConfig.HDRSupported = supported > 0 ? true : false;

                            // Now add this to the HDR config list.                        
                            if (!myDisplayConfig.HdrConfigs.ContainsKey(displayTarget.DisplayID.DisplayLogicalIndex))
                            {
                                // Save the new display config only if we haven't already
                                myDisplayConfig.HdrConfigs.Add(displayTarget.DisplayID.DisplayLogicalIndex, hdrConfig);
                            }
                        }
                    }

                }

                // Add the Intel Display Identifiers
                myDisplayConfig.DisplayIdentifiers = GetCurrentDisplayIdentifiers();
            }
            else
            {
                SharedLogger.logger.Error($"IntelLibrary/GetIntelDisplayConfig: ERROR - Tried to run GetIntelDisplayConfig but the Intel ADL library isn't initialised!");
                throw new IntelLibraryException($"Tried to run GetIntelDisplayConfig but the Intel ADL library isn't initialised!");
            }

            // Return the configuration
            return myDisplayConfig;
        }


        public string PrintActiveConfig()
        {
            string stringToReturn = "";

            // Get the current config
            INTEL_DISPLAY_CONFIG displayConfig = ActiveDisplayConfig;

            stringToReturn += $"****** Intel VIDEO CARDS *******\n";


            if (_initialised)
            {
                // Get the number of Intel adapters that the OS knows about
                int numAdapters = 0;
                ctl_result_t ADLRet = ADLImport.ADL2_Adapter_NumberOfAdapters_Get(_adlContextHandle, out numAdapters);
                if (ADLRet == ctl_result_t.ADL_OK)
                {
                    SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: ADL2_Adapter_NumberOfAdapters_Get returned the number of Intel Adapters the OS knows about ({numAdapters}).");
                }
                else
                {
                    SharedLogger.logger.Error($"IntelLibrary/PrintActiveConfig: ERROR - ADL2_Adapter_NumberOfAdapters_Get returned ADL_STATUS {ADLRet} when trying to get number of Intel adapters in the computer.");
                }

                // Figure out primary adapter
                int primaryAdapterIndex = 0;
                ADLRet = ADLImport.ADL2_Adapter_Primary_Get(_adlContextHandle, out primaryAdapterIndex);
                if (ADLRet == ctl_result_t.ADL_OK)
                {
                    SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: The primary adapter has index {primaryAdapterIndex}.");
                }
                else
                {
                    SharedLogger.logger.Error($"IntelLibrary/PrintActiveConfig: ERROR - ADL2_Adapter_Primary_Get returned ADL_STATUS {ADLRet} when trying to get the primary adapter info from all the Intel adapters in the computer.");
                }

                // Now go through each adapter and get the information we need from it
                for (int adapterIndex = 0; adapterIndex < numAdapters; adapterIndex++)
                {
                    // Skip this adapter if it isn't active
                    int adapterActiveStatus = ADLImport.ADL_FALSE;
                    ADLRet = ADLImport.ADL2_Adapter_Active_Get(_adlContextHandle, adapterIndex, out adapterActiveStatus);
                    if (ADLRet == ctl_result_t.ADL_OK)
                    {
                        if (adapterActiveStatus == ADLImport.ADL_TRUE)
                        {
                            SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: ADL2_Adapter_Active_Get returned ADL_TRUE - Intel Adapter #{adapterIndex} is active! We can continue.");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: ADL2_Adapter_Active_Get returned ADL_FALSE - Intel Adapter #{adapterIndex} is NOT active, so skipping.");
                            continue;
                        }
                    }
                    else
                    {
                        SharedLogger.logger.Warn($"IntelLibrary/PrintActiveConfig: WARNING - ADL2_Adapter_Active_Get returned ADL_STATUS {ADLRet} when trying to see if Intel Adapter #{adapterIndex} is active. Trying to skip this adapter so something at least works.");
                        continue;
                    }

                    // Get the Adapter info for this adapter and put it in the AdapterBuffer
                    SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: Running ADL2_Adapter_AdapterInfoX4_Get to get the information about Intel Adapter #{adapterIndex}.");
                    int numAdaptersInfo = 0;
                    IntPtr adapterInfoBuffer = IntPtr.Zero;
                    ADLRet = ADLImport.ADL2_Adapter_AdapterInfoX4_Get(_adlContextHandle, adapterIndex, out numAdaptersInfo, out adapterInfoBuffer);
                    if (ADLRet == ctl_result_t.ADL_OK)
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: ADL2_Adapter_AdapterInfoX4_Get returned information about Intel Adapter #{adapterIndex}.");
                    }
                    else
                    {
                        SharedLogger.logger.Error($"IntelLibrary/PrintActiveConfig: ERROR - ADL2_Adapter_AdapterInfoX4_Get returned ADL_STATUS {ADLRet} when trying to get the adapter info from Intel Adapter #{adapterIndex}. Trying to skip this adapter so something at least works.");
                        continue;
                    }

                    ADL_ADAPTER_INFOX2[] adapterArray = new ADL_ADAPTER_INFOX2[numAdaptersInfo];
                    if (numAdaptersInfo > 0)
                    {
                        IntPtr currentDisplayTargetBuffer = adapterInfoBuffer;
                        for (int i = 0; i < numAdaptersInfo; i++)
                        {
                            // build a structure in the array slot
                            adapterArray[i] = new ADL_ADAPTER_INFOX2();
                            // fill the array slot structure with the data from the buffer
                            adapterArray[i] = (ADL_ADAPTER_INFOX2)Marshal.PtrToStructure(currentDisplayTargetBuffer, typeof(ADL_ADAPTER_INFOX2));
                            // destroy the bit of memory we no longer need
                            //Marshal.DestroyStructure(currentDisplayTargetBuffer, typeof(ADL_ADAPTER_INFOX2));
                            // advance the buffer forwards to the next object
                            currentDisplayTargetBuffer = (IntPtr)((long)currentDisplayTargetBuffer + Marshal.SizeOf(adapterArray[i]));
                        }
                        // Free the memory used by the buffer                        
                        Marshal.FreeCoTaskMem(adapterInfoBuffer);
                    }

                    SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: Converted ADL2_Adapter_AdapterInfoX4_Get memory buffer into a {adapterArray.Length} long array about Intel Adapter #{adapterIndex}.");

                    //INTEL_ADAPTER_CONFIG savedAdapterConfig = new INTEL_ADAPTER_CONFIG();
                    ADL_ADAPTER_INFOX2 oneAdapter = adapterArray[0];
                    if (oneAdapter.Exist != 1)
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: Intel Adapter #{oneAdapter.AdapterIndex.ToString()} doesn't exist at present so skipping detection for this adapter.");
                        continue;
                    }

                    // Print out what we need
                    stringToReturn += $"Adapter #{adapterIndex}\n";
                    stringToReturn += $"Adapter Exists: {oneAdapter.Exist}\n";
                    stringToReturn += $"Adapter Present: {oneAdapter.Present}\n";
                    stringToReturn += $"Adapter Name: {oneAdapter.AdapterName}\n";
                    stringToReturn += $"Adapter Display Name: {oneAdapter.DisplayName}\n";
                    stringToReturn += $"Adapter Driver Path: {oneAdapter.DriverPath}\n";
                    stringToReturn += $"Adapter Driver Path Extension: {oneAdapter.DriverPathExt}\n";
                    stringToReturn += $"Adapter UDID: {oneAdapter.UDID}\n";
                    stringToReturn += $"Adapter Vendor ID: {oneAdapter.VendorID}\n";
                    stringToReturn += $"Adapter PNP String: {oneAdapter.PNPString}\n";
                    stringToReturn += $"Adapter PCI Device Number: {oneAdapter.DeviceNumber}\n";
                    stringToReturn += $"Adapter PCI Bus Number: {oneAdapter.BusNumber}\n";
                    stringToReturn += $"Adapter Windows OS Display Index: {oneAdapter.OSDisplayIndex}\n";
                    stringToReturn += $"Adapter Display Connected: {oneAdapter.DisplayConnectedSet}\n";
                    stringToReturn += $"Adapter Display Mapped in Windows: {oneAdapter.DisplayMappedSet}\n";
                    stringToReturn += $"Adapter Is Forcibly Enabled: {oneAdapter.ForcibleSet}\n";
                    stringToReturn += $"Adapter GetLock is Set: {oneAdapter.GenLockSet}\n";
                    stringToReturn += $"Adapter LDA Display is Set: {oneAdapter.LDADisplaySet}\n";
                    stringToReturn += $"Adapter Display Configuration is stretched horizontally across two displays: {oneAdapter.Manner2HStretchSet}\n";
                    stringToReturn += $"Adapter Display Configuration is stretched vertically across two displays: {oneAdapter.Manner2VStretchSet}\n";
                    stringToReturn += $"Adapter Display Configuration is a clone of another display: {oneAdapter.MannerCloneSet}\n";
                    stringToReturn += $"Adapter Display Configuration is an extension of another display: {oneAdapter.MannerExtendedSet}\n";
                    stringToReturn += $"Adapter Display Configuration is an N Strech across 1 GPU: {oneAdapter.MannerNStretch1GPUSet}\n";
                    stringToReturn += $"Adapter Display Configuration is an N Strech across more than one GPU: {oneAdapter.MannerNStretchNGPUSet}\n";
                    stringToReturn += $"Adapter Display Configuration is a single display: {oneAdapter.MannerSingleSet}\n";
                    stringToReturn += $"Adapter timing override: {oneAdapter.ModeTimingOverrideSet}\n";
                    stringToReturn += $"Adapter has MultiVPU set: {oneAdapter.MultiVPUSet}\n";
                    stringToReturn += $"Adapter has non-local set (it is a remote display): {oneAdapter.NonLocalSet}\n";
                    stringToReturn += $"Adapter is a Show Type Projector: {oneAdapter.ShowTypeProjectorSet}\n\n";

                }

                // Now we still try to get the information from each display we need to print 
                int numDisplayTargets = 0;
                int numDisplayMaps = 0;
                IntPtr displayTargetBuffer = IntPtr.Zero;
                IntPtr displayMapBuffer = IntPtr.Zero;
                ADLRet = ADLImport.ADL2_Display_DisplayMapConfig_Get(_adlContextHandle, -1, out numDisplayMaps, out displayMapBuffer, out numDisplayTargets, out displayTargetBuffer, ADLImport.ADL_DISPLAY_DISPLAYMAP_OPTION_GPUINFO);
                if (ADLRet == ctl_result_t.ADL_OK)
                {
                    SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: ADL2_Display_DisplayMapConfig_Get returned information about all displaytargets connected to all Intel adapters.");

                    // Free the memory used by the buffer to avoid heap corruption
                    Marshal.FreeCoTaskMem(displayMapBuffer);

                    ADL_DISPLAY_TARGET[] displayTargetArray = { };
                    if (numDisplayTargets > 0)
                    {
                        IntPtr currentDisplayTargetBuffer = displayTargetBuffer;
                        //displayTargetArray = new ADL_DISPLAY_TARGET[numDisplayTargets];
                        displayTargetArray = new ADL_DISPLAY_TARGET[numDisplayTargets];
                        for (int i = 0; i < numDisplayTargets; i++)
                        {
                            // build a structure in the array slot
                            displayTargetArray[i] = new ADL_DISPLAY_TARGET();
                            //displayTargetArray[i] = new ADL_DISPLAY_TARGET();
                            // fill the array slot structure with the data from the buffer
                            displayTargetArray[i] = (ADL_DISPLAY_TARGET)Marshal.PtrToStructure(currentDisplayTargetBuffer, typeof(ADL_DISPLAY_TARGET));
                            //displayTargetArray[i] = (ADL_DISPLAY_TARGET)Marshal.PtrToStructure(currentDisplayTargetBuffer, typeof(ADL_DISPLAY_TARGET));
                            // destroy the bit of memory we no longer need
                            Marshal.DestroyStructure(currentDisplayTargetBuffer, typeof(ADL_DISPLAY_TARGET));
                            // advance the buffer forwards to the next object
                            currentDisplayTargetBuffer = (IntPtr)((long)currentDisplayTargetBuffer + Marshal.SizeOf(displayTargetArray[i]));
                            //currentDisplayTargetBuffer = (IntPtr)((long)currentDisplayTargetBuffer + Marshal.SizeOf(displayTargetArray[i]));

                        }
                        // Free the memory used by the buffer                        
                        Marshal.FreeCoTaskMem(displayTargetBuffer);
                    }

                    foreach (var displayTarget in displayTargetArray)
                    {
                        int forceDetect = 0;
                        int numDisplays;
                        IntPtr displayInfoBuffer;
                        ADLRet = ADLImport.ADL2_Display_DisplayInfo_Get(_adlContextHandle, displayTarget.DisplayID.DisplayLogicalAdapterIndex, out numDisplays, out displayInfoBuffer, forceDetect);
                        if (ADLRet == ctl_result_t.ADL_OK)
                        {
                            if (displayTarget.DisplayID.DisplayLogicalAdapterIndex == -1)
                            {
                                SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: ADL2_Display_DisplayInfo_Get returned information about all displaytargets connected to all Intel adapters.");
                                continue;
                            }
                            SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: ADL2_Display_DisplayInfo_Get returned information about all displaytargets connected to all Intel adapters.");
                        }
                        else if (ADLRet == ctl_result_t.ADL_ERR_NULL_POINTER || ADLRet == ctl_result_t.ADL_ERR_NOT_SUPPORTED)
                        {
                            SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: ADL2_Display_DisplayInfo_Get returned ADL_ERR_NULL_POINTER so skipping getting display info from all Intel adapters.");
                            continue;
                        }
                        else
                        {
                            SharedLogger.logger.Error($"IntelLibrary/PrintActiveConfig: ERROR - ADL2_Display_DisplayInfo_Get returned ADL_STATUS {ADLRet} when trying to get the display target info from all Intel adapters in the computer.");
                        }

                        ADL_DISPLAY_INFO[] displayInfoArray = { };
                        if (numDisplays > 0)
                        {
                            IntPtr currentDisplayInfoBuffer = displayInfoBuffer;
                            displayInfoArray = new ADL_DISPLAY_INFO[numDisplays];
                            for (int i = 0; i < numDisplays; i++)
                            {
                                // build a structure in the array slot
                                displayInfoArray[i] = new ADL_DISPLAY_INFO();
                                // fill the array slot structure with the data from the buffer
                                displayInfoArray[i] = (ADL_DISPLAY_INFO)Marshal.PtrToStructure(currentDisplayInfoBuffer, typeof(ADL_DISPLAY_INFO));
                                // destroy the bit of memory we no longer need
                                Marshal.DestroyStructure(currentDisplayInfoBuffer, typeof(ADL_DISPLAY_INFO));
                                // advance the buffer forwards to the next object
                                currentDisplayInfoBuffer = (IntPtr)((long)currentDisplayInfoBuffer + Marshal.SizeOf(displayInfoArray[i]));
                                //currentDisplayTargetBuffer = (IntPtr)((long)currentDisplayTargetBuffer + Marshal.SizeOf(displayTargetArray[i]));

                            }
                            // Free the memory used by the buffer                        
                            Marshal.FreeCoTaskMem(displayInfoBuffer);
                        }

                        // Now we need to get all the displays connected to this adapter so that we can get their HDR state
                        foreach (var displayInfoItem in displayInfoArray)
                        {

                            // Ignore the display if it isn't connected (note: we still need to see if it's actively mapped to windows!)
                            if (!displayInfoItem.DisplayConnectedSet)
                            {
                                continue;
                            }

                            // If the display is not mapped in windows then we only want to skip this display if all alldisplays is false
                            if (!displayInfoItem.DisplayMappedSet)
                            {
                                continue;
                            }

                            stringToReturn += $"\n****** Intel DISPLAY INFO *******\n";
                            stringToReturn += $"Display #{displayInfoItem.DisplayID.DisplayLogicalIndex}\n";
                            stringToReturn += $"Display connected via Adapter #{displayInfoItem.DisplayID.DisplayLogicalAdapterIndex}\n";
                            stringToReturn += $"Display Name: {displayInfoItem.DisplayName}\n";
                            stringToReturn += $"Display Manufacturer Name: {displayInfoItem.DisplayManufacturerName}\n";
                            stringToReturn += $"Display Type: {displayInfoItem.DisplayType.ToString("G")}\n";
                            stringToReturn += $"Display connector: {displayInfoItem.DisplayConnector.ToString("G")}\n";
                            stringToReturn += $"Display controller index: {displayInfoItem.DisplayControllerIndex}\n";
                            stringToReturn += $"Display Connected: {displayInfoItem.DisplayConnectedSet}\n";
                            stringToReturn += $"Display Mapped in Windows: {displayInfoItem.DisplayMappedSet}\n";
                            stringToReturn += $"Display Is Forcibly Enabled: {displayInfoItem.ForcibleSet}\n";
                            stringToReturn += $"Display GetLock is Set: {displayInfoItem.GenLockSet}\n";
                            stringToReturn += $"LDA Display is Set: {displayInfoItem.LDADisplaySet}\n";
                            stringToReturn += $"Display Configuration is stretched horizontally across two displays: {displayInfoItem.Manner2HStretchSet}\n";
                            stringToReturn += $"Display Configuration is stretched vertically across two displays: {displayInfoItem.Manner2VStretchSet}\n";
                            stringToReturn += $"Display Configuration is a clone of another display: {displayInfoItem.MannerCloneSet}\n";
                            stringToReturn += $"Display Configuration is an extension of another display: {displayInfoItem.MannerExtendedSet}\n";
                            stringToReturn += $"Display Configuration is an N Strech across 1 GPU: {displayInfoItem.MannerNStretch1GPUSet}\n";
                            stringToReturn += $"Display Configuration is an N Strech across more than one GPU: {displayInfoItem.MannerNStretchNGPUSet}\n";
                            stringToReturn += $"Display Configuration is a single display: {displayInfoItem.MannerSingleSet}\n";
                            stringToReturn += $"Display timing override: {displayInfoItem.ModeTimingOverrideSet}\n";
                            stringToReturn += $"Display has MultiVPU set: {displayInfoItem.MultiVPUSet}\n";
                            stringToReturn += $"Display has non-local set (it is a remote display): {displayInfoItem.NonLocalSet}\n";
                            stringToReturn += $"Display is a Show Type Projector: {displayInfoItem.ShowTypeProjectorSet}\n\n";

                            // Get some more Display Info (if we can!)
                            ADL_DDC_INFO2 ddcInfo;
                            ADLRet = ADLImport.ADL2_Display_DDCInfo2_Get(_adlContextHandle, displayInfoItem.DisplayID.DisplayLogicalAdapterIndex, displayInfoItem.DisplayID.DisplayLogicalIndex, out ddcInfo);
                            if (ADLRet == ctl_result_t.ADL_OK)
                            {
                                SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: ADL2_Display_DDCInfo2_Get returned information about DDC Information for display {displayInfoItem.DisplayID.DisplayLogicalIndex} connected to Intel adapter {displayInfoItem.DisplayID.DisplayLogicalAdapterIndex}.");
                                if (ddcInfo.SupportsDDC == 1)
                                {
                                    // The display supports DDC and returned some data!
                                    SharedLogger.logger.Trace($"IntelLibrary/PrintActiveConfig: ADL2_Display_DDCInfo2_Get returned information about DDC Information for display {displayInfoItem.DisplayID.DisplayLogicalIndex} connected to Intel adapter {displayInfoItem.DisplayID.DisplayLogicalAdapterIndex}.");
                                    stringToReturn += $"DDC Information: \n";
                                    stringToReturn += $"- Display Name: {ddcInfo.DisplayName}\n";
                                    stringToReturn += $"- Display Manufacturer ID: {ddcInfo.ManufacturerID}\n";
                                    stringToReturn += $"- Display Product ID: {ddcInfo.ProductID}\n";
                                    stringToReturn += $"- Display Serial ID: {ddcInfo.SerialID}\n";
                                    stringToReturn += $"- Display FreeSync Flags: {ddcInfo.FreesyncFlags}\n";
                                    stringToReturn += $"- Display FreeSync HDR Supported: {ddcInfo.FreeSyncHDRSupported}\n";
                                    stringToReturn += $"- Display FreeSync HDR Backlight Supported: {ddcInfo.FreeSyncHDRBacklightSupported}\n";
                                    stringToReturn += $"- Display FreeSync HDR Local Dimming Supported: {ddcInfo.FreeSyncHDRLocalDimmingSupported}\n";
                                    stringToReturn += $"- Display is Digital Device: {ddcInfo.IsDigitalDevice}\n";
                                    stringToReturn += $"- Display is HDMI Audio Device: {ddcInfo.IsHDMIAudioDevice}\n";
                                    stringToReturn += $"- Display is Projector Device: {ddcInfo.IsProjectorDevice}\n";
                                    stringToReturn += $"- Display Supported Colourspace: {ddcInfo.SupportedColorSpace}\n";
                                    stringToReturn += $"- Display Supported HDR: {ddcInfo.SupportedHDR}\n";
                                    stringToReturn += $"- Display Supported Transfer Function: {ddcInfo.SupportedTransferFunction}\n";
                                    stringToReturn += $"- Display Supports AI: {ddcInfo.SupportsAI}\n";
                                    stringToReturn += $"- Display Supports DDC: {ddcInfo.SupportsDDC}\n";
                                    stringToReturn += $"- Display Supports DolbyVision: {ddcInfo.DolbyVisionSupported}\n";
                                    stringToReturn += $"- Display Supports CEA861_3: {ddcInfo.CEA861_3Supported}\n";
                                    stringToReturn += $"- Display Supports sxvYCC601: {ddcInfo.SupportsxvYCC601}\n";
                                    stringToReturn += $"- Display Supports sxvYCC709: {ddcInfo.SupportsxvYCC709}\n";
                                    stringToReturn += $"- Display Average Luminance Data: {ddcInfo.AvgLuminanceData}\n";
                                    stringToReturn += $"- Display Diffuse Screen Reflectance: {ddcInfo.DiffuseScreenReflectance}\n";
                                    stringToReturn += $"- Display Specular Screen Reflectance: {ddcInfo.SpecularScreenReflectance}\n";
                                    stringToReturn += $"- Display Max Backlight Min Luminance: {ddcInfo.MaxBacklightMinLuminanceData}\n";
                                    stringToReturn += $"- Display Max Backlight Max Luminance: {ddcInfo.MaxBacklightMaxLuminanceData}\n";
                                    stringToReturn += $"- Display Min Luminance: {ddcInfo.MinLuminanceData}\n";
                                    stringToReturn += $"- Display Max Luminance: {ddcInfo.MaxLuminanceData}\n";
                                    stringToReturn += $"- Display Min Backlight Min Luminance: {ddcInfo.MinBacklightMinLuminanceData}\n";
                                    stringToReturn += $"- Display Min Backlight Max Luminance: {ddcInfo.MinBacklightMaxLuminanceData}\n";
                                    stringToReturn += $"- Display Min Luminance No Dimming: {ddcInfo.MinLuminanceNoDimmingData}\n";
                                    stringToReturn += $"- Display Native Chromacity Red X: {ddcInfo.NativeDisplayChromaticityRedX}\n";
                                    stringToReturn += $"- Display Native Chromacity Red Y: {ddcInfo.NativeDisplayChromaticityRedY}\n";
                                    stringToReturn += $"- Display Native Chromacity Green X: {ddcInfo.NativeDisplayChromaticityGreenX}\n";
                                    stringToReturn += $"- Display Native Chromacity Green Y: {ddcInfo.NativeDisplayChromaticityGreenY}\n";
                                    stringToReturn += $"- Display Native Chromacity Blue X: {ddcInfo.NativeDisplayChromaticityBlueX}\n";
                                    stringToReturn += $"- Display Native Chromacity Blue Y: {ddcInfo.NativeDisplayChromaticityBlueY}\n";
                                    stringToReturn += $"- Display Native Chromacity White X: {ddcInfo.NativeDisplayChromaticityWhiteX}\n";
                                    stringToReturn += $"- Display Native Chromacity White Y: {ddcInfo.NativeDisplayChromaticityWhiteY}\n";
                                    stringToReturn += $"- Display Packed Pixel Supported: {ddcInfo.PackedPixelSupported}\n";
                                    stringToReturn += $"- Display Panel Pixel Format: {ddcInfo.PanelPixelFormat}\n";
                                    stringToReturn += $"- Display Pixel Format Limited Range: {ddcInfo.PixelFormatLimitedRange}\n";
                                    stringToReturn += $"- Display PTMCx: {ddcInfo.PTMCx}\n";
                                    stringToReturn += $"- Display PTMCy: {ddcInfo.PTMCy}\n";
                                    stringToReturn += $"- Display PTM Refresh Rate: {ddcInfo.PTMRefreshRate}\n";

                                    stringToReturn += $"- Display Serial ID: {ddcInfo.SerialID}\n";
                                }

                            }

                        }
                    }

                }

                stringToReturn += $"\n****** Intel EYEFINITY (SLS) *******\n";
                if (displayConfig.SlsConfig.IsSlsEnabled)
                {
                    stringToReturn += $"Intel Eyefinity is Enabled\n";
                    if (displayConfig.SlsConfig.SLSMapConfigs.Count > 1)
                    {
                        stringToReturn += $"There are {displayConfig.SlsConfig.SLSMapConfigs.Count} Intel Eyefinity (SLS) configurations in use.\n";
                    }
                    if (displayConfig.SlsConfig.SLSMapConfigs.Count == 1)
                    {
                        stringToReturn += $"There is 1 Intel Eyefinity (SLS) configurations in use.\n";
                    }
                    else
                    {
                        stringToReturn += $"There are no Intel Eyefinity (SLS) configurations in use.\n";
                    }

                    int count = 0;
                    foreach (var slsMap in displayConfig.SlsConfig.SLSMapConfigs)
                    {
                        stringToReturn += $"NOTE: This Eyefinity (SLS) screen will be treated as a single display by Windows.\n";
                        stringToReturn += $"The Intel Eyefinity (SLS) Grid Topology #{count} is {slsMap.SLSMap.Grid.SLSGridColumn} Columns x {slsMap.SLSMap.Grid.SLSGridRow} Rows\n";
                        stringToReturn += $"The Intel Eyefinity (SLS) Grid Topology #{count} involves {slsMap.SLSMap.NumSLSTarget} Displays\n";
                    }

                }
                else
                {
                    stringToReturn += $"Intel Eyefinity (SLS) is Disabled\n";
                }

            }
            else
            {
                SharedLogger.logger.Error($"IntelLibrary/PrintActiveConfig: ERROR - Tried to run GetSomeDisplayIdentifiers but the Intel ADL library isn't initialised!");
                throw new IntelLibraryException($"Tried to run PrintActiveConfig but the Intel ADL library isn't initialised!");
            }



            stringToReturn += $"\n\n";
            // Now we also get the Windows CCD Library info, and add it to the above
            stringToReturn += WinLibrary.GetLibrary().PrintActiveConfig();

            return stringToReturn;
        }

        public bool SetActiveConfig(INTEL_DISPLAY_CONFIG displayConfig)
        {

            if (_initialised)
            {
                // Set the initial state of the ADL_STATUS
                ctl_result_t ADLRet = 0;

                // set the display locations
                if (displayConfig.SlsConfig.IsSlsEnabled)
                {
                    // We need to change to an Eyefinity (SLS) profile, so we need to apply the new SLS Topologies
                    SharedLogger.logger.Trace($"IntelLibrary/SetActiveConfig: SLS is enabled in the new display configuration, so we need to set it");

                    foreach (INTEL_SLSMAP_CONFIG slsMapConfig in displayConfig.SlsConfig.SLSMapConfigs)
                    {
                        // Attempt to turn on this SLS Map Config if it exists in the Intel Radeon driver config database
                        ADLRet = ADLImport.ADL2_Display_SLSMapConfig_SetState(_adlContextHandle, slsMapConfig.SLSMap.AdapterIndex, slsMapConfig.SLSMap.SLSMapIndex, ADLImport.ADL_TRUE);
                        if (ADLRet == ctl_result_t.ADL_OK)
                        {
                            SharedLogger.logger.Trace($"IntelLibrary/SetActiveConfig: ADL2_Display_SLSMapConfig_SetState successfully set the SLSMAP with index {slsMapConfig.SLSMap.SLSMapIndex} to TRUE for adapter {slsMapConfig.SLSMap.AdapterIndex}.");
                        }
                        else
                        {
                            SharedLogger.logger.Error($"IntelLibrary/SetActiveConfig: ERROR - ADL2_Display_SLSMapConfig_SetState returned ADL_STATUS {ADLRet} when trying to set the SLSMAP with index {slsMapConfig.SLSMap.SLSMapIndex} to TRUE for adapter {slsMapConfig.SLSMap.AdapterIndex}.");

                            // If we get an error with just tturning it on, then we need to actually try to created a new Eyefinity map and then enable it
                            // If we reach this stage, then the user has discarded the Intel Eyefinity mode in Intel due to a bad UI design, and we need to work around that slight issue.
                            // (BTW that's FAR to easy to do in the Intel Radeon GUI)
                            // NOTE: There is a slight issue with way of doing things. Although we create a much more robust way of working, we also will never ever actually use the Eyefinity config as saved.
                            //       Instead, we will always drop through to creating an Eyefinity config each time, the only saving grace being that the Intel Driver is smart enough to notice this and it will reuse the same SLSMapIndex number.
                            //       This at least means that we won't keep filling the Intel Driver up with additional EYefinity configs! It will instaed only add one more additional Intel Config if it works this way.

                            int supportedSLSLayoutImageMode;
                            int reasonForNotSupportSLS;
                            ADLRet = ADLImport.ADL2_Display_SLSMapConfig_Valid(_adlContextHandle, slsMapConfig.SLSMap.AdapterIndex, slsMapConfig.SLSMap, slsMapConfig.SLSTargets.Count, slsMapConfig.SLSTargets.ToArray(), out supportedSLSLayoutImageMode, out reasonForNotSupportSLS, ADLImport.ADL_DISPLAY_SLSMAPCONFIG_CREATE_OPTION_RELATIVETO_CURRENTANGLE);
                            if (ADLRet == ctl_result_t.ADL_OK)
                            {
                                SharedLogger.logger.Trace($"IntelLibrary/SetActiveConfig: ADL2_Display_SLSMapConfig_Valid successfully validated a new SLSMAP config for adapter {slsMapConfig.SLSMap.AdapterIndex}.");
                            }
                            else
                            {
                                SharedLogger.logger.Error($"IntelLibrary/SetActiveConfig: ERROR - ADL2_Display_SLSMapConfig_Valid returned ADL_STATUS {ADLRet} when trying to create a new SLSMAP for adapter {slsMapConfig.SLSMap.AdapterIndex}.");
                                return false;
                            }

                            // Create and apply the new SLSMap
                            int newSlsMapIndex;
                            ADLRet = ADLImport.ADL2_Display_SLSMapConfig_Create(_adlContextHandle, slsMapConfig.SLSMap.AdapterIndex, slsMapConfig.SLSMap, slsMapConfig.SLSTargets.Count, slsMapConfig.SLSTargets.ToArray(), slsMapConfig.BezelModePercent, out newSlsMapIndex, ADLImport.ADL_DISPLAY_SLSMAPCONFIG_CREATE_OPTION_RELATIVETO_CURRENTANGLE);
                            if (ADLRet == ctl_result_t.ADL_OK)
                            {
                                if (newSlsMapIndex != -1)
                                {
                                    SharedLogger.logger.Trace($"IntelLibrary/SetActiveConfig: ADL2_Display_SLSMapConfig_Create successfully created the new SLSMAP we just created with index {newSlsMapIndex} to TRUE for adapter {slsMapConfig.SLSMap.AdapterIndex}.");

                                    // At this point we have created a new Intel Eyefinity Config
                                }
                                else
                                {
                                    SharedLogger.logger.Error($"IntelLibrary/SetActiveConfig: ERROR - ADL2_Display_SLSMapConfig_Create returned ADL_STATUS {ADLRet} but the returned SLSMapIndex was -1, which indicates that the new SLSMAP failed to create for adapter {slsMapConfig.SLSMap.AdapterIndex}.");
                                }
                            }
                            else
                            {
                                SharedLogger.logger.Error($"IntelLibrary/SetActiveConfig: ERROR - ADL2_Display_SLSMapConfig_Create returned ADL_STATUS {ADLRet} when trying to create a new SLSMAP for adapter {slsMapConfig.SLSMap.AdapterIndex}.");
                                return false;
                            }

                        }

                    }

                }
                else
                {
                    // We need to change to a plain, non-Eyefinity (SLS) profile, so we need to disable any SLS Topologies if they are being used
                    SharedLogger.logger.Trace($"IntelLibrary/SetActiveConfig: SLS is not used in the new display configuration, so we need to set it to disabled if it's configured currently");

                    if (ActiveDisplayConfig.SlsConfig.IsSlsEnabled)
                    {
                        // We need to disable the current Eyefinity (SLS) profile to turn it off
                        SharedLogger.logger.Trace($"IntelLibrary/SetActiveConfig: SLS is enabled in the current display configuration, so we need to turn it off");

                        foreach (INTEL_SLSMAP_CONFIG slsMapConfig in ActiveDisplayConfig.SlsConfig.SLSMapConfigs)
                        {
                            // Turn off this SLS Map Config
                            ADLRet = ADLImport.ADL2_Display_SLSMapConfig_SetState(_adlContextHandle, slsMapConfig.SLSMap.AdapterIndex, slsMapConfig.SLSMap.SLSMapIndex, ADLImport.ADL_FALSE);
                            if (ADLRet == ctl_result_t.ADL_OK)
                            {
                                SharedLogger.logger.Trace($"IntelLibrary/SetActiveConfig: ADL2_Display_SLSMapConfig_SetState successfully disabled the SLSMAP with index {slsMapConfig.SLSMap.SLSMapIndex} for adapter {slsMapConfig.SLSMap.AdapterIndex}.");
                            }
                            else
                            {
                                SharedLogger.logger.Error($"IntelLibrary/SetActiveConfig: ERROR - ADL2_Display_SLSMapConfig_SetState returned ADL_STATUS {ADLRet} when trying to set the SLSMAP with index {slsMapConfig.SLSMap.SLSMapIndex} to FALSE for adapter {slsMapConfig.SLSMap.AdapterIndex}.");
                                return false;
                            }

                        }
                    }

                }

            }
            else
            {
                SharedLogger.logger.Error($"IntelLibrary/SetActiveConfig: ERROR - Tried to run SetActiveConfig but the Intel ADL library isn't initialised!");
                throw new IntelLibraryException($"Tried to run SetActiveConfig but the Intel ADL library isn't initialised!");
            }

            return true;
        }


        public bool SetActiveConfigOverride(INTEL_DISPLAY_CONFIG displayConfig)
        {
            if (_initialised)
            {
                // Set the initial state of the ADL_STATUS
                ctl_result_t ADLRet = 0;

                // We want to set the Intel HDR settings now
                // We got through each of the attached displays and set the HDR

                // Go through each of the HDR configs we have
                foreach (var hdrConfig in displayConfig.HdrConfigs)
                {
                    // Try and find the HDR config displays in the list of currently connected displays
                    foreach (var displayInfoItem in ActiveDisplayConfig.DisplayTargets)
                    {
                        try
                        {
                            // If we find the HDR config display in the list of currently connected displays then try to set the HDR setting we recorded earlier
                            if (hdrConfig.Key == displayInfoItem.DisplayID.DisplayLogicalIndex)
                            {
                                if (hdrConfig.Value.HDREnabled)
                                {
                                    ADLRet = ADLImport.ADL2_Display_HDRState_Set(_adlContextHandle, hdrConfig.Value.AdapterIndex, displayInfoItem.DisplayID, 1);
                                    if (ADLRet == ctl_result_t.ADL_OK)
                                    {
                                        SharedLogger.logger.Trace($"IntelLibrary/SetActiveConfigOverride: ADL2_Display_HDRState_Set was able to turn on HDR for display {displayInfoItem.DisplayID.DisplayLogicalIndex}.");
                                    }
                                    else
                                    {
                                        SharedLogger.logger.Error($"IntelLibrary/SetActiveConfigOverride: ADL2_Display_HDRState_Set was NOT able to turn on HDR for display {displayInfoItem.DisplayID.DisplayLogicalIndex}.");
                                    }
                                }
                                else
                                {
                                    ADLRet = ADLImport.ADL2_Display_HDRState_Set(_adlContextHandle, hdrConfig.Value.AdapterIndex, displayInfoItem.DisplayID, 0);
                                    if (ADLRet == ctl_result_t.ADL_OK)
                                    {
                                        SharedLogger.logger.Trace($"IntelLibrary/SetActiveConfigOverride: ADL2_Display_HDRState_Set was able to turn off HDR for display {displayInfoItem.DisplayID.DisplayLogicalIndex}.");
                                    }
                                    else
                                    {
                                        SharedLogger.logger.Error($"IntelLibrary/SetActiveConfigOverride: ADL2_Display_HDRState_Set was NOT able to turn off HDR for display {displayInfoItem.DisplayID.DisplayLogicalIndex}.");
                                    }
                                }
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Error(ex, $"IntelLibrary/GetIntelDisplayConfig: Exception! ADL2_Display_HDRState_Set was NOT able to change HDR for display {displayInfoItem.DisplayID.DisplayLogicalIndex}.");
                            continue;
                        }
                    }

                }
            }
            else
            {
                SharedLogger.logger.Error($"IntelLibrary/SetActiveConfig: ERROR - Tried to run SetActiveConfigOverride but the Intel ADL library isn't initialised!");
                throw new IntelLibraryException($"Tried to run SetActiveConfigOverride but the Intel ADL library isn't initialised!");
            }
            return true;
        }



        public bool IsActiveConfig(INTEL_DISPLAY_CONFIG displayConfig)
        {

            // Check whether the display config is in use now
            SharedLogger.logger.Trace($"IntelLibrary/IsActiveConfig: Checking whether the display configuration is already being used.");
            if (displayConfig.Equals(_activeDisplayConfig))
            {
                SharedLogger.logger.Trace($"IntelLibrary/IsActiveConfig: The display configuration is already being used (supplied displayConfig Equals currentWindowsDisplayConfig)");
                return true;
            }
            else
            {
                SharedLogger.logger.Trace($"IntelLibrary/IsActiveConfig: The display configuration is NOT currently in use (supplied displayConfig Equals currentWindowsDisplayConfig)");
                return false;
            }

        }

        public bool IsValidConfig(INTEL_DISPLAY_CONFIG displayConfig)
        {
            // We want to check the Intel Eyefinity (SLS) config is valid
            SharedLogger.logger.Trace($"IntelLibrary/IsValidConfig: Testing whether the display configuration is valid");
            // 
            if (displayConfig.SlsConfig.IsSlsEnabled)
            {
                // At the moment we just assume the config is true so we try to use it
                return true;
            }
            else
            {
                // Its not a Mosaic topology, so we just let it pass, as it's windows settings that matter.
                return true;
            }
        }

        public bool IsPossibleConfig(INTEL_DISPLAY_CONFIG displayConfig)
        {
            // We want to check the Intel profile can be used now
            SharedLogger.logger.Trace($"IntelLibrary/IsPossibleConfig: Testing whether the Intel display configuration is possible to be used now");

            // Check that we have all the displayConfig DisplayIdentifiers we need available now
            if (displayConfig.DisplayIdentifiers.All(value => _allConnectedDisplayIdentifiers.Contains(value)))
            {
                SharedLogger.logger.Trace($"IntelLibrary/IsPossibleConfig: Success! The Intel display configuration is possible to be used now");
                return true;
            }
            else
            {
                SharedLogger.logger.Trace($"IntelLibrary/IsPossibleConfig: Uh oh! The Inteldisplay configuration is possible cannot be used now");
                return false;
            }
        }

        public List<string> GetCurrentDisplayIdentifiers()
        {
            SharedLogger.logger.Trace($"IntelLibrary/GetCurrentDisplayIdentifiers: Getting the current display identifiers for the displays in use now");
            bool allDisplays = false;
            return GetSomeDisplayIdentifiers(allDisplays);
        }

        public List<string> GetAllConnectedDisplayIdentifiers()
        {
            SharedLogger.logger.Trace($"IntelLibrary/GetAllConnectedDisplayIdentifiers: Getting all the display identifiers that can possibly be used");
            bool allDisplays = true;
            _allConnectedDisplayIdentifiers = GetSomeDisplayIdentifiers(allDisplays);

            return _allConnectedDisplayIdentifiers;
        }

        private List<string> GetSomeDisplayIdentifiers(bool allDisplays = false)
        {
            SharedLogger.logger.Debug($"IntelLibrary/GetSomeDisplayIdentifiers: Generating unique Display Identifiers");

            List<string> displayIdentifiers = new List<string>();

            if (_initialised)
            {
                // Get the number of Intel adapters that the OS knows about
                int numAdapters = 0;
                ctl_result_t ADLRet = ADLImport.ADL2_Adapter_NumberOfAdapters_Get(_adlContextHandle, out numAdapters);
                if (ADLRet == ctl_result_t.ADL_OK)
                {
                    SharedLogger.logger.Trace($"IntelLibrary/GetSomeDisplayIdentifiers: ADL2_Adapter_NumberOfAdapters_Get returned the number of Intel Adapters the OS knows about ({numAdapters}).");
                }
                else
                {
                    SharedLogger.logger.Error($"IntelLibrary/GetSomeDisplayIdentifiers: ERROR - ADL2_Adapter_NumberOfAdapters_Get returned ADL_STATUS {ADLRet} when trying to get number of Intel adapters in the computer.");
                    throw new IntelLibraryException($"GetSomeDisplayIdentifiers returned ADL_STATUS {ADLRet} when trying to get number of Intel adapters in the computer");
                }

                // Figure out primary adapter
                int primaryAdapterIndex = 0;
                ADLRet = ADLImport.ADL2_Adapter_Primary_Get(_adlContextHandle, out primaryAdapterIndex);
                if (ADLRet == ctl_result_t.ADL_OK)
                {
                    SharedLogger.logger.Trace($"IntelLibrary/ADL2_Adapter_Primary_Get: The primary adapter has index {primaryAdapterIndex}.");
                }
                else
                {
                    SharedLogger.logger.Error($"IntelLibrary/GetSomeDisplayIdentifiers: ERROR - ADL2_Adapter_Primary_Get returned ADL_STATUS {ADLRet} when trying to get the primary adapter info from all the Intel adapters in the computer.");
                    throw new IntelLibraryException($"GetSomeDisplayIdentifiers returned ADL_STATUS {ADLRet} when trying to get the adapter info from all the Intel adapters in the computer");
                }

                // Now go through each adapter and get the information we need from it
                for (int adapterIndex = 0; adapterIndex < numAdapters; adapterIndex++)
                {
                    // Skip this adapter if it isn't active
                    int adapterActiveStatus = ADLImport.ADL_FALSE;
                    ADLRet = ADLImport.ADL2_Adapter_Active_Get(_adlContextHandle, adapterIndex, out adapterActiveStatus);
                    if (ADLRet == ctl_result_t.ADL_OK)
                    {
                        if (adapterActiveStatus == ADLImport.ADL_TRUE)
                        {
                            SharedLogger.logger.Trace($"IntelLibrary/GetSomeDisplayIdentifiers: ADL2_Adapter_Active_Get returned ADL_TRUE - Intel Adapter #{adapterIndex} is active! We can continue.");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"IntelLibrary/GetSomeDisplayIdentifiers: ADL2_Adapter_Active_Get returned ADL_FALSE - Intel Adapter #{adapterIndex} is NOT active, so skipping.");
                            continue;
                        }
                    }
                    else
                    {
                        SharedLogger.logger.Warn($"IntelLibrary/GetSomeDisplayIdentifiers: WARNING - ADL2_Adapter_Active_Get returned ADL_STATUS {ADLRet} when trying to see if Intel Adapter #{adapterIndex} is active. Trying to skip this adapter so something at least works.");
                        continue;
                    }

                    // Get the Adapter info for this adapter and put it in the AdapterBuffer
                    SharedLogger.logger.Trace($"IntelLibrary/GetSomeDisplayIdentifiers: Running ADL2_Adapter_AdapterInfoX4_Get to get the information about Intel Adapter #{adapterIndex}.");
                    int numAdaptersInfo = 0;
                    IntPtr adapterInfoBuffer = IntPtr.Zero;
                    ADLRet = ADLImport.ADL2_Adapter_AdapterInfoX4_Get(_adlContextHandle, adapterIndex, out numAdaptersInfo, out adapterInfoBuffer);
                    if (ADLRet == ctl_result_t.ADL_OK)
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/GetSomeDisplayIdentifiers: ADL2_Adapter_AdapterInfoX4_Get returned information about Intel Adapter #{adapterIndex}.");
                    }
                    else
                    {
                        SharedLogger.logger.Error($"IntelLibrary/GetSomeDisplayIdentifiers: ERROR - ADL2_Adapter_AdapterInfoX4_Get returned ADL_STATUS {ADLRet} when trying to get the adapter info from Intel Adapter #{adapterIndex}. Trying to skip this adapter so something at least works.");
                        continue;
                    }

                    ADL_ADAPTER_INFOX2[] adapterArray = new ADL_ADAPTER_INFOX2[numAdaptersInfo];
                    if (numAdaptersInfo > 0)
                    {
                        IntPtr currentDisplayTargetBuffer = adapterInfoBuffer;
                        for (int i = 0; i < numAdaptersInfo; i++)
                        {
                            // build a structure in the array slot
                            adapterArray[i] = new ADL_ADAPTER_INFOX2();
                            // fill the array slot structure with the data from the buffer
                            adapterArray[i] = (ADL_ADAPTER_INFOX2)Marshal.PtrToStructure(currentDisplayTargetBuffer, typeof(ADL_ADAPTER_INFOX2));
                            // destroy the bit of memory we no longer need
                            //Marshal.DestroyStructure(currentDisplayTargetBuffer, typeof(ADL_ADAPTER_INFOX2));
                            // advance the buffer forwards to the next object
                            currentDisplayTargetBuffer = (IntPtr)((long)currentDisplayTargetBuffer + Marshal.SizeOf(adapterArray[i]));
                        }
                        // Free the memory used by the buffer                        
                        Marshal.FreeCoTaskMem(adapterInfoBuffer);
                    }

                    SharedLogger.logger.Trace($"IntelLibrary/GetSomeDisplayIdentifiers: Converted ADL2_Adapter_AdapterInfoX4_Get memory buffer into a {adapterArray.Length} long array about Intel Adapter #{adapterIndex}.");

                    //INTEL_ADAPTER_CONFIG savedAdapterConfig = new INTEL_ADAPTER_CONFIG();
                    ADL_ADAPTER_INFOX2 oneAdapter = adapterArray[0];
                    if (oneAdapter.Exist != 1)
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/GetSomeDisplayIdentifiers: Intel Adapter #{oneAdapter.AdapterIndex.ToString()} doesn't exist at present so skipping detection for this adapter.");
                        continue;
                    }

                    // Only skip non-present displays if we want all displays information
                    if (allDisplays && oneAdapter.Present != 1)
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/GetSomeDisplayIdentifiers: Intel Adapter #{oneAdapter.AdapterIndex.ToString()} isn't enabled at present so skipping detection for this adapter.");
                        continue;
                    }

                    // Now we still try to get the information we need for the Display Identifiers
                    // Go grab the DisplayMaps and DisplayTargets as that is useful infor for creating screens
                    int numDisplayTargets = 0;
                    int numDisplayMaps = 0;
                    IntPtr displayTargetBuffer = IntPtr.Zero;
                    IntPtr displayMapBuffer = IntPtr.Zero;
                    ADLRet = ADLImport.ADL2_Display_DisplayMapConfig_Get(_adlContextHandle, adapterIndex, out numDisplayMaps, out displayMapBuffer, out numDisplayTargets, out displayTargetBuffer, ADLImport.ADL_DISPLAY_DISPLAYMAP_OPTION_GPUINFO);
                    if (ADLRet == ctl_result_t.ADL_OK)
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: ADL2_Display_DisplayMapConfig_Get returned information about all displaytargets connected to Intel adapter {adapterIndex}.");
                    }
                    else
                    {
                        SharedLogger.logger.Error($"IntelLibrary/GetIntelDisplayConfig: ERROR - ADL2_Display_DisplayMapConfig_Get returned ADL_STATUS {ADLRet} when trying to get the display target info from Intel adapter {adapterIndex} in the computer.");
                        continue;
                    }

                    ADL_DISPLAY_TARGET[] displayTargetArray = { };
                    if (numDisplayTargets > 0)
                    {
                        IntPtr currentDisplayTargetBuffer = displayTargetBuffer;
                        //displayTargetArray = new ADL_DISPLAY_TARGET[numDisplayTargets];
                        displayTargetArray = new ADL_DISPLAY_TARGET[numDisplayTargets];
                        for (int i = 0; i < numDisplayTargets; i++)
                        {
                            // build a structure in the array slot
                            displayTargetArray[i] = new ADL_DISPLAY_TARGET();
                            //displayTargetArray[i] = new ADL_DISPLAY_TARGET();
                            // fill the array slot structure with the data from the buffer
                            displayTargetArray[i] = (ADL_DISPLAY_TARGET)Marshal.PtrToStructure(currentDisplayTargetBuffer, typeof(ADL_DISPLAY_TARGET));
                            //displayTargetArray[i] = (ADL_DISPLAY_TARGET)Marshal.PtrToStructure(currentDisplayTargetBuffer, typeof(ADL_DISPLAY_TARGET));
                            // destroy the bit of memory we no longer need
                            Marshal.DestroyStructure(currentDisplayTargetBuffer, typeof(ADL_DISPLAY_TARGET));
                            // advance the buffer forwards to the next object
                            currentDisplayTargetBuffer = (IntPtr)((long)currentDisplayTargetBuffer + Marshal.SizeOf(displayTargetArray[i]));
                            //currentDisplayTargetBuffer = (IntPtr)((long)currentDisplayTargetBuffer + Marshal.SizeOf(displayTargetArray[i]));

                        }
                        // Free the memory used by the buffer                        
                        Marshal.FreeCoTaskMem(displayTargetBuffer);
                    }

                    int forceDetect = 0;
                    int numDisplays;
                    IntPtr displayInfoBuffer;
                    ADLRet = ADLImport.ADL2_Display_DisplayInfo_Get(_adlContextHandle, adapterIndex, out numDisplays, out displayInfoBuffer, forceDetect);
                    if (ADLRet == ctl_result_t.ADL_OK)
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: ADL2_Display_DisplayInfo_Get returned information about all displaytargets connected to Intel adapter {adapterIndex}.");
                    }
                    else if (ADLRet == ctl_result_t.ADL_ERR_NULL_POINTER || ADLRet == ctl_result_t.ADL_ERR_NOT_SUPPORTED)
                    {
                        SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: ADL2_Display_DisplayInfo_Get returned ADL_ERR_NULL_POINTER so skipping getting display info from this Intel adapter {adapterIndex}.");
                        continue;
                    }
                    else
                    {
                        SharedLogger.logger.Error($"IntelLibrary/GetIntelDisplayConfig: ERROR - ADL2_Display_DisplayInfo_Get returned ADL_STATUS {ADLRet} when trying to get the display target info from Intel adapter {adapterIndex} in the computer.");
                    }

                    ADL_DISPLAY_INFO[] displayInfoArray = { };
                    if (numDisplays > 0)
                    {
                        IntPtr currentDisplayInfoBuffer = displayInfoBuffer;
                        displayInfoArray = new ADL_DISPLAY_INFO[numDisplays];
                        for (int i = 0; i < numDisplays; i++)
                        {
                            // build a structure in the array slot
                            displayInfoArray[i] = new ADL_DISPLAY_INFO();
                            // fill the array slot structure with the data from the buffer
                            displayInfoArray[i] = (ADL_DISPLAY_INFO)Marshal.PtrToStructure(currentDisplayInfoBuffer, typeof(ADL_DISPLAY_INFO));
                            // destroy the bit of memory we no longer need
                            Marshal.DestroyStructure(currentDisplayInfoBuffer, typeof(ADL_DISPLAY_INFO));
                            // advance the buffer forwards to the next object
                            currentDisplayInfoBuffer = (IntPtr)((long)currentDisplayInfoBuffer + Marshal.SizeOf(displayInfoArray[i]));
                            //currentDisplayTargetBuffer = (IntPtr)((long)currentDisplayTargetBuffer + Marshal.SizeOf(displayTargetArray[i]));

                        }
                        // Free the memory used by the buffer                        
                        Marshal.FreeCoTaskMem(displayInfoBuffer);
                    }


                    // Now we need to get all the displays connected to this adapter so that we can get their HDR state
                    foreach (var displayInfoItem in displayInfoArray)
                    {

                        // Ignore the display if it isn't connected (note: we still need to see if it's actively mapped to windows!)
                        if (!displayInfoItem.DisplayConnectedSet)
                        {
                            continue;
                        }

                        // If the display is not mapped in windows then we only want to skip this display if all alldisplays is false
                        if (!displayInfoItem.DisplayMappedSet && !allDisplays)
                        {
                            continue;
                        }

                        // Create an array of all the important display info we need to create the display identifier
                        List<string> displayInfo = new List<string>();
                        displayInfo.Add("Intel");
                        try
                        {
                            displayInfo.Add(oneAdapter.DeviceNumber.ToString());
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Warn(ex, $"IntelLibrary/GetSomeDisplayIdentifiers: Exception getting Intel Adapter Device Number from video card. Substituting with a # instead");
                            displayInfo.Add("#");
                        }
                        try
                        {
                            displayInfo.Add(oneAdapter.AdapterName);
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Warn(ex, $"IntelLibrary/GetSomeDisplayIdentifiers: Exception getting Intel Adapter Name from video card. Substituting with a # instead");
                            displayInfo.Add("#");
                        }
                        try
                        {
                            displayInfo.Add(displayInfoItem.DisplayConnector.ToString("G"));
                        }
                        catch (Exception ex)
                        {
                            SharedLogger.logger.Warn(ex, $"IntelLibrary/GetSomeDisplayIdentifiers: Exception getting Display Connector from video card. Substituting with a # instead");
                            displayInfo.Add("#");
                        }

                        // Get some more Display Info (if we can!)
                        ADL_DDC_INFO2 ddcInfo = new ADL_DDC_INFO2();
                        ADLRet = ADLImport.ADL2_Display_DDCInfo2_Get(_adlContextHandle, adapterIndex, displayInfoItem.DisplayID.DisplayLogicalIndex, out ddcInfo);
                        if (ADLRet == ctl_result_t.ADL_OK)
                        {
                            SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: ADL2_Display_DDCInfo2_Get returned information about DDC Information for display {displayInfoItem.DisplayID.DisplayLogicalIndex} connected to Intel adapter {adapterIndex}.");
                            if (ddcInfo.SupportsDDC == 1)
                            {
                                // The display supports DDC and returned some data!
                                SharedLogger.logger.Trace($"IntelLibrary/GetIntelDisplayConfig: ADL2_Display_DDCInfo2_Get returned information about DDC Information for display {displayInfoItem.DisplayID.DisplayLogicalIndex} connected to Intel adapter {adapterIndex}.");

                                try
                                {
                                    displayInfo.Add(ddcInfo.ManufacturerID.ToString());
                                }
                                catch (Exception ex)
                                {
                                    SharedLogger.logger.Warn(ex, $"IntelLibrary/GetSomeDisplayIdentifiers: Exception getting Intel Display EDID Manufacturer Code from video card. Substituting with a # instead");
                                    displayInfo.Add("#");
                                }
                                try
                                {
                                    displayInfo.Add(ddcInfo.ProductID.ToString());
                                }
                                catch (Exception ex)
                                {
                                    SharedLogger.logger.Warn(ex, $"IntelLibrary/GetSomeDisplayIdentifiers: Exception getting Intel Display EDID Product Code from video card. Substituting with a # instead");
                                    displayInfo.Add("#");
                                }
                                try
                                {
                                    displayInfo.Add(ddcInfo.DisplayName.ToString());
                                }
                                catch (Exception ex)
                                {
                                    SharedLogger.logger.Warn(ex, $"IntelLibrary/GetSomeDisplayIdentifiers: Exception getting Intel Display Name from video card. Substituting with a # instead");
                                    displayInfo.Add("#");
                                }
                            }
                            else
                            {
                                // The display does NOT support DDC and nothing was returned! We need to find the information some other way!

                                try
                                {
                                    displayInfo.Add(displayInfoItem.DisplayManufacturerName.ToString());
                                }
                                catch (Exception ex)
                                {
                                    SharedLogger.logger.Warn(ex, $"IntelLibrary/GetSomeDisplayIdentifiers: Exception getting Intel Display Manufacturer Name 2 from video card. Substituting with a # instead");
                                    displayInfo.Add("#");
                                }
                                try
                                {
                                    displayInfo.Add(displayInfoItem.DisplayName.ToString());
                                }
                                catch (Exception ex)
                                {
                                    SharedLogger.logger.Warn(ex, $"IntelLibrary/GetSomeDisplayIdentifiers: Exception getting Intel Display Name 2 from video card. Substituting with a # instead");
                                    displayInfo.Add("#");
                                }
                            }
                        }
                        else
                        {
                            SharedLogger.logger.Error($"IntelLibrary/GetIntelDisplayConfig: ERROR - ADL2_Display_DDCInfo2_Get returned ADL_STATUS {ADLRet} when trying to get the display target info from Intel adapter {adapterIndex} in the computer.");

                            // ADL2_Display_DDCInfo2_Get had a problem and nothing was returned! We need to find the information some other way!

                            try
                            {
                                displayInfo.Add(displayInfoItem.DisplayManufacturerName.ToString());
                            }
                            catch (Exception ex)
                            {
                                SharedLogger.logger.Warn(ex, $"IntelLibrary/GetSomeDisplayIdentifiers: Exception getting Intel Display Manufacturer Name 2 from video card. Substituting with a # instead");
                                displayInfo.Add("#");
                            }
                            try
                            {
                                displayInfo.Add(displayInfoItem.DisplayName.ToString());
                            }
                            catch (Exception ex)
                            {
                                SharedLogger.logger.Warn(ex, $"IntelLibrary/GetSomeDisplayIdentifiers: Exception getting Intel Display Name 2 from video card. Substituting with a # instead");
                                displayInfo.Add("#");
                            }
                        }



                        // Create a display identifier out of it
                        string displayIdentifier = String.Join("|", displayInfo);
                        // Add it to the list of display identifiers so we can return it
                        // but only add it if it doesn't already exist. Otherwise we get duplicates :/
                        if (!displayIdentifiers.Contains(displayIdentifier))
                        {
                            displayIdentifiers.Add(displayIdentifier);
                            SharedLogger.logger.Debug($"ProfileRepository/GenerateProfileDisplayIdentifiers: DisplayIdentifier: {displayIdentifier}");
                        }
                    }
                }
            }
            else
            {
                SharedLogger.logger.Error($"IntelLibrary/GetSomeDisplayIdentifiers: ERROR - Tried to run GetSomeDisplayIdentifiers but the Intel ADL library isn't initialised!");
                throw new IntelLibraryException($"Tried to run GetSomeDisplayIdentifiers but the Intel ADL library isn't initialised!");
            }


            // Sort the display identifiers
            displayIdentifiers.Sort();

            return displayIdentifiers;
        }

    }

    [global::System.Serializable]
    public class IntelLibraryException : Exception
    {
        public IntelLibraryException() { }
        public IntelLibraryException(string message) : base(message) { }
        public IntelLibraryException(string message, Exception inner) : base(message, inner) { }
        protected IntelLibraryException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}