using System;

namespace PdfExtractor.Models
{
    public struct Operation
    {
        public string Account { get; set; }

        public DateTime DateTime { get; set; }

        public Money Amount { get; set; } // TODO: type

        public string Category { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return DateTime.ToString();
        }

        public string Serialize()
        {
            var dateTime = DateTime.ToString("dd.MM.yyyy HH:mm");
            return $"{dateTime}\t{Amount}\t{Description}";
        }
    }

    public class Money
    {
        public static Money FromString(string money)
        {
            var tokens = money.Split(" ");
            return new Money
            {
                Value = double.Parse(tokens[0]),
                Currency = tokens[1]
            };
        }
        
        public double Value { get; set; }
        
        public string Currency { get; set; }
    }
}
