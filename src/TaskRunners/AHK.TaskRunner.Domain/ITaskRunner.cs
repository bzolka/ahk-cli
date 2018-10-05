using System;
using System.Threading.Tasks;

namespace AHK.TaskRunner
{
    public interface ITaskRunner : IDisposable
    {
        Task<RunnerResult> Run();
    }
}
