namespace AHK.TaskRunner
{
    public interface ITaskRunnerFactory
    {
        ITaskRunner CreateRunner(RunnerTask task);
    }
}
