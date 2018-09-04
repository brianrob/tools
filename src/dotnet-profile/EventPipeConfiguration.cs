using System;
using System.Collections;

namespace Microsoft.DotNet.Tools.Profile
{
    internal sealed class EventPipeConfiguration
    {
        // Environment variable names.
        private static readonly string Config_Enable = "COMPlus_EnableEventPipe";
        private static readonly string Config_ProviderConfiguration = "COMPlus_EventPipeConfig";
        private static readonly string Config_Rundown = "COMPlus_EventPipeRundown";
        private static readonly string Config_CircularMB = "COMPlus_EventPipeCircularMB";
        private static readonly string Config_OutputFile = "COMPlus_EventPipeOutputFile";

        // Marker file extension.
        internal static readonly string MarkerFileExtension = ".ctl";

        internal EventPipeConfiguration()
        {
            // By default, assume we want to allow tracing to be enabled on-demand.
            EnableValue = 4;
        }

        // Create a configuration object from the environment.
        internal static EventPipeConfiguration FromEnvironment()
        {
            EventPipeConfiguration config = new EventPipeConfiguration();
            
            // By default, assume that tracing is disabled.
            config.EnableValue = 0;

            // Translate data from the environment variables into the configuration object.
            IDictionary vars = Environment.GetEnvironmentVariables();

            if (vars.Contains(Config_Enable))
            {
                string strValue = (string)vars[Config_Enable];
                config.EnableValue = Convert.ToUInt32(strValue);
            }
            if (vars.Contains(Config_ProviderConfiguration))
            {
                config.ProviderConfiguration = (string)vars[Config_ProviderConfiguration];
            }
            if(vars.Contains(Config_OutputFile))
            {
                config.TraceFilePath = (string)vars[Config_OutputFile];
            }
            if(vars.Contains(Config_CircularMB))
            {
                config.CircularMB = (uint)vars[Config_CircularMB];
            }
            if(vars.Contains(Config_Rundown))
            {
                string strValue = (string)vars[Config_Rundown];
                if("0".Equals(strValue))
                {
                    config.EnableRundown = false;
                }
                else if("1".Equals(strValue))
                {
                    config.EnableRundown = true;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("COMPlus_Rundown");
                }
            }

            return config;
        }

        internal uint EnableValue
        {
            get; set;
        }

        internal string ProviderConfiguration
        {
            get; set;
        }

        internal string TraceFilePath
        {
            get; set;
        }

        internal uint CircularMB
        {
            get; set;
        }

        internal bool EnableRundown
        {
            get; set;
        }

        internal static void SetConfig(EventPipeConfiguration config)
        {
            SetEnvironmentVariable(Config_Enable, config.EnableValue.ToString());
            if (!string.IsNullOrEmpty(config.ProviderConfiguration))
            {
                SetEnvironmentVariable(Config_ProviderConfiguration, config.ProviderConfiguration);
            }
            if (!string.IsNullOrEmpty(config.TraceFilePath))
            {
                SetEnvironmentVariable(Config_OutputFile, config.TraceFilePath);
            }
            if (config.CircularMB != 0)
            {
                SetEnvironmentVariable(Config_CircularMB, config.CircularMB.ToString());
            }
            SetEnvironmentVariable(Config_Rundown, config.EnableRundown ? "1" : "0");
        }

        internal static void ClearConfig()
        {
            SetEnvironmentVariable(Config_Enable, null);
            SetEnvironmentVariable(Config_ProviderConfiguration, null);
            SetEnvironmentVariable(Config_OutputFile, null);
            SetEnvironmentVariable(Config_CircularMB, null);
            SetEnvironmentVariable(Config_Rundown, null);
        }

        private static void SetEnvironmentVariable(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.User);
        }
    }
}
