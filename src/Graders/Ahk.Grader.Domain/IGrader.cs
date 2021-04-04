using System;
using System.Threading.Tasks;

namespace Ahk.Grader
{
    public interface IGrader : IDisposable
    {
        Task<GraderResult> GradeResult();
    }
}