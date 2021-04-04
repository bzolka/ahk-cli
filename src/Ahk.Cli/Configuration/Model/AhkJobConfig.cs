namespace Ahk.Configuration
{
    public class AhkJobConfig : IConfigValidator
    {
        public DockerConfig Docker { get; set; }
        public TrxConfig Trx { get; set; }
        public ConsoleMessageGraderConfig ConsoleMessageGrader { get; set; }

        public void Validate()
        {
            if (Docker == null)
                throw new System.Exception("Docker konfiguracio hianyzik");

            Docker.Validate();
            Trx?.Validate();
            ConsoleMessageGrader?.Validate();
        }
    }
}
