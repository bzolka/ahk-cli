namespace Ahk.Configuration
{
    public class TrxConfig : IConfigValidator
    {
        public string? TrxFileName { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(TrxFileName))
                throw new System.Exception("Trx file name not specified");
        }
    }
}
