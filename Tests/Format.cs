using System;
using System.IO;
using PdfExtractor;
using PdfExtractor.Models;
using Xunit;

namespace Tests
{
    public class FormatTest
    {
        [Fact]
        public void WholeDirectory()
        {
            var detector = new FormatDetector();
            foreach (var file in Directory.EnumerateFiles(Data.Root))
            {
                var format = FormatDetector.GetFormat(file);

                if (Path.GetExtension(file) == ".txt")
                {
                    Assert.Equal(Format.RawText, format);
                    continue;
                }
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith("leha") ||
                    fileName.StartsWith("maha"))
                {
                    Assert.Equal(Format.Tinkoff, format);
                    continue;
                }
                if (fileName.StartsWith("sber-debet") ||
                    fileName.StartsWith("sber-credit"))
                {
                    Assert.Equal(Format.Sber, format);
                    continue;
                }
                if (fileName.StartsWith("sber-dollar") ||
                    fileName.StartsWith("sber-euro") ||
                    fileName.StartsWith("sber-nakop") ||
                    fileName.StartsWith("sber-popol") ||
                    fileName.StartsWith("sber-sbereg") ||
                    fileName.StartsWith("sber-sbervklad"))
                {
                    Assert.Equal(Format.SberVklad, format);
                    continue;
                }

                throw new InvalidOperationException($"No format for {file}");
            }
        }
    }
}
