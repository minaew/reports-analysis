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
            var operations = new Parser().Parse(Data.Manual2).ToList();
            Assert.Equal(14, operations.Count);

            var operation = operations[3];
            Assert.Equal(new DateTime(2023, 6, 21), operation.DateTime);
            Assert.Equal(8000, operation.Amount.Value);
            Assert.Equal("amd", operation.Amount.Currency);
            Assert.Equal("квиз", operation.Description);
        }
    }
}
