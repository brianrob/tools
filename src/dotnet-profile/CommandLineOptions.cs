using System;
using CommandLine;

namespace Microsoft.DotNet.Tools.Profile
{
    [Verb("set-config", HelpText = "Set the profiling configuration for new .NET processes.")]
    internal sealed class SetConfigOptions
    {
        [Option("file", HelpText = "The path to the trace file.", Required = true)]
        public string FilePath { get; set; }

        [Option("providers", HelpText = "The provider configuration string.")]
        public string ProviderConfig { get; set; }

        [Option("circularmb", HelpText = "The size of the circular buffer in megabytes.")]
        public uint CircularMB { get; set; }

        [Option("rundown", Default = true, HelpText = "Whether or not rundown should be enabled.")]
        public bool EnableRundown { get; set; }
    }

    [Verb("clear-config", HelpText = "Clear the profiling configuration for new .NET processes.")]
    internal sealed class ClearConfigOptions
    { }

    [Verb("start", HelpText = "Start profiling.")]
    internal sealed class StartConfigOptions
    { }

    [Verb("stop", HelpText = "Stop profiling.")]
    internal sealed class StopConfigOptions
    { }
}
