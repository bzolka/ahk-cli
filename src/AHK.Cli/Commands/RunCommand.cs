using System;
using System.Threading.Tasks;
using AHK.Execution;
using Microsoft.Extensions.Logging;

namespace AHK
{
    internal static class RunCommand
    {
        public static async Task<int> Execute(string executionConfifFile, string assignmentsDir, string resultsDir, AppConfig appConfig, ILogger logger)
        {
            try
            {
                Console.WriteLine("Megoldasok beolvasasa...");
                var jobsLoader = new JobsLoader(logger);
                var tasks = jobsLoader.ReadFrom(executionConfifFile, assignmentsDir, resultsDir);
                Console.WriteLine("Megoldasok beolvasasa kesz.");

                Console.WriteLine("Kiertekeles futtatasa...");
                var executor = new Executor(logger, appConfig.MaxTaskRuntime);
                var execStatistics = await executor.Execute(tasks, new ConsoleProgress());
                Console.WriteLine("Kiertekeles futtatasa kesz.");

                if (execStatistics.HasFailed())
                {
                    Console.WriteLine("A kiertekeles kozben hibak voltak; alkalmazas vagy konfiguracios problema lehet.");
                    Console.WriteLine($"Osszesen {execStatistics.AllTasks} megoldas / {execStatistics.ExecutedSuccessfully} sikeresen kiertekelve, {execStatistics.FailedExecution} kiertekelese SIKERTELEN.");
                    return -1;
                }
                else
                {
                    Console.WriteLine($"Osszesen {execStatistics.ExecutedSuccessfully} megoldas kiertelekese elkeszult.");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Futasi hiba.");
                return -1;
            }
        }

        private class ConsoleProgress : IProgress<int>
        {
            public void Report(int value)
                => Console.WriteLine($"Kiertekeles: {value}%");
        }
    }
}
