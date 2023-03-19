using AHK.Configuration.Model;

namespace AHK.Configuration
{
    public class AHKJobConfig : IConfigValidator
    {
        public DockerConfig Docker { get; set; }
        public TrxConfig Trx { get; set; }
        public XlsxWriterConfig XlsxWriter { get; set; }
        public ConsoleMessageGraderConfig ConsoleMessageGrader { get; set; }
        // BZ: not null indicates that command line is to be used instead of docker ("DockerConfig Docker" is ignored in this case)
        public LocalCmdConfig Command { get; set; }

        public void Validate()
        {
            if (Docker == null && Command == null)
                throw new System.Exception($"A konfiguracio ervenytelen, az alábbi szekciok egyikenek leteznie kell: {nameof(Command)}/{nameof(Docker)}");

            Docker?.Validate();
            Command?.Validate();
            Trx?.Validate();
            XlsxWriter?.Validate();
            ConsoleMessageGrader?.Validate();
        }
    }
}
