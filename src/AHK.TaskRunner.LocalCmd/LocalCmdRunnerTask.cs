using System;
using System.Collections.Generic;
using System.Text;

namespace AHK.TaskRunner
{
    public class LocalCmdRunnerTask: RunnerTask
    {
        public readonly string Command;
        public readonly string SolutionPath;
        public readonly string ResultArtifactPath;
        public readonly Dictionary<string, string> EnvironmentVariables;


        public LocalCmdRunnerTask(Guid taskId, string solutionPath, string resultArtifactPath,
            TimeSpan evaluationTimeout, string command, Dictionary<string, string> environmentVariables) : base(taskId, evaluationTimeout)
        {
            this.SolutionPath = solutionPath;
            this.ResultArtifactPath = resultArtifactPath;
            this.Command = command;
            this.EnvironmentVariables = environmentVariables;
        }
    }
}
