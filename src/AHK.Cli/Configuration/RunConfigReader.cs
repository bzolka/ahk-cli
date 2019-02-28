using System;
using Microsoft.Extensions.Configuration;

namespace AHK.Configuration
{
    internal static class RunConfigReader
    {
        public static AHKJobConfig GetAndValidateConfig(string configFilePath)
        {
            var configurationRoot = new ConfigurationBuilder().AddJsonFile(configFilePath).Build();
            var c = configurationRoot.Get<AHKJobConfig>();
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
