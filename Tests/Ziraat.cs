using System;
using System.Linq;
using Xunit;
using PdfExtractor.Parsers;

namespace Tests
{
    public class Ziraat
    {
        [Fact]
        public void Count()
        {
            var parser = new ZiraatParser();
            Assert.Equal(11,
                parser.Parse(Data.Ziraat).Count());
        }

        [Fact]
        public void Date()
        {
            var operations = new ZiraatParser().Parse(Data.Ziraat).ToList();
            Assert.True(
                operations.Exists(o => o.DateTime == new DateTime(2023, 1, 8))
                );
        }
    }
}
