using System;
using Microsoft.Extensions.Configuration;

namespace Ahk.Configuration
{
    internal static class RunConfigReader
    {
        public static AhkJobConfig GetAndValidateConfig(string configFilePath)
        {
            var configurationRoot = new ConfigurationBuilder().AddJsonFile(configFilePath).Build();
            var c = configurationRoot.Get<AhkJobConfig>();
            try
            {
                c.Validate();
            }
            catch (Exception ex)
            {
                throw new Exception($"A futtato konfiguracios fajlban ({configFilePath}) hiba van: {ex.Message}");
            }
            return c;
        }
    }
}
