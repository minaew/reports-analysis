using System;
using Xunit;
using PdfExtractor;

namespace Tests
{
    public class SberVklad
    {
        private readonly IParser _parser = new SberVkladParser();

        [Fact]
        public void OnlineFirst()
        {
            var operation = _parser.Parse(Data.PopolnajOnline)[0];

            Assert.Equal(operation,
                        new Operation
                        {
                            DateTime = new DateTime(2021, 9, 18),
                            Amount = -500,
                            Description = "Выдача процентов"
                        });
        }

        [Fact]
        public void LastOnPage()
        {
            var operation = _parser.Parse(Data.PopolnajOnline)[4];

            Assert.Equal(operation,
                        new Operation
                        {
                            DateTime = new DateTime(2021, 6, 7),
                            Amount = 61.49,
                            Description = "Пролонгация"
                        });
        }

        [Fact]
        public void VeryLast()
        {
            var operation = _parser.Parse(Data.PopolnajOnline)[^1];

            Assert.Equal(operation,
                        new Operation
                        {
                            DateTime = new DateTime(2020, 10, 4),
                            Amount = -600,
                            Description = "Выдача процентов"
                        });
        }

        [Fact]
        public void Count()
        {
            var count = _parser.Parse(Data.PopolnajOnline).Count;

            Assert.Equal(15, count);
        }

        [Fact]
        public void DollarVklad()
        {
            var operations = _parser.Parse(Data.DollarVklad);

            Assert.Equal(4, operations.Count);

            Assert.Equal(operations[0],
                        new Operation
                        {
                            DateTime = new DateTime(2021, 8, 12),
                            Amount = 0,
                            Description = "Капитализация"
                        });
        }

        [Fact]
        public void EuroVklad()
        {
            var operations = _parser.Parse(Data.DollarVklad);

            Assert.Equal(4, operations.Count);

            Assert.Equal(operations[0],
                        new Operation
                        {
                            DateTime = new DateTime(2021, 8, 12),
                            Amount = 0,
                            Description = "Капитализация"
                        });
        }
    }
}