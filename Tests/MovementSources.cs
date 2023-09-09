using System.IO;
using System.Text;

namespace ReportAnalysis.Tests
{
    public static class MovementSources
    {
        public static Stream GetManual()
        {
            var content = @"3000, ass
22.04, 10, р, cat1
23.04, 2, з, cat2
23.04, 1, ч, cat1
01.05, 320, х, cat3
01.05, 320, х, cat3
10.05, 3, м, cat2
25.05, 15, з, cat3
27.05, 8, пролд, cat3
28.05, 10, фыва, cat4
";
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }

        public static Stream GetManual2()
        {
            var content = @"2023, amd
21.06, 8000, квиз
";
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }
    }
}