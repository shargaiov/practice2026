using System;
using Xunit;
using task11;

namespace task11tests
{
    public class CalculatorTests
    {
        private readonly ICalculator _calc;

        public CalculatorTests()
        {

            var generator = new CalculatorGenerator();
            _calc = generator.Generate();
        }

        [Theory]
        [InlineData(5, 7, 12)]
        [InlineData(-5, 5, 0)]
        [InlineData(-10, -20, -30)]
        public void Add_ShouldSumNumbersProperly(int first, int second, int expectedOutput)
        {
            Assert.Equal(expectedOutput, _calc.Add(first, second));
        }

        [Theory]
        [InlineData(15, 5, 10)]
        [InlineData(0, 8, -8)]
        [InlineData(-5, -5, 0)]
        public void Minus_ShouldSubtractNumbersProperly(int first, int second, int expectedOutput)
        {
            Assert.Equal(expectedOutput, _calc.Minus(first, second));
        }

        [Theory]
        [InlineData(4, 6, 24)]
        [InlineData(-3, 8, -24)]
        [InlineData(0, 100, 0)]
        public void Mul_ShouldMultiplyNumbersProperly(int first, int second, int expectedOutput)
        {
            Assert.Equal(expectedOutput, _calc.Mul(first, second));
        }

        [Theory]
        [InlineData(20, 4, 5)]
        [InlineData(-15, 3, -5)]
        [InlineData(9, -3, -3)]
        public void Div_ShouldDivideNumbersProperly(int first, int second, int expectedOutput)
        {
            Assert.Equal(expectedOutput, _calc.Div(first, second));
        }

        [Fact]
        public void Div_ShouldThrowException_WhenDividingByZero()
        {
            Assert.Throws<DivideByZeroException>(() => _calc.Div(42, 0));
        }
    }
}