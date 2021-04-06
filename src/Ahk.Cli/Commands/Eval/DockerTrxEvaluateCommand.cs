using System.Threading.Tasks;
using Ahk.Grader;
using Ahk.TaskRunner;
using CliFx.Attributes;
using CliFx.Exceptions;
using Microsoft.Extensions.Logging;

namespace Ahk.Commands.Eval
{
    [Command("eval docker trx", Description = "Run evaluation using Docker container and Trx grader")]
    public class DockerTrxEvaluateCommand : DockerEvaluateCommandBase
    {
        public DockerTrxEvaluateCommand(ILogger logger)
            : base(logger)
        {
        }

        [CommandOption("trxfile", Description = "Name of the trx file to parse. Expected in the output folder of the container.", IsRequired = true)]
        public string TrxFileName { get; set; } = string.Empty;

        protected override Task executeCommandCore()
        {
            if (string.IsNullOrEmpty(ArtifactDirInContainer))
                throw new CommandException("Trx grader requires the trx file to be present in the artifact directory within the container, hence the artifact-path argument needs to be specified");

            return base.executeCommandCore();
        }

        protected override IGrader CreateGrader(string submissionSource, string studentId, string artifactPath, RunnerResult runnerResult)
            => new TrxGrader(new TrxGraderTask(submissionSource, studentId, System.IO.Path.Combine(artifactPath, this.TrxFileName)), logger);
    }
}
