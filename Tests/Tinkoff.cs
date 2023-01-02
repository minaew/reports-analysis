using System;
using System.IO;
using System.Linq;
using Xunit;
using PdfExtractor.Parsers;
using PdfExtractor.Models;

namespace Tests
{
    public class Tinkoff
    {
        [Fact]
        public void TrivialOutcome()
        {
            var parser = new TinkoffParser();
            var operations = parser.Parse(Data.LehaDecember);

            Assert.Contains(new Operation
            {
                DateTime = new DateTime(2021, 12, 4, 13, 42, 0),
                Amount = new Money(-1269.98, "rub"),
                Description = "Оплата в PYATEROCHKA 21152 Zelenograd RUS"
            }, operations);
        }

        [Fact]
        public void TrivialIncome()
        {
            var parser = new TinkoffParser();
            var operations = parser.Parse(Data.LehaDecember);

            Assert.Contains(new Operation
            {
                DateTime = new DateTime(2021, 12, 16, 22, 57, 0),
                Amount = new Money(1000, "rub"),
                Description = "Пополнение. Система быстрых платежей"
            }, operations);
        }

        [Fact]
        public void Count()
        {
            var parser = new TinkoffParser();
            var operations = parser.Parse(Data.LehaDecember);

            Assert.Equal(48, operations.Count());
        }

        [Fact]
        public void ParseAll()
        {
            var parser = new TinkoffParser();

            foreach (var file in Directory.GetFiles(Data.Root, "leha-*")
                         .Concat(Directory.GetFiles(Data.Root, "maha-*")))
            {
                try
                {
                    var operations = parser.Parse(file);
                }
                catch (Exception ex)
                {
                    throw new Exception(file, ex);
                }
            }
        }
    }
}