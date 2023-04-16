using System;
using System.Linq;
using Xunit;
using ReportAnalysis.Core;
using ReportAnalysis.Core.Parsers;
using ReportAnalysis.Core.Models;
using ReportAnalysis.Core.Interfaces;

namespace ReportAnalysis.Tests
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

        [Fact]
        public void Identity()
        {
            Assert.Equal(new SberParser().Identify(Data.Sber),
                         Data.LehaSberIdentity);
        }

        [Fact]
        public void Range()
        {
            RangeInternal("01.10.2022-11.12.2022", Data.Ararat);
            RangeInternal("11.08.2022-10.09.2022", Data.MahaSeptember);
            RangeInternal("02.10.2022-01.11.2022", Data.Tink);
            RangeInternal("01.10.2022-09.12.2022", Data.Sber);
            RangeInternal("12.06.2022-12.12.2022", Data.Deniz);
        }

        private void RangeInternal(string range, string path)
        {
            Assert.Equal(DateRange.Parse(range), new Ranger().GetRange(path));
        }
    }
}