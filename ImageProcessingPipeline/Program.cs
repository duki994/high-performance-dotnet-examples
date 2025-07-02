// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using ImageProcessingPipeline;

BenchmarkRunner.Run<ImageProcessingBenchmark>();

Console.WriteLine("Hello, World!");