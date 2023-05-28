using Xunit;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Tests
{
    public class Ranges
    {
        [Fact]
        public void NonOverlap()
        {
            var range1 = DateRange.Parse("01.01.2022-10.01.2022");
            var range2 = DateRange.Parse("15.01.2022-20.01.2022");

            var range = range1.Add(range2);

            Assert.Equal(2, range.Ranges.Count);
            Assert.Equal(range.Ranges[0], DateRange.Parse("01.01.2022-10.01.2022"));
            Assert.Equal(range.Ranges[1], DateRange.Parse("15.01.2022-20.01.2022"));
        }

        [Fact]
        public void Overlap()
        {
            var range1 = DateRange.Parse("01.01.2022-10.01.2022");
            var range2 = DateRange.Parse("05.01.2022-15.01.2022");

            var range = range1.Add(range2);

            Assert.Equal(1, range.Ranges.Count);
            Assert.Equal(range.Ranges[0], DateRange.Parse("01.01.2022-15.01.2022"));
        }

        [Fact]
        public void Fold()
        {
            var range1 = DateRange.Parse("01.01.2022-10.01.2022");
            var range2 = DateRange.Parse("03.01.2022-08.01.2022");

            var range = range1.Add(range2);

            Assert.Equal(1, range.Ranges.Count);
            Assert.Equal(range.Ranges[0], DateRange.Parse("01.01.2022-10.01.2022"));
        }
    }
}