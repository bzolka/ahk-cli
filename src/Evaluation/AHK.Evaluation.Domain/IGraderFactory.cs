namespace AHK.Evaluation
{
    public interface IGraderFactory
    {
        IGrader CreateGrader(EvaluationTask forTask);
    }
}
