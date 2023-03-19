using System;
using System.Collections.Generic;
using System.Text;

namespace AHK.Configuration
{


    public class LocalCmdConfig
    {
        public class EnvItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public string Command { get; set; }
        public TimeSpan EvaluationTimeout { get; set; } = TimeSpan.FromMinutes(5);

        // Could not trivially deserialize into a dictionary, using an array is a workaround
        // public EnvItem[] EnvVars { get; set; }

        EnvItem[] envVars;

        // Empty array is deserialized as null when loading a LocalCmdConfig object from json by the config system, which is deceiving for clients of this class.
        // Let's fix this in the getter.
        public EnvItem[] EnvVars {
            get => envVars == null ? Array.Empty<EnvItem>() : envVars;
            set => envVars = value;
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Command))
                throw new Exception("Command (parancs) hianyzik");
        }
    }


}
