using System.Diagnostics;
using System.Linq;
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
    }
}
