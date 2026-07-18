using System;
using System.Threading;

namespace task14;

public static class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (threadsNumber < 1) throw new ArgumentException("Число потоков >= 1");
        if (step <= 0) throw new ArgumentException("Шаг должен быть > 0");
        if (a >= b) throw new ArgumentException("Граница A должна быть меньше B");

        double totalResult = 0.0;
        int stepsCount = (int)Math.Ceiling((b - a) / step);
        double stepSize = (b - a) / stepsCount;

        using Barrier barrier = new Barrier(threadsNumber + 1);
        
        Thread[] workerThreads = new Thread[threadsNumber];
        int baseStepsPerThread = stepsCount / threadsNumber;
        int extraSteps = stepsCount % threadsNumber;

        for (int i = 0; i < threadsNumber; i++)
        {
            int threadId = i;
            int startStep = threadId * baseStepsPerThread + Math.Min(threadId, extraSteps);
            int endStep = startStep + baseStepsPerThread + (threadId < extraSteps ? 1 : 0);

            workerThreads[i] = new Thread(() =>
            {
                double threadLocalSum = 0.0;
                for (int s = startStep; s < endStep; s++)
                {
                    double x1 = a + s * stepSize;
                    double x2 = x1 + stepSize;
                    threadLocalSum += (function(x1) + function(x2)) * stepSize * 0.5;
                }

                AddDoubleAtomically(ref totalResult, threadLocalSum);
                
                barrier.SignalAndWait();
            });
            workerThreads[i].Start();
        }

        barrier.SignalAndWait();
        return totalResult;
    }

    private static void AddDoubleAtomically(ref double target, double value)
    {
        double initial, computed;
        do
        {
            initial = target;
            computed = initial + value;
        } 
        while (Interlocked.CompareExchange(ref target, computed, initial) != initial);
    }
}