namespace AHK.Configuration
{
    public class AHKJobConfig : IConfigValidator
    {
        public DockerConfig Docker { get; set; }
        public TrxConfig Trx { get; set; }

        public void Validate()
        {
            if (Docker == null)
                throw new System.Exception("Docker konfiguracio hianyzik");

            if (Trx == null)
                throw new System.Exception("Trx konfiguracio hianyzik");

            Docker.Validate();
            Trx.Validate();
        }
    }
}
