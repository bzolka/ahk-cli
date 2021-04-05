using System;
using System.Collections.Generic;
using System.Linq;

namespace Ahk.Grader
{
    public class GraderResultBuilder
    {
        public const string UnknownExercise = ExerciseResult.DefaultExerciseName;
        public const string UnknownTest = TestResult.DefaultTestName;

        private readonly Dictionary<string, List<TestResult>> exercises = new Dictionary<string, List<TestResult>>(StringComparer.OrdinalIgnoreCase);

        public void Add(string exerciseName, TestResult testResult)
        {
            if (testResult == null)
                return;

            if (string.IsNullOrEmpty(exerciseName))
                exerciseName = UnknownExercise;

            if (this.exercises.TryGetValue(exerciseName, out var list))
                list.Add(testResult);
            else
                this.exercises[exerciseName] = new List<TestResult>() { testResult };
        }

        public void AddResult(string exerciseName, string testName, int resultPoints, string? description)
            => Add(exerciseName, new TestResult(testName, resultPoints, GradingOutcomes.Graded, description));

        public void AddInconclusive(string exerciseName, string testName, string? description)
            => Add(exerciseName, new TestResult(testName, 0, GradingOutcomes.Inconclusive, description));

        public void AddFailedToGrade(string exerciseName, string testName, string? description)
            => Add(exerciseName, new TestResult(testName, 0, GradingOutcomes.FailedToGrade, description));

        public GraderResult ToResult()
        {
            if (exercises.Count == 0)
                return GraderResult.NoResult;

            return new GraderResult(exercises.Select(exr => new ExerciseResult(exr.Key, exr.Value)).ToList());
        }
    }
}
