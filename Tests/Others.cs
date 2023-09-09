using System;
using System.Linq;
using Xunit;
using ReportAnalysis.Core;

namespace ReportAnalysis.Tests
{
    public class Others
    {
        [Fact]
        public void ManualFormatV2()
        {
            var operations = new Parser().Parse(MovementSources.GetManual2()).ToList();
            Assert.Single(operations);

            var operation = operations[0];
            Assert.Equal(new DateTime(2023, 6, 21), operation.DateTime);
            Assert.Equal(8000, operation.Amount.Value);
            Assert.Equal("amd", operation.Amount.Currency);
            Assert.Equal("квиз", operation.Description);
        }
    }
}
