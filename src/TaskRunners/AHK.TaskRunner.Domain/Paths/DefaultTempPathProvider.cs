namespace AHK.TaskRunner
{
    public class DefaultTempPathProvider : ITempPathProvider
    {
        public static DefaultTempPathProvider Instance = new DefaultTempPathProvider();

        public TempPathScope GetTempDirectory()
            => new TempPathScope(System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName()));
    }
}
