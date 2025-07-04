using BenchmarkDotNet.Attributes;

namespace BatchProcessingExample;

[MemoryDiagnoser]
public class BatchProcessingBenchmark
{
    private readonly int batchSize = 1000;
    private BatchCalculator calculator;
    private List<double> data;

    [GlobalSetup]
    public void Setup()
    {
        data = DataGenerator.GenerateData(10_000);
        calculator = new BatchCalculator();

        var csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "results.csv");

        if (File.Exists(csvFilePath)) File.Delete(csvFilePath);
    }

    [Benchmark(Baseline = true)]
    public void IndividualProcessing()
    {
        calculator.ProcessDataSequentially(data);
    }

    [Benchmark]
    public void BatchProcessing()
    {
        calculator.ProcessDataInBatches(data, batchSize);
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        GC.Collect();
    }
}