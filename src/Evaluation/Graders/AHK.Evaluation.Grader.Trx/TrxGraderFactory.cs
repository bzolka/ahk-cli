namespace AHK.Evaluation.Grader
{
    public class TrxGraderFactory : IGraderFactory
    {
        public IGrader CreateGrader(EvaluationTask forTask) => new TrxGrader(forTask, new TrxGraderOptions(forTask.EvaluationConfig.TrxFileName, forTask.EvaluationConfig.ResultFileName));
    }
}
