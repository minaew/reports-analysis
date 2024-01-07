using System;
using System.IO;
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
            CountInternal(MovementSources.GetManual(), 1);
            CountInternal(Data.Deniz, 104);
            CountInternal(Data.Ziraat, 11);
            CountInternal(Data.Vacation, 40);
        }

        private static void CountInternal(string path, int count)
        {
            Assert.Equal(count, new Parser().Parse(path).Count());
        }

        private static void CountInternal(Stream stream, int count)
        {
            Assert.Equal(count, new Parser().Parse(stream).Count());
        }

        [Fact]
        public void Exists()
        {
            ExistsInternal(Data.Ziraat, o => o.DateTime == new DateTime(2023, 1, 8));

            ExistsInternal(Data.Ararat, o => o.DateTime == new DateTime(2022, 10, 28, 13, 6, 17) &&
                                             o.Amount.Value == 720500 &&
                                             o.Amount.Currency == "amd");

            ExistsInternal(Data.Ararat2, o => o.DateTime == new DateTime(2022, 12, 27, 14, 45, 34)
                                              && o.Amount.Value == 1371300
                                              && o.Amount.Currency == "amd");
        }

        private static void ExistsInternal(string path, Func<Operation, bool> predicate)
        {
            Assert.Equal(1, new Parser().Parse(path).Count(predicate));
        }

        [Fact]
        public void Identity()
        {
            IdentityInternal("sber-leha-2022-10-01-2022-12-09.pdf", Data.LehaSberIdentity);
            IdentityInternal("tink-leha-2022-10.pdf", Data.LehaTinkIdentity);
            IdentityInternal("tink-leha-2022-11.pdf", Data.LehaTinkIdentity);
            IdentityInternal("tink-maha-2022-09.pdf", Data.MahaTinkIdentity);
            IdentityInternal("tink-maha-2022-10.pdf", Data.MahaTinkIdentity);
        }

        private static void IdentityInternal(string name, string identity)
        {
            Assert.Equal(new Identifier().Identify(Path.Combine(Data.Root, name)), identity);
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
            RangeInternal("01.06.2023-01.07.2023", MovementSources.GetManual());
            RangeInternal("18.09.2022-11.10.2022", Data.Vacation);
        }

        private void RangeInternal(string range, string path)
        {
            Assert.Equal(DateRange.Parse(range), new Ranger().GetRange(path));
        }

        private void RangeInternal(string range, Stream stream)
        {
            Assert.Equal(DateRange.Parse(range), new Ranger().GetRange(stream));
        }

        [Fact]
        public void First()
        {
            FirstInternal("16.08.2022 15:49", Data.MahaSeptember);
        }

        private static void FirstInternal(string date, string path)
        {
            var dateTime = DateTime.ParseExact(date, "dd.MM.yyyy HH:mm", null);
            var firstOperationDate = new Parser().Parse(path).First().DateTime;
            Assert.Equal(dateTime, firstOperationDate);
        }

        [Fact]
        public void Last()
        {
            LastDate("10.09.2022 23:03", Data.MahaSeptember);
            LastDescription("SBERBANK_ONL@IN_PLATEZ", Data.Sber);
            LastDescription("Tinkoff Bank", Data.Sber2);
        }

        private static void LastDate(string date, string path)
        {
            var dateTime = DateTime.ParseExact(date, "dd.MM.yyyy HH:mm", null);
            var firstOperationDate = new Parser().Parse(path).Last().DateTime;
            Assert.Equal(dateTime, firstOperationDate);
        }

        private static void LastDescription(string description, string path)
        {
            var lastOperationDescription = new Parser().Parse(path).Last().Description;
            Assert.Equal(description, lastOperationDescription);
        }
    }
}
