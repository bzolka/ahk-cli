using Microsoft.Extensions.Logging;

namespace AHK.Configuration
{
    interface IConfigValidator
    {
        bool Validate(ILogger logger);
    }
}
