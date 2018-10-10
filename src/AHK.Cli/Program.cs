using Microsoft.Extensions.Configuration;

namespace AHK
{
    class Program
    {
        static void Main(string[] args)
        {
            var appConfig = getAppConfig();

            createCliApp(appConfig)
                .Execute(args);
        }

        private static Microsoft.Extensions.CommandLineUtils.CommandLineApplication createCliApp(AppConfig appConfig)
        {
            var cliApp = new Microsoft.Extensions.CommandLineUtils.CommandLineApplication(throwOnUnexpectedArg: false);
            cliApp.Command("run",
                runCommandConfig =>
                {
                    runCommandConfig.Description = "Kiertekeles futtatasa hallgatoi megoldasokon";
                    var megoldasArg = runCommandConfig.Option("-m|--megoldas", "Hallgatoi megoldasokat tartalmazo konyvtar", Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue);
                    var eredmenyArg = runCommandConfig.Option("-e|--eredmeny", "Eredmenyek ebbe a konyvtarba keruljenek", Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue);

                    runCommandConfig.OnExecute(async () =>
                    {
                        if (!megoldasArg.HasValue() || !eredmenyArg.HasValue())
                        {
                            runCommandConfig.ShowHelp();
                            return -1;
                        }

                        return await RunCommand.Execute(megoldasArg.Value(), eredmenyArg.Value(), appConfig);
                    });
                },
                throwOnUnexpectedArg: false);
            cliApp.HelpOption("-?|-h|--help");


            cliApp.FullName = "AUTomatikusa Hazi Kiertekelo";
            cliApp.OnExecute(() => { cliApp.ShowHelp(); return 0; });

            return cliApp;
        }

        private static AppConfig getAppConfig()
        {
            var configRoot = new ConfigurationBuilder()
                                    .AddJsonFile(@"AppConfig.json", optional: true, reloadOnChange: false)
                                    .AddEnvironmentVariables("AHK_")
                                    .Build();
            var appConfig = new AppConfig();
            configRoot.Bind(appConfig);
            return appConfig;
        }
    }
}
