using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace CpuBoundVectorizedOptimizationExample;

public class CpuBoundVectorOptimizationBenchmarks
{
    private double[] data;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var size = 10_000_000;
        data = new double[size];
        var random = new Random(42);

        for (var i = 0; i < size; i++)
            // reasonable input values for polynomial computation
            data[i] = random.NextDouble() * 1000;
    }


    [Benchmark]
    public double[] OriginalProcessing()
    {
        return ProcessData(data);
    }

    [Benchmark]
    public double[] OptimizedProcessing()
    {
        return ProcessDataOptimized(data);
    }

    [IterationCleanup]
    public void Cleanup()
    {
        GC.Collect();
    }

    private double[] ProcessData(double[] input)
    {
        var result = new double[input.Length];
        for (var i = 0; i < input.Length; i++) result[i] = ComplexComputation(data[i]);
        return result;
    }

    private double[] ProcessDataOptimized(double[] input)
    {
        var length = input.Length;
        var result = new double[length];
        var vectorSize = Vector<double>.Count;

        var threeVector = new Vector<double>(3.0);
        var twoVector = new Vector<double>(2.0);

        var i = 0;
        for (; i <= length - vectorSize; i++)
        {
            var inputVector = new Vector<double>(data, i); // this is `x`

            var squared = Vector.Multiply(inputVector, threeVector); // x * x
            var threeX = Vector.Multiply(squared, threeVector); // x * 3
            var sum = Vector.Add(squared, threeX); // (x * x) + (3 * x)
            var resultVector = Vector.Add(sum, twoVector); // [(x * x) + (3 * x)] + 2

            resultVector.CopyTo(result, i);
        }

        // calculate rest sequentially / without vector optimization
        for (; i < length; i++) result[i] = ComplexComputation(data[i]);

        return result;
    }

    private static double ComplexComputation(double x)
    {
        return x * x + 3 * x + 2;
    }
}