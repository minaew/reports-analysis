using System;
using System.Linq;
using Xunit;
using PdfExtractor;
using PdfExtractor.Parsers;
using PdfExtractor.Models;

namespace Tests
{
    public class Common
    {
        [Fact]
        public void Count()
        {
            CountInternal(new AraratParser(), Data.Ararat, 77);
            CountInternal(new TinkoffParser(), Data.MahaSeptember, 47);
            CountInternal(new SberParser(), Data.Sber, 28);
            CountInternal(new ManualParser(), Data.Manual, 29);
            CountInternal(new DenizParser(), Data.Deniz, 104);
            CountInternal(new ZiraatParser(), Data.Ziraat, 11);
            CountInternal(new ExpensesAppParser(), Data.Vacation, 40);
        }

        private static void CountInternal(IParser parser, string path, int count)
        {
            Assert.Equal(count, parser.Parse(path).Count());
        }

        [Fact]
        public void Exists()
        {
            ExistsInternal(new ZiraatParser(), Data.Ziraat, o => o.DateTime == new DateTime(2023, 1, 8));
        }

        private static void ExistsInternal(IParser parser, string path, Func<Operation, bool> predicate)
        {
            Assert.Equal(1, parser.Parse(path).Count(predicate));
        }
    }
}