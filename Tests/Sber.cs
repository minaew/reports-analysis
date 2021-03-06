using System;
using System.Linq;
using PdfExtractor;
using Xunit;
using PdfExtractor.Parsers;
using PdfExtractor.Models;

namespace Tests
{
    public class Sber
    {
        private readonly IParser _parser = new SberParser();

        [Fact]
        public void SmallAmount()
        {
            var operation = _parser.Parse(Data.DebetReport)[0];
            Assert.Equal(operation,
                         new Operation
                         {
                             DateTime = new DateTime(2021, 9, 30, 10, 3, 0),
                             Amount = -5.27,
                             Description = "KOPILKA KARTA-VKLAD"
                         });
        }

        [Fact]
        public void BigAmount()
        {
            var operation = _parser.Parse(Data.DebetReport)[2];
            Assert.Equal(operation,
                         new Operation
                         {
                             DateTime = new DateTime(2021, 9, 29, 9, 54, 0),
                             Amount = 48363.64,
                             Description = "Аванс по заработной плате"
                         });
        }

        [Fact]
        public void LastOnFirstPageOperation()
        {
            var operation = _parser.Parse(Data.DebetReport)[10];
            Assert.Equal(operation,
                         new Operation
                         {
                             DateTime = new DateTime(2021, 9, 23, 10, 13, 0),
                             Amount = -50.45,
                             Description = "KOPILKA KARTA-VKLAD"
                         });
        }

        [Fact]
        public void VeryLastOperation()
        {
            var operation = _parser.Parse(Data.DebetReport)[^1];
            Assert.Equal(operation,
                         new Operation
                         {
                             DateTime = new DateTime(2020, 10, 1, 1, 23, 0),
                             Amount = -4800,
                             Description = "SBOL перевод 5469****6838 Ф. ВАДИМ ДМИТРИЕВИЧ"
                         });
        }

        [Fact]
        public void Overal()
        {
            var operations = _parser.Parse(Data.DebetReport);

            var overalIncome = operations.Select(o => o.Amount).Where(a => a > 0).Sum();
            Assert.Equal(1567116.33, overalIncome);

            var overalOutcome = operations.Select(o => o.Amount).Where(a => a < 0).Sum();
            Assert.Equal(-1496303.76, Math.Round(overalOutcome, 2));
        }

        [Fact]
        public void FirstCredit()
        {
            var operation = _parser.Parse(Data.CreditReport)[0];
            Assert.Equal(operation, 
                         new Operation
                         {
                             DateTime = new DateTime(2021, 9, 27, 9, 32, 0),
                             Amount = 215,
                             Description = "SBOL перевод 4276****8215 М. МАРИЯ МИХАЙЛОВНА"
                         });
        }

        [Fact]
        public void LastOnPageCredit()
        {
            var operation = _parser.Parse(Data.CreditReport)[10];
            Assert.Equal(operation, 
                         new Operation
                         {
                             DateTime = new DateTime(2021, 9, 20, 9, 36, 0),
                             Amount = 130,
                             Description = "SBOL перевод 4276****8215 М. МАРИЯ МИХАЙЛОВНА"
                         });
        }

        [Fact]
        public void VeryLastCredit()
        {
            var operation = _parser.Parse(Data.CreditReport)[^1];
            Assert.Equal(operation, 
                         new Operation
                         {
                             DateTime = new DateTime(2020, 10, 4, 10, 43, 0),
                             Amount = 1600,
                             Description = "SBOL перевод 4276****8215 М. МАРИЯ МИХАЙЛОВНА"
                         });
        }

        [Fact]
        public void OveralCredit()
        {
            var operations = _parser.Parse(Data.CreditReport);

            var overalIncome = operations.Select(o => o.Amount).Where(a => a > 0).Sum();
            Assert.Equal(70864.93, overalIncome);

            var overaOutcome = operations.Select(o => o.Amount).Where(a => a < 0).Sum();
            Assert.Equal(-40214.30 - 1582.49, overaOutcome);
        }
    }
}
