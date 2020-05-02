using ERService.Infrastructure.Helpers;
using System;
using Xunit;
using Xunit.Abstractions;

namespace InfrastructureLibTestXUnit.Helpers
{
    public class BarcodeGeneratorTest
    {
        private readonly ITestOutputHelper _outputHelper;

        public BarcodeGeneratorTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void GenerateBase64StringShouldReturnNonEmptyString()
        {
            var simpleOrderNumber = "01/042020";
            var result = BarcodeGenerator.GenerateBase64String(simpleOrderNumber);

            Assert.NotNull(result);
            Assert.True(!String.IsNullOrWhiteSpace(result));

            _outputHelper.WriteLine(result);
        }
    }
}
