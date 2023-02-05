using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace PdfExtractor.Helpers
{
    internal static class ExcelHelper
    {
        public static string? GetString(Cell cell, SharedStringTable? strings)
        {
            if (cell.DataType == null)
            {
                return null;
            }
            if (cell.DataType == CellValues.String)
            {
                return cell.InnerText;
            }
            if (cell.DataType == CellValues.SharedString)
            {
                Debug.Assert(strings != null);
                var id = int.Parse(cell.InnerText);
                var item = strings.Elements<SharedStringItem>().ElementAt(id);
                return item?.Text?.Text;
            }

            return null;
        }

        public static string? GetString(int rowNumber, int columnNumber, SheetData sheetData, SharedStringTable? strings)
        {
            var row = sheetData.Cast<Row>().ElementAtOrDefault(rowNumber);
            if (row == null) return null;

            var cell = row.Cast<Cell>().ElementAtOrDefault(columnNumber);
            if (cell == null) return null;

            return GetString(cell, strings);
        }

        public static string? GetString(string path, int row, int column)
        {
            using var doc = SpreadsheetDocument.Open(path, false);

            var workbookPart = doc.WorkbookPart;
            if (workbookPart == null) return null;

            var sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            if (sheets == null) return null;

            var sheetList = sheets.Cast<Sheet>().ToList();
            if (sheetList == null) return null;
            if (sheetList.Count != 1) return null;

            var id = sheetList[0].Id?.Value ?? string.Empty;
            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(id);
            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            Debug.Assert(sheetData != null);
            var strings = workbookPart.SharedStringTablePart?.SharedStringTable;

            return GetString(row, column, sheetData, strings);
        }

        public static double GetNumber(Cell cell)
        {
            if (cell.DataType == null) return 0;

            if (cell.DataType == CellValues.Number)
            {
                return double.Parse(cell.InnerText);
            }

            return 0;
        }

        public static IEnumerable<Sheet> GetSheets(SpreadsheetDocument document)
        {
            return document.WorkbookPart
                ?.Workbook?.GetFirstChild<Sheets>()
                ?.Cast<Sheet>()
                ??
                Enumerable.Empty<Sheet>();
        }

        public static IEnumerable<Row> GetRows(WorkbookPart? workbookPart, Sheet sheet)
        {
            if (workbookPart == null) throw new ArgumentNullException(nameof(workbookPart));

            var id = sheet.Id?.Value ?? string.Empty;
            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(id);

            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            return sheetData?.Cast<Row>() ?? Enumerable.Empty<Row>();
        }
    }
}
