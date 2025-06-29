﻿using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace ParallelCollections;

[MemoryDiagnoser]
public class PartitioningBenchmark
{
    private List<int> _listData;
    private int[] _arrayData;
    
    private ListProcessing _listProcessing;
    private PartitionedProcessing _partitionedProcessing;

    [Params(1_000_000)] // 1 mil. elements
    public int N;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _listData = new List<int>(N);
        _arrayData = new int[N];

        var random = new Random(100);

        for (int i = 0; i < N; i++)
        {
            int value = random.Next(1, 100);
            _listData.Add(value);
            _arrayData[i] = value;
        }
        
        _listProcessing = new ListProcessing(_listData);
        _partitionedProcessing = new PartitionedProcessing(_arrayData);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
    
    [Benchmark]
    public void ListProcessing()
    {
        _listProcessing.ProcessData();
    }

    [Benchmark]
    public void PartitionedProcessing()
    {
        _partitionedProcessing.ProcessData();
    }
}