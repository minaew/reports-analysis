using System;

namespace PdfExtractor
{
    public struct Operation
    {
        public string Account { get; set; }

        public DateTime DateTime { get; set; }

        public double Amount { get; set; } // TODO: type

        public string Category { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            return DateTime.ToString();
        }

        public string Serialize()
        {
            return $"{DateTime}\t{Amount}\t{Description}";
        }
    }
}
