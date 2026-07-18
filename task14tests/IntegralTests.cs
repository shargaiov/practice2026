using System;
using Xunit;
using task14;

namespace task14tests;

public class IntegralTests
{
    [Fact]
    public void Test_LinearFunction_SymmetricInterval_ResultZero()
    {
        Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, x => x, 0.0001, 4), 3);
    }

    [Fact]
    public void Test_SinFunction_SymmetricInterval_ResultZero()
    {
        Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, Math.Sin, 0.00001, 8), 3);
    }

    [Fact]
    public void Test_LinearFunction_PositiveInterval_ResultCalculated()
    {
        Assert.Equal(12.5, DefiniteIntegral.Solve(0, 5, x => x, 0.000001, 8), 3);
    }

    [Fact]
    public void Test_ConstantFunction_ResultCorrect()
    {
        Assert.Equal(5.0, DefiniteIntegral.Solve(0, 1, x => 5.0, 0.0001, 2), 3);
    }
}