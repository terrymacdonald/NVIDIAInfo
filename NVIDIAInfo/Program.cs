using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using DisplayMagicianShared;
using NLog.Config;
using DisplayMagicianShared.NVIDIA;
using DisplayMagicianShared.Windows;
using System.Collections.Generic;
using System.Linq;

namespace NVIDIAInfo
{
    class Program
    {

        public struct NVIDIAINFO_DISPLAY_CONFIG
        {
            public NVIDIA_DISPLAY_CONFIG NVIDIAConfig;
            public WINDOWS_DISPLAY_CONFIG WindowsConfig;
        }

        static NVIDIAINFO_DISPLAY_CONFIG myDisplayConfig = new NVIDIAINFO_DISPLAY_CONFIG();

        static void Main(string[] args)
        {

            // Prepare NLog for logging
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            //string date = DateTime.Now.ToString("yyyyMMdd.HHmmss");
            string AppLogFilename = Path.Combine($"NVIDIAInfo.log");

            // Rules for mapping loggers to targets          
            NLog.LogLevel logLevel = NLog.LogLevel.Trace;

            // Create the log file target
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = AppLogFilename,
                DeleteOldFileOnStartup = true
            };

            // Create a logging rule to use the log file target
            var loggingRule = new LoggingRule("LogToFile");
            loggingRule.EnableLoggingForLevels(logLevel, NLog.LogLevel.Fatal);
            loggingRule.Targets.Add(logfile);
            loggingRule.LoggerNamePattern = "*";
            config.LoggingRules.Add(loggingRule);

            // Apply config           
            NLog.LogManager.Configuration = config;

            // Start the Log file
            SharedLogger.logger.Info($"NVIDIAInfo/Main: Starting NVIDIAInfo v1.0.2");


            Console.WriteLine($"\nNVIDIAInfo v1.0.2");
            Console.WriteLine($"=================");
            Console.WriteLine($"By Terry MacDonald 2021\n");

            // First check that we have an NVIDIA Video Card in this PC
            List<string> videoCardVendors = WinLibrary.GetLibrary().GetCurrentPCIVideoCardVendors();
            if (!NVIDIALibrary.GetLibrary().PCIVendorIDs.All(value => videoCardVendors.Contains(value)))
            {
                SharedLogger.logger.Error($"NVIDIAInfo/Main: There are no NVIDIA Video Cards enabled within this computer. NVIDIAInfo requires at least one NVIDIA Video Card to work. Please use DisplayMagician instead.");
                Console.WriteLine($"ERROR - There are no NVIDIA Video Cards enabled within this computer. NVIDIAInfo requires at least one NVIDIA Video Card to work.");
                Console.WriteLine($"        Please use DisplayMagician instead. See https://displaymagician.littlebitbig.com for more information.");
                Console.WriteLine();
                Environment.Exit(1);
            }

            if (args.Length > 0)
            {
                if (args[0] == "save")
                {
                    SharedLogger.logger.Debug($"NVIDIAInfo/Main: The save command was provided");
                    if (args.Length != 2)
                    {
                        Console.WriteLine($"ERROR - You need to provide a filename in which to save display settings");
                        SharedLogger.logger.Error($"NVIDIAInfo/Main: ERROR - You need to provide a filename in which to save display settings");
                        Environment.Exit(1);
                    }
                    SharedLogger.logger.Debug($"NVIDIAInfo/Main: Attempting to save the display settings to {args[1]} as save command was provided");
                    saveToFile(args[1]);
                    if (!File.Exists(args[1]))
                    {
                        Console.WriteLine($"ERROR - Couldn't save settings to the file {args[1]}");
                        SharedLogger.logger.Error($"NVIDIAInfo/Main: ERROR - Couldn't save settings to the file {args[1]}");
                        Environment.Exit(1);
                    }
                }
                else if (args[0] == "load")
                {
                    SharedLogger.logger.Debug($"NVIDIAInfo/Main: The load command was provided");
                    if (args.Length != 2)
                    {
                        Console.WriteLine($"ERROR - You need to provide a filename from which to load display settings");
                        SharedLogger.logger.Error($"NVIDIAInfo/Main: ERROR - You need to provide a filename from which to load display settings");
                        Environment.Exit(1);
                    }
                    SharedLogger.logger.Debug($"NVIDIAInfo/Main: Attempting to use the display settings in {args[1]} as load command was provided");
                    if (!File.Exists(args[1]))
                    {
                        Console.WriteLine($"ERROR - Couldn't find the file {args[1]} to load settings from it");
                        SharedLogger.logger.Error($"NVIDIAInfo/Main: ERROR - Couldn't find the file {args[1]} to load settings from it");
                        Environment.Exit(1);
                    }
                    loadFromFile(args[1]);
                }
                else if (args[0] == "possible")
                {
                    SharedLogger.logger.Debug($"NVIDIAInfo/Main: The possible command was provided");
                    if (args.Length != 2)
                    {
                        Console.WriteLine($"ERROR - You need to provide a filename from which we will check if the display settings are possible");
                        SharedLogger.logger.Error($"NVIDIAInfo/Main: ERROR - You need to provide a filename from which we will check if the display settings are possible");
                        Environment.Exit(1);
                    }
                    SharedLogger.logger.Debug($"NVIDIAInfo/Main: showing if the {args[1]} is a valid display cofig file as possible command was provided");
                    if (!File.Exists(args[1]))
                    {
                        Console.WriteLine($"ERROR - Couldn't find the file {args[1]} to check the settings from it");
                        SharedLogger.logger.Error($"NVIDIAInfo/Main: ERROR - Couldn't find the file {args[1]} to check the settings from it");
                        Environment.Exit(1);
                    }
                    possibleFromFile(args[1]);
                }
                else if (args[0] == "equal")
                {
                    SharedLogger.logger.Debug($"NVIDIAInfo/Main: The equal command was provided");
                    if (args.Length != 3)
                    {
                        Console.WriteLine($"ERROR - You need to provide two filenames in order for us to see if they are equal.");
                        Console.WriteLine($"        Equal means they are exactly the same.");
                        SharedLogger.logger.Error($"NVIDIAInfo/Main: ERROR - You need to provide two filenames in order for us to see if they are equal.");
                        Environment.Exit(1);
                    }
                    SharedLogger.logger.Debug($"NVIDIAInfo/Main: showing if {args[1]} and {args[2]} are both a valid display config files as equals command was provided");
                    if (!File.Exists(args[1]))
                    {
                        Console.WriteLine($"ERROR - Couldn't find the file {args[1]} to check the settings from it");
                        SharedLogger.logger.Error($"NVIDIAInfo/Main: ERROR - Couldn't find the file {args[1]} to check the settings from it");
                        Environment.Exit(1);
                    }
                    if (!File.Exists(args[2]))
                    {
                        Console.WriteLine($"ERROR - Couldn't find the file {args[2]} to check the settings from it");
                        SharedLogger.logger.Error($"NVIDIAInfo/Main: ERROR - Couldn't find the file {args[2]} to check the settings from it");
                        Environment.Exit(1);
                    }
                    equalFromFiles(args[1],args[2]);
                }
                else if (args[0] == "currentids")
                {
                    SharedLogger.logger.Debug($"NVIDIAInfo/Main: showing currently connected display ids as currentids command was provided");
                    Console.WriteLine("The current display identifiers are:");
                    SharedLogger.logger.Info($"NVIDIAInfo/Main: The current display identifiers are:");
                    foreach (string displayId in NVIDIALibrary.GetLibrary().GetCurrentDisplayIdentifiers())
                    {
                        Console.WriteLine(@displayId);
                        SharedLogger.logger.Info($@"{displayId}");
                    }
                }
                else if (args[0] == "allids")
                {
                    SharedLogger.logger.Debug($"NVIDIAInfo/Main: showing all display ids as allids command was provided");
                    Console.WriteLine("All connected display identifiers are:");
                    SharedLogger.logger.Info($"NVIDIAInfo/Main: All connected display identifiers are:");
                    foreach (string displayId in NVIDIALibrary.GetLibrary().GetAllConnectedDisplayIdentifiers())
                    {
                        Console.WriteLine(@displayId);
                        SharedLogger.logger.Info($@"{displayId}");
                    }
                }
                else if (args[0] == "print")
                {
                    SharedLogger.logger.Debug($"NVIDIAInfo/Main: printing display info as print command was provided");
                    Console.WriteLine(NVIDIALibrary.GetLibrary().PrintActiveConfig());                    
                }

                else if (args[0] == "help" || args[0] == "--help" || args[0] == "-h" || args[0] == "/?" || args[0] == "-?")
                {
                    SharedLogger.logger.Debug($"NVIDIAInfo/Main: Showing help as help command was provided");
                    showHelp();
                    Environment.Exit(1);
                }
                else
                {
                    SharedLogger.logger.Debug($"NVIDIAInfo/Main: Showing help as an invalid command was provided");
                    showHelp();
                    Console.WriteLine("*** ERROR - Invalid command line parameter provided! ***\n");
                    Environment.Exit(1);
                }
            }
            else
            {
                SharedLogger.logger.Debug($"NVIDIAInfo/Main: Showing help as help command was provided");
                showHelp();
                Environment.Exit(1);                
            }
            Console.WriteLine();
            Environment.Exit(0);
        }

        static void showHelp()
        {
            Console.WriteLine($"NVIDIAInfo is a little program to help test setting display layout and HDR settings in Windows 10 64-bit and later.\n");
            Console.WriteLine($"You need to have the latest NVIDIA Driver installed and an NVIDIA video card in order to run this software.\n");
            Console.WriteLine($"You can run it without any command line parameters, and it will print all the information it can find from the \nNVIDIA driver and the Windows Display CCD interface.\n");
            Console.WriteLine($"You can also run it with 'NVIDIAInfo save myfilename.cfg' and it will save the current display configuration into\nthe myfilename.cfg file.\n");
            Console.WriteLine($"This is most useful when you subsequently use the 'NVIDIAInfo load myfilename.cfg' command, as it will load the\ndisplay configuration from the myfilename.cfg file and make it live. In this way, you can make yourself a library\nof different cfg files with different display layouts, then use the NVIDIAInfo load command to swap between them.\n\n");
            Console.WriteLine($"Valid commands:\n");
            Console.WriteLine($"\t'NVIDIAInfo print' will print information about your current display setting.");
            Console.WriteLine($"\t'NVIDIAInfo save myfilename.cfg' will save your current display setting to the myfilename.cfg file.");
            Console.WriteLine($"\t'NVIDIAInfo load myfilename.cfg' will load and apply the display setting in the myfilename.cfg file.");
            Console.WriteLine($"\t'NVIDIAInfo possible myfilename.cfg' will test the display setting in the myfilename.cfg file to see\n\t\tif it is possible to use that display profile now.");
            Console.WriteLine($"\nUse DisplayMagician to store display settings for each game you have. https://github.com/terrymacdonald/DisplayMagician\n");
        }

        static void saveToFile(string filename)
        {
            SharedLogger.logger.Trace($"NVIDIAInfo/saveToFile: Attempting to save the current display configuration to the {filename}.");

            SharedLogger.logger.Trace($"NVIDIAInfo/saveToFile: Getting the current Active Config");
            // Get the current configuration
            myDisplayConfig.NVIDIAConfig = NVIDIALibrary.GetLibrary().GetActiveConfig();
            myDisplayConfig.WindowsConfig = WinLibrary.GetLibrary().GetActiveConfig();

            SharedLogger.logger.Trace($"NVIDIAInfo/saveToFile: Attempting to convert the current Active Config objects to JSON format");
            // Save the object to file!
            try
            {
                SharedLogger.logger.Trace($"NVIDIAInfo/saveToFile: Attempting to convert the current Active Config objects to JSON format");

                var json = JsonConvert.SerializeObject(myDisplayConfig, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    DefaultValueHandling = DefaultValueHandling.Populate,
                    TypeNameHandling = TypeNameHandling.Auto

                });


                if (!string.IsNullOrWhiteSpace(json))
                {
                    SharedLogger.logger.Error($"NVIDIAInfo/saveToFile: Saving the display settings to {filename}.");

                    File.WriteAllText(filename, json, Encoding.Unicode);

                    SharedLogger.logger.Error($"NVIDIAInfo/saveToFile: Display settings successfully saved to {filename}.");
                    Console.WriteLine($"Display settings successfully saved to {filename}.");
                }
                else
                {
                    SharedLogger.logger.Error($"NVIDIAInfo/saveToFile: The JSON string is empty after attempting to convert the current Active Config objects to JSON format");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NVIDIAInfo/saveToFile: ERROR - Unable to save the profile repository to the {filename}.");
                SharedLogger.logger.Error(ex, $"NVIDIAInfo/saveToFile: Saving the display settings to the {filename}.");
            }
        }

        static void loadFromFile(string filename)
        {
            string json = "";
            try
            {
                SharedLogger.logger.Trace($"NVIDIAInfo/loadFromFile: Attempting to load the display configuration from {filename} to use it.");
                json = File.ReadAllText(filename, Encoding.Unicode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NVIDIAInfo/loadFromFile: ERROR - Tried to read the JSON file {filename} to memory but File.ReadAllTextthrew an exception.");
                SharedLogger.logger.Error(ex, $"NVIDIAInfo/loadFromFile: Tried to read the JSON file {filename} to memory but File.ReadAllTextthrew an exception.");
            }

            if (!string.IsNullOrWhiteSpace(json))
            {
                SharedLogger.logger.Trace($"NVIDIAInfo/loadFromFile: Contents exist within {filename} so trying to read them as JSON.");
                try
                {
                    myDisplayConfig = JsonConvert.DeserializeObject<NVIDIAINFO_DISPLAY_CONFIG>(json, new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Include,
                        TypeNameHandling = TypeNameHandling.Auto,
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    });
                    SharedLogger.logger.Trace($"NVIDIAInfo/loadFromFile: Successfully parsed {filename} as JSON.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"NVIDIAInfo/loadFromFile: ERROR - Tried to parse the JSON in the {filename} but the JsonConvert threw an exception.");
                    SharedLogger.logger.Error(ex, $"NVIDIAInfo/loadFromFile: Tried to parse the JSON in the {filename} but the JsonConvert threw an exception.");
                }

                if (!NVIDIALibrary.GetLibrary().IsActiveConfig(myDisplayConfig.NVIDIAConfig) && !WinLibrary.GetLibrary().IsActiveConfig(myDisplayConfig.WindowsConfig))
                {
                    if (NVIDIALibrary.GetLibrary().IsPossibleConfig(myDisplayConfig.NVIDIAConfig))
                    {
                        SharedLogger.logger.Trace($"NVIDIAInfo/loadFromFile: The NVIDIA display settings within {filename} are possible to use right now, so we'll use attempt to use them.");
                        Console.WriteLine($"Attempting to apply NVIDIA display config from {filename}");
                        bool itWorkedforNVIDIA = NVIDIALibrary.GetLibrary().SetActiveConfig(myDisplayConfig.NVIDIAConfig);                        
                        
                        if (itWorkedforNVIDIA) 
                        {
                            SharedLogger.logger.Trace($"NVIDIAInfo/loadFromFile: The NVIDIA display settings within {filename} were successfully applied.");
                            // Then let's try to also apply the windows changes
                            // Note: we are unable to check if the Windows CCD display config is possible, as it won't match if either the current display config is a Mosaic config,
                            // or if the display config we want to change to is a Mosaic config. So we just have to assume that it will work!
                            bool itWorkedforWindows = WinLibrary.GetLibrary().SetActiveConfig(myDisplayConfig.WindowsConfig);
                            if (itWorkedforWindows)
                            {
                                SharedLogger.logger.Trace($"NVIDIAInfo/loadFromFile: The Windows CCD display settings within {filename} were successfully applied.");
                                Console.WriteLine($"NVIDIAInfo Display config successfully applied");
                            }
                            else
                            {
                                SharedLogger.logger.Trace($"NVIDIAInfo/loadFromFile: The Windows CCD display settings within {filename} were NOT applied correctly.");
                                Console.WriteLine($"ERROR - NVIDIAInfo Windows CCD settings were not applied correctly.");
                            }
                            
                        }
                        else
                        {
                            SharedLogger.logger.Trace($"NVIDIAInfo/loadFromFile: The NVIDIA display settings within {filename} were NOT applied correctly.");
                            Console.WriteLine($"ERROR - NVIDIAInfo NVIDIA display settings were not applied correctly.");
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine($"ERROR - Cannot apply the NVIDIA display config in {filename} as it is not currently possible to use it.");
                        SharedLogger.logger.Error($"NVIDIAInfo/loadFromFile: ERROR - Cannot apply the NVIDIA display config in {filename} as it is not currently possible to use it.");
                    }
                }
                else
                {
                    Console.WriteLine($"The display settings in {filename} are already installed. No need to install them again. Exiting.");
                    SharedLogger.logger.Info($"NVIDIAInfo/loadFromFile: The display settings in {filename} are already installed. No need to install them again. Exiting.");
                }

            }
            else
            {
                Console.WriteLine($"ERROR - The {filename} profile JSON file exists but is empty! So we're going to treat it as if it didn't exist.");
                SharedLogger.logger.Error($"NVIDIAInfo/loadFromFile: The {filename} profile JSON file exists but is empty! So we're going to treat it as if it didn't exist.");
            }
        }

        static void possibleFromFile(string filename)
        {
            string json = "";
            try
            {
                SharedLogger.logger.Trace($"NVIDIAInfo/possibleFromFile: Attempting to load the display configuration from {filename} to see if it's possible.");
                json = File.ReadAllText(filename, Encoding.Unicode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NVIDIAInfo/possibleFromFile: ERROR - Tried to read the JSON file {filename} to memory but File.ReadAllTextthrew an exception.");
                SharedLogger.logger.Error(ex, $"NVIDIAInfo/possibleFromFile: Tried to read the JSON file {filename} to memory but File.ReadAllTextthrew an exception.");
            }

            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    SharedLogger.logger.Trace($"NVIDIAInfo/possibleFromFile: Contents exist within {filename} so trying to read them as JSON.");
                    myDisplayConfig = JsonConvert.DeserializeObject<NVIDIAINFO_DISPLAY_CONFIG>(json, new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Include,
                        TypeNameHandling = TypeNameHandling.Auto,
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    });
                    SharedLogger.logger.Trace($"NVIDIAInfo/possibleFromFile: Successfully parsed {filename} as JSON.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"NVIDIAInfo/possibleFromFile: ERROR - Tried to parse the JSON in the {filename} but the JsonConvert threw an exception.");
                    SharedLogger.logger.Error(ex, $"NVIDIAInfo/possibleFromFile: Tried to parse the JSON in the {filename} but the JsonConvert threw an exception.");
                }

                if (NVIDIALibrary.GetLibrary().IsPossibleConfig(myDisplayConfig.NVIDIAConfig))
                {
                    SharedLogger.logger.Trace($"NVIDIAInfo/possibleFromFile: The NVIDIA & Windows CCD display settings in {filename} are compatible with this computer.");
                    Console.WriteLine($"The NVIDIA display settings in {filename} are compatible with this computer.");
                    Console.WriteLine($"You can apply them with the command 'NVIDIAInfo load {filename}'");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIAInfo/possibleFromFile: The {filename} file contains a display setting that will NOT work on this computer right now.");
                    SharedLogger.logger.Trace($"NVIDIAInfo/possibleFromFile: This may be because the required screens are turned off, or some other change has occurred on the PC.");
                    Console.WriteLine($"The {filename} file contains a display setting that will NOT work on this computer right now.");
                    Console.WriteLine($"This may be because the required screens are turned off, or some other change has occurred on the PC.");
                }

            }
            else
            {
                SharedLogger.logger.Error($"NVIDIAInfo/possibleFromFile: The {filename} profile JSON file exists but is empty! So we're going to treat it as if it didn't exist.");
                Console.WriteLine($"NVIDIAInfo/possibleFromFile: The {filename} profile JSON file exists but is empty! So we're going to treat it as if it didn't exist.");
            }
        }

        static void equalFromFiles(string filename, string otherFilename)
        {
            string json = ""; 
            string otherJson = "";
            NVIDIAINFO_DISPLAY_CONFIG displayConfig = new NVIDIAINFO_DISPLAY_CONFIG();
            NVIDIAINFO_DISPLAY_CONFIG otherDisplayConfig = new NVIDIAINFO_DISPLAY_CONFIG();
            SharedLogger.logger.Trace($"NVIDIAInfo/equalFromFile: Attempting to compare the display configuration from {filename} and {otherFilename} to see if they are equal.");
            try
            {
                json = File.ReadAllText(filename, Encoding.Unicode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NVIDIAInfo/equalFromFile: ERROR - Tried to read the JSON file {filename} to memory but File.ReadAllTextthrew an exception.");
                SharedLogger.logger.Error(ex, $"NVIDIAInfo/equalFromFile: Tried to read the JSON file {filename} to memory but File.ReadAllTextthrew an exception.");
            }

            try
            {
                otherJson = File.ReadAllText(otherFilename, Encoding.Unicode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NVIDIAInfo/equalFromFile: ERROR - Tried to read the JSON file {otherFilename} to memory but File.ReadAllTextthrew an exception.");
                SharedLogger.logger.Error(ex, $"NVIDIAInfo/equalFromFile: Tried to read the JSON file {otherFilename} to memory but File.ReadAllTextthrew an exception.");
            }

            if (!string.IsNullOrWhiteSpace(json)&&!string.IsNullOrWhiteSpace(otherJson))
            {
                try
                {
                    SharedLogger.logger.Trace($"NVIDIAInfo/equalFromFile: Contents exist within {filename} so trying to read them as JSON.");
                    displayConfig = JsonConvert.DeserializeObject<NVIDIAINFO_DISPLAY_CONFIG>(json, new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Include,
                        TypeNameHandling = TypeNameHandling.Auto,
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    });
                    SharedLogger.logger.Trace($"NVIDIAInfo/equalFromFile: Successfully parsed {filename} as JSON.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"NVIDIAInfo/equalFromFile: ERROR - Tried to parse the JSON in the {filename} but the JsonConvert threw an exception.");
                    SharedLogger.logger.Error(ex, $"NVIDIAInfo/equalFromFile: Tried to parse the JSON in the {filename} but the JsonConvert threw an exception.");
                }
                try
                {
                    SharedLogger.logger.Trace($"NVIDIAInfo/equalFromFile: Contents exist within {otherFilename} so trying to read them as JSON.");
                    otherDisplayConfig = JsonConvert.DeserializeObject<NVIDIAINFO_DISPLAY_CONFIG>(otherJson, new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Include,
                        TypeNameHandling = TypeNameHandling.Auto,
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    });
                    SharedLogger.logger.Trace($"NVIDIAInfo/equalFromFile: Successfully parsed {filename} as JSON.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"NVIDIAInfo/equalFromFile: ERROR - Tried to parse the JSON in the {filename} but the JsonConvert threw an exception.");
                    SharedLogger.logger.Error(ex, $"NVIDIAInfo/equalFromFile: Tried to parse the JSON in the {filename} but the JsonConvert threw an exception.");
                }

                if (displayConfig.NVIDIAConfig.Equals(otherDisplayConfig.NVIDIAConfig))
                {
                    SharedLogger.logger.Trace($"NVIDIAInfo/equalFromFile: The NVIDIA display settings in {filename} and {otherFilename} are equal.");
                    Console.WriteLine($"The NVIDIA display settings in {filename} and {otherFilename} are equal.");
                }
                else
                {
                    SharedLogger.logger.Trace($"NVIDIAInfo/equalFromFile: The NVIDIA display settings in {filename} and {otherFilename} are NOT equal.");
                    Console.WriteLine($"The NVIDIA display settings in {filename} and {otherFilename} are NOT equal.");
                }

            }
            else
            {
                SharedLogger.logger.Error($"NVIDIAInfo/equalFromFile: The {filename} or {otherFilename} JSON files exist but at least one of them is empty! Cannot continue.");
                Console.WriteLine($"NVIDIAInfo/equalFromFile: The {filename} or {otherFilename} JSON files exist but at least one of them is empty! Cannot continue.");
            }
        }
        
    }
}
