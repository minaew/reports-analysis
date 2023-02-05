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
    // data start from A9
    // columns: date, details, receipt number, amount (try), balance (try)
    public class DenizParser : IParser
    {
        private const string Account = "deniz-maha";

        public IEnumerable<Operation> Parse(string path)
        {
            using var doc = SpreadsheetDocument.Open(path, false);

            var workbookPart = doc.WorkbookPart;
            var strings = workbookPart?.SharedStringTablePart?.SharedStringTable;

            foreach (var sheet in ExcelHelper.GetSheets(doc))
            {
                foreach (var row in ExcelHelper.GetRows(workbookPart, sheet).Skip(8))
                {
                    var cells = row.Cast<Cell>().ToList();
                    Debug.Assert(cells.Count == 5);

                    var date = ExcelHelper.GetString(cells[0], strings);
                    if (date == null)
                    {
                        throw new ParsingException();
                    }
                    var details = ExcelHelper.GetString(cells[1], strings);
                    var amount = ExcelHelper.GetNumber(cells[3]);

                    yield return new Operation
                    {
                        DateTime = DateTime.Parse(date),
                        Account = Account,
                        Amount = new Money(amount, "try"),
                        Description = details
                    };
                }
            }
        }
    }
}