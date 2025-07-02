namespace BatchProcessingExample;

public class BatchCalculator
{
    private readonly string csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "results.csv");
    private readonly object fileLock = new();

    public BatchCalculator()
    {
        if (!File.Exists(csvFilePath))
        {
            File.WriteAllText(csvFilePath, "Result\n");
        }
    }

    public void ProcessDataSequentially(List<double> data)
    {
        foreach (var result in data.Select(ComplexCalculation))
        {
            StoreResult(result);
        }
    }

    public void ProcessDataInBatches(List<double> data, int batchSize)
    {
        for (int i = 0; i < data.Count; i += batchSize)
        {
            var batch = data.GetRange(i, Math.Min(batchSize, data.Count - i));
            var results = new List<double>(batch.Count);

            results.AddRange(batch.Select(ComplexCalculation));

            StoreBatchResults(results);
        }
    }


    private double ComplexCalculation(double input)
    {
        double result = 0;

        for (int i = 0; i < 1000; i++)
        {
            result += Math.Sqrt(input) * Math.Sin(i) / (Math.Cos(i + input) + 1);
        }

        return result;
    }

    private void StoreResult(double result)
    {
        lock (fileLock)
        {
            File.AppendAllText(csvFilePath, $"{result}\n");
        }
    }

    private void StoreBatchResults(List<double> results)
    {
        lock (fileLock)
        {
            using var writer = new StreamWriter(csvFilePath, append: true);
            
            foreach (var item in results)
            {
                writer.WriteLine(item);
            }
        }
    }
}