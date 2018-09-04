using System;
using System.IO;
using CommandLine;

namespace Microsoft.DotNet.Tools.Profile
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Parse command line arguments and execute the desired operation.
            int retCode = CommandLine.Parser.Default.ParseArguments<SetConfigOptions, ClearConfigOptions, StartConfigOptions, StopConfigOptions>(args)
                .MapResult(
                    (SetConfigOptions options) => RunSetConfig(options),
                    (ClearConfigOptions options) => RunClearConfig(options),
                    (StartConfigOptions options) => RunStart(options),
                    (StopConfigOptions options) => RunStop(options),
                    errs => 1);

            // Determine whether we should write a status message to the console.
            if (retCode != 1)
            {
                if (retCode == 0)
                {
                    Console.WriteLine("Completed successfully.");
                }
                else if (retCode != -1)
                {
                    Console.WriteLine("An unexpected error occurred.");
                }
            }
        }

        private static int RunSetConfig(SetConfigOptions options)
        {
            // Create a new configuration object and fill it with the specified arguments.
            EventPipeConfiguration config = new EventPipeConfiguration();
            if (options.CircularMB > 0)
            {
                config.CircularMB = options.CircularMB;
            }
            if (!string.IsNullOrEmpty(options.FilePath))
            {
                config.TraceFilePath = Path.GetFullPath(options.FilePath);
            }
            if (!string.IsNullOrEmpty(options.ProviderConfig))
            {
                config.ProviderConfiguration = options.ProviderConfig;
            }
            config.EnableRundown = options.EnableRundown;

            // Clear any existing configuration.
            EventPipeConfiguration.ClearConfig();

            // Set the specified configuration.
            EventPipeConfiguration.SetConfig(config);

            Console.WriteLine("The environment has been configured successfully.  Open a new shell to continue.");

            return 0;
        }

        private static int RunClearConfig(ClearConfigOptions options)
        {
            // Clear the configuration.
            EventPipeConfiguration.ClearConfig();

            return 0;
        }

        private static int RunStart(StartConfigOptions options)
        {
            // Get the existing configuration from the environment.
            EventPipeConfiguration config = EventPipeConfiguration.FromEnvironment();

            // Make sure that tracing has been configured.
            if(config.EnableValue == 0)
            {
                Console.WriteLine("dotnet-profile set-config must be run before tracing can be started.");
                return -1;
            }

            // Get the trace file path and make sure it is valid.
            string traceFilePath = config.TraceFilePath;
            if (!Path.IsPathRooted(traceFilePath))
            {
                Console.WriteLine("dotnet-profile set-config must be run before tracing can be started.");
                return -1;
            }

            // Build the marker file path.
            string markerFilePath = traceFilePath + EventPipeConfiguration.MarkerFileExtension;
            if(File.Exists(markerFilePath))
            {
                Console.WriteLine("Tracing has already been started.");
                return -1;
            }

            // Enable tracing by creating the marker file.
            using (File.Create(markerFilePath))
            {
            }

            return 0;
        }

        private static int RunStop(StopConfigOptions options)
        {
            // Get the existing configuration from the environment.
            EventPipeConfiguration config = EventPipeConfiguration.FromEnvironment();

            // Make sure that tracing has been configured.
            if (config.EnableValue == 0)
            {
                Console.WriteLine("dotnet-profile set-config must be run before tracing can be started.");
                return -1;
            }

            // Get the trace file path and make sure it is valid.
            string traceFilePath = config.TraceFilePath;
            if (!Path.IsPathRooted(traceFilePath))
            {
                Console.WriteLine("dotnet-profile set-config must be run before tracing can be started.");
                return -1;
            }

            // Build the marker file path.
            string markerFilePath = traceFilePath + EventPipeConfiguration.MarkerFileExtension;
            if (!File.Exists(markerFilePath))
            {
                Console.WriteLine("Tracing must be started before it can be stopped.");
                return -1;
            }

            // Disable tracing by deleting the marker file.
            File.Delete(markerFilePath);

            return 0;
        }
    }
}
