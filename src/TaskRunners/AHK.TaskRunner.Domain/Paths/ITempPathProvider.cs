namespace AHK.TaskRunner
{
    public interface ITempPathProvider
    {
        TempPathScope GetTempDirectory();
    }
}
