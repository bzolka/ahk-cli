using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace AHK.Configuration
{
    internal static class RunConfigReader
    {
        private const int SuspiciousRunConfigSizeThreshold = 30 * 1024; // 30K

        public static AHKJobConfig Get(string path)
        {
            if (Directory.Exists(path))
            {
                var configFile = Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly)
                                                .FirstOrDefault(f => Path.GetFileName(f).Equals("ahkconfig.json", StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(configFile))
                {
                    if (new FileInfo(configFile).Length > SuspiciousRunConfigSizeThreshold) // safety check to make sure the file is not maliciously large to cause out-of-memory error
                        throw new Exception($"Gyanus shkconfig.json fajl: {configFile}");

                    return GetAndValidateConfig(configFile);
                }
            }

            return null;
        }

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
