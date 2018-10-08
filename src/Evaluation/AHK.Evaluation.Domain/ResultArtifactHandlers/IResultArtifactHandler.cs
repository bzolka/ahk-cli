namespace AHK.Evaluation
{
    /// <summary>
    /// Provides a path for placing the execution result artifacts to.
    /// </summary>
    public interface IResultArtifactHandler
    {
        string GetPathFor(string studentId);
    }
}
