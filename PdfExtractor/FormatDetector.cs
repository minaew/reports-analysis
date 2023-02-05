using System.IO;
using PdfExtractor.Helpers;
using PdfExtractor.Models;

namespace PdfExtractor
{
    public class FormatDetector
    {
        public static Format GetFormat(string path) => Path.GetExtension(path) switch
        {
            ".txt" => Format.RawText,
            ".xlsx" => DetectExcelFormat(path),
            ".json" => Format.ExpencesApp,
            ".pdf" => DetectPdfFormat(path),
            _ => Format.Invalid,
        };

        private static Format DetectPdfFormat(string path)
        {
            if (PdfHelper.GetFirstWord(path) == "ԱՐԱՐԱՏԲԱՆԿ")
            {
                return Format.Ararat;
            }
            if (PdfHelper.ContainsSequential(path,
                "ДАТА", "ОПЕРАЦИИ", "НАИМЕНОВАНИЕ", "ОПЕРАЦИИ", "ШИФР", "СУММА", "ОПЕРАЦИИ", "ОСТАТОК", "НА", "СЧЁТЕ"))
            {
                return Format.SberVklad;
            }
            if (PdfHelper.ContainsSequential(path,
                "ДАТА", "ОПЕРАЦИИ", "(МСК)", "КАТЕГОРИЯ", "СУММА", "В", "ВАЛЮТЕ", "СЧЁТА", "ОСТАТОК", "ПО", "СЧЁТУ"))
            {
                return Format.Sber;
            }
            if (PdfHelper.ContainsSequential(path,
                "ДАТА", "ОПЕРАЦИИ", "(МСК)", "КАТЕГОРИЯ", "СУММА", "В", "ВАЛЮТЕ", "СЧЁТА"))
            {
                return Format.Sber;
            }
            if (PdfHelper.ContainsSequential(path,
                "Дата", "и", "время", "Дата", "обработки", "Сумма", "Сумма", "операции", "банком", "операции", "списания",
                "Описание"))
            {
                return Format.Tinkoff;
            }

            return Format.Invalid;
        }

        private static Format DetectExcelFormat(string path)
        {
            // A2 = 'Name Surname/Title :' -> deniz
            if (ExcelHelper.GetString(path, 1, 0) == "Name Surname/Title :")
            {
                return Format.Deniz;
            }
            // A7 = 'Account Number' -> ziraat
            if (ExcelHelper.GetString(path, 6, 0) == "Account Number")
            {
                return Format.Ziraat;
            }

            return Format.Invalid;
        }
    }
}
