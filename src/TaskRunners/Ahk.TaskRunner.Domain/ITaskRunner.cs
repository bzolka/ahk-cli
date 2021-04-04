using System;
using System.Threading.Tasks;

namespace Ahk.TaskRunner
{
    public interface ITaskRunner : IDisposable
    {
        Task<RunnerResult> Run();
    }
}
