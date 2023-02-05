using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ParseFormat = PdfExtractor.Models.Format;
using PdfExtractor.Helpers;

namespace PdfExtractor
{
    public class FormatDetector
    {
        public ParseFormat GetFormat(string path)
        {
            return Path.GetExtension(path) switch
            {
                ".txt" => ParseFormat.RawText,
                ".xlsx" => DetectExcelFormat(path),
                ".json" => ParseFormat.ExpencesApp,
                ".pdf" => DetectPdfFormat(path),
                _ => ParseFormat.Invalid,
            };
        }

        private static ParseFormat DetectPdfFormat(string path)
        {
            using var document = PdfDocument.Open(path);

            // fixme: performance
            var allWords = document.GetPages().SelectMany(p => p.GetWords()).ToList();

            if (allWords[0].Text == "ԱՐԱՐԱՏԲԱՆԿ")
            {
                return ParseFormat.Ararat;
            }
            if (ContainsSequencial(allWords, "ДАТА", "ОПЕРАЦИИ", "НАИМЕНОВАНИЕ", "ОПЕРАЦИИ",
                                        "ШИФР", "СУММА", "ОПЕРАЦИИ", "ОСТАТОК", "НА", "СЧЁТЕ"))
            {
                return ParseFormat.SberVklad;
            }
            if (ContainsSequencial(allWords, "ДАТА", "ОПЕРАЦИИ", "(МСК)", "КАТЕГОРИЯ",
                                        "СУММА", "В", "ВАЛЮТЕ", "СЧЁТА", "ОСТАТОК", "ПО", "СЧЁТУ"))
            {
                return ParseFormat.Sber;
            }
            if (ContainsSequencial(allWords, "ДАТА", "ОПЕРАЦИИ", "(МСК)", "КАТЕГОРИЯ",
                                        "СУММА", "В", "ВАЛЮТЕ", "СЧЁТА"))
            {
                return ParseFormat.Sber;
            }
            if (ContainsSequencial(allWords, "Дата", "и", "время", "Дата", "обработки", "Сумма", "Сумма",
                                        "операции", "банком", "операции", "списания", "Описание"))
            {
                return ParseFormat.Tinkoff;
            }
            return ParseFormat.Invalid;
        }

        private static bool ContainsSequencial(IReadOnlyList<Word> words, params string[] sequence)
        {
            for (var i = 0; i < words.Count - sequence.Length; i++)
            {
                var equals = true;
                for (var j = 0; j < sequence.Length; j++)
                {
                    if (words[i + j].Text != sequence[j])
                    {
                        equals = false;
                        break;
                    }
                }
                if (equals)
                {
                    return true;
                }
            }

            return false;
        }

        private static ParseFormat DetectExcelFormat(string path)
        {
            using var doc = SpreadsheetDocument.Open(path, false);

            var workbookPart = doc.WorkbookPart;
            Debug.Assert(workbookPart != null);
            Debug.Assert(workbookPart.Workbook != null);
            var sheets = workbookPart.Workbook.GetFirstChild<Sheets>()?.Cast<Sheet>().ToList();
            Debug.Assert(sheets != null);
            if (sheets.Count != 1)
            {
                return ParseFormat.Invalid;
            }

            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheets[0].Id);
            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            Debug.Assert(sheetData != null);
            var strings = workbookPart.SharedStringTablePart?.SharedStringTable;

            // deniz A2 - Name Surname/Title :
            var text = ExcelHelper.GetString(1, 0, sheetData, strings);
            if (text == "Name Surname/Title :")
            {
                return ParseFormat.Deniz;
            }
            // zirat A7 - Account Number
            text = ExcelHelper.GetString(6, 0, sheetData, strings);
            if (text == "Account Number")
            {
                return ParseFormat.Ziraat;
            }

            return ParseFormat.Invalid;
        }
    }
}
