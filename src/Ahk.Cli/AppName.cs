using System.Reflection;

namespace Ahk
{
    internal static class AppName
    {
        public const string Name = @"AUTomated homework evaluation";
        public const string Description = @".NET Core console application for executing assessment on student homework submissions by running a containerized evaluation logic - https://github.com/akosdudas/ahk-cli";
        public static readonly string Version = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString(3) ?? string.Empty;
    }
}
