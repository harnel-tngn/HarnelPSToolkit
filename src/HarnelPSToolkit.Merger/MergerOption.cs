using CommandLine;

namespace ProblemSolving.Templates.Merger
{
    public sealed class MergerOption
    {
        public string DllDirectory => Path.GetDirectoryName(DllPath)!;

        [Option("DllPath", Required = true)]
        public string DllPath { get; set; } = default!;

        [Option("EntrypointPath", Required = true)]
        public string EntrypointPath { get; set; } = default!;

        [Option("StandalonePath", Required = true)]
        public string StandalonePath { get; set; } = default!;
    }
}
