using System;
using System.Threading.Tasks;

namespace AHK.Grader
{
    public interface IGrader : IDisposable
    {
        Task GradeResult();
        Task GradeFailedExecution(string errorMessage);
    }
}