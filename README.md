# NVIDIAInfo

NVIDIAInfo is a test programme designed to exercise the NVAPI library that I developed for DisplayMagician. This little programme helps me validate that the library is working properly, and that it will work when added to the main DisplayMagician code.

This codebase is unlikely to be supported once DisplayMagician is working, but feel free to fork if you would like. Also feel free to send in suggestions for fixes to the C# NVIDIA library interface. Any help is appreciated!

NVIDIAInfo works using the NVIDIA API and the Windows Display CCD interface to configure your display settings for you. You can set up your display settings exactly how you like them using NVIDIA Setup and Windows Display Setup, and then use NVIDIAInfo to save those settings to a file.

NVIDIAInfo records exactly how you setup your display settings, including screen position, resolution, HDR settings, and even which screen is your main one, and then NVIDIAInfo saves those settings to a file. 

NOTE: NVIDIAInfo doesn't handle AMD Eyefinity. Please see [AMDInfo](https://github.com/terrymacdonald/AMDInfo) for that!

Command line examples:

- Show what settings you currently are using: `NVIDIAInfo print`
- Save the settings you currently are using to a file to use later: `NVIDIAInfo save my-cool-settings.cfg`
- Load the settings you saved earlier and use them now: `NVIDIAInfo load my-cool-settings.cfg`
- Show whether the display config file can be used: `NVIDIAInfo possible my-cool-settings.cfg`


## To setup this software:

- Firstly, set up your display configuration using NVIDIA settings and the Windows Display settings exactly as you want to use them (e.g. one single NVIDIA Surround window using 3 screens)
- Next, save the settings you currently are using to a file to use later, using a command like `NVIDIAInfo save triple-surround-on.cfg`
- Next, change your display configuration using NVIDIA settings and the Windows Display settings to another display configuration you'd like to have (e.g. 3 single screens without using NVIDIA Surround)
- Next, save those settings to a different file to use later, using a command like `NVIDIAInfo save triple-screen.cfg`

Now that you've set up the different display configurations, you can swap between them using a command like this:

- To load the triple screen setup using NVIDIA surround: `NVIDIAInfo load triple-surround-on.cfg`
- To load the triple screen without NVIDIA surround: `NVIDIAInfo load triple-screen.cfg`

Feel free to use different config file names, and to set up what ever display configurations you like. Enjoy!