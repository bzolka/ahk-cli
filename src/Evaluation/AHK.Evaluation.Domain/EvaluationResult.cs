using System;
using System.Collections.Generic;
using System.Text;

namespace AHK.Evaluation
{
    public class EvaluationResult
    {
        public static EvaluationResult NotExecuted()
        {
            throw new NotImplementedException();
        }

        public static EvaluationResult Failed(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
