﻿using System;

namespace AHK.Evaluation
{
    partial class EvaluationStatisticsRecorder
    {
        public class EvaluationStatisticsRecorderScope : IDisposable
        {
            private readonly EvaluationStatisticsRecorder recorder;
            private bool committed = false;

            public EvaluationStatisticsRecorderScope(EvaluationStatisticsRecorder recorder)
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
