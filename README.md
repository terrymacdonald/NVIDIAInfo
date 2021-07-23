# NVIDIAInfo

NVIDIAInfo is a test programme designed to exercise the CCD library that I developed for DisplayMagician. This little programme helps me validate that the library is working properly, and that it will work when added to the main DisplayMagician code.

This codebase is unlikely to be supported once DisplayMagician is working, but feel free to fork if you would like. Also feel free to send in suggestions for fixes to the C# NVIDIA library interface. Any help is appreciated!

NVIDIAInfo works using the NVIDIA API and the Windows Display CCD interface to configure your display settings for you. You can set up your display settings exactly how you like them using NVIDIA Setup and Windows Display Setup, and then use NVIDIAInfo to save those settings to a file.

NVIDIAInfo records exactly how you setup your display settings, including screen position, resolution, HDR settings, and even which screen is your main one, and then NVIDIAInfo saves those settings to a file. 

NOTE: NVIDIAInfo doesn't handle AMD Eyefinity. Please see [AMDInfo](https://github.com/terrymacdonald/AMDInfo) for that!

You can store a unique NVIDIAInfo settings file for each of your display configurations. Then you can use NVIDIAInfo to load an apply those settings! 

Command line examples:

- Show what settings you currently are using: `NVIDIAInfo`
- Save the settings you currently are using to a file to use later: `NVIDIAInfo save my-cool-settings.cfg`
- Load the settings you saved earlier and use them now: `NVIDIAInfo load my-cool-settings.cfg`
