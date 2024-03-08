using NvAPIWrapper.Native.General.Structures;
using NvAPIWrapper.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace DisplayMagicianShared.NVIDIA
{
    public static class NVAPI
    {

        /// <summary>
        ///     This function returns information about the system's chipset.
        /// </summary>
        /// <returns>Information about the system's chipset</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid argument</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IChipsetInfo GetChipsetInfo()
        {
            var getChipSetInfo = DelegateFactory.GetDelegate<Delegates.General.NvAPI_SYS_GetChipSetInfo>();

            foreach (var acceptType in getChipSetInfo.Accepts())
            {
                var instance = acceptType.Instantiate<IChipsetInfo>();

                using (var chipsetInfoReference = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getChipSetInfo(chipsetInfoReference);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return chipsetInfoReference.ToValueType<IChipsetInfo>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }


        /// <summary>
        ///     This API returns display driver version and driver-branch string.
        /// </summary>
        /// <param name="branchVersion">Contains the driver-branch string after successful return.</param>
        /// <returns>Returns driver version</returns>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: NVAPI not initialized</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        public static uint GetDriverAndBranchVersion(out string branchVersion)
        {
            var status = DelegateFactory.GetDelegate<Delegates.General.NvAPI_SYS_GetDriverAndBranchVersion>()(
                out var driverVersion, out var branchVersionShortString);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            branchVersion = branchVersionShortString.Value;

            return driverVersion;
        }

        /// <summary>
        ///     This function converts an NvAPI error code into a null terminated string.
        /// </summary>
        /// <param name="statusCode">The error code to convert</param>
        /// <returns>The string corresponding to the error code</returns>
        // ReSharper disable once FlagArgument
        public static string GetErrorMessage(Status statusCode)
        {
            statusCode =
                DelegateFactory.GetDelegate<Delegates.General.NvAPI_GetErrorMessage>()(statusCode, out var message);

            if (statusCode != Status.Ok)
            {
                return null;
            }

            return message.Value;
        }

        /// <summary>
        ///     This function returns a string describing the version of the NvAPI library. The contents of the string are human
        ///     readable. Do not assume a fixed format.
        /// </summary>
        /// <returns>User readable string giving NvAPI version information</returns>
        /// <exception cref="NVIDIAApiException">See NVIDIAApiException.Status for the reason of the exception.</exception>
        public static string GetInterfaceVersionString()
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.General.NvAPI_GetInterfaceVersionString>()(out var version);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return version.Value;
        }

        /// <summary>
        ///     This function returns the current lid and dock information.
        /// </summary>
        /// <returns>Current lid and dock information</returns>
        /// <exception cref="NVIDIAApiException">Status.Error: Generic error</exception>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Requested feature not supported</exception>
        /// <exception cref="NVIDIAApiException">Status.HandleInvalidated: Handle is no longer valid</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: NvAPI_Initialize() has not been called</exception>
        public static LidDockParameters GetLidAndDockInfo()
        {
            var dockInfo = typeof(LidDockParameters).Instantiate<LidDockParameters>();
            var status = DelegateFactory.GetDelegate<Delegates.General.NvAPI_SYS_GetLidAndDockInfo>()(ref dockInfo);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return dockInfo;
        }

        /// <summary>
        ///     This function initializes the NvAPI library (if not already initialized) but always increments the ref-counter.
        ///     This must be called before calling other NvAPI_ functions.
        /// </summary>
        /// <exception cref="NVIDIAApiException">Status.Error: Generic error</exception>
        /// <exception cref="NVIDIAApiException">Status.LibraryNotFound: nvapi.dll can not be loaded</exception>
        public static void Initialize()
        {
            var status = DelegateFactory.GetDelegate<Delegates.General.NvAPI_Initialize>()();

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     PRIVATE - Requests to restart the display driver
        /// </summary>
        public static void RestartDisplayDriver()
        {
            var status = DelegateFactory.GetDelegate<Delegates.General.NvAPI_RestartDisplayDriver>()();

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     Decrements the ref-counter and when it reaches ZERO, unloads NVAPI library.
        ///     This must be called in pairs with NvAPI_Initialize.
        ///     Note: By design, it is not mandatory to call NvAPI_Initialize before calling any NvAPI.
        ///     When any NvAPI is called without first calling NvAPI_Initialize, the internal ref-counter will be implicitly
        ///     incremented. In such cases, calling NvAPI_Initialize from a different thread will result in incrementing the
        ///     ref-count again and the user has to call NvAPI_Unload twice to unload the library. However, note that the implicit
        ///     increment of the ref-counter happens only once.
        ///     If the client wants unload functionality, it is recommended to always call NvAPI_Initialize and NvAPI_Unload in
        ///     pairs.
        ///     Unloading NvAPI library is not supported when the library is in a resource locked state.
        ///     Some functions in the NvAPI library initiates an operation or allocates certain resources and there are
        ///     corresponding functions available, to complete the operation or free the allocated resources. All such function
        ///     pairs are designed to prevent unloading NvAPI library.
        ///     For example, if NvAPI_Unload is called after NvAPI_XXX which locks a resource, it fails with NVAPI_ERROR.
        ///     Developers need to call the corresponding NvAPI_YYY to unlock the resources, before calling NvAPI_Unload again.
        /// </summary>
        /// <exception cref="NVIDIAApiException">Status.Error: Generic error</exception>
        /// <exception cref="NVIDIAApiException">
        ///     Status.ApiInUse: At least an API is still being called hence cannot unload NVAPI
        ///     library from process
        /// </exception>
        public static void Unload()
        {
            var status = DelegateFactory.GetDelegate<Delegates.General.NvAPI_Unload>()();

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }


        /// <summary>
        ///     This API controls the display color configurations.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="colorData">The structure to be filled with information requested or applied on the display.</param>
        public static void ColorControl<TColorData>(uint displayId, ref TColorData colorData)
            where TColorData : struct, IColorData
        {
            var c = colorData as IColorData;
            ColorControl(displayId, ref c);
            colorData = (TColorData)c;
        }

        /// <summary>
        ///     This API controls the display color configurations.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="colorData">The structure to be filled with information requested or applied on the display.</param>
        public static void ColorControl(uint displayId, ref IColorData colorData)
        {
            var colorControl = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_Disp_ColorControl>();
            var type = colorData.GetType();

            if (!colorControl.Accepts().Contains(type))
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            using (var colorDataReference = ValueTypeReference.FromValueType(colorData, type))
            {
                var status = colorControl(displayId, colorDataReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                colorData = colorDataReference.ToValueType<IColorData>(type);
            }
        }

        /// <summary>
        ///     This API controls the display color configurations.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="colorDatas">The list of structures to be filled with information requested or applied on the display.</param>
        /// <returns>The structure that succeed in requesting information or used for applying configuration on the display.</returns>
        // ReSharper disable once IdentifierTypo
        public static IColorData ColorControl(uint displayId, IColorData[] colorDatas)
        {
            foreach (var colorData in colorDatas)
            {
                try
                {
                    var c = colorData;
                    ColorControl(displayId, ref c);

                    return c;
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    throw;
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This function converts the unattached display handle to an active attached display handle.
        ///     At least one GPU must be present in the system and running an NVIDIA display driver.
        /// </summary>
        /// <param name="display">An unattached display handle to convert.</param>
        /// <returns>Display handle of newly created display.</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid UnAttachedDisplayHandle handle.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static DisplayHandle CreateDisplayFromUnAttachedDisplay(UnAttachedDisplayHandle display)
        {
            var createDisplayFromUnAttachedDisplay =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_CreateDisplayFromUnAttachedDisplay>();
            var status = createDisplayFromUnAttachedDisplay(display, out var newDisplay);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return newDisplay;
        }

        /// <summary>
        ///     This function deletes the custom display configuration, specified from the registry for all the displays whose
        ///     display IDs are passed.
        /// </summary>
        /// <param name="displayIds">Array of display IDs on which custom display configuration should be removed.</param>
        /// <param name="customDisplay">The custom display to remove.</param>
        public static void DeleteCustomDisplay(uint[] displayIds, CustomDisplay customDisplay)
        {
            if (displayIds.Length == 0)
            {
                return;
            }

            using (var displayIdsReference = ValueTypeArray.FromArray(displayIds))
            {
                using (var customDisplayReference = ValueTypeReference.FromValueType(customDisplay))
                {
                    var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_DeleteCustomDisplay>()(
                        displayIdsReference,
                        (uint)displayIds.Length,
                        customDisplayReference
                    );

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }
                }
            }
        }

        /// <summary>
        ///     This API enumerates the custom timing specified by the enum index.
        /// </summary>
        /// <param name="displayId">The display id of the display.</param>
        /// <returns>A list of <see cref="CustomDisplay" /></returns>
        public static IEnumerable<CustomDisplay> EnumCustomDisplays(uint displayId)
        {
            var instance = typeof(CustomDisplay).Instantiate<CustomDisplay>();

            using (var customDisplayReference = ValueTypeReference.FromValueType(instance))
            {
                for (uint i = 0; i < uint.MaxValue; i++)
                {
                    var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_EnumCustomDisplay>()(
                        displayId,
                        i,
                        customDisplayReference
                    );

                    if (status == Status.EndEnumeration)
                    {
                        yield break;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    yield return customDisplayReference.ToValueType<CustomDisplay>().GetValueOrDefault();
                }
            }
        }

        /// <summary>
        ///     This function returns the handle of all NVIDIA displays
        ///     Note: Display handles can get invalidated on a mode-set, so the calling applications need to re-enum the handles
        ///     after every mode-set.
        /// </summary>
        /// <returns>Array of display handles.</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device found in the system</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static DisplayHandle[] EnumNvidiaDisplayHandle()
        {
            var enumNvidiaDisplayHandle =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_EnumNvidiaDisplayHandle>();
            var results = new List<DisplayHandle>();
            uint i = 0;

            while (true)
            {
                var status = enumNvidiaDisplayHandle(i, out var displayHandle);

                if (status == Status.EndEnumeration)
                {
                    break;
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                results.Add(displayHandle);
                i++;
            }

            return results.ToArray();
        }

        /// <summary>
        ///     This function returns the handle of all unattached NVIDIA displays
        ///     Note: Display handles can get invalidated on a mode-set, so the calling applications need to re-enum the handles
        ///     after every mode-set.
        /// </summary>
        /// <returns>Array of unattached display handles.</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device found in the system</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static UnAttachedDisplayHandle[] EnumNvidiaUnAttachedDisplayHandle()
        {
            var enumNvidiaUnAttachedDisplayHandle =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_EnumNvidiaUnAttachedDisplayHandle>();
            var results = new List<UnAttachedDisplayHandle>();
            uint i = 0;

            while (true)
            {
                var status = enumNvidiaUnAttachedDisplayHandle(i, out var displayHandle);

                if (status == Status.EndEnumeration)
                {
                    break;
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                results.Add(displayHandle);
                i++;
            }

            return results.ToArray();
        }

        /// <summary>
        ///     This function gets the active outputId associated with the display handle.
        /// </summary>
        /// <param name="display">
        ///     NVIDIA Display selection. It can be DisplayHandle.DefaultHandle or a handle enumerated from
        ///     DisplayApi.EnumNVidiaDisplayHandle().
        /// </param>
        /// <returns>
        ///     The active display output ID associated with the selected display handle hNvDisplay. The output id will have
        ///     only one bit set. In the case of Clone or Span mode, this will indicate the display outputId of the primary display
        ///     that the GPU is driving.
        /// </returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="NVIDIAApiException">Status.ExpectedDisplayHandle: display is not a valid display handle.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static OutputId GetAssociatedDisplayOutputId(DisplayHandle display)
        {
            var getAssociatedDisplayOutputId =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetAssociatedDisplayOutputId>();
            var status = getAssociatedDisplayOutputId(display, out var outputId);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return outputId;
        }

        /// <summary>
        ///     This function returns the handle of the NVIDIA display that is associated with the given display "name" (such as
        ///     "\\.\DISPLAY1").
        /// </summary>
        /// <param name="name">Display name</param>
        /// <returns>Display handle of associated display</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Display name is null.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device maps to that display name.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static DisplayHandle GetAssociatedNvidiaDisplayHandle(string name)
        {
            var getAssociatedNvidiaDisplayHandle =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetAssociatedNvidiaDisplayHandle>();
            var status = getAssociatedNvidiaDisplayHandle(name, out var display);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return display;
        }

        /// <summary>
        ///     For a given NVIDIA display handle, this function returns a string (such as "\\.\DISPLAY1") to identify the display.
        /// </summary>
        /// <param name="display">Handle of the associated display</param>
        /// <returns>Name of the display</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Display handle is null.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device maps to that display name.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static string GetAssociatedNvidiaDisplayName(DisplayHandle display)
        {
            var getAssociatedNvidiaDisplayName =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetAssociatedNvidiaDisplayName>();
            var status = getAssociatedNvidiaDisplayName(display, out var displayName);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return displayName.Value;
        }

        /// <summary>
        ///     This function returns the handle of an unattached NVIDIA display that is associated with the given display "name"
        ///     (such as "\\DISPLAY1").
        /// </summary>
        /// <param name="name">Display name</param>
        /// <returns>Display handle of associated unattached display</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Display name is null.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device maps to that display name.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static UnAttachedDisplayHandle GetAssociatedUnAttachedNvidiaDisplayHandle(string name)
        {
            var getAssociatedUnAttachedNvidiaDisplayHandle =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetAssociatedUnAttachedNvidiaDisplayHandle>();
            var status = getAssociatedUnAttachedNvidiaDisplayHandle(name, out var display);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return display;
        }

        /// <summary>
        ///     This API lets caller retrieve the current global display configuration.
        ///     Note: User should dispose all returned PathInfo objects
        /// </summary>
        /// <returns>Array of path information</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid input parameter.</exception>
        /// <exception cref="NVIDIAApiException">Status.DeviceBusy: ModeSet has not yet completed. Please wait and call it again.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IPathInfo[] GetDisplayConfig()
        {
            var getDisplayConfig = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetDisplayConfig>();
            uint allAvailable = 0;
            var status = getDisplayConfig(ref allAvailable, ValueTypeArray.Null);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            if (allAvailable == 0)
            {
                return new IPathInfo[0];
            }

            foreach (var acceptType in getDisplayConfig.Accepts())
            {
                var count = allAvailable;
                var instances = acceptType.Instantiate<IPathInfo>().Repeat((int)allAvailable);

                using (var pathInfos = ValueTypeArray.FromArray(instances, acceptType))
                {
                    status = getDisplayConfig(ref count, pathInfos);

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    instances = pathInfos.ToArray<IPathInfo>((int)count, acceptType);
                }

                if (instances.Length <= 0)
                {
                    return new IPathInfo[0];
                }

                // After allocation, we should make sure to dispose objects
                // In this case however, the responsibility is on the user shoulders
                instances = instances.AllocateAll().ToArray();

                using (var pathInfos = ValueTypeArray.FromArray(instances, acceptType))
                {
                    status = getDisplayConfig(ref count, pathInfos);

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return pathInfos.ToArray<IPathInfo>((int)count, acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     Gets the build title of the Driver Settings Database for a display
        /// </summary>
        /// <param name="displayHandle">The display handle to get DRS build title.</param>
        /// <returns>The DRS build title.</returns>
        public static string GetDisplayDriverBuildTitle(DisplayHandle displayHandle)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDisplayDriverBuildTitle>()(displayHandle,
                    out var name);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return name.Value;
        }

        /// <summary>
        ///     This function retrieves the available driver memory footprint for the GPU associated with a display.
        /// </summary>
        /// <param name="displayHandle">Handle of the display for which the memory information of its GPU is to be extracted.</param>
        /// <returns>The memory footprint available in the driver.</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA GPU driving a display was found.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IDisplayDriverMemoryInfo GetDisplayDriverMemoryInfo(DisplayHandle displayHandle)
        {
            var getMemoryInfo = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDisplayDriverMemoryInfo>();

            foreach (var acceptType in getMemoryInfo.Accepts())
            {
                var instance = acceptType.Instantiate<IDisplayDriverMemoryInfo>();

                using (var displayDriverMemoryInfo = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getMemoryInfo(displayHandle, displayDriverMemoryInfo);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return displayDriverMemoryInfo.ToValueType<IDisplayDriverMemoryInfo>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API retrieves the Display Id of a given display by display name. The display must be active to retrieve the
        ///     displayId. In the case of clone mode or Surround gaming, the primary or top-left display will be returned.
        /// </summary>
        /// <param name="displayName">Name of display (Eg: "\\DISPLAY1" to retrieve the displayId for.</param>
        /// <returns>Display ID of the requested display.</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more args passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry-point not available</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static uint GetDisplayIdByDisplayName(string displayName)
        {
            var getDisplayIdByDisplayName =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetDisplayIdByDisplayName>();
            var status = getDisplayIdByDisplayName(displayName, out var display);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return display;
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current saturation level from the Digital Vibrance Control
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <returns>An instance of the PrivateDisplayDVCInfo structure containing requested information.</returns>
        public static PrivateDisplayDVCInfo GetDVCInfo(DisplayHandle display)
        {
            var instance = typeof(PrivateDisplayDVCInfo).Instantiate<PrivateDisplayDVCInfo>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDVCInfo>()(
                    display,
                    OutputId.Invalid,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayDVCInfo>().GetValueOrDefault();
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current saturation level from the Digital Vibrance Control
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <returns>An instance of the PrivateDisplayDVCInfo structure containing requested information.</returns>
        public static PrivateDisplayDVCInfo GetDVCInfo(OutputId displayId)
        {
            var instance = typeof(PrivateDisplayDVCInfo).Instantiate<PrivateDisplayDVCInfo>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDVCInfo>()(
                    DisplayHandle.DefaultHandle,
                    displayId,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayDVCInfo>().GetValueOrDefault();
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current and the default saturation level from the Digital Vibrance Control.
        ///     The difference between this API and the 'GetDVCInfo()' includes the possibility to get the default
        ///     saturation level as well as to query under saturated configurations.
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <returns>An instance of the PrivateDisplayDVCInfoEx structure containing requested information.</returns>
        public static PrivateDisplayDVCInfoEx GetDVCInfoEx(DisplayHandle display)
        {
            var instance = typeof(PrivateDisplayDVCInfoEx).Instantiate<PrivateDisplayDVCInfoEx>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDVCInfoEx>()(
                    display,
                    OutputId.Invalid,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayDVCInfoEx>().GetValueOrDefault();
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current and the default saturation level from the Digital Vibrance Control.
        ///     The difference between this API and the 'GetDVCInfo()' includes the possibility to get the default
        ///     saturation level as well as to query under saturated configurations.
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <returns>An instance of the PrivateDisplayDVCInfoEx structure containing requested information.</returns>
        public static PrivateDisplayDVCInfoEx GetDVCInfoEx(OutputId displayId)
        {
            var instance = typeof(PrivateDisplayDVCInfoEx).Instantiate<PrivateDisplayDVCInfoEx>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetDVCInfoEx>()(
                    DisplayHandle.DefaultHandle,
                    displayId,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayDVCInfoEx>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API returns the current info-frame data on the specified device (monitor).
        /// </summary>
        /// <param name="displayHandle">The display handle of the device to retrieve HDMI support information for.</param>
        /// <param name="outputId">The target display's output id, or <see cref="OutputId.Invalid"/> to determine automatically.</param>
        /// <returns>An instance of a type implementing the <see cref="IHDMISupportInfo" /> interface.</returns>
        public static IHDMISupportInfo GetHDMISupportInfo(DisplayHandle displayHandle, OutputId outputId = OutputId.Invalid)
        {
            var getHDMISupportInfo = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetHDMISupportInfo>();

            foreach (var acceptType in getHDMISupportInfo.Accepts())
            {
                var instance = acceptType.Instantiate<IHDMISupportInfo>();

                using (var supportInfoReference = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getHDMISupportInfo(displayHandle, (uint)outputId, supportInfoReference);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return supportInfoReference.ToValueType<IHDMISupportInfo>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API returns the current info-frame data on the specified device (monitor).
        /// </summary>
        /// <param name="displayId">The display id of the device to retrieve HDMI support information for.</param>
        /// <returns>An instance of a type implementing the <see cref="IHDMISupportInfo" /> interface.</returns>
        public static IHDMISupportInfo GetHDMISupportInfo(uint displayId)
        {
            var getHDMISupportInfo = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetHDMISupportInfo>();

            foreach (var acceptType in getHDMISupportInfo.Accepts())
            {
                var instance = acceptType.Instantiate<IHDMISupportInfo>();

                using (var supportInfoReference = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getHDMISupportInfo(DisplayHandle.DefaultHandle, displayId, supportInfoReference);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return supportInfoReference.ToValueType<IHDMISupportInfo>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current default HUE angle
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <returns>An instance of the PrivateDisplayHUEInfo structure containing requested information.</returns>
        public static PrivateDisplayHUEInfo GetHUEInfo(DisplayHandle display)
        {
            var instance = typeof(PrivateDisplayHUEInfo).Instantiate<PrivateDisplayHUEInfo>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetHUEInfo>()(
                    display,
                    OutputId.Invalid,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayHUEInfo>().GetValueOrDefault();
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API returns the current and default HUE angle
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <returns>An instance of the PrivateDisplayHUEInfo structure containing requested information.</returns>
        public static PrivateDisplayHUEInfo GetHUEInfo(OutputId displayId)
        {
            var instance = typeof(PrivateDisplayHUEInfo).Instantiate<PrivateDisplayHUEInfo>();

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetHUEInfo>()(
                    DisplayHandle.DefaultHandle,
                    displayId,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return displayDVCInfoReference.ToValueType<PrivateDisplayHUEInfo>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API returns all the monitor capabilities.
        /// </summary>
        /// <param name="displayId">The target display id.</param>
        /// <param name="capabilitiesType">The type of capabilities requested.</param>
        /// <returns>An instance of <see cref="MonitorCapabilities" />.</returns>
        public static MonitorCapabilities? GetMonitorCapabilities(
            uint displayId,
            MonitorCapabilitiesType capabilitiesType)
        {
            var instance = new MonitorCapabilities(capabilitiesType);

            using (var monitorCapReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetMonitorCapabilities>()(
                    displayId,
                    monitorCapReference
                );

                if (status == Status.Error)
                {
                    return null;
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }


                instance = monitorCapReference.ToValueType<MonitorCapabilities>().GetValueOrDefault();

                if (!instance.IsValid)
                {
                    return null;
                }

                return instance;
            }
        }

        /// <summary>
        ///     This API returns all the color formats and bit depth values supported by a given display port monitor.
        /// </summary>
        /// <param name="displayId">The target display id.</param>
        /// <returns>A list of <see cref="MonitorColorData" /> instances.</returns>
        public static MonitorColorData[] GetMonitorColorCapabilities(uint displayId)
        {
            var getMonitorColorCapabilities =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetMonitorColorCapabilities>();
            var count = 0u;

            var status = getMonitorColorCapabilities(displayId, ValueTypeArray.Null, ref count);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            if (count == 0)
            {
                return new MonitorColorData[0];
            }

            var array = typeof(MonitorColorData).Instantiate<MonitorColorData>().Repeat((int)count);

            using (var monitorCapsReference = ValueTypeArray.FromArray(array))
            {
                status = getMonitorColorCapabilities(displayId, monitorCapsReference, ref count);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return monitorCapsReference.ToArray<MonitorColorData>((int)count);

            }
        }

        /// <summary>
        ///     This API returns the Display ID of the GDI Primary.
        /// </summary>
        /// <returns>Display ID of the GDI Primary.</returns>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: GDI Primary not on an NVIDIA GPU.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry-point not available</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static uint GetGDIPrimaryDisplayId()
        {
            var getGDIPrimaryDisplay =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetGDIPrimaryDisplayId>();
            var status = getGDIPrimaryDisplay(out var displayId);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return displayId;
        }

        /// <summary>
        ///     This API gets High Dynamic Range (HDR) capabilities of the display.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="driverExpandDefaultHDRParameters">
        ///     A boolean value indicating if the EDID HDR parameters should be expanded (true) or the actual current HDR
        ///     parameters should be reported (false).
        /// </param>
        /// <returns>HDR capabilities of the display</returns>
        public static HDRCapabilitiesV1 GetHDRCapabilities(uint displayId, bool driverExpandDefaultHDRParameters)
        {
            var hdrCapabilities = new HDRCapabilitiesV1(driverExpandDefaultHDRParameters);

            using (var hdrCapabilitiesReference = ValueTypeReference.FromValueType(hdrCapabilities))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_Disp_GetHdrCapabilities>()(
                    displayId,
                    hdrCapabilitiesReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return hdrCapabilitiesReference.ToValueType<HDRCapabilitiesV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API queries current state of one of the various scan-out composition parameters on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <param name="parameter">Scan-out composition parameter to by queried.</param>
        /// <param name="container">Additional container containing the returning data associated with the specified parameter.</param>
        /// <returns>Scan-out composition parameter value.</returns>
        public static ScanOutCompositionParameterValue GetScanOutCompositionParameter(
            uint displayId,
            ScanOutCompositionParameter parameter,
            out float container)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutCompositionParameter>()(
                displayId,
                parameter,
                out var parameterValue,
                out container
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return parameterValue;
        }

        /// <summary>
        ///     This API queries the desktop and scan-out portion of the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <returns>Desktop area to displayId mapping information.</returns>
        public static ScanOutInformationV1 GetScanOutConfiguration(uint displayId)
        {
            var instance = typeof(ScanOutInformationV1).Instantiate<ScanOutInformationV1>();

            using (var scanOutInformationReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutConfigurationEx>()(
                    displayId,
                    scanOutInformationReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return scanOutInformationReference.ToValueType<ScanOutInformationV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API queries the desktop and scan-out portion of the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <param name="desktopRectangle">Desktop area of the display in desktop coordinates.</param>
        /// <param name="scanOutRectangle">Scan-out area of the display relative to desktopRect.</param>
        public static void GetScanOutConfiguration(
            uint displayId,
            out Rectangle desktopRectangle,
            out Rectangle scanOutRectangle)
        {
            var instance1 = typeof(Rectangle).Instantiate<Rectangle>();
            var instance2 = typeof(Rectangle).Instantiate<Rectangle>();

            using (var desktopRectangleReference = ValueTypeReference.FromValueType(instance1))
            {
                using (var scanOutRectangleReference = ValueTypeReference.FromValueType(instance2))
                {
                    var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutConfiguration>()(
                        displayId,
                        desktopRectangleReference,
                        scanOutRectangleReference
                    );

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    desktopRectangle = desktopRectangleReference.ToValueType<Rectangle>().GetValueOrDefault();
                    scanOutRectangle = scanOutRectangleReference.ToValueType<Rectangle>().GetValueOrDefault();
                }
            }
        }

        /// <summary>
        ///     This API queries current state of the intensity feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <returns>Intensity state data.</returns>
        public static ScanOutIntensityStateV1 GetScanOutIntensityState(uint displayId)
        {
            var instance = typeof(ScanOutIntensityStateV1).Instantiate<ScanOutIntensityStateV1>();

            using (var scanOutIntensityReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutIntensityState>()(
                    displayId,
                    scanOutIntensityReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return scanOutIntensityReference.ToValueType<ScanOutIntensityStateV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API queries current state of the warping feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to query the configuration.</param>
        /// <returns>The warping state data.</returns>
        public static ScanOutWarpingStateV1 GetScanOutWarpingState(uint displayId)
        {
            var instance = typeof(ScanOutWarpingStateV1).Instantiate<ScanOutWarpingStateV1>();

            using (var scanOutWarpingReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_GetScanOutWarpingState>()(
                    displayId,
                    scanOutWarpingReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return scanOutWarpingReference.ToValueType<ScanOutWarpingStateV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API lets caller enumerate all the supported NVIDIA display views - nView and DualView modes.
        /// </summary>
        /// <param name="display">
        ///     NVIDIA Display selection. It can be DisplayHandle.DefaultHandle or a handle enumerated from
        ///     DisplayApi.EnumNVidiaDisplayHandle().
        /// </param>
        /// <returns>Array of supported views.</returns>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid input parameter.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static TargetViewMode[] GetSupportedViews(DisplayHandle display)
        {
            var getSupportedViews = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetSupportedViews>();
            uint allAvailable = 0;
            var status = getSupportedViews(display, ValueTypeArray.Null, ref allAvailable);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            if (allAvailable == 0)
            {
                return new TargetViewMode[0];
            }

            if (!getSupportedViews.Accepts().Contains(typeof(TargetViewMode)))
            {
                throw new NVIDIANotSupportedException("This operation is not supported.");
            }

            using (
                var viewModes =
                    ValueTypeArray.FromArray(TargetViewMode.Standard.Repeat((int)allAvailable).Cast<object>(),
                        typeof(TargetViewMode).GetEnumUnderlyingType()))
            {
                status = getSupportedViews(display, viewModes, ref allAvailable);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return viewModes.ToArray<TargetViewMode>((int)allAvailable,
                    typeof(TargetViewMode).GetEnumUnderlyingType());
            }
        }

        /// <summary>
        ///     This function calculates the timing from the visible width/height/refresh-rate and timing type info.
        /// </summary>
        /// <param name="displayId">Display ID of the display.</param>
        /// <param name="timingInput">Inputs used for calculating the timing.</param>
        /// <returns>An instance of the <see cref="Timing" /> structure.</returns>
        public static Timing GetTiming(uint displayId, TimingInput timingInput)
        {
            var instance = typeof(Timing).Instantiate<Timing>();

            using (var timingInputReference = ValueTypeReference.FromValueType(timingInput))
            {
                using (var timingReference = ValueTypeReference.FromValueType(instance))
                {
                    var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_GetTiming>()(
                        displayId,
                        timingInputReference,
                        timingReference
                    );

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return timingReference.ToValueType<Timing>().GetValueOrDefault();
                }
            }
        }

        /// <summary>
        ///     This function returns the display name given, for example, "\\DISPLAY1", using the unattached NVIDIA display handle
        /// </summary>
        /// <param name="display">Handle of the associated unattached display</param>
        /// <returns>Name of the display</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Display handle is null.</exception>
        /// <exception cref="NVIDIAApiException">Status.NvidiaDeviceNotFound: No NVIDIA device maps to that display name.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static string GetUnAttachedAssociatedDisplayName(UnAttachedDisplayHandle display)
        {
            var getUnAttachedAssociatedDisplayName =
                DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GetUnAttachedAssociatedDisplayName>();
            var status = getUnAttachedAssociatedDisplayName(display, out var displayName);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return displayName.Value;
        }

        /// <summary>
        ///     This API controls the InfoFrame values.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="infoFrameData">The structure to be filled with information requested or applied on the display.</param>
        public static void InfoFrameControl(uint displayId, ref InfoFrameData infoFrameData)
        {
            var infoFrameControl = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_Disp_InfoFrameControl>();

            using (var infoFrameDataReference = ValueTypeReference.FromValueType(infoFrameData))
            {
                var status = infoFrameControl(displayId, infoFrameDataReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                infoFrameData = infoFrameDataReference.ToValueType<InfoFrameData>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API is used to restore the display configuration, that was changed by calling <see cref="TryCustomDisplay" />.
        ///     This function must be called only after a custom display configuration is tested on the hardware, using
        ///     <see cref="TryCustomDisplay" />, otherwise no action is taken.
        ///     On Vista, <see cref="RevertCustomDisplayTrial" /> should be called with an active display that was affected during
        ///     the <see cref="TryCustomDisplay" /> call, per GPU.
        /// </summary>
        /// <param name="displayIds">Array of display ids on which custom display configuration is to be reverted.</param>
        public static void RevertCustomDisplayTrial(uint[] displayIds)
        {
            if (displayIds.Length == 0)
            {
                return;
            }

            using (var displayIdsReference = ValueTypeArray.FromArray(displayIds))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_RevertCustomDisplayTrial>()(
                    displayIdsReference,
                    (uint)displayIds.Length
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     This API configures High Dynamic Range (HDR) and Extended Dynamic Range (EDR) output.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="hdrColorData">The structure to be filled with information requested or applied on the display.</param>
        public static void HDRColorControl<THDRColorData>(uint displayId, ref THDRColorData hdrColorData)
            where THDRColorData : struct, IHDRColorData
        {
            var c = hdrColorData as IHDRColorData;
            HDRColorControl(displayId, ref c);
            hdrColorData = (THDRColorData)c;
        }

        /// <summary>
        ///     This API configures High Dynamic Range (HDR) and Extended Dynamic Range (EDR) output.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="hdrColorData">The structure to be filled with information requested or applied on the display.</param>
        public static void HDRColorControl(uint displayId, ref IHDRColorData hdrColorData)
        {
            var hdrColorControl = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_Disp_HdrColorControl>();
            var type = hdrColorData.GetType();

            if (!hdrColorControl.Accepts().Contains(type))
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            using (var hdrColorDataReference = ValueTypeReference.FromValueType(hdrColorData, type))
            {
                var status = hdrColorControl(displayId, hdrColorDataReference);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                hdrColorData = hdrColorDataReference.ToValueType<IHDRColorData>(type);
            }
        }

        /// <summary>
        ///     This function saves the current hardware display configuration on the specified Display IDs as a custom display
        ///     configuration.
        ///     This function should be called right after <see cref="TryCustomDisplay" /> to save the custom display from the
        ///     current hardware context.
        ///     This function will not do anything if the custom display configuration is not tested on the hardware.
        /// </summary>
        /// <param name="displayIds">Array of display ids on which custom display configuration is to be saved.</param>
        /// <param name="isThisOutputIdOnly">
        ///     If set, the saved custom display will only be applied on the monitor with the same
        ///     outputId.
        /// </param>
        /// <param name="isThisMonitorOnly">
        ///     If set, the saved custom display will only be applied on the monitor with the same EDID
        ///     ID or the same TV connector in case of analog TV.
        /// </param>
        public static void SaveCustomDisplay(uint[] displayIds, bool isThisOutputIdOnly, bool isThisMonitorOnly)
        {
            if (displayIds.Length == 0)
            {
                return;
            }

            using (var displayIdsReference = ValueTypeArray.FromArray(displayIds))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_SaveCustomDisplay>()(
                    displayIdsReference,
                    (uint)displayIds.Length,
                    isThisOutputIdOnly ? 1 : (uint)0,
                    isThisMonitorOnly ? 1 : (uint)0
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     This API configures High Dynamic Range (HDR) and Extended Dynamic Range (EDR) output.
        /// </summary>
        /// <param name="displayId">The targeted display id.</param>
        /// <param name="colorDatas">The list of structures to be filled with information requested or applied on the display.</param>
        /// <returns>The structure that succeed in requesting information or used for applying configuration on the display.</returns>
        // ReSharper disable once IdentifierTypo
        public static IHDRColorData HDRColorControl(uint displayId, IHDRColorData[] colorDatas)
        {
            foreach (var colorData in colorDatas)
            {
                try
                {
                    var c = colorData;
                    HDRColorControl(displayId, ref c);

                    return c;
                }
                catch (NVIDIAApiException e)
                {
                    if (e.Status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    throw;
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API lets caller apply a global display configuration across multiple GPUs.
        ///     If all sourceIds are zero, then NvAPI will pick up sourceId's based on the following criteria :
        ///     - If user provides SourceModeInfo then we are trying to assign 0th SourceId always to GDIPrimary.
        ///     This is needed since active windows always moves along with 0th sourceId.
        ///     - For rest of the paths, we are incrementally assigning the SourceId per adapter basis.
        ///     - If user doesn't provide SourceModeInfo then NVAPI just picks up some default SourceId's in incremental order.
        ///     Note : NVAPI will not intelligently choose the SourceIDs for any configs that does not need a mode-set.
        /// </summary>
        /// <param name="pathInfos">Array of path information</param>
        /// <param name="flags">Flags for applying settings</param>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: NVAPI not initialized</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: Invalid input parameter.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static void SetDisplayConfig(IPathInfo[] pathInfos, DisplayConfigFlags flags)
        {
            var setDisplayConfig = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_SetDisplayConfig>();

            if (pathInfos.Length > 0 && !setDisplayConfig.Accepts().Contains(pathInfos[0].GetType()))
            {
                throw new NVIDIANotSupportedException("This operation is not supported.");
            }

            using (var arrayReference = ValueTypeArray.FromArray(pathInfos))
            {
                var status = setDisplayConfig((uint)pathInfos.Length, arrayReference, flags);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current saturation level for the Digital Vibrance Control
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <param name="currentLevel">
        ///     The saturation level to be set.
        /// </param>
        public static void SetDVCLevel(DisplayHandle display, int currentLevel)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetDVCLevel>()(
                display,
                OutputId.Invalid,
                currentLevel
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current saturation level for the Digital Vibrance Control
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <param name="currentLevel">
        ///     The saturation level to be set.
        /// </param>
        public static void SetDVCLevel(OutputId displayId, int currentLevel)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetDVCLevel>()(
                DisplayHandle.DefaultHandle,
                displayId,
                currentLevel
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current saturation level for the Digital Vibrance Control.
        ///     The difference between this API and the 'SetDVCLevel()' includes the possibility to set under saturated
        ///     levels.
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <param name="currentLevel">
        ///     The saturation level to be set.
        /// </param>
        public static void SetDVCLevelEx(DisplayHandle display, int currentLevel)
        {
            var instance = new PrivateDisplayDVCInfoEx(currentLevel);

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetDVCLevelEx>()(
                    display,
                    OutputId.Invalid,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current saturation level for the Digital Vibrance Control.
        ///     The difference between this API and the 'SetDVCLevel()' includes the possibility to set under saturated
        ///     levels.
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <param name="currentLevel">
        ///     The saturation level to be set.
        /// </param>
        public static void SetDVCLevelEx(OutputId displayId, int currentLevel)
        {
            var instance = new PrivateDisplayDVCInfoEx(currentLevel);

            using (var displayDVCInfoReference = ValueTypeReference.FromValueType(instance))
            {
                var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetDVCLevelEx>()(
                    DisplayHandle.DefaultHandle,
                    displayId,
                    displayDVCInfoReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current HUE angle
        /// </summary>
        /// <param name="display">
        ///     The targeted display's handle.
        /// </param>
        /// <param name="currentAngle">
        ///     The HUE angle to be set.
        /// </param>
        public static void SetHUEAngle(DisplayHandle display, int currentAngle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetHUEAngle>()(
                display,
                OutputId.Invalid,
                currentAngle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// [PRIVATE]
        /// <summary>
        ///     This API sets the current HUE angle
        /// </summary>
        /// <param name="displayId">
        ///     The targeted display output id.
        /// </param>
        /// <param name="currentAngle">
        ///     The HUE angle to be set.
        /// </param>
        public static void SetHUEAngle(OutputId displayId, int currentAngle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetHUEAngle>()(
                DisplayHandle.DefaultHandle,
                displayId,
                currentAngle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This function overrides the refresh rate on the given display.
        ///     The new refresh rate can be applied right away in this API call or deferred to be applied with the next OS
        ///     mode-set.
        ///     The override is good for only one mode-set (regardless whether it's deferred or immediate).
        /// </summary>
        /// <param name="display">The display handle to override refresh rate of.</param>
        /// <param name="refreshRate">The override refresh rate.</param>
        /// <param name="isDeferred">
        ///     A boolean value indicating if the refresh rate override should be deferred to the next OS
        ///     mode-set.
        /// </param>
        public static void SetRefreshRateOverride(DisplayHandle display, float refreshRate, bool isDeferred)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetRefreshRateOverride>()(
                display,
                OutputId.Invalid,
                refreshRate,
                isDeferred ? 1u : 0
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This function overrides the refresh rate on the given output mask.
        ///     The new refresh rate can be applied right away in this API call or deferred to be applied with the next OS
        ///     mode-set.
        ///     The override is good for only one mode-set (regardless whether it's deferred or immediate).
        /// </summary>
        /// <param name="outputMask">The output(s) to override refresh rate of.</param>
        /// <param name="refreshRate">The override refresh rate.</param>
        /// <param name="isDeferred">
        ///     A boolean value indicating if the refresh rate override should be deferred to the next OS
        ///     mode-set.
        /// </param>
        public static void SetRefreshRateOverride(OutputId outputMask, float refreshRate, bool isDeferred)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_SetRefreshRateOverride>()(
                DisplayHandle.DefaultHandle,
                outputMask,
                refreshRate,
                isDeferred ? 1u : 0
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API sets various parameters that configure the scan-out composition feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to apply the intensity control.</param>
        /// <param name="parameter">The scan-out composition parameter to be set.</param>
        /// <param name="parameterValue">The value to be set for the specified parameter.</param>
        /// <param name="container">Additional container for data associated with the specified parameter.</param>
        // ReSharper disable once TooManyArguments
        public static void SetScanOutCompositionParameter(
            uint displayId,
            ScanOutCompositionParameter parameter,
            ScanOutCompositionParameterValue parameterValue,
            ref float container)
        {
            var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutCompositionParameter>()(
                displayId,
                parameter,
                parameterValue,
                ref container
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API enables and sets up per-pixel intensity feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to apply the intensity control.</param>
        /// <param name="scanOutIntensity">The intensity texture info.</param>
        /// <param name="isSticky">Indicates whether the settings will be kept over a reboot.</param>
        public static void SetScanOutIntensity(uint displayId, IScanOutIntensity scanOutIntensity, out bool isSticky)
        {
            Status status;
            int isStickyInt;

            if (scanOutIntensity == null)
            {
                status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutIntensity>()(
                    displayId,
                    ValueTypeReference.Null,
                    out isStickyInt
                );
            }
            else
            {
                using (var scanOutWarpingReference =
                    ValueTypeReference.FromValueType(scanOutIntensity, scanOutIntensity.GetType()))
                {
                    status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutIntensity>()(
                        displayId,
                        scanOutWarpingReference,
                        out isStickyInt
                    );
                }
            }

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            isSticky = isStickyInt > 0;
        }

        /// <summary>
        ///     This API enables and sets up the warping feature on the specified display.
        /// </summary>
        /// <param name="displayId">Combined physical display and GPU identifier of the display to apply the intensity control.</param>
        /// <param name="scanOutWarping">The warping data info.</param>
        /// <param name="maximumNumberOfVertices">The maximum number of vertices.</param>
        /// <param name="isSticky">Indicates whether the settings will be kept over a reboot.</param>
        // ReSharper disable once TooManyArguments
        public static void SetScanOutWarping(
            uint displayId,
            ScanOutWarpingV1? scanOutWarping,
            ref int maximumNumberOfVertices,
            out bool isSticky)
        {
            Status status;
            int isStickyInt;

            if (scanOutWarping == null || maximumNumberOfVertices == 0)
            {
                maximumNumberOfVertices = 0;
                status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutWarping>()(
                    displayId,
                    ValueTypeReference.Null,
                    ref maximumNumberOfVertices,
                    out isStickyInt
                );
            }
            else
            {
                using (var scanOutWarpingReference = ValueTypeReference.FromValueType(scanOutWarping.Value))
                {
                    status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_GPU_SetScanOutWarping>()(
                        displayId,
                        scanOutWarpingReference,
                        ref maximumNumberOfVertices,
                        out isStickyInt
                    );
                }
            }

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            isSticky = isStickyInt > 0;
        }

        /// <summary>
        ///     This API is used to set up a custom display without saving the configuration on multiple displays.
        /// </summary>
        /// <param name="displayIdCustomDisplayPairs">A list of display ids with corresponding custom display instances.</param>
        public static void TryCustomDisplay(IDictionary<uint, CustomDisplay> displayIdCustomDisplayPairs)
        {
            if (displayIdCustomDisplayPairs.Count == 0)
            {
                return;
            }

            using (var displayIdsReference = ValueTypeArray.FromArray(displayIdCustomDisplayPairs.Keys.ToArray()))
            {
                using (var customDisplaysReference =
                    ValueTypeArray.FromArray(displayIdCustomDisplayPairs.Values.ToArray()))
                {
                    var status = DelegateFactory.GetDelegate<Delegates.Display.NvAPI_DISP_TryCustomDisplay>()(
                        displayIdsReference,
                        (uint)displayIdCustomDisplayPairs.Count,
                        customDisplaysReference
                    );

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }
                }
            }
        }


            /// <summary>
        ///     This API enables or disables the current Mosaic topology based on the setting of the incoming 'enable' parameter.
        ///     An "enable" setting enables the current (previously set) Mosaic topology.
        ///     Note that when the current Mosaic topology is retrieved, it must have an isPossible value of true or an error will
        ///     occur.
        ///     A "disable" setting disables the current Mosaic topology.
        ///     The topology information will persist, even across reboots.
        ///     To re-enable the Mosaic topology, call this function again with the enable parameter set to true.
        /// </summary>
        /// <param name="enable">true to enable the current Mosaic topo, false to disable it.</param>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Mosaic is not supported with the existing hardware.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.TopologyNotPossible: The current topology is not currently possible.</exception>
        /// <exception cref="NVIDIAApiException">Status.ModeChangeFailed: There was an error changing the display mode.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        public static void EnableCurrentTopology(bool enable)
        {
            var status =
                DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_EnableCurrentTopo>()((uint)(enable ? 1 : 0));

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     Enumerates the current active grid topologies. This includes Mosaic, IG, and Panoramic topologies, as well as
        ///     single displays.
        /// </summary>
        /// <returns>The list of active grid topologies.</returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IGridTopology[] EnumDisplayGrids()
        {
            var mosaicEnumDisplayGrids =
                DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_EnumDisplayGrids>();

            var totalAvailable = 0u;
            var status = mosaicEnumDisplayGrids(ValueTypeArray.Null, ref totalAvailable);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            if (totalAvailable == 0)
            {
                return new IGridTopology[0];
            }

            foreach (var acceptType in mosaicEnumDisplayGrids.Accepts())
            {
                var counts = totalAvailable;
                var instance = acceptType.Instantiate<IGridTopology>();

                using (
                    var gridTopologiesByRef = ValueTypeArray.FromArray(instance.Repeat((int)counts).AsEnumerable()))
                {
                    status = mosaicEnumDisplayGrids(gridTopologiesByRef, ref counts);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return gridTopologiesByRef.ToArray<IGridTopology>((int)counts, acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     Determines the set of available display modes for a given grid topology.
        /// </summary>
        /// <param name="gridTopology">The grid topology to use.</param>
        /// <returns></returns>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static IDisplaySettings[] EnumDisplayModes(IGridTopology gridTopology)
        {
            var mosaicEnumDisplayModes = DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_EnumDisplayModes>();

            using (var gridTopologyByRef = ValueTypeReference.FromValueType(gridTopology, gridTopology.GetType()))
            {
                var totalAvailable = 0u;
                var status = mosaicEnumDisplayModes(gridTopologyByRef, ValueTypeArray.Null, ref totalAvailable);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                if (totalAvailable == 0)
                {
                    return new IDisplaySettings[0];
                }

                foreach (var acceptType in mosaicEnumDisplayModes.Accepts(2))
                {
                    var counts = totalAvailable;
                    var instance = acceptType.Instantiate<IDisplaySettings>();

                    using (
                        var displaySettingByRef =
                            ValueTypeArray.FromArray(instance.Repeat((int)counts).AsEnumerable()))
                    {
                        status = mosaicEnumDisplayModes(gridTopologyByRef, displaySettingByRef, ref counts);

                        if (status == Status.IncompatibleStructureVersion)
                        {
                            continue;
                        }

                        if (status != Status.Ok)
                        {
                            throw new NVIDIAApiException(status);
                        }

                        return displaySettingByRef.ToArray<IDisplaySettings>((int)counts, acceptType);
                    }
                }

                throw new NVIDIANotSupportedException("This operation is not supported.");
            }
        }

        /// <summary>
        ///     This API returns information for the current Mosaic topology.
        ///     This includes topology, display settings, and overlap values.
        ///     You can call NvAPI_Mosaic_GetTopoGroup() with the topology if you require more information.
        ///     If there isn't a current topology, then TopologyBrief.Topology will be Topology.None.
        /// </summary>
        /// <param name="topoBrief">The current Mosaic topology</param>
        /// <param name="displaySettings">The current per-display settings</param>
        /// <param name="overlapX">The pixel overlap between horizontal displays</param>
        /// <param name="overlapY">The pixel overlap between vertical displays</param>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Mosaic is not supported with the existing hardware.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        // ReSharper disable once TooManyArguments
        public static void GetCurrentTopology(
            out TopologyBrief topoBrief,
            out IDisplaySettings displaySettings,
            out int overlapX,
            out int overlapY)
        {
            var mosaicGetCurrentTopo = DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_GetCurrentTopo>();
            topoBrief = typeof(TopologyBrief).Instantiate<TopologyBrief>();

            foreach (var acceptType in mosaicGetCurrentTopo.Accepts())
            {
                displaySettings = acceptType.Instantiate<IDisplaySettings>();

                using (var displaySettingsByRef = ValueTypeReference.FromValueType(displaySettings, acceptType))
                {
                    var status = mosaicGetCurrentTopo(ref topoBrief, displaySettingsByRef, out overlapX, out overlapY);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    displaySettings = displaySettingsByRef.ToValueType<IDisplaySettings>(acceptType);

                    return;
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API returns the X and Y overlap limits required if the given Mosaic topology and display settings are to be
        ///     used.
        /// </summary>
        /// <param name="topoBrief">
        ///     The topology for getting limits This must be one of the topo briefs returned from
        ///     GetSupportedTopoInfo().
        /// </param>
        /// <param name="displaySettings">
        ///     The display settings for getting the limits. This must be one of the settings returned
        ///     from GetSupportedTopoInfo().
        /// </param>
        /// <param name="minOverlapX">X overlap minimum</param>
        /// <param name="maxOverlapX">X overlap maximum</param>
        /// <param name="minOverlapY">Y overlap minimum</param>
        /// <param name="maxOverlapY">Y overlap maximum</param>
        /// <exception cref="ArgumentException">displaySettings is of invalid type.</exception>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Mosaic is not supported with the existing hardware.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        // ReSharper disable once TooManyArguments
        public static void GetOverlapLimits(
            TopologyBrief topoBrief,
            IDisplaySettings displaySettings,
            out int minOverlapX,
            out int maxOverlapX,
            out int minOverlapY,
            out int maxOverlapY)
        {
            var mosaicGetOverlapLimits = DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_GetOverlapLimits>();

            if (!mosaicGetOverlapLimits.Accepts().Contains(displaySettings.GetType()))
            {
                throw new ArgumentException("Parameter type is not supported.", nameof(displaySettings));
            }

            using (
                var displaySettingsByRef = ValueTypeReference.FromValueType(displaySettings, displaySettings.GetType()))
            {
                var status = mosaicGetOverlapLimits(topoBrief, displaySettingsByRef, out minOverlapX, out maxOverlapX,
                    out minOverlapY, out maxOverlapY);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     This API returns information on the topologies and display resolutions supported by Mosaic mode.
        ///     NOTE: Not all topologies returned can be set immediately. Some of the topologies returned might not be valid for
        ///     one reason or another.  It could be due to mismatched or missing displays.  It could also be because the required
        ///     number of GPUs is not found.
        ///     Once you get the list of supported topologies, you can call GetTopologyGroup() with one of the Mosaic topologies if
        ///     you need more information about it.
        ///     It is possible for this function to return NVAPI_OK with no topologies listed in the return structure.  If this is
        ///     the case, it means that the current hardware DOES support Mosaic, but with the given configuration no valid
        ///     topologies were found.  This most likely means that SLI was not enabled for the hardware. Once enabled, you should
        ///     see valid topologies returned from this function.
        /// </summary>
        /// <param name="topologyType">The type of topologies the caller is interested in getting.</param>
        /// <returns>Information about what topologies and display resolutions are supported for Mosaic.</returns>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Mosaic is not supported with the existing hardware.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: TopologyType is invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry-point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        /// <exception cref="NVIDIANotSupportedException">This operation is not supported.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static ISupportedTopologiesInfo GetSupportedTopologiesInfo(TopologyType topologyType)
        {
            var mosaicGetSupportedTopoInfo =
                DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_GetSupportedTopoInfo>();

            foreach (var acceptType in mosaicGetSupportedTopoInfo.Accepts())
            {
                var instance = acceptType.Instantiate<ISupportedTopologiesInfo>();

                using (var supportedTopologiesInfoByRef = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = mosaicGetSupportedTopoInfo(supportedTopologiesInfoByRef, topologyType);

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return supportedTopologiesInfoByRef.ToValueType<ISupportedTopologiesInfo>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API returns a structure filled with the details of the specified Mosaic topology.
        ///     If the pTopoBrief passed in matches the current topology, then information in the brief and group structures will
        ///     reflect what is current. Thus the brief would have the current 'enable' status, and the group would have the
        ///     current overlap values. If there is no match, then the returned brief has an 'enable' status of FALSE (since it is
        ///     obviously not enabled), and the overlap values will be 0.
        /// </summary>
        /// <param name="topoBrief">
        ///     The topology for getting the details. This must be one of the topology briefs returned from
        ///     GetSupportedTopoInfo().
        /// </param>
        /// <returns>The topology details matching the brief</returns>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Mosaic is not supported with the existing hardware.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        public static TopologyGroup GetTopologyGroup(TopologyBrief topoBrief)
        {
            var result = typeof(TopologyGroup).Instantiate<TopologyGroup>();
            var status =
                DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_GetTopoGroup>()(topoBrief, ref result);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return result;
        }

        /// <summary>
        ///     This API sets the Mosaic topology and performs a mode switch using the given display settings.
        /// </summary>
        /// <param name="topoBrief">
        ///     The topology to set. This must be one of the topologies returned from GetSupportedTopoInfo(),
        ///     and it must have an isPossible value of true.
        /// </param>
        /// <param name="displaySettings">
        ///     The per display settings to be used in the Mosaic mode. This must be one of the settings
        ///     returned from GetSupportedTopoInfo().
        /// </param>
        /// <param name="overlapX">
        ///     The pixel overlap to use between horizontal displays (use positive a number for overlap, or a
        ///     negative number to create a gap.) If the overlap is out of bounds for what is possible given the topo and display
        ///     setting, the overlap will be clamped.
        /// </param>
        /// <param name="overlapY">
        ///     The pixel overlap to use between vertical displays (use positive a number for overlap, or a
        ///     negative number to create a gap.) If the overlap is out of bounds for what is possible given the topo and display
        ///     setting, the overlap will be clamped.
        /// </param>
        /// <param name="enable">
        ///     If true, the topology being set will also be enabled, meaning that the mode set will occur. If
        ///     false, you don't want to be in Mosaic mode right now, but want to set the current Mosaic topology so you can enable
        ///     it later with EnableCurrentTopo()
        /// </param>
        /// <exception cref="ArgumentException">displaySettings is of invalid type.</exception>
        /// <exception cref="NVIDIAApiException">Status.NotSupported: Mosaic is not supported with the existing hardware.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        // ReSharper disable once TooManyArguments
        public static void SetCurrentTopology(
            TopologyBrief topoBrief,
            IDisplaySettings displaySettings,
            int overlapX,
            int overlapY,
            bool enable)
        {
            var mosaicSetCurrentTopo = DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_SetCurrentTopo>();

            if (!mosaicSetCurrentTopo.Accepts().Contains(displaySettings.GetType()))
            {
                throw new ArgumentException("Parameter type is not supported.", nameof(displaySettings));
            }

            using (
                var displaySettingsByRef = ValueTypeReference.FromValueType(displaySettings, displaySettings.GetType()))
            {
                var status = mosaicSetCurrentTopo(topoBrief, displaySettingsByRef, overlapX, overlapY,
                    (uint)(enable ? 1 : 0));

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     Sets a new display topology, replacing any existing topologies that use the same displays.
        ///     This function will look for an SLI configuration that will allow the display topology to work.
        ///     To revert to a single display, specify that display as a 1x1 grid.
        /// </summary>
        /// <param name="gridTopologies">The topology details to set.</param>
        /// <param name="flags">One of the SetDisplayTopologyFlag flags</param>
        /// <exception cref="NVIDIAApiException">Status.TopologyNotPossible: One or more of the display grids are not valid.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoActiveSLITopology: No matching GPU topologies could be found.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        public static void SetDisplayGrids(
            IGridTopology[] gridTopologies,
            SetDisplayTopologyFlag flags = SetDisplayTopologyFlag.NoFlag)
        {
            using (var gridTopologiesByRef = ValueTypeArray.FromArray(gridTopologies.AsEnumerable()))
            {
                var status =
                    DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_SetDisplayGrids>()(gridTopologiesByRef,
                        (uint)gridTopologies.Length,
                        flags);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     Determines if a list of grid topologies is valid. It will choose an SLI configuration in the same way that
        ///     SetDisplayGrids() does.
        ///     On return, each element in the pTopoStatus array will contain any errors or warnings about each grid topology. If
        ///     any error flags are set, then the topology is not valid. If any warning flags are set, then the topology is valid,
        ///     but sub-optimal.
        ///     If the ALLOW_INVALID flag is set, then it will continue to validate the grids even if no SLI configuration will
        ///     allow all of the grids. In this case, a grid grid with no matching GPU topology will have the error flags
        ///     NO_GPU_TOPOLOGY or NOT_SUPPORTED set.
        ///     If the ALLOW_INVALID flag is not set and no matching SLI configuration is found, then it will skip the rest of the
        ///     validation and throws a NVIDIAApiException with Status.NoActiveSLITopology.
        /// </summary>
        /// <param name="gridTopologies">The array of grid topologies to verify.</param>
        /// <param name="flags">One of the SetDisplayTopologyFlag flags</param>
        /// <exception cref="NVIDIAApiException">Status.NoActiveSLITopology: No matching GPU topologies could be found.</exception>
        /// <exception cref="NVIDIAApiException">Status.InvalidArgument: One or more arguments passed in are invalid.</exception>
        /// <exception cref="NVIDIAApiException">Status.ApiNotInitialized: The NvAPI API needs to be initialized first.</exception>
        /// <exception cref="NVIDIAApiException">Status.NoImplementation: This entry point not available.</exception>
        /// <exception cref="NVIDIAApiException">Status.Error: Miscellaneous error occurred.</exception>
        public static DisplayTopologyStatus[] ValidateDisplayGrids(
            IGridTopology[] gridTopologies,
            SetDisplayTopologyFlag flags = SetDisplayTopologyFlag.NoFlag)
        {
            using (var gridTopologiesByRef = ValueTypeArray.FromArray(gridTopologies.AsEnumerable()))
            {
                var statuses =
                    typeof(DisplayTopologyStatus).Instantiate<DisplayTopologyStatus>().Repeat(gridTopologies.Length);
                var status =
                    DelegateFactory.GetDelegate<Delegates.Mosaic.NvAPI_Mosaic_ValidateDisplayGrids>()(flags,
                        gridTopologiesByRef,
                        ref statuses, (uint)gridTopologies.Length);

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return statuses;
            }
        }




        /// <summary>
        ///     This API adds an executable name to a profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="application">Input <see cref="IDRSApplication" /> instance containing the executable name.</param>
        /// <returns>The newly created instance of <see cref="IDRSApplication" />.</returns>
        public static IDRSApplication CreateApplication(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            IDRSApplication application)
        {
            using (var applicationReference = ValueTypeReference.FromValueType(application, application.GetType()))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_CreateApplication>()(
                    sessionHandle,
                    profileHandle,
                    applicationReference
                );

                if (status == Status.IncompatibleStructureVersion)
                {
                    throw new NVIDIANotSupportedException("This operation is not supported.");
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return applicationReference.ToValueType<IDRSApplication>(application.GetType());
            }
        }

        /// <summary>
        ///     This API creates an empty profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profile">Input to the <see cref="DRSProfileV1" /> instance.</param>
        /// <returns>The newly created profile handle.</returns>
        public static DRSProfileHandle CreateProfile(DRSSessionHandle sessionHandle, DRSProfileV1 profile)
        {
            using (var profileReference = ValueTypeReference.FromValueType(profile))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_CreateProfile>()(
                    sessionHandle,
                    profileReference,
                    out var profileHandle
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return profileHandle;
            }
        }

        /// <summary>
        ///     This API allocates memory and initializes the session.
        /// </summary>
        /// <returns>The newly created session handle.</returns>
        public static DRSSessionHandle CreateSession()
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_CreateSession>()(out var sessionHandle);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return sessionHandle;
        }

        /// <summary>
        ///     This API removes an executable from a profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="application">Input all the information about the application to be removed.</param>
        public static void DeleteApplication(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            IDRSApplication application)
        {
            using (var applicationReference = ValueTypeReference.FromValueType(application, application.GetType()))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_DeleteApplicationEx>()(
                    sessionHandle,
                    profileHandle,
                    applicationReference
                );

                if (status == Status.IncompatibleStructureVersion)
                {
                    throw new NVIDIANotSupportedException("This operation is not supported.");
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     This API removes an executable name from a profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="applicationName">Input the executable name to be removed.</param>
        public static void DeleteApplication(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            string applicationName)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_DeleteApplication>()(
                sessionHandle,
                profileHandle,
                new UnicodeString(applicationName)
            );

            if (status == Status.IncompatibleStructureVersion)
            {
                throw new NVIDIANotSupportedException("This operation is not supported.");
            }

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API deletes a profile or sets it back to a predefined value.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        public static void DeleteProfile(DRSSessionHandle sessionHandle, DRSProfileHandle profileHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_DeleteProfile>()(
                sessionHandle,
                profileHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API deletes a setting or sets it back to predefined value.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="settingId">Input settingId to be deleted.</param>
        public static void DeleteProfileSetting(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            uint settingId)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_DeleteProfileSetting>()(
                sessionHandle,
                profileHandle,
                settingId
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API frees the allocated resources for the session handle.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        public static void DestroySession(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_DestroySession>()(sessionHandle);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API enumerates all the applications in a given profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <returns>Instances of <see cref="IDRSApplication" /> with all the attributes filled.</returns>
        [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
        public static IEnumerable<IDRSApplication> EnumApplications(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle)
        {
            var maxApplicationsPerRequest = 8;
            var enumApplications = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_EnumApplications>();

            foreach (var acceptType in enumApplications.Accepts())
            {
                var i = 0u;

                while (true)
                {
                    var instances = acceptType.Instantiate<IDRSApplication>().Repeat(maxApplicationsPerRequest);

                    using (var applicationsReference = ValueTypeArray.FromArray(instances, acceptType))
                    {
                        var count = (uint)instances.Length;
                        var status = enumApplications(
                            sessionHandle,
                            profileHandle,
                            i,
                            ref count,
                            applicationsReference
                        );

                        if (status == Status.IncompatibleStructureVersion)
                        {
                            break;
                        }

                        if (status == Status.EndEnumeration)
                        {
                            yield break;
                        }

                        if (status != Status.Ok)
                        {
                            throw new NVIDIAApiException(status);
                        }

                        foreach (var application in applicationsReference.ToArray<IDRSApplication>(
                            (int)count,
                            acceptType))
                        {
                            yield return application;
                            i++;
                        }

                        if (count < maxApplicationsPerRequest)
                        {
                            yield break;
                        }
                    }
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API enumerates all the Ids of all the settings recognized by NVAPI.
        /// </summary>
        /// <returns>An array of <see cref="uint" />s filled with the settings identification numbers of available settings.</returns>
        public static uint[] EnumAvailableSettingIds()
        {
            var settingIdsCount = (uint)ushort.MaxValue;
            var settingIds = 0u.Repeat((int)settingIdsCount);

            using (var settingIdsArray = ValueTypeArray.FromArray(settingIds))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_EnumAvailableSettingIds>()(
                    settingIdsArray,
                    ref settingIdsCount
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return settingIdsArray.ToArray<uint>((int)settingIdsCount);
            }
        }

        /// <summary>
        ///     This API enumerates all available setting values for a given setting.
        /// </summary>
        /// <param name="settingId">Input settingId.</param>
        /// <returns>All available setting values.</returns>
        public static DRSSettingValues EnumAvailableSettingValues(uint settingId)
        {
            var settingValuesCount = (uint)DRSSettingValues.MaximumNumberOfValues;
            var settingValues = typeof(DRSSettingValues).Instantiate<DRSSettingValues>();

            using (var settingValuesReference = ValueTypeReference.FromValueType(settingValues))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_EnumAvailableSettingValues>()(
                    settingId,
                    ref settingValuesCount,
                    settingValuesReference
                );

                if (status == Status.IncompatibleStructureVersion)
                {
                    throw new NVIDIANotSupportedException("This operation is not supported.");
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return settingValuesReference.ToValueType<DRSSettingValues>(typeof(DRSSettingValues));
            }
        }

        /// <summary>
        ///     This API enumerates through all the profiles in the session.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <returns>Instances of <see cref="DRSProfileHandle" /> each representing a profile.</returns>
        public static IEnumerable<DRSProfileHandle> EnumProfiles(DRSSessionHandle sessionHandle)
        {
            var i = 0u;

            while (true)
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_EnumProfiles>()(
                    sessionHandle,
                    i,
                    out var profileHandle
                );

                if (status == Status.EndEnumeration)
                {
                    yield break;
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                yield return profileHandle;
                i++;
            }
        }

        /// <summary>
        ///     This API enumerates all the settings of a given profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <returns>Instances of <see cref="DRSSettingV1" />.</returns>
        [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
        public static IEnumerable<DRSSettingV1> EnumSettings(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle)
        {
            var maxSettingsPerRequest = 32;
            var enumSettings = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_EnumSettings>();

            var i = 0u;

            while (true)
            {
                var instances = typeof(DRSSettingV1).Instantiate<DRSSettingV1>().Repeat(maxSettingsPerRequest);

                using (var applicationsReference = ValueTypeArray.FromArray(instances))
                {
                    var count = (uint)instances.Length;
                    var status = enumSettings(
                        sessionHandle,
                        profileHandle,
                        i,
                        ref count,
                        applicationsReference
                    );

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        throw new NVIDIANotSupportedException("This operation is not supported.");
                    }

                    if (status == Status.EndEnumeration)
                    {
                        yield break;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    foreach (var application in applicationsReference.ToArray<DRSSettingV1>(
                        (int)count,
                        typeof(DRSSettingV1))
                    )
                    {
                        yield return application;
                        i++;
                    }

                    if (count < maxSettingsPerRequest)
                    {
                        yield break;
                    }
                }
            }
        }

        /// <summary>
        ///     This API searches the application and the associated profile for the given application name.
        ///     If a fully qualified path is provided, this function will always return the profile
        ///     the driver will apply upon running the application (on the path provided).
        /// </summary>
        /// <param name="sessionHandle">Input to the hSession handle</param>
        /// <param name="applicationName">Input appName. For best results, provide a fully qualified path of the type</param>
        /// <param name="profileHandle">The profile handle of the profile that the found application belongs to.</param>
        /// <returns>An instance of <see cref="IDRSApplication" />.</returns>
        [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
        public static IDRSApplication FindApplicationByName(
            DRSSessionHandle sessionHandle,
            string applicationName,
            out DRSProfileHandle? profileHandle)
        {
            var findApplicationByName = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_FindApplicationByName>();

            foreach (var acceptType in findApplicationByName.Accepts())
            {
                var instance = acceptType.Instantiate<IDRSApplication>();

                using (var applicationReference = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = findApplicationByName(
                        sessionHandle,
                        new UnicodeString(applicationName),
                        out var applicationProfileHandle,
                        applicationReference
                    );

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status == Status.ExecutableNotFound)
                    {
                        profileHandle = null;

                        return null;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    profileHandle = applicationProfileHandle;

                    return applicationReference.ToValueType<IDRSApplication>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     This API finds a profile in the current session.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileName">Input profileName.</param>
        /// <returns>The profile handle.</returns>
        public static DRSProfileHandle FindProfileByName(DRSSessionHandle sessionHandle, string profileName)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_FindProfileByName>()(
                sessionHandle,
                new UnicodeString(profileName),
                out var profileHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return profileHandle;
        }

        /// <summary>
        ///     This API gets information about the given application.  The input application name
        ///     must match exactly what the Profile has stored for the application.
        ///     This function is better used to retrieve application information from a previous
        ///     enumeration.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="applicationName">Input application name.</param>
        /// <returns>
        ///     An instance of <see cref="IDRSApplication" /> with all attributes filled if found; otherwise
        ///     <see langword="null" />.
        /// </returns>
        [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
        public static IDRSApplication GetApplicationInfo(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            string applicationName)
        {
            var getApplicationInfo = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetApplicationInfo>();

            foreach (var acceptType in getApplicationInfo.Accepts())
            {
                var instance = acceptType.Instantiate<IDRSApplication>();

                using (var applicationReference = ValueTypeReference.FromValueType(instance, acceptType))
                {
                    var status = getApplicationInfo(
                        sessionHandle,
                        profileHandle,
                        new UnicodeString(applicationName),
                        applicationReference
                    );

                    if (status == Status.IncompatibleStructureVersion)
                    {
                        continue;
                    }

                    if (status == Status.ExecutableNotFound)
                    {
                        return null;
                    }

                    if (status != Status.Ok)
                    {
                        throw new NVIDIAApiException(status);
                    }

                    return applicationReference.ToValueType<IDRSApplication>(acceptType);
                }
            }

            throw new NVIDIANotSupportedException("This operation is not supported.");
        }

        /// <summary>
        ///     Returns the handle to the current global profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <returns>Base profile handle.</returns>
        public static DRSProfileHandle GetBaseProfile(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetBaseProfile>()(
                sessionHandle,
                out var profileHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return profileHandle;
        }

        /// <summary>
        ///     This API returns the handle to the current global profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <returns>Current global profile handle.</returns>
        public static DRSProfileHandle GetCurrentGlobalProfile(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetCurrentGlobalProfile>()(
                sessionHandle,
                out var profileHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return profileHandle;
        }

        /// <summary>
        ///     This API obtains the number of profiles in the current session object.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <returns>Number of profiles in the current session.</returns>
        public static int GetNumberOfProfiles(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetNumProfiles>()(
                sessionHandle,
                out var profileCount
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return (int)profileCount;
        }

        /// <summary>
        ///     This API gets information about the given profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <returns>An instance of <see cref="DRSProfileV1" /> with all attributes filled.</returns>
        public static DRSProfileV1 GetProfileInfo(DRSSessionHandle sessionHandle, DRSProfileHandle profileHandle)
        {
            var profile = typeof(DRSProfileV1).Instantiate<DRSProfileV1>();

            using (var profileReference = ValueTypeReference.FromValueType(profile))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetProfileInfo>()(
                    sessionHandle,
                    profileHandle,
                    profileReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return profileReference.ToValueType<DRSProfileV1>().GetValueOrDefault();
            }
        }

        /// <summary>
        ///     This API gets information about the given setting.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="settingId">Input settingId.</param>
        /// <returns>An instance of <see cref="DRSSettingV1" /> describing the setting if found; otherwise <see langword="null" />.</returns>
        public static DRSSettingV1? GetSetting(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            uint settingId)
        {
            var instance = typeof(DRSSettingV1).Instantiate<DRSSettingV1>();

            using (var settingReference = ValueTypeReference.FromValueType(instance, typeof(DRSSettingV1)))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetSetting>()(
                    sessionHandle,
                    profileHandle,
                    settingId,
                    settingReference
                );

                if (status == Status.IncompatibleStructureVersion)
                {
                    throw new NVIDIANotSupportedException("This operation is not supported.");
                }

                if (status == Status.SettingNotFound)
                {
                    return null;
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }

                return settingReference.ToValueType<DRSSettingV1>(typeof(DRSSettingV1));
            }
        }

        /// <summary>
        ///     This API gets the binary identification number of a setting given the setting name.
        /// </summary>
        /// <param name="settingName">Input Unicode settingName.</param>
        /// <returns>The corresponding settingId.</returns>
        public static uint GetSettingIdFromName(string settingName)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetSettingIdFromName>()(
                new UnicodeString(settingName),
                out var settingId
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return settingId;
        }

        /// <summary>
        ///     This API gets the setting name given the binary identification number.
        /// </summary>
        /// <param name="settingId">Input settingId.</param>
        /// <returns>Corresponding settingName.</returns>
        public static string GetSettingNameFromId(uint settingId)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_GetSettingNameFromId>()(
                settingId,
                out var settingName
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }

            return settingName.Value;
        }

        /// <summary>
        ///     This API loads and parses the settings data.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        public static void LoadSettings(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_LoadSettings>()(sessionHandle);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API loads settings from the given file path.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle</param>
        /// <param name="fileName">Binary full file path.</param>
        public static void LoadSettings(DRSSessionHandle sessionHandle, string fileName)
        {
            var unicodeFileName = new UnicodeString(fileName);
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_LoadSettingsFromFile>()(
                sessionHandle,
                unicodeFileName
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API restores the whole system to predefined(default) values.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        public static void RestoreDefaults(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_RestoreAllDefaults>()(
                sessionHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API restores the given profile to predefined(default) values.
        ///     Any and all user specified modifications will be removed.
        ///     If the whole profile was set by the user, the profile will be removed.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        public static void RestoreDefaults(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_RestoreProfileDefault>()(
                sessionHandle,
                profileHandle
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API restores the given profile setting to predefined(default) values.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="settingId">Input settingId.</param>
        public static void RestoreDefaults(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            uint settingId)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_RestoreProfileDefaultSetting>()(
                sessionHandle,
                profileHandle,
                settingId
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API saves the settings data to the system.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        public static void SaveSettings(DRSSessionHandle sessionHandle)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_SaveSettings>()(sessionHandle);

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API saves settings to the given file path.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="fileName">Binary full file path.</param>
        public static void SaveSettings(DRSSessionHandle sessionHandle, string fileName)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_SaveSettingsToFile>()(
                sessionHandle,
                new UnicodeString(fileName)
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     This API sets the current global profile in the driver.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileName">Input the new current global profile name.</param>
        public static void SetCurrentGlobalProfile(DRSSessionHandle sessionHandle, string profileName)
        {
            var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_SetCurrentGlobalProfile>()(
                sessionHandle,
                new UnicodeString(profileName)
            );

            if (status != Status.Ok)
            {
                throw new NVIDIAApiException(status);
            }
        }

        /// <summary>
        ///     Specifies flags for a given profile. Currently only the GPUSupport is
        ///     used to update the profile. Neither the name, number of settings or applications
        ///     or other profile information can be changed with this function.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="profile">Input the new profile info.</param>
        public static void SetProfileInfo(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            DRSProfileV1 profile)
        {
            using (var profileReference = ValueTypeReference.FromValueType(profile))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_SetProfileInfo>()(
                    sessionHandle,
                    profileHandle,
                    profileReference
                );

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }

        /// <summary>
        ///     This API adds/modifies a setting to a profile.
        /// </summary>
        /// <param name="sessionHandle">Input to the session handle.</param>
        /// <param name="profileHandle">Input profile handle.</param>
        /// <param name="setting">
        ///     An instance of <see cref="DRSSettingV1" /> containing the setting identification number and new
        ///     value for the setting.
        /// </param>
        public static void SetSetting(
            DRSSessionHandle sessionHandle,
            DRSProfileHandle profileHandle,
            DRSSettingV1 setting)
        {
            using (var settingReference = ValueTypeReference.FromValueType(setting, setting.GetType()))
            {
                var status = DelegateFactory.GetDelegate<Delegates.DRS.NvAPI_DRS_SetSetting>()(
                    sessionHandle,
                    profileHandle,
                    settingReference
                );

                if (status == Status.IncompatibleStructureVersion)
                {
                    throw new NVIDIANotSupportedException("This operation is not supported.");
                }

                if (status != Status.Ok)
                {
                    throw new NVIDIAApiException(status);
                }
            }
        }



    }
}
