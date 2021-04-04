namespace Ahk.TaskRunner
{
    public interface ITempPathProvider
    {
        TempPathScope GetTempDirectory();
    }
}
