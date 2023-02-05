using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using PdfExtractor.Helpers;
using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    // excel document
    // data start from A13
    // columns: date, invoice no, explanation, transaction amount, balance
    public class ZiraatParser : IParser
    {
        private const string Account = "ziraat-maha";

        public IEnumerable<Operation> Parse(string path)
        {
            using var doc = SpreadsheetDocument.Open(path, false);

            var workbookPart = doc.WorkbookPart;
            var sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            var strings = workbookPart.SharedStringTablePart?.SharedStringTable;
            foreach (var sheet in sheets.Cast<Sheet>())
            {
                var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);

                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                foreach (var row in sheetData.Cast<Row>().Skip(12))
                {
                    var cells = row.Cast<Cell>().ToList();

                    var date = ExcelHelper.GetString(cells[0], strings);
                    if (string.IsNullOrEmpty(date)) break;

                    var explanations = ExcelHelper.GetString(cells[2], strings);
                    var amount = GetNumber(cells[3]);

                    yield return new Operation
                    {
                        DateTime = DateTime.Parse(date),
                        Account = Account,
                        Amount = new Money(amount, "try"),
                        Description = explanations
                    };
                }
            }
        }

        private static double GetNumber(Cell cell)
        {
            Debug.Assert(cell.DataType == CellValues.Number);

            return double.Parse(cell.InnerText.Replace(",", ""));
        }
    }
}
