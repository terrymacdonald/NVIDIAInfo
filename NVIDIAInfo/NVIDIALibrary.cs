using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using DisplayMagicianShared;
using System.ComponentModel;
using DisplayMagicianShared.Windows;

namespace DisplayMagicianShared.NVIDIA
{
    
    [StructLayout(LayoutKind.Sequential)]
    public struct NVIDIA_MOSAIC_CONFIG : IEquatable<NVIDIA_MOSAIC_CONFIG>
    {
        public bool IsMosaicEnabled;
        public NV_MOSAIC_TOPO_BRIEF MosaicTopologyBrief;
        public NV_MOSAIC_DISPLAY_SETTING_V2 MosaicDisplaySettings;
        public Int32 OverlapX;
        public Int32 OverlapY;
        public NV_MOSAIC_GRID_TOPO_V2[] MosaicGridTopos;
        public UInt32 MosaicGridCount;
        public List<NV_RECT[]> MosaicViewports;
        public UInt32 PrimaryDisplayId;

        public bool Equals(NVIDIA_MOSAIC_CONFIG other)
        => IsMosaicEnabled == other.IsMosaicEnabled &&
           MosaicTopologyBrief.Equals(other.MosaicTopologyBrief) &&
           MosaicDisplaySettings.Equals(other.MosaicDisplaySettings) &&
           OverlapX == other.OverlapX &&
           OverlapY == other.OverlapY &&
           MosaicGridTopos.Equals(other.MosaicGridTopos) &&
           MosaicGridCount == other.MosaicGridCount &&
           MosaicViewports.Equals(other.MosaicViewports) &&
           PrimaryDisplayId == other.PrimaryDisplayId;

        public override int GetHashCode()
        {
            return (IsMosaicEnabled, MosaicTopologyBrief, MosaicDisplaySettings, OverlapX, OverlapY, MosaicGridTopos, MosaicGridCount, MosaicViewports, PrimaryDisplayId).GetHashCode();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NVIDIA_HDR_CONFIG : IEquatable<NVIDIA_HDR_CONFIG>
    {
        public Dictionary<UInt32, NV_HDR_CAPABILITIES_V2> HdrCapabilities;
        public Dictionary<UInt32, NV_HDR_COLOR_DATA_V2> HdrColorData;
        public bool IsNvHdrEnabled;

        public bool Equals(NVIDIA_HDR_CONFIG other)
        => HdrCapabilities == other.HdrCapabilities &&
           HdrColorData == other.HdrColorData &&
           IsNvHdrEnabled == other.IsNvHdrEnabled;

        public override int GetHashCode()
        {
            return (HdrCapabilities, HdrColorData, IsNvHdrEnabled).GetHashCode();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NVIDIA_DISPLAY_CONFIG : IEquatable<NVIDIA_DISPLAY_CONFIG>
    {
        public NVIDIA_MOSAIC_CONFIG MosaicConfig;
        public NVIDIA_HDR_CONFIG HdrConfig;

        public bool Equals(NVIDIA_DISPLAY_CONFIG other)
        => MosaicConfig.Equals(other.MosaicConfig) &&
           HdrConfig.Equals(other.HdrConfig);

        public override int GetHashCode()
        {
            return (MosaicConfig, HdrConfig).GetHashCode();
        }
    }

    class NVIDIALibrary : IDisposable
    {

        // Static members are 'eagerly initialized', that is, 
        // immediately when class is loaded for the first time.
        // .NET guarantees thread safety for static initialization
        private static NVIDIALibrary _instance = new NVIDIALibrary();

        private static WinLibrary _winLibrary = new WinLibrary();

        private bool _initialised = false;
        private bool _haveSessionHandle = false;

        // To detect redundant calls
        private bool _disposed = false;

        // Instantiate a SafeHandle instance.
        private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);
        private IntPtr _nvapiSessionHandle = IntPtr.Zero;

        static NVIDIALibrary() { }
        public NVIDIALibrary()
        {           

            try
            {
                SharedLogger.logger.Trace($"NVIDIALibrary/NVIDIALibrary: Attempting to load the NVIDIA NVAPI DLL");
                // Attempt to prelink all of the NVAPI functions
                //Marshal.PrelinkAll(typeof(NVImport));

                // If we get here then we definitely have the NVIDIA driver available.
                NVAPI_STATUS NVStatus = NVAPI_STATUS.NVAPI_ERROR;
                SharedLogger.logger.Trace("NVIDIALibrary/NVIDIALibrary: Intialising NVIDIA NVAPI library interface");
                // Step 1: Initialise the NVAPI
                try
                {
                    if (NVImport.IsAvailable())
                    {
                        _initialised = true;
                        SharedLogger.logger.Trace($"NVIDIALibrary/NVIDIALibrary: NVIDIA NVAPI library was initialised successfully");
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/NVIDIALibrary: Error intialising NVIDIA NVAPI library. NvAPI_Initialize() returned error code {NVStatus}");
                    }
                    
                }
                catch (Exception ex)
                {
                    SharedLogger.logger.Trace(ex, $"NVIDIALibrary/NVIDIALibrary: Exception intialising NVIDIA NVAPI library. NvAPI_Initialize() caused an exception.");
                }

                // Step 2: Get a session handle that we can use for all other interactions
                /*try
                {
                    NVStatus = NVImport.NvAPI_DRS_CreateSession(out _nvapiSessionHandle);
                    if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                    {
                        _haveSessionHandle = true;
                        SharedLogger.logger.Trace($"NVIDIALibrary/NVIDIALibrary: NVIDIA NVAPI library DRS session handle was created successfully");
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/NVIDIALibrary: Error creating a NVAPI library DRS session handle. NvAPI_DRS_CreateSession() returned error code {NVStatus}");
                    }
                }
                catch (Exception ex)
                {
                    SharedLogger.logger.Trace(ex, $"NVIDIALibrary/NVIDIALibrary: Exception creating a NVAPI library DRS session handle. NvAPI_DRS_CreateSession() caused an exception.");
                }*/

                _winLibrary = WinLibrary.GetLibrary();

            }
            catch (DllNotFoundException ex)
            {
                // If this fires, then the DLL isn't available, so we need don't try to do anything else
                SharedLogger.logger.Info(ex, $"NVIDIALibrary/NVIDIALibrary: Exception trying to load the NVIDIA NVAPI DLL. This generally means you don't have the NVIDIA driver installed.");
            }                        

        }

        ~NVIDIALibrary()
        {
            SharedLogger.logger.Trace("NVIDIALibrary/~NVIDIALibrary: Destroying NVIDIA NVAPI library interface");
            // If the NVAPI library was initialised, then we need to free it up.
            if (_initialised)
            {
                NVAPI_STATUS NVStatus = NVAPI_STATUS.NVAPI_ERROR;
                // If we have a session handle we need to free it up first
                if (_haveSessionHandle)
                {
                    try
                    {
                        //NVStatus = NVImport.NvAPI_DRS_DestorySession(_nvapiSessionHandle);
                        if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                        {
                            _haveSessionHandle = true;
                            SharedLogger.logger.Trace($"NVIDIALibrary/NVIDIALibrary: NVIDIA NVAPI library DRS session handle was successfully destroyed");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/NVIDIALibrary: Error destroying the NVAPI library DRS session handle. NvAPI_DRS_DestorySession() returned error code {NVStatus}");
                        }
                    }
                    catch(Exception ex)
                    {
                        SharedLogger.logger.Trace(ex, $"NVIDIALibrary/NVIDIALibrary: Exception destroying the NVIDIA NVAPI library. NvAPI_DRS_DestorySession() caused an exception.");
                    }                
                }

                try
                {
                    //NVImport.NvAPI_Unload();
                    SharedLogger.logger.Trace($"NVIDIALibrary/NVIDIALibrary: NVIDIA NVAPI library was unloaded successfully");
                }
                catch(Exception ex)
                {
                    SharedLogger.logger.Trace(ex, $"NVIDIALibrary/NVIDIALibrary: Exception unloading the NVIDIA NVAPI library. NvAPI_Unload() caused an exception.");
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

                //NVImport.ADL_Main_Control_Destroy();

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
                return new List<string>() { "10DE" };
            }
        }


        public static NVIDIALibrary GetLibrary()
        {
            return _instance;
        }
        
       

        public NVIDIA_DISPLAY_CONFIG GetActiveConfig()
        {
            SharedLogger.logger.Trace($"NVIDIALibrary/GetActiveConfig: Getting the currently active config");
            bool allDisplays = true;
            return GetNVIDIADisplayConfig(allDisplays);
        }

        private NVIDIA_DISPLAY_CONFIG GetNVIDIADisplayConfig(bool allDisplays = false)
        {
            NVIDIA_DISPLAY_CONFIG myDisplayConfig = new NVIDIA_DISPLAY_CONFIG();
            //myDisplayConfig.AdapterConfigs = new List<NVIDIA_ADAPTER_CONFIG>();

            if (_initialised)
            {
                // Enumerate all the Physical GPUs
                PhysicalGpuHandle[] physicalGpus = new PhysicalGpuHandle[NVImport.NV_MAX_PHYSICAL_GPUS];
                uint physicalGpuCount = 0;
                NVAPI_STATUS NVStatus = NVImport.NvAPI_EnumPhysicalGPUs(ref physicalGpus, out physicalGpuCount);
                if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_EnumPhysicalGPUs returned {physicalGpuCount} Physical GPUs");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Error getting physical GPU count. NvAPI_EnumPhysicalGPUs() returned error code {NVStatus}");
                }

                // Go through the Physical GPUs one by one
                for (uint physicalGpuIndex = 0; physicalGpuIndex < physicalGpuCount; physicalGpuIndex++)
                {
                    //This function retrieves the Quadro status for the GPU (1 if Quadro, 0 if GeForce)
                    uint quadroStatus = 0;
                    NVStatus = NVImport.NvAPI_GPU_GetQuadroStatus(physicalGpus[physicalGpuIndex], out quadroStatus);
                    if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                    {
                        if (quadroStatus == 0)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NVIDIA Video Card is one from the GeForce range");
                        }
                        else if (quadroStatus == 1)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NVIDIA Video Card is one from the Quadro range");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NVIDIA Video Card is neither a GeForce or Quadro range vodeo card (QuadroStatus = {quadroStatus})");
                        }
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Error GETTING qUADRO STATUS. NvAPI_GPU_GetQuadroStatus() returned error code {NVStatus}");
                    }
                }

                // Get current Mosaic Topology settings in brief (check whether Mosaic is on)
                NV_MOSAIC_TOPO_BRIEF mosaicTopoBrief = new NV_MOSAIC_TOPO_BRIEF();
                NV_MOSAIC_DISPLAY_SETTING_V2  mosaicDisplaySettings = new NV_MOSAIC_DISPLAY_SETTING_V2();
                int mosaicOverlapX = 0;
                int mosaicOverlapY = 0;
                NVStatus = NVImport.NvAPI_Mosaic_GetCurrentTopo(ref mosaicTopoBrief, ref mosaicDisplaySettings, out mosaicOverlapX, out mosaicOverlapY);
                if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");                    
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_NOT_SUPPORTED)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_ARGUMENT)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                }

                // Check if there is a topology and that Mosaic is enabled
                if (mosaicTopoBrief.Topo != NV_MOSAIC_TOPO.NV_MOSAIC_TOPO_NONE && mosaicTopoBrief.Enabled == 1)
                {
                    // Mosaic is enabled!
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NVIDIA Mosaic is enabled.");
                    myDisplayConfig.MosaicConfig.MosaicTopologyBrief = mosaicTopoBrief;
                    myDisplayConfig.MosaicConfig.MosaicDisplaySettings = mosaicDisplaySettings;
                    myDisplayConfig.MosaicConfig.OverlapX = mosaicOverlapX;
                    myDisplayConfig.MosaicConfig.OverlapY = mosaicOverlapY;
                    myDisplayConfig.MosaicConfig.IsMosaicEnabled = true;

                    // Get more Mosaic Topology detailed settings
                    NV_MOSAIC_TOPO_GROUP mosaicTopoGroup = new NV_MOSAIC_TOPO_GROUP();
                     NVStatus = NVImport.NvAPI_Mosaic_GetTopoGroup(ref mosaicTopoBrief, ref mosaicTopoGroup);
                     if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                     {
                         SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");
                     }
                     else if (NVStatus == NVAPI_STATUS.NVAPI_NOT_SUPPORTED)
                     {
                         SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                     }
                     else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_ARGUMENT)
                     {
                         SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                     }
                     else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                     {
                         SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                     }
                     else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                     {
                         SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                     }
                     else if (NVStatus == NVAPI_STATUS.NVAPI_INCOMPATIBLE_STRUCT_VERSION)
                     {
                         SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The version of the structure passed in is not supported by this driver. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                     }
                     else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                     {
                         SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                     }
                     else
                     {
                         SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                     }

                    // Figure out how many Mosaic Grid topoligies there are                    
                    uint mosaicGridCount = 0;
                    NVStatus = NVImport.NvAPI_Mosaic_EnumDisplayGrids(ref mosaicGridCount);
                    if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");
                    }

                    // Get Current Mosaic Grid settings using the Grid topologies fnumbers we got before
                    NV_MOSAIC_GRID_TOPO_V2[] mosaicGridTopos = new NV_MOSAIC_GRID_TOPO_V2[mosaicGridCount];                    
                    NVStatus = NVImport.NvAPI_Mosaic_EnumDisplayGrids(ref mosaicGridTopos, ref mosaicGridCount);
                    if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_NOT_SUPPORTED)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_ARGUMENT)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }

                    myDisplayConfig.MosaicConfig.MosaicGridTopos = mosaicGridTopos;
                    myDisplayConfig.MosaicConfig.MosaicGridCount = mosaicGridCount;

                    List<NV_RECT[]> allViewports = new List<NV_RECT[]> { };
                    foreach (NV_MOSAIC_GRID_TOPO_V2 gridTopo in mosaicGridTopos)
                    {
                        // Get Current Mosaic Grid settings using the Grid topologies numbers we got before
                        NV_RECT[] viewports = new NV_RECT[NVImport.NV_MOSAIC_MAX_DISPLAYS];
                        byte bezelCorrected = 0;
                        NVStatus = NVImport.NvAPI_Mosaic_GetDisplayViewportsByResolution(gridTopo.Displays[0].DisplayId, 0, 0, ref viewports, ref bezelCorrected);
                        if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_NOT_SUPPORTED)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_ARGUMENT)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_MOSAIC_NOT_ACTIVE)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The requested action cannot be performed without Mosaic being enabled. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                        }
                        // Save the viewports to the List
                        allViewports.Add(viewports);

                    }

                    myDisplayConfig.MosaicConfig.MosaicViewports = allViewports;
                }
                else
                {
                    // Mosaic isn't enabled
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NVIDIA Mosaic is NOT enabled.");
                    myDisplayConfig.MosaicConfig.MosaicTopologyBrief = mosaicTopoBrief; 
                    myDisplayConfig.MosaicConfig.IsMosaicEnabled = false;
                    myDisplayConfig.MosaicConfig.MosaicGridTopos = new NV_MOSAIC_GRID_TOPO_V2[] { };
                    myDisplayConfig.MosaicConfig.MosaicViewports = new List<NV_RECT[]>();
    }

                // Check if Mosaic is possible and log that so we know if troubleshooting bugs
                if (mosaicTopoBrief.IsPossible == 1)
                {
                    // Mosaic is possible!
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NVIDIA Mosaic is possible. Mosaic topology would be {mosaicTopoBrief.Topo.ToString("G")}.");
                }
                else
                {
                    // Mosaic isn't possible
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NVIDIA Mosaic is NOT possible.");
                }


                // We want to get the primary monitor
                NVStatus = NVImport.NvAPI_DISP_GetGDIPrimaryDisplayId(out UInt32 primaryDisplayId);
                if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_DISP_GetGDIPrimaryDisplayId returned OK.");
                    myDisplayConfig.MosaicConfig.PrimaryDisplayId = primaryDisplayId;
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_NVIDIA_DEVICE_NOT_FOUND)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: There are no NVIDIA video cards in this computer. NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_ARGUMENT)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more argumentss passed in are invalid. NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {NVStatus}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {NVStatus}");
                }

                // We want to get the number of displays we have
                // Go through the Physical GPUs one by one
                for (uint physicalGpuIndex = 0; physicalGpuIndex < physicalGpuCount; physicalGpuIndex++)
                {
                    //This function retrieves the number of display IDs we know about
                    UInt32 displayCount = 0;
                    NVStatus = NVImport.NvAPI_GPU_GetConnectedDisplayIds(physicalGpus[physicalGpuIndex], ref displayCount, 0);
                    if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_DISP_GetGDIPrimaryDisplayId returned OK. We have {displayCount} physical GPUs");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_INSUFFICIENT_BUFFER)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The input buffer is not large enough to hold it's contents. NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_DISPLAY_ID)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {NVStatus}");
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {NVStatus}");
                    }

                    if (displayCount > 0)
                    {
                        // Now we try to get the information about the displayIDs
                        NV_GPU_DISPLAYIDS_V2[] displayIds = new NV_GPU_DISPLAYIDS_V2[displayCount];
                        NVStatus = NVImport.NvAPI_GPU_GetConnectedDisplayIds(physicalGpus[physicalGpuIndex], ref displayIds, ref displayCount, 0);
                        if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_GPU_GetConnectedDisplayIds returned OK. We have {displayCount} physical GPUs");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_INSUFFICIENT_BUFFER)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The input buffer is not large enough to hold it's contents. NvAPI_GPU_GetConnectedDisplayIds() returned error code {NVStatus}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_DISPLAY_ID)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_GPU_GetConnectedDisplayIds() returned error code {NVStatus}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_GPU_GetConnectedDisplayIds() returned error code {NVStatus}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_GPU_GetConnectedDisplayIds() returned error code {NVStatus}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_GPU_GetConnectedDisplayIds() returned error code {NVStatus}");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_DISP_GetGDIPrimaryDisplayId() returned error code {NVStatus}");
                        }
                        

                        // Time to get the HDR capabilities and settings for each display
                        bool isNvHdrEnabled = false;
                        Dictionary<UInt32, NV_HDR_CAPABILITIES_V2> allHdrCapabilities = new Dictionary<UInt32, NV_HDR_CAPABILITIES_V2>();
                        Dictionary<UInt32, NV_HDR_COLOR_DATA_V2> allHdrColorData = new Dictionary<UInt32, NV_HDR_COLOR_DATA_V2>();
                        for (int displayIndex = 0; displayIndex < displayCount; displayIndex++)
                        {
                            if (allDisplays)
                            {
                                // We want all physicallyconnected or connected displays
                                if (!(displayIds[displayIndex].isConnected || displayIds[displayIndex].isPhysicallyConnected))
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
                            
                            

                            // Now we get the HDR capabilities of the display
                            NV_HDR_CAPABILITIES_V2 hdrCapabilities = new NV_HDR_CAPABILITIES_V2();
                            NVStatus = NVImport.NvAPI_Disp_GetHdrCapabilities(displayIds[displayIndex].DisplayId, ref hdrCapabilities);
                            if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Disp_GetHdrCapabilities returned OK.");
                                if (hdrCapabilities.SupportFlags.HasFlag(NV_HDR_CAPABILITIES_V2_FLAGS.IsST2084EotfSupported))
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} supports HDR mode ST2084 EOTF");
                                }
                                else
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} DOES NOT support HDR mode ST2084 EOTF");
                                }
                                if (hdrCapabilities.SupportFlags.HasFlag(NV_HDR_CAPABILITIES_V2_FLAGS.IsDolbyVisionSupported))
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} supports DolbyVision HDR");
                                }
                                else
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} DOES NOT support DolbyVision HDR");
                                }
                                if (hdrCapabilities.SupportFlags.HasFlag(NV_HDR_CAPABILITIES_V2_FLAGS.IsEdrSupported))
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} supports EDR");
                                }
                                else
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} DOES NOT support EDR");
                                }
                                if (hdrCapabilities.SupportFlags.HasFlag(NV_HDR_CAPABILITIES_V2_FLAGS.IsTraditionalHdrGammaSupported))
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} supports Traditional HDR Gama");
                                }
                                else
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} DOES NOT support Traditional HDR Gama");
                                }

                                if (hdrCapabilities.SupportFlags.HasFlag(NV_HDR_CAPABILITIES_V2_FLAGS.IsTraditionalSdrGammaSupported))
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} supports Traditional SDR Gama");
                                }
                                else
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} DOES NOT supports Traditional SDR Gama");
                                }
                                if (hdrCapabilities.SupportFlags.HasFlag(NV_HDR_CAPABILITIES_V2_FLAGS.DriverExpandDefaultHdrParameters))
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} supports Driver Expanded Default HDR Parameters");
                                }
                                else
                                {
                                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Display {displayIds[displayIndex].DisplayId} DOES NOT support Driver Expanded Default HDR Parameters ");
                                }

                                allHdrCapabilities.Add(displayIds[displayIndex].DisplayId,hdrCapabilities);
                            }
                            else if (NVStatus == NVAPI_STATUS.NVAPI_INSUFFICIENT_BUFFER)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The input buffer is not large enough to hold it's contents. NvAPI_Disp_GetHdrCapabilities() returned error code {NVStatus}");
                            }
                            else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_DISPLAY_ID)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_Disp_GetHdrCapabilities() returned error code {NVStatus}");
                            }
                            else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_Disp_GetHdrCapabilities() returned error code {NVStatus}");
                            }
                            else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_Disp_GetHdrCapabilities() returned error code {NVStatus}");
                            }
                            else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_Disp_GetHdrCapabilities() returned error code {NVStatus}");
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting HDR color capabilities from your display! NvAPI_Disp_GetHdrCapabilities() returned error code {NVStatus}. It's most likely that your monitor {displayIds[displayIndex].DisplayId} doesn't support HDR.");
                            }

                            // Now we get the HDR colour settings of the display
                            NV_HDR_COLOR_DATA_V2 hdrColorData = new NV_HDR_COLOR_DATA_V2();
                            NVStatus = NVImport.NvAPI_Disp_HdrColorControl(displayIds[displayIndex].DisplayId, ref hdrColorData);
                            if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Disp_HdrColorControl returned OK. HDR mode is currently {hdrColorData.HdrMode.ToString("G")}.");
                                if (hdrColorData.HdrMode != NV_HDR_MODE.NV_HDR_MODE_OFF)
                                {
                                    isNvHdrEnabled = true;
                                }                                
                                allHdrColorData.Add(displayIds[displayIndex].DisplayId,hdrColorData);
                            }
                            else if (NVStatus == NVAPI_STATUS.NVAPI_INSUFFICIENT_BUFFER)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The input buffer is not large enough to hold it's contents. NvAPI_Disp_HdrColorControl() returned error code {NVStatus}");
                            }
                            else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_DISPLAY_ID)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_Disp_HdrColorControl() returned error code {NVStatus}");
                            }
                            else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_Disp_HdrColorControl() returned error code {NVStatus}");
                            }
                            else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_Disp_HdrColorControl() returned error code {NVStatus}");
                            }
                            else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                            {
                                SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_Disp_HdrColorControl() returned error code {NVStatus}.");                                
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting HDR color settings! NvAPI_Disp_HdrColorControl() returned error code {NVStatus}. It's most likely that your monitor {displayIds[displayIndex].DisplayId} doesn't support HDR.");
                            }
                        }

                        // Store the HDR information
                        myDisplayConfig.HdrConfig.IsNvHdrEnabled = isNvHdrEnabled;
                        myDisplayConfig.HdrConfig.HdrCapabilities = allHdrCapabilities;
                        myDisplayConfig.HdrConfig.HdrColorData = allHdrColorData;

                    }
                    
                }               

            }
            else
            {
                SharedLogger.logger.Error($"NVIDIALibrary/GetNVIDIADisplayConfig: ERROR - Tried to run GetNVIDIADisplayConfig but the NVIDIA ADL library isn't initialised!");
                throw new NVIDIALibraryException($"Tried to run GetNVIDIADisplayConfig but the NVIDIA ADL library isn't initialised!");
            }
            
            // Return the configuration
            return myDisplayConfig;
        }


        public string PrintActiveConfig()
        {
            string stringToReturn = "";

            // Get the size of the largest Active Paths and Modes arrays
            int pathCount = 0;
            int modeCount = 0;
            WIN32STATUS err = CCDImport.GetDisplayConfigBufferSizes(QDC.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount);
            if (err != WIN32STATUS.ERROR_SUCCESS)
            {
                SharedLogger.logger.Error($"NVIDIALibrary/PrintActiveConfig: ERROR - GetDisplayConfigBufferSizes returned WIN32STATUS {err} when trying to get the maximum path and mode sizes");
                throw new NVIDIALibraryException($"GetDisplayConfigBufferSizes returned WIN32STATUS {err} when trying to get the maximum path and mode sizes");
            }

            SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Getting the current Display Config path and mode arrays");
            var paths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            var modes = new DISPLAYCONFIG_MODE_INFO[modeCount];
            err = CCDImport.QueryDisplayConfig(QDC.QDC_ONLY_ACTIVE_PATHS, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero);
            if (err == WIN32STATUS.ERROR_INSUFFICIENT_BUFFER)
            {
                SharedLogger.logger.Warn($"NVIDIALibrary/PrintActiveConfig: The displays were modified between GetDisplayConfigBufferSizes and QueryDisplayConfig so we need to get the buffer sizes again.");
                SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Getting the size of the largest Active Paths and Modes arrays");
                // Screen changed in between GetDisplayConfigBufferSizes and QueryDisplayConfig, so we need to get buffer sizes again
                // as per https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-querydisplayconfig 
                err = CCDImport.GetDisplayConfigBufferSizes(QDC.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount);
                if (err != WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Error($"NVIDIALibrary/PrintActiveConfig: ERROR - GetDisplayConfigBufferSizes returned WIN32STATUS {err} when trying to get the maximum path and mode sizes again");
                    throw new NVIDIALibraryException($"GetDisplayConfigBufferSizes returned WIN32STATUS {err} when trying to get the maximum path and mode sizes again");
                }
                SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Getting the current Display Config path and mode arrays");
                paths = new DISPLAYCONFIG_PATH_INFO[pathCount];
                modes = new DISPLAYCONFIG_MODE_INFO[modeCount];
                err = CCDImport.QueryDisplayConfig(QDC.QDC_ONLY_ACTIVE_PATHS, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero);
                if (err == WIN32STATUS.ERROR_INSUFFICIENT_BUFFER)
                {
                    SharedLogger.logger.Error($"NVIDIALibrary/PrintActiveConfig: ERROR - The displays were still modified between GetDisplayConfigBufferSizes and QueryDisplayConfig, even though we tried twice. Something is wrong.");
                    throw new NVIDIALibraryException($"The displays were still modified between GetDisplayConfigBufferSizes and QueryDisplayConfig, even though we tried twice. Something is wrong.");
                }
                else if (err != WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Error($"NVIDIALibrary/PrintActiveConfig: ERROR - QueryDisplayConfig returned WIN32STATUS {err} when trying to query all available displays again");
                    throw new NVIDIALibraryException($"QueryDisplayConfig returned WIN32STATUS {err} when trying to query all available displays again.");
                }
            }
            else if (err != WIN32STATUS.ERROR_SUCCESS)
            {
                SharedLogger.logger.Error($"NVIDIALibrary/PrintActiveConfig: ERROR - QueryDisplayConfig returned WIN32STATUS {err} when trying to query all available displays");
                throw new NVIDIALibraryException($"QueryDisplayConfig returned WIN32STATUS {err} when trying to query all available displays.");
            }

            foreach (var path in paths)
            {
                stringToReturn += $"----++++==== Path ====++++----\n";

                // get display source name
                var sourceInfo = new DISPLAYCONFIG_SOURCE_DEVICE_NAME();
                sourceInfo.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME;
                sourceInfo.Header.Size = (uint)Marshal.SizeOf<DISPLAYCONFIG_SOURCE_DEVICE_NAME>();
                sourceInfo.Header.AdapterId = path.SourceInfo.AdapterId;
                sourceInfo.Header.Id = path.SourceInfo.Id;
                err = CCDImport.DisplayConfigGetDeviceInfo(ref sourceInfo);
                if (err == WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Found Display Source {sourceInfo.ViewGdiDeviceName} for source {path.SourceInfo.Id}.");
                    stringToReturn += $"****** Interrogating Display Source {path.SourceInfo.Id} *******\n";
                    stringToReturn += $"Found Display Source {sourceInfo.ViewGdiDeviceName}\n";
                    stringToReturn += $"\n";
                }
                else
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/PrintActiveConfig: WARNING - DisplayConfigGetDeviceInfo returned WIN32STATUS {err} when trying to get the source info for source adapter #{path.SourceInfo.AdapterId}");
                }


                // get display target name
                var targetInfo = new DISPLAYCONFIG_TARGET_DEVICE_NAME();
                targetInfo.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
                targetInfo.Header.Size = (uint)Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>();
                targetInfo.Header.AdapterId = path.TargetInfo.AdapterId;
                targetInfo.Header.Id = path.TargetInfo.Id;
                err = CCDImport.DisplayConfigGetDeviceInfo(ref targetInfo);
                if (err == WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Connector Instance: {targetInfo.ConnectorInstance} for source {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: EDID Manufacturer ID: {targetInfo.EdidManufactureId} for source {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: EDID Product Code ID: {targetInfo.EdidProductCodeId} for source {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Flags Friendly Name from EDID: {targetInfo.Flags.FriendlyNameFromEdid} for source {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Flags Friendly Name Forced: {targetInfo.Flags.FriendlyNameForced} for source {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Flags EDID ID is Valid: {targetInfo.Flags.EdidIdsValid} for source {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Monitor Device Path: {targetInfo.MonitorDevicePath} for source {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Monitor Friendly Device Name: {targetInfo.MonitorFriendlyDeviceName} for source {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Output Technology: {targetInfo.OutputTechnology} for source {path.TargetInfo.Id}.");

                    stringToReturn += $"****** Interrogating Display Target {targetInfo.MonitorFriendlyDeviceName} *******\n";
                    stringToReturn += $" Connector Instance: {targetInfo.ConnectorInstance}\n";
                    stringToReturn += $" EDID Manufacturer ID: {targetInfo.EdidManufactureId}\n";
                    stringToReturn += $" EDID Product Code ID: {targetInfo.EdidProductCodeId}\n";
                    stringToReturn += $" Flags Friendly Name from EDID: {targetInfo.Flags.FriendlyNameFromEdid}\n";
                    stringToReturn += $" Flags Friendly Name Forced: {targetInfo.Flags.FriendlyNameForced}\n";
                    stringToReturn += $" Flags EDID ID is Valid: {targetInfo.Flags.EdidIdsValid}\n";
                    stringToReturn += $" Monitor Device Path: {targetInfo.MonitorDevicePath}\n";
                    stringToReturn += $" Monitor Friendly Device Name: {targetInfo.MonitorFriendlyDeviceName}\n";
                    stringToReturn += $" Output Technology: {targetInfo.OutputTechnology}\n";
                    stringToReturn += $"\n";
                }
                else
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/PrintActiveConfig: WARNING - DisplayConfigGetDeviceInfo returned WIN32STATUS {err} when trying to get the target info for display #{path.TargetInfo.Id}");
                }


                // get display adapter name
                var adapterInfo = new DISPLAYCONFIG_ADAPTER_NAME();
                adapterInfo.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME;
                adapterInfo.Header.Size = (uint)Marshal.SizeOf<DISPLAYCONFIG_ADAPTER_NAME>();
                adapterInfo.Header.AdapterId = path.TargetInfo.AdapterId;
                adapterInfo.Header.Id = path.TargetInfo.Id;
                err = CCDImport.DisplayConfigGetDeviceInfo(ref adapterInfo);
                if (err == WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/PrintActiveConfig: Found Adapter Device Path {adapterInfo.AdapterDevicePath} for source {path.TargetInfo.AdapterId}.");
                    stringToReturn += $"****** Interrogating Display Adapter {adapterInfo.AdapterDevicePath} *******\n";
                    stringToReturn += $" Display Adapter {adapterInfo.AdapterDevicePath}\n";
                    stringToReturn += $"\n";
                }
                else
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetWindowsDisplayConfig: WARNING - DisplayConfigGetDeviceInfo returned WIN32STATUS {err} when trying to get the adapter device path for target #{path.TargetInfo.AdapterId}");
                }

                // get display target preferred mode
                var targetPreferredInfo = new DISPLAYCONFIG_TARGET_PREFERRED_MODE();
                targetPreferredInfo.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE;
                targetPreferredInfo.Header.Size = (uint)Marshal.SizeOf<DISPLAYCONFIG_TARGET_PREFERRED_MODE>();
                targetPreferredInfo.Header.AdapterId = path.TargetInfo.AdapterId;
                targetPreferredInfo.Header.Id = path.TargetInfo.Id;
                err = CCDImport.DisplayConfigGetDeviceInfo(ref targetPreferredInfo);
                if (err == WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Target Preferred Width {targetPreferredInfo.Width} for target {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Target Preferred Height {targetPreferredInfo.Height} for target {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Target Video Signal Info Active Size: ({targetPreferredInfo.TargetMode.TargetVideoSignalInfo.ActiveSize.Cx}x{targetPreferredInfo.TargetMode.TargetVideoSignalInfo.ActiveSize.Cy} for target {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Target Video Signal Info Total Size: ({targetPreferredInfo.TargetMode.TargetVideoSignalInfo.TotalSize.Cx}x{targetPreferredInfo.TargetMode.TargetVideoSignalInfo.TotalSize.Cy} for target {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Target Video Signal Info HSync Frequency: {targetPreferredInfo.TargetMode.TargetVideoSignalInfo.HSyncFreq} for target {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Target Video Signal Info VSync Frequency: {targetPreferredInfo.TargetMode.TargetVideoSignalInfo.VSyncFreq} for target {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Target Video Signal Info Pixel Rate: {targetPreferredInfo.TargetMode.TargetVideoSignalInfo.PixelRate} for target {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Target Video Signal Info Scan Line Ordering: {targetPreferredInfo.TargetMode.TargetVideoSignalInfo.ScanLineOrdering} for target {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Target Video Signal Info Video Standard: {targetPreferredInfo.TargetMode.TargetVideoSignalInfo.VideoStandard} for target {path.TargetInfo.Id}.");

                    stringToReturn += $"****** Interrogating Target Preferred Mode for Display {path.TargetInfo.Id} *******\n";
                    stringToReturn += $" Target Preferred Width {targetPreferredInfo.Width} for target {path.TargetInfo.Id}\n";
                    stringToReturn += $" Target Preferred Height {targetPreferredInfo.Height} for target {path.TargetInfo.Id}\n";
                    stringToReturn += $" Target Video Signal Info Active Size: ({targetPreferredInfo.TargetMode.TargetVideoSignalInfo.ActiveSize.Cx}x{targetPreferredInfo.TargetMode.TargetVideoSignalInfo.ActiveSize.Cy}\n";
                    stringToReturn += $" Target Video Signal Info Total Size: ({targetPreferredInfo.TargetMode.TargetVideoSignalInfo.TotalSize.Cx}x{targetPreferredInfo.TargetMode.TargetVideoSignalInfo.TotalSize.Cy}\n";
                    stringToReturn += $" Target Video Signal Info HSync Frequency: {targetPreferredInfo.TargetMode.TargetVideoSignalInfo.HSyncFreq}\n";
                    stringToReturn += $" Target Video Signal Info VSync Frequency: {targetPreferredInfo.TargetMode.TargetVideoSignalInfo.VSyncFreq}\n";
                    stringToReturn += $" Target Video Signal Info Pixel Rate: {targetPreferredInfo.TargetMode.TargetVideoSignalInfo.PixelRate}\n";
                    stringToReturn += $" Target Video Signal Info Scan Line Ordering: {targetPreferredInfo.TargetMode.TargetVideoSignalInfo.ScanLineOrdering}\n";
                    stringToReturn += $" Target Video Signal Info Video Standard: {targetPreferredInfo.TargetMode.TargetVideoSignalInfo.VideoStandard}\n";
                    stringToReturn += $"\n";
                }
                else
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetWindowsDisplayConfig: WARNING - DisplayConfigGetDeviceInfo returned WIN32STATUS {err} when trying to get the preferred target name for display #{path.TargetInfo.Id}");
                }

                // get display target base type
                var targetBaseTypeInfo = new DISPLAYCONFIG_TARGET_BASE_TYPE();
                targetBaseTypeInfo.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE;
                targetBaseTypeInfo.Header.Size = (uint)Marshal.SizeOf<DISPLAYCONFIG_TARGET_BASE_TYPE>();
                targetBaseTypeInfo.Header.AdapterId = path.TargetInfo.AdapterId;
                targetBaseTypeInfo.Header.Id = path.TargetInfo.Id;
                err = CCDImport.DisplayConfigGetDeviceInfo(ref targetBaseTypeInfo);
                if (err == WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Virtual Resolution is Disabled: {targetBaseTypeInfo.BaseOutputTechnology} for target {path.TargetInfo.Id}.");

                    stringToReturn += $"****** Interrogating Target Base Type for Display {path.TargetInfo.Id} *******\n";
                    stringToReturn += $" Base Output Technology: {targetBaseTypeInfo.BaseOutputTechnology}\n";
                    stringToReturn += $"\n";
                }
                else
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetWindowsDisplayConfig: WARNING - DisplayConfigGetDeviceInfo returned WIN32STATUS {err} when trying to get the target base type for display #{path.TargetInfo.Id}");
                }

                // get display support virtual resolution
                var supportVirtResInfo = new DISPLAYCONFIG_SUPPORT_VIRTUAL_RESOLUTION();
                supportVirtResInfo.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SUPPORT_VIRTUAL_RESOLUTION;
                supportVirtResInfo.Header.Size = (uint)Marshal.SizeOf<DISPLAYCONFIG_SUPPORT_VIRTUAL_RESOLUTION>();
                supportVirtResInfo.Header.AdapterId = path.TargetInfo.AdapterId;
                supportVirtResInfo.Header.Id = path.TargetInfo.Id;
                err = CCDImport.DisplayConfigGetDeviceInfo(ref supportVirtResInfo);
                if (err == WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Base Output Technology: {supportVirtResInfo.IsMonitorVirtualResolutionDisabled} for target {path.TargetInfo.Id}.");
                    stringToReturn += $"****** Interrogating Target Supporting virtual resolution for Display {path.TargetInfo.Id} *******\n";
                    stringToReturn += $" Virtual Resolution is Disabled: {supportVirtResInfo.IsMonitorVirtualResolutionDisabled}\n";
                    stringToReturn += $"\n";
                }
                else
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetWindowsDisplayConfig: WARNING - DisplayConfigGetDeviceInfo returned WIN32STATUS {err} when trying to find out the virtual resolution support for display #{path.TargetInfo.Id}");
                }

                //get advanced color info
                var colorInfo = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
                colorInfo.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
                colorInfo.Header.Size = (uint)Marshal.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>();
                colorInfo.Header.AdapterId = path.TargetInfo.AdapterId;
                colorInfo.Header.Id = path.TargetInfo.Id;
                err = CCDImport.DisplayConfigGetDeviceInfo(ref colorInfo);
                if (err == WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Advanced Color Supported: {colorInfo.AdvancedColorSupported} for target {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Advanced Color Enabled: {colorInfo.AdvancedColorEnabled} for target {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Advanced Color Force Disabled: {colorInfo.AdvancedColorForceDisabled} for target {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Bits per Color Channel: {colorInfo.BitsPerColorChannel} for target {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Color Encoding: {colorInfo.ColorEncoding} for target {path.TargetInfo.Id}.");
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found Wide Color Enforced: {colorInfo.WideColorEnforced} for target {path.TargetInfo.Id}.");

                    stringToReturn += $"****** Interrogating Advanced Color Info for Display {path.TargetInfo.Id} *******\n";
                    stringToReturn += $" Advanced Color Supported: {colorInfo.AdvancedColorSupported}\n";
                    stringToReturn += $" Advanced Color Enabled: {colorInfo.AdvancedColorEnabled}\n";
                    stringToReturn += $" Advanced Color Force Disabled: {colorInfo.AdvancedColorForceDisabled}\n";
                    stringToReturn += $" Bits per Color Channel: {colorInfo.BitsPerColorChannel}\n";
                    stringToReturn += $" Color Encoding: {colorInfo.ColorEncoding}\n";
                    stringToReturn += $" Wide Color Enforced: {colorInfo.WideColorEnforced}\n";
                    stringToReturn += $"\n";
                }
                else
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetWindowsDisplayConfig: WARNING - DisplayConfigGetDeviceInfo returned WIN32STATUS {err} when trying to find out the virtual resolution support for display #{path.TargetInfo.Id}");
                }

                // get SDR white levels
                var whiteLevelInfo = new DISPLAYCONFIG_SDR_WHITE_LEVEL();
                whiteLevelInfo.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL;
                whiteLevelInfo.Header.Size = (uint)Marshal.SizeOf<DISPLAYCONFIG_SDR_WHITE_LEVEL>();
                whiteLevelInfo.Header.AdapterId = path.TargetInfo.AdapterId;
                whiteLevelInfo.Header.Id = path.TargetInfo.Id;
                err = CCDImport.DisplayConfigGetDeviceInfo(ref whiteLevelInfo);
                if (err == WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetWindowsDisplayConfig: Found SDR White Level: {whiteLevelInfo.SDRWhiteLevel} for target {path.TargetInfo.Id}.");

                    stringToReturn += $"****** Interrogating SDR Whilte Level for Display {path.TargetInfo.Id} *******\n";
                    stringToReturn += $" SDR White Level: {whiteLevelInfo.SDRWhiteLevel}\n";
                    stringToReturn += $"\n";
                }
                else
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetWindowsDisplayConfig: WARNING - DisplayConfigGetDeviceInfo returned WIN32STATUS {err} when trying to find out the SDL white level for display #{path.TargetInfo.Id}");
                }
            }
            return stringToReturn;
        }

        public bool SetActiveConfig(NVIDIA_DISPLAY_CONFIG displayConfig)
        {

            if (_initialised)
            {

                NVAPI_STATUS NVStatus = NVAPI_STATUS.NVAPI_ERROR;
                // We want to get the current config
                NVIDIA_DISPLAY_CONFIG currentDisplayConfig = GetNVIDIADisplayConfig();

                // We want to check the NVIDIA Surround (Mosaic) config is valid
                SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Testing whether the display configuration is valid");
                // 
                if (displayConfig.MosaicConfig.IsMosaicEnabled)
                {

                    // We need to change to a Mosaic profile, so we need to apply the new Mosaic Topology
                    NV_MOSAIC_SETDISPLAYTOPO_FLAGS setTopoFlags = NV_MOSAIC_SETDISPLAYTOPO_FLAGS.NONE;                    

                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Yay! The display settings provided are valid to apply! Attempting to apply them now.");
                    // If we get here then the display is valid, so now we actually apply the new Mosaic Topology
                    NVStatus = NVImport.NvAPI_Mosaic_SetDisplayGrids(displayConfig.MosaicConfig.MosaicGridTopos, displayConfig.MosaicConfig.MosaicGridCount, setTopoFlags);
                    if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_NO_ACTIVE_SLI_TOPOLOGY)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: No matching GPU topologies could be found. NvAPI_Mosaic_SetDisplayGrids() returned error code {NVStatus}");
                        return false;
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_TOPO_NOT_POSSIBLE)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The topology passed in is not currently possible. NvAPI_Mosaic_SetDisplayGrids() returned error code {NVStatus}");
                        return false;
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_ARGUMENT)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_SetDisplayGrids() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_SetDisplayGrids() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_SetDisplayGrids() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_INCOMPATIBLE_STRUCT_VERSION)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: The version of the structure passed in is not compatible with this entrypoint. NvAPI_Mosaic_SetDisplayGrids() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_MODE_CHANGE_FAILED)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: There was an error changing the display mode. NvAPI_Mosaic_SetDisplayGrids() returned error code {NVStatus}");
                        return false;
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                    {
                        SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_Mosaic_SetDisplayGrids() returned error code {NVStatus}");
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }

                }                
                else if (!displayConfig.MosaicConfig.IsMosaicEnabled && currentDisplayConfig.MosaicConfig.IsMosaicEnabled)
                {
                    // We are on a Mosaic profile now, and we need to change to a non-Mosaic profile
                    // We need to disable the Mosaic Topology

                    // Turn off Mosaic
                    uint enable = 0;
                    NVStatus = NVImport.NvAPI_Mosaic_EnableCurrentTopo(enable);
                    if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_NOT_SUPPORTED)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_TOPO_NOT_POSSIBLE)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The topology passed in is not currently possible. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                        return false;
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_ARGUMENT)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_INCOMPATIBLE_STRUCT_VERSION)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The version of the structure passed in is not compatible with this entrypoint. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_MODE_CHANGE_FAILED)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: There was an error disabling the display mode. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                        return false;
                    }
                    else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                    {
                        SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }
                    else
                    {
                        SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                    }
                }
                else if (!displayConfig.MosaicConfig.IsMosaicEnabled && !currentDisplayConfig.MosaicConfig.IsMosaicEnabled)
                {
                    // We are on a non-Mosaic profile now, and we are changing to a non-Mosaic profile
                    // so there is nothing to do as far as NVIDIA is concerned!
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: We are on a non-Mosaic profile now, and we are changing to a non-Mosaic profile so there is nothing to do as far as NVIDIA is concerned!");
                }

                // Now, we have the current HDR settings, and the existing HDR settings, so we go through and we attempt to set each display color settings
                foreach (var wantedHdrColorData in displayConfig.HdrConfig.HdrColorData)
                {
                    // If we have HDR settings for the display, then attempt to set them
                    if (currentDisplayConfig.HdrConfig.HdrColorData.ContainsKey(wantedHdrColorData.Key))
                    {
                        // Now we set the HDR colour settings of the display
                        NV_HDR_COLOR_DATA_V2 hdrColorData = wantedHdrColorData.Value;
                        NVStatus = NVImport.NvAPI_Disp_HdrColorControl(wantedHdrColorData.Key, ref hdrColorData);
                        if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: NvAPI_Disp_HdrColorControl returned OK. We just successfully set the HDR mode to {hdrColorData.HdrMode.ToString("G")}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_INSUFFICIENT_BUFFER)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The input buffer is not large enough to hold it's contents. NvAPI_Disp_HdrColorControl() returned error code {NVStatus}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_DISPLAY_ID)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The input monitor is either not connected or is not a DP or HDMI panel. NvAPI_Disp_HdrColorControl() returned error code {NVStatus}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_Disp_HdrColorControl() returned error code {NVStatus}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_Disp_HdrColorControl() returned error code {NVStatus}");
                        }
                        else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                        {
                            SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_Disp_HdrColorControl() returned error code {NVStatus}");
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Disp_HdrColorControl() returned error code {NVStatus}. It's most likely that your monitor {wantedHdrColorData.Key} doesn't support HDR.");
                        }
                    }                    
                }

            }
            else
            {
                SharedLogger.logger.Error($"NVIDIALibrary/SetActiveConfig: ERROR - Tried to run SetActiveConfig but the NVIDIA NVAPI library isn't initialised!");
                throw new NVIDIALibraryException($"Tried to run SetActiveConfig but the NVIDIA NVAPI library isn't initialised!");
            }


            
            return true;
        }

        public bool IsActiveConfig(NVIDIA_DISPLAY_CONFIG displayConfig)
        {
            // Get the current windows display configs to compare to the one we loaded
            bool allDisplays = false;
            NVIDIA_DISPLAY_CONFIG currentDisplayConfig = GetNVIDIADisplayConfig(allDisplays);

            // Check whether the display config is in use now
            SharedLogger.logger.Trace($"NVIDIALibrary/IsActiveConfig: Checking whether the display configuration is already being used.");
            if (displayConfig.Equals(currentDisplayConfig))
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

        public bool IsPossibleConfig(NVIDIA_DISPLAY_CONFIG displayConfig)
        {
            // We want to check the NVIDIA Surround (Mosaic) config is valid
            SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Testing whether the display configuration is valid");
            // 
            if (displayConfig.MosaicConfig.IsMosaicEnabled)
            {

                /*// Figure out how many Mosaic Grid topoligies there are                    
                uint mosaicGridCount = 0;
                NVAPI_STATUS NVStatus = NVImport.NvAPI_Mosaic_EnumDisplayGrids(ref mosaicGridCount);
                if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");
                }

                // Get Current Mosaic Grid settings using the Grid topologies fnumbers we got before
                //NV_MOSAIC_GRID_TOPO_V2[] mosaicGridTopos = new NV_MOSAIC_GRID_TOPO_V2[mosaicGridCount];
                NV_MOSAIC_GRID_TOPO_V1[] mosaicGridTopos = new NV_MOSAIC_GRID_TOPO_V1[mosaicGridCount];
                NVStatus = NVImport.NvAPI_Mosaic_EnumDisplayGrids(ref mosaicGridTopos, ref mosaicGridCount);
                if (NVStatus == NVAPI_STATUS.NVAPI_OK)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: NvAPI_Mosaic_GetCurrentTopo returned OK.");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_NOT_SUPPORTED)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_ARGUMENT)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetNVIDIADisplayConfig: A miscellaneous error occurred. NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetNVIDIADisplayConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_GetCurrentTopo() returned error code {NVStatus}");
                }
                */

                NV_MOSAIC_SETDISPLAYTOPO_FLAGS setTopoFlags = NV_MOSAIC_SETDISPLAYTOPO_FLAGS.NONE;
                bool topoValid = false;
                NV_MOSAIC_DISPLAY_TOPO_STATUS_V1[] topoStatuses = new NV_MOSAIC_DISPLAY_TOPO_STATUS_V1[displayConfig.MosaicConfig.MosaicGridCount];
                NVAPI_STATUS NVStatus = NVImport.NvAPI_Mosaic_ValidateDisplayGrids(setTopoFlags, ref displayConfig.MosaicConfig.MosaicGridTopos, ref topoStatuses, displayConfig.MosaicConfig.MosaicGridCount);
                //NV_MOSAIC_DISPLAY_TOPO_STATUS_V1[] topoStatuses = new NV_MOSAIC_DISPLAY_TOPO_STATUS_V1[mosaicGridCount];
                //NVStatus = NVImport.NvAPI_Mosaic_ValidateDisplayGrids(setTopoFlags, ref mosaicGridTopos, ref topoStatuses, mosaicGridCount);
                if (NVStatus == NVAPI_STATUS.NVAPI_OK)
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
                else if (NVStatus == NVAPI_STATUS.NVAPI_NOT_SUPPORTED)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: Mosaic is not supported with the existing hardware. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_NO_ACTIVE_SLI_TOPOLOGY)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: No matching GPU topologies could be found. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_TOPO_NOT_POSSIBLE)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The topology passed in is not currently possible. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_INVALID_ARGUMENT)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: One or more argumentss passed in are invalid. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_API_NOT_INITIALIZED)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The NvAPI API needs to be initialized first. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_NO_IMPLEMENTATION)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: This entry point not available in this NVIDIA Driver. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_INCOMPATIBLE_STRUCT_VERSION)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: The version of the structure passed in is not compatible with this entrypoint. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_MODE_CHANGE_FAILED)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: There was an error changing the display mode. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {NVStatus}");
                }
                else if (NVStatus == NVAPI_STATUS.NVAPI_ERROR)
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/SetActiveConfig: A miscellaneous error occurred. NvAPI_Mosaic_ValidateDisplayGrids() returned error code {NVStatus}");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/SetActiveConfig: Some non standard error occurred while getting Mosaic Topology! NvAPI_Mosaic_ValidateDisplayGrids() returned error code {NVStatus}");
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
                }
            }
            else
            {
                // Its not a Mosaic topology, so we just let it pass, as it's windows settings that matter.
                return true;
            }
        }

        public List<string> GetCurrentDisplayIdentifiers()
        {
            SharedLogger.logger.Error($"NVIDIALibrary/GetCurrentDisplayIdentifiers: Getting the current display identifiers for the displays in use now");
            return GetSomeDisplayIdentifiers(QDC.QDC_ONLY_ACTIVE_PATHS);
        }

        public List<string> GetAllConnectedDisplayIdentifiers()
        {
            SharedLogger.logger.Error($"NVIDIALibrary/GetAllConnectedDisplayIdentifiers: Getting all the display identifiers that can possibly be used");
            return GetSomeDisplayIdentifiers(QDC.QDC_ALL_PATHS);
        }

        private List<string> GetSomeDisplayIdentifiers(QDC selector = QDC.QDC_ONLY_ACTIVE_PATHS)
        {
            SharedLogger.logger.Debug($"NVIDIALibrary/GetCurrentDisplayIdentifiers: Generating the unique Display Identifiers for the currently active configuration");

            List<string> displayIdentifiers = new List<string>();

            SharedLogger.logger.Trace($"NVIDIALibrary/GetCurrentDisplayIdentifiers: Testing whether the display configuration is valid (allowing tweaks).");
            // Get the size of the largest Active Paths and Modes arrays
            int pathCount = 0;
            int modeCount = 0;
            WIN32STATUS err = CCDImport.GetDisplayConfigBufferSizes(QDC.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount);
            if (err != WIN32STATUS.ERROR_SUCCESS)
            {
                SharedLogger.logger.Error($"NVIDIALibrary/PrintActiveConfig: ERROR - GetDisplayConfigBufferSizes returned WIN32STATUS {err} when trying to get the maximum path and mode sizes");
                throw new NVIDIALibraryException($"GetDisplayConfigBufferSizes returned WIN32STATUS {err} when trying to get the maximum path and mode sizes");
            }

            SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Getting the current Display Config path and mode arrays");
            var paths = new DISPLAYCONFIG_PATH_INFO[pathCount];
            var modes = new DISPLAYCONFIG_MODE_INFO[modeCount];
            err = CCDImport.QueryDisplayConfig(QDC.QDC_ONLY_ACTIVE_PATHS, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero);
            if (err == WIN32STATUS.ERROR_INSUFFICIENT_BUFFER)
            {
                SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: The displays were modified between GetDisplayConfigBufferSizes and QueryDisplayConfig so we need to get the buffer sizes again.");
                SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Getting the size of the largest Active Paths and Modes arrays");
                // Screen changed in between GetDisplayConfigBufferSizes and QueryDisplayConfig, so we need to get buffer sizes again
                // as per https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-querydisplayconfig 
                err = CCDImport.GetDisplayConfigBufferSizes(QDC.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount);
                if (err != WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Error($"NVIDIALibrary/GetSomeDisplayIdentifiers: ERROR - GetDisplayConfigBufferSizes returned WIN32STATUS {err} when trying to get the maximum path and mode sizes again");
                    throw new NVIDIALibraryException($"GetDisplayConfigBufferSizes returned WIN32STATUS {err} when trying to get the maximum path and mode sizes again");
                }
                SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Getting the current Display Config path and mode arrays");
                paths = new DISPLAYCONFIG_PATH_INFO[pathCount];
                modes = new DISPLAYCONFIG_MODE_INFO[modeCount];
                err = CCDImport.QueryDisplayConfig(QDC.QDC_ONLY_ACTIVE_PATHS, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero);
                if (err == WIN32STATUS.ERROR_INSUFFICIENT_BUFFER)
                {
                    SharedLogger.logger.Error($"NVIDIALibrary/GetSomeDisplayIdentifiers: ERROR - The displays were still modified between GetDisplayConfigBufferSizes and QueryDisplayConfig, even though we tried twice. Something is wrong.");
                    throw new NVIDIALibraryException($"The displays were still modified between GetDisplayConfigBufferSizes and QueryDisplayConfig, even though we tried twice. Something is wrong.");
                }
                else if (err != WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Error($"NVIDIALibrary/GetSomeDisplayIdentifiers: ERROR - QueryDisplayConfig returned WIN32STATUS {err} when trying to query all available displays again");
                    throw new NVIDIALibraryException($"QueryDisplayConfig returned WIN32STATUS {err} when trying to query all available displays again.");
                }
            }
            else if (err != WIN32STATUS.ERROR_SUCCESS)
            {
                SharedLogger.logger.Error($"NVIDIALibrary/GetSomeDisplayIdentifiers: ERROR - QueryDisplayConfig returned WIN32STATUS {err} when trying to query all available displays");
                throw new NVIDIALibraryException($"QueryDisplayConfig returned WIN32STATUS {err} when trying to query all available displays.");
            }

            foreach (var path in paths)
            {
                if (path.TargetInfo.TargetAvailable == false)
                {
                    // We want to skip this one cause it's not valid
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Skipping path due to TargetAvailable not existing in display #{path.TargetInfo.Id}");
                    continue;
                }

                // get display source name
                var sourceInfo = new DISPLAYCONFIG_SOURCE_DEVICE_NAME();
                sourceInfo.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME;
                sourceInfo.Header.Size = (uint)Marshal.SizeOf<DISPLAYCONFIG_SOURCE_DEVICE_NAME>();
                sourceInfo.Header.AdapterId = path.SourceInfo.AdapterId;
                sourceInfo.Header.Id = path.SourceInfo.Id;
                err = CCDImport.DisplayConfigGetDeviceInfo(ref sourceInfo);
                if (err == WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Successfully got the source info from {path.SourceInfo.Id}.");
                }
                else
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: WARNING - DisplayConfigGetDeviceInfo returned WIN32STATUS {err} when trying to get the target info for display #{path.SourceInfo.Id}");
                }

                // get display target name
                var targetInfo = new DISPLAYCONFIG_TARGET_DEVICE_NAME();
                targetInfo.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
                targetInfo.Header.Size = (uint)Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>();
                targetInfo.Header.AdapterId = path.TargetInfo.AdapterId;
                targetInfo.Header.Id = path.TargetInfo.Id;
                err = CCDImport.DisplayConfigGetDeviceInfo(ref targetInfo);
                if (err == WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Successfully got the target info from {path.TargetInfo.Id}.");
                }
                else
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: WARNING - DisplayConfigGetDeviceInfo returned WIN32STATUS {err} when trying to get the target info for display #{path.TargetInfo.Id}");
                }

                // get display adapter name
                var adapterInfo = new DISPLAYCONFIG_ADAPTER_NAME();
                adapterInfo.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME;
                adapterInfo.Header.Size = (uint)Marshal.SizeOf<DISPLAYCONFIG_ADAPTER_NAME>();
                adapterInfo.Header.AdapterId = path.TargetInfo.AdapterId;
                adapterInfo.Header.Id = path.TargetInfo.Id;
                err = CCDImport.DisplayConfigGetDeviceInfo(ref adapterInfo);
                if (err == WIN32STATUS.ERROR_SUCCESS)
                {
                    SharedLogger.logger.Trace($"NVIDIALibrary/GetSomeDisplayIdentifiers: Successfully got the display name info from {path.TargetInfo.Id}.");
                }
                else
                {
                    SharedLogger.logger.Warn($"NVIDIALibrary/GetSomeDisplayIdentifiers: WARNING - DisplayConfigGetDeviceInfo returned WIN32STATUS {err} when trying to get the target info for display #{path.TargetInfo.Id}");
                }

                // Create an array of all the important display info we need to record
                List<string> displayInfo = new List<string>();
                displayInfo.Add("WINAPI");
                try
                {
                    displayInfo.Add(adapterInfo.AdapterDevicePath.ToString());
                }
                catch (Exception ex)
                {
                    SharedLogger.logger.Warn(ex, $"NVIDIALibrary/GetSomeDisplayIdentifiers: Exception getting Windows Display Adapter Device Path from video card. Substituting with a # instead");
                    displayInfo.Add("#");
                }
                try
                {
                    displayInfo.Add(targetInfo.OutputTechnology.ToString());
                }
                catch (Exception ex)
                {
                    SharedLogger.logger.Warn(ex, $"NVIDIALibrary/GetSomeDisplayIdentifiers: Exception getting Windows Display Connector Instance from video card. Substituting with a # instead");
                    displayInfo.Add("#");
                }
                try
                {
                    displayInfo.Add(targetInfo.EdidManufactureId.ToString());
                }
                catch (Exception ex)
                {
                    SharedLogger.logger.Warn(ex, $"NVIDIALibrary/GetSomeDisplayIdentifiers: Exception getting Windows Display EDID Manufacturer Code from video card. Substituting with a # instead");
                    displayInfo.Add("#");
                }
                try
                {
                    displayInfo.Add(targetInfo.EdidProductCodeId.ToString());
                }
                catch (Exception ex)
                {
                    SharedLogger.logger.Warn(ex, $"NVIDIALibrary/GetSomeDisplayIdentifiers: Exception getting Windows Display EDID Product Code from video card. Substituting with a # instead");
                    displayInfo.Add("#");
                }
                try
                {
                    displayInfo.Add(targetInfo.MonitorFriendlyDeviceName.ToString());
                }
                catch (Exception ex)
                {
                    SharedLogger.logger.Warn(ex, $"NVIDIALibrary/GetSomeDisplayIdentifiers: Exception getting Windows Display Target Friendly name from video card. Substituting with a # instead");
                    displayInfo.Add("#");
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

            // Sort the display identifiers
            displayIdentifiers.Sort();

            return displayIdentifiers;
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