using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using ReportAnalysis.Core;

namespace ReportAnalysis.Tests
{
    public class Others
    {
        [Fact]
        public void ManualFormatV2()
        {
            var operations = new Parser().Parse(MovementSources.GetManual()).ToList();
            Assert.Single(operations);

            var operation = operations[0];
            Assert.Equal(new DateTime(2023, 6, 21), operation.DateTime);
            Assert.Equal(-8000, operation.Amount.Value);
            Assert.Equal("amd", operation.Amount.Currency);
            Assert.Equal("квиз", operation.Description);
        }
    }

    public static class MovementSources
    {
        public static Stream GetManual()
        {
            var content = @"01.06.2023-01.07.2023, amd
21.06, 8000, квиз
";
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }
    }
}
