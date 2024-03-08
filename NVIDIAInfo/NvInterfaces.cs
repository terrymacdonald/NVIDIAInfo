using NvAPIWrapper.Native.Display;
using NvAPIWrapper.Native.Display.Structures;
using NvAPIWrapper.Native.General;
using NvAPIWrapper.Native.Interfaces.Display;
using NvAPIWrapper.Native.Interfaces.Mosaic;
using NvAPIWrapper.Native.Mosaic;
using NvAPIWrapper.Native.Mosaic.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayMagicianShared.NVIDIA
{
    /// <summary>
    ///     Marker interface for all types that should be allocated before passing to the managed code
    /// </summary>
    public interface IAllocatable : IInitializable, IDisposable
    {
        void Allocate();
    }

    /// <summary>
    ///     Interface for all pointer based handles
    /// </summary>
    public interface IHandle
    {
        /// <summary>
        ///     Returns true if the handle is null and not pointing to a valid location in the memory
        /// </summary>
        bool IsNull { get; }

        /// <summary>
        ///     Gets the address of the handle in the memory
        /// </summary>
        IntPtr MemoryAddress { get; }
    }

    /// <summary>
    ///     Marker interface for all types that should be filled with information before passing to un-managed code
    /// </summary>
    public interface IInitializable
    {
    }

    /// <summary>
    ///     Contains data corresponding to color information
    /// </summary>
    public interface IColorData
    {
        /// <summary>
        ///     Gets the color data color depth
        /// </summary>
        ColorDataDepth? ColorDepth { get; }

        /// <summary>
        ///     Gets the color data dynamic range
        /// </summary>
        ColorDataDynamicRange? DynamicRange { get; }

        /// <summary>
        ///     Gets the color data color format
        /// </summary>
        ColorDataFormat ColorFormat { get; }

        /// <summary>
        ///     Gets the color data color space
        /// </summary>
        ColorDataColorimetry Colorimetry { get; }

        /// <summary>
        ///     Gets the color data selection policy
        /// </summary>
        ColorDataSelectionPolicy? SelectionPolicy { get; }

        /// <summary>
        ///     Gets the color data desktop color depth
        /// </summary>
        ColorDataDesktopDepth? DesktopColorDepth { get; }
    }

    /// <summary>
    ///     Holds information regarding a display color space configurations
    /// </summary>
    public interface IDisplayColorData
    {
        /// <summary>
        ///     Gets the first primary color space coordinate (e.g. Red for RGB) [(0.0, 0.0)-(1.0, 1.0)]
        /// </summary>
        ColorDataColorCoordinate FirstColorCoordinate { get; }

        /// <summary>
        ///     Gets the second primary color space coordinate (e.g. Green for RGB) [(0.0, 0.0)-(1.0, 1.0)]
        /// </summary>
        ColorDataColorCoordinate SecondColorCoordinate { get; }

        /// <summary>
        ///     Gets the third primary color space coordinate (e.g. Blue for RGB) [(0.0, 0.0)-(1.0, 1.0)]
        /// </summary>
        ColorDataColorCoordinate ThirdColorCoordinate { get; }

        /// <summary>
        ///     Gets the white color space coordinate [(0.0, 0.0)-(1.0, 1.0)]
        /// </summary>
        ColorDataColorCoordinate WhiteColorCoordinate { get; }
    }

    /// <summary>
    ///     Holds the Digital Vibrance Control information regarding the saturation level.
    /// </summary>
    public interface IDisplayDVCInfo
    {
        /// <summary>
        ///     Gets the current saturation level
        /// </summary>
        int CurrentLevel { get; }

        /// <summary>
        ///     Gets the default saturation level
        /// </summary>
        int DefaultLevel { get; }

        /// <summary>
        ///     Gets the maximum valid saturation level
        /// </summary>
        int MaximumLevel { get; }

        /// <summary>
        ///     Gets the minimum valid saturation level
        /// </summary>
        int MinimumLevel { get; }
    }

    /// <summary>
    ///     Contains information about the HDMI capabilities of the GPU, output and the display device attached
    /// </summary>
    public interface IHDMISupportInfo
    {
        /// <summary>
        ///     Gets the display's EDID 861 Extension Revision
        /// </summary>
        uint EDID861ExtensionRevision { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the GPU is capable of HDMI output
        /// </summary>
        bool IsGPUCapableOfHDMIOutput { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the display is connected via HDMI
        /// </summary>
        bool IsHDMIMonitor { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of Adobe RGB if such data is available;
        ///     otherwise null
        /// </summary>
        bool? IsMonitorCapableOfAdobeRGB { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of Adobe YCC601 if such data is available;
        ///     otherwise null
        /// </summary>
        bool? IsMonitorCapableOfAdobeYCC601 { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of basic audio
        /// </summary>
        bool IsMonitorCapableOfBasicAudio { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of sYCC601 if such data is available;
        ///     otherwise null
        /// </summary>
        bool? IsMonitorCapableOfsYCC601 { get; }


        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of underscan
        /// </summary>
        bool IsMonitorCapableOfUnderscan { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of xvYCC601
        /// </summary>
        // ReSharper disable once IdentifierTypo
        bool IsMonitorCapableOfxvYCC601 { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of xvYCC709
        /// </summary>
        // ReSharper disable once IdentifierTypo
        bool IsMonitorCapableOfxvYCC709 { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of YCbCr422
        /// </summary>
        bool IsMonitorCapableOfYCbCr422 { get; }

        /// <summary>
        ///     Gets a boolean value indicating that the connected display is capable of YCbCr444
        /// </summary>
        bool IsMonitorCapableOfYCbCr444 { get; }
    }

    /// <summary>
    ///     Contains information regarding HDR color data
    /// </summary>
    public interface IHDRColorData
    {
        /// <summary>
        ///     Gets the HDR color depth if available; otherwise null
        ///     For Dolby Vision only, should and will be ignored if HDR is on
        /// </summary>
        ColorDataDepth? ColorDepth { get; }

        /// <summary>
        ///     Gets the HDR color format if available; otherwise null
        /// </summary>
        ColorDataFormat? ColorFormat { get; }

        /// <summary>
        ///     Gets the HDR dynamic range if available; otherwise null
        /// </summary>
        ColorDataDynamicRange? DynamicRange { get; }

        /// <summary>
        ///     Gets the HDR mode
        /// </summary>
        ColorDataHDRMode HDRMode { get; }

        /// <summary>
        ///     Gets the color space coordinates
        /// </summary>
        MasteringDisplayColorData MasteringDisplayData { get; }
    }

    /// <summary>
    ///     Interface for all PathInfo structures
    /// </summary>
    public interface IPathInfo : IDisposable
    {
        /// <summary>
        ///     Identifies sourceId used by Windows CCD. This can be optionally set.
        /// </summary>
        uint SourceId { get; }

        /// <summary>
        ///     Contains information about the source mode
        /// </summary>
        SourceModeInfo SourceModeInfo { get; }

        /// <summary>
        ///     Contains information about path targets
        /// </summary>
        IEnumerable<IPathTargetInfo> TargetsInfo { get; }
    }

    /// <summary>
    ///     Interface for all PathTargetInfo structures
    /// </summary>
    public interface IPathTargetInfo
    {
        /// <summary>
        ///     Contains extra information. NULL for Non-NVIDIA Display.
        /// </summary>
        PathAdvancedTargetInfo? Details { get; }

        /// <summary>
        ///     Display identification
        /// </summary>
        uint DisplayId { get; }
    }

    /// <summary>
    /// Contains information regarding the scan-out intensity data
    /// </summary>
    public interface IScanOutIntensity
    {

        /// <summary>
        ///     Gets the array of floating values building an intensity RGB texture
        /// </summary>
        float[] BlendingTexture { get; }

        /// <summary>
        ///     Gets the height of the input texture
        /// </summary>
        uint Height { get; }


        /// <summary>
        ///     Gets the width of the input texture
        /// </summary>
        uint Width { get; }
    }

    /// <summary>
    ///     Interface for all ChipsetInfo structures
    /// </summary>
    public interface IChipsetInfo
    {
        /// <summary>
        ///     Chipset device name
        /// </summary>
        string ChipsetName { get; }

        /// <summary>
        ///     Chipset device identification
        /// </summary>
        int DeviceId { get; }

        /// <summary>
        ///     Chipset information flags - obsolete
        /// </summary>
        ChipsetInfoFlag Flags { get; }

        /// <summary>
        ///     Chipset vendor identification
        /// </summary>
        int VendorId { get; }

        /// <summary>
        ///     Chipset vendor name
        /// </summary>
        string VendorName { get; }
    }

    /// <summary>
    ///     Interface for all DisplaySettings structures
    /// </summary>
    public interface IDisplaySettings
    {
        /// <summary>
        ///     Bits per pixel
        /// </summary>
        int BitsPerPixel { get; }

        /// <summary>
        ///     Display frequency
        /// </summary>
        int Frequency { get; }

        /// <summary>
        ///     Display frequency in x1k
        /// </summary>
        uint FrequencyInMillihertz { get; }

        /// <summary>
        ///     Per-display height
        /// </summary>
        int Height { get; }

        /// <summary>
        ///     Per-display width
        /// </summary>
        int Width { get; }
    }

    /// <summary>
    ///     Interface for all GridTopology structures
    /// </summary>
    public interface IGridTopology
    {
        /// <summary>
        ///     Enable SLI acceleration on the primary display while in single-wide mode (For Immersive Gaming only). Will not be
        ///     persisted. Value undefined on get.
        /// </summary>
        bool AcceleratePrimaryDisplay { get; }

        /// <summary>
        ///     When enabling and doing the mode-set, do we switch to the bezel-corrected resolution
        /// </summary>
        bool ApplyWithBezelCorrectedResolution { get; }

        /// <summary>
        ///     Enable as Base Mosaic (Panoramic) instead of Mosaic SLI (for NVS and Quadro-boards only)
        /// </summary>
        bool BaseMosaicPanoramic { get; }

        /// <summary>
        ///     Number of columns
        /// </summary>
        int Columns { get; }

        /// <summary>
        ///     Topology displays; Displays are done as [(row * columns) + column]
        /// </summary>
        IEnumerable<IGridTopologyDisplay> Displays { get; }

        /// <summary>
        ///     Display settings
        /// </summary>
        DisplaySettingsV1 DisplaySettings { get; }

        /// <summary>
        ///     If necessary, reloading the driver is permitted (for Vista and above only). Will not be persisted. Value undefined
        ///     on get.
        /// </summary>
        bool DriverReloadAllowed { get; }

        /// <summary>
        ///     Enable as immersive gaming instead of Mosaic SLI (for Quadro-boards only)
        /// </summary>
        bool ImmersiveGaming { get; }

        /// <summary>
        ///     Number of rows
        /// </summary>
        int Rows { get; }
    }

    /// <summary>
    ///     Interface for all GridTopologyDisplay structures
    /// </summary>
    public interface IGridTopologyDisplay
    {
        /// <summary>
        ///     Gets the clone group identification; Reserved, must be 0
        /// </summary>
        uint CloneGroup { get; }

        /// <summary>
        ///     Gets the display identification
        /// </summary>
        uint DisplayId { get; }

        /// <summary>
        ///     Gets the horizontal overlap (+overlap, -gap)
        /// </summary>
        int OverlapX { get; }

        /// <summary>
        ///     Gets the vertical overlap (+overlap, -gap)
        /// </summary>
        int OverlapY { get; }


        /// <summary>
        ///     Gets the type of display pixel shift
        /// </summary>
        PixelShiftType PixelShiftType { get; }

        /// <summary>
        ///     Gets the rotation of display
        /// </summary>
        Rotate Rotation { get; }
    }

    /// <summary>
    ///     Interface for all SupportedTopologiesInfo structures
    /// </summary>
    public interface ISupportedTopologiesInfo
    {
        /// <summary>
        ///     List of per display settings possible
        /// </summary>
        IEnumerable<IDisplaySettings> DisplaySettings { get; }

        /// <summary>
        ///     List of supported topologies with only brief details
        /// </summary>
        IEnumerable<TopologyBrief> TopologyBriefs { get; }
    }
}
