using System;

namespace Ahk.Execution
{
    partial class ExecutionStatisticsRecorder
    {
        public class ExecutionStatisticsRecorderScope : IDisposable
        {
            private readonly ExecutionStatisticsRecorder recorder;
            private bool committed = false;

            public ExecutionStatisticsRecorderScope(ExecutionStatisticsRecorder recorder)
                => this.recorder = recorder ?? throw new ArgumentNullException(nameof(recorder));

            public void Dispose()
            {
                if (!committed)
                    OnExecutionFailed();
            }

            public void OnExecutionFailed()
            {
                ++this.recorder.FailedExecution;
                committed = true;
            }

            public void OnExecutionCompleted()
            {
                ++this.recorder.ExecutedSuccessfully;
                committed = true;
            }
        }
    }
}
