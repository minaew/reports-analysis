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
            Assert.Equal(new DateRange(new DateTime(2022, 10, 1), new DateTime(2022, 12, 11)),
                         new Ranger().GetRange(Data.Ararat));

            Assert.Equal(new DateRange(new DateTime(2022, 8, 11), new DateTime(2022, 9, 10)),
                         new Ranger().GetRange(Data.MahaSeptember));

            Assert.Equal(new DateRange(new DateTime(2022, 10, 2), new DateTime(2022, 11, 1)),
                         new Ranger().GetRange(Data.Tink));

            Assert.Equal(new DateRange(new DateTime(2022, 10, 1), new DateTime(2022, 12, 9)),
                         new Ranger().GetRange(Data.Sber));
        }
    }
}