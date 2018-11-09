using System;
using System.Threading.Tasks;
using AHK.Execution;
using Microsoft.Extensions.Logging;

namespace AHK
{
    internal static class Runner
    {
        public static async Task<int> Go(string assignmentsDir, string executionConfigFile, string resultsDir, AppConfig appConfig, ILogger logger)
        {
            try
            {
                Console.WriteLine("Megoldasok beolvasasa...");
                var runConfig = JobsLoader.Load(assignmentsDir, executionConfigFile, resultsDir, logger);
                Console.WriteLine("Megoldasok beolvasasa kesz.");

                Console.WriteLine("Kiertekeles futtatasa...");
                var executor = new Executor(logger, appConfig.MaxTaskRuntime);
                var execStatistics = await executor.Execute(runConfig, new ConsoleProgress());
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
