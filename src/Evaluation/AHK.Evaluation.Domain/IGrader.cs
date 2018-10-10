using System;
using System.Threading.Tasks;

namespace AHK.Evaluation
{
    public interface IGrader : IDisposable
    {
        Task GradeResult(string directory);
        Task StoreFailedExecution(string errorMessage);
    }
}