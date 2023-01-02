using System.Linq;
using PdfExtractor.Parsers;
using Xunit;

namespace Tests
{
    public class Ararat
    {
        [Fact]
        public void Count()
        {
            var parser = new AraratParser();
            Assert.Equal(77,
                parser.Parse(Data.AraratFirst).Count());
        }
    }
}