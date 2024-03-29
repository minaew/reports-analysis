using System;
using System.IO;
using System.Linq;
using Xunit;
using ReportAnalysis.Core;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Tests
{
    public class Tinkoff
    {
        [Fact]
        // 11.08.22 09:39 11.08.22 1.00 ₽ 1.00 ₽ Оплата в Mos.Transport MOSKVA RUS
        public void TrivialOutcome()
        {
            var operations = new Parser().Parse(Data.MahaSeptember).ToList();

            Assert.Equal(operations[3],
            new Operation
            {
                Account = Data.MahaTinkIdentity,
                DateTime = new DateTime(2022, 8, 11, 9, 39, 0),
                Amount = new Money(-1, "rub"),
                Description = "Оплата в Mos.Transport MOSKVA RUS"
            });
        }

        [Fact]
        // 10.09.22 10.09.22 +155.57 ₽ +155.57 ₽ Проценты на остаток
        public void TrivialIncome()
        {
            var operations = new Parser().Parse(Data.MahaSeptember);

            Assert.Contains(new Operation
            {
                Account = Data.MahaTinkIdentity,
                DateTime = new DateTime(2022, 09, 10, 0, 0, 0),
                Amount = new Money(155.57, "rub"),
                Description = "Проценты на остаток"
            }, operations);
        }

        [Fact]
        // 01.09.22 21:10 02.09.22 100.00 ₽ 100.00 ₽ Внешний банковский перевод счёт 40703810238000008427, ПАО
        //                                           СБЕРБАНК
        public void MultiLine()
        {
            var operations = new Parser().Parse(Data.MahaSeptember);

            Assert.Contains(new Operation
            {
                Account = Data.MahaTinkIdentity,
                DateTime = new DateTime(2022, 09, 1, 21, 10, 0),
                Amount = new Money(-100, "rub"),
                Description = "Внешний банковский перевод счёт 40703810238000008427, ПАО СБЕРБАНК"
            }, operations);
        }

        [Fact]
        public void Totals()
        {
            var values = new Parser()
                .Parse(Data.MahaSeptember)
                .Select(o => o.Amount.Value)
                .ToList();

            var income = values.Where(v => v > 0).Sum();
            Assert.Equal(104950.05 + 155.57 + 543.00, income, 1);

            var outcome = values.Where(v => v < 0).Sum();
            Assert.Equal(-56277.11, outcome, 1);
        }
    }
}
