namespace AHK.Evaluation
{
    /// <summary>
    /// Provides a service to get the solutions of a student.
    /// </summary>
    public interface ITaskSolutionProvider
    {
        string GetSolutionPath(string studentId);
    }
}
