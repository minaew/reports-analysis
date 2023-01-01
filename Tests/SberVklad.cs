using System;
using System.Linq;
using Xunit;
using PdfExtractor;
using PdfExtractor.Parsers;
using PdfExtractor.Models;

namespace Tests
{
    public class SberVklad
    {
        private readonly IParser _parser = new SberVkladParser();

        [Fact]
        public void OnlineFirst()
        {
            var operation = _parser.Parse(Data.PopolnajOnline).First();

            Assert.Equal(operation,
                        new Operation
                        {
                            DateTime = new DateTime(2021, 9, 18),
                            Amount = new Money
                            {
                                Value = -500,
                                Currency = "rub"
                            },
                            Description = "Выдача процентов"
                        });
        }

        [Fact]
        public void LastOnPage()
        {
            var operation = _parser.Parse(Data.PopolnajOnline).ElementAt(4);

            Assert.Equal(operation,
                        new Operation
                        {
                            DateTime = new DateTime(2021, 6, 7),
                            Amount = new Money
                            {
                                Value = 61.49,
                                Currency = "rub"
                            },
                            Description = "Пролонгация"
                        });
        }

        [Fact]
        public void VeryLast()
        {
            var operation = _parser.Parse(Data.PopolnajOnline).ToList()[^1];

            Assert.Equal(operation,
                        new Operation
                        {
                            DateTime = new DateTime(2020, 10, 4),
                            Amount = new Money
                            {
                                Value = -600,
                                Currency = "rub"
                            },
                            Description = "Выдача процентов"
                        });
        }

        [Fact]
        public void Count()
        {
            var count = _parser.Parse(Data.PopolnajOnline).Count();

            Assert.Equal(15, count);
        }

        [Fact]
        public void DollarVklad()
        {
            var operations = _parser.Parse(Data.DollarVklad).ToList();

            Assert.Equal(4, operations.Count);

            Assert.Equal(operations[0],
                        new Operation
                        {
                            DateTime = new DateTime(2021, 8, 12),
                            Amount = new Money
                            {
                                Value = 0,
                                Currency = "rub"
                            },
                            Description = "Капитализация"
                        });
        }

        [Fact]
        public void EuroVklad()
        {
            var operations = _parser.Parse(Data.DollarVklad).ToList();

            Assert.Equal(4, operations.Count);

            Assert.Equal(operations[0],
                        new Operation
                        {
                            DateTime = new DateTime(2021, 8, 12),
                            Amount = new Money
                            {
                                Value = 0,
                                Currency = "rub"
                            },
                            Description = "Капитализация"
                        });
        }
    }
}