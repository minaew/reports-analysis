using System;
using System.Linq;
using Xunit;
using ReportAnalysis.Core;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Tests
{
    public class Common
    {
        [Fact]
        public void Count()
        {
            CountInternal(Data.Ararat, 77);
            CountInternal(Data.MahaSeptember, 47);
            CountInternal(Data.Sber, 28);
            CountInternal(Data.Manual, 29);
            CountInternal(Data.Deniz, 104);
            CountInternal(Data.Ziraat, 11);
            CountInternal(Data.Vacation, 40);
        }

        private static void CountInternal(string path, int count)
        {
            Assert.Equal(count, new Parser().Parse(path).Count());
        }

        [Fact]
        public void Exists()
        {
            ExistsInternal(Data.Ziraat, o => o.DateTime == new DateTime(2023, 1, 8));
        }

        private static void ExistsInternal(string path, Func<Operation, bool> predicate)
        {
            Assert.Equal(1, new Parser().Parse(path).Count(predicate));
        }

        [Fact]
        public void Identity()
        {
            Assert.Equal(new Identifier().Identify(Data.Sber), Data.LehaSberIdentity);
        }

        [Fact]
        public void Range()
        {
            RangeInternal("01.10.2022-11.12.2022", Data.Ararat);
            RangeInternal("11.08.2022-10.09.2022", Data.MahaSeptember);
            RangeInternal("02.10.2022-01.11.2022", Data.Tink);
            RangeInternal("01.10.2022-09.12.2022", Data.Sber);
            RangeInternal("12.06.2022-12.12.2022", Data.Deniz);
            RangeInternal("25.07.2022-25.01.2023", Data.Ziraat);
            RangeInternal("22.10.2022-23.03.2023", Data.Manual);
            RangeInternal("18.09.2022-11.10.2022", Data.Vacation);
        }

        private void RangeInternal(string range, string path)
        {
            Assert.Equal(DateRange.Parse(range), new Ranger().GetRange(path));
        }
    }
}
