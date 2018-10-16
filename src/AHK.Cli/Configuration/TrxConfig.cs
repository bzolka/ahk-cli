﻿using Microsoft.Extensions.Logging;

namespace AHK.Configuration
{
    public class TrxConfig : IConfigValidator
    {
        public string TrxFileName { get; set; }
        public string ResultFileName { get; set; }

        public bool Validate(ILogger logger)
        {
            if (string.IsNullOrEmpty(TrxFileName))
            {
                logger.LogError("Trx file name not specified");
                return false;
            }

            if (string.IsNullOrEmpty(ResultFileName))
            {
                logger.LogError("Trx result file name not specified");
                return false;
            }

            return true;
        }
    }
}