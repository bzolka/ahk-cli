using Microsoft.Extensions.Logging;

namespace AHK.Configuration
{
    public class AHKJobConfig : IConfigValidator
    {
        public DockerConfig Docker { get; set; }
        public TrxConfig Trx { get; set; }
        public string ResultXlsxName { get; set; }

        public bool Validate(ILogger logger)
        {
            if (Docker == null)
            {
                logger.LogError("Docker configuration not specified");
                return false;
            }

            if (Trx == null)
            {
                logger.LogError("Trx configuration not specified");
                return false;
            }

            if (string.IsNullOrEmpty(ResultXlsxName))
            {
                logger.LogError("Trx result file name not specified");
                return false;
            }

            return Docker.Validate(logger)
                    && Trx.Validate(logger);
        }
    }
}
