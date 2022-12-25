using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    // excel document
    // data start from A9
    // columns: date, details, receipt number, amount (try), balance (try)
    public class DenizParser : IParser
    {
        private const string Account = "deniz-maha";

        public IEnumerable<Operation> Parse(string path)
        {
            using var doc = SpreadsheetDocument.Open(path, false);

            var workbookPart = doc.WorkbookPart;
            var sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            foreach (var sheet in sheets.Cast<Sheet>())
            {
                var worksheetPart = (WorksheetPart) workbookPart.GetPartById(sheet.Id);

                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                foreach (var row in sheetData.Cast<Row>())
                {
                    if (row.RowIndex < 9) continue;

                    var cells = row.Cast<Cell>().ToList();
                    Debug.Assert(cells.Count == 5);

                    var date = GetString(cells[0], workbookPart);
                    var details = GetString(cells[1], workbookPart);
                    var amount = GetNumber(cells[3]);

                    yield return new Operation
                    {
                        DateTime = DateTime.Parse(date),
                        Account = Account,
                        Amount = new Money
                        {
                            Currency = "TRY",
                            Value = amount
                        },
                        Description = details
                    };
                }
            }
        }

        private static string GetString(Cell cell, WorkbookPart workbookPart)
        {
            Debug.Assert(cell.DataType == CellValues.SharedString);

            var id = int.Parse(cell.InnerText);
            var item = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
            return item.Text.Text;
        }
        
        private static double GetNumber(Cell cell)
        {
            Debug.Assert(cell.DataType == CellValues.Number);

            return double.Parse(cell.InnerText);
        }
    }
}