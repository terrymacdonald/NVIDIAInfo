using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DisplayMagicianShared.NVIDIA
{
    /// <summary>
    ///     This structure defines a group of topologies that work together to create one overall layout.  All of the supported
    ///     topologies are represented with this structure.
    ///     For example, a 'Passive Stereo' topology would be represented with this structure, and would have separate topology
    ///     details for the left and right eyes. The count would be 2. A 'Basic' topology is also represented by this
    ///     structure, with a count of 1.
    ///     The structure is primarily used internally, but is exposed to applications in a read-only fashion because there are
    ///     some details in it that might be useful (like the number of rows/cols, or connected display information).  A user
    ///     can get the filled-in structure by calling NvAPI_Mosaic_GetTopoGroup().
    ///     You can then look at the detailed values within the structure.  There are no entry points which take this structure
    ///     as input (effectively making it read-only).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [StructureVersion(1)]
    public struct TopologyGroup : IInitializable, IEquatable<TopologyGroup>
    {
        /// <summary>
        ///     Maximum number of topologies per each group
        /// </summary>
        public const int MaxTopologyPerGroup = 2;

        internal StructureVersion _Version;
        internal readonly TopologyBrief _Brief;
        internal readonly uint _TopologiesCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxTopologyPerGroup)]
        internal readonly TopologyDetails[]
            _TopologyDetails;

        /// <summary>
        ///     The brief details of this topology
        /// </summary>
        public TopologyBrief Brief
        {
            get => _Brief;
        }

        /// <summary>
        ///     Information about the topologies within this group
        /// </summary>
        public TopologyDetails[] TopologyDetails
        {
            get => _TopologyDetails.Take((int)_TopologiesCount).ToArray();
        }

        /// <inheritdoc />
        public bool Equals(TopologyGroup other)
        {
            return _Brief.Equals(other._Brief) &&
                   _TopologiesCount == other._TopologiesCount &&
                   _TopologyDetails.SequenceEqual(other._TopologyDetails);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is TopologyGroup group && Equals(group);
        }

        /// <summary>
        ///     Checks for equality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are equal, otherwise false</returns>
        public static bool operator ==(TopologyGroup left, TopologyGroup right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Checks for inequality between two objects of same type
        /// </summary>
        /// <param name="left">The first object</param>
        /// <param name="right">The second object</param>
        /// <returns>true, if both objects are not equal, otherwise false</returns>
        public static bool operator !=(TopologyGroup left, TopologyGroup right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _Brief.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)_TopologiesCount;
                hashCode = (hashCode * 397) ^ (_TopologyDetails?.GetHashCode() ?? 0);

                return hashCode;
            }
        }
    }

    /// <summary>
    ///     Contains possible values for the color data command
    /// </summary>
    public enum ColorDataCommand : uint
    {
        /// <summary>
        ///     Get the current color data
        /// </summary>
        Get = 1,

        /// <summary>
        ///     Set the current color data
        /// </summary>
        Set,

        /// <summary>
        ///     Check if the passed color data is supported
        /// </summary>
        IsSupportedColor,

        /// <summary>
        ///     Get the default color data
        /// </summary>
        GetDefault
    }

    /// <summary>
    ///     Contains possible values for the color data depth
    /// </summary>
    public enum ColorDataDepth : uint
    {
        /// <summary>
        ///     Default color depth meaning that the current setting should be kept
        /// </summary>
        Default = 0,

        /// <summary>
        ///     6bit per color depth
        /// </summary>
        BPC6 = 1,

        /// <summary>
        ///     8bit per color depth
        /// </summary>
        BPC8 = 2,

        /// <summary>
        ///     10bit per color depth
        /// </summary>
        BPC10 = 3,

        /// <summary>
        ///     12bit per color depth
        /// </summary>
        BPC12 = 4,

        /// <summary>
        ///     16bit per color depth
        /// </summary>
        BPC16 = 5
    }

    /// <summary>
    ///     Contains possible values for the color data desktop color depth
    /// </summary>
    public enum ColorDataDesktopDepth : uint
    {
        /// <summary>
        ///     Default color depth meaning that the current setting should be kept
        /// </summary>
        Default = 0x0,

        /// <summary>
        ///     8bit per integer color component
        /// </summary>
        BPC8 = 0x1,

        /// <summary>
        ///     10bit integer per color component
        /// </summary>
        BPC10 = 0x2,

        /// <summary>
        ///     16bit float per color component
        /// </summary>
        BPC16Float = 0x3,

        /// <summary>
        ///     16bit float per color component wide color gamut
        /// </summary>
        BPC16FloatWcg = 0x4,

        /// <summary>
        ///     16bit float per color component HDR
        /// </summary>
        BPC16FloatHDR = 0x5
    }

    /// <summary>
    ///     Contains possible values for color data dynamic range
    /// </summary>
    public enum ColorDataDynamicRange : uint
    {
        /// <summary>
        ///     VESA standard progress signal
        /// </summary>
        VESA = 0,

        /// <summary>
        ///     CEA interlaced signal
        /// </summary>
        CEA,

        /// <summary>
        ///     Automatically select the best value
        /// </summary>
        Auto
    }

    /// <summary>
    ///     Contains possible color data color format values
    /// </summary>
    public enum ColorDataFormat : uint
    {
        /// <summary>
        ///     RGB color format
        /// </summary>
        RGB = 0,

        /// <summary>
        ///     YUV422 color format
        /// </summary>
        YUV422,

        /// <summary>
        ///     YUV444 color format
        /// </summary>
        YUV444,

        /// <summary>
        ///     YUV420 color format
        /// </summary>
        YUV420,

        /// <summary>
        ///     Default color format
        /// </summary>
        Default = 0xFE,

        /// <summary>
        ///     Automatically select the best color format
        /// </summary>
        Auto = 0xFF
    }

    /// <summary>
    ///     Contains possible values for the HDR color data command
    /// </summary>
    public enum ColorDataHDRCommand : uint
    {
        /// <summary>
        ///     Get the current HDR color data
        /// </summary>
        Get = 0,

        /// <summary>
        ///     Set the current HDR color data
        /// </summary>
        Set = 1
    }

    /// <summary>
    ///     Contains possible color data HDR modes
    /// </summary>
    public enum ColorDataHDRMode : uint
    {
        /// <summary>
        ///     Turn off HDR.
        /// </summary>
        Off = 0,

        /// <summary>
        ///     Source: CCCS [a.k.a FP16 scRGB, linear, sRGB primaries, [-65504,0, 65504] range, RGB(1,1,1) = 80nits]
        ///     Output: UHDA HDR [a.k.a HDR10, RGB/YCC 10/12bpc ST2084(PQ) EOTF RGB(1,1,1) = 10000 nits, Rec2020 color primaries,
        ///     ST2086 static HDR metadata].
        ///     This is the only supported production HDR mode.
        /// </summary>
        UHDA = 2,

        /// <summary>
        ///     Source: CCCS (a.k.a FP16 scRGB)
        ///     Output: EDR (Extended Dynamic Range) - HDR content is tone-mapped and gamut mapped to output on regular SDR display
        ///     set to max luminance ( ~300 nits ).
        /// </summary>
        [Obsolete("Do not use! Internal test mode only, to be removed.", false)]
        EDR = 3,

        /// <summary>
        ///     Source: any
        ///     Output: SDR (Standard Dynamic Range), we continuously send SDR EOTF InfoFrame signaling, HDMI compliance testing.
        /// </summary>
        [Obsolete("Do not use! Internal test mode only, to be removed.", false)]
        SDR = 4,

        /// <summary>
        ///     Source: HDR10 RGB 10bpc
        ///     Output: HDR10 RGB 10 colorDepth - signal UHDA HDR mode (PQ + Rec2020) to the sink but send source pixel values
        ///     unmodified (no PQ or Rec2020 conversions) - assumes source is already in HDR10 format.
        /// </summary>
        [Obsolete("Experimental mode only, not for production!", false)]
        UHDAPassthrough = 5,

        /// <summary>
        ///     Source: CCCS (a.k.a FP16 scRGB)
        ///     Output: notebook HDR
        /// </summary>
        [Obsolete("Do not use! Internal test mode only, to be removed.", false)]
        UHDANB = 6,

        /// <summary>
        ///     Source: RGB8 Dolby Vision encoded (12 colorDepth YCbCr422 packed into RGB8)
        ///     Output: Dolby Vision encoded : Application is to encoded frames in DV format and embed DV dynamic metadata as
        ///     described in Dolby Vision specification.
        /// </summary>
        [Obsolete("Experimental mode only, not for production!", false)]
        DolbyVision = 7
    }

    /// <summary>
    ///     Possible values for the color data selection policy
    /// </summary>
    public enum ColorDataSelectionPolicy : uint
    {
        /// <summary>
        ///     Application or the Nvidia Control Panel user configuration are used to decide the best color format
        /// </summary>
        User = 0,

        /// <summary>
        ///     Driver or the Operating System decides the best color format
        /// </summary>
        BestQuality = 1,

        /// <summary>
        ///     Default value, <see cref="BestQuality" />
        /// </summary>
        Default = BestQuality,

        /// <summary>
        ///     Unknown policy
        /// </summary>
        Unknown = 0xFF
    }

    /// <summary>
    ///     Possible color formats
    /// </summary>
    public enum ColorFormat
    {
        /// <summary>
        ///     Unknown, driver will choose one automatically.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     8bpp mode
        /// </summary>
        P8 = 41,

        /// <summary>
        ///     16bpp mode
        /// </summary>
        R5G6B5 = 23,

        /// <summary>
        ///     32bpp mode
        /// </summary>
        A8R8G8B8 = 21,

        /// <summary>
        ///     64bpp (floating point)
        /// </summary>
        A16B16G16R16F = 113
    }

    /// <summary>
    ///     Flags for applying settings, used by NvAPI_DISP_SetDisplayConfig()
    /// </summary>
    [Flags]
    public enum DisplayConfigFlags
    {
        /// <summary>
        ///     None
        /// </summary>
        None = 0,

        /// <summary>
        ///     Do not apply
        /// </summary>
        ValidateOnly = 0x00000001,

        /// <summary>
        ///     Save to the persistence storage
        /// </summary>
        SaveToPersistence = 0x00000002,

        /// <summary>
        ///     Driver reload is permitted if necessary
        /// </summary>
        DriverReloadAllowed = 0x00000004,

        /// <summary>
        ///     Refresh OS mode list.
        /// </summary>
        ForceModeEnumeration = 0x00000008,

        /// <summary>
        ///     Tell OS to avoid optimizing CommitVidPn call during a modeset
        /// </summary>
        ForceCommitVideoPresentNetwork = 0x00000010
    }

    /// <summary>
    /// Possible display port color depths
    /// </summary>
    public enum DisplayPortColorDepth : uint
    {
        /// <summary>
        /// Default color depth
        /// </summary>
        Default = 0,
        /// <summary>
        /// 6 bit per color color depth
        /// </summary>
        BPC6,
        /// <summary>
        /// 8 bit per color color depth
        /// </summary>
        BPC8,
        /// <summary>
        /// 10 bit per color color depth
        /// </summary>
        BPC10,
        /// <summary>
        /// 12 bit per color color depth
        /// </summary>
        BPC12,

        /// <summary>
        /// 16 bit per color color depth
        /// </summary>
        BPC16,
    }

    /// <summary>
    ///     Possible display port color formats
    /// </summary>
    public enum DisplayPortColorFormat : uint
    {
        /// <summary>
        ///     RGB color format
        /// </summary>
        RGB = 0,

        /// <summary>
        ///     YCbCr422 color format
        /// </summary>
        YCbCr422 = 1,

        /// <summary>
        ///     YCbCr444 color format
        /// </summary>
        YCbCr444 = 2
    }

    /// <summary>
    ///     Contains possible audio channel allocations (speaker placements)
    /// </summary>
    public enum InfoFrameAudioChannelAllocation : uint
    {
        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Empty [4] Empty [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        FrFl = 0,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Empty [4] Empty [5] Low Frequency Effects [6] Front Right [7] Front Left
        /// </summary>
        LfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Empty [4] Front Center [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        FcFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Empty [4] Front Center [5] Low Frequency Effects [6] Front Right [7] Front Left
        /// </summary>
        FcLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Rear Center [4] Empty [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        RcFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Rear Center [4] Empty [5] Low Frequency Effects [6] Front Right [7] Front Left
        /// </summary>
        RcLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Rear Center [4] Front Center [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        RcFcFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Empty [3] Rear Center [4] Front Center [5] Low Frequency Effects [6] Front Right [7] Front
        ///     Left
        /// </summary>
        RcFcLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Rear Right [3] Rear Left [4] Empty [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        RrRlFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Rear Right [3] Rear Left [4] Empty [5] Low Frequency Effects [6] Front Right [7] Front Left
        /// </summary>
        RrRlLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        RrRlFcFrFl,

        /// <summary>
        ///     [0] Empty [1] Empty [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6] Front Right [7]
        ///     Front Left
        /// </summary>
        RrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Rear Center [2] Rear Right [3] Rear Left [4] Empty [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        RcRrRlFrFl,

        /// <summary>
        ///     [0] Empty [1] Rear Center [2] Rear Right [3] Rear Left [4] Empty [5] Low Frequency Effects [6] Front Right [7]
        ///     Front Left
        /// </summary>
        RcRrRlLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Rear Center [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        RcRrRlFcFrFl,

        /// <summary>
        ///     [0] Empty [1] Rear Center [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6] Front Right
        ///     [7] Front Left
        /// </summary>
        RcRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Rear Right Of Center [1] Rear Left Of Center [2] Rear Right [3] Rear Left [4] Empty [5] Empty [6] Front Right
        ///     [7] Front Left
        /// </summary>
        RrcRlcRrRlFrFl,

        /// <summary>
        ///     [0] Rear Right Of Center [1] Rear Left Of Center [2] Rear Right [3] Rear Left [4] Empty [5] Low Frequency Effects
        ///     [6] Front Right [7] Front Left
        /// </summary>
        RrcRlcRrRlLfeFrFl,

        /// <summary>
        ///     [0] Rear Right Of Center [1] Rear Left Of Center [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front
        ///     Right [7] Front Left
        /// </summary>
        RrcRlcRrRlFcFrFl,

        /// <summary>
        ///     [0] Rear Right Of Center [1] Rear Left Of Center [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency
        ///     Effects [6] Front Right [7] Front Left
        /// </summary>
        RrcRlcRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Empty [4] Empty [5] Empty [6] Front Right [7]
        ///     Front Left
        /// </summary>
        FrcFlcFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Empty [4] Empty [5] Low Frequency Effects [6]
        ///     Front Right [7] Front Left
        /// </summary>
        FrcFlcLfeFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Empty [4] Front Center [5] Empty [6] Front Right
        ///     [7] Front Left
        /// </summary>
        FrcFlcFcFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Empty [4] Front Center [5] Low Frequency Effects
        ///     [6] Front Right [7] Front Left
        /// </summary>
        FrcFlcFcLfeFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Rear Center [4] Empty [5] Empty [6] Front Right
        ///     [7] Front Left
        /// </summary>
        FrcFlcRcFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Rear Center [4] Empty [5] Low Frequency Effects
        ///     [6] Front Right [7] Front Left
        /// </summary>
        FrcFlcRcLfeFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Rear Center [4] Front Center [5] Empty [6] Front
        ///     Right [7] Front Left
        /// </summary>
        FrcFlcRcFcFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Empty [3] Rear Center [4] Front Center [5] Low Frequency
        ///     Effects [6] Front Right [7] Front Left
        /// </summary>
        FrcFlcRcFcLfeFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Rear Right [3] Rear Left [4] Empty [5] Empty [6] Front Right
        ///     [7] Front Left
        /// </summary>
        FrcFlcRrRlFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Rear Right [3] Rear Left [4] Empty [5] Low Frequency Effects
        ///     [6] Front Right [7] Front Left
        /// </summary>
        FrcFlcRrRlLfeFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6]
        ///     Front Right [7] Front Left
        /// </summary>
        FrcFlcRrRlFcFrFl,

        /// <summary>
        ///     [0] Front Right Of Center [1] Front Left Of Center [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency
        ///     Effects [6] Front Right [7] Front Left
        /// </summary>
        FrcFlcRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Empty [1] Front Center High [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7] Front
        ///     Left
        /// </summary>
        FchRrRlFcFrFl,

        /// <summary>
        ///     [0] Empty [1] Front Center High [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6] Front
        ///     Right [7] Front Left
        /// </summary>
        FchRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] TopCenter [1] Empty [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7] Front Left
        /// </summary>
        TcRrRlFcFrFl,

        /// <summary>
        ///     [0] TopCenter [1] Empty [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6] Front Right [7]
        ///     Front Left
        /// </summary>
        TcRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Front Right High [1] Front Left High [2] Rear Right [3] Rear Left [4] Empty [5] Empty [6] Front Right [7] Front
        ///     Left
        /// </summary>
        FrhFlhRrRlFrFl,

        /// <summary>
        ///     [0] Front Right High [1] Front Left High [2] Rear Right [3] Rear Left [4] Empty [5] Low Frequency Effects [6] Front
        ///     Right [7] Front Left
        /// </summary>
        FrhFlhRrRlLfeFrFl,

        /// <summary>
        ///     [0] Front Right Wide [1] Front Left Wide [2] Rear Right [3] Rear Left [4] Empty [5] Empty [6] Front Right [7] Front
        ///     Left
        /// </summary>
        FrwFlwRrRlFrFl,

        /// <summary>
        ///     [0] Front Right Wide [1] Front Left Wide [2] Rear Right [3] Rear Left [4] Empty [5] Low Frequency Effects [6] Front
        ///     Right [7] Front Left
        /// </summary>
        FrwFlwRrRlLfeFrFl,

        /// <summary>
        ///     [0] TopCenter [1] Rear Center [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7] Front
        ///     Left
        /// </summary>
        TcRcRrRlFcFrFl,

        /// <summary>
        ///     [0] TopCenter [1] Rear Center [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6] Front
        ///     Right [7] Front Left
        /// </summary>
        TcRcRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Front Center High [1] Rear Center [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7]
        ///     Front Left
        /// </summary>
        FchRcRrRlFcFrFl,

        /// <summary>
        ///     [0] Front Center High [1] Rear Center [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6]
        ///     Front Right [7] Front Left
        /// </summary>
        FchRcRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] TopCenter [1] Front Center High [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right [7]
        ///     Front Left
        /// </summary>
        TcFcRrRlFcFrFl,

        /// <summary>
        ///     [0] TopCenter [1] Front Center High [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects [6]
        ///     Front Right [7] Front Left
        /// </summary>
        TcFchRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Front Right High [1] Front Left High [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right
        ///     [7] Front Left
        /// </summary>
        FrhFlhRrRlFcFrFl,

        /// <summary>
        ///     [0] Front Right High [1] Front Left High [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects
        ///     [6] Front Right [7] Front Left
        /// </summary>
        FrhFlhRrRlFcLfeFrFl,

        /// <summary>
        ///     [0] Front Right Wide [1] Front Left Wide [2] Rear Right [3] Rear Left [4] Front Center [5] Empty [6] Front Right
        ///     [7] Front Left
        /// </summary>
        FrwFlwRrRlFcFeFl,

        /// <summary>
        ///     [0] Front Right Wide [1] Front Left Wide [2] Rear Right [3] Rear Left [4] Front Center [5] Low Frequency Effects
        ///     [6] Front Right [7] Front Left
        /// </summary>
        FrwFlwRrRlFcLfeFrFl,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 511
    }

    /// <summary>
    ///     Contains possible audio channels
    /// </summary>
    public enum InfoFrameAudioChannelCount : uint
    {
        /// <summary>
        ///     Data is available in the header of source data
        /// </summary>
        InHeader = 0,

        /// <summary>
        ///     Two channels
        /// </summary>
        Two,

        /// <summary>
        ///     Three channels
        /// </summary>
        Three,

        /// <summary>
        ///     Four channels
        /// </summary>
        Four,

        /// <summary>
        ///     Five channels
        /// </summary>
        Five,

        /// <summary>
        ///     Six channels
        /// </summary>
        Six,

        /// <summary>
        ///     Seven channels
        /// </summary>
        Seven,

        /// <summary>
        ///     Eight channels
        /// </summary>
        Eight,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 15
    }

    /// <summary>
    ///     Contains possible audio codecs
    /// </summary>
    public enum InfoFrameAudioCodec : uint
    {
        /// <summary>
        ///     Data is available in the header of source data
        /// </summary>
        InHeader = 0,

        /// <summary>
        ///     Pulse-code modulation
        /// </summary>
        PCM,

        /// <summary>
        ///     Dolby AC-3
        /// </summary>
        AC3,

        /// <summary>
        ///     MPEG1
        /// </summary>
        MPEG1,

        /// <summary>
        ///     MP3 (MPEG-2 Audio Layer III)
        /// </summary>
        MP3,

        /// <summary>
        ///     MPEG2
        /// </summary>
        MPEG2,

        /// <summary>
        ///     Advanced Audio Coding
        /// </summary>
        AACLC,

        /// <summary>
        ///     DTS
        /// </summary>
        DTS,

        /// <summary>
        ///     Adaptive Transform Acoustic Coding
        /// </summary>
        ATRAC,

        /// <summary>
        ///     Direct Stream Digital
        /// </summary>
        DSD,

        /// <summary>
        ///     Dolby Digital Plus
        /// </summary>
        EAC3,

        /// <summary>
        ///     DTS High Definition
        /// </summary>
        DTSHD,

        /// <summary>
        ///     Meridian Lossless Packing
        /// </summary>
        MLP,

        /// <summary>
        ///     DST
        /// </summary>
        DST,

        /// <summary>
        ///     Windows Media Audio Pro
        /// </summary>
        WMAPRO,

        /// <summary>
        ///     Extended audio codec value should be used to get information regarding audio codec
        /// </summary>
        UseExtendedCodecType,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 31
    }

    /// <summary>
    ///     Contains possible extended audio codecs
    /// </summary>
    public enum InfoFrameAudioExtendedCodec : uint
    {
        /// <summary>
        ///     Use the primary audio codec type, data not available
        /// </summary>
        UseCodecType = 0,

        /// <summary>
        ///     High-Efficiency Advanced Audio Coding
        /// </summary>
        HEAAC,

        /// <summary>
        ///     High-Efficiency Advanced Audio Coding 2
        /// </summary>
        HEAACVersion2,

        /// <summary>
        ///     MPEG Surround
        /// </summary>
        MPEGSurround,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 63
    }

    /// <summary>
    ///     Contains possible audio low frequency effects channel playback level
    /// </summary>
    public enum InfoFrameAudioLFEPlaybackLevel : uint
    {
        /// <summary>
        ///     Data not available
        /// </summary>
        NoData = 0,

        /// <summary>
        ///     No change to the source audio
        /// </summary>
        Plus0Decibel,

        /// <summary>
        ///     Adds 10 decibel
        /// </summary>
        Plus10Decibel,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }

    /// <summary>
    ///     Contains possible audio channel level shift values
    /// </summary>
    public enum InfoFrameAudioLevelShift : uint
    {
        /// <summary>
        ///     No change to the source audio
        /// </summary>
        Shift0Decibel = 0,

        /// <summary>
        ///     Shifts 1 decibel
        /// </summary>
        Shift1Decibel,

        /// <summary>
        ///     Shifts 2 decibel
        /// </summary>
        Shift2Decibel,

        /// <summary>
        ///     Shifts 3 decibel
        /// </summary>
        Shift3Decibel,

        /// <summary>
        ///     Shifts 4 decibel
        /// </summary>
        Shift4Decibel,

        /// <summary>
        ///     Shifts 5 decibel
        /// </summary>
        Shift5Decibel,

        /// <summary>
        ///     Shifts 6 decibel
        /// </summary>
        Shift6Decibel,

        /// <summary>
        ///     Shifts 7 decibel
        /// </summary>
        Shift7Decibel,

        /// <summary>
        ///     Shifts 8 decibel
        /// </summary>
        Shift8Decibel,

        /// <summary>
        ///     Shifts 9 decibel
        /// </summary>
        Shift9Decibel,

        /// <summary>
        ///     Shifts 10 decibel
        /// </summary>
        Shift10Decibel,

        /// <summary>
        ///     Shifts 11 decibel
        /// </summary>
        Shift11Decibel,

        /// <summary>
        ///     Shifts 12 decibel
        /// </summary>
        Shift12Decibel,

        /// <summary>
        ///     Shifts 13 decibel
        /// </summary>
        Shift13Decibel,

        /// <summary>
        ///     Shifts 14 decibel
        /// </summary>
        Shift14Decibel,

        /// <summary>
        ///     Shifts 15 decibel
        /// </summary>
        Shift15Decibel,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 31
    }

    /// <summary>
    ///     Contains possible audio sample rates (sampling frequency)
    /// </summary>
    public enum InfoFrameAudioSampleRate : uint
    {
        /// <summary>
        ///     Data is available in the header of source data
        /// </summary>
        InHeader = 0,

        /// <summary>
        ///     31kHz sampling frequency
        /// </summary>
        F32000Hz,

        /// <summary>
        ///     44.1kHz sampling frequency
        /// </summary>
        F44100Hz,

        /// <summary>
        ///     48kHz sampling frequency
        /// </summary>
        F48000Hz,

        /// <summary>
        ///     88.2kHz sampling frequency
        /// </summary>
        F88200Hz,

        /// <summary>
        ///     96kHz sampling frequency
        /// </summary>
        F96000Hz,

        /// <summary>
        ///     176.4kHz sampling frequency
        /// </summary>
        F176400Hz,

        /// <summary>
        ///     192kHz sampling frequency
        /// </summary>
        F192000Hz,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 15
    }

    /// <summary>
    ///     Contains possible audio sample size (bit depth)
    /// </summary>
    public enum InfoFrameAudioSampleSize : uint
    {
        /// <summary>
        ///     Data is available in the header of source data
        /// </summary>
        InHeader = 0,

        /// <summary>
        ///     16bit audio sample size
        /// </summary>
        B16,

        /// <summary>
        ///     20bit audio sample size
        /// </summary>
        B20,

        /// <summary>
        ///     24bit audio sample size
        /// </summary>
        B24,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }

    /// <summary>
    ///     Contains possible values for info-frame properties that accept or return a boolean value
    /// </summary>
    public enum InfoFrameBoolean : uint
    {
        /// <summary>
        ///     False
        /// </summary>
        False = 0,

        /// <summary>
        ///     True
        /// </summary>
        True,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 3
    }

    /// <summary>
    ///     Possible commands for info-frame operations
    /// </summary>
    public enum InfoFrameCommand : uint
    {
        /// <summary>
        ///     Returns the fields in the info-frame with values set by the manufacturer (NVIDIA or OEM)
        /// </summary>
        GetDefault = 0,

        /// <summary>
        ///     Sets the fields in the info-frame to auto, and info-frame to the default info-frame for use in a set.
        /// </summary>
        Reset,

        /// <summary>
        ///     Get the current info-frame state.
        /// </summary>
        Get,

        /// <summary>
        ///     Set the current info-frame state (flushed to the monitor), the values are one time and do not persist.
        /// </summary>
        Set,

        /// <summary>
        ///     Get the override info-frame state, non-override fields will be set to value = AUTO, overridden fields will have the
        ///     current override values.
        /// </summary>
        GetOverride,

        /// <summary>
        ///     Set the override info-frame state, non-override fields will be set to value = AUTO, other values indicate override;
        ///     persist across mode-set and reboot.
        /// </summary>
        SetOverride,

        /// <summary>
        ///     Get properties associated with info-frame (each of the info-frame type will have properties).
        /// </summary>
        GetProperty,

        /// <summary>
        ///     Set properties associated with info-frame.
        /// </summary>
        SetProperty
    }

    /// <summary>
    ///     Contains possible info-frame data type
    /// </summary>
    public enum InfoFrameDataType : uint
    {
        /// <summary>
        ///     Auxiliary Video data
        /// </summary>
        AuxiliaryVideoInformation = 2,

        /// <summary>
        ///     Audio data
        /// </summary>
        AudioInformation = 4,
    }

    /// <summary>
    ///     Contains possible info-frame property modes
    /// </summary>
    public enum InfoFramePropertyMode : uint
    {
        /// <summary>
        ///     Driver determines whether to send info-frames.
        /// </summary>
        Auto = 0,

        /// <summary>
        ///     Driver always sends info-frame.
        /// </summary>
        Enable,

        /// <summary>
        ///     Driver never sends info-frame.
        /// </summary>
        Disable,

        /// <summary>
        ///     Driver only sends info-frame when client requests it via info-frame escape call.
        /// </summary>
        AllowOverride
    }

    /// <summary>
    ///     Contains possible values for AVI aspect ratio portions
    /// </summary>
    public enum InfoFrameVideoAspectRatioActivePortion : uint
    {
        /// <summary>
        ///     Disabled or not available
        /// </summary>
        Disabled = 0,

        /// <summary>
        ///     Letter box 16x9
        /// </summary>
        LetterboxGreaterThan16X9 = 4,

        /// <summary>
        ///     Equal to the source frame size
        /// </summary>
        EqualCodedFrame = 8,

        /// <summary>
        ///     Centered 4x3 ratio
        /// </summary>
        Center4X3 = 9,

        /// <summary>
        ///     Centered 16x9 ratio
        /// </summary>
        Center16X9 = 10,

        /// <summary>
        ///     Centered 14x9 ratio
        /// </summary>
        Center14X9 = 11,

        /// <summary>
        ///     Bordered 4x3 on 14x9
        /// </summary>
        Bordered4X3On14X9 = 13,

        /// <summary>
        ///     Bordered 16x9 on 14x9
        /// </summary>
        Bordered16X9On14X9 = 14,

        /// <summary>
        ///     Bordered 16x9 on 4x3
        /// </summary>
        Bordered16X9On4X3 = 15,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 31
    }

    /// <summary>
    ///     Gets the possible values for AVI source aspect ratio
    /// </summary>
    public enum InfoFrameVideoAspectRatioCodedFrame : uint
    {
        /// <summary>
        ///     No data available
        /// </summary>
        NoData = 0,

        /// <summary>
        ///     The 4x3 aspect ratio
        /// </summary>
        Aspect4X3,

        /// <summary>
        ///     The 16x9 aspect ratio
        /// </summary>
        Aspect16X9,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }

    /// <summary>
    ///     Contains possible AVI bar data that are available and should be used
    /// </summary>
    public enum InfoFrameVideoBarData : uint
    {
        /// <summary>
        ///     No bar data present
        /// </summary>
        NotPresent = 0,

        /// <summary>
        ///     Vertical bar
        /// </summary>
        Vertical,

        /// <summary>
        ///     Horizontal bar
        /// </summary>
        Horizontal,

        /// <summary>
        ///     Both sides have bars
        /// </summary>
        Both,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }

    /// <summary>
    ///     Contains possible AVI color formats
    /// </summary>
    public enum InfoFrameVideoColorFormat : uint
    {
        /// <summary>
        ///     The RGB color format
        /// </summary>
        RGB = 0,

        /// <summary>
        ///     The YCbCr422 color format
        /// </summary>
        YCbCr422,

        /// <summary>
        ///     The YCbCr444 color format
        /// </summary>
        YCbCr444,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }

    /// <summary>
    ///     Contains possible values for the AVI color space
    /// </summary>
    public enum InfoFrameVideoColorimetry : uint
    {
        /// <summary>
        ///     No data available
        /// </summary>
        NoData = 0,

        /// <summary>
        ///     The SMPTE170M color space
        /// </summary>
        SMPTE170M,

        /// <summary>
        ///     The ITURBT709 color space
        /// </summary>
        ITURBT709,

        /// <summary>
        ///     Extended colorimetry value should be used to get information regarding AVI color space
        /// </summary>
        UseExtendedColorimetry,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }

    /// <summary>
    ///     Contains possible AVI content type
    /// </summary>
    public enum InfoFrameVideoContentType : uint
    {
        /// <summary>
        ///     Graphics content
        /// </summary>
        Graphics = 0,

        /// <summary>
        ///     Photo content
        /// </summary>
        Photo,

        /// <summary>
        ///     Cinematic content
        /// </summary>
        Cinema,

        /// <summary>
        ///     Gaming content
        /// </summary>
        Game,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }

    /// <summary>
    ///     Contains possible values for the AVI extended color space
    /// </summary>
    public enum InfoFrameVideoExtendedColorimetry : uint
    {
        /// <summary>
        ///     The xvYCC601 color space
        /// </summary>
        // ReSharper disable once InconsistentNaming
        xvYCC601 = 0,

        /// <summary>
        ///     The xvYCC709 color space
        /// </summary>
        // ReSharper disable once InconsistentNaming
        xvYCC709,

        /// <summary>
        ///     The sYCC601 color space
        /// </summary>
        // ReSharper disable once InconsistentNaming
        sYCC601,

        /// <summary>
        ///     The AdobeYCC601 color space
        /// </summary>
        AdobeYCC601,

        /// <summary>
        ///     The AdobeRGB color space
        /// </summary>
        AdobeRGB,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 15
    }

    /// <summary>
    ///     Contains possible AVI video content modes
    /// </summary>
    public enum InfoFrameVideoITC : uint
    {
        /// <summary>
        ///     Normal video content (Consumer Electronics)
        /// </summary>
        VideoContent = 0,

        /// <summary>
        ///     Information Technology content
        /// </summary>
        ITContent,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 3
    }

    /// <summary>
    ///     Contains possible values for the AVI non uniform picture scaling
    /// </summary>
    public enum InfoFrameVideoNonUniformPictureScaling : uint
    {
        /// <summary>
        ///     No data available
        /// </summary>
        NoData = 0,

        /// <summary>
        ///     Horizontal scaling
        /// </summary>
        Horizontal,

        /// <summary>
        ///     Vertical scaling
        /// </summary>
        Vertical,

        /// <summary>
        ///     Scaling in both directions
        /// </summary>
        Both,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }

    /// <summary>
    ///     Contains possible AVI pixel repetition values
    /// </summary>
    public enum InfoFrameVideoPixelRepetition : uint
    {
        /// <summary>
        ///     No pixel repetition
        /// </summary>
        None = 0,

        /// <summary>
        ///     Two pixel repetition
        /// </summary>
        X2,

        /// <summary>
        ///     Three pixel repetition
        /// </summary>
        X3,

        /// <summary>
        ///     Four pixel repetition
        /// </summary>
        X4,

        /// <summary>
        ///     Five pixel repetition
        /// </summary>
        X5,

        /// <summary>
        ///     Six pixel repetition
        /// </summary>
        X6,

        /// <summary>
        ///     Seven pixel repetition
        /// </summary>
        X7,

        /// <summary>
        ///     Eight pixel repetition
        /// </summary>
        X8,

        /// <summary>
        ///     Nine pixel repetition
        /// </summary>
        X9,

        /// <summary>
        ///     Ten pixel repetition
        /// </summary>
        X10,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 31
    }

    /// <summary>
    ///     Contains possible values for the AVI RGB quantization
    /// </summary>
    public enum InfoFrameVideoRGBQuantization : uint
    {
        /// <summary>
        ///     Default setting
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Limited RGB range [16-235] (86%)
        /// </summary>
        LimitedRange,

        /// <summary>
        ///     Full RGB range [0-255] (100%)
        /// </summary>
        FullRange,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }

    /// <summary>
    ///     Contains possible values for AVI scan information
    /// </summary>
    public enum InfoFrameVideoScanInfo : uint
    {
        /// <summary>
        ///     No data available
        /// </summary>
        NoData = 0,

        /// <summary>
        ///     Overscan
        /// </summary>
        OverScan,

        /// <summary>
        ///     Underscan
        /// </summary>
        UnderScan,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }

    /// <summary>
    ///     Contains possible AVI YCC quantization
    /// </summary>
    public enum InfoFrameVideoYCCQuantization : uint
    {
        /// <summary>
        ///     Limited YCC range
        /// </summary>
        LimitedRange = 0,

        /// <summary>
        ///     Full YCC range
        /// </summary>
        FullRange,

        /// <summary>
        ///     Auto (Unspecified)
        /// </summary>
        Auto = 7
    }

    /// <summary>
    ///     Possible values for the monitor capabilities connector type
    /// </summary>
    public enum MonitorCapabilitiesConnectorType : uint
    {
        /// <summary>
        ///     Unknown or invalid connector
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     VGA connector
        /// </summary>
        VGA,

        /// <summary>
        ///     Composite connector (TV)
        /// </summary>
        TV,

        /// <summary>
        ///     DVI connector
        /// </summary>
        DVI,

        /// <summary>
        ///     HDMI connector
        /// </summary>
        HDMI,

        /// <summary>
        ///     Display Port connector
        /// </summary>
        DisplayPort
    }

    /// <summary>
    ///     Contains possible values for the monitor capabilities type
    /// </summary>
    public enum MonitorCapabilitiesType : uint
    {
        /// <summary>
        ///     The Vendor Specific Data Block
        /// </summary>
        VSDB = 0x1000,

        /// <summary>
        ///     The Video Capability Data Block
        /// </summary>
        VCDB = 0x1001
    }

    /// <summary>
    ///     Possible rotate modes
    /// </summary>
    public enum Rotate : uint
    {
        /// <summary>
        ///     No rotation
        /// </summary>
        Degree0 = 0,

        /// <summary>
        ///     90 degree rotation
        /// </summary>
        Degree90 = 1,

        /// <summary>
        ///     180 degree rotation
        /// </summary>
        Degree180 = 2,

        /// <summary>
        ///     270 degree rotation
        /// </summary>
        Degree270 = 3,

        /// <summary>
        ///     This value is ignored
        /// </summary>
        Ignored = 4
    }

    /// <summary>
    ///     Possible scaling modes
    /// </summary>
    public enum Scaling
    {
        /// <summary>
        ///     No change
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Balanced  - Full Screen
        /// </summary>
        ToClosest = 1,

        /// <summary>
        ///     Force GPU - Full Screen
        /// </summary>
        ToNative = 2,

        /// <summary>
        ///     Force GPU - Centered\No Scaling
        /// </summary>
        GPUScanOutToNative = 3,

        /// <summary>
        ///     Force GPU - Aspect Ratio
        /// </summary>
        ToAspectScanOutToNative = 5,

        /// <summary>
        ///     Balanced  - Aspect Ratio
        /// </summary>
        ToAspectScanOutToClosest = 6,

        /// <summary>
        ///     Balanced  - Centered\No Scaling
        /// </summary>
        GPUScanOutToClosest = 7,

        /// <summary>
        ///     Customized scaling - For future use
        /// </summary>
        Customized = 255
    }

    /// <summary>
    ///     Holds a list of possible scan out composition configurable parameters
    /// </summary>
    public enum ScanOutCompositionParameter : uint
    {
        /// <summary>
        ///     Warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethod = 0
    }

    /// <summary>
    ///     Holds a list of possible scan out composition parameter values
    /// </summary>
    public enum ScanOutCompositionParameterValue : uint
    {
        /// <summary>
        ///     Default parameter value
        /// </summary>
        Default = 0,

        /// <summary>
        ///     BiLinear value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBiLinear = 0x100,

        /// <summary>
        ///     Bicubic Triangular value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBicubicTriangular = 0x101,

        /// <summary>
        ///     Bicubic Bell Shaped value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBicubicBellShaped = 0x102,

        /// <summary>
        ///     Bicubic B-Spline value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBicubicBSpline = 0x103,

        /// <summary>
        ///     Bicubic Adaptive Triangular value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBicubicAdaptiveTriangular = 0x104,

        /// <summary>
        ///     Bicubic Adaptive Bell Shaped value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBicubicAdaptiveBellShaped = 0x105,

        /// <summary>
        ///     Bicubic Adaptive B-Spline value for the warping re-sampling method parameter
        /// </summary>
        WarpingReSamplingMethodBicubicAdaptiveBSpline = 0x106
    }

    / <summary>
    ///     Display spanning for Windows XP
    /// </summary>
    public enum SpanningOrientation
    {
        /// <summary>
        ///     No spanning
        /// </summary>
        None = 0,

        /// <summary>
        ///     Horizontal spanning
        /// </summary>
        Horizontal = 1,

        /// <summary>
        ///     Vertical spanning
        /// </summary>
        Vertical = 2
    }

    /// <summary>
    ///     Contains possible values for the type of the Static Metadata Descriptor block structure
    /// </summary>
    public enum StaticMetadataDescriptorId : uint
    {
        /// <summary>
        ///     Type 1 Static Metadata Descriptor block structure
        /// </summary>
        StaticMetadataType1 = 0
    }

    /// <summary>
    ///     Possible TV formats
    /// </summary>
    public enum TVFormat : uint
    {
        /// <summary>
        ///     Display is not a TV
        /// </summary>
        None = 0,

        /// <summary>
        ///     Standard definition NTSC M signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD_NTSCM = 0x00000001,

        /// <summary>
        ///     Standard definition NTSC J signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD_NTSCJ = 0x00000002,

        /// <summary>
        ///     Standard definition PAL M signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD_PALM = 0x00000004,

        /// <summary>
        ///     Standard definition PAL DFGH signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD_PALBDGH = 0x00000008,

        /// <summary>
        ///     Standard definition PAL N signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD_PAL_N = 0x00000010,

        /// <summary>
        ///     Standard definition PAL NC signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD_PAL_NC = 0x00000020,

        /// <summary>
        ///     Extended definition with height of 576 pixels interlaced
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD576i = 0x00000100,

        /// <summary>
        ///     Extended definition with height of 480 pixels interlaced
        /// </summary>
        // ReSharper disable once InconsistentNaming
        SD480i = 0x00000200,

        /// <summary>
        ///     Extended definition with height of 480 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        ED480p = 0x00000400,

        /// <summary>
        ///     Extended definition with height of 576 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        ED576p = 0x00000800,

        /// <summary>
        ///     High definition with height of 720 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD720p = 0x00001000,

        /// <summary>
        ///     High definition with height of 1080 pixels interlaced
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD1080i = 0x00002000,

        /// <summary>
        ///     High definition with height of 1080 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD1080p = 0x00004000,

        /// <summary>
        ///     High definition 50 frames per second with height of 720 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD720p50 = 0x00008000,

        /// <summary>
        ///     High definition 24 frames per second with height of 1080 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD1080p24 = 0x00010000,

        /// <summary>
        ///     High definition 50 frames per second with height of 1080 pixels interlaced
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD1080i50 = 0x00020000,

        /// <summary>
        ///     High definition 50 frames per second with height of 1080 pixels progressive
        /// </summary>
        // ReSharper disable once InconsistentNaming
        HD1080p50 = 0x00040000,

        /// <summary>
        ///     Ultra high definition 30 frames per second
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp30 = 0x00080000,

        /// <summary>
        ///     Ultra high definition 30 frames per second with width of 3840 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp30_3840 = UHD4Kp30,

        /// <summary>
        ///     Ultra high definition 25 frames per second
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp25 = 0x00100000,

        /// <summary>
        ///     Ultra high definition 25 frames per second with width of 3840 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp25_3840 = UHD4Kp25,

        /// <summary>
        ///     Ultra high definition 24 frames per second
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp24 = 0x00200000,

        /// <summary>
        ///     Ultra high definition 24 frames per second with width of 3840 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp24_3840 = UHD4Kp24,

        /// <summary>
        ///     Ultra high definition 24 frames per second with SMPTE signal
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp24_SMPTE = 0x00400000,

        /// <summary>
        ///     Ultra high definition 50 frames per second with width of 3840 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp50_3840 = 0x00800000,

        /// <summary>
        ///     Ultra high definition 60 frames per second with width of 3840 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp60_3840 = 0x00900000,

        /// <summary>
        ///     Ultra high definition 30 frames per second with width of 4096 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp30_4096 = 0x00A00000,

        /// <summary>
        ///     Ultra high definition 25 frames per second with width of 4096 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp25_4096 = 0x00B00000,

        /// <summary>
        ///     Ultra high definition 24 frames per second with width of 4096 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp24_4096 = 0x00C00000,

        /// <summary>
        ///     Ultra high definition 50 frames per second with width of 4096 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp50_4096 = 0x00D00000,

        /// <summary>
        ///     Ultra high definition 60 frames per second with width of 4096 pixels
        /// </summary>
        // ReSharper disable once InconsistentNaming
        UHD4Kp60_4096 = 0x00E00000,

        /// <summary>
        ///     Any other standard definition TV format
        /// </summary>
        SDOther = 0x01000000,

        /// <summary>
        ///     Any other extended definition TV format
        /// </summary>
        EDOther = 0x02000000,

        /// <summary>
        ///     Any other high definition TV format
        /// </summary>
        HDOther = 0x04000000,

        /// <summary>
        ///     Any other TV format
        /// </summary>
        Any = 0x80000000
    }

    /// <summary>
    ///     Display view modes
    /// </summary>
    public enum TargetViewMode
    {
        /// <summary>
        ///     Standard view mode
        /// </summary>
        Standard = 0,

        /// <summary>
        ///     Cloned view mode
        /// </summary>
        Clone = 1,

        /// <summary>
        ///     Horizontal span view mode
        /// </summary>
        HorizontalSpan = 2,

        /// <summary>
        ///     Vertical span view mode
        /// </summary>
        VerticalSpan = 3,

        /// <summary>
        ///     Dual view mode
        /// </summary>
        DualView = 4,

        /// <summary>
        ///     Multi view mode
        /// </summary>
        MultiView = 5
    }

    /// <summary>
    ///     Horizontal synchronized polarity modes
    /// </summary>
    public enum TimingHorizontalSyncPolarity : byte
    {
        /// <summary>
        ///     Positive horizontal synchronized polarity
        /// </summary>
        Positive = 0,

        /// <summary>
        ///     Negative horizontal synchronized polarity
        /// </summary>
        Negative = 1,

        /// <summary>
        ///     Default horizontal synchronized polarity
        /// </summary>
        Default = Negative
    }

    /// <summary>
    ///     Timing override modes
    /// </summary>
    public enum TimingOverride
    {
        /// <summary>
        ///     Current timing
        /// </summary>
        Current = 0,

        /// <summary>
        ///     Auto timing
        /// </summary>
        Auto,

        /// <summary>
        ///     EDID timing
        /// </summary>
        EDID,

        /// <summary>
        ///     VESA DMT timing
        /// </summary>
        DMT,

        /// <summary>
        ///     VESA DMT timing with reduced blanking
        /// </summary>
        DMTReducedBlanking,

        /// <summary>
        ///     VESA CVT timing
        /// </summary>
        CVT,

        /// <summary>
        ///     VESA CVT timing with reduced blanking
        /// </summary>
        CVTReducedBlanking,

        /// <summary>
        ///     VESA GTF
        /// </summary>
        GTF,

        /// <summary>
        ///     EIA 861x PreDefined timing
        /// </summary>
        EIA861,

        /// <summary>
        ///     AnalogTV PreDefined timing
        /// </summary>
        AnalogTV,

        /// <summary>
        ///     NVIDIA Custom timing
        /// </summary>
        Custom,

        /// <summary>
        ///     NVIDIA PreDefined timing
        /// </summary>
        Predefined,

        /// <summary>
        ///     NVIDIA PreDefined timing
        /// </summary>
        PSF = Predefined,

        /// <summary>
        ///     ASPR timing
        /// </summary>
        ASPR,

        /// <summary>
        ///     Override for SDI timing
        /// </summary>
        SDI,

        /// <summary>
        ///     Not used
        /// </summary>
        Max
    }

    /// <summary>
    ///     Timing scan modes
    /// </summary>
    public enum TimingScanMode : ushort
    {
        /// <summary>
        ///     Progressive scan mode
        /// </summary>
        Progressive = 0,

        /// <summary>
        ///     Interlaced scan mode
        /// </summary>
        Interlaced = 1,

        /// <summary>
        ///     Interlaced scan mode with extra vertical blank
        /// </summary>
        InterlacedWithExtraVerticalBlank = 1,

        /// <summary>
        ///     Interlaced scan mode without extra vertical blank
        /// </summary>
        InterlacedWithNoExtraVerticalBlank = 2
    }

    /// <summary>
    ///     Vertical synchronized polarity modes
    /// </summary>
    public enum TimingVerticalSyncPolarity : byte
    {
        /// <summary>
        ///     Positive vertical synchronized polarity
        /// </summary>
        Positive = 0,

        /// <summary>
        ///     Negative vertical synchronized polarity
        /// </summary>
        Negative = 1,

        /// <summary>
        ///     Default vertical synchronized polarity
        /// </summary>
        Default = Positive
    }

    /// <summary>
    ///     Holds a list of possible warping vertex formats
    /// </summary>
    public enum WarpingVerticeFormat : uint
    {
        /// <summary>
        ///     XYUVRQ Triangle Strip vertex format
        /// </summary>
        TriangleStripXYUVRQ = 0,

        /// <summary>
        ///     XYUVRQ Triangles format
        /// </summary>
        TrianglesXYUVRQ = 1
    }


}
