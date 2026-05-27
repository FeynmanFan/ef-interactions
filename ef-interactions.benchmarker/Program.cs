namespace ef_interactions.benchmarker
{
    using BenchmarkDotNet.Running;

    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<EfLoadingBenchmarks>();
        }
    }
}
