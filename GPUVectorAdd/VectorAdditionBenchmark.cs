using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using ILGPU.Runtime.OpenCL;

namespace GPUVectorAdd;

public class VectorAdditionBenchmark
{
    // Adjust based on testing needs
    private const int Length = 1 << 26;

    private float[] a;
    private Accelerator accelerator;
    private float[] b;

    private Context context;
    private float[] cpuResult;
    private Action<Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>> gpuKernel;
    private float[] gpuResult;
    
    #region GPU (or accelerator) allocated memory

    private MemoryBuffer1D<float, Stride1D.Dense> bufferA;
    private MemoryBuffer1D<float, Stride1D.Dense> bufferB;
    private MemoryBuffer1D<float, Stride1D.Dense> bufferResult;

    #endregion
    
    [GlobalSetup]
    public void GlobalSetup()
    {
        a = new float[Length];
        b = new float[Length];
        cpuResult = new float[Length];
        gpuResult = new float[Length];

        for (var i = 0; i < Length; i++)
        {
            a[i] = i;
            b[i] = Length - i;
        }

        context = Context.Create(builder => builder.Cuda());
        accelerator = context.CreateCLAccelerator(0);



        // compile the GPU kernel
        gpuKernel = accelerator
            .LoadAutoGroupedStreamKernel<Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>>(
                GpuVectorAddition);

        // Allocate GPU buffers once and reuse them
        bufferA = accelerator.Allocate1D(a);
        bufferB = accelerator.Allocate1D(b);
        bufferResult = accelerator.Allocate1D<float>(Length);
    }

    private void CpuAddition()
    {
        for (var i = 0; i < Length; i++)
        {
            cpuResult[i] = a[i] + b[i];
        }
    }

    private static void GpuVectorAddition(Index1D index, ArrayView<float> a, ArrayView<float> b,
        ArrayView<float> result)
    {
        result[index] = a[index] + b[index];
    }

    [Benchmark]
    public void CpuAdd()
    {
        CpuAddition();
    }

    [Benchmark]
    public void GpuAdd()
    {
        gpuKernel(new Index1D(Length), bufferA.View, bufferB.View, bufferResult.View);
        accelerator.Synchronize();
    }


    [GlobalCleanup]
    public void Cleanup()
    {
        bufferA.Dispose();
        bufferB.Dispose();
        bufferResult.Dispose();
        accelerator.Dispose();
        context.Dispose();
    }
}